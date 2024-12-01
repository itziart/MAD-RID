using UnityEngine;

public class NpcDropHandler : InteractableObject
{
    public NPC npc; // Reference to the associated NPC
    public DialogManager dialogManager;

    public override bool Interact(ItemData itemData)
    {
        if (npc == null || dialogManager == null)
        {
            Debug.Log("NpcDropHandler: Invalid NPC or DialogManager reference.");
            return false;
        }

        if (itemData == null)
        {
            Debug.Log("NpcDropHandler: No item data provided.");
            ShowDialog("Don't give me that!");
            return false;
        }

        // Check if the item completes the NPC's quest
        if (npc.TryCompleteQuest(itemData))
        {
            Debug.Log($"NpcDropHandler: Quest completed with the item '{itemData.itemName}'!");
            return true;
        }

        // Item doesn't complete the quest
        ShowDialog("Don't give me that!");
        return false;
    }

    private void ShowDialog(string dialog)
    {
        if (dialogManager != null && npc != null)
        {
            dialogManager.ShowDialog(npc.npcData.npcPortrait, npc.npcData.npcName, dialog);
        }
    }
}