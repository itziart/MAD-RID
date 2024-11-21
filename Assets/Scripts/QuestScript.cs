using UnityEngine;

public class GameQuest : MonoBehaviour
{
    public string questName;              // Name of the quest
    public string description;            // Description of the quest
    public GameObject requiredItem;       // Item needed to complete the quest
    public GameObject rewardItem;         // Reward for completing the quest
    public bool isCompleted = false;      // Completion status

    public GameQuest(string questName, string description, GameObject requiredItem, GameObject rewardItem)
    {
        this.questName = questName;
        this.description = description;
        this.requiredItem = requiredItem;
        this.rewardItem = rewardItem;
    }

    public bool CheckIfCompleted(Player player)
    {
        // Check if the player has the required item
        if (requiredItem != null && player.inventory.HasItem(requiredItem))
        {
            return true;
        }
        return false;
    }

    public void CompleteQuest(Player player)
    {
        if (CheckIfCompleted(player))
        {
            isCompleted = true;

            // Remove the required item from the player's inventory
            player.inventory.RemoveItem(requiredItem);

            // Reward the player
            if (rewardItem != null)
            {
                player.inventory.AddItem(rewardItem);
                Debug.Log($"Quest '{questName}' completed! You received a {rewardItem.name}.");
            }
            else
            {
                Debug.Log($"Quest '{questName}' completed!");
            }
        }
        else
        {
            Debug.Log($"Quest '{questName}' cannot be completed yet. You need a {requiredItem.name}.");
        }
    }
}