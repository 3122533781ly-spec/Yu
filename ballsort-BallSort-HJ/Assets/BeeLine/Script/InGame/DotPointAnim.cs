using DG.Tweening;
using Lei31.SElement;
using UnityEngine;

public class DotPointAnim : SimpleElementComponent<DotPoint>
{
    private InGameLineBee _inGame;
    [SerializeField] private GameObject Light;

    public void PlayBeeAttack()
    {
        AnimatorHelper.SetTrigger(Context.BeeAnimator, "TriggerAttack");
    }

    public void PlayWin()
    {
        AnimatorHelper.SetTrigger(Context.Model.DogAnimator, "TriggerWin");
    }

    public void PlayDotHeart()
    {
        AnimatorHelper.SetTrigger(Context.Model.DogAnimator, "TriggerHeart");
    }

    private void OnEnable()
    {
        Context.Model.State.OnValueChange += StateChange;
    }

    private void OnDisable()
    {
        Context.Model.State.OnValueChange -= StateChange;
    }

    private void StateChange(DotPointState oldState, DotPointState newState)
    {
        if (newState == DotPointState.Linking)
        {
            Light.SetActive(true);
        }
        else
        {
            Light.SetActive(false);
        }
    }
}