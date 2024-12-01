using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "NPC/NPC Data")] // Create a new NPCData asset
public class NPCData : ScriptableObject
{
    // Class to store all the NPC Data

    public string npcName; // The NPC's name
    public Sprite npcPortrait; // Portrait for dialog display
    public string questDialogue; // Dialogue shown during active quest
    public string questCompletedDialogue; // Dialogue shown after quest completion
    public string defaultDialogue = "Hello there!"; // Default greeting
}