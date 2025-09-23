using System;
using System.Collections.Generic;
using _02.Scripts.InGame;
using _02.Scripts.InGame.UI;
using _02.Scripts.LevelEdit;
using NPOI.SS.Formula.Functions;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.U2D;

namespace ProjectSpace.BubbleMatch.Scripts.Util
{
    [Serializable]
    [CreateAssetMenu(fileName = "SpriteManager", menuName = "ScriptTableObjects/SpriteManager", order = 1)]
    public class SpriteManager : ScriptableSingleton<SpriteManager>
    {
        [Header("管子")][SerializeField] private Sprite pipeIcon;
        [Header("球")][SerializeField] private List<Sprite> ballIcon;
        [Header("？球")][SerializeField] private Sprite grayBallIcon;
        [Header("皮肤数据")][SerializeField] private SkinDataList_SO skinData;
        [Header("道具")][SerializeField] private List<Sprite> toolIcon;
        [Header("管头")][SerializeField] private List<Sprite> pipeHeadIcon;
        [Header("管身")][SerializeField] private List<Sprite> pipeBodyIcon;
        [Header("特殊球")][SerializeField] private List<Sprite> specialBall;
        [Header("物品对应图标")][SerializeField] private List<GoodTypeSpriteAssetPair> goodTypeIcon;
        private List<InGameBallUI> inGameBallUIs = new List<InGameBallUI>();

        private List<InGamePipeUI> inGamePipeUIs = new List<InGamePipeUI>();

        protected override void HandleOnEnable()
        {
            base.HandleOnEnable();
        }

        public Sprite GetGoodTypeIconByType(GoodType goodType, int subType)
        {
            var res = toolIcon[0];

            var findObj = goodTypeIcon.Find(x => x.GoodType == goodType && x.GoodSubType == subType);
            if (findObj != null)
            {
                res = findObj.Sprite;
            }

            return res;
        }

        private int ballSkinID;
        private int bgSkinID;

        public void InitSkin()
        {
            DialogManager.Instance.GetDialog<LevelUIDialog>().Init();
            //if (PlayerPrefs.HasKey("ClickBallSkin"))
            //    SetBallSkin(PlayerPrefs.GetInt("ClickBallSkin"));
            //else
            //    SetBallSkin(0);

            //if (PlayerPrefs.HasKey("ClickThemeSkin"))
            //    SetThemeSkin(PlayerPrefs.GetInt("ClickThemeSkin"));
            //else
            //    SetThemeSkin(0);
        }

        public void SetBallSkin(int value)
        {
            if (ballSkinID == value && value != 0)
                return;
            var normal = skinData.GetBallSkin(value).Clone();
            normal.AddRange(specialBall);
            ballIcon = normal;

            for (int i = 0; i < inGameBallUIs.Count; i++)
            {
                inGameBallUIs[i].SetIcon();
            }

            ballSkinID = value;
        }

        public Sprite GetGoodTypeIcon(GoodType type, int subType)
        {
            if (type == GoodType.Coin)
            {
                return toolIcon[0];
            }

            return toolIcon[(int)subType];
        }

        public List<Sprite> GetPipeSkin(int value)
        {
            if (value < 0)
            {
                value = 1;
            }
            else if (value >= pipeHeadIcon.Count)
            {
                value = pipeHeadIcon.Count - 1;
            }

            var res = new List<Sprite> { pipeHeadIcon[value], pipeBodyIcon[value] };
            return res;
        }

        public void SetTubeSkin(int value)
        {
            pipeIcon = skinData.GetSkin(value, SkinType.Tube);

            for (int i = 0; i < inGamePipeUIs.Count; i++)
            {
                inGamePipeUIs[i].SetPipeSprite();
            }
        }

