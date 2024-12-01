using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public DialogManager dialogManager;

    [Header("Player Dialog Configuration")]
    public string startDialog = "Did this dude really have to throw up right by the doors?! I should go to the other exit...";
    public string endDialog = "People are so weird... I am done with this madness...";
    public Button menuButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        TriggerStartLevelDialogs();
        menuButton.onClick.AddListener(GoToMenu);
    }

    public void TriggerStartLevelDialogs()
    {
        dialogManager.ShowPlayerDialog(startDialog);
    }

    public void TriggerEndLevelDialogs()
    {
        StartCoroutine(TriggerEndLevelDialogsAfterFade());
    }

    private IEnumerator TriggerEndLevelDialogsAfterFade()
    {
        // Wait for the fade-out operation to complete
        yield return StartCoroutine(dialogManager.FadeScreen(true)); // Fade out to black

        // Show the dialog after the fade, inform dialogManager about the end of level
        dialogManager.isLevelFinished = true;
        dialogManager.ShowPlayerDialog(endDialog);
    }

    public void FinishLevel()
    {
        // Start fading in before transitioning to the next scene
        StartCoroutine(FinishLevelWithFadeIn());
    }

    private IEnumerator FinishLevelWithFadeIn()
    {
        // Trigger the fade-in effect before scene change
        yield return StartCoroutine(dialogManager.FadeScreen(true)); // Fade in to transparent

        // Proceed to the next level after fade-in is complete
        GoToLevelTransition();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void GoToLevelTransition()
    {
        SceneManager.LoadScene("NextLevelTransition");
    }
}