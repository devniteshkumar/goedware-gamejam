using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveSO", menuName = "Scriptable Objects/EnemyWaveSO")]
public class EnemyWaveSO : ScriptableObject
{
    public List<Wave> waves;
}

[System.Serializable]
public class Wave
{
    public int WaveDuration;
    public List<EnemyType> EnemyTypes;
    public List<Resource> resourcesGivenAtEnd;
    public int noOfResourcesToGiveFromList;
    public int timeAfterNextWaveStart;
}

[System.Serializable]
public class EnemyType
{
    public GameObject EnemyPrefab;
    public int amountToSpawn;
    public float minDistanceFromPlayerToSpawn;
    [Tooltip("0 for all to be spawned at Start of Wave, 0.5 for all to be spawned at half time of Wave and 1 for all to be spawned at End")] public float spawnStartTime_0to1;
    [Tooltip("Time till all Enemies should be spawned")] public float spawnEndTime_0to1;
    public int spawnStartAngle;
    public int spawnEndAngle;
}
