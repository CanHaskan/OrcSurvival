using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.Player;
        playerController = player.GetComponent<PlayerController>();
        GameManager.instance.RegisterPowerUp();

        
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerController.SpeedPowerUp();
            Destroy(gameObject);
        }
    }
}
