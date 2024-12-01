using System.Collections;
using UnityEngine;

public class DrunkNPC : NPC
{
    public Sprite vomitingSprite;      // Sprite to display when the NPC is vomiting
    public Sprite vomitingPortrait;    // Portrait to display when the NPC is vomiting
    public GameObject vomitPrefab;     // Vomit object to spawn
    public Transform vomitSpawnLocation; // Where vomit appears
    public ChairScript chair;          // Reference to the chair GameObject
    public GameObject removedNPC;      // Reference to the removed NPC GameObject
    public int vomitSortingIndex;      // Sorting index of the vomit object
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    private Sprite originalPortrait;   // Store the original portrait to avoid overwriting it
    public AudioSource soundFX;        // Reference to the sound effect


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the original portrait at the start to restore later if needed
        if (npcData != null)
        {
            originalPortrait = npcData.npcPortrait;
        }
    }

    // This function gets called when the player completes the quest
    protected override void OnQuestComplete()
    {
        // Temporarily set the vomiting portrait for the NPC
        if (vomitingPortrait != null)
        {
            npcData.npcPortrait = vomitingPortrait;
        }

        // Change the NPC's sprite to the vomiting sprite
        if (vomitingSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = vomitingSprite;
        }

        // Start the fade-out and handle the quest completion visuals
        dialogManager.StartFadeOut();
        dialogManager.StartCoroutine(HandleQuestCompletionAndVomit());
    }

    private IEnumerator HandleQuestCompletionAndVomit()
    {
        soundFX.Play();
        // Wait for the fade-out to complete
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // Remove the NPC if needed (e.g., if they vomit and should be removed)
        if (removedNPC != null)
        {
            Destroy(removedNPC);
        }

        // Spawn vomit at the chair location
        if (vomitPrefab != null && vomitSpawnLocation != null)
        {
            Vector3 chairOffset = new Vector3(-0.8795f + 0.85f, 0.5978f, 0f);
            // Instantiate the vomit object at the specified location
            GameObject vomitInstance = Instantiate(vomitPrefab, vomitSpawnLocation.position + chairOffset, Quaternion.identity);

            // Find the Vomit script in the child GameObject "DropHitbox"
            Transform dropHitboxTransform = vomitInstance.transform.Find("DropHitbox");
            TilemapPositionSorting vomiPositionSorting = vomitInstance.GetComponent<TilemapPositionSorting>();

            if (dropHitboxTransform != null)
            {
                Vomit vomitScript = dropHitboxTransform.GetComponent<Vomit>();

                if (vomitScript != null && chair != null)
                {
                    vomiPositionSorting.customSortingOrder = vomitSortingIndex;
                    vomitScript.chair = chair; // Link the chair to the vomit

                    Debug.Log($"Assigned chair '{chair.name}' to vomit.");
                }
                else
                {
                    Debug.LogWarning("Failed to assign chair to vomit. Ensure DropHitbox has a Vomit script and a chair is assigned.");
                }
            }
            else
            {
                Debug.LogWarning("DropHitbox child not found in vomitPrefab. Ensure the prefab has a correctly named DropHitbox child.");
            }
        }

        Debug.Log("Drunk NPC completed quest and vomited.");

        // Display the "Quest Completed" dialog
        dialogManager.ShowPlayerDialog("Disgusting... But somehow worked...");

        // Restore the original portrait after the quest is complete
        RestoreOriginalPortrait();

        // Start the fade-in process
        dialogManager.StartFadeIn();
    }

    private void RestoreOriginalPortrait()
    {
        // Restore the original portrait back to the NPC data
        if (npcData != null && originalPortrait != null)
        {
            npcData.npcPortrait = originalPortrait;
        }
    }
}
