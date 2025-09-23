using System.Collections.Generic;
using System.Linq;
using _02.Scripts.InGame.Controller;
using _02.Scripts.LevelEdit;
using _02.Scripts.Util;
using ProjectSpace.BubbleMatch.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _02.Scripts.InGame.UI
{
    public class InGamePipeUI : ElementUI<global::InGame>
    {
        [SerializeField] public GridLayoutGroup gridLayoutGroup;
        [SerializeField] public GridLayoutGroup emptyLayoutGroup;
        [SerializeField] public GridLayoutGroup bodyLayoutGroup;

        [FormerlySerializedAs("pipe")]
        [SerializeField]
        private PipeSizeController pipeController;

        [SerializeField] private InGameBallUI ballPrefab;
        [SerializeField] private Button pipeButton;

        [SerializeField] public RectTransform popToPos;
        [SerializeField] public RectTransform emptyPanel;
        [SerializeField] public RectTransform pipeControllerPanel;

        [SerializeField] private ParticleSystem fullPipeEff;
        [SerializeField] private RectTransform FenJieXian;
        public readonly Common.Stack<InGameBallUI> BallLevelEdits = new Common.Stack<InGameBallUI>();
        public bool isAddPipe;
        private List<RectTransform> _emptyList = new List<RectTransform>();
        private PipeData _pipeData;
        private bool _isPlayed;

        private void OnEnable()
        {
            pipeButton.onClick.AddListener(ClickPipe);
            SpriteManager.Instance.AddPipeData(this);
            EventDispatcher.instance.Regist(AppEventType.PlayerPipeSkinChange, RefreshSKin);
        }

        private void OnDisable()
        {
            pipeButton.onClick.RemoveListener(ClickPipe);
            SpriteManager.Instance.RemovePipeData(this);
            EventDispatcher.instance.UnRegist(AppEventType.PlayerPipeSkinChange, RefreshSKin);
        }

        public int PipeLength()
        {
            return bodyLayoutGroup.transform.childCount;
        }

        public PipeData GetPipeData()
        {
            return _pipeData;
        }

        public void SetPipeSprite()
        {
            pipeController.SetPipe(_pipeData);
            // pipe.sprite = SpriteManager.Instance.GetPipeIcon();
        }

        /// <summary>
        /// 初始化管子
        /// </summary>
        public void InitPipe(PipeData initPipeData)
        {
            _pipeData = initPipeData;
            pipeController.SetPipe(_pipeData);
            // pipe.sprite = SpriteManager.Instance.GetPipeIcon();
            SetPipeSize();

            for (int i = 0; i < _pipeData.ballDataStack.Count; i++)
            {
                var data = _pipeData.ballDataStack.GetDataByIndex(i);
                var obj = Instantiate(ballPrefab, gridLayoutGroup.transform);
                _context.Views.Add(obj);
                obj.InitBall(data);
                obj.name = $"Ball{data.type}_{i + 1}";
                PushBall(obj);
                GetAndInitPushToPos();
            }

            CheckTop();
        }

        public bool CanAddPipeSize()
        {
            return _pipeData.IsCanAddPipeCapacity() &&
                   _pipeData.pipeCapacity < Context.CellMapModel.LevelData.GetPipeCapacity();
        }

        public void AddPipeSize()
        {
            if (CanAddPipeSize())
            {
                _pipeData.pipeCapacity++;
                SetPipeSize();
            }
        }

        // public PipeAndBallData GetPipeData()
        // {
        //     return UtilClass.GetSizeFitter(Context.CellMapModel.LevelData.pipeNumber, _pipeData.pipeCapacity);
        // }

        public bool CanPushBallLimit(BallData ballData)
        {
            var judgment1 = BallLevelEdits.Count == 0;
            if (judgment1)
            {
                return true;
            }

            var judgment2 = BallLevelEdits.Count < (int)_pipeData.pipeCapacity;
            var judgment3 = ballData.type == BallLevelEdits.Peek().GetBallData().type;

            return judgment2 && judgment3;
        }

        /// <summary>
        /// 添加一颗球
        /// </summary>
        /// <param name="ballUI"></param>
        public void PushBall(InGameBallUI ballUI)
        {
            BallLevelEdits.Push(ballUI);
        }

        /// <summary>
        /// 是否有可以弹出的球
        /// </summary>
        /// <returns></returns>
        public InGameBallUI PopBall()
        {
            InGameBallUI ballUI = null;

            if (BallLevelEdits.Count > 0)
            {
                ballUI = BallLevelEdits.Pop();
            }

            return ballUI;
        }

        private void ClickPipe()
        {
            _context.GetController<InGameMatchController>().ClickPipe(this);
        }

        public bool PipeFullOrEmpty()
        {
            if (BallLevelEdits.Count == 0)
            {
                return true;
            }

            var array = BallLevelEdits.ToList();
            var isHaveOther = array.Find(x => x.GetBallData().type != array[0].GetBallData().type) == null;

            return BallLevelEdits.Count == (int)_pipeData.pipeCapacity && isHaveOther;
        }

        public void CheckTop()
        {
            if (BallLevelEdits.Count != 0 && Context.CellMapModel.LevelData.blindBox)
            {
                BallLevelEdits.Peek().SetAlready();
            }
        }

        private bool IsFull()
        {
            return BallLevelEdits.Count == (int)_pipeData.pipeCapacity && BallLevelEdits.Count > 0;
        }

        private bool IsHaveOtherBall()
        {
            var type = BallLevelEdits.Peek().GetBallData().type;
            var ballList = BallLevelEdits.ToList();
            var isHaveOtherBall = ballList.Find(x => x.GetBallData().type != type) != null;
            return isHaveOtherBall;
        }

        private bool IsFullAndOneType()
        {
            return IsFull() && !IsHaveOtherBall() && !_isPlayed &&
                   (!isAddPipe || Context.CellMapModel.LevelData.GetPipeCapacity() == _pipeData.pipeCapacity); //达到最大容量
        }

        /// <summary>
        /// 之前胜利判断，通过每次操作将球落下，落下时检测是否完成
        /// 新的特殊玩法胜利，检测必须在转盘弹窗关闭后
        /// </summary>
        public void TriggerFullEff()
        {
            //当前管子是否装满了
            if (IsFullAndOneType())
            {
                fullPipeEff.Play();
                _isPlayed = true;

                //所有球去除问号
                foreach (var ball in BallLevelEdits)
                {
                    ball.SetAlready();
                }

                AudioClipHelper.Instance.PlaySound(AudioClipEnum.FullPipe);
                //判断是否在特殊玩法中，特殊玩法胜利依赖装盘弹窗
                if (IsSpecialModel())
                {
                    SpecialPipeTrigger();
                }
            }
        }

        private bool IsSpecialModel()
        {
            var ballType = BallLevelEdits.Peek().GetBallData().type;
            return (ballType == BallType.Coin || ballType == BallType.Money);
        }

        /// <summary>
        /// 特殊管子触发胜利有几种特殊情况
        /// </summary>
        private void SpecialPipeTrigger()
        {
            var toPos =
                CoinFlyAnim.Instance.GetTargetIconPos(AnimIconType.BigTurn);
            //遍历所有球在的位置，生成动画
            for (int i = 0; i < _emptyList.Count; i++)
            {
                var ballType = BallLevelEdits.Peek().GetBallData().type;
                if (ballType == BallType.Coin)
                {
                    CoinFlyAnim.Instance.Play(1, _emptyList[i].position, toPos, AnimIconType.BigTurn,
                        () => { },
                        () =>
                        {
                            EventDispatcher.instance.DispatchEvent(AppEventType.FinishSpecialPipe,
                                0.5f / _emptyList.Count);
                        });
                }
                else if (ballType == BallType.Money)
                {
                    CoinFlyAnim.Instance.Play(1, _emptyList[i].position, toPos, AnimIconType.BigTurnMoney,
                        () => { },
                        () =>
                        {
                            EventDispatcher.instance.DispatchEvent(AppEventType.FinishSpecialPipe,
                                0.5f / _emptyList.Count);
                        });
                }
            }
        }

        public void ControllerEmptyList()
        {
            var lastObh = _emptyList.FirstOrDefault();
            if (lastObh)
            {
                _emptyList.Remove(lastObh);
                DestroyImmediate(lastObh.gameObject);
            }
        }

        public RectTransform GetAndInitPushToPos()
        {
            var spawnEmpty = Instantiate(Context.GetController<InGameMatchController>().empty, emptyPanel.transform);
            _emptyList.Add(spawnEmpty);
            return spawnEmpty;
        }

        #region 管子大小属性控制

        [Header("管子大小属性控制")][SerializeField] private RectTransform rootRectTransform;

        private List<RectTransform> pipeControllerEmpty = new List<RectTransform>();

        /// <summary>
        /// 根据容量初始化管子图片
        /// </summary>
        private void SetPipeSize()
        {
            var h = InGameManager.Instance.pipeSizeConfig.GetTotalHigh(_pipeData.pipeCapacity);
            var w = InGameManager.Instance.pipeSizeConfig.GetWidth();
            var currentAlreadySpawn = pipeControllerEmpty.Count;
            rootRectTransform.sizeDelta = new Vector2(w, h);

            for (int i = 0; i < (int)_pipeData.pipeCapacity - currentAlreadySpawn; i++)
            {
                var obj = Instantiate(FenJieXian,
                    pipeControllerPanel.transform);
                pipeControllerEmpty.Add(obj);
            }

            var pipeConfig = UtilClass.GetSizeFitter(Context.CellMapModel.LevelData.pipeNumber, _pipeData.pipeCapacity);
            // gridLayoutGroup.cellSize = new Vector2(pipeConfig.ball, pipeConfig.ball);
            // emptyLayoutGroup.cellSize = new Vector2(pipeConfig.ball, pipeConfig.ball);
            // bodyLayoutGroup.cellSize = new Vector2(pipeConfig.ball, pipeConfig.ball);
            pipeController.RefreshSKin();
        }

        private void RefreshSKin(object[] objs)
        {
            SetPipeSize();
        }

        #endregion 管子大小属性控制
    }
}