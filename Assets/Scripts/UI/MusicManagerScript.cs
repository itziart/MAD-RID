using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("MusicManager requires an AudioSource component!");
        }
    }

    /// <summary>
    /// Changes the music based on the screen type.
    /// </summary>
    public void PlayMusicForScreen(bool isGameplay)
    {
        AudioClip targetClip = isGameplay ? gameplayMusic : menuMusic;

        if (audioSource.clip == targetClip && audioSource.isPlaying)
            return; // Music already playing, no need to change

        audioSource.clip = targetClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}