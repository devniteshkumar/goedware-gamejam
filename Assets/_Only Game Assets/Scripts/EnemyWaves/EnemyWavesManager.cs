using System.Collections.Generic;
using UnityEngine;

public class EnemyWavesManager : MonoBehaviour
{
    [Header("References")]
    public EnemyWaveSO EnemyWaveSO;

    [Header("Properties")]
    [SerializeField] private Transform spawnCenter;
    public float totalWaveTime;
    public float time;
    public float waveTime;
    public int currentWave = -1;
    public List<EnemyProperties> allEnemyPropertiesInWave = new();

    private void Start()
    {
        FindTotalWaveTime();
        FindCorrectWaveNumber();
    }

    private void Update()
    {
        if (time > totalWaveTime)
        {
            GameManager.Instance.debugMessageTextToShow = "Level Complete!";
            return;
        }

        time += Time.deltaTime;
        waveTime += Time.deltaTime;

        EnemySpawn();
    }

    private void EnemySpawn()
    {
        Wave wave = EnemyWaveSO.waves[currentWave];

        if (waveTime >= wave.WaveDuration)
        {
            FindCorrectWaveNumber();
        }


        for (int i = 0; i < wave.EnemyTypes.Count; i++)
        {
            EnemyType enemyType = wave.EnemyTypes[i];
            EnemyProperties enemyProperties = allEnemyPropertiesInWave[i];

            float waveDuration = wave.WaveDuration;

            if (enemyProperties.amountSpawned >= enemyProperties.enemyType.amountToSpawn) continue;

            if (waveTime >= enemyProperties.spawnStartTime && waveTime <= enemyProperties.spawnEndTime)
            {
                if (waveTime >= enemyProperties.spawnTime * enemyProperties.amountSpawned + enemyProperties.spawnStartTime)
                {
                    Vector2 spawnPos = GetSpawnPosition(enemyProperties.enemyType);
                    Instantiate(enemyProperties.enemyType.EnemyPrefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                    enemyProperties.amountSpawned++;
                }
            }
        }

        Vector2 GetSpawnPosition(EnemyType enemyType)
        {
            float angle = Random.Range(enemyType.spawnStartAngle, enemyType.spawnEndAngle);
            float radius = enemyType.minDistanceFromPlayerToSpawn + Random.Range(-2, 4);

            Vector2 spawnOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            Vector2 center = spawnCenter.transform.position; 

            return center + spawnOffset;
        }

    }



    void FindTotalWaveTime()
    {
        int comparingTime = 0;
        for (int i = 0; i < EnemyWaveSO.waves.Count; i++)
        {
            Wave wave = EnemyWaveSO.waves[i];
            comparingTime += wave.WaveDuration;
        }
        totalWaveTime = comparingTime;
    }


    void FindCorrectWaveNumber()
    {
        int comparingTime = 0;
        for (int i = 0; i < EnemyWaveSO.waves.Count; i++)
        {
            Wave wave = EnemyWaveSO.waves[i];
            comparingTime += wave.WaveDuration;
            if (comparingTime > time)
            {
                if (currentWave != i)
                {
                    currentWave = i;
                    NewWaveSetup(i);
                }
                return;
            }
        }
    }

    private void NewWaveSetup(int i)
    {
        waveTime = 0;
        allEnemyPropertiesInWave.Clear();
        for (int j = 0; j < EnemyWaveSO.waves[i].EnemyTypes.Count; j++)
        {
            EnemyType enemyType = EnemyWaveSO.waves[i].EnemyTypes[j];
            float duration = EnemyWaveSO.waves[i].WaveDuration;
            allEnemyPropertiesInWave.Add(new EnemyProperties(enemyType, duration));

        }
        GameManager.Instance.debugMessageTextToShow = "New Wave Spawning";
    }

    public class EnemyProperties
    {
        public EnemyType enemyType;
        public float spawnRate;
        public float spawnTime;
        public int amountSpawned;
        public float spawnEndTime;
        public float spawnStartTime;

        public EnemyProperties(EnemyType enemyType, float waveDuration)
        {
            this.enemyType = enemyType;
            float actualSpawnTime = (enemyType.spawnEndTime_0to1 - enemyType.spawnStartTime_0to1) * waveDuration;
            spawnRate = enemyType.amountToSpawn / actualSpawnTime;
            spawnTime = 1f / spawnRate;
            amountSpawned = 0;
            spawnStartTime = enemyType.spawnStartTime_0to1 * waveDuration;
            spawnEndTime = enemyType.spawnEndTime_0to1 * waveDuration;
        }
    }
}
