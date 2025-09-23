using System.Collections;
using System.Collections.Generic;
using Lei31.SElement;
using UnityEngine;

[SelectionBase]
public class DotPoint : SimpleElement
{
    public Transform Transform
    {
        get { return _transform.Get(gameObject); }
    }

    [SerializeField] public DotPointData Data;
    [SerializeField] public Animator BeeAnimator;

    public DotPointModel Model { get; private set; }

    public bool CanLink()
    {
        return Data.IsBee && Model.State.Value == DotPointState.Standby;
    }

    public void FlipToDog()
    {
        Model.HasFlip = true;
        GetBehaviour<DotPointView>().PlayFlipDogAnim();
    }

    public void Set(DotPointData data)
    {
        Data = data;
        GetBehaviour<DotPointView>().RefreshView(Data);
    }

    public void SetPointLinking()
    {
        Model.State.Value = DotPointState.Linking;
    }

    public void SetPointToStandby()
    {
        Model.State.Value = DotPointState.Standby;
    }

    private void Awake()
    {
        Model = new DotPointModel(this);
        InitSimpleElement<DotPoint>();
    }

    private DelayGetComponent<Transform> _transform = new DelayGetComponent<Transform>();
}