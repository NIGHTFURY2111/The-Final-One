
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject player;

    private void Awake()
    {
        player.TryGetComponent(out Player_Entity playerEntity);
        playerEntity.OnRespawnTrigger += respawnPlayer;
    }
    public void RestartLevel() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Player DED");
    }

    public void respawnPlayer(GameObject player, GameObject respawnPoint)
    {
        player.transform.position = respawnPoint.transform.position;
        player.transform.rotation = respawnPoint.transform.rotation;
        Debug.Log("Player Respawned at " + respawnPoint.name);
    }

}
