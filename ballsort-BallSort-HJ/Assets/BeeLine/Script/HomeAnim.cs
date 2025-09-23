using System.Threading.Tasks;
using Fangtang;
using UnityEngine;

public class HomeAnim : ElementBehavior<Home>
{
    public async Task PlayEnterAnim()
    {
        AnimatorHelper.SetTrigger(_animator, "TriggerOut");
        //await TaskExtension.DelaySecond(_animationDuration);
    }

    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private Animator _animator;
}