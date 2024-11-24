using UnityEngine;
using UnityEngine.EventSystems;

public class NpcDropHandler : MonoBehaviour, IDropHandler
{
    public NPC npc; // Reference to the NPC script on this GameObject

    public void OnDrop(PointerEventData eventData)
    {
        // Get the dragged item
        DragHandler draggedItem = eventData.pointerDrag.GetComponent<DragHandler>();
        if (draggedItem != null && draggedItem.itemData != null)
        {
            // Check if the dropped item completes the quest
            if (npc != null)
            {
                bool questCompleted = npc.TryCompleteQuest(draggedItem.itemData);

                if (questCompleted)
                {
                    Debug.Log("Quest completed!");
                }
                else
                {
                    Debug.Log("This item is not what the NPC needs.");
                }
            }
        }
    }
}
