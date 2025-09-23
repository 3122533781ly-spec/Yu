using System.Collections.Generic;
using _02.Scripts.Config;
using _02.Scripts.Home.Active;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using _02.Scripts.InGame.Controller;
using _02.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using UnityEngine;
using UnityEngine.UI;
using System;

using _02.Scripts.Home.Active;

namespace _02.Scripts.InGame.UI
{
    public class InGamePlayingUI : ElementUI<global::InGame>
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private Button btnback;
        [SerializeField] private Button btnSetting;
        [SerializeField] private Button skinEntryBtn;
        [SerializeField] private Button restartSetting;
        [SerializeField] private Button revocationTool;
        [SerializeField] private Button addNewPipeTool;

        // [SerializeField] private DialogButtonItem removeAdActive;
        [SerializeField] private SmartLocalizedText levelTxt;

        [SerializeField] private Text addPipeTxt;
        [SerializeField] private Text revocationToolTxt;
        [SerializeField] private Image revocationToolAdIcon;
        [SerializeField] private Image addPipeToolAdIcon;
        [SerializeField] private List<GameObject> redeemBtn;

        private void OnEnable()
        {
            if (Game.Instance.Model.IsWangZhuan())
            {
                btnback.SetActive(false);
                btnSetting.gameObject.SetActive(true);
                skinEntryBtn.gameObject.SetActive(true);
            }
            btnSetting.onClick.AddListener(ClickSetting);
            btnback.onClick.AddListener(ClickBack);
            skinEntryBtn.onClick.AddListener(OpenSkinMainUI);
            revocationTool.onClick.AddListener(RevocationTool);
            addNewPipeTool.onClick.AddListener(AddNewPipe);
            SetBackground(false);
            Game.Instance.CurrencyModel.RegisterToolChangeAction(GoodType.Tool, GoodSubType.AddPipe, RefreshUI);
            Game.Instance.CurrencyModel.RegisterToolChangeAction(GoodType.Tool, GoodSubType.RevocationTool, RefreshUI);

            Game.Instance.Model.CanRedeem.OnValueChange += HandleCanRedeemChange;
            EventDispatcher.instance.Regist(AppEventType.PlayerStepCountChange, RefreshUI);
        }

        private void OnDisable()
        {
            btnSetting.onClick.RemoveListener(ClickSetting);
            btnback.onClick.RemoveListener(ClickBack);
            skinEntryBtn.onClick.RemoveListener(OpenSkinMainUI);
            revocationTool.onClick.RemoveListener(RevocationTool);
            addNewPipeTool.onClick.RemoveListener(AddNewPipe);
            // removeAdDialog.onClick.RemoveListener(ShowRemoveAdDialog);

            Game.Instance.Model.CanRedeem.OnValueChange -= HandleCanRedeemChange;
            EventDispatcher.instance.UnRegist(AppEventType.PlayerStepCountChange, RefreshUI);
            Game.Instance.CurrencyModel.UnregisterToolChangeAction(GoodType.Tool, GoodSubType.AddPipe, RefreshUI);
            Game.Instance.CurrencyModel.UnregisterToolChangeAction(GoodType.Tool, GoodSubType.RevocationTool, RefreshUI);
        }

        private void RefreshUI(int arg1, int arg2)
        {
            RefreshUI();
        }

        private void RefreshUI(object[] objs)
        {
            RefreshUI();
        }

        private void ClickSetting()
        {
            DialogManager.Instance.GetDialog<OptionDialog>().ShowDialog();
        }

        private void ClickBack()
        {
            TransitionManager.Instance.Transition(0.5f, () => { SceneManager.LoadScene("InGame"); },
                   0.5f);
        }

        public void OpenSkinMainUI()
        {
            DialogManager.Instance.GetDialog<DressUpDialog>().ShowDialog();
            //  DialogManager.Instance.GetDialog<DressUpDialog>().SetBtnState(skinEntryBtn, false);
        }

        public void SetTimeActive(bool b)
        {
        }

        public void SetTime(int secound)
        {
        }

