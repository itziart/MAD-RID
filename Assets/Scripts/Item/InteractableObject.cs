using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    // Abstract class that defines how an object can be interacted with.
    // NPCs and Items use this class to implement the interactions functionality

    /// Class that Defines how this object interacts with a dropped item.
    /// <param name="itemData"> The data of the dropped item.</param>
    /// <returns>True if the interaction was successful, otherwise false.</returns>
    public abstract bool Interact(ItemData itemData);
}