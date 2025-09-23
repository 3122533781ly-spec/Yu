using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurntableGoodsItem : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private Text countText;
    [SerializeField]
    private float angle;

    private GoodsData goodsData;
    private int goodsCount = 0;

    // Start is called before the first frame update
    private void Start()
    {
    }

    /// <summary>
    /// 初始化物品
    /// </summary>
    /// <param name="data"></param>
    /// <param name="count"></param>
    public void Initialize(GoodsData data, int count = 0)
    {
        goodsData = data;
        goodsCount = count;
        iconImage.sprite = data.GetGoodTypeIcon();
        // iconImage.SetNativeSize();
        countText.text = count.ToString();
    }

    /// <summary>
    /// 获取物品
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public GoodsData GetGoodsData(out int count)
    {
        count = goodsCount;
        return goodsData;
    }

    public float GetAngle()
    {
        return angle;
    }
}
