using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public DialogManager dialogManager;

    [Header("Player Dialog Configuration")]
    public string welcomeDialog = "Welcome to the game! \nPress E or Click with your mouse to close the dialog boxes and advance!";
    public string startDialog = "Welcome to the level! Press E/Space/Enter/Mouse Click to get rid of the dialog boxes!";

    public string endDialog = "Those NPCs are nuts i swear to god... they are nuts!";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        TriggerStartLevelDialogs();
    }

    public void TriggerStartLevelDialogs()
    {
        dialogManager.ShowPlayerDialog(welcomeDialog);

        while (dialogManager.IsDialogActive())
        {
            Time.timeScale = 0f;
        }

        dialogManager.ShowPlayerDialog(startDialog);
    }

    public void TriggerEndLevelDialogs()
    {
        dialogManager.ShowPlayerDialog(endDialog);

    }
}
