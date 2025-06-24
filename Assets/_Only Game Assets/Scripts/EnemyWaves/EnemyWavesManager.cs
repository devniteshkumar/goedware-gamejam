using System.Collections.Generic;
using UnityEngine;

public class EnemyWavesManager : MonoBehaviour
{
    [Header("References")]
    public EnemyWaveSO EnemyWaveSO;

    [Header("Properties")]
    [SerializeField] private Transform spawnCenter;
    public int currentWave = -1;
    public bool LevelDone;
    public bool pauseAll;
    public bool startWaveWaitingTime;
    
    [Header("Data While Running")]
    public List<EnemyProperties> allEnemyPropertiesInWave = new();
    public List<GameObject> allEnemiesInWave = new();


    [Header("Time")]
    public float totalWaveTime;
    public float time;
    public float waveWaitingTime;
    public float waveTime;

    private void Start()
    {
        FindTotalWaveTime();
        FindCorrectWaveNumber();
    }

    private void Update()
    {
        CheckIfEnemiesDied();
        if (allEnemiesInWave.Count <= 0)
        {
            pauseAll = false;
        }

        if (LevelDone || pauseAll) return;
        if (time > totalWaveTime && !LevelDone)
        {
            GameManager.Instance.debugMessageTextToShow = "Level Complete!";
            LevelDone = true;
            return;
        }

        if (startWaveWaitingTime)
        {
            waveWaitingTime -= Time.deltaTime;
            if (waveWaitingTime <= 0)
            {
                startWaveWaitingTime = false;
            }
        }

        time += Time.deltaTime;
        waveTime += Time.deltaTime;

        EnemySpawn();
    }
    private void CheckIfEnemiesDied()
    {
        for (int i = allEnemiesInWave.Count - 1; i >= 0; i--)
        {
            if (allEnemiesInWave[i] == null)
            {
                allEnemiesInWave.RemoveAt(i);
            }
        }
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
                    allEnemiesInWave.Add(Instantiate(enemyProperties.enemyType.EnemyPrefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360))));
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
            comparingTime += wave.timeAfterNextWaveStart;
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
            comparingTime += wave.timeAfterNextWaveStart;
            if (comparingTime > time)
            {
                if (currentWave != i && waveWaitingTime <= 0)
                {
                    CurrentWaveDone();
                    currentWave = i;
                    NewWaveSetup(i);
                }
                else if (allEnemiesInWave.Count <= 0)
                {
                    startWaveWaitingTime = true;
                }
                else
                {
                    pauseAll = true;
                }

                return;
            }
        }
    }

    private void CurrentWaveDone()
    {

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
        waveWaitingTime = EnemyWaveSO.waves[currentWave].timeAfterNextWaveStart;
        GameManager.Instance.debugMessageTextToShow = "New Wave Spawning";
    }

    [System.Serializable]
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
