using System.Collections;
using System.Collections.Generic;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

public class DollarClearDataConfig : ScriptableConfigGroup<DollarClearData, DollarClearDataConfig>
{
    /// <summary>
    /// 获取当前能得到的美刀
    /// </summary>
    /// <returns></returns>
    public float GetAwardCash()
    {
        float amount = 0;
        var coin = Game.Instance.CurrencyModel.GetCurrentMoney();
        foreach (var item in All)
        {
            if (coin >= item.range.x && coin <= item.range.y)
            {
                amount = item.number;

                return amount;
            }
        }

        return 0.01f;
    }

    /// <summary>
    /// 收取当前的奖励
    /// </summary>
    /// <returns></returns>
    public float ClaimCurrentCash()
    {
        float count = GetAwardCash();
        Debug.Log($"获取金猪美刀：{count}");
        // Game.Instance.CurrencyModel.Money.Value += count;
        return count;
    }

    public bool IsCash()
    {
        return Random.Range(0, 1000) <= All[GetRange()].probability;
    }

    private int GetRange()
    {
        var coin = Game.Instance.CurrencyModel.GetCurrentMoney();
        int index = 0;
        foreach (var item in All)
        {
            if (coin >= item.range.x && coin <= item.range.y)
            {
                break;
            }

            index++;
        }

        return index;
    }

    /// <summary>
    /// 获取奖励的钻石
    /// </summary>
    /// <returns></returns>
    public int GetAwardGem()
    {
        int amount = 0;
        var coin = Game.Instance.CurrencyModel.Diamond.Value;
        foreach (var item in All)
        {
            if (coin >= item.gemRange.x && coin <= item.gemRange.y)
            {
                amount = item.gemNumber;
            }
        }

        return amount;
    }
}