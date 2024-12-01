using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public bool isGameplayScreen; // Set this in the Inspector for each scene

    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusicForScreen(isGameplayScreen); // Inform the MusicManager if a Gameplay Music or Menu Music should be played
        }
    }
}