using UnityEngine;

public class DrunkNPC : NPC
{
    public GameObject vomitPrefab; // Vomit object to spawn
    public Transform vomitSpawnLocation; // Where vomit appears
    public ChairScript chair; // Reference to the chair GameObject
    public GameObject removedNPC; // Reference to the removed NPC GameObject

    protected override void OnQuestComplete()
    {
        base.OnQuestComplete(); // Optionally call base behavior (e.g., play effects)

        // Remove the NPC
        Destroy(gameObject);
        if (removedNPC != null) // If there was a selected NPC to remove (due to throwing up)
        {
            Destroy(removedNPC);
        }

        // Replace the chair with vomit
        if (vomitPrefab != null && vomitSpawnLocation != null)
        {
            // Instantiate the vomit object at the specified location
            GameObject vomitInstance = Instantiate(vomitPrefab, vomitSpawnLocation.position, Quaternion.identity);

            // Assign the chair reference to the Vomit script
            Vomit vomitScript = vomitInstance.GetComponent<Vomit>();
            if (vomitScript != null && chair != null)
            {
                vomitScript.chair = chair; // Link the chair to the vomit
                Debug.Log($"Assigned chair '{chair.name}' to vomit.");
            }
            else
            {
                Debug.LogWarning("Failed to assign chair to vomit. Ensure vomitPrefab has a Vomit script and a chair is assigned.");
            }
        }

        Debug.Log("Drunk NPC completed quest and vomited.");
    }
}