using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    // Destroy the Vomit GameObject after cleaning.
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
