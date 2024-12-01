using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Script that handles the Player's Level Dialogs and the transition to next level

    public DialogManager dialogManager; // Reference to the DialogManager

    // Player Dialogs (Configurable in the Unity Inspector)
    [Header("Player Dialog Configuration")]
    public string startDialog = "Did this dude really have to throw up right by the doors?! I should go to the other exit...";
    public string endDialog = "People are so weird... I am done with this madness...";

    public Button menuButton; // UI Button to handle going back to the main menu

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        TriggerStartLevelDialogs(); // Show start level dialogs
        menuButton.onClick.AddListener(GoToMenu); // Configure menuButton to go back to the main menu
    }

    // Method to trigger the start level dialogs
    private void TriggerStartLevelDialogs()
    {
        dialogManager.ShowPlayerDialog(startDialog);
    }

    // Method to trigger the end level dialogs and screen fade out
    public void TriggerEndLevelDialogs()
    {
        StartCoroutine(TriggerEndLevelDialogsAfterFade());
    }

    // Method that handles the end level dialogs and screen fade out
    private IEnumerator TriggerEndLevelDialogsAfterFade()
    {
        // Wait for the fade-out operation to complete
        yield return StartCoroutine(dialogManager.FadeScreen(true)); // Fade out to black

        // Show the dialog after the fade, inform dialogManager about the end of level
        dialogManager.isLevelFinished = true;
        dialogManager.ShowPlayerDialog(endDialog);
    }

    // Method to trigger the finish of the level
    public void FinishLevel()
    {
        // Start fading in before transitioning to the next scene
        StartCoroutine(FinishLevelWithFadeIn());
    }

    // Method to handle the finish of the level
    private IEnumerator FinishLevelWithFadeIn()
    {
        // Trigger the fade-in effect before scene change
        yield return StartCoroutine(dialogManager.FadeScreen(true)); // Fade in to transparent

        // Proceed to the next level after fade-in is complete
        GoToLevelTransition();
    }

    // Method to go back to the main menu
    public void GoToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    // Method to go progress to the next level scene
    public void GoToLevelTransition()
    {
        SceneManager.LoadScene("NextLevelTransition");
    }
}