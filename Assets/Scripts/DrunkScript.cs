using UnityEngine;

public class DrunkNPC : NPC
{
    public GameObject vomitPrefab; // Vomit object to spawn
    public Transform vomitSpawnLocation; // Where vomit appears
    public GameObject chair; // Reference to the chair GameObject
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
            Instantiate(vomitPrefab, vomitSpawnLocation.position, Quaternion.identity);
        }

        if (chair != null)
        {
            chair.SetActive(false); // Hide the chair (or replace with a new sprite)
        }

        Debug.Log("Drunk NPC completed quest and vomited.");
    }
}