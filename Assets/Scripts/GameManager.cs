using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq.Expressions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
    
{
    public static GameManager instance = null;
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] powerUpSpawns;
    [SerializeField] GameObject tanker;
    [SerializeField] GameObject ranger;
    [SerializeField] GameObject soldier;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject healthPowerUp;
    [SerializeField] GameObject speedPowerUp;
    [SerializeField] Text levelText;
    [SerializeField] int maxPowerUps = 4;
    [SerializeField] Text endGameText;
    [SerializeField] int finalLevel=20;






    private bool gameOver = false;
    private int currentLevel;
    private float generatedSpawnTime = 1;
    private float currentSpawnTime = 0;
    private float powerUpSpawnTime = 10; // Test için 10 saniye, normalde 60
    private float currentPowerUpSpawnTime = 0;

    private GameObject newEnemy;
    private int powerups = 0;
    private GameObject newPowerUp;


    private List<EnemyHealth> enemies = new List<EnemyHealth>();
    private List<EnemyHealth> killedEnemies = new List<EnemyHealth>();

    public void RegisterEnemy(EnemyHealth enemy)
    {
        enemies.Add(enemy);
    }

    public void KilledEnemy(EnemyHealth enemy)
    {
        killedEnemies.Add(enemy);
    }

    public void RegisterPowerUp()
    {
        powerups++;
    }

    public bool GameOver
    {
        get
        {
            return gameOver;
        }
    }

    public GameObject Player
    {
        get
        {
            return player;
        }
    }

    public GameObject Arrow
    {
        get { return arrow; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        enemies.Clear();
        killedEnemies.Clear();
        gameOver = false;
        currentLevel = 1;
        currentSpawnTime = 0;
        currentPowerUpSpawnTime = 0;

        endGameText.GetComponent<Text>().enabled = false;
        StartCoroutine(spawn());
        StartCoroutine(powerUpSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
        currentSpawnTime += Time.deltaTime;
        currentPowerUpSpawnTime += Time.deltaTime;
    }
    public void PlayerHit(int currentHP)
    {
        if (currentHP > 0)
        {
            gameOver = false;
        }
        else
        {
            gameOver = true;
            StartCoroutine(endGame("Defeat!"));
        }
    }

    IEnumerator spawn()
    {
        while (!gameOver)
        {
            if (currentSpawnTime > generatedSpawnTime)
            {
                currentSpawnTime = 0;

                if (enemies.Count < currentLevel)
                {
                    if (spawnPoints == null || spawnPoints.Length == 0)
                    {
                        Debug.LogError("Spawn Points array is null or empty!");
                        yield return null;
                        continue;
                    }

                    int randomNumber = Random.Range(0, spawnPoints.Length);
                    GameObject spawnLocation = spawnPoints[randomNumber];

                    if (spawnLocation == null)
                    {
                        Debug.LogError("Spawn location is null!");
                        yield return null;
                        continue;
                    }

                    int randomEnemy = Random.Range(0, 3);
                    if (randomEnemy == 0)
                    {
                        if (soldier == null)
                        {
                            Debug.LogError("Soldier prefab is not assigned!");
                            yield return null;
                            continue;
                        }
                        newEnemy = Instantiate(soldier) as GameObject;
                    }
                    else if (randomEnemy == 1)
                    {
                        if (ranger == null)
                        {
                            Debug.LogError("Ranger prefab is not assigned!");
                            yield return null;
                            continue;
                        }
                        newEnemy = Instantiate(ranger) as GameObject;
                    }
                    else if (randomEnemy == 2)
                    {
                        if (tanker == null)
                        {
                            Debug.LogError("Tanker prefab is not assigned!");
                            yield return null;
                            continue;
                        }
                        newEnemy = Instantiate(tanker) as GameObject;
                    }

                    if (newEnemy != null)
                    {
                        newEnemy.transform.position = spawnLocation.transform.position;
                        newEnemy.SetActive(true); // Ensure enemy is active
                        
                        // Manually register enemy if EnemyHealth script exists
                        EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            RegisterEnemy(enemyHealth);
                        }
                        else
                        {
                            Debug.LogWarning($"EnemyHealth component not found on {newEnemy.name}!");
                        }
                    }
                }

                if (killedEnemies.Count == currentLevel && currentLevel != finalLevel)
                {
                    enemies.Clear();
                    killedEnemies.Clear();

                    yield return new WaitForSeconds(3f);
                    currentLevel++;
                    if (levelText != null)
                    {
                        levelText.text = "Level " + currentLevel;
                    }
                }

                if (killedEnemies.Count == finalLevel)
                {
                    StartCoroutine(endGame("Victory!"));
                    break;
                }
            }

            yield return null;
        }
    }

    IEnumerator powerUpSpawn()
    {
        while (!gameOver)
        {
            if (currentPowerUpSpawnTime > powerUpSpawnTime)
            {
                currentPowerUpSpawnTime = 0;

                if (powerups < maxPowerUps)
                {
                    if (powerUpSpawns == null || powerUpSpawns.Length == 0)
                    {
                        Debug.LogError("Power Up Spawn Points array is null or empty!");
                        yield return null;
                        continue;
                    }

                    int randomNumber = Random.Range(0, powerUpSpawns.Length);
                    GameObject powerupspawnLocation = powerUpSpawns[randomNumber];

                    if (powerupspawnLocation == null)
                    {
                        Debug.LogError("Power Up spawn location is null!");
                        yield return null;
                        continue;
                    }

                    int randomPowerUp = Random.Range(0, 2);
                    if (randomPowerUp == 0)
                    {
                        if (healthPowerUp == null)
                        {
                            Debug.LogError("Health PowerUp prefab is not assigned!");
                            yield return null;
                            continue;
                        }
                        newPowerUp = Instantiate(healthPowerUp) as GameObject;
                    }
                    else if (randomPowerUp == 1)
                    {
                        if (speedPowerUp == null)
                        {
                            Debug.LogError("Speed PowerUp prefab is not assigned!");
                            yield return null;
                            continue;
                        }
                        newPowerUp = Instantiate(speedPowerUp) as GameObject;
                    }

                    if (newPowerUp != null)
                    {
                        newPowerUp.transform.position = powerupspawnLocation.transform.position;
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator endGame(string outcome)
    {
        endGameText.text = outcome;
        endGameText.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("GameMenu");
    }
}
