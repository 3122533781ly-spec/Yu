using System;
using System.Collections.Generic;
using _02.Scripts.InGame.UI;
using _02.Scripts.Util;
using DG.Tweening;
using Fangtang;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;
using EventDispatcher = _02.Scripts.Util.EventDispatcher;

namespace _02.Scripts.InGame.Controller
{
    public class InGameMatchController : ElementBehavior<global::InGame>
    {
        [SerializeField] public RectTransform empty;

        // 可配置的下落速度参数
        [SerializeField, Range(0.1f, 2f)] private float dropSpeed = 0.5f;

        private InGameBallUI _popBallUI;
        private InGamePipeUI _popPipeUI;
        private InGamePipeUI _pushPipeUI;

        //玩家操作信息，key弹出，value推进
        private Stack<List<InGamePipeUI>> _playerStep =
            new Stack<List<InGamePipeUI>>();

        private bool _isPushOnAnime;
        private bool _isPopOnAnime;
        private bool _isCoercion;
        private bool _isStartTwoAnime;
        public bool _isDropAnime;

        public void ClickPipe(InGamePipeUI clickPipeUI, bool isPlaySound = true)
        {
            if (!_isStartTwoAnime)
            {
                if (_isPushOnAnime || _isPopOnAnime)
                {
                    return;
                }
            }

            if (isPlaySound)
            {
                AudioClipHelper.Instance.PlaySound(AudioClipEnum.ClickPipe, 0.94f);
                //VibrationManager.Instance.SelectedBlockImpact();
            }

            if (_popBallUI)
            {
                if (clickPipeUI.CanPushBallLimit(_popBallUI.GetBallData()) || clickPipeUI == _popPipeUI || _isCoercion)
                {
                    _pushPipeUI = clickPipeUI;
                    SetPushIsAnime(true);
                    PushBall(_popPipeUI, clickPipeUI);
                }
                else
                {
                    _isStartTwoAnime = true;
                    ClickPipe(_popPipeUI, false); //放下
                    _pushPipeUI = clickPipeUI;
                    ClickPipe(clickPipeUI, false); //拿起一个新的
                    // Debug.LogError("clickOtherPipe");
                }
            }
            else
            {
                _popPipeUI = clickPipeUI;
                PopBall(_popPipeUI);
            }
        }

        private void PopBall(InGamePipeUI inGamePipeUI)
        {
            //如果当前球正在下落，并点击弹出，则直接弹出
            var polBall = PopBallAndFly(inGamePipeUI);
            if (polBall)
            {
                SetPopIsAnime(true);
                polBall.StopPushAnime();
                _popBallUI = polBall;
            }
        }

        private void PushBall(InGamePipeUI popPipeUI, InGamePipeUI inGamePipeUI)
        {
            PushBallAndFly(popPipeUI, inGamePipeUI, _popBallUI);
            _popBallUI = null;
        }

        public void Win()
        {
            Context.Win();
        }

        public void CleanAllData()
        {
            _popPipeUI = null;
            _pushPipeUI = null;
            _popPipeUI = null;
            _playerStep = new Stack<List<InGamePipeUI>>();
            _isPushOnAnime = false;
            _isPopOnAnime = false;
            _isCoercion = false;
            _isStartTwoAnime = false;
        }

        public Stack<List<InGamePipeUI>> GetPlayerStep()
        {
            return _playerStep;
        }

        #region Anime

        private void SetPushIsAnime(bool setAnime)
        {
            _isPushOnAnime = setAnime;
        }

        private void SetIsDropAnime(bool setAnime)
        {
            _isDropAnime = setAnime;
        }

        private void SetPopIsAnime(bool setAnime)
        {
            _isPopOnAnime = setAnime;
        }

