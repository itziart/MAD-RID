using UnityEngine;
using System.Collections;

public class SitNPC : NPC
{
    private ChairScript[] chairs; // Array to hold all chair objects in the scene
    private SpriteRenderer spriteRenderer; // Cache the SpriteRenderer component for performance

    [Header("Sitting Configuration")]
    public GameObject npcPrefab; // The prefab of the NPC to be spawned sitting in the chair

    // Called when the object is initialized
    protected override void Start()
    {
        base.Start(); // Call the base class Start() method
        RefreshChairList(); // Populate the chair list at the start

        // Cache the SpriteRenderer component for easier access
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"No SpriteRenderer found on {gameObject.name}!"); // Log error if no SpriteRenderer is found
        }
    }

    // Override the base class Interact method to add custom logic
    public override void Interact()
    {
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned!"); // Ensure DialogManager is available
            return;
        }

        if (npcData == null)
        {
            Debug.LogError("npcData is missing on " + gameObject.name); // Ensure npcData is available
            return;
        }

        // Handle interaction based on quest status
        if (hasQuest && !questCompleted)
        {
            if (!questActive)
            {
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue); // Show quest dialogue
                questActive = true;
                return;
            }
            if (CheckQuestCondition()) // Check if the quest conditions are met
            {
                questCompleted = true; // Mark quest as completed
                OnQuestComplete(); // Handle quest completion
                if (isFinalQuest)
                {
                    levelManager.TriggerEndLevelDialogs(); // Trigger end level dialogs if it's the final quest
                }

                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue); // Show quest completed dialogue
            }
            else
            {
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue); // Show quest dialogue again if condition not met
            }
        }
        else if (questCompleted)
        {
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue); // Show completed quest dialogue
        }
        else
        {
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.defaultDialogue); // Show default dialogue
        }
    }

    // Override base method to check if the NPC can sit in a chair
    protected override bool CheckQuestCondition()
    {
        RefreshChairList(); // Refresh the list of chairs

        ChairScript freeChair = FindFreeChair(); // Find an available (free) chair
        if (freeChair != null)
        {
            StartCoroutine(HandleChairTransition(freeChair)); // Start the transition to sit in the chair
            return true;
        }

        return false; // No available chair found
    }

    // Refreshes the list of chairs in the scene
    private void RefreshChairList()
    {
        chairs = FindObjectsOfType<ChairScript>(); // Find all ChairScript objects in the scene
    }

    // Finds the first free chair in the list
    private ChairScript FindFreeChair()
    {
        foreach (ChairScript chair in chairs)
        {
            if (chair.isFree) // Check if the chair is free
            {
                return chair; // Return the first free chair found
            }
        }

        return null; // No free chair found
    }

    // Handles the transition of the NPC sitting in a chair
    private IEnumerator HandleChairTransition(ChairScript chair)
    {
        // Start fade-out effect using the DialogManager
        dialogManager.StartFadeOut();

        // Wait for the fade duration to complete
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // Move the NPC to sit in the chair and update its sprite
        SitInChair(chair);

        // Start fade-in effect after sitting down
        dialogManager.StartFadeIn();
    }

    // Positions the NPC in the chair and sets up necessary sorting
    private void SitInChair(ChairScript chair)
    {
        // Ensure npcPrefab is assigned before attempting to instantiate it
        if (npcPrefab == null)
        {
            Debug.LogError("NPC Prefab is not assigned!"); // Log error if npcPrefab is missing
            return;
        }

        // Calculate the correct position for the NPC relative to the chair
        Vector3 floorToSitOffset = new Vector3(-0.069f, 0.831f, 0f); // Offset to adjust the NPC's sitting position
        Vector3 sitPosition = chair.transform.position + floorToSitOffset;

        // Instantiate a new NPC instance sitting in the chair
        GameObject newNpcInstance = Instantiate(npcPrefab, sitPosition, Quaternion.identity);

        // Get the TilemapPositionSorting component from the chair to handle sorting order
        TilemapPositionSorting chairSorting = chair.GetComponent<TilemapPositionSorting>();
        if (chairSorting != null)
        {
            int chairSortingOrder = chairSorting.GetSortingOrder();
            int desiredSortingOrder = chairSortingOrder + 1; // The NPC should sit above the chair in the sorting order

            // If the chair has a custom sorting order, use that for the NPC
            if (chairSorting.customSortingOrder != 0)
            {
                desiredSortingOrder = chairSorting.customSortingOrder + 1;
            }

            // Ensure the sorting order is unique by checking for conflicts with other objects
            if (IsSortingOrderTaken(desiredSortingOrder))
            {
                int increment = 1;
                while (IsSortingOrderTaken(desiredSortingOrder + increment))
                {
                    increment++; // Increment the sorting order until an available slot is found
                }
                desiredSortingOrder += increment;
            }

            // Assign the sorting order to the new NPC instance
            TilemapPositionSorting newNPCSorting = newNpcInstance.GetComponent<TilemapPositionSorting>();
            newNPCSorting.customSortingOrder = desiredSortingOrder;
        }

        // Destroy the original NPC instance since it's now sitting
        Destroy(gameObject);

        Debug.Log($"{npcData.npcName} is now sitting on the chair: {chair.name}. New NPC position: {sitPosition}"); // Log the action
    }

    // Helper function to check if a sorting order is already taken by another object
    private bool IsSortingOrderTaken(int sortingOrder)
    {
        // Check all SpriteRenderers in the scene for conflicts in sorting order
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();
        foreach (var sprite in allSprites)
        {
            if (sprite.sortingOrder == sortingOrder)
            {
                return true; // Sorting order is already taken, return true
            }
        }
        return false; // Sorting order is free, return false
    }

    // Override the quest completion handler to make the NPC sit in a chair
    protected override IEnumerator HandleQuestCompletion()
    {
        // Show quest effect if any
        if (questEffectPrefab != null && effectSpawnLocation != null)
        {
            Instantiate(questEffectPrefab, effectSpawnLocation.position, Quaternion.identity);
        }

        // Start fade-out effect
        dialogManager.StartFadeOut();
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // Find a free chair and make the NPC sit
        ChairScript freeChair = FindFreeChair();
        if (freeChair != null)
        {
            SitInChair(freeChair); // Sit in the chair once it's found
            // Start fade-in effect
            dialogManager.StartFadeIn();
        }
        else
        {
            Debug.LogWarning($"{npcData.npcName} could not find a free chair!"); // Log warning if no free chair is found
        }
    }
}
