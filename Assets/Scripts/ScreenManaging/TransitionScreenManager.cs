using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScreenManager : MonoBehaviour
{
    // Go To Levels Screen
    public void ChangeScene()
    {
        SceneManager.LoadScene("LevelsScreen");
    }
}
