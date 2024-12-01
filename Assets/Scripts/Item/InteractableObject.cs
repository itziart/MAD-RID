using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{

    /// Defines how this object interacts with a dropped item.
    /// <param name="itemData">The data of the dropped item.</param>
    /// <returns>True if the interaction was successful, otherwise false.</returns>
    public abstract bool Interact(ItemData itemData);
}