using Lei31.Localizetion;
using System.Collections;
using System.Collections.Generic;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;
using UnityEngine.UI;

namespace Redeem
{
    public class RedeemBaseEntry : MonoBehaviour
    {
        [SerializeField] private Text textRewardTargetValue;

        [SerializeField] private UnderwayPanel underwayPanel;
        [SerializeField] private ConditionPanel conditionPanel;
        [SerializeField] private CanConfirmPanel canConfirmPanel;
        [SerializeField] private GameObject underReviewPanel;
        [SerializeField] private RejectedPanel rejectedPanel;
        [SerializeField] private GameObject approvedPanel;
        [SerializeField] private GameObject finishRedeemPanel;

        public RedeemEntryBaseData Data { get; private set; }
        private bool hasAddAction = false;

        public virtual void Show(RedeemEntryBaseData data)
        {
            Data = data;
            hasAddAction = false;
            RedeemConfigBaseData redeemData = data.GetRedeemData();
            textRewardTargetValue.text = $"${redeemData.money}";

            AddAction();
            RefreshUI();
        }

        private void RefreshUI()
        {
            RedeemConfigBaseData redeemData = Data.GetRedeemData();

            bool canShowUnderway = Data.state == RedeemEntryState.Underway || Data.state == RedeemEntryState.CanApplyRedeem;
            underwayPanel.gameObject.SetActive(canShowUnderway);
            if (canShowUnderway)
            {
                underwayPanel.Init(HandleApplyRedeem);
                string progressStr = Data.type == RedeemEntryType.PayPal ?
                    $"${Game.Instance.CurrencyModel.GetCurrentMoney():0.00}/${redeemData.money}" :
                    $"{Game.Instance.CurrencyModel.Diamond.Value}/{((GiftCardRedeemData)redeemData).diamond}";
                underwayPanel.Show(progressStr, Data.state);
            }

            conditionPanel.gameObject.SetActive(Data.state == RedeemEntryState.NeedFinishCondition);
            if (Data.state == RedeemEntryState.NeedFinishCondition)
            {
                conditionPanel.Show(Data);
            }

            canConfirmPanel.gameObject.SetActive(Data.state == RedeemEntryState.CanConfirm);
            if (Data.state == RedeemEntryState.CanConfirm)
            {
                canConfirmPanel.Init(HandleConfirmRedeem);
            }

            underReviewPanel.SetActive(Data.state == RedeemEntryState.UnderReview);

            rejectedPanel.gameObject.SetActive(Data.state == RedeemEntryState.Rejected);
            if (Data.state == RedeemEntryState.Rejected)
            {
                //显示拒绝原因
                rejectedPanel.Init(HandleAgain);
                rejectedPanel.Show($"{LocalizationManager.Instance.GetTextByTag(LocalizationConst.BE_REJECTED)},{((PayPalEntryData)Data).serverNote}");
            }
            approvedPanel.SetActive(Data.state == RedeemEntryState.Approved);
            finishRedeemPanel.SetActive(Data.state == RedeemEntryState.FinishRedeem);
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            RemoveAction();
        }

        private void AddAction()
        {
            if (!hasAddAction)
            {
                hasAddAction = true;
                Data.onStateChanged += HandleStateChanged;
            }
        }

        private void RemoveAction()
        {
            if (hasAddAction)
            {
                hasAddAction = false;
                Data.onStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged()
        {
            RefreshUI();
        }

        private void HandleApplyRedeem()
        {
            Game.Instance.GetSystem<RedeemSystem>().HandleApplyRedeem(Data);
        }

        private void HandleConfirmRedeem()
        {
            Game.Instance.GetSystem<RedeemSystem>().HandleConfirmRedeem(Data);
        }

        /// <summary>
        /// 被拒绝后选择again 
        /// </summary>
        private void HandleAgain()
        {
            Game.Instance.GetSystem<RedeemSystem>().HandleAgain(Data);
        }
    }
}

