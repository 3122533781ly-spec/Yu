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
        // 【核心修改】用VerticalLayoutGroup替代GridLayoutGroup，支持不规则物体
        [SerializeField] public VerticalLayoutGroup ballVerticalLayout;    // 球的垂直布局组
        [SerializeField] public VerticalLayoutGroup emptyVerticalLayout;   // 空物体的垂直布局组
        [SerializeField] public VerticalLayoutGroup bodyVerticalLayout;    // 管子主体的垂直布局组

        // 【新增】布局自适应组件，确保容器根据子物体大小自动调整
        [SerializeField] public ContentSizeFitter ballSizeFitter;          // 球布局的自适应
        [SerializeField] public ContentSizeFitter emptySizeFitter;         // 空物体布局的自适应
        [SerializeField] public ContentSizeFitter bodySizeFitter;          // 主体布局的自适应

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

            // 【初始化布局参数】确保垂直布局支持不规则物体
            InitVerticalLayoutSettings();
        }

        private void OnDisable()
        {
            pipeButton.onClick.RemoveListener(ClickPipe);
            SpriteManager.Instance.RemovePipeData(this);
            EventDispatcher.instance.UnRegist(AppEventType.PlayerPipeSkinChange, RefreshSKin);
        }

        /// <summary>
        /// 【新增】初始化垂直布局参数，关键是取消强制拉伸，保留子物体原有大小
        /// </summary>
        private void InitVerticalLayoutSettings()
        {
            // 通用布局配置：取消强制拉伸，保留子物体自身大小
            void SetLayoutCommon(VerticalLayoutGroup layout, ContentSizeFitter fitter)
            {
                if (layout == null) return;
                // 取消水平/垂直强制拉伸（核心：不改变子物体原有大小）
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                // 子物体对齐方式（可根据需求调整，此处居中）
               // layout.childAlignment = TextAnchor.MiddleCenter;
                // 子物体间距（根据原GridLayout的spacing调整，此处默认10）
                layout.spacing = 0f;
                // 内边距（保留原逻辑，可根据需求调整）
                layout.padding = new RectOffset(0, 0, 0, 0);

                // 自适应配置：容器根据子物体大小自动调整
                if (fitter == null) return;
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // 水平方向自适应
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;   // 垂直方向自适应
                fitter.enabled = false;
            }

            // 分别配置三个垂直布局组
            SetLayoutCommon(ballVerticalLayout, ballSizeFitter);
            SetLayoutCommon(emptyVerticalLayout, emptySizeFitter);
            SetLayoutCommon(bodyVerticalLayout, bodySizeFitter);
        }

        /// <summary>
        /// 【修改】获取管子长度（从body布局的子物体数量计算）
        /// </summary>
        public int PipeLength()
        {
            return bodyVerticalLayout != null ? bodyVerticalLayout.transform.childCount : 0;
        }

        public PipeData GetPipeData()
        {
            return _pipeData;
        }

        public void SetPipeSprite()
        {
            pipeController.SetPipe(_pipeData);
        }

        /// <summary>
        /// 初始化管子（核心修改：用ballVerticalLayout替代原gridLayoutGroup）
        /// </summary>
        public void InitPipe(PipeData initPipeData)
        {
            _pipeData = initPipeData;
            pipeController.SetPipe(_pipeData);
            SetPipeSize();

            // 初始化球：将球添加到垂直布局组，保留原有大小
            for (int i = 0; i < _pipeData.ballDataStack.Count; i++)
            {
                var data = _pipeData.ballDataStack.GetDataByIndex(i);
                // 【修改】将球实例化到垂直布局组下
                var obj = Instantiate(ballPrefab, ballVerticalLayout.transform);
                _context.Views.Add(obj);
                obj.InitBall(data);
                obj.name = $"Ball{data.type}_{i + 1}";

                // 【关键】保留球的原有大小（不被布局强制拉伸）
                var ballRect = obj.GetComponent<RectTransform>();
                if (ballRect != null)
                {
                    // 锁定球的大小（根据预制体原有大小，或手动设置）
                    ballRect.anchorMin = new Vector2(0.5f, 0.5f);
                    ballRect.anchorMax = new Vector2(0.5f, 0.5f);
                    ballRect.pivot = new Vector2(0.5f, 0.5f);
                    ballRect.anchoredPosition = Vector2.zero; // 让布局组控制位置
                }

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
        /// 添加一颗球（逻辑不变，布局由垂直布局组自动处理）
        /// </summary>
        public void PushBall(InGameBallUI ballUI)
        {
            BallLevelEdits.Push(ballUI);
            // 【新增】刷新布局，确保新球添加后自适应大小
            if (ballVerticalLayout != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(ballVerticalLayout.GetComponent<RectTransform>());
            }
        }

        /// <summary>
        /// 弹出一颗球（逻辑不变，布局自动刷新）
        /// </summary>
        public InGameBallUI PopBall()
        {
            InGameBallUI ballUI = null;

            if (BallLevelEdits.Count > 0)
            {
                ballUI = BallLevelEdits.Pop();
                // 【新增】删除球后刷新布局
                if (ballVerticalLayout != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(ballVerticalLayout.GetComponent<RectTransform>());
                }
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
                   (!isAddPipe || Context.CellMapModel.LevelData.GetPipeCapacity() == _pipeData.pipeCapacity);
        }

        public void TriggerFullEff()
        {
            if (IsFullAndOneType())
            {
                fullPipeEff.Play();
                _isPlayed = true;

                foreach (var ball in BallLevelEdits)
                {
                    ball.SetAlready();
                }

                AudioClipHelper.Instance.PlaySound(AudioClipEnum.FullPipe);
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

        private void SpecialPipeTrigger()
        {
            var toPos = CoinFlyAnim.Instance.GetTargetIconPos(AnimIconType.BigTurn);
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
                // 【新增】删除空物体后刷新布局
                if (emptyVerticalLayout != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(emptyVerticalLayout.GetComponent<RectTransform>());
                }
            }
        }

        /// <summary>
        /// 【修改】空物体添加到垂直布局组，支持不规则大小
        /// </summary>
        public RectTransform GetAndInitPushToPos()
        {
            // 【修改】将空物体实例化到垂直布局组下
            var spawnEmpty = Instantiate(Context.GetController<InGameMatchController>().empty, emptyVerticalLayout.transform);
            // 【关键】保留空物体原有大小
            var emptyRect = spawnEmpty.GetComponent<RectTransform>();
            if (emptyRect != null)
            {
                emptyRect.anchorMin = new Vector2(0.5f, 0.5f);
                emptyRect.anchorMax = new Vector2(0.5f, 0.5f);
                emptyRect.pivot = new Vector2(0.5f, 0.5f);
                emptyRect.anchoredPosition = Vector2.zero;
            }
            _emptyList.Add(spawnEmpty);

            // 【新增】刷新空物体布局
            if (emptyVerticalLayout != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(emptyVerticalLayout.GetComponent<RectTransform>());
            }
            return spawnEmpty;
        }

        #region 管子大小属性控制（修改布局组件引用）
        [Header("管子大小属性控制")]
        [SerializeField] private RectTransform rootRectTransform;

        private List<RectTransform> pipeControllerEmpty = new List<RectTransform>();

        /// <summary>
        /// 根据容量初始化管子图片（修改布局组件引用为垂直布局）
        /// </summary>
        private void SetPipeSize()
        {
            var h = InGameManager.Instance.pipeSizeConfig.GetTotalHigh(_pipeData.pipeCapacity);
            var w = InGameManager.Instance.pipeSizeConfig.GetWidth();
            var currentAlreadySpawn = pipeControllerEmpty.Count;
            rootRectTransform.sizeDelta = new Vector2(w, h);

            // 生成分割线（逻辑不变，布局由bodyVerticalLayout控制）
            for (int i = 0; i < (int)_pipeData.pipeCapacity - currentAlreadySpawn; i++)
            {
                var obj = Instantiate(FenJieXian, bodyVerticalLayout.transform);
                pipeControllerEmpty.Add(obj);
                // 【关键】保留分割线原有大小
                var lineRect = obj.GetComponent<RectTransform>();
                if (lineRect != null)
                {
                    lineRect.anchorMin = new Vector2(0.5f, 0.5f);
                    lineRect.anchorMax = new Vector2(0.5f, 0.5f);
                    lineRect.pivot = new Vector2(0.5f, 0.5f);
                    lineRect.anchoredPosition = Vector2.zero;
                }
            }

            // 【修改】移除原GridLayout的cellSize配置，改用垂直布局的自动适配
            var pipeConfig = UtilClass.GetSizeFitter(Context.CellMapModel.LevelData.pipeNumber, _pipeData.pipeCapacity);
            pipeController.RefreshSKin();

            // 【新增】刷新主体布局
            if (bodyVerticalLayout != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(bodyVerticalLayout.GetComponent<RectTransform>());
            }
        }

        private void RefreshSKin(object[] objs)
        {
            SetPipeSize();
        }
        #endregion 管子大小属性控制
    }
}