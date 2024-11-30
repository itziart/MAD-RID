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
    public Image screenFadeImage; // Reference to the black fade panel
    public float fadeDuration = 1f; // How long the fade lasts

    private bool isDialogActive = false;

    private void Start()
    {
        dialogPanel.SetActive(false);
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

    // Method to start the fade effect
    public void StartFadeOut()
    {
        StartCoroutine(FadeScreen(true)); // Fade out
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeScreen(false)); // Fade in
    }

    private IEnumerator FadeScreen(bool fadeOut)
    {
        float time = 0f;
        screenFadeImage.gameObject.SetActive(true);

        Color initialColor = screenFadeImage.color;
        Color targetColor = fadeOut ? Color.black : new Color(0, 0, 0, 0);

        while (time < fadeDuration)
        {
            screenFadeImage.color = Color.Lerp(initialColor, targetColor, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set correctly
        screenFadeImage.color = targetColor;
        if (!fadeOut)
        {
            screenFadeImage.gameObject.SetActive(false); // Hide after fade in
        }
    }
}
