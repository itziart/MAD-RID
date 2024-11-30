using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using UnityEngine.UI; // For UI button handling

public class StartScreenManager : MonoBehaviour
{
    // References to the buttons
    public Button startLevelButton;
    public Button creditsButton;
    public Button levelsChoiceButton;

    private void Start()
    {
        // Ensure buttons are assigned
        if (startLevelButton != null)
        {
            startLevelButton.onClick.AddListener(GoToFirstLevel);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(GoToCredits);
        }

        if (levelsChoiceButton != null)
        {
            // levelsChoiceButton.onClick.AddListener(GoToLevelsChoice);
        }
    }

    // Method to go to the first level
    private void GoToFirstLevel()
    {
        // Load the first level scene 
        SceneManager.LoadScene("TutorialLevel");
    }

    //Method to go to the credits scene
    public void GoToCredits()
    {
        // Load the credits scene 
        SceneManager.LoadScene("CreditsScreen");
    }

    // Method to go to the levels choice screen
    private void GoToLevelsChoice()
    {
        // Load the levels choice screen
        SceneManager.LoadScene("LevelsChoice");
    }
}