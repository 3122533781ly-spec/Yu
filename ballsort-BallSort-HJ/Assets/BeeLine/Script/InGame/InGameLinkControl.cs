using Fangtang;
using UnityEngine;

public class InGameLinkControl : ElementBehavior<InGameLineBee>
{
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Context.MatchModel.LinkedList.Count > 0)
            {
                PlayFlipAnim();
            }
        }

        if (Input.GetMouseButton(0) && Context.MatchModel.LinkedList.Count > 0)
        {
            UpdateLinkedList();
        }
    }

    private void UpdateLinkedList()
    {
        DotPoint lastPoint = Context.MatchModel.LastPoint;
        Vector2 touchWordPoint = Context.GameCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dirLastToTouch = (touchWordPoint - (Vector2)lastPoint.Transform.position).normalized;
        DotPoint target = GameHelper.GetDirPoint(Context.MatchModel.AllPoint, lastPoint, dirLastToTouch);

        if (target == null)
        {
            return;
        }
        if (Vector2.Distance(lastPoint.Transform.position, touchWordPoint) >= _checkDis)
        {
            if (target.CanLink())
            {
                AddToLink(target);
            }
            else if (Context.MatchModel.LinkedList.Count >= 2 &&
                     target.Data.IsBee && target == Context.MatchModel.LastTwoPoint)
            {
                RemoveFormLink(lastPoint);
            }
        }
    }

    private async void PlayFlipAnim()
    {
        float duration = Context.MatchModel.LinkedList[0].GetBehaviour<DotPointView>().ScaleDuration;

        for (int i = 0; i < Context.MatchModel.LinkedList.Count; i++)
        {
            Context.MatchModel.LinkedList[i].FlipToDog();
        }

        await TaskExtension.DelaySecond(duration + 0.5f);

        if (Context.MatchModel.IsAllPointDog())
        {
            //Win
            Context.GameWin();
        }
        else
        {
            Context.GameFailed();
        }
    }

    private void OnEnable()
    {
        Context.EventModel.OnTouchPoint += TouchPoint;
    }

    private void OnDisable()
    {
        Context.EventModel.OnTouchPoint -= TouchPoint;
    }

    //只有第一个点需要使用此处
    private void TouchPoint(DotPoint point)
    {
        if (NeedAddNewPointToLink(point))
        {
            AddToLink(point);
        }
    }

    private void AddToLink(DotPoint target)
    {
        LineBeeInGameAudio.Instance.PlayTapPoint();
        target.SetPointLinking();
        Context.MatchModel.AddToLast(target);
    }

    private void RemoveFormLink(DotPoint target)
    {
        target.SetPointToStandby();
        Context.MatchModel.RemoveLast(target);
    }

    private bool NeedAddNewPointToLink(DotPoint target)
    {
        if (Context.MatchModel.LinkedList.Count <= 0)
        {
            return true;
        }

        DotPoint last = Context.MatchModel.LastPoint;
        if (BoardMapHelper.IsNeighbor(last, target) && target.Model.State.Value == DotPointState.Standby)
        {
            return true;
        }

        return false;
    }

    [SerializeField] private float _checkDis = 0.941f * 3;
}