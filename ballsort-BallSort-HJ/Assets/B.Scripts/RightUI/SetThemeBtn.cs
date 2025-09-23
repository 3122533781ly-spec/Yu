using _02.Scripts.InGame.UI;
using Fangtang;
using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetThemeBtn : MonoBehaviour
{
    public Image icon;

    public GameObject selectedObj;

    public GameObject lockObj;

    private bool isLock;

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
                lockThemeSkin();
                if (!DialogManager.Instance.GetDialog<DressUpDialog>().themeView.lockTheme.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().themeView.lockTheme.Add(this);

                if (DialogManager.Instance.GetDialog<DressUpDialog>().themeView.unlockTheme.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().themeView.unlockTheme.Remove(this);
            }
            else
            {
                UnlockThemeSkin();
                if (DialogManager.Instance.GetDialog<DressUpDialog>().themeView.lockTheme.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().themeView.lockTheme.Remove(this);

                if (!DialogManager.Instance.GetDialog<DressUpDialog>().themeView.unlockTheme.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().themeView.unlockTheme.Add(this);

                PlayerPrefs.SetInt("ThemeSkin" + id, 1);
            }
        }
    }

    //初始化按钮，设置按钮图片
    public void Init(Sprite icon, int id)
    {
        this.id = id;
        this.icon.sprite = icon;
        if (PlayerPrefs.HasKey("ThemeSkin" + id) || id == 0)
            IsLock = false;
        else
            IsLock = true;
        toggle.group = transform.parent.GetComponent<UnityEngine.UI.ToggleGroup>();
    }

    // Start is called before the first frame update
    public void SetThemeSkin(bool isOn)
    {
        if (isOn)
        {
            selectedObj.SetActive(true);
            PlayerPrefs.SetInt("ClickThemeSkin", id);
            SpriteManager.Instance.SetThemeSkin(id);
        }
        else
        {
            selectedObj.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public void UnlockThemeSkin()
    {
        lockObj.SetActive(false);
        toggle.enabled = true;
    }

    // Start is called before the first frame update
    public void lockThemeSkin()
    {
        lockObj.SetActive(true);
        toggle.enabled = false;
    }
}