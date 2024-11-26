using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel; // Reference to the dialog panel.
    public TextMeshProUGUI dialogText; // Reference to the TMP Text component.

    private void Start()
    {
        dialogPanel.SetActive(false); // Hide the dialog panel initially.
    }

    public void ShowDialog(string text)
    {
        dialogText.text = text; // Set the dialog text.
        dialogPanel.SetActive(true); // Show the dialog panel.
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false); // Hide the dialog panel.
    }


}