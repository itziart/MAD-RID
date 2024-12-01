using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using UnityEngine.UI; // For UI button handling

public class LevelScreenManager : MonoBehaviour
{
    // References to the buttons
    public Button menuButton;
    public Button level1Button;
    public Button level2Button;

    private void Start()
    {
        // Ensure buttons are assigned
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }

        if (level1Button != null)
        {
            level1Button.onClick.AddListener(GoToFirstLevel);
        }

        if (level2Button != null)
        {
            level2Button.onClick.AddListener(GoToSecondLevel);
        }
    }
    // Method to go to the StartScreen
    private void GoToMenu()
    {
        // Load the levels choice screen
        SceneManager.LoadScene("StartScreen");
    }

    // Method to go to the first level
    private void GoToFirstLevel()
    {
        // Load the first level scene 
        SceneManager.LoadScene("Level1");
    }

    //Method to go to the credits scene
    public void GoToSecondLevel()
    {
        // Load the credits scene 
        SceneManager.LoadScene("Level2");
    }


}