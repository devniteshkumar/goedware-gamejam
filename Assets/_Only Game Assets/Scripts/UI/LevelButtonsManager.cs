using UnityEngine;
using UnityEngine.UI;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public Button[] LevelButtons;
    void Awake()
    {
        int UnlockedLevel = SceneController.UnlockedLevel;
        Debug.Log(UnlockedLevel);
        for (int i = 0; i < LevelButtons.Length; i++)
        {
            if (i < UnlockedLevel) LevelButtons[i].interactable = true;
            else LevelButtons[i].interactable = false;
        }
    }
}
