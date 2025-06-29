using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;
using Unity.VisualScripting;
public class SceneController : MonoBehaviour
{
    public static int CurrentLevel;
    public static int UnlockedLevel;
    public Button[] LevelButtons;
    public Animator FadeAnimator;
    public float TransitionTime;
    public GameObject WinScreen;
    public GameObject LoseScreen;
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
        LoadScene("Palaksh",FadeAnimator);
    }
    public void Retry()
    {
        StartLevel(CurrentLevel);
    }
    public void NextLevel()
    {
        CurrentLevel++;
        UnlockedLevel = Mathf.Max(UnlockedLevel, CurrentLevel);
        StartLevel(CurrentLevel);
    }
    public void MainMenu()
    {
        LoadScene("Main Menu", FadeAnimator);
    }
    public void Complete(){
        WinScreen.SetActive(true);
    }
    public void Lose(){
        LoseScreen.SetActive(true);
    }
    IEnumerator SceneTransition(string name,Animator Transition){
        Transition.enabled=true;
        Transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionTime);
        SceneManager.LoadSceneAsync(name);   
    }
}
