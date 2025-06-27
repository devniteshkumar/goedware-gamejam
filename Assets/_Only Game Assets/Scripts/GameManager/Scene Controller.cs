using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public Animator FadeAnimator;
    public float TransitionTime;    
    public void LoadScene(string name,Animator animator){
        StartCoroutine(SceneTransition(name,animator));
    }
    public void Quit(){
        Application.Quit();
    }
    public void Play(){
        LoadScene("Level Map",FadeAnimator);
    }
    public void StartLevel(int Level)
    {
        LoadScene("Level"+Level.ToString(),FadeAnimator);
    }
    public void MainMenu(){
        LoadScene("Main Menu",FadeAnimator);
    }
    public void Complete(){
        LoadScene("Game Complete",FadeAnimator);
    }
    public void Lose(){
        LoadScene("Lose Screen",FadeAnimator);
    }
    IEnumerator SceneTransition(string name,Animator Transition){
        Transition.enabled=true;
        Transition.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionTime);
        SceneManager.LoadSceneAsync(name);   
    }
}
