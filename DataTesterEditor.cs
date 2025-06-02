using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DataTester))]
public class DataTesterEditor : Editor
{
    private const int MaxPointsToDraw = 150; // daha az veri göster, daha akıcı olur

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataTester dt = (DataTester)target;

        GUILayout.Space(20);
        GUILayout.Label("Channel Graph (Throttle, Roll, Pitch, Yaw)", EditorStyles.boldLabel);

        DrawMultiGraph(new Dictionary<string, (Queue<int>, Color)>
        {
            { "Throttle", (dt.throttleHistory, Color.green) },
            { "Roll",     (dt.rollHistory,     Color.cyan) },
            { "Pitch",    (dt.pitchHistory,    Color.magenta) },
            { "Yaw",      (dt.yawHistory,      Color.yellow) },
        }, 0, 2000, height: 200);

        // ✅ Sürekli kendini yenile
        Repaint();
    }

    void DrawMultiGraph(Dictionary<string, (Queue<int> values, Color color)> channels, int min, int max, int height = 150)
    {
        Rect rect = GUILayoutUtility.GetRect(600, height);
        EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));

        Handles.BeginGUI();

        foreach (var pair in channels)
        {
            var values = pair.Value.values;
            var color = pair.Value.color;

            if (values == null || values.Count < 2) continue;

            int[] valArray = new int[values.Count];
            values.CopyTo(valArray, 0);

            // Downsample: fazla veri varsa azalt
            int step = Mathf.Max(1, valArray.Length / MaxPointsToDraw);
            int count = valArray.Length / step;
            Vector3[] points = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                int idx = i * step;
                float x = rect.x + (i / (float)(count - 1)) * rect.width;
                float norm = Mathf.InverseLerp(min, max, valArray[idx]);
                float y = rect.yMax - norm * rect.height;
                points[i] = new Vector3(x, y, 0);
            }

            Handles.color = color;
            Handles.DrawAAPolyLine(2f, points); // kalınlık 2
        }

        Handles.EndGUI();
    }
}
