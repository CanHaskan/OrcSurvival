using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;





public class EnemyMove : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent nav;
    private Animator anim;
    private EnemyHealth enemyHealth;


    void Awake()
    {
        // Initialize components in Awake
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.Player != null)
        {
            player = GameManager.instance.Player.transform;
        }
        else
        {
            Debug.LogError("GameManager.instance or Player is null in EnemyMove!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && nav != null && enemyHealth != null && !GameManager.instance.GameOver && enemyHealth.IsAlive)
        {
            nav.SetDestination(player.position);
        }
        else if (enemyHealth != null && !enemyHealth.IsAlive)
        {
            if (nav != null)
            {
                nav.enabled = false;
            }
        }
        else if (GameManager.instance.GameOver)
        {
            if (nav != null)
            {
                nav.enabled = false;
            }
            if (anim != null)
            {
                anim.Play("Idle");
            }
        }
    }
}
