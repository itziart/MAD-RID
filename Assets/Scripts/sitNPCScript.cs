using UnityEngine;

public class SitNPC : NPC
{
    private ChairScript[] chairs;

    protected override void Start()
    {
        base.Start();
        RefreshChairList();
    }

    public override void Interact()
    {
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned!");
            return;
        }

        if (npcData == null)
        {
            Debug.LogError("npcData is missing on " + gameObject.name);
            return;
        }

        if (hasQuest && !questCompleted)
        {
            if (!questActive)
            {
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue);
                questActive = true;
                return;
            }
            if (CheckQuestCondition())
            {
                questCompleted = true;
                OnQuestComplete();
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue);
            }
            else
            {
                dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questDialogue);
            }
        }
        else if (questCompleted)
        {
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.questCompletedDialogue);
        }
        else
        {
            dialogManager.ShowDialog(npcData.npcPortrait, npcData.npcName, npcData.defaultDialogue);
        }
    }


    protected override bool CheckQuestCondition()
    {
        RefreshChairList();

        ChairScript freeChair = FindFreeChair();
        if (freeChair != null)
        {
            SitInChair(freeChair);
            return true;
        }

        return false;
    }

    private void RefreshChairList()
    {
        chairs = FindObjectsOfType<ChairScript>();
    }

    private ChairScript FindFreeChair()
    {
        foreach (ChairScript chair in chairs)
        {
            if (chair.isFree)
            {
                return chair;
            }
        }

        return null;
    }

    private void SitInChair(ChairScript chair)
    {
        chair.SetFree(false);
        transform.position = chair.transform.position;
        Debug.Log($"{npcData.npcName} is now sitting on the chair: {chair.name}.");
    }
}
