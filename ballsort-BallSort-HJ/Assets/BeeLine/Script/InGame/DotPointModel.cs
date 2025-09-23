using UnityEngine;
using Lei31.Utils;

public class DotPointModel
{
    public bool CanTouch => _context.Data.Value == 2;

    public WrapperData<DotPointState> State { get; private set; }

    public Vector3 InMapLocalPosition { get; set; }

    public bool HasFlip { get; set; }

    public Animator DogAnimator { get; private set; }

    public void SetDogAnimator(Animator animator)
    {
        DogAnimator = animator;
    }

    public DotPointModel(DotPoint context)
    {
        _context = context;
        State = new WrapperData<DotPointState>(DotPointState.Standby);
        HasFlip = false;
    }

    private DotPoint _context;
}

public enum DotPointState
{
    Standby,
    Linking,
}