using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditScreenManager : MonoBehaviour
{
    public Button menubutton; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if (menubutton != null)
        {
            menubutton.onClick.AddListener(GoToMenu); 
        }
    }
    private void GoToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
    // Update is called once per frame
    
}
