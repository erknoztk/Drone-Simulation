using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject pauseMenu; // UI panelini buraya sürükle (Inspector'dan)
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTrainingArena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TraningArena"); // dikkat: Traning yazýmý
    }

    public void LoadBombArena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BombArena");
    }

    public void LoadKamikazeArena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("KamikazeArena");
    }
}
