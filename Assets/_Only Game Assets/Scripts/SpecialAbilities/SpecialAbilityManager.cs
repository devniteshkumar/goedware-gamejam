using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialAbilityManager : MonoBehaviour
{
    public List<Resource> AllResources = new();


    [Header("Properties")]
    public ResourceTypes from;
    public ResourceTypes to;
    public float fromAmount;
    public float toAmount;
    public bool convert;
    public List<RectTransform> resourceContainers = new();

    [Header("testing...")]

    [Header("UI References")]
    public TMP_Dropdown from_Dropdown;
    public TMP_Dropdown to_Dropdown;
    public TMP_InputField amountInput;
    public TMP_Text toAmountText;
    public RectTransform parentOfResourceContainer;
    public RectTransform resourceContainerPrefab;


    [Header("References")]
    public AbilityConversionSO AbilityConversionSO;
    public Animator UIAnimator;
    public InputActionAsset inputActions;
    private InputAction convertAction;
    private InputAction toggleUIAction;

    private void Start()
    {
        if (from_Dropdown != null && to_Dropdown != null && amountInput != null)
        {
            InitializeFromDropdowns();
            from_Dropdown.onValueChanged.AddListener(OnFromValueChanged);
            to_Dropdown.onValueChanged.AddListener(OnToValueChanged);
            amountInput.onEndEdit.AddListener(OnAmountEndEdit);
            InitializeResourceContainers();
        }
    }

    private void InitializeResourceContainers()
    {
        foreach (Resource resource in AllResources)
        {
            RectTransform resourceContainer = Instantiate(resourceContainerPrefab, parentOfResourceContainer);
            resourceContainer.gameObject.SetActive(true);
            TMP_Text[] tMP_Texts = resourceContainer.GetComponentsInChildren<TMP_Text>();
            tMP_Texts[0].text = resource.resourceType.ToString();
            tMP_Texts[1].text = resource.amount.ToString();
            resourceContainers.Add(resourceContainer);
        }
    }

    public void SetResourceContainers()
    {
        foreach (RectTransform resourceContainer in resourceContainers)
        {
            TMP_Text[] tMP_Texts = resourceContainer.GetComponentsInChildren<TMP_Text>();
            string resourceName = tMP_Texts[0].text;

            if (Enum.TryParse(resourceName, out ResourceTypes resourceType))
            {
                Resource res = GetResource(resourceType);
                if (res != null)
                    tMP_Texts[1].text = res.amount.ToString();
            }
        }
    }


    private void OnAmountEndEdit(string value)
    {
        fromAmount = float.Parse(value);
        toAmount = CalculateConvertedResource();
        toAmountText.text = toAmount.ToString();
    }

    private void InitializeFromDropdowns()
    {
        List<string> fromOptionLabels = new();

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
        toAmount = CalculateConvertedResource();
        toAmountText.text = toAmount.ToString();
    }


    private void OnFromValueChanged(int value)
    {
        from = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), from_Dropdown.options[value].text);
        InitializeToDropdown();
    }

    private void OnToValueChanged(int value)
    {
        to = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), to_Dropdown.options[value].text);
        toAmount = CalculateConvertedResource();
        toAmountText.text = toAmount.ToString();
    }


    private void OnEnable()
    {
        if (inputActions != null)
        {
            var uiMap = inputActions.FindActionMap("Actions");

            convertAction = uiMap.FindAction("Convert");
            toggleUIAction = uiMap.FindAction("ToggleUI");

            convertAction.performed += _ => OnConvertPressed();
            toggleUIAction.performed += _ => OnToggleUIPressed();

            convertAction.Enable();
            toggleUIAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (convertAction.enabled && toggleUIAction.enabled)
        {
            convertAction.Disable();
            toggleUIAction.Disable();
        }
    }


    private void Update()
    {
        // Optional — for manual inspector trigger
        if (convert)
        {
            convert = false;
            OnConvertPressed();
        }
    }

    private void OnConvertPressed()
    {
        if (ConvertSpecialAbility(from, fromAmount, to))
        {
            GameManager.Instance.debugMessageTextToShow = "Converted";
            SetResourceContainers();
        }
    }

    private void OnToggleUIPressed()
    {
        UIAnimator.SetBool("Load", !UIAnimator.GetBool("Load"));
        Time.timeScale = Time.timeScale == 1 ? 0.1f : 1f;
    }

    public void UnLoadUI()
    {
        Time.timeScale = 1f;
        UIAnimator.SetBool("Load", false);
    }


    public void DoConversion()
    {
        if (ConvertSpecialAbility(from, fromAmount, to))
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

    public float CalculateConvertedResource()
    {
        foreach (Conversion conversion in AbilityConversionSO.Conversions)
        {
            if (conversion.startingResource.resourceType == from && conversion.convertedResource.resourceType == to)
            {
                float ratio = conversion.convertedResource.amount / conversion.startingResource.amount;
                float result = fromAmount * ratio;
                return result;
            }
        }

        GameManager.Instance.debugMessageTextToShow = "Conversion is not defined!";
        return 0;
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
    NoOfTeleports,
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