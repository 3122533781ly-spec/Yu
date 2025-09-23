using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Fangtang;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class InGamePointEnterAnim : ElementBehavior<InGameLineBee>
{
    [Button]
    public void Test()
    {
        PlayBlockEnterAnim(null);
    }

    public void PlayGameFailedAnim(Action onCompleted)
    {
        _onGameFailedCompleted = onCompleted;
        PlayGameFailedAnim();
    }

    public void PlayGameWinAnim(Action onCompleted)
    {
        _onGameWinCompleted = onCompleted;
        PlayGameWinAnim();
    }

    public void PlayBlockEnterAnim(Action onCompleted)
    {
        _onEnterCompleted = onCompleted;
        Ready();
        PlayMoveIn();
    }

    private void Ready()
    {
        for (int i = 0; i < Context.MatchModel.AllPoint.Count; i++)
        {
            DotPoint dot = Context.MatchModel.AllPoint[i];
            if (dot.Data.IsBee)
            {
                Vector2 localPos = Random.insideUnitCircle.normalized * _enterReadyRadius;
                dot.Transform.localPosition = localPos;
            }
            else
            {
                Context.MatchModel.AllPoint[i].gameObject.SetActive(false);
            }
        }
    }

    [SerializeField] private float _enterReadyRadius = 10f;

    public async void PlayMoveIn()
    {
        await TaskExtension.DelaySecond(0.3f);

        if (LineBee.Instance.LevelModel.MaxUnlockLevel.Value == 1)
        {
            HandleTutorialHand();
        }
    }

    private async void PlayGameWinAnim()
    {
        //for (int i = 0; i < Context.MatchModel.AllPoint.Count; i++)
        //{
        //    DotPoint dot = Context.MatchModel.AllPoint[i];
        //    dot.GetBehaviour<DotPointAnim>().PlayWin();
        //}

        //await TaskExtension.DelaySecond(1.5f);
        _onGameWinCompleted.Invoke();
    }

    private async void PlayGameFailedAnim()
    {
        List<DotPoint> effectDot = new List<DotPoint>();
        for (int i = 0; i < Context.MatchModel.AllPoint.Count; i++)
        {
            DotPoint dot = Context.MatchModel.AllPoint[i];
            if (dot.Data.IsBee && !dot.Model.HasFlip)
            {
                dot.GetBehaviour<DotPointAnim>().PlayBeeAttack();

                effectDot.AddRange(GetEffectDogPoint(dot));
            }
        }

        //await TaskExtension.DelaySecond(0.6f);

        for (int i = 0; i < effectDot.Count; i++)
        {
            effectDot[i].GetBehaviour<DotPointAnim>().PlayDotHeart();
        }

        //await TaskExtension.DelaySecond(2f);

        _onGameFailedCompleted.Invoke();
    }

    private List<DotPoint> GetEffectDogPoint(DotPoint target)
    {
        List<DotPoint> result = new List<DotPoint>();
        List<DotPoint> neighbors = GameHelper.GetNeighbor(Context.MatchModel.AllPoint, target);

        for (int i = 0; i < neighbors.Count; i++)
        {
            if (!neighbors[i].Data.IsBee || neighbors[i].Model.HasFlip)
            {
                result.Add(neighbors[i]);
            }
        }

        return result;
    }

    private void HandleTutorialHand()
    {
        Context.GetView<InGameTutorialUI>().Activate();

        DotPoint topPoint = Context.MatchModel.AllPoint[0];
        Vector2 screenPoint = Context.GameCamera.WorldToScreenPoint(topPoint.Transform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(Context.GetView<InGameTutorialUI>().HandParent,
        screenPoint,
          UICamera.Instance.Camera, out Vector2 localPoint);

        Context.GetView<InGameTutorialUI>().SetHandRectPosY(localPoint.y);
    }

    [SerializeField] private float _moveInDuration = 1f;
    [SerializeField] private AudioClip _clipBeeMove;
    [SerializeField] private AudioClip _clipBGM;
    private Action _onEnterCompleted;
    private Action _onGameWinCompleted;
    private Action _onGameFailedCompleted;
}