using UnityEngine;

// Script to handle interactions where items are given to an NPC to complete a quest
public class NpcDropHandler : InteractableObject
{
    public NPC npc; // Reference to the NPC this handler is associated with
    public DialogManager dialogManager; // Reference to the dialog manager for showing dialogs

    // This function is called when the player interacts with this object
    public override bool Interact(ItemData itemData)
    {
        // Ensure the NPC and dialog manager references are valid
        if (npc == null || dialogManager == null)
        {
            Debug.Log("NpcDropHandler: Invalid NPC or DialogManager reference."); // Log a warning for missing references
            return false; // Interaction fails due to invalid setup
        }

        // Handle the case where no item data is provided during the interaction
        if (itemData == null)
        {
            Debug.Log("NpcDropHandler: No item data provided."); // Log the absence of item data
            ShowDialog("Don't give me that!"); // Show a default dialog to the player
            return false; // Interaction does not succeed
        }

        // Attempt to complete the NPC's quest with the provided item
        if (npc.TryCompleteQuest(itemData))
        {
            Debug.Log($"NpcDropHandler: Quest completed with the item '{itemData.itemName}'!"); // Log quest completion success
            return true; // Interaction succeeds because the quest is completed
        }

        // If the item does not complete the quest, show a rejection dialog
        ShowDialog("Don't give me that!"); // Inform the player that the item was incorrect
        return false; // Interaction fails as the item doesn't meet quest requirements
    }

    // Displays a dialog box with NPC's portrait, name, and the provided message
    private void ShowDialog(string dialog)
    {
        if (dialogManager != null && npc != null) // Ensure dialog manager and NPC references are valid
        {
            dialogManager.ShowDialog(npc.npcData.npcPortrait, npc.npcData.npcName, dialog); // Display the dialog with NPC-specific visuals and message
        }
    }
}
