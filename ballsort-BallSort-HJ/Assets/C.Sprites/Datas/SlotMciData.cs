using System;
using _02.Scripts.Util;

[Serializable]
public class SlotMciData : IConfig
{
    public int id;

    //��ƷID
    public int goodsId;
    public GoodType goodType;
    public int goodSubtype;

    //����
    public int probability;
    public int count;

    //�������(N����0.1����10%�����㷨�����ý��=��100-����ǰ��Ԫֵ����N)
    public float rateAmount;

    public int poolId;

    public int ID => id;

    /// <summary>
    /// ��ȡ��Ʒ
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