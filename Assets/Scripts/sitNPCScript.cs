using UnityEngine;
using System.Collections;

public class SitNPC : NPC
{
    private ChairScript[] chairs;
    private SpriteRenderer spriteRenderer; // Cache the SpriteRenderer component


    [Header("Sitting Configuration")]
    public GameObject npcPrefab; // The prefab of the NPC to be spawned sitting in chair



    protected override void Start()
    {
        base.Start();
        RefreshChairList();

        // Cache the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"No SpriteRenderer found on {gameObject.name}!");
        }
    }

    public override void Interact()
    {
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned!");
            return;
        }

        if (npcData == null)
        {
            Debug.LogError("npcData is missing on " + gameObject.name);
            return;
        }

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
                OnQuestComplete();
                levelManager.TriggerEndLevelDialogs();
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue);
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

    protected override bool CheckQuestCondition()
    {
        RefreshChairList();

        ChairScript freeChair = FindFreeChair();
        if (freeChair != null)
        {
            StartCoroutine(HandleChairTransition(freeChair));
            return true;
        }

        return false;
    }

    private void RefreshChairList()
    {
        chairs = FindObjectsOfType<ChairScript>();
    }

    private ChairScript FindFreeChair()
    {
        foreach (ChairScript chair in chairs)
        {
            if (chair.isFree)
            {
                return chair;
            }
        }

        return null;
    }

    private IEnumerator HandleChairTransition(ChairScript chair)
    {
        // Start fade-out using DialogManager
        dialogManager.StartFadeOut();

        // Wait for the fade duration
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // Perform the movement and sprite change
        SitInChair(chair);

        // Start fade-in using DialogManager
        dialogManager.StartFadeIn();
    }

    private void SitInChair(ChairScript chair)
    {
        // Ensure npcPrefab is assigned
        if (npcPrefab == null)
        {
            Debug.LogError("NPC Prefab is not assigned!");
            return;
        }

        // Create a new NPC instance at the chair's position with the correct offset
        Vector3 floorToSitOffset = new Vector3(-0.069f, 0.831f, 0f); // Distance from the Chair position to the NPC correct sitting position
        Vector3 sitPosition = chair.transform.position + floorToSitOffset;

        GameObject newNpcInstance = Instantiate(npcPrefab, sitPosition, Quaternion.identity);


        // Now set the sorting order for the new NPC instance
        TilemapPositionSorting chairSorting = chair.GetComponent<TilemapPositionSorting>();
        if (chairSorting != null)
        {
            int chairSortingOrder = chairSorting.GetSortingOrder();
            int desiredSortingOrder = chairSortingOrder + 1;

            // If the chair has a customSortingOrder set, use it for the NPC
            if (chairSorting.customSortingOrder != 0)
            {
                desiredSortingOrder = chairSorting.customSortingOrder + 1;
            }

            // Check if sorting order is taken and increment if necessary
            if (IsSortingOrderTaken(desiredSortingOrder))
            {
                int increment = 1;
                while (IsSortingOrderTaken(desiredSortingOrder + increment))
                {
                    increment++;
                }
                desiredSortingOrder += increment;
            }

            TilemapPositionSorting newNPCSorting = newNpcInstance.GetComponent<TilemapPositionSorting>();
            newNPCSorting.customSortingOrder=desiredSortingOrder;
        }

        // Optionally disable the old NPC (if needed)
        Destroy(gameObject);  // Remove the original NPC instance

        Debug.Log($"{npcData.npcName} is now sitting on the chair: {chair.name}. New NPC position: {sitPosition}");
    }

    // Helper function to check if a sorting order is already taken
    private bool IsSortingOrderTaken(int sortingOrder)
    {
        // Check all objects in the scene to see if any has the same sorting order
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();
        foreach (var sprite in allSprites)
        {
            if (sprite.sortingOrder == sortingOrder)
            {
                return true;
            }
        }
        return false;
    }





    protected override IEnumerator HandleQuestCompletion()
    {
        // Show quest effect if any
        if (questEffectPrefab != null && effectSpawnLocation != null)
        {
            Instantiate(questEffectPrefab, effectSpawnLocation.position, Quaternion.identity);
        }

        // Start fade-out
        dialogManager.StartFadeOut();
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // Move to chair location and update sprite
        ChairScript freeChair = FindFreeChair();
        if (freeChair != null)
        {
            SitInChair(freeChair);
            // Start fade-in
            dialogManager.StartFadeIn();
        }
        else
        {
            Debug.LogWarning($"{npcData.npcName} could not find a free chair!");
        }

    }

}
