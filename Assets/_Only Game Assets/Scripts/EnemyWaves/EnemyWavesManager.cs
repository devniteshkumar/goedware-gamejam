using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyWavesManager : MonoBehaviour
{
    [Header("References")]
    public EnemyWaveSO EnemyWaveSO;
    public TMP_Text totalWTText;
    public TMP_Text WTText;
    public TMP_Text waveNoText;
    public TMP_Text waitingTimeText;
    public SpecialAbilityManager specialAbilityManager;

    [Header("Give Resource at End")]
    public TMP_Text amountToChooseText;
    public GameObject prefabContainer;
    public GameObject giveResourceButtonPrefab; 
    public Animator animator;
    public List<Resource> buttonResources = new();
    public List<GameObject> resourceButtonObjs = new();
    public int given;
    public int toGive;

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
    public float totalWaveTime_Conatant;
    public float time;
    public float waveWaitingTime;
    public float waveTime;

    private void Start()
    {
        FindTotalWaveTime();
        FindCorrectWaveNumber();
        specialAbilityManager = FindAnyObjectByType<SpecialAbilityManager>();
    }

    private void Update()
    {
        CheckIfEnemiesDied();
        if (allEnemiesInWave.Count <= 0)
        {
            pauseAll = false;
        }

        if (LevelDone || pauseAll) return;
        if (time > totalWaveTime_Conatant && !LevelDone && allEnemiesInWave.Count <= 0)
        {
            SceneController.instance.Complete();
            // SceneManager.LoadScene(GameManager.Instance.levels[++GameManager.Instance.currentScene]);
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
        totalWTText.text = time.ToString();
        WTText.text = waveTime.ToString();
        waveNoText.text = $"Wave: {currentWave}";
        if (startWaveWaitingTime)
            waitingTimeText.text = $"Waiting for Next Wave: {waveWaitingTime}";
        else
            waitingTimeText.text = "";
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


    private void SetResourcePanel()
    {
        given = 0;
        GameManager.Instance.pause = true;
        List<Resource> totalResources = EnemyWaveSO.waves[currentWave].resourcesGivenAtEnd;
        int totalNoofResources = totalResources.Count;
        toGive = EnemyWaveSO.waves[currentWave].noOfResourcesToGiveFromList;

        animator.SetBool("load", true);
        amountToChooseText.text = $"Choose {toGive} Resources For Next Wave";

        for (int i = 0; i < totalNoofResources; i++)
        {
            resourceButtonObjs.Add(Instantiate(giveResourceButtonPrefab, prefabContainer.transform));
            resourceButtonObjs[i].gameObject.SetActive(true);
            buttonResources.Add(totalResources[i]);
            TMP_Text resourceAmountText = resourceButtonObjs[i].GetComponentInChildren<TMP_Text>();
            resourceAmountText.text = $"{buttonResources[i].resourceType} : {buttonResources[i].amount}";
            Resource resource = buttonResources[i];
            resourceButtonObjs[i].GetComponentInChildren<Button>().onClick.AddListener(() => { GiveResource(resource); });
        }
    }

    public void GiveResource(Resource resource)
    {
        if (given >= toGive) return;

        SpecialAbilityManager.GetResource(resource.resourceType).amount += resource.amount;
        specialAbilityManager.SyncHealth();


        given++;
        if (given == toGive)
        {
            ResetGiveResourcePanel();
        }
    }

    private void ResetGiveResourcePanel()
    {
        buttonResources.Clear();
        foreach (var item in resourceButtonObjs)
        {
            Destroy(item);
        }
        resourceButtonObjs.Clear();
        animator.SetBool("load", false);
        GameManager.Instance.pause = false;
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
                    allEnemiesInWave.Add(Instantiate(enemyProperties.enemyType.EnemyPrefab, spawnPos, Quaternion.identity));
                    enemyProperties.amountSpawned++;
                }
            }
        }

        Vector2 GetSpawnPosition(EnemyType enemyType)
        {
            float angle = Random.Range(enemyType.spawnStartAngle, enemyType.spawnEndAngle);
            float radius = enemyType.minDistanceFromPlayerToSpawn + Random.Range(-2, 4);

            spawnCenter = GameObject.FindGameObjectWithTag("Player").transform;
            Vector2 center = spawnCenter.transform.position;
            Vector2 spawnOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;

            if (((center + spawnOffset).x < -22f) || ((center + spawnOffset).x > 15.5f))
            {
                angle = -angle;
                spawnOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            }
            if (((center + spawnOffset).y < -8f) || ((center + spawnOffset).y > 24f))
            {
                angle = 180-angle;
                spawnOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            }

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
        totalWaveTime_Conatant = comparingTime;
        SpecialAbilityManager.GetResource(ResourceTypes.Time).amount += comparingTime;
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
                if (currentWave == i && allEnemiesInWave.Count > 0 && waveTime >= EnemyWaveSO.waves[i].WaveDuration)
                {
                    pauseAll = true;
                }
                
                if (currentWave + 1 == i && waveWaitingTime <= 0 && !pauseAll)
                {
                    CurrentWaveDone();
                    currentWave = i;
                    NewWaveSetup(i);
                }
                if (currentWave == i && allEnemiesInWave.Count <= 0 && waveTime >= EnemyWaveSO.waves[i].WaveDuration)
                {
                    startWaveWaitingTime = true;
                }

                return;
            }
        }
    }

    private void CurrentWaveDone()
    {
        if (currentWave == -1)
        {
            return;
        }

        SetResourcePanel();

    }


    private void NewWaveSetup(int i)
    {
        allEnemyPropertiesInWave.Clear();
        for (int j = 0; j < EnemyWaveSO.waves[i].EnemyTypes.Count; j++)
        {
            EnemyType enemyType = EnemyWaveSO.waves[i].EnemyTypes[j];
            float duration = EnemyWaveSO.waves[i].WaveDuration;
            allEnemyPropertiesInWave.Add(new EnemyProperties(enemyType, duration));
        }
        waveWaitingTime = EnemyWaveSO.waves[currentWave].timeAfterNextWaveStart;
        startWaveWaitingTime = false;
        waveTime = 0;
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
