using System.Collections;
using UnityEngine;

public class DrunkNPC : NPC
{
    public Sprite vomitingSprite;      // Sprite to display when the NPC is vomiting
    public GameObject vomitPrefab;    // Vomit object to spawn
    public Transform vomitSpawnLocation; // Where vomit appears
    public ChairScript chair;         // Reference to the chair GameObject
    public GameObject removedNPC;     // Reference to the removed NPC GameObject
    public int vomitSortingIndex;     // Sorting index of the vomit object
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnQuestComplete()
    {
        base.OnQuestComplete(); // Optionally call base behavior (e.g., play effects)

        // Change the NPC's sprite to the vomiting sprite
        if (vomitingSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = vomitingSprite;
        }

        // Start fade-out and handle quest completion visuals
        dialogManager.StartFadeOut();
        dialogManager.StartCoroutine(HandleVomitAfterFade());
    }

    private IEnumerator HandleVomitAfterFade()
    {
        // Wait for the fade-out to complete
        yield return new WaitForSeconds(dialogManager.fadeDuration);

        // If there was a selected NPC to remove (e.g., due to throwing up)
        if (removedNPC != null)
        {
            Destroy(removedNPC);
        }

        // Change the NPC's sprite to the vomiting sprite
        if (vomitingSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = vomitingSprite;
        }

        // Replace the chair with vomit
        if (vomitPrefab != null && vomitSpawnLocation != null)
        {
            Vector3 chairOffset = new Vector3(-0.8795f + 0.85f, 0.5978f, 0f);
            // Instantiate the vomit object at the specified location
            GameObject vomitInstance = Instantiate(vomitPrefab, vomitSpawnLocation.position+chairOffset, Quaternion.identity);

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

        // Start the fade-in process
        dialogManager.StartFadeIn();
    }

}
