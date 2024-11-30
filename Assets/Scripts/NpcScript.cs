using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData npcData; // Reference to the NPC's static data
    public bool hasQuest = false;
    public bool questCompleted = false;
    public bool questConditionSatisfied = false;
    public bool questActive = false;
    public bool isFinalQuest = false;
    public string questItemRequired = "Key"; // Name of the required item

    public GameObject questEffectPrefab; // Optional: Assign a prefab for the quest completion effect
    public Transform effectSpawnLocation; // Optional: Where the effect appears (e.g., vomit position)
    public Transform newSpawnLocation; // Optional: Where the NPC moves after quest completion

    protected DialogManager dialogManager;
    protected LevelManager levelManager;
    

    protected virtual void Start()
    {
        dialogManager = FindObjectOfType<DialogManager>(); // Find the dialog manager in the scene.
        levelManager = FindObjectOfType<LevelManager>();

        if (npcData == null)
        {
            Debug.LogError($"NPC {gameObject.name} is missing NPCData!");
        }
    }

    public virtual void Interact()
    {
        if (dialogManager == null || npcData == null) return;

        if (hasQuest && !questCompleted)
        {
            if (!questActive)
            {
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue);
                questActive = true;
                return;
            }
            if (CheckQuestCondition())
            {
                questCompleted = true;
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue);
                OnQuestComplete();
            }
            else
            {
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue);
            }
        }
        else if (questCompleted)
        {
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue);
        }
        else
        {
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.defaultDialogue);
        }
    }

    public bool TryCompleteQuest(ItemData item)
    {
        if (questCompleted)
        {
            Debug.Log($"{npcData.npcName}: Quest already completed.");
            return false;
        }

        if (item != null && item.itemName == questItemRequired)
        {
            questCompleted = true;
            Debug.Log($"{npcData.npcName}: {npcData.questCompletedDialogue}");

            OnQuestComplete();
            return true;
        }

        Debug.Log($"{npcData.npcName}: This is not the item I need.");
        return false;
    }

    protected virtual void OnQuestComplete()
    {
        StartCoroutine(HandleQuestCompletion());
    }

    protected virtual IEnumerator HandleQuestCompletion()
    {
        // Show quest effect if any
        if (questEffectPrefab != null && effectSpawnLocation != null)
        {
            Instantiate(questEffectPrefab, effectSpawnLocation.position, Quaternion.identity);
        }

        // Start fade-out
        dialogManager.StartFadeOut();
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // Default behavior: destroy the NPC if not overridden
        Destroy(gameObject);
        Debug.Log($"{npcData.npcName} has been removed from the scene.");
        dialogManager.StartFadeIn();

        if (isFinalQuest)
        {
            levelManager.TriggerEndLevelDialogs();
        }

    }


    protected virtual bool CheckQuestCondition()
    {
        return false; // Default implementation, overridden in subclasses
    }
}
