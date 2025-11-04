using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] GameObject pauseMenuUI;

    bool isPaused = false;
    public void PausePressed()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuUI.SetActive(false);
       
    }
    public void ResumeGame()
    {
     
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(true);
    }
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
