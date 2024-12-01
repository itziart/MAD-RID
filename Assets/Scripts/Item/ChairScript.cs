using UnityEngine;

public class ChairScript : MonoBehaviour
{ 
    // Class that handles the chair script
    
    public bool isFree = false; // Stores if the chair is free

    // Method to set the chair as free
    public void SetFree(bool free)
    {
        isFree = free;
    }
}
