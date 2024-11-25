using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName = "NPC Name";
    public bool hasQuest = false;
    public bool questCompleted = false;
    public string questItemRequired = "Key"; // Name of the required item
    public string questDialogue = "I need a key to open the door.";
    public string questCompletedDialogue = "Thank you for completing my quest!";

    private DialogManager dialogManager;

    private void Start()
    {
        dialogManager = FindObjectOfType<DialogManager>(); // Find the dialog manager in the scene.
    }

    public void Interact()
    {
        if (dialogManager == null) return;

        if (hasQuest && !questCompleted)
        {
            dialogManager.ShowDialog($"{npcName}: {questDialogue}");
        }
        else if (questCompleted)
        {
            dialogManager.ShowDialog($"{npcName}: {questCompletedDialogue}");
        }
        else
        {
            dialogManager.ShowDialog($"{npcName}: Hello there!");
        }
    }

    public bool TryCompleteQuest(ItemData item)
    {
        if (questCompleted)
        {
            Debug.Log($"{npcName}: Quest already completed.");
            return false;
        }

        if (item != null && item.itemName == questItemRequired)
        {
            questCompleted = true;
            Debug.Log($"{npcName}: {questCompletedDialogue}");
            return true;
        }

        Debug.Log($"{npcName}: This is not the item I need.");
        return false;
    }
}