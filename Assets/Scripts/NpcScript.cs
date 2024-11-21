using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName = "NPC Name";              // Name of the NPC
    public bool hasQuest = false;                   // Whether the NPC has a quest
    public bool questCompleted = false;             // Whether the NPC's quest is completed
    public string questItemRequired = "Key";        // Item required to complete the quest
    public string questDialogue = "I need a key to open the door."; // Dialogue for the quest
    public string questCompletedDialogue = "Thank you for completing my quest!"; // Dialogue after quest completion

    public void Interact()
    {
        if (hasQuest && !questCompleted)
        {
            Debug.Log($"{npcName}: {questDialogue}");
            // Optionally, trigger a UI dialog box to display questDialogue
        }
        else if (questCompleted)
        {
            Debug.Log($"{npcName}: {questCompletedDialogue}");
            // Optionally, trigger a UI dialog box to display questCompletedDialogue
        }
        else
        {
            Debug.Log($"{npcName}: Hello there!");
        }
    }

    public void CompleteQuest(string itemGiven)
    {
        if (!hasQuest)
        {
            Debug.Log($"{npcName}: I have no tasks for you.");
            return;
        }

        if (itemGiven == questItemRequired)
        {
            questCompleted = true;
            Debug.Log($"{npcName}: Thank you for bringing me the {questItemRequired}. Quest complete!");
        }
        else
        {
            Debug.Log($"{npcName}: This isn't the item I asked for.");
        }
    }
}