using Fangtang;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

namespace _02.Scripts.InGame.Controller
{
    public class InGameGuideController : ElementBehavior<global::InGame>
    {
        [SerializeField] private SmartLocalizedText guidText;

        private InGameGuideUI _guideUI;

        protected override void OnInit()
        {
        }

        public void CheckGuid()
        {
            if (Game.Instance.LevelModel.MaxUnlockLevel.Value == 1 && Game.Instance.LevelModel.EnterLevelID == 1)
            {
                f1();
            }
            else if (Game.Instance.LevelModel.MaxUnlockLevel.Value == 2 && Game.Instance.LevelModel.EnterLevelID == 2)
            {
                f2();
            }
            else
            {
                guidText.SetActiveVirtual(false);
            }
        }

        void f1()
        {
            var guides = GuideUIConfig.Instance.GetGuideByType(1);
            if (guides.Count > 0)
            {
               // JobUtils.Delay(0.1f, () => { GuideManager.Instance.InitGuideData(guides); });
            }
        }

        void f2()
        {
            var guides = GuideUIConfig.Instance.GetGuideByType(2);
            if (guides.Count > 0)
            {
                guidText.SetActiveVirtual(true);
            }
        }

        public void Dispose()
        {
            if (_guideUI)
            {
                GameObject.DestroyImmediate(_guideUI.gameObject);
                _guideUI = null;
            }

           // if (!GuideManager.IsQuiting) GuideManager.Instance.OnGuideStepEnd = null;
        }
    }
}