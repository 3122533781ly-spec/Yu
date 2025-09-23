using System.Collections.Generic;

public class GoodsConfig : ScriptableConfigGroup<GoodsData, GoodsConfig>
{
    /// <summary>
    /// 获取所有指定类型的商品
    /// </summary>
    /// <param name="goodType"></param>
    /// <returns></returns>
    public List<GoodsData> FindAll(GoodType goodType)
    {
        return All.FindAll(ret => { return ret.Type == goodType; });
    }


    /// <summary>
    /// 获取所有指定类型的商品
    /// </summary>
    /// <param name="goodType"></param>
    /// <param name="subType"></param>
    /// <returns></returns>
    public GoodsData FindData(GoodType goodType, int subType)
    {
        return All.Find(ret => ret.Type == goodType && ret.subType == subType);
    }

    /// <summary>
    /// 获取所有极度缺少的碎片+钻石+美刀
    /// </summary>
    /// <returns></returns>
    public List<GoodsData> GetGoodsDatasLessAmount()
    {
        //var list = All.FindAll(ret => { return ret.Type == GoodType.GoodsChip && ret.GetGoodsNumToFull() > 3; });
        //list.AddRange(FindAll(GoodType.Gem));
        //list.AddRange(FindAll(GoodType.Money));
        //list.AddRange(FindAll(GoodType.Prop));
        return SlotMciDataConfig.Instance.GetAllGoodsData();
    }
}