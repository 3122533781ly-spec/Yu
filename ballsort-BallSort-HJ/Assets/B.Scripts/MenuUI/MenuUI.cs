using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    //�˵���ť
    public Button menuBtn; 
     

    // Start is called before the first frame update
    void Start()
    {
        menuBtn.onClick.AddListener(() => { ClickMenuBtn(); }); 
    }
 

  

    /// <summary>
    /// ���²˵���ť
    /// </summary>
    private void ClickMenuBtn()
    {
        DialogManager.Instance.GetDialog<OptionDialog>().ShowDialog();
        //OptionObj.SetActive(true);
    } 

    /// <summary>
    /// ���¹ر�ѡ����尴ť
    /// </summary>
    private void CloseMenuBtn()
    {
        DialogManager.Instance.GetDialog<OptionDialog>().CloseDialog();
        //OptionObj.SetActive(false);
    }  

}
