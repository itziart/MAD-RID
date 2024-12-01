using UnityEngine;

public class CloseApplication : MonoBehaviour
{
    // Script to close the application using the Exit button
    public void QuitApplication()
    {
        Debug.Log("Application is quitting...");

        // Quits the application
        Application.Quit();

#if UNITY_EDITOR
        // If running in the Unity Editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