        public void SetBackground(bool b)
        {
            background.SetActive(b);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public void Restart()
        {
            if (Game.Instance.LevelModel.CopiesType == CopiesType.Thread)
            {
                var enterId = Game.Instance.LevelModel.EnterLevelID;
                if (enterId == default)
                {
                    enterId = Game.Instance.LevelModel.MaxUnlockLevel.Value;
                }

                Game.Instance.RestartGame("RestartCurrentLevel", enterId,
                    forceShowAd: true);
            }
            else if (Game.Instance.LevelModel.CopiesType == CopiesType.SpecialLevel)
            {
                Game.Instance.RestartGame("RestartCurrentLevel", Game.Instance.LevelModel.EnterCopies1ID,
                    CopiesType.SpecialLevel, forceShowAd: true);
            }
        }

        #region ADS

        /// <summary>
        /// 插屏广告
        /// </summary>
        // private void ShowIntersistal()
        // {
        //     if (ADMudule.IsInterstitialReady())
        //     {
        //         ADMudule.ShowInterstitialAds("Win",
        //             _ => { App.Instance.RestartGame(App.Instance.LevelModel.EnterLevelID); });
        //     }
        // }

        #endregion ADS

        private void RevocationTool()
        {
            if (Game.Instance.CurrencyModel.CanUseTool(GoodType.Tool, GoodSubType.RevocationTool))
            {
                Context.GetController<InGameMatchController>().RevocationTool();
            }
            else
            {
                ADMudule.ShowRewardedAd("WatchAd_GetRevocationTool", (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        RewardClaimHandle.ClaimReward(
                            new RewardData(GoodType.Tool, 5, (int)GoodSubType.RevocationTool),
                            "InGame", IapStatus.Free);
                    }
                });
            }
        }

        private void AddNewPipe()
        {
            if (Game.Instance.CurrencyModel.CanUseTool(GoodType.Tool, GoodSubType.AddPipe))
            {
                Context.GetController<InGameMapController>().AddNewPipe(() =>
                {
                    Game.Instance.CurrencyModel.ConsumeGoodNumber(GoodType.Tool, (int)GoodSubType.AddPipe, 1);
                });
            }
            else
            {
                ADMudule.ShowRewardedAd("WatchAd_GetNewPipe", (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        RewardClaimHandle.ClaimReward(new RewardData(GoodType.Tool, 1, (int)GoodSubType.AddPipe),
                            "InGame", IapStatus.Free);
                    }
                });
            }
        }

        public void RefreshUI()
        {
            levelTxt.text = Game.Instance.LevelModel.CopiesType == CopiesType.Thread
                ? $"Level {Game.Instance.LevelModel.EnterLevelID}"
                : "SPECIAL LEVEL";

            var addPipeToolCount = Game.Instance.CurrencyModel.GetGoodNumber(GoodType.Tool, GoodSubType.AddPipe);
            var revocationToolCount =
                Game.Instance.CurrencyModel.GetGoodNumber(GoodType.Tool, GoodSubType.RevocationTool);

            addPipeTxt.text = $"{addPipeToolCount}";
            revocationToolTxt.text = $"{revocationToolCount}";

            //if (addPipeToolCount < 1)
            //    revocationToolTxt.SetActive(false);
            //else revocationToolTxt.SetActive(true);
            addPipeToolAdIcon.SetActiveVirtual(addPipeToolCount <= 0);
            revocationToolAdIcon.SetActiveVirtual(revocationToolCount <= 0);

            revocationToolAdIcon.color = UtilClass.HexToColor(!RevocationToolOtherJug() ? "#4F4F4F" : "#FFFFFF");
            addPipeToolAdIcon.color = UtilClass.HexToColor(!AddPipeOtherJug() ? "#4F4F4F" : "#FFFFFF");

            addNewPipeTool.enabled = AddPipeOtherJug();
            addNewPipeTool.interactable = AddPipeOtherJug();
            revocationTool.enabled = RevocationToolOtherJug();
            revocationTool.interactable = RevocationToolOtherJug();
        }

        //大于5管并且数量足够
        private bool AddPipeOtherJug()
        {
            if (Context.GetModel<InGameModel>().CanAddPipe())
                return false;

            return Context.CellMapModel.LevelPipeList.Count > 5;

            // &&
            // Game.Instance.CurrencyModel.CanUseTool(GoodType.Tool, GoodSubType.AddPipe);
        }

        //数量足够并且有步数
        private bool RevocationToolOtherJug()
        {
            return Context.GetController<InGameMatchController>().GetPlayerStep().Count > 0 ||
                   !Game.Instance.CurrencyModel.CanUseTool(GoodType.Tool, GoodSubType.RevocationTool);
            // &&
            // Game.Instance.CurrencyModel.CanUseTool(GoodType.Tool, GoodSubType.AddPipe);
        }

        private void HandleCanRedeemChange(bool oldValue, bool newValue)
        {
            foreach (var item in redeemBtn)
            {
                item.SetActiveVirtual(false);
            }

          //  _context.GetView<FristInGameReward>().CheckIsShow();
        }

        public void ShowSlotItemHideBigTurn(bool showSlot)
        {
            //老虎机按钮
            redeemBtn[0].SetActiveVirtual(showSlot);
            //大转盘图标
            redeemBtn[2].SetActiveVirtual(!showSlot);
        }
    }
}