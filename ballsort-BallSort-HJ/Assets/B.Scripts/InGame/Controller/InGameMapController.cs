using System;
using _02.Scripts.Config;
using _02.Scripts.LevelEdit;
using _02.Scripts.Util;
using Fangtang;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _02.Scripts.InGame.Controller
{
    public class InGameMapController : ElementBehavior<global::InGame>
    {
        private void OnEnable()
        {
            EventDispatcher.instance.Regist(AppEventType.PlayerPipeSkinChange, RefreshSKin);
        }

        private void OnDisable()
        {
            EventDispatcher.instance.UnRegist(AppEventType.PlayerPipeSkinChange, RefreshSKin);
        }

        //确保当前是最大关卡，并且没有通过特殊关卡，在判断可以进入最大关卡
        private void SetLevelType()
        {
            Game.Instance.LevelModel.CopiesType = CopiesType.Thread;
            Game.Instance.LevelModel.SetEnterLevelID(Game.Instance.LevelModel.EnterLevelID);
        }

        /// <summary>
        /// 读取新的关卡数据时，确保当前是最大关卡，并且没有通过特殊关卡，在判断可以进入最大关卡
        /// </summary>
        private void SetLevelData()
        {
            SetLevelDataByConfig(LevelConfig.Instance, Game.Instance.LevelModel.EnterLevelID);
            Context.GetModel<InGameModel>().LevelData.SetRandomCoin();

            SetSizeFitter();
            CleanAllPipe();
            InitAllPipe();
        }

        private void SetLevelDataByConfig(LevelConfig config, int enterLevel)
        {
            var isHave =
                config.TryGetConfigByID(enterLevel, out var levelData);
            if (isHave)
            {
                Context.GetModel<InGameModel>().LevelData = levelData;
            }
            else
            {
                SetLevelWhenNotF(config);
            }
        }

        private void SetLevelWhenNotF(LevelConfig config)
        {
            var newId = Random.Range(1, config.All.Count);
            var newLevelData = config.GetConfigByID(newId);
            Context.GetModel<InGameModel>().LevelData = newLevelData;
            Game.Instance.LevelModel.EnterLevelID = newId;
        }

        public void StartGame()
        {
            SetLevelType();
            SetLevelData();
        }

        #region PipeSpawn&Destory

        private void InitAllPipe()
        {
            var model = Context.GetModel<InGameModel>();
            var prefab = GameStage.Instance.cellPrefab;
            var pos = GameStage.Instance.spawnRectTransform;
            var pos2 = GameStage.Instance.spawnRectTransform2;
            var index = 1;
            var count = Context.GetModel<InGameModel>().LevelData.pipeDataList.Count;
            var a = (count + 1) / 2;

            //init
            foreach (var pipeData in Context.GetModel<InGameModel>().LevelData.pipeDataList)
            {
                var spawnPos = pos;
                if (index > a && count > 5)
                {
                    spawnPos = pos2;
                }

                var spawnObj = Instantiate(prefab, spawnPos.transform);
                Context.Views.Add(spawnObj);
                spawnObj.InitPipe(pipeData);
                spawnObj.name = $"Pipe_{index}";
                model.LevelPipeList.Add(spawnObj);
                model.temp = model.LevelPipeList.Count;
                index++;
            }
        }

        public void AddNewPipe(Action callBack)
        {
            if (!Context.GetController<InGameMatchController>().CanUseTool() &&
                Context.GetController<InGameMatchController>()._isDropAnime)
            {
                return;
            }

            var model = Context.GetModel<InGameModel>();
            var prefab = GameStage.Instance.cellPrefab;
            var pos = GameStage.Instance.spawnRectTransform;
            var pos2 = GameStage.Instance.spawnRectTransform2;
            var index = Context.GetModel<InGameModel>().LevelData.pipeDataList.Count;

            //init
            var spawnPos = pos;
            if (index > 5)
            {
                spawnPos = pos2;
            }

            var addPipe = model.LevelPipeList[model.LevelPipeList.Count - 1];

            if (model.IsUseAddPipeTool() && addPipe.CanAddPipeSize())
            {
                addPipe.AddPipeSize();
                callBack?.Invoke();
            }
            else if (!model.IsUseAddPipeTool())
            {
                var spawnObj = Instantiate(prefab, spawnPos.transform);
                Context.Views.Add(spawnObj);
                spawnObj.InitPipe(new PipeData(PipeCapacity.Capacity1));
                spawnObj.isAddPipe = true;
                spawnObj.name = $"Pipe_{index + 1}";
                model.LevelPipeList.Add(spawnObj);

                callBack?.Invoke();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)GameStage.Instance.spawnPanel.transform);
        }

        private void CleanAllPipe()
        {
            var model = Context.GetModel<InGameModel>();
            foreach (var oldPipe in model.LevelPipeList)
            {
                DestroyImmediate(oldPipe.gameObject);
            }

            model.LevelPipeList.Clear();

            Context.GetController<InGameMatchController>().CleanAllData();
        }

        #endregion PipeSpawn&Destory

        #region sizeController

        private void RefreshSKin(object[] objs)
        {
            SetSizeFitter();
        }

        private void SetSizeFitter()
        {
            var spawn1Rect = (RectTransform)GameStage.Instance.spawnRectTransform.transform;
            var spawn2Rect = (RectTransform)GameStage.Instance.spawnRectTransform2.transform;
            //管子容量
            var pipeCapacity = Context.CellMapModel.LevelData.GetPipeCapacity();
            //管子属性配置
            var pipeConfig = UtilClass.GetSizeFitter(Context.CellMapModel.LevelData.pipeNumber, pipeCapacity);
            //设置管子间上下左右间距
            GameStage.Instance.spawnPanel.spacing = pipeConfig.pipeHSpace;
            float spacing = pipeConfig.pipeWSpace - pipeConfig.pipeW;
            var pipeCount = (int)Context.CellMapModel.LevelData.pipeNumber;
            InGameManager.Instance.root.localScale = Vector3.one;
            GameStage.Instance.spawnRectTransform.spacing = spacing;
            GameStage.Instance.spawnRectTransform2.spacing = spacing;

            //设置预生成高度
            var high = InGameManager.Instance.pipeSizeConfig.GetTotalHigh(pipeCapacity);
            spawn1Rect.SetSizeDeltaY(high);
            spawn2Rect.SetSizeDeltaY(Context.CellMapModel.LevelData.GetPipeCount() > 5 ? high : 0);
            //小于5管不需要特殊处理适配，根据配置就行
            //管子数量大于5会出现高度或者宽度溢出
            if (pipeCount >= 5 && Context.CellMapModel.LevelData.GetPipeCapacity() != PipeCapacity.Capacity5)
            {
                InGameManager.Instance.root.localScale *= InGameManager.Instance.scale;
            }
            else if (pipeCount >= 5 && Context.CellMapModel.LevelData.GetPipeCapacity() == PipeCapacity.Capacity5)
            {
                InGameManager.Instance.root.localScale *= 0.9f;
            }
            else
            {
                return;
            }

            ResetSpace();
        }

        //根据当前比例适配
        private void ResetSpace()
        {
            var spacing = GameStage.Instance.spawnRectTransform.spacing;
            var pipeCount = (int)Context.CellMapModel.LevelData.pipeNumber;
            var oneLinePipeCount = (pipeCount + 1) / 2;
            if (pipeCount == 5)
            {
                oneLinePipeCount = 5;
            }

            var pipeW = InGameManager.Instance.pipeSizeConfig.GetWidth();
            var currentSize = pipeW * oneLinePipeCount + spacing * (oneLinePipeCount - 1);
            currentSize *= InGameManager.Instance.root.localScale.x;
            var w = 1080 - 20;
            if ((currentSize - w) >= 10)
            {
                //总共溢出
                var expend = currentSize - w;

                var spc = expend / oneLinePipeCount;
                GameStage.Instance.spawnRectTransform.spacing = spacing - spc;
                GameStage.Instance.spawnRectTransform2.spacing = spacing - spc;
            }
            else if ((w - currentSize) >= 10)
            {
                var expend = w - currentSize;
                var spc = expend / oneLinePipeCount;
                GameStage.Instance.spawnRectTransform.spacing = spacing + spc;
                GameStage.Instance.spawnRectTransform2.spacing = spacing + spc;
            }
        }

        #endregion sizeController
    }
}