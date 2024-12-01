using UnityEngine;

public class Vomit : InteractableObject
{
    private Animator animator;
    public ChairScript chair;
    public override bool Interact(ItemData itemData)
    {
        if (itemData != null && itemData.itemName == "Mop")
        {
            if (chair != null)
            {
                chair.SetFree(true);
            }

            Debug.Log("Cleaned up vomit with the mop!");
            if (transform.parent != null)
            {
                animator = GetComponentInParent<Animator>();
                animator.SetTrigger("TRMopAnimation");
                //Destroy(transform.parent.gameObject); // Remove the parent GameObject from the scene
            }
            
            else
            {
                Debug.LogWarning("This object has no parent. Destroying the current object instead.");
                Destroy(gameObject); // Fallback in case there is no parent
            }
            return true;
        }

        Debug.Log("You need a mop to clean this up!");
        return false;
    }
}