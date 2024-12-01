using System.Collections;
using UnityEngine;

// Script to handle the behavior of a drunk NPC, including vomiting and quest completion
public class DrunkNPC : NPC
{
    public Sprite vomitingSprite;      // Sprite to display when the NPC is vomiting
    public Sprite vomitingPortrait;   // Portrait to display in dialogs when the NPC is vomiting
    public GameObject vomitPrefab;    // Prefab for the vomit object to spawn
    public Transform vomitSpawnLocation; // Location where vomit will appear
    public ChairScript chair;         // Reference to the associated chair GameObject
    public GameObject removedNPC;     // Reference to the NPC GameObject to remove upon vomiting
    public int vomitSortingIndex;     // Sorting index for the vomit object
    private SpriteRenderer spriteRenderer; // Reference to the NPC's SpriteRenderer component

    private Sprite originalPortrait;  // The NPC's original portrait, stored for restoration
    public AudioSource soundFX;       // Reference to the sound effect played when vomiting occurs

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Save the original portrait for later restoration, ensuring npcData is valid
        if (npcData != null)
        {
            originalPortrait = npcData.npcPortrait;
        }
    }

    // Handles the quest completion logic when triggered
    protected override void OnQuestComplete()
    {
        // Set the vomiting portrait for dialogs if it exists
        if (vomitingPortrait != null)
        {
            npcData.npcPortrait = vomitingPortrait;
        }

        // Change the NPC's sprite to the vomiting sprite for visual representation
        if (vomitingSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = vomitingSprite;
        }

        // Start the fade-out effect and initiate quest completion handling
        dialogManager.StartFadeOut();
        dialogManager.StartCoroutine(HandleQuestCompletionAndVomit());
    }

    private IEnumerator HandleQuestCompletionAndVomit()
    {
        soundFX.Play(); // Play the vomiting sound effect

        // Wait for the fade-out duration to complete
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // Remove the NPC GameObject if specified (e.g., they leave after vomiting)
        if (removedNPC != null)
        {
            Destroy(removedNPC);
        }

        // Spawn a vomit object at the designated location
        if (vomitPrefab != null && vomitSpawnLocation != null)
        {
            Vector3 chairOffset = new Vector3(-0.8795f + 0.85f, 0.5978f, 0f); // Adjusted spawn offset
            GameObject vomitInstance = Instantiate(vomitPrefab, vomitSpawnLocation.position + chairOffset, Quaternion.identity);

            // Locate the child GameObject "DropHitbox" within the vomit prefab
            Transform dropHitboxTransform = vomitInstance.transform.Find("DropHitbox");
            TilemapPositionSorting vomiPositionSorting = vomitInstance.GetComponent<TilemapPositionSorting>();

            // If the DropHitbox and chair exist, link the vomit to the chair
            if (dropHitboxTransform != null)
            {
                Vomit vomitScript = dropHitboxTransform.GetComponent<Vomit>();

                if (vomitScript != null && chair != null)
                {
                    vomiPositionSorting.customSortingOrder = vomitSortingIndex; // Set sorting order
                    vomitScript.chair = chair; // Assign the chair to the vomit instance

                    Debug.Log($"Assigned chair '{chair.name}' to vomit.");
                }
                else
                {
                    // Log a warning if the setup is incomplete or components are missing
                    Debug.LogWarning("Failed to assign chair to vomit. Ensure DropHitbox has a Vomit script and a chair is assigned.");
                }
            }
            else
            {
                // Log a warning if the DropHitbox child is not found
                Debug.LogWarning("DropHitbox child not found in vomitPrefab. Ensure the prefab has a correctly named DropHitbox child.");
            }
        }

        Debug.Log("Drunk NPC completed quest and vomited.");

        // Display a Player Dialog after completion of the quest
        dialogManager.ShowPlayerDialog("Disgusting... But somehow worked...");

        // Restore the NPC's original portrait for consistency after the quest
        RestoreOriginalPortrait();

        // Start the fade-in effect to resume normal gameplay visuals
        dialogManager.StartFadeIn();
    }

    private void RestoreOriginalPortrait()
    {
        // Restore the NPC's original portrait if both the data and portrait are valid
        if (npcData != null && originalPortrait != null)
        {
            npcData.npcPortrait = originalPortrait;
        }
    }
}
