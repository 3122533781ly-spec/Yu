using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;


public class RedeemInfoDialog : Dialog
{
    [SerializeField] private Button shareButton;
    [SerializeField] private Button playButton;

    public override void CloseDialog()
    {
        base.CloseDialog();
        shareButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
    }

    public override void ShowDialog()
    {
        if (Game.Instance.Model.IsWangZhuan() && Game.Instance.LevelModel.EnterLevelID ==
            Game.Instance.LevelModel.MaxUnlockLevel.Value - 1 && Game.Instance.LevelModel.MaxUnlockLevel.Value == 2 &&
            !CPlayerPrefs.GetBool("isShowRedeemInfo", false))
        {
            base.ShowDialog();
            shareButton.onClick.AddListener(ShareFriend);
            CPlayerPrefs.SetBool("isShowRedeemInfo", true);
        }
    }

    private void ShareFriend()
    {
        SDKManager.NativeShare((bool isSuccess) =>
        {
            if (isSuccess)
            {
                CloseDialog();
            }
        });
    }
}