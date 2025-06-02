using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using System;

public class DataTester : MonoBehaviour
{
    public string selectedPort = "None";
    private SerialPort serialPort;
    public int baudRate = 115200;

    public Queue<int> throttleHistory = new Queue<int>();
    public Queue<int> rollHistory = new Queue<int>();
    public Queue<int> pitchHistory = new Queue<int>();
    public Queue<int> yawHistory = new Queue<int>();
    public int maxHistory = 200;

    public int throttle, roll, pitch, yaw;
    public int rot1, rot2;
    public int sw1, sw2, sw3, sw4;

    void Start()
    {
        DetectAndConnectPort();
    }

    void DetectAndConnectPort()
    {
        foreach (string port in SerialPort.GetPortNames())
        {
            try
            {
                var testPort = new SerialPort(port, baudRate) { ReadTimeout = 250, ReadBufferSize = 4096 };
                testPort.Open();
                testPort.Write("ping");

                selectedPort = port;
                serialPort = testPort;
                Debug.Log("Connected to port: " + selectedPort);
                return;
            }
            catch
            {
                continue;
            }
        }
        Debug.LogError("No valid Arduino COM port found.");
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            try
            {
                string data = serialPort.ReadExisting();
                ProcessSerialData(data);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Serial read error: " + ex.Message);
            }
        }
    }

    void ProcessSerialData(string data)
    {
        string[] parts = data.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (part.StartsWith("T:")) throttle = Parse(part, 2);
            else if (part.StartsWith("R:")) roll = Parse(part, 2);
            else if (part.StartsWith("P:")) pitch = Parse(part, 2);
            else if (part.StartsWith("Y:")) yaw = Parse(part, 2);
            else if (part.StartsWith("ROT1:")) rot1 = Parse(part, 5);
            else if (part.StartsWith("ROT2:")) rot2 = Parse(part, 5);
            else if (part.StartsWith("SW1:")) sw1 = Parse(part, 4);
            else if (part.StartsWith("SW2:")) sw2 = Parse(part, 4);
            else if (part.StartsWith("SW3:")) sw3 = Parse(part, 4);
            else if (part.StartsWith("SW4:")) sw4 = Parse(part, 4);
        }

        // Veri güncellendiyse sadece o zaman grafiðe ekle
        EnqueueWithLimit(throttleHistory, throttle);
        EnqueueWithLimit(rollHistory, roll);
        EnqueueWithLimit(pitchHistory, pitch);
        EnqueueWithLimit(yawHistory, yaw);
    }

    int Parse(string input, int skip)
    {
        if (int.TryParse(input.Substring(skip), out int result))
            return result;
        return 0;
    }

    void EnqueueWithLimit(Queue<int> queue, int value)
    {
        queue.Enqueue(value);
        if (queue.Count > maxHistory)
            queue.Dequeue();
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
