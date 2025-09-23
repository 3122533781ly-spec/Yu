using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DressUpOptionBtn : MonoBehaviour
{
    public GameObject controlObject;

    public List<Sprite> bgImages = new List<Sprite>();

    public Image bgImage;

    public List<Sprite> iconImages = new List<Sprite>();

    public Image iconImage;

    public GameObject topNameText;

    public string btnName;

    public bool firstShow;

    private void Start()
    {
        if (firstShow)
        {
            GetComponent<Toggle>().isOn = true;
            ClickDressUpOptionBtn(true);
        }
    }

    public void ClickDressUpOptionBtn(bool isOn)
    {
        if (isOn)
        {
            AudioClipHelper.Instance.PlaySkinButtonTap();
            if (DressUpDialog.nowTopName != null)
                DressUpDialog.nowTopName.SetActive(false);

            topNameText.SetActive(true);
            DressUpDialog.nowTopName = topNameText;

            


            controlObject.SetActive(true);
            bgImage.sprite = bgImages[0];
            iconImage.sprite = iconImages[0];
        }
        else
        {
            controlObject.SetActive(false);
            bgImage.sprite = bgImages[1];
            iconImage.sprite = iconImages[1];
        }
    }
}
