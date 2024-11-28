using UnityEngine;

public class NpcDropHandler : InteractableObject
{
    public NPC npc; // Reference to the associated NPC
    public DialogManager dialogManager;

    public override bool Interact(ItemData itemData)
    {
        if (npc == null || itemData == null)
        {
            Debug.Log("Invalid NPC or item data.");
            return false;
        }

        // Check if the item completes the NPC's quest
        if (npc.TryCompleteQuest(itemData))
        {
            Debug.Log("Quest completed with the dropped item!");
            return true;
        }

        dialogManager.ShowDialog($"{npc.npcName}: Don't give me that!");
        return false;
    }
}