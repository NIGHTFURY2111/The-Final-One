using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] GameObject pauseMenuUI;

    [SerializeField] SO_InputReader InputReader;

    public static bool isPaused { get; private set; } = false;

    public void OnCloseMenu(InputValue input) => OnOpenMenu(input);
    public void OnOpenMenu(InputValue input)
    {
        if (input.isPressed)
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
    }
    void PauseGame()
    {
        InputReader.DisableInput();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        isPaused = true;
       
    }
    public void ResumeGame()
    {
     
        InputReader.EnableInput();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }
    public void BackToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        InputReader.EnableInput();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
