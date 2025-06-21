using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityConversion", menuName = "Scriptable Objects/AbilityConversion")]
public class AbilityConversionSO : ScriptableObject
{
    [SerializeField] public List<Conversion> Conversions = new();
}

[System.Serializable]
public class Conversion
{
    public ResourceAmount startingResource;
    public ResourceAmount convertedResource;
}

[System.Serializable]
public class ResourceAmount
{
    public ResourceTypes resourceType;
    public float amount;
}