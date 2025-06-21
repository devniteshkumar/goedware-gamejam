using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAbilityManager : MonoBehaviour
{
    public List<Resource> AllResources = new();


    [Header("Properties")]
    public ResourceTypes from;
    public ResourceTypes to;
    public float amount;
    public bool convert;

    [Header("testing...")]

    [Header("References")]
    public AbilityConversionSO AbilityConversionSO;
    public TMP_Dropdown from_Dropdown;
    public TMP_Dropdown to_Dropdown;
    public Animator UIAnimator;

    private void Start()
    {
        InitializeFromDropdowns();
        from_Dropdown.onValueChanged.AddListener(OnFromValueChanged);
        to_Dropdown.onValueChanged.AddListener(OnToValueChanged);
    }

    private void InitializeFromDropdowns()
    {
        List<string> fromOptionLabels = new ();

        foreach (Conversion conversion in AbilityConversionSO.Conversions)
        {
            string val = conversion.startingResource.resourceType.ToString();
            if (!fromOptionLabels.Contains(val))
                fromOptionLabels.Add(val);
        }

        from_Dropdown.ClearOptions();
        from_Dropdown.AddOptions(fromOptionLabels);
        from = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), from_Dropdown.options[0].text);

        InitializeToDropdown();
    }

    private void InitializeToDropdown()
    {
        List<string> toOptionLabels = new();

        foreach (Conversion conversion in AbilityConversionSO.Conversions)
        {
            string val = conversion.convertedResource.resourceType.ToString();
            if (from == conversion.startingResource.resourceType && !toOptionLabels.Contains(val))
            {
                toOptionLabels.Add(val);
            }
        }

        to_Dropdown.ClearOptions();
        to_Dropdown.AddOptions(toOptionLabels);
        to = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), to_Dropdown.options[0].text);
    }


    private void OnFromValueChanged(int value)
    {
        from = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), from_Dropdown.options[value].text);
        InitializeToDropdown();
    }

    private void OnToValueChanged(int value)
    {
        to = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), to_Dropdown.options[value].text);
    }

    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter) || convert)
        {
            convert = false;
            if (ConvertSpecialAbility(from, amount, to))
            {
                GameManager.Instance.debugMessageTextToShow = "Converted";
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            UIAnimator.SetBool("Load", true);
        }
    }

    public void UnLoadUI()
    {
        UIAnimator.SetBool("Load", false);
    }

    public void DoConversion()
    {
        if (ConvertSpecialAbility(from, amount, to))
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