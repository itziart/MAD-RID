using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditScreenManager : MonoBehaviour
{
    public Button menubutton; // Reference to the button
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if (menubutton != null)
        {
            menubutton.onClick.AddListener(GoToMenu);  // Assign the GoToMenu method to the button
        }
    }

    // Method to Load the new scene using SceneManager
    private void GoToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    
}
