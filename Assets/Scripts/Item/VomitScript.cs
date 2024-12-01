using UnityEngine;

// Script to handle interactions with vomit objects in the game world
public class Vomit : InteractableObject
{
    private Animator animator; // Animator component for playing cleaning animations
    public ChairScript chair; // Reference to the associated chair (if any) to free it upon cleaning

    // Overrides the Interact method from the InteractableObject base class
    public override bool Interact(ItemData itemData)
    {
        // Check if the player is using the correct item (a mop)
        if (itemData != null && itemData.itemName == "Mop")
        {
            // If the vomit is associated with a chair, free the chair
            if (chair != null)
            {
                chair.SetFree(true);
            }

            Debug.Log("Cleaned up vomit with the mop!");

            // Check if the object has a parent
            if (transform.parent != null)
            {
                // Trigger the mop animation in the parent's Animator component
                animator = GetComponentInParent<Animator>();
                animator.SetTrigger("TRMopAnimation");
            }
            else
            {
                // If no parent exists, log a warning and destroy the vomit object directly
                Debug.LogWarning("This object has no parent. Destroying the current object instead.");
                Destroy(gameObject); // Cleanup fallback
            }

            return true; // Indicate successful interaction
        }

        // Inform the player that a mop is required to clean the vomit
        Debug.Log("You need a mop to clean this up!");
        return false; // Indicate unsuccessful interaction
    }

}