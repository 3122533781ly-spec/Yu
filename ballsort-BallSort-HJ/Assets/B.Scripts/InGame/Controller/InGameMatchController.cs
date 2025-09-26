using System;
using System.Collections.Generic;
using _02.Scripts.InGame.UI;
using _02.Scripts.Util;
using DG.Tweening;
using Fangtang;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher = _02.Scripts.Util.EventDispatcher;

namespace _02.Scripts.InGame.Controller
{
    public class InGameMatchController : ElementBehavior<global::InGame>
    {
        [SerializeField] public RectTransform empty;

        [SerializeField, Range(0.1f, 2f)] private float dropSpeed = 0.5f;

        private InGameBallUI _popBallUI;
        private InGamePipeUI _popPipeUI;
        private InGamePipeUI _pushPipeUI;

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
                    ClickPipe(_popPipeUI, false);
                    _pushPipeUI = clickPipeUI;
                    ClickPipe(clickPipeUI, false);
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
            var polBall = PopBallAndFly(inGamePipeUI);
            if (polBall)
            {
                SetPopIsAnime(true);
                polBall.StopPushAnime();
                _popBallUI = polBall;
            }
        }
        public InGameBallUI FlyBall()
        {
            return _popBallUI;
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

        private void PushBallAndFly(InGamePipeUI inGamePipeUI, InGamePipeUI pushPipeUI, InGameBallUI ballUI)
        {
            var timeScale = 1;
            var emptyRectTransform = pushPipeUI.GetAndInitPushToPos(ballUI,ballUI.GetBallData());
            emptyRectTransform.gameObject.GetComponent<Image>().SetAlpha(0);
           var popPipe = inGamePipeUI;
            pushPipeUI.PushBall(ballUI);

            SetPushIsAnime(true);

            FlyToTopPos(pushPipeUI, ballUI, () =>
            {
                SetIsDropAnime(true);
                var sequence = DOTween.Sequence();
                ballUI.SetPushAnime(sequence);
                var bottomPosition = emptyRectTransform.transform.position;

                if (_popPipeUI != pushPipeUI && !_isStartTwoAnime)
                {
                    _popPipeUI.CheckTop();
                }
                RectTransform ballRect = ballUI.GetComponent<RectTransform>();
       
                sequence.Append(ballUI.transform.DOMove(bottomPosition, dropSpeed * timeScale))
                        .Join(ballUI.transform.DORotate(Vector3.zero, dropSpeed * timeScale).SetEase(Ease.InOutSine))
                        .OnComplete(() =>
                        {
                          ballUI.transform.SetParent(pushPipeUI.ballVerticalLayout.transform, true);
                            ballUI.SetPushAnime(null);
                            pushPipeUI.TriggerFullEff();
                            Context.CheckIsOver();
                            Context.GetView<InGamePlayingUI>().SetBar();
                            AddPlayStep(popPipe, pushPipeUI);
                            _isCoercion = false;
                            _isStartTwoAnime = false;
                            SetIsDropAnime(false);
                            SetPushIsAnime(false);
                        })
                        .OnKill(() =>
                        {
                            ballUI.SetPushAnime(null);
                            pushPipeUI.TriggerFullEff();
                            Context.CheckIsOver();
                            Context.GetView<InGamePlayingUI>().SetBar();
                            SetIsDropAnime(false);
                            SetPushIsAnime(false);
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

        private void FlyToTopPos(InGamePipeUI inGamePipeUI, InGameBallUI popBall, Action callBack = null)
        {
            popBall.transform.SetParent(inGamePipeUI.popToPos);
            var sequence = DOTween.Sequence();
            sequence.Append(popBall.transform.DOMove(inGamePipeUI.popToPos.position, 0.15f))
                    .Join(popBall.transform.DORotate(new Vector3(0, 0, 25f), 0.15f).SetEase(Ease.InOutSine));
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

        public void RevocationTool()
        {
            if (CanUseTool())
            {
                var data = _playerStep.Pop();
                var push = data[1];
                var pop = data[0];

                _isCoercion = true;
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
                _playerStep.Push(step);
                EventDispatcher.instance.DispatchEvent(AppEventType.PlayerStepCountChange);
            }
        }

        #endregion ToolLogic
    }
}
