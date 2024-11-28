using UnityEngine;

public class SitNPC : NPC
{
    private ChairScript[] chairs; // Array to store all chairs in the scene

    private void Start()
    {
        // Find all ChairScript objects in the scene
        chairs = FindObjectsOfType<ChairScript>();
    }

    protected override void OnQuestComplete()
    {
        // Check if there is at least one free chair
        ChairScript freeChair = FindFreeChair();
        if (freeChair != null)
        {
            Debug.Log($"{npcName}: Found a free chair - {freeChair.name}. Completing quest!");

            // Perform additional actions for completing the quest
            base.OnQuestComplete();

            // Optional: Assign the NPC to sit in the free chair
            SitInChair(freeChair);
        }
        else
        {
            Debug.Log($"{npcName}: No free chairs found. Quest cannot be completed.");
        }
    }

    private ChairScript FindFreeChair()
    {
        foreach (ChairScript chair in chairs)
        {
            if (chair.isFree)
            {
                return chair; // Return the first free chair found
            }
        }

        return null; // No free chairs available
    }

    private void SitInChair(ChairScript chair)
    {
        // Mark the chair as occupied
        chair.SetFree(false);

        // Move the NPC to the chair's position
        transform.position = chair.transform.position;

        // Optionally, play a sitting animation or adjust visuals here
        Debug.Log($"{npcName} is now sitting on the chair: {chair.name}.");
    }
}