using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

public class GetRewardDialog : Dialog
{
    public Image icon;
    private GoodSubType2 type;
    [SerializeField] private Button NorBtn;
    [SerializeField] private Button ADBtn;



    public static bool hasWatchedAD = false;
    private GoodsData currentGoodsData;
    private System.Action downCallback = null;
    private float rateAmount = 1f;

    private void OnEnable()
    {
        NorBtn.onClick.AddListener(ReceiveFree);
        ADBtn.onClick.AddListener(ReceiveAD);
    }

    private void OnDisable()
    {
         NorBtn.onClick.RemoveListener(ReceiveFree);
         ADBtn.onClick.RemoveListener(ReceiveAD);
    }

    private void ReceiveFree()
    {
        Game.Instance.CurrencyModel.AddNewGoodCount(type);

        this.Deactivate();
    }
    private void ReceiveAD()
    {
       
    }


    public void Init(GoodSubType2 rewardType)
    {
        base.ShowDialog();
        icon.sprite = Resources.Load<Sprite>($"Chest/{rewardType}");
        icon.SetNativeSize();
        this.type = rewardType;
    }

   
}