using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float enemySpawnRate = 5.0f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyContainer;

    [SerializeField] private float minPowerupSpawnRate;
    [SerializeField] private float maxPowerupSpawnRate;
    [SerializeField] private List<GameObject> powerups;

    [SerializeField] private float topSpawnPosition;
    [SerializeField] private float spawnXRange;
    private bool stopSpawning;
    // Start is called before the first frame update
    void Start()
    {
        stopSpawning = true;

    }



    IEnumerator EnemySpawnRoutine()
    {
        GameObject newEnemy;
        while (!stopSpawning)
        {
            newEnemy = Instantiate(enemyPrefab, RandomSpawnPosition(), Quaternion.identity);
            newEnemy.transform.SetParent(enemyContainer.transform);
            yield return new WaitForSeconds(enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while(!stopSpawning)
        {
            Instantiate(powerups[Random.Range(0, powerups.Count)], RandomSpawnPosition() , Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(minPowerupSpawnRate,maxPowerupSpawnRate));
        }
    }
    public Vector3 RandomSpawnPosition()
    {
        float xPos = Random.Range(-spawnXRange, spawnXRange);
        return new Vector3(xPos, topSpawnPosition, 0);
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }

    public void OnAsteroidDestroy()
    {
        stopSpawning = false;
        StartCoroutine(EnemySpawnRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

}
