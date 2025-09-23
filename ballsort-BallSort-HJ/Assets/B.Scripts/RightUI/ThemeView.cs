using _02.Scripts.Config;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeView : MonoBehaviour
{
    public Transform content;

    public Text countText;

    public SetThemeBtn setThemeBtnObj;

    public List<Sprite> themeIcons = new List<Sprite>();

    public List<SetThemeBtn> unlockTheme = new List<SetThemeBtn>();

    public List<SetThemeBtn> lockTheme = new List<SetThemeBtn>();

    public int unLockValue;

    public GameObject adsObj;

    public GameObject getAllSkinObj;

    public Text priceCoinText;

    private ScrollRect themeScrollRect => GetComponent<ScrollRect>();

    public IEnumerator PopCloseSlide()
    {
        themeScrollRect.vertical = false;
        yield return new WaitForSeconds(0.15f);
        themeScrollRect.vertical = true;
    }

    public void Init()
    {
        lockTheme.Clear();
        unlockTheme.Clear();

        for (int i = 0; i < themeIcons.Count; i++)
        {
            SetThemeBtn temp = Instantiate(setThemeBtnObj, content);
            temp.Init(themeIcons[i], i);
        }

        //if (PlayerPrefs.HasKey("ClickThemeSkin"))
        //    unlockTheme.Find(t => t.id == PlayerPrefs.GetInt("ClickThemeSkin")).toggle.isOn = true;
        //else
        //    unlockTheme[0].toggle.isOn = true;

        unLockValue = unlockTheme.Count;
    }

    private void OnEnable()
    {
        InspectionQuantity();
    }

    private void OnDisable()
    {
        getAllSkinObj.SetActive(false);
    }

    //计数
    private void InspectionQuantity()
    {
        if (!countText.gameObject.activeInHierarchy)
        {
            countText.SetActive(true);
        }
        unLockValue = unlockTheme.Count;
        countText.text = string.Format(":{0}/{1}", unLockValue, themeIcons.Count);

        if (unlockTheme.Count >= themeIcons.Count)
        {
            adsObj.SetActive(false);
            DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetAllTip(getAllSkinObj);
        }
        else
        {
            adsObj.SetActive(true);
            priceCoinText.text = ConstantConfig.Instance.GetThemePriceCoin().ToString();
        }
    }

    public void GetRdmSkin()
    {
        int rdmValue = Random.Range(0, lockTheme.Count);
        DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetSkinUI(GoodType.SkinTheme, lockTheme[rdmValue].id);
        Debug.Log("背景Id" + lockTheme[rdmValue].id);
        lockTheme[rdmValue].IsLock = false;
        InspectionQuantity();
        print("随机一个背景皮肤解锁");
    }

    public SetThemeBtn GetRdmSkinData()
    {
        int rdmValue = Random.Range(0, lockTheme.Count);
        return lockTheme[rdmValue];
    }

    public void GetSkin(int id)
    {
        var lockObj = lockTheme.Find(b => b.id == id);
        if (lockObj)
        {
            lockTheme.Find(b => b.id == id).IsLock = false;
            InspectionQuantity();
        }
    }
}