using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 20;
    [SerializeField] private float timeSinceLastHit = 0.5f;
    [SerializeField] private float dissapearSpeed = 2f;

    private AudioSource audio;
    private float timer = 0f;
    private Animator anim;
    private NavMeshAgent nav;
    private bool isAlive;
    private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
    private bool dissapearEnemy = false;
    private int currentHealth;
    private ParticleSystem blood;

    public bool IsAlive
    {
        get { return isAlive; }
    }

    void Awake()
    {
        // Initialize components in Awake to ensure they're ready
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        blood = GetComponentInChildren<ParticleSystem>();
        isAlive = true;
        currentHealth = startingHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        // GameManager handles registration manually
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (dissapearEnemy)
        {
            transform.Translate(-Vector3.up * dissapearSpeed * Time.deltaTime);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if(timer>= timeSinceLastHit && !GameManager.instance.GameOver)
        {
            if (other.tag == "PlayerWeapon")
            {
                takeHit();
                timer = 0f;
            }
        }
    }

    void takeHit()
    {
        if (currentHealth > 0)
        {
            if (audio != null && audio.clip != null)
            {
                audio.PlayOneShot(audio.clip);
            }
            if (anim != null)
            {
                anim.CrossFade("Hurt", 0.1f);
            }
            currentHealth -= 10;
            if (blood != null)
            {
                blood.Play();
            }
        }
        if (currentHealth <= 0)
        {
            isAlive = false;
            KillEnemy();
        }
    }

    void KillEnemy()
    {
        GameManager.instance.KilledEnemy(this);
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }
        if (nav != null)
        {
            nav.enabled = false;
        }
        if (anim != null)
        {
            anim.SetTrigger("EnemyDie");
        }
        if (rigidBody != null)
        {
            rigidBody.isKinematic = true;
        }
        if (blood != null)
        {
            blood.Play();
        }

        StartCoroutine(removeEnemy());
    }

    IEnumerator removeEnemy()
    {
        yield return new WaitForSeconds(4f);
        dissapearEnemy = true;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

    }
}
