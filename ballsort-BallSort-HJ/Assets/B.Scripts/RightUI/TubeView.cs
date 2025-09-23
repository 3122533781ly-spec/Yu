using _02.Scripts.Config;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TubeView : MonoBehaviour
{
    public Transform content;

    public Text countText;

    public SetTubeBtn setTubeBtnObj;

    public List<Sprite> tubeIcons = new List<Sprite>();

    public List<SetTubeBtn> unlockTube = new List<SetTubeBtn>();

    public List<SetTubeBtn> lockTube = new List<SetTubeBtn>();

    public int unLockValue;

    public GameObject adsObj;

    public GameObject getAllSkinObj;

    private ScrollRect tubeScrollRect => GetComponent<ScrollRect>();

    public Text priceCoinText;

    public IEnumerator PopCloseSlide()
    {
        tubeScrollRect.vertical = false;
        yield return new WaitForSeconds(0.15f);
        tubeScrollRect.vertical = true;
    }

    // Start is called before the first frame update
    public void Init()
    {
        //lockTube.Clear();
        //unlockTube.Clear();

        //for (int i = 0; i < tubeIcons.Count; i++)
        //{
        //    SetTubeBtn temp = Instantiate(setTubeBtnObj, content);
        //    temp.Init(tubeIcons[i], i);
        //}

        //unLockValue = unlockTube.Count;
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
        //if (!countText.gameObject.activeInHierarchy)
        //{
        //    countText.SetActive(true);
        //}

        //unLockValue = unlockTube.Count;
        //countText.text = string.Format(":{0}/{1}", unLockValue, tubeIcons.Count);
        //if (unlockTube.Count >= tubeIcons.Count)
        //{
        //    adsObj.SetActive(false);
        //    DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetAllTip(getAllSkinObj);
        //}
        //else
        //{
        //    adsObj.SetActive(true);
        //    priceCoinText.text = ConstantConfig.Instance.GetTubePriceCoin().ToString();
        //}
    }

    public void GetRdmSkin()
    {
        int rdmValue = Random.Range(0, lockTube.Count);
        DialogManager.Instance.GetDialog<DressUpDialog>().ShowGetSkinUI(GoodType.SkinTube, lockTube[rdmValue].id);
        lockTube[rdmValue].IsLock = false;
        InspectionQuantity();
        print("随机一个管皮肤解锁");
    }

    //public SetTubeBtn GetRdmSkinData()
    //{
    //    //int rdmValue = Random.Range(0, lockTube.Count);
    //    //return lockTube[rdmValue];
    //}

    public void GetSkin(int id)
    {
        //    var lockObj = lockTube.Find(b => b.id == id);
        //    if (lockObj)
        //    {
        //        lockTube.Find(b => b.id == id).IsLock = false;
        //        InspectionQuantity();
        //    }
        //}
    }
}