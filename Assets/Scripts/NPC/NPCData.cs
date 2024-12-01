using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName; // The NPC's name
    public Sprite npcPortrait; // Portrait for dialog display
    public string questDialogue; // Dialogue shown during active quest
    public string questCompletedDialogue; // Dialogue shown after quest completion
    public string defaultDialogue = "Hello there!"; // Default greeting
}