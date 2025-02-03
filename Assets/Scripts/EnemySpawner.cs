using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public BoxCollider spawnAreaCollider;
    public int initialSpawnCount = 0; 
    public float respawnDelay = 5f; 
    public int maxEnemies = 10; 
    public float activationDistance = 20f; 

    public int currentEnemyCount = 0; 
    private bool isActive = false; 
    private Transform playerTransform; 

    public static EnemySpawner Instance; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        if (spawnAreaCollider == null)
        {
            return;
        }

        playerTransform = PlayerController.Instance.transform;

        StartCoroutine(ManageSpawnerActivation());
    }

    IEnumerator ManageSpawnerActivation()
    {
        while (true)
        {
            float distanceToSpawnArea = Vector3.Distance(playerTransform.position, spawnAreaCollider.transform.position);

            if (PlayerController.Instance.onCombat && distanceToSpawnArea > activationDistance)
            {
                if (!isActive)
                {
                    isActive = true;
                    SpawnEnemies(initialSpawnCount);
                    StartCoroutine(ManageEnemyRespawn());
                }
            }
            else if (!PlayerController.Instance.onCombat)
            {
                if (isActive)
                {
                    isActive = false;
                    StopCoroutine(ManageEnemyRespawn());
                    EliminateAllEnemies();
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ManageEnemyRespawn()
    {
        while (isActive)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(respawnDelay);
        }
    }

    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab && spawnAreaCollider)
        {
            Vector3 spawnPosition = GetRandomPositionInCollider(spawnAreaCollider);
            
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            currentEnemyCount++;
        }
    }

    Vector3 GetRandomPositionInCollider(BoxCollider collider)
    {
        if (collider.size == Vector3.zero)
        {
            return collider.transform.position;
        }

        Vector3 center = collider.center;
        Vector3 size = collider.size;

        Vector3 randomPoint = new Vector3(
            Random.Range(center.x - size.x / 2, center.x + size.x / 2),
            Random.Range(center.y - size.y / 2, center.y + size.y / 2),
            Random.Range(center.z - size.z / 2, center.z + size.z / 2)
        );

        return collider.transform.TransformPoint(randomPoint);
    }

    public void EnemyDied()
    {
        currentEnemyCount--;

        if (currentEnemyCount <= 0 && PlayerController.Instance.onCombat)
        {
            SpawnEnemies(initialSpawnCount);
        }
    }

    public void EliminateAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().DieNoDrop();
        }
        currentEnemyCount = 0;
    }
}
