
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   
    public void LevelStart(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void GameQuit()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}