        public void SetThemeSkin(int value)
        {
            if (bgSkinID == value && value != 0)
                return;
            if (Game.Instance.Model.IsWangZhuan())
            {
                InGameManager.Instance.bg.sprite = skinData.GetSkin(value, SkinType.Bg);
                for (int i = 0; i < inGamePipeUIs.Count; i++)
                {
                    inGamePipeUIs[i].SetPipeSprite();
                }
                bgSkinID = value;
                return;
            }
            Debug.Log("场景" + SceneManager.CurrentScene);
            if (SceneManager.CurrentScene == "InGame")
            {
                GameStage.Instance.bg.sprite = skinData.GetSkin(value, SkinType.Bg);
            }
            else
            {
                InGameManager.Instance.bg.sprite = skinData.GetSkin(value, SkinType.Bg);
                for (int i = 0; i < inGamePipeUIs.Count; i++)
                {
                    inGamePipeUIs[i].SetPipeSprite();
                }
            }

            bgSkinID = value;
        }

        public void SetThemeSkinInLineBee()
        {
        }

        public void SetLineBeeBg(int value)
        {
            if (SceneManager.CurrentScene == "InGame")
                GameStage.Instance.bg.sprite = skinData.GetSkin(value, SkinType.Bg);
        }

        public void SetBallSortBg(int value)
        {
            if (InGameManager.Instance.bg == null)
            {
                return;
            }
            InGameManager.Instance.bg.sprite = skinData.GetSkin(value, SkinType.Bg);
            for (int i = 0; i < inGamePipeUIs.Count; i++)
            {
                inGamePipeUIs[i].SetPipeSprite();
            }
        }

        public Sprite GetPipeIcon()
        {
            return pipeIcon;
        }

        public Sprite GetBallIcon(bool levelDataBlindBox, BallType ballType)
        {
            if (levelDataBlindBox)
            {
                return GetMysBallSpr();
            }

            var index = (int)ballType - 1;
            if (index < 0 || index >= ballIcon.Count)
            {
                return ballIcon[0];
            }

            return ballIcon[index];
        }

        public Sprite GetLineBallIcon()
        {
            var index = 0;
            return ballIcon[index];
        }

        public Color GetLineColor()
        {
            return skinData.GetLineColor(PlayerPrefs.GetInt("ClickBallSkin"));
        }

        public Sprite GetMysBallSpr()
        {
            return skinData.GetMysBallSprite(PlayerPrefs.GetInt("ClickBallSkin"));
        }

        public void AddBallData(InGameBallUI inGameBallUI)
        {
            inGameBallUIs.Add(inGameBallUI);
        }

        public void RemoveBallData(InGameBallUI inGameBallUI)
        {
            inGameBallUIs.Remove(inGameBallUI);
        }

        public void AddPipeData(InGamePipeUI inGamePipeUI)
        {
            inGamePipeUIs.Add(inGamePipeUI);
        }

        public void RemovePipeData(InGamePipeUI inGamePipeUI)
        {
            inGamePipeUIs.Remove(inGamePipeUI);
        }
    }

    [Serializable]
    public class GoodTypeSpriteAssetPair
    {
        [SerializeField] private GoodType goodType;
        [SerializeField] private int subType;
        [SerializeField] private Sprite sprite;

        public GoodType GoodType
        {
            get => goodType;
            set => goodType = value;
        }

        public int GoodSubType
        {
            get => subType;
            set => subType = value;
        }

        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }
    }

    [Serializable]
    public class SpriteAssetPair
    {
        [SerializeField] private string spriteName;
        [SerializeField] private Sprite sprite;

        public string SpriteName
        {
            get => spriteName;
            set => spriteName = value;
        }

        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }
    }

    [Serializable]
    public class SpriteAssetPairList
    {
        [SerializeField] private string spriteName;
        [SerializeField] private SpriteAtlas sprite;

        public string SpriteName
        {
            get => spriteName;
            set => spriteName = value;
        }

        public SpriteAtlas Sprite
        {
            get => sprite;
            set => sprite = value;
        }
    }
}