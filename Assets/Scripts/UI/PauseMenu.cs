using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool Paused = false;

    [Header("Menus")]
    [SerializeField] private GameObject PauseMenuCanvas;
    [SerializeField] private GameObject OptionsMenuCanvas;
    [SerializeField] public GameObject GameOverCanvas;
    [SerializeField] public GameObject GameWinCanvas;


    // Start is called before the first frame update
    void Start()
    {
        PauseMenuCanvas.SetActive(false); // Ensure the pause menu canvas is not active when the scene starts
        OptionsMenuCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
        GameWinCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false; // Explicitly set Paused to false
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                Play();
            }
            else if (Time.timeScale > 0)
            {
                Stop();
            }
        }
    }

    void Stop()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    public void ResumeButton()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DisplayGameOverScreen()
    {
        GameOverCanvas.SetActive(true);
    }

    public void DisplayGameWinScreen()
    {
        GameWinCanvas.SetActive(true);
    }
}