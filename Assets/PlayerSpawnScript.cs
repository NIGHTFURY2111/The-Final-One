using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnScript : MonoBehaviour
{
    public Transform[] spawnPoints;
    private int m_playerCount;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.position = spawnPoints[m_playerCount % spawnPoints.Length].position;
        m_playerCount++;
    }
}
