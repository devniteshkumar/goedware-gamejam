using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Values To Modify")]
    [Tooltip("Set This string to show some message in Game View and it will show it for 3 secs")]public string debugMessageTextToShow = "";
    
    [Header("Properties _ Don't Change")]
    float textShowingTimer = 0;
    float timeToShowText = 3;
    bool startTimer;


    [Header("References")]
    public TMP_Text DebugMessageText;


    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (DebugMessageText.text != debugMessageTextToShow)
        {
            DebugMessageText.text = debugMessageTextToShow;
            startTimer = true;
            textShowingTimer = 0;
        }

        if (startTimer)
        {
            textShowingTimer += Time.deltaTime;
            if (textShowingTimer >= timeToShowText)
            {
                textShowingTimer = 0;
                startTimer = false;
                debugMessageTextToShow = "";
                DebugMessageText.text = debugMessageTextToShow;
            }
        }
    }
}
