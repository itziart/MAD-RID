using UnityEngine;

public class NpcDropHandler : MonoBehaviour
{
    public NPC npc; // Reference to the NPC script on this GameObject

    // Method to handle the item being dropped on the NPC's drop hitbox
    public bool TryDropItem(ItemData itemData)
    {
        // If the NPC is null or doesn't require an item, return false
        if (npc == null || itemData == null)
        {
            Debug.Log("Invalid item or NPC reference.");
            return false;
        }

        // Check if the dropped item is the one the NPC needs
        bool questCompleted = npc.TryCompleteQuest(itemData);

        if (questCompleted)
        {
            Debug.Log("Quest completed with the dropped item.");
            return true;
        }
        else
        {
            Debug.Log("This is not the correct item for the NPC.");
            return false;
        }
    }
}