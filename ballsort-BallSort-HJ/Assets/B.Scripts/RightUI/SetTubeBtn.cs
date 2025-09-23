using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections;
using System.Collections.Generic;
using _02.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class SetTubeBtn : MonoBehaviour
{
    public Image icon;

    public GameObject selectedObj;

    public GameObject lockObj;

    bool isLock;

    public Toggle toggle;

    public int id;

    public bool IsLock
    {
        get => isLock;
        set
        {
            isLock = value;
            if (isLock)
            {
                lockTubeSkin();
                if (!DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.lockTube.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.lockTube.Add(this);

                if (DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.unlockTube.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.unlockTube.Remove(this);
            }
            else
            {
              
                UnlockTubeSkin();
                if (DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.lockTube.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.lockTube.Remove(this);

                if (!DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.unlockTube.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().tubeView.unlockTube.Add(this);

                PlayerPrefs.SetInt("TubeSkin" + id, 1);
            }
        }
    }

    //初始化按钮，设置按钮图片
    public void Init(Sprite icon, int id)
    {
        this.id = id;
        this.icon.sprite = icon;
        if (PlayerPrefs.HasKey("TubeSkin" + id) || id == 0)
            IsLock = false;
        else
            IsLock = true;
        toggle.group = transform.parent.GetComponent<UnityEngine.UI.ToggleGroup>();
    }

    // Start is called before the first frame update
    public void SetTubeSkin(bool isOn)
    {
        if (isOn)
        {
            selectedObj.SetActive(true);
            // SpriteManager.Instance.SetTubeSkin(id);
            PlayerPrefs.SetInt("ClickTubeSkin", id);
            EventDispatcher.instance.DispatchEvent(AppEventType.PlayerPipeSkinChange);
        }
        else
        {
            selectedObj.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public void UnlockTubeSkin()
    {
        lockObj.SetActive(false);
        toggle.enabled = true;
    }

    // Start is called before the first frame update
    public void lockTubeSkin()
    {
        lockObj.SetActive(true);
        toggle.enabled = false;
    }

 
}
