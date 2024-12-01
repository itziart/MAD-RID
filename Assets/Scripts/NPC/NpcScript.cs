using System.Collections;
using UnityEngine;

// Base class for NPCs that interact with players, manage quests, and perform scripted actions
public class NPC : MonoBehaviour
{
    public NPCData npcData; // Reference to the NPC's static data, including dialogues and portrait
    public bool hasQuest = false; // Indicates whether the NPC offers a quest
    public bool questCompleted = false; // Tracks if the NPC's quest has been completed
    public bool questConditionSatisfied = false; // Tracks if the quest's condition is met
    public bool questActive = false; // Indicates if the quest is currently active
    public bool isFinalQuest = false; // Marks if this is the final quest in the level
    public string questItemRequired = "Key"; // The name of the item required to complete the quest

    public GameObject questEffectPrefab; // (Optional) Visual effect prefab for quest completion (e.g., sparkles)
    public Transform effectSpawnLocation; // (Optional) Position where the visual effect should appear
    public Transform newSpawnLocation; // (Optional) Where the NPC moves after quest completion

    protected DialogManager dialogManager; // Reference to the dialog manager for showing dialogs
    protected LevelManager levelManager; // Reference to the level manager for triggering events like end-of-level dialogs

    // Initialize the NPC's components and validate setup
    protected virtual void Start()
    {
        dialogManager = FindObjectOfType<DialogManager>(); // Automatically find the dialog manager in the scene
        levelManager = FindObjectOfType<LevelManager>(); // Automatically find the level manager in the scene

        // Ensure the NPC has associated data for proper functionality
        if (npcData == null)
        {
            Debug.LogError($"NPC {gameObject.name} is missing NPCData!"); // Log an error if no NPCData is assigned
        }
    }

    // Method called when the player interacts with the NPC
    public virtual void Interact()
    {
        if (dialogManager == null || npcData == null) return; // Exit if dialog manager or NPC data is missing

        // If the NPC has an active quest that is not yet completed
        if (hasQuest && !questCompleted)
        {
            if (!questActive)
            {
                // Start the quest by showing the quest dialogue
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue);
                questActive = true; // Mark the quest as active
                return;
            }

            if (CheckQuestCondition())
            {
                // Complete the quest if the condition is satisfied
                questCompleted = true;
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue);
                OnQuestComplete(); // Trigger quest completion behavior
            }
            else
            {
                // Show the quest dialogue again if the condition is not met
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue);
            }
        }
        else if (questCompleted)
        {
            // If the quest is already completed, show the quest completion dialogue
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue);
        }
        else
        {
            // Default interaction when no quest is active
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.defaultDialogue);
        }
    }

    // Attempt to complete the quest with the provided item
    public bool TryCompleteQuest(ItemData item)
    {
        if (questCompleted)
        {
            Debug.Log($"{npcData.npcName}: Quest already completed."); // Log that the quest is already completed
            return false; // Return false since the quest is already done
        }

        if (item != null && item.itemName == questItemRequired)
        {
            // Complete the quest if the item matches the required item
            questCompleted = true;
            Debug.Log($"{npcData.npcName}: {npcData.questCompletedDialogue}");

            OnQuestComplete(); // Trigger quest completion behavior

            if (isFinalQuest)
            {
                levelManager.TriggerEndLevelDialogs(); // Trigger special behavior for final quests
            }

            return true; // Return true as the quest was successfully completed
        }

        Debug.Log($"{npcData.npcName}: This is not the item I need."); // Log a message if the item doesn't match
        return false; // Return false since the quest is not completed
    }

    // Called when the quest is successfully completed; can be overridden for custom behavior
    protected virtual void OnQuestComplete()
    {
        StartCoroutine(HandleQuestCompletion()); // Start the quest completion process
    }

    // Handles the quest completion process, including effects, fade-out, and NPC removal
    protected virtual IEnumerator HandleQuestCompletion()
    {
        // Show quest effect (if any) at the specified location
        if (questEffectPrefab != null && effectSpawnLocation != null)
        {
            Instantiate(questEffectPrefab, effectSpawnLocation.position, Quaternion.identity);
        }

        // Start the fade-out animation
        dialogManager.StartFadeOut();
        yield return new WaitForSeconds(dialogManager.fadeDuration); // Wait for the fade-out to complete

        // Default behavior: remove the NPC from the scene
        Destroy(gameObject);
        Debug.Log($"{npcData.npcName} has been removed from the scene."); // Log NPC removal

        // Start the fade-in animation
        dialogManager.StartFadeIn();
        Debug.Log("Fade-in started."); // Log that fade-in has begun
    }

    // Default implementation of a quest condition check, can be overridden for custom conditions
    protected virtual bool CheckQuestCondition()
    {
        return false; // By default, quest condition is not satisfied
    }
}
