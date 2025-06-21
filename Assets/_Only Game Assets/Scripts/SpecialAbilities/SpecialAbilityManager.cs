using System.Collections.Generic;
using UnityEngine;

public class SpecialAbilityManager : MonoBehaviour
{
    public List<Resource> AllResources = new();
    [Header("testing...")]
    public ResourceTypes from;
    public ResourceTypes to;
    public float amount;

    [Header("References")]
    public AbilityConversionSO AbilityConversionSO;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && ConvertSpecialAbility(from, amount, to))
        {
            GameManager.Instance.debugMessageTextToShow = "Converted";
        }
    }

    public bool ConvertSpecialAbility(ResourceTypes from_resource, float amount, ResourceTypes to_resource)
    {
        Resource resourceToChange = GetResource(from_resource);
        if (resourceToChange.amount < amount)
        {
            GameManager.Instance.debugMessageTextToShow = "You don't have this much amount to convert";
            return false;
        }

        return Convert(resourceToChange);
        

        bool Convert(Resource resource)
        {
            bool converted = false;
            foreach (Conversion conversion in AbilityConversionSO.Conversions)
            {
                if (conversion.startingResource.resourceType != resource.resourceType)
                    continue;
                if (conversion.convertedResource.resourceType != to_resource)
                    continue;

                resource.amount -= amount;

                float finalAmount = conversion.convertedResource.amount / conversion.startingResource.amount * amount;
                Resource resourceToAdd = GetResource(to_resource);
                resourceToAdd.amount += finalAmount;
                converted = true;
            }

            if (!converted)
            {
                GameManager.Instance.debugMessageTextToShow = "Conversion is not Defined!";
            }

            return converted;
        }
    }



    public Resource GetResource(ResourceTypes resourceType)
    {
        foreach (Resource r in AllResources)
        {
            if (r.resourceType != resourceType)
                continue;

            return r;
        }


        GameManager.Instance.debugMessageTextToShow = "Resource is not there / Can't Find";
        return null;
    }

}

public enum ResourceTypes
{
    Time,
    MovementSpeed,
    AttackDamage,
    AttackingRadius,
    Health,
    TimeFreeze,
}

[System.Serializable]
public class Resource
{
    public ResourceTypes resourceType;
    public float amount;

    public Resource(ResourceTypes resourceType, float amount)
    {
        this.resourceType = resourceType;
        this.amount = amount;
    }
}