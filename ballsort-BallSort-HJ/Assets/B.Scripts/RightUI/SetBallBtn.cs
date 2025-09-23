using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SetBallBtn : MonoBehaviour
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
                lockBallSkin();
                if (!DialogManager.Instance.GetDialog<DressUpDialog>().ballView.lockBall.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().ballView.lockBall.Add(this);

                if (DialogManager.Instance.GetDialog<DressUpDialog>().ballView.unlockBall.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().ballView.unlockBall.Remove(this);
            }
            else
            {
                UnlockBallSkin();
                if (DialogManager.Instance.GetDialog<DressUpDialog>().ballView.lockBall.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().ballView.lockBall.Remove(this);

                if (!DialogManager.Instance.GetDialog<DressUpDialog>().ballView.unlockBall.Contains(this))
                    DialogManager.Instance.GetDialog<DressUpDialog>().ballView.unlockBall.Add(this);

                PlayerPrefs.SetInt("BallSkin" + id, 1);
            }
        }
    }

    //��ʼ����ť�����ð�ťͼƬ
    public void Init(Sprite icon, int id)
    {
        this.id = id;
        this.icon.sprite = icon;
        if (PlayerPrefs.HasKey("BallSkin" + id) || id == 0)
            IsLock = false;
        else
            IsLock = true;
        toggle.group = transform.parent.GetComponent<UnityEngine.UI.ToggleGroup>();
    }

    // Start is called before the first frame update
    public void SetBallSkin(bool isOn)
    {
        if (isOn)
        {
            selectedObj.SetActive(true);
            PlayerPrefs.SetInt("ClickBallSkin", id);
            SpriteManager.Instance.SetBallSkin(id);
        }
        else
        {
            selectedObj.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public void UnlockBallSkin()
    {
        lockObj.SetActive(false);
        toggle.enabled = true;
    }

    // Start is called before the first frame update
    public void lockBallSkin()
    {
        lockObj.SetActive(true);
        toggle.enabled = false;
    }
}