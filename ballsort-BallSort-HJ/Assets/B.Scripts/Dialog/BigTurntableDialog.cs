using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using _02.Scripts.Config;
using _02.Scripts.DressUpUI;
using _02.Scripts.Util;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using UnityEngine;
using UnityEngine.UI;

public class BigTurntableDialogContent : DialogContent
{
    public Action CloseAction;
}

public class BigTurntableDialog : Dialog<BigTurntableDialogContent>
{
    [SerializeField] private Button leftButton = null;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform tableBackImage;
    [SerializeField] private List<TurntableGoodsItem> turntableGoods;
    [SerializeField] private float rotateTime = 2;

    [SerializeField] private Button adAgainButton;
    [SerializeField] private Button noButton;

    [SerializeField] private RectTransform moneyPoint;
    [SerializeField] private RectTransform gemPoint;


    [SerializeField] private RectTransform TurntableRootPanel;
    [SerializeField] private RectTransform endPoint;

    private int spinTimes = 0;
    private SlotMciData hitSlotMciData;
    private GoodsData hitGoodsData;


    // Start is called before the first frame update
    private void OnEnable()
    {
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(OnButtonClicked);
        }

        var t = TurntableRootPanel.anchoredPosition;
        t.y = endPoint.anchoredPosition.y + Screen.height / 2;
        TurntableRootPanel.anchoredPosition = t;
        TurntableRootPanel.DOAnchorPos(endPoint.anchoredPosition, 0.3f);

        adAgainButton.onClick.AddListener(OnAgainClicked);
        noButton.onClick.AddListener(CloseDialog);
    }

    private void OnDisable()
    {
        if (leftButton != null)
        {
            leftButton.onClick.RemoveListener(OnButtonClicked);
        }

        adAgainButton.onClick.RemoveListener(OnAgainClicked);
        noButton.onClick.RemoveListener(CloseDialog);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        turntableGoods.Clear();
        turntableGoods.AddRange(transform.GetComponentsInChildren<TurntableGoodsItem>());
    }
#endif

    private void OnButtonClicked()
    {
    }

    private void OnAgainClicked()
    {
        ADMudule.ShowRewardedAd("BigTurntableDialog", ret =>
        {
            if (ret)
            {
                FreshUI();
                StartCoroutine(DelayStart());
            }
        });
    }


    public override void ShowDialogWithContext(BigTurntableDialogContent outContent)
    {
        if (!Game.Instance.LevelModel.GetTrigger())
        {
            base.ShowDialogWithContext(outContent);
            Game.Instance.LevelModel.SetTrigger(true);
            spinTimes = 0;
            FreshUI();
            StartCoroutine(DelayStart());
        }
    }

    public override void CloseDialog()
    {
        base.CloseDialog();
        Content.CloseAction?.Invoke();
    }

    /// <summary>
    /// 显示按钮
    /// </summary>
    /// <param name="show"></param>
    private void ShowButtons(bool show)
    {
        adAgainButton.transform.parent.gameObject.SetActive(show);
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    private void FreshUI()
    {
        if (spinTimes < ConstantConfig.Instance.GetBigTurn())
        {
            var alldatas = GoodsConfig.Instance.GetGoodsDatasLessAmount();
            foreach (var item in turntableGoods)
            {
                item.Initialize(alldatas.ListRandom());
            }

            hitSlotMciData = SlotMciDataConfig.Instance.GetHitSlotMciData();
            hitGoodsData = hitSlotMciData.GetGoodsData();
            int index = 0;
            float d = float.MinValue;
            for (int i = 0; i < turntableGoods.Count; i++)
            {
                var t = Vector2.Distance(turntableGoods[i].transform.position, topPoint.position);
                if (d < t)
                {
                    d = t;
                    index = i;
                }
            }

            turntableGoods[index].Initialize(hitSlotMciData.GetGoodsData());
        }
        else
        {
            CloseDialog();
        }
    }

    /// <summary>
    /// 延迟转动转盘
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayStart()
    {
        ShowButtons(false);
        yield return new WaitForSeconds(0.5f);
        float v = 360 * (int) rotateTime + 180;
        tableBackImage.DOLocalRotate(new Vector3(0, 0, -v), rotateTime, RotateMode.LocalAxisAdd);
        spinTimes++;
        yield return new WaitForSeconds(rotateTime + 0.5f);
        ShowButtons(true);

        // var hitGoodsData = GetHitGoodsData(out _);
        var reward = new MoneyReward
        {
            goodType = hitGoodsData.Type,
            subType = hitGoodsData.subType,
        };

        if (hitSlotMciData.goodType == GoodType.Money)
        {
            var count = GetRandomMoney();
            reward.moneyCount = count;
        }
        else
        {
            reward.count = hitSlotMciData.count;
        }

        DialogManager.Instance.ShowDialogWithContext<GiftClaimDialogContext>(DialogName.GiftClaimDialog,
            new GiftClaimDialogContext
            {
                CurrentGoodsData = reward,
                TopRectTransform = hitSlotMciData.goodType == GoodType.Money ? moneyPoint : gemPoint,
                HasWatchedADj = hitSlotMciData.goodType == GoodType.Money,
                CloseAction = () =>
                {
                    if (spinTimes >= ConstantConfig.Instance.GetBigTurn())
                    {
                        CloseDialog();
                    }
                }
            });
    }

    private float GetRandomMoney()
    {
        var res = (ConstantConfig.Instance.GetNumberFromList(8) -
                   Game.Instance.CurrencyModel.GetCurrentMoney()) *
                  hitSlotMciData.rateAmount;
        if (res <= 0)
        {
            res = 0.01f;
        }

        return res;
    }
}