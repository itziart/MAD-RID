using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class IntroSceneManager : MonoBehaviour
{
    // Function to change scene after animation finishes
    public void ChangeScene()
    {
        // Load the Start Screen Scene (change the scene name as needed)
        SceneManager.LoadScene("StartScreen");
    }
}