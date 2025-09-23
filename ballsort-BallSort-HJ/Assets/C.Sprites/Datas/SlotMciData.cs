using System;
using _02.Scripts.Util;

[Serializable]
public class SlotMciData : IConfig
{
    public int id;

    //物品ID
    public int goodsId;
    public GoodType goodType;
    public int goodSubtype;

    //概率
    public int probability;
    public int count;

    //奖励金额(N（填0.1代表10%）；算法，所得金额=（100-金猪当前美元值）×N)
    public float rateAmount;

    public int poolId;

    public int ID => id;

    /// <summary>
    /// 获取物品
    /// </summary>
    /// <returns></returns>
    public GoodsData GetGoodsData()
    {
        if (goodsId != -1)
        {
            return GoodsConfig.Instance.All.Find(x =>
                x.Type == goodType && x.subType == goodSubtype && x.IntID == goodsId);
        }

        return null;
    }
}