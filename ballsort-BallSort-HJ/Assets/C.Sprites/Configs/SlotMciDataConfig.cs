using System.Collections.Generic;
using _02.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;

public class SlotMciDataConfig : ScriptableConfigGroup<SlotMciData, SlotMciDataConfig>
{
    public List<SlotMciData> GetPool()
    {
        var res = All.FindAll(x => x.poolId == 1);
        if (Game.Instance.CurrencyModel.GetCurrentMoney() >= 99)
        {
            res = All.FindAll(x => x.poolId == 2);
        }

        return res;
    }

    /// <summary>
    /// ��ȡ�������е���Ʒ
    /// </summary>
    /// <returns></returns>
    public SlotMciData GetHitSlotMciData()
    {
        var num = 0;
        var list = new List<int>();
        foreach (var item in GetPool())
        {
            list.Add(item.probability);
            num += item.probability;
        }

        var it = UtilClass.GetProbitIndex(list, num);
        return GetPool()[it];
    }

    /// <summary>
    /// ��ȡ�ϻ�������ʾ��������Ʒ
    /// </summary>
    /// <returns></returns>
    public List<GoodsData> GetAllGoodsData()
    {
        var rt = new List<GoodsData>();
        foreach (var item in GetPool())
        {
            if ((int) item.goodsId == -1)
            {
                continue;
            }

            if (GoodsConfig.Instance.TryGetConfigByID((int) item.goodsId, out GoodsData data))
            {
                rt.Add(data);
            }
        }

        return rt;
    }

    /// <summary>
    /// �Ƿ�������ϻ�������
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool IsSlotGoods(GoodsData data)
    {
        bool yes = false;
        switch (data.Type)
        {
            case GoodType.Prop:
            case GoodType.Gem:
            case GoodType.Money:
                yes = true;
                break;
            default:
                break;
        }

        return yes;
    }

    /// <summary>
    /// ��ȡ��Ʒ��RateAmount
    /// </summary>
    /// <param name="goodsId"></param>
    /// <returns></returns>
    public float GetRateAmount(int goodsId)
    {
        var item = GetPool().FindAll(v => (int) v.goodsId == goodsId);
        if (item == null || item.Count <= 0)
        {
            return 0f;
        }

        return item.ListRandom().rateAmount;
    }
}