using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
public class TaskDialog : Dialog
{
    private QuestConfig quest;
    [SerializeField] private Text descriptionLabel;
    // Start is called before the first frame update
    public void InitDialog(QuestConfig questconfig)
    {
        quest = questconfig;
        base.ShowDialog();
        RefreshDes();


    }

    private void RefreshDes()
    {
        switch (quest.questType)
        {
            case QuestType.Completebowlplating:
            {
                    descriptionLabel.text = quest.description;
                    break;
                }
            case QuestType.Completeplateplating:
            {
                    descriptionLabel.text = quest.description;
                    break;
                }
            case QuestType.Completecupplating:
            {
                    descriptionLabel.text = quest.description;
                    break;
                }
            case QuestType.Completechopsticksplating:
                {
                    descriptionLabel.text = quest.description;
                    break;
                }
            case QuestType.Completeknifeplating:
                {
                    descriptionLabel.text = quest.description;
                    break;
                }
        }
    }


}
