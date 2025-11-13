
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player_Entity player;

    private void Start()
    {
        //Player_Entity instance =  Instantiate(player, spawnLocation.position, spawnLocation.rotation);
        player.OnRespawnTrigger.AddListener(respawnPlayer);
        player.OnDeathTrigger.AddListener( RestartLevel);
    }
    public void RestartLevel() 
    {
        Debug.Log("Player DED");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void respawnPlayer()
    {
        player.respawnPlayer();
    }

}
