
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    GameObject RespawnPoint;
    private void Awake()
    {
        RespawnPoint = getRespawnPoint();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.HasTag(Enum_Tag.player))
        {
            other.TryGetComponent<Player_Entity>(out Player_Entity player);
            player.UpdateRespawnPoint(RespawnPoint);
        }
    }

    GameObject getRespawnPoint()
    {
        return (transform.childCount > 0)?  transform.GetChild(0).gameObject : 
                                            gameObject;
    }
}