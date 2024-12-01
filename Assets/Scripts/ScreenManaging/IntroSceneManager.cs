using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class IntroSceneManager : MonoBehaviour
{
    private Animator animator;
    // Function to change scene after animation finishes
    public void ChangeScene()
    {
        // Load the Start Screen Scene
        SceneManager.LoadScene("StartScreen");
    }

    void Start()
    {
        {
            animator = GetComponent<Animator>();    
            if (animator != null)
            {
                animator.SetTrigger("TRStartAnimation");
            }

        }
    }
}