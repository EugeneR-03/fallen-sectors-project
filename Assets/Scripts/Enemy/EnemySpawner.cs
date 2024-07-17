using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    private int _difficultyLevel;
    private float _difficultyHealthMultiplier;
    private float _waveSpawnRate;
    private float _waveGap;
    private int _waveEnemiesCount;
    private float _spawnRate;
    private float _spawnDistance;
    private float _maxEnemies;
    private float _currentEnemies;
    private float _currentCurrencyFromEnemies;
    private float _currencyMultiplier;
    [SerializeField] private GameObject _enemyOrdinary;
    [SerializeField] private GameObject _enemyFast;
    [SerializeField] private GameObject _enemyArmored;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private UIController _uIController;


    // Start is called before the first frame update
    void Start()
    {
        _difficultyLevel = 0;
        _difficultyHealthMultiplier = 1.0f;
        _waveSpawnRate = 1f;
        _waveGap = 4 * 60f;
        _waveEnemiesCount = 50;
        _spawnRate = 2.5f;
        _spawnDistance = 10f;
        _maxEnemies = 1000f;
        _currentEnemies = 0f;
        _currentCurrencyFromEnemies = 0f;
        _currencyMultiplier = 1f;
        StartCoroutine(SpawnOrdinaryEnemiesRoutine(_spawnRate));
        StartCoroutine(SpawnFastEnemiesRoutine(_spawnRate));
        StartCoroutine(SpawnArmoredEnemiesRoutine(_spawnRate));
        StartCoroutine(SpawnWavesOfEnemiesRoutine(_waveGap, _waveEnemiesCount, _waveSpawnRate));
        StartCoroutine(IncreaseLevelOfDifficultyRoutine());
    }

    public Vector2 CalculateSpawnPosition()
    {
        Vector2 playerPosition = _playerTransform.position;
        Vector2 spawnPosition = playerPosition + UnityEngine.Random.insideUnitCircle.normalized * _spawnDistance;
        return spawnPosition;
    }

    private void SpawnEnemy(GameObject enemy)
    {
        if (_currentEnemies >= _maxEnemies)
        {
            return;
        }
        Vector3 spawnPosition = CalculateSpawnPosition();
        GameObject enemyClone = Instantiate(enemy, spawnPosition, Quaternion.identity);
        EnemyController enemyController = enemyClone.GetComponent<EnemyController>();
        enemyController.SetHealthMultiplier(_difficultyHealthMultiplier);
        _currentEnemies++;
    }

    private IEnumerator SpawnOrdinaryEnemiesRoutine(float spawnRate)
    {
        while (true)
        {
            SpawnEnemy(_enemyOrdinary);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private IEnumerator SpawnFastEnemiesRoutine(float spawnRate)
    {
        yield return new WaitForSeconds(2*60);
        while (true)
        {
            SpawnEnemy(_enemyFast);
            yield return new WaitForSeconds(spawnRate*5);
        }
    }

    private IEnumerator SpawnArmoredEnemiesRoutine(float spawnRate)
    {
        yield return new WaitForSeconds(4*60);
        while (true)
        {
            SpawnEnemy(_enemyArmored);
            yield return new WaitForSeconds(spawnRate*10);
        }
    }

    private IEnumerator SpawnWaveOfEnemiesRoutine(int numberOfEnemies, float waveSpawnRate)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy(_enemyOrdinary);
            if (i % 5 == 0)
            {
                SpawnEnemy(_enemyFast);
            }
            if (i % 10 == 0)
            {
                SpawnEnemy(_enemyArmored);
            }
            yield return new WaitForSeconds(waveSpawnRate);
        }
    }

    private IEnumerator SpawnWavesOfEnemiesRoutine(float waveGap, int waveEnemiesCount, float waveSpawnRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(waveGap);
            StartCoroutine(SpawnWaveOfEnemiesRoutine(waveEnemiesCount, waveSpawnRate));
            yield return new WaitForSeconds(60);
        }
    }

    public void AddAllCurrencyFromEnemiesToPlayersCurrency()
    {
        UserDataController.Instance.AddCurrencyForConstantUpgrades((int)Math.Floor(_currentCurrencyFromEnemies));
    }

    private void AddToCurrentCurrencyFromEnemies(float amount)
    {
        _currentCurrencyFromEnemies += amount * _currencyMultiplier;
    }

    public void KillEnemy()
    {
        _currentEnemies--;
        _uIController.UpdateKillsCount(1);
    }

    public void KillEnemyAndAddCurrency(float amount)
    {
        KillEnemy();
        AddToCurrentCurrencyFromEnemies(amount);
    }

    public void IncreaseLevelOfDifficulty()
    {
        _difficultyHealthMultiplier *= 1.05f;
        _waveSpawnRate *= 0.8f;
        _waveEnemiesCount += 5;
        _spawnRate *= 0.95f;
        _currencyMultiplier *= 1.1f;
        _difficultyLevel++;
    }

    private IEnumerator IncreaseLevelOfDifficultyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            IncreaseLevelOfDifficulty();
        }
    }

    public static GameObject FindClosestEnemy(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;
        float maxDistance = 10f;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(position, enemy.transform.position);
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (distance < closestDistance && 
                distance < maxDistance && 
                !enemyController.IsDead)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
