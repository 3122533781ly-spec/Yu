using Lei31.Localizetion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Redeem
{
    public class ConditionPanel : MonoBehaviour
    {
        [SerializeField] private Text textDes;

        [SerializeField] private Text textProgress1;
        [SerializeField] private Image conditionProgress1;

        [SerializeField] private GameObject progressPanel2;
        [SerializeField] private Text textProgress2;
        [SerializeField] private Image conditionProgress2;

        [SerializeField] private GameObject sharePanel;
        [SerializeField] private Text textLink;
        [SerializeField] private Button btnShare;
        [SerializeField] private Button btnCopyLink;

        public RedeemEntryBaseData Data { get; private set; }

        private bool hasAddAction = false;

        public void Show(RedeemEntryBaseData data)
        {
            Data = data;
            //MoneyRedeemData moneyRedeemData = data.GetMoneyRedeemData();

            RefreshUI();
            AddAction();
        }

        private void RefreshUI()
        {
            sharePanel.SetActive(Data.nowConditionProgressData.type == RedeemConditionType.InvitePlayer ||
                Data.nowConditionProgressData.type == RedeemConditionType.ActivePlayer);
            progressPanel2.SetActive(Data.nowConditionProgressData.type == RedeemConditionType.ActivePlayer);

            textProgress1.text = Data.nowConditionProgressData.GetProgressStr();
            conditionProgress1.fillAmount = Data.nowConditionProgressData.GetProgress();

            string desStr = "";
            switch (Data.nowConditionProgressData.type)
            {
                case RedeemConditionType.WatchAD:
                    desStr = $"{LocalizationManager.Instance.GetTextByTag(LocalizationConst.CONDITION_WATCH_AD, Data.nowConditionProgressData.needCount)}";
                    break;
                case RedeemConditionType.InvitePlayer:
                    textLink.text = Data.inviteLink;//赋值邀请链接
                    desStr = $"{LocalizationManager.Instance.GetTextByTag(LocalizationConst.CONDITION_INVITE_PLAYER, Data.nowConditionProgressData.needCount)}";
                    break;
                case RedeemConditionType.ActivePlayer:
                    textLink.text = Data.inviteLink;//赋值邀请链接
                    desStr = $"{LocalizationManager.Instance.GetTextByTag(LocalizationConst.CONDITION_ACTIVE_PLAYER, Data.nowConditionProgressData.needCount)}";

                    textProgress1.text = $"{LocalizationManager.Instance.GetTextByTag(LocalizationConst.TAG_ACTIVED)} {Data.nowConditionProgressData.GetProgressStr()}";

                    textProgress2.text = $"{LocalizationManager.Instance.GetTextByTag(LocalizationConst.TAG_INVITED)} {Data.nowConditionProgressData.invitePlayer}/{Data.nowConditionProgressData.needCount}";
                    conditionProgress2.fillAmount = (float)Data.nowConditionProgressData.invitePlayer / Data.nowConditionProgressData.needCount;
                    break;
                case RedeemConditionType.PlayGame:
                    desStr = $"{LocalizationManager.Instance.GetTextByTag(LocalizationConst.CONDITION_PLAY_GAME, Data.nowConditionProgressData.needCount)}";
                    break;
                case RedeemConditionType.LoginGame:
                    desStr = DataFormater.ToTimeString(Data.nowConditionProgressData.GetRemainTime(OnlineTimeManager.Instance.GetNowRealTime()));
                    break;
                default:
                    break;
            }
            textDes.text = desStr;
        }

        private void AddAction()
        {
            if (!hasAddAction)
            {
                hasAddAction = true;
                Data.onDataChanged += RefreshUI;
            }
        }
        private void RemoveAction()
        {
            if (hasAddAction)
            {
                hasAddAction = false;
                Data.onDataChanged -= RefreshUI;
            }
        }
        private void OnEnable()
        {
            OnlineTimeManager.Instance.OnlineTimeChangeAction += HandleOnlineTimeChanged;
            btnShare.onClick.AddListener(ClickShare);
            btnCopyLink.onClick.AddListener(ClickCopyLink);
        }

        private void OnDisable()
        {
            if (!OnlineTimeManager.IsQuiting)
            {
                OnlineTimeManager.Instance.OnlineTimeChangeAction -= HandleOnlineTimeChanged;
                btnShare.onClick.RemoveListener(ClickShare);
                btnCopyLink.onClick.RemoveListener(ClickCopyLink);
                RemoveAction();
            }
        }

        private void HandleOnlineTimeChanged()
        {
            if (Data.nowConditionProgressData.type == RedeemConditionType.LoginGame)
            {
                int remainTime = Data.nowConditionProgressData.GetRemainTime(OnlineTimeManager.Instance.GetNowRealTime());
                if (remainTime >= 0)
                {
                    textDes.text = DataFormater.ToTimeString(remainTime);
                }
                else
                {
                    Data.EnterNextCondition();
                }

            }
        }

        private void ClickShare()
        {
            SDKManager.NativeShare(Data.inviteLink);
            StaticModule.RedeemShare(Data.GetRedeemData().money);
        }
        private void ClickCopyLink()
        {
            UnityEngine.GUIUtility.systemCopyBuffer = Data.inviteLink;
        }
    }
}