        /// <summary>
        /// 添加一颗球,并且飞过去
        /// </summary>
        /// <param name="inGamePipeUI"></param>
        /// <param name="pushPipeUI"></param>
        /// <param name="ballUI"></param>
        private void PushBallAndFly(InGamePipeUI inGamePipeUI, InGamePipeUI pushPipeUI, InGameBallUI ballUI)
        {
            var timeScale = 1;
            var emptyRectTransform = pushPipeUI.GetAndInitPushToPos();
            var popPipe = inGamePipeUI;
            pushPipeUI.PushBall(ballUI);

            // 正确设置动画状态
            SetPushIsAnime(true);

            FlyToTopPos(pushPipeUI, ballUI, () =>
            {
                SetIsDropAnime(true);
                var sequence = DOTween.Sequence();
                ballUI.SetPushAnime(sequence);
                var bottomPosition = emptyRectTransform.position;

                // 删除错误的状态重置
                // _context.GetController<InGameMatchController>().SetPushIsAnime(false);

                if (_popPipeUI != pushPipeUI && !_isStartTwoAnime)
                {
                    _popPipeUI.CheckTop();
                }

                // 使用 dropSpeed 控制下落速度
                sequence.Append(ballUI.transform.DOMove(bottomPosition, dropSpeed * timeScale))
                    .OnComplete(() =>
                    {
                        ballUI.transform.SetParent(pushPipeUI.gridLayoutGroup.transform);
                        ballUI.SetPushAnime(null);
                        pushPipeUI.TriggerFullEff();
                        Context.CheckIsOver();
                        AddPlayStep(popPipe, pushPipeUI);
                        _isCoercion = false;
                        _isStartTwoAnime = false;
                        SetIsDropAnime(false);
                        SetPushIsAnime(false); // 在动画真正完成后重置状态
                    })
                    .OnKill(() =>
                    {
                        ballUI.SetPushAnime(null);
                        pushPipeUI.TriggerFullEff();
                        Context.CheckIsOver();
                        SetIsDropAnime(false);
                        SetPushIsAnime(false); // 在动画被取消时也重置状态
                    });
            });
        }

        private InGameBallUI PopBallAndFly(InGamePipeUI inGamePipeUI)
        {
            var popBall = _popPipeUI.PopBall();
            if (popBall)
            {
                _popPipeUI.ControllerEmptyList();
                FlyToTopPos(inGamePipeUI, popBall, () => { SetPopIsAnime(false); });
            }

            return popBall;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inGamePipeUI"></param>
        /// <param name="popBall"></param>
        /// <param name="callBack"></param>
        private void FlyToTopPos(InGamePipeUI inGamePipeUI, InGameBallUI popBall, Action callBack = null)
        {
            popBall.transform.SetParent(inGamePipeUI.popToPos);
            var sequence = DOTween.Sequence();
            sequence.Append(popBall.transform.DOMove(inGamePipeUI.popToPos.position, 0.1f));
            sequence.OnComplete(() => { callBack?.Invoke(); });
            sequence.OnKill(() =>
            {
                if (_isStartTwoAnime)
                {
                    _isStartTwoAnime = false;
                }
            });
        }

        #endregion Anime

        #region ToolLogic

        private bool BallIsFlyToPipe()
        {
            if (_popBallUI)
            {
                var pos = _popBallUI.transform.parent.name;
                return pos == "FlyToPos";
            }

            return false;
        }

        public bool CanUseTool()
        {
            return _playerStep.Count > 0 && !_isPopOnAnime && !_isStartTwoAnime && !_isPushOnAnime && !_isCoercion &&
                   !BallIsFlyToPipe();
        }

        /// <summary>
        /// 撤回道具
        /// </summary>
        public void RevocationTool()
        {
            if (CanUseTool())
            {
                var data = _playerStep.Pop();
                var push = data[1];
                var pop = data[0];

                _isCoercion = true;
                // Debug.LogError($"弹出{pop.name},{push.name}");
                ClickPipe(push, false);
                JobUtils.Delay(0.201f, () => { ClickPipe(pop, false); });
                AudioClipHelper.Instance.PlaySound(AudioClipEnum.RevocationTool);
                EventDispatcher.instance.DispatchEvent(AppEventType.PlayerStepCountChange);
                Game.Instance.CurrencyModel.ConsumeGoodNumber(GoodType.Tool, (int)GoodSubType.RevocationTool, 1);
            }
        }

        private void AddPlayStep(InGamePipeUI popPipeUI, InGamePipeUI pushPipeUI)
        {
            if (_pushPipeUI != _popPipeUI && !_isCoercion && !_isStartTwoAnime)
            {
                var step = new List<InGamePipeUI>();
                step.Add(popPipeUI);
                step.Add(pushPipeUI);
                // Debug.LogError($"加入{_popPipeUI.name},{_pushPipeUI.name}");
                _playerStep.Push(step);
                EventDispatcher.instance.DispatchEvent(AppEventType.PlayerStepCountChange);
            }
        }

        #endregion ToolLogic
    }
}