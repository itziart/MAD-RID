using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogPanel; // Panel that holds the dialog UI
    public Image portraitImage; // Image component for the NPC's portrait
    public TMP_Text nameText; // Text component for the NPC's name
    public TMP_Text dialogText; // Text component for the dialog itself
    public Sprite playerPortrait; // Portrait for the player
    public Image screenFadeImage; // Image used for screen fade effect
    public LevelManager levelManager; // Reference to the LevelManager to finish the level

    [Header("Settings")]
    public float fadeDuration = 1f; // Duration of the fade effect
    public float textDisplaySpeed = 0.02f; // Speed at which dialog text is revealed

    private Queue<(Sprite portrait, string name, string text)> dialogQueue = new Queue<(Sprite, string, string)>(); // Queue to store dialog entries
    private bool isDialogActive = false; // Flag to track if dialog is currently being shown
    private bool isDisplayingText = false; // Flag to track if text is still being displayed
    public bool isLevelFinished = false; // Flag to check if the level is completed

    // Start is called before the first frame update
    private void Start()
    {
        screenFadeImage.gameObject.SetActive(false); // Initially hide the fade effect
    }

    
    // Displays a single dialog entry (portrait, name, and text).
    public void ShowDialog(Sprite npcPortrait, string npcName, string dialog)
    {
        ClearDialogQueue(); // Clear the previous dialog queue

        dialogQueue.Enqueue((npcPortrait, npcName, dialog)); // Add new dialog to the queue
        StartDialogSequence(); // Start displaying the dialog sequence
    }

    
    // Displays a sequence of dialog entries from a list or table.
    public void ShowDialogSequence(List<(Sprite portrait, string name, string text)> dialogs)
    {
        ClearDialogQueue(); // Clear the previous dialog queue

        foreach (var dialog in dialogs) // Add each dialog entry to the queue
        {
            dialogQueue.Enqueue(dialog);
        }

        StartDialogSequence(); // Start displaying the dialog sequence
    }

    // Starts the dialog sequence if it's not already active
    private void StartDialogSequence()
    {
        if (dialogQueue.Count > 0 && !isDialogActive)
        {
            dialogPanel.SetActive(true); // Show the dialog panel
            isDialogActive = true; // Mark dialog as active
            DisplayNextDialog(); // Display the first dialog entry
        }
    }

    // Coroutine to display the dialog text one character at a time
    private IEnumerator DisplayDialogText(string text)
    {
        isDisplayingText = true; // Set flag to true while displaying text
        dialogText.text = ""; // Clear the current text

        foreach (char c in text)
        {
            dialogText.text += c; // Add one character at a time
            yield return new WaitForSeconds(textDisplaySpeed); // Wait for the specified speed before displaying the next character
        }

        isDisplayingText = false; // Set flag to false once text is fully displayed
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDialogActive && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))) // Check for input to skip text or move to next dialog
        {
            if (!isDisplayingText) // Only proceed if text is finished displaying
            {
                if (dialogQueue.Count > 0)  // Ensure there is dialog left in the queue
                {
                    DisplayNextDialog(); // Display the next dialog entry
                }
                else
                {
                    HideDialog();  // If no more dialog, hide the dialog panel
                }
            }
        }
    }

    // Displays the next dialog entry in the queue
    private void DisplayNextDialog()
    {
        if (dialogQueue.Count > 0)
        {
            var dialog = dialogQueue.Dequeue(); // Dequeue the next dialog
            portraitImage.sprite = dialog.portrait; // Set the portrait image
            nameText.text = dialog.name; // Set the name text

            StopAllCoroutines(); // Stop any currently running coroutines
            StartCoroutine(DisplayDialogText(dialog.text));  // Start displaying the dialog text
        }
        else
        {
            HideDialog();  // Hide dialog panel if no dialogs are left
        }
    }

    // Hides the dialog panel and checks if the level should be finished
    private void HideDialog()
    {
        dialogPanel.SetActive(false); // Hide the dialog panel
        isDialogActive = false; // Mark dialog as inactive
        if (isLevelFinished)
        {
            levelManager.FinishLevel(); // Finish the level if the flag is set
        }
    }

    // Starts the fade-out screen effect
    public void StartFadeOut()
    {
        StartCoroutine(FadeScreen(true)); // Fade to black
    }

    // Starts the fade-in screen effect
    public void StartFadeIn()
    {
        StartCoroutine(FadeScreen(false)); // Fade to transparent
    }

    // Coroutine to handle the screen fade effect (fade out or fade in)
    public IEnumerator FadeScreen(bool fadeOut)
    {
        float time = 0f;

        screenFadeImage.gameObject.SetActive(true); // Ensure the screen fade image is active

        // Set initial and target colors based on the fade direction
        Color initialColor = fadeOut ? new Color(0, 0, 0, 0) : Color.black;
        Color targetColor = fadeOut ? Color.black : new Color(0, 0, 0, 0);

        screenFadeImage.color = initialColor; // Set the initial color

        // Perform the fade effect
        while (time < fadeDuration)
        {
            screenFadeImage.color = Color.Lerp(initialColor, targetColor, time / fadeDuration); // Gradually change the color
            time += Time.deltaTime; // Increment time
            yield return null; // Wait for the next frame
        }

        screenFadeImage.color = targetColor; // Set the final color

        // If fade-in is complete, deactivate the screen fade image
        if (!fadeOut)
        {
            screenFadeImage.gameObject.SetActive(false);
        }
    }

    // Displays a single dialog for the player.
    public void ShowPlayerDialog(string dialog)
    {
        ShowDialog(playerPortrait, "Player", dialog); // Call ShowDialog with player-specific data
    }

    
    // Clears the current dialog queue.
    private void ClearDialogQueue()
    {
        dialogQueue.Clear(); // Clear the dialog queue to reset
    }
}
