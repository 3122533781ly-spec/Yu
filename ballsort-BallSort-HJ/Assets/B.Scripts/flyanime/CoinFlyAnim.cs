using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _02.Scripts.Util;
using DG.Tweening;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using ProjectSpace.Lei31Utils.Scripts.IAPModule;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoinFlyAnim : MonoSingleton<CoinFlyAnim>
{
    public const string CoinFlyObjID = "CoinFlyObject";
    public const string ManuaFlyObjID = "ManuaFlyObject";
    public const string LevelChallengeFlyObjID = "LevelChallengeFlyObjID";
    public const string StarFlyObjID = "StarFlyObject";
    public const string ConsumeStarFlyObjID = "ConsumeStarFlyObjID";
    public const string MoneyFlyObject = "MoneyFlyObject";
    public const string BigTurnFlyObject = "BigTurnFlyObject";
    public const string BigTurnMoneyFlyObject = "BigTurnMoneyFlyObject";
    public const string GemFlyObject = "GemFlyObject";

    private void SetCamera(Camera camera)
    {
        Canvas canvas = _iconParent.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = camera;
            canvas.sortingLayerName = "UI";
        }
    }

    private List<MoneyReward> currentHaveList;

    public void PlayRewardList(List<MoneyReward> rewardList, Vector3 formPoint, Action onCompleted = null,
        Action onEnd = null)
    {
        currentHaveList = rewardList.FindAll(x => GetIconType(x.goodType) != AnimIconType.Null).Clone();
        //可以飞行物品为空
        if (currentHaveList.Count == 0)
        {
            onCompleted?.Invoke();
            return;
        }

        Play(currentHaveList[0].GetFlyAnimeCount(), formPoint, GetIconType((GoodType)currentHaveList[0].goodType),
            onEnd: () =>
            {
                currentHaveList.RemoveAt(0);
                PlayRewardList(currentHaveList, formPoint, onCompleted, onEnd);
            });
    }

    public AnimIconType GetIconType(GoodType goodId, int subType = 0)
    {
        var res = AnimIconType.Null;
        switch (goodId)
        {
            case GoodType.Coin:
                res = AnimIconType.Coin;
                break;

            case GoodType.Star:
                res = AnimIconType.Star;
                break;

            case GoodType.Money:
                res = AnimIconType.Money;
                break;

            case GoodType.Gem:
                res = AnimIconType.Diamond;
                break;
        }

        return res;
    }

    public void Play(int num, Vector3 formPoint, AnimIconType iconType = AnimIconType.Coin, Action onCompleted = null,
        Action onEnd = null, float quicken = 1)
    {
        if (num == 0)
        {
            return;
        }

        SetCamera(UICamera.Instance.Camera);
        Vector3 posTo = GetTargetIconPos(iconType);
        num = Math.Min(20, num);
        Play(num, formPoint, posTo, onCompleted, iconType, onEnd, quicken);
    }

    public void DressBtnPlay(Transform Targetpos, int num, Vector3 formPoint, AnimIconType iconType = AnimIconType.Coin, Action onCompleted = null,
        Action onEnd = null, float quicken = 1)
    {
        if (num == 0)
        {
            return;
        }

        SetCamera(UICamera.Instance.Camera);
        Vector3 posTo = Targetpos.position;
        num = Math.Min(20, num);
        Play(num, formPoint, posTo, onCompleted, iconType, onEnd, quicken);
    }

    public void Play(int num, Vector3 formPoint, Vector3 posTo, AnimIconType iconType = AnimIconType.Coin,
        Action onCompleted = null, Action onEnd = null, float quicken = 1)
    {
        if (num == 0)
        {
            return;
        }

        SetCamera(UICamera.Instance.Camera);
        if (posTo == Vector3.zero) posTo = GetTargetIconPos(iconType);
        num = Math.Min(GetTargetIconNum(iconType), num);
        Play(num, formPoint, posTo, onCompleted, iconType, onEnd, quicken);
    }

    private int GetTargetIconNum(AnimIconType iconType)
    {
        switch (iconType)
        {
            case AnimIconType.Manual:
            case AnimIconType.Coin:
                return 20;

            default:
                return 10;
        }
    }

    private string GetTargetIconStr(AnimIconType iconType)
    {
        switch (iconType)
        {
            case AnimIconType.Manual:
                return ManuaFlyObjID;

            case AnimIconType.Diamond:
                return GemFlyObject;

            case AnimIconType.levelChallenge:
                return LevelChallengeFlyObjID;

            case AnimIconType.Star:
                return StarFlyObjID;

            case AnimIconType.ConsumeStar:
                return ConsumeStarFlyObjID;

            case AnimIconType.BigTurn:
                return BigTurnFlyObject;

            case AnimIconType.BigTurnMoney:
                return BigTurnMoneyFlyObject;

            case AnimIconType.GemFlyObject:
                return GemFlyObject;

            case AnimIconType.Signin:
            case AnimIconType.Money:
                return MoneyFlyObject;

            case AnimIconType.AddTime:
            case AnimIconType.AddPlate:
            case AnimIconType.MergeFood:
            case AnimIconType.ReBackTool:
            case AnimIconType.RefreshFood:
                return $"{iconType}FlyObjID";

            default:
            case AnimIconType.Coin:
                return CoinFlyObjID;
        }
    }

    private float GetTargetDlay(AnimIconType iconType)
    {
        switch (iconType)
        {
            case AnimIconType.Manual:
            case AnimIconType.Diamond:
            case AnimIconType.levelChallenge:
                return 0.1f;
            // case AnimIconType.Star:
            //     return 1f;
            case AnimIconType.ConsumeStar:
                return 0.1f;

            case AnimIconType.AddTime:
            case AnimIconType.AddPlate:
            case AnimIconType.MergeFood:
            case AnimIconType.ReBackTool:
            case AnimIconType.RefreshFood:
            case AnimIconType.Star:
                return 0.1f;

            default:
            case AnimIconType.Coin:
                return 0.5f;
        }
    }

    public Transform GetTargetIconRect(AnimIconType iconType)
    {
        switch (iconType)
        {
            case AnimIconType.Manual:
                return Game.Instance.ManualTrans;

            case AnimIconType.Diamond:
                return Game.Instance.DiamondTrans;

            case AnimIconType.MagicStick:
                return Game.Instance.MagicTrans;

            case AnimIconType.Money:
                return Game.Instance.MoneyTrans;

            case AnimIconType.Star:
                return Game.Instance.StarTrans;

            case AnimIconType.BigTurn:
                return Game.Instance.BigTurnTrans;

            case AnimIconType.BigTurnMoney:
                return Game.Instance.BigTurnTrans;

            case AnimIconType.AddTime:
            case AnimIconType.AddPlate:
            case AnimIconType.MergeFood:
            case AnimIconType.ReBackTool:
            case AnimIconType.RefreshFood:
                return _backpack;

            case AnimIconType.Coin:
                return Game.Instance.CoinTrans;

            default:
                return Game.Instance.CoinTrans;
        }
    }

    public Vector3 GetTargetIconPos(AnimIconType iconType)
    {
        switch (iconType)
        {
            case AnimIconType.Manual:
                return Game.Instance.ManualTrans.position;

            case AnimIconType.Diamond:
                return Game.Instance.DiamondTrans.position;

            case AnimIconType.MagicStick:
                return Game.Instance.MagicTrans.position;

            case AnimIconType.Money:
                return Game.Instance.MoneyTrans.position;

            case AnimIconType.GemFlyObject:
                return Game.Instance.MoneyTrans.position;

            case AnimIconType.Star:
                return Game.Instance.StarTrans.position;

            case AnimIconType.BigTurn:
                return Game.Instance.BigTurnTrans.position;

            case AnimIconType.BigTurnMoney:
                return Game.Instance.BigTurnTrans.position;

            case AnimIconType.AddTime:
            case AnimIconType.AddPlate:
            case AnimIconType.MergeFood:
            case AnimIconType.ReBackTool:
            case AnimIconType.RefreshFood:
                return _backpack.position;

            case AnimIconType.Coin:
                return Game.Instance.CoinTrans.position;

            default:
                return Game.Instance.CoinTrans.position;
        }
    }

    private bool CheckShowBackpack(AnimIconType iconType)
    {
        return iconType >= AnimIconType.AddTime && iconType <= AnimIconType.RefreshFood;
    }

    public void Play(int num, Vector2 screenPointForm, Vector2 screenPointTo, Action onCompleted = null,
        AnimIconType iconType = AnimIconType.Coin, Action onEnd = null, float quicken = 1)
    {
        AudioClipHelper.Instance.PlaySound(AudioClipEnum.GetCoin);
        List<CoinFlyBase> flyCoinTrans = Create(num, iconType);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_iconParent, screenPointForm, null,
            out Vector2 formLocalPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_iconParent, screenPointTo, null,
            out Vector2 toLocalPoint);
        for (int i = 0; i < flyCoinTrans.Count; i++)
        {
            flyCoinTrans[i].RectT.anchoredPosition = formLocalPoint;
            flyCoinTrans[i].Play(quicken);
        }

        StartCoroutine(FlyPlay(iconType, toLocalPoint, flyCoinTrans, onCompleted, onEnd, quicken));
    }

    private IEnumerator FlyPlay(AnimIconType iconType, Vector2 flyTo, List<CoinFlyBase> flyCoinTrans,
        Action onCompleted = null, Action onEnd = null, float quicken = 1)
    {
        if (CheckShowBackpack(iconType)) _backpack.SetActive(true);
        _mask.SetActive(true);
        var dely = GetTargetDlay(iconType);
        yield return Yielders.Get(dely);

        float awit = 0.3f / quicken, reduce = 0.3f / quicken;
        for (int i = 0; i < flyCoinTrans.Count; i++)
        {
            Sequence sequence = DOTween.Sequence();
            Vector2 randomTo = new Vector2(Random.Range(-_blastRange, _blastRange),
                Random.Range(-_blastRange, _blastRange));
            sequence.Append(flyCoinTrans[i].RectT
                .DOAnchorPos(flyCoinTrans[i].RectT.anchoredPosition + randomTo, _blastDuration / quicken)
                .SetEase(Ease.OutCirc));
            sequence.AppendInterval(awit);
            sequence.Append(flyCoinTrans[i].RectT.DOAnchorPos(flyTo, _flyDuration / quicken));
            EventDispatcher.instance.DispatchEvent(AppEventType.Shake);
            yield return Yielders.Get(0.01f);
        }

        yield return Yielders.Get(_flyDuration / quicken + awit + _blastDuration / quicken - reduce);
        EventDispatcher.instance.DispatchEvent(AppEventType.Shake);
        onCompleted?.Invoke();
        if (_backpack.gameObject.activeSelf)
        {
            var seq = DOTween.Sequence();
            seq.AppendInterval(0.2f);
            seq.Append(_backpack.DOPunchScale(Vector3.one * 0.41f, reduce));
            seq.AppendInterval(0.2f);
            seq.AppendCallback(() => { _backpack.SetActive(false); });
        }

        yield return Yielders.Get(reduce);

        var icon = GetTargetIconStr(iconType);
        Transform rootParent = PoolManager.Instance.GetRootTransform(icon);
        for (int i = 0; i < flyCoinTrans.Count; i++)
        {
            flyCoinTrans[i].Stop();
            flyCoinTrans[i].RectT.SetParent(rootParent);
            PoolManager.Instance.FreeObject(icon, flyCoinTrans[i].gameObject);
        }

        flyCoinTrans = null;
        _mask.SetActive(false);
        onEnd?.Invoke();
    }

    private List<CoinFlyBase> Create(int num, AnimIconType iconType = AnimIconType.Coin)
    {
        List<CoinFlyBase> result = new List<CoinFlyBase>();
        var icon = GetTargetIconStr(iconType);
        for (int i = 0; i < num; i++)
        {
            CoinFlyBase item = PoolManager.Instance.CreateObject(icon).GetComponent<CoinFlyBase>();
            item.RectT.SetParent(_iconParent);
            item.RectT.ResetLocal();
            result.Add(item);
        }

        return result;
    }

    public async void SingleIconFly(int differentValue, Vector3 screenPointTo, AnimIconType iconType,
        Action onCompleted)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_iconParent, screenPointTo, null,
            out Vector2 toLocalPoint);

        GameObject targetObjPrefab = GetTargetIconAndPos(iconType);
        RectTransform targetRect = Instantiate(targetObjPrefab, _iconParent).GetComponent<RectTransform>();
        targetRect.anchoredPosition = Vector2.zero;
        if (targetRect == null) return;

        targetRect.SetActive(true);
        targetRect.transform.localScale = Vector3.one * 0.2f;
        targetRect.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack);
        Text animIconText = targetRect.GetComponentInChildren<Text>(true);
        if (animIconText != null) animIconText.gameObject.SetActive(true);
        if (animIconText != null) animIconText.text = differentValue.ToString();
        _mask.SetActive(true);
        await Task.Delay((int)(0.6f * 1000));
        targetRect.DOScale(Vector3.one, _flyDuration);
        targetRect.DOAnchorPos(toLocalPoint, _flyDuration).onComplete = delegate
        {
            _mask.SetActive(false);
            onCompleted?.Invoke();
            if (targetRect != null) Destroy(targetRect.gameObject);
        };
    }

    private GameObject GetTargetIconAndPos(AnimIconType iconType)
    {
        switch (iconType)
        {
            case AnimIconType.Manual:
                return _manualIcon;

            case AnimIconType.Diamond:
                return _actionPointIcon;

            case AnimIconType.GoldBrick:
                return _goldBrickIcon;

            default:
                throw new ArgumentOutOfRangeException(nameof(iconType), iconType, null);
        }
    }

    public void ShowMask(bool show)
    {
        _lockMask.SetActive(show);
    }

    /// <summary>
    /// 是否在展示主页面飞奖励
    /// </summary>
    public bool IsShowMask()
    {
        return _lockMask.gameObject.activeSelf;
    }

    #region SpecialFunc

    public void SpecialPlay(int num, Vector3 formPoint, Vector3 toPoint, AnimIconType iconType = AnimIconType.Coin,
        Action onCompleted = null, Action everyIconEnd = null)
    {
        if (num == 0)
        {
            return;
        }

        SetCamera(UICamera.Instance.Camera);
        // Vector3 posTo = GetTargetIconPos(iconType);
        num = Math.Min(20, num);
        ConsumePlay(num, formPoint, toPoint, onCompleted, everyIconEnd, iconType);
    }

    //top飞去其他
    public void ConsumePlay(int num, Vector3 formPoint, AnimIconType iconType = AnimIconType.Coin,
        Action onCompleted = null, Action everyIconEnd = null)
    {
        if (num == 0)
        {
            return;
        }

        SetCamera(UICamera.Instance.Camera);
        Vector3 posTo = GetTargetIconPos(iconType);
        num = Math.Min(20, num);
        ConsumePlay(num, posTo, formPoint, onCompleted, everyIconEnd, iconType);
    }

    private void ConsumePlay(int num, Vector2 screenPointForm, Vector2 screenPointTo, Action onCompleted = null,
        Action everyIconEnd = null,
        AnimIconType iconType = AnimIconType.Coin)
    {
        // AudioClipHelper.Instance.PlaySound(AudioClipEnum.GetCoin);
        List<CoinFlyBase> flyCoinTrans = Create(num, iconType);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_iconParent, screenPointForm, null,
            out Vector2 formLocalPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_iconParent, screenPointTo, null,
            out Vector2 toLocalPoint);
        for (int i = 0; i < flyCoinTrans.Count; i++)
        {
            flyCoinTrans[i].RectT.anchoredPosition = formLocalPoint;
            flyCoinTrans[i].SetActiveVirtual(false);
        }

        StartCoroutine(SpecialFlyPlay(iconType, toLocalPoint, flyCoinTrans, onCompleted, everyIconEnd));
    }

    /// <summary>
    /// 0.2s发射一颗，0.4s到达
    /// </summary>
    /// <param name="iconType"></param>
    /// <returns></returns>
    private IEnumerator SpecialFlyPlay(AnimIconType iconType, Vector2 flyTo, List<CoinFlyBase> flyCoinTrans,
        Action onCompleted = null,
        Action everyIconEnd = null)
    {
        var flytime = 1f;
        _mask.SetActive(true);
        // var dely = GetTargetDlay(iconType);
        // yield return Yielders.Get(dely);

        float awit = 0.2f;
        for (int i = 0; i < flyCoinTrans.Count; i++)
        {
            var i1 = i;
            var item = flyCoinTrans[i1];
            item.SetActiveVirtual(true);

            Sequence sequence = DOTween.Sequence();
            var waitTime = awit;
            flyCoinTrans[i1].Play();
            sequence.Append(flyCoinTrans[i].RectT.DOAnchorPos(flyTo, flytime));
            sequence.OnComplete(() =>
            {
                everyIconEnd?.Invoke();
                // AudioClipHelper.Instance.PlaySound(AudioClipEnum.ClickFood);
                var icon = GetTargetIconStr(iconType);
                Transform rootParent = PoolManager.Instance.GetRootTransform(icon);

                item.Stop();
                item.RectT.SetParent(rootParent);
                PoolManager.Instance.FreeObject(icon, flyCoinTrans[i1].gameObject);
                if (i1 == flyCoinTrans.Count - 1)
                {
                    onCompleted?.Invoke();
                }
            });

            yield return Yielders.Get(waitTime);
        }

        yield return Yielders.Get(flytime + awit * (flyCoinTrans.Count - 1));
        flyCoinTrans = null;
        _mask.SetActive(false);
    }

    public void EndAllAnime()
    {
    }

    #endregion SpecialFunc

    [SerializeField] private float _flyDuration = 0.5f;

    [FormerlySerializedAs("_coinParent")]
    [SerializeField]
    private RectTransform _iconParent;

    [SerializeField] private float _blastRange = 100f;
    [SerializeField] private float _blastDuration = 0.5f;
    [SerializeField] private GameObject _manualIcon;
    [SerializeField] private GameObject _goldBrickIcon;
    [SerializeField] private GameObject _actionPointIcon;
    [SerializeField] private GameObject _mask;
    [SerializeField] private GameObject _lockMask;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform _backpack;
}

public enum AnimIconType
{
    Null = -1,
    Coin = 0,
    Manual,
    Diamond,
    GoldBrick,
    MagicStick,
    levelChallenge,
    Star,
    ConsumeStar,
    BigTurn,
    BigTurnMoney,
    Signin,

    AddTime = 100, //时间道具
    AddPlate, //盘子道具
    MergeFood, //合并食物
    ReBackTool, //撤回
    RefreshFood, //打乱
    Money,
    GemFlyObject
}