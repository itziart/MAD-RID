using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName = "NPC Name";
    public bool hasQuest = false;
    public bool questCompleted = false;
    public bool questConditionSatisfied = false;
    public string questItemRequired = "Key"; // Name of the required item
    public string questDialogue = "I need a key to open the door.";
    public string questCompletedDialogue = "Thank you for completing my quest!";

    public GameObject questEffectPrefab; // Optional: Assign a prefab for the quest completion effect
    public Transform effectSpawnLocation; // Optional: Where the effect appears (e.g., vomit position)

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
        else if (questConditionSatisfied)
        {
            questCompleted = true;
            dialogManager.ShowDialog($"{npcName}: {questCompletedDialogue}");
        }
        else if (questCompleted)
        {
            dialogManager.ShowDialog($"{npcName}: {questCompletedDialogue}");
        }
        else
        {
            dialogManager.ShowDialog($"...: Get lost kid!");
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

            // Trigger unique quest completion behavior
            OnQuestComplete();

            return true;
        }

        Debug.Log($"{npcName}: This is not the item I need.");
        return false;
    }

    // This method is virtual so it can be overridden in derived classes
    protected virtual void OnQuestComplete()
    {
        // Default behavior: Spawn a quest effect, if assigned
        if (questEffectPrefab != null && effectSpawnLocation != null)
        {
            Instantiate(questEffectPrefab, effectSpawnLocation.position, Quaternion.identity);
        }
        Destroy(gameObject);
        Debug.Log($"{npcName}: Quest effect triggered.");
    }
}
