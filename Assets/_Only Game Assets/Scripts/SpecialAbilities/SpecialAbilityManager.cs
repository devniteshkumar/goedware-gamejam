using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialAbilityManager : MonoBehaviour
{
    public static List<Resource> AllResources = new();


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
    public PlayerMovement playerMovement;
    public HealthSystem playerHealth;
    public AbilityConversionSO AbilityConversionSO;
    public Animator UIAnimator;
    public InputActionAsset inputActions;
    private InputAction convertAction;
    private InputAction toggleUIAction;

    [Header("Input Var")]
    public int useSpecialAbility;


    private void Start()
    {
        AllResources.Clear();
        AllResources.Add(new Resource(ResourceTypes.Time, 120));
        AllResources.Add(new Resource(ResourceTypes.TimeFreeze, 0));
        AllResources.Add(new Resource(ResourceTypes.MovementSpeed, 10));
        AllResources.Add(new Resource(ResourceTypes.NoOfTeleports, 0));
        AllResources.Add(new Resource(ResourceTypes.AttackDamage, 5));
        AllResources.Add(new Resource(ResourceTypes.AttackingRadius, 0.5f));
        AllResources.Add(new Resource(ResourceTypes.Health, 0));
        AllResources.Add(new Resource(ResourceTypes.GiveDamage, 0));


        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerHealth = playerMovement.GetComponent<HealthSystem>();


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

            //convertAction.performed += _ => OnConvertPressed();
            //toggleUIAction.performed += _ => OnToggleUIPressed();

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
            playerHealth.currentHealth = GetResource(ResourceTypes.Health).amount;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            useSpecialAbility = 1;
            audio_manager.Instance.PlaySound(audio_manager.Instance.convert);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            useSpecialAbility = 2;
            audio_manager.Instance.PlaySound(audio_manager.Instance.convert);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            useSpecialAbility = 3;
            audio_manager.Instance.PlaySound(audio_manager.Instance.convert);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            useSpecialAbility = 4;
            audio_manager.Instance.PlaySound(audio_manager.Instance.convert);
        }

        if (useSpecialAbility != 0)
        {
            UsingSpecialAbility(useSpecialAbility);
            useSpecialAbility = 0;
        }


        GetResource(ResourceTypes.Time).amount -= Time.deltaTime;

        if (GetResource(ResourceTypes.TimeFreeze).amount > 0)
        {
            Time.timeScale = 0.1f;
            GetResource(ResourceTypes.TimeFreeze).amount -= Time.unscaledDeltaTime;
        }
        else if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }

        if (GetResource(ResourceTypes.GiveDamage).amount > 0)
        {
            Debug.Log("Starting damage distribution...");

            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log("Enemies found: " + allEnemies.Length);

            float totalDamage = GetResource(ResourceTypes.GiveDamage).amount;
            float damagePerEnemy = totalDamage / (allEnemies.Length == 0 ? 1 : allEnemies.Length);
            Debug.Log("Total damage to give: " + totalDamage);
            Debug.Log("Damage per enemy: " + damagePerEnemy);

            // Reset total damage
            GetResource(ResourceTypes.GiveDamage).amount = 0;

            foreach (var enemy in allEnemies)
            {
                if (enemy == null)
                {
                    Debug.LogWarning("Encountered null enemy object.");
                    continue;
                }

                Debug.Log("Processing enemy: " + enemy.name);

                HealthSystem health = enemy.GetComponent<HealthSystem>();
                if (health == null)
                {
                    Debug.LogWarning("Enemy " + enemy.name + " missing HealthSystem component.");
                    continue;
                }

                Debug.Log("Dealing " + damagePerEnemy + " damage to: " + enemy.name);
                health.TakeDamage(damagePerEnemy);
            }


            Debug.Log("All damage distributed.");
        }
    }

    private void UsingSpecialAbility(int n)
    {
        switch (n)
        {
            case 1:
                from = ResourceTypes.Time;
                to = ResourceTypes.TimeFreeze;
                fromAmount = 5;
                convert = true;
                break;
            case 2:
                from = ResourceTypes.AttackDamage;
                to = ResourceTypes.AttackingRadius;
                fromAmount = 1;
                convert = true;
                break;
            case 3:
                from = ResourceTypes.MovementSpeed;
                to = ResourceTypes.NoOfTeleports;
                fromAmount = 1;
                convert = true;
                break;
            case 4:
                from = ResourceTypes.Health;
                to = ResourceTypes.GiveDamage;
                fromAmount = 4;
                convert = true;
                break;
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

    private bool ConvertSpecialAbility(ResourceTypes from_resource, float amount, ResourceTypes to_resource)
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

    public static Resource GetResource(ResourceTypes resourceType)
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
    GiveDamage,
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