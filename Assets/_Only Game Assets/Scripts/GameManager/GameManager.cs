using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Values To Modify")]
    [Tooltip("Set This string to show some message in Game View and it will show it for 3 secs")]public string debugMessageTextToShow = "";
    
    [Header("Properties _ Don't Change")]
    float textShowingTimer = 0;
    float timeToShowText = 3;
    bool startTimer;


    [Header("References")]
    [SerializeField] TMP_Text DebugMessageText;


    [Header("Game Values")]
    public List<string> levels = new();
    public int currentScene = 0;


    public static GameManager Instance { get; private set; }

    void Awake()
    {
        SceneController.UnlockedLevel = 1;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        //LoadSceneAdditiveIfNotLoaded("Nitesh");
        //LoadSceneAdditiveIfNotLoaded("Vishal");
    }

    private void LoadSceneAdditiveIfNotLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == sceneName)
            {
                return;
            }
        }

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
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
