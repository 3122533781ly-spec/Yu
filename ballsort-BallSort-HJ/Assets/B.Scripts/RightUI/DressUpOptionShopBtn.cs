using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DressUpOptionShopBtn : MonoBehaviour
{
    

    public GameObject controlObject;

    public GameObject LockControlObject;

    public List<Sprite> bgImages = new List<Sprite>();

    public Image bgImage;

    public List<Sprite> iconImages = new List<Sprite>();

    public GameObject countText;

    public Image iconImage;

    public bool isLock;

    public GameObject topNameText;

    public string btnName;

    public GameObject downUI;

    public void ClickDressUpOptionBtn(bool isOn)
    {
        if (isOn)
        {
            AudioClipHelper.Instance.PlaySkinButtonTap();
            if (DressUpDialog.nowTopName != null)
                DressUpDialog.nowTopName.SetActive(false);

            topNameText.SetActive(true);
            DressUpDialog.nowTopName = topNameText;

            downUI.SetActive(false);
            bgImage.sprite = bgImages[0];
            iconImage.sprite = iconImages[0];
            countText.SetActive(false);
            if (!isLock)
            {
                controlObject.SetActive(true);
            }
            else
            {
                LockControlObject.SetActive(true);
            }
        }
        else
        {
            downUI.SetActive(true);
            bgImage.sprite = bgImages[1];
            iconImage.sprite = iconImages[1];
            countText.SetActive(true);
            if (!isLock)
            {
                controlObject.SetActive(false);
            }
            else
            {
                LockControlObject.SetActive(false);
            }

        }
    }
}
