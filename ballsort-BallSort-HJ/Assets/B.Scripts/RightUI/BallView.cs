using _02.Scripts.Config;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallView : MonoBehaviour
{
    public Transform content;

    public Text countText;

    public SetBallBtn setBallBtnObj;

    public List<Sprite> ballIcons = new List<Sprite>();

    public List<SetBallBtn> unlockBall = new List<SetBallBtn>();

    public List<SetBallBtn> lockBall = new List<SetBallBtn>();

    public int unLockValue;

    public GameObject adsObj;

    public GameObject getAllSkinObj;

    private ScrollRect ballScrollRect => GetComponent<ScrollRect>();

    public Text priceCoinText;

    public IEnumerator PopCloseSlide()
    {
        ballScrollRect.vertical = false;
        yield return new WaitForSeconds(0.15f);
        ballScrollRect.vertical = true;
    }

    // Start is called before the first frame update
    public void Init()
    {
        lockBall.Clear();
        unlockBall.Clear();

        for (int i = 0; i < ballIcons.Count; i++)
        {
            SetBallBtn temp = Instantiate(setBallBtnObj, content);
            temp.Init(ballIcons[i], i);

        }


        if (PlayerPrefs.HasKey("ClickBallSkin"))
            unlockBall.Find(b => b.id == PlayerPrefs.GetInt("ClickBallSkin")).toggle.isOn = true;
        else
            unlockBall[0].toggle.isOn = true;

        unLockValue = unlockBall.Count;
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
    public void InspectionQuantity()
    {
        if (!countText.gameObject.activeInHierarchy)
        {
            countText.SetActive(true);
        }
        unLockValue = unlockBall.Count;
        countText.text = string.Format(":{0}/{1}", unLockValue, ballIcons.Count);
        if (unlockBall.Count >= ballIcons.Count)
        {
            adsObj.SetActive(false);

            DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetAllTip(getAllSkinObj);

        }
        else
        {
            adsObj.SetActive(true);
             priceCoinText.text = ConstantConfig.Instance.GetBallPriceCoin().ToString();
        }
    }

    public void GetRdmSkin()
    {
        int rdmValue = Random.Range(0, lockBall.Count);
        DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetSkinUI(GoodType.SkinBall, lockBall[rdmValue].id);
        lockBall[rdmValue].IsLock = false;
        InspectionQuantity();
     
        print("随机一个球皮肤解锁");
    }

    public SetBallBtn GetRdmSkinData()
    {
        int rdmValue = Random.Range(0, lockBall.Count);
        return lockBall[rdmValue];
    }

    public void GetSkin(int id)
    {
        var lockObj = lockBall.Find(b => b.id == id);
        if (lockObj)
        {
            lockObj.IsLock = false;
            InspectionQuantity();
        }
    }



}
