using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public Sprite playerPortrait;
    public Image screenFadeImage; // Reference to the black fade panel
    public float fadeDuration = 1f; // How long the fade lasts

    public bool isDialogActive = false;


    private void Start()
    {
        // dialogPanel.SetActive(false);
        screenFadeImage.gameObject.SetActive(false); // Hide the fade initially
    }

    public void ShowDialog(Sprite npcPortrait, string npcName, string dialog)
    {
        portraitImage.sprite = npcPortrait;
        nameText.text = npcName;
        dialogText.text = dialog;
        dialogPanel.SetActive(true);
        isDialogActive = true;
    }



    private void Update()
    {
        if (isDialogActive && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            HideDialog();
        }
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        isDialogActive = false;
    }

    public bool IsDialogActive()
    {
        return isDialogActive;
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeScreen(true)); // Fade out to black
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeScreen(false)); // Fade in to transparent
    }

    private IEnumerator FadeScreen(bool fadeOut)
    {
        float time = 0f;

        screenFadeImage.gameObject.SetActive(true);

        // Set the starting color based on fade direction
        screenFadeImage.color = fadeOut ? new Color(0, 0, 0, 0) : Color.black;

        // Determine target colors for the fade
        Color initialColor = screenFadeImage.color;
        Color targetColor = fadeOut ? Color.black : new Color(0, 0, 0, 0);

        // Perform the fade
        while (time < fadeDuration)
        {
            screenFadeImage.color = Color.Lerp(initialColor, targetColor, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set correctly
        screenFadeImage.color = targetColor;

        // If fade-in is complete, hide the screen fade image
        if (!fadeOut)
        {
            screenFadeImage.gameObject.SetActive(false);
        }
    }

    public void ShowPlayerDialog(string dialog)
    {
        Debug.Log($"Showing player dialog: {dialog}");
        // Use a default "Player" name for the dialog
        nameText.text = "Player";
        dialogText.text = dialog;

        // Assign a default player portrait
        portraitImage.sprite = playerPortrait;

        dialogPanel.SetActive(true);
        isDialogActive = true;
    }


}
