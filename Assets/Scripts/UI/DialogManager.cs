using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogPanel;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public Sprite playerPortrait;
    public Image screenFadeImage;
    public LevelManager levelManager;

    [Header("Settings")]
    public float fadeDuration = 1f; // How long the fade lasts
    public float textDisplaySpeed = 0.02f; // Speed at which text is revealed

    private Queue<(Sprite portrait, string name, string text)> dialogQueue = new Queue<(Sprite, string, string)>(); // Queue for dialog entries
    private bool isDialogActive = false;
    private bool isDisplayingText = false;
    public bool isLevelFinished = false;

    private void Start()
    {
        screenFadeImage.gameObject.SetActive(false); // Hide the fade initially
    }

    /// <summary>
    /// Displays a single dialog entry (portrait, name, and text).
    /// </summary>
    public void ShowDialog(Sprite npcPortrait, string npcName, string dialog)
    {
        ClearDialogQueue();

        dialogQueue.Enqueue((npcPortrait, npcName, dialog));
        StartDialogSequence();
    }

    /// <summary>
    /// Displays a sequence of dialog entries from a list or table.
    /// </summary>
    public void ShowDialogSequence(List<(Sprite portrait, string name, string text)> dialogs)
    {
        ClearDialogQueue();

        foreach (var dialog in dialogs)
        {
            dialogQueue.Enqueue(dialog);
        }

        StartDialogSequence();
    }

    private void StartDialogSequence()
    {
        if (dialogQueue.Count > 0 && !isDialogActive)
        {
            dialogPanel.SetActive(true);
            isDialogActive = true;
            DisplayNextDialog();
        }
    }


    private IEnumerator DisplayDialogText(string text)
    {
        isDisplayingText = true;
        dialogText.text = "";

        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textDisplaySpeed);
        }

        isDisplayingText = false;
    }

    private void Update()
    {
        if (isDialogActive && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            if (!isDisplayingText)
            {
                // Move to the next dialog entry
                if (dialogQueue.Count > 0)  // Ensure there's a dialog left in the queue
                {
                    DisplayNextDialog();  // Move to next dialog
                }
                else
                {
                    HideDialog();  // If there are no dialogs left, hide the dialog panel
                }
            }
        }
    }

    private void DisplayNextDialog()
    {
        if (dialogQueue.Count > 0)
        {
            var dialog = dialogQueue.Dequeue();
            portraitImage.sprite = dialog.portrait;
            nameText.text = dialog.name;

            StopAllCoroutines();
            StartCoroutine(DisplayDialogText(dialog.text));  // Start displaying text for the new dialog
        }
        else
        {
            HideDialog();  // Hide dialog when the queue is empty
        }
    }

    private void HideDialog()
    {
        dialogPanel.SetActive(false);
        isDialogActive = false;
        if (isLevelFinished)
        {
            levelManager.FinishLevel();
        }
    }


    public void StartFadeOut()
    {
        StartCoroutine(FadeScreen(true)); // Fade out to black
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeScreen(false)); // Fade in to transparent
    }

    public IEnumerator FadeScreen(bool fadeOut)
    {
        float time = 0f;

        // Ensure the screen fade image is active at the start
        screenFadeImage.gameObject.SetActive(true);

        // Set the starting and target colors based on fade direction
        Color initialColor = fadeOut ? new Color(0, 0, 0, 0) : Color.black;
        Color targetColor = fadeOut ? Color.black : new Color(0, 0, 0, 0);

        // Set the initial color to avoid abrupt changes
        screenFadeImage.color = initialColor;

        // Perform the fade
        while (time < fadeDuration)
        {
            screenFadeImage.color = Color.Lerp(initialColor, targetColor, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set precisely
        screenFadeImage.color = targetColor;

        // If fade-in is complete, disable the screen fade image
        if (!fadeOut)
        {
            screenFadeImage.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Displays a single dialog for the player.
    /// </summary>
    public void ShowPlayerDialog(string dialog)
    {
        ShowDialog(playerPortrait, "Player", dialog);
    }

    /// <summary>
    /// Clears the current dialog queue.
    /// </summary>
    private void ClearDialogQueue()
    {
        dialogQueue.Clear();
    }
}