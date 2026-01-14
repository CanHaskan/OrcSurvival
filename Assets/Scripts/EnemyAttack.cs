using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    [SerializeField] private float range = 3f;
    [SerializeField] private float timeBetweenAttacks = 1f;

    private Animator anim;
    private GameObject player;
    private bool playerInRange;
    private BoxCollider[] weaponColliders;
    private EnemyHealth enemyHealth;

    void Awake()
    {
        // Initialize components in Awake
        enemyHealth = GetComponent<EnemyHealth>();
        weaponColliders = GetComponentsInChildren<BoxCollider>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.Player != null)
        {
            player = GameManager.instance.Player;
        }
        else
        {
            Debug.LogError("GameManager.instance or Player is null in EnemyAttack!");
        }
        StartCoroutine(attack());
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && enemyHealth != null && enemyHealth.IsAlive)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < range)
            {
                playerInRange = true;
            }
            else
            {
                playerInRange = false;
            }
        }
        else
        {
            playerInRange = false;
        }
    }

    IEnumerator attack()
    {
        if (enemyHealth == null || anim == null || player == null)
        {
            Debug.LogError("EnemyAttack: enemyHealth, anim, or player is null!");
            yield break;
        }

        while (enemyHealth.IsAlive && !GameManager.instance.GameOver)
        {
            if (playerInRange && anim != null)
            {
                anim.CrossFade("Attack", 0.1f);
            }
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    public void EnemyBeginAttack()
    {
        foreach(var weapon in weaponColliders)
        {
            weapon.enabled = true;
        }

    }
    public void EnemyEndAttack()
    {
        foreach (var weapon in weaponColliders)
        {
            weapon.enabled = false;
        }

    }
}
