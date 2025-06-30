using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;
using Unity.VisualScripting;
public class SceneController : MonoBehaviour
{
    [SerializeField]public static SceneController instance;
    public static int CurrentLevel=1;
    public static int UnlockedLevel=1;
    public Button[] LevelButtons;
    public Animator FadeAnimator;
    public float TransitionTime;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    void Awake()
    {
        instance = this;
    }
    public void LoadScene(string name, Animator animator)
    {
        StartCoroutine(SceneTransition(name, animator));
    }
    public void Quit(){
        Application.Quit();
    }
    public void Play(){
        LoadScene("Level Map",FadeAnimator);
    }
    public void StartLevel(int Level)
    {
        CurrentLevel = Level;
        Time.timeScale = 1f;
        LoadScene("N" + CurrentLevel.ToString() ,FadeAnimator);
    }
    public void Retry()
    {
        StartLevel(CurrentLevel);
    }
    public void NextLevel()
    {
        StartLevel(CurrentLevel);
    }
    public void MainMenu()
    {
        LoadScene("Main Menu", FadeAnimator);
    }
    public void Instructions()
    {
        LoadScene("Instructions",FadeAnimator);
    }
    public IEnumerator Complete()
    {
        yield return new WaitForSeconds(2f);
        UnlockedLevel = Mathf.Max(UnlockedLevel, CurrentLevel + 1);
        WinScreen.SetActive(true);
    }
    public IEnumerator Lose(){
        yield return new WaitForSeconds(2f);
        UnlockedLevel = Mathf.Max(UnlockedLevel, CurrentLevel+1);
        LoseScreen.SetActive(true);
    }
    IEnumerator SceneTransition(string name,Animator Transition){
        Transition.enabled=true;
        Transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionTime);
        SceneManager.LoadSceneAsync(name);   
    }
}
