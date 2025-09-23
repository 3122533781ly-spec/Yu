using UnityEngine;
using UnityEngine.Serialization;

namespace SoyBean.IAP
{
    public class IAPConfig : ScriptableConfigGroup<IAPItemData, IAPConfig>
    {
        [SerializeField] public IAPItemData SavingPotData;

        [FormerlySerializedAs("InfiniteManualData")] [SerializeField]
        public IAPItemData InfiniteManual15Data;

        public IAPItemData GetRemoveADProduct()
        {
            for (int i = 0; i < All.Count; i++)
            {
                if (All[i].Rewards.Count == 1)
                {
                    GoodsData reward = GoodsConfig.Instance.GetConfigByID((int) All[i].Rewards[0].goodType);
                    if (reward.Type == GoodType.RemoveAD)
                    {
                        return All[i];
                    }
                }
            }

            return null;
        }

        private void OnEnable()
        {
        }
    }
}