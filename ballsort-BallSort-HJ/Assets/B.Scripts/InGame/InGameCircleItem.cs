using System;
using _02.Scripts.Home.Active;
using _02.Scripts.InGame.UI;
using _02.Scripts.Util;
using DG.Tweening;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectSpace.WangZ.Scripts.InGame
{
    public class InGameCircleItem : ElementUI<global::InGame>
    {
        [SerializeField] private Image progressImage;
        [SerializeField] private FloatNumberDisplayer floatNumber;
        [SerializeField] private InGamePlayingUI inGamePlayingUI;

        //转盘已经转动次数
        private int _playedTimes = 0;

        private void Awake()
        {
            gameObject.SetActiveVirtual(Game.Instance.Model.IsWangZhuan() &&
                                        Game.Instance.LevelModel.CopiesType == CopiesType.SpecialLevel);
        }

        private void OnEnable()
        {
            progressImage.fillAmount = 0;
            _playedTimes = 0;
            floatNumber.ResetNumber(0);
            EventDispatcher.instance.Regist(AppEventType.FinishSpecialPipe, SetProgress);
        }

        private void OnDisable()
        {
            EventDispatcher.instance.UnRegist(AppEventType.FinishSpecialPipe, SetProgress);
        }

        private void SetProgress(object[] objs)
        {
            float data = (float) objs[0];
            floatNumber.Number += data;
            progressImage.DOFillAmount(floatNumber.Number, 0.5f);
            if (floatNumber.Number >= 1 && !Game.Instance.LevelModel.GetTrigger() && !_context.IsWin())
            {
                //打开大转盘弹窗，关闭时进度清空&&判断游戏是否结束,中途完成大转盘
                DialogManager.Instance.ShowDialogWithContext(DialogName.BigTurntableDialog,
                    new BigTurntableDialogContent
                    {
                        CloseAction = () =>
                        {
                            floatNumber.ResetNumber(0);
                            progressImage.fillAmount = 0;
                            inGamePlayingUI.ShowSlotItemHideBigTurn(true);
                            EventDispatcher.instance.DispatchEvent(AppEventType.NeedCheckPipeOver);
                        }
                    });
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DialogManager.Instance.ShowDialog(DialogName.BigTurntableDialog);
            }
        }
#endif

        /// <summary>
        /// 尝试获取需要完成的管子数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool TryGetBallToShow(out int count)
        {
            switch (_playedTimes)
            {
                // case 0:
                //     count = (int)ConstDataConfig.Instance.GetNumber(6);
                //     return true;
                // case 1:
                //     count = (int)ConstDataConfig.Instance.GetNumber(7);
                //     return true;
                // case 2:
                //     count = (int)ConstDataConfig.Instance.GetNumber(8);
                //     return true;
                default:
                    break;
            }

            count = 1000000;
            return false;
        }

        // /// <summary>
        // /// 展示转盘
        // /// </summary>
        // /// <param name="breakBallsForTurntable"></param>
        // /// <returns></returns>
        // public bool ShowTurntableWithCheck(int breakBallsForTurntable)
        // {
        //     if (TryGetBallToShow(out int count))
        //     {
        //         Debug.Log($"流程：大转盘，{breakBallsForTurntable} / {count}");
        //         progressImage.fillAmount = breakBallsForTurntable / (float) count;
        //         if (breakBallsForTurntable >= count)
        //         {
        //             _playedTimes++;
        //             DialogManager.Instance.GetDialog<BigTurntableDialog>().Show(() =>
        //             {
        //                 if (progressImage != null)
        //                 {
        //                     progressImage.fillAmount = 0;
        //                 }
        //             });
        //             return true;
        //         }
        //     }
        //     else
        //     {
        //         progressImage.fillAmount = 0;
        //     }
        //
        //     return false;
        // }
    }
}