using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using Animation = Spine.Animation;

public class PlotRoleItem : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private int speak;
    [SerializeField] private float posX;

    List<string> acts;
    //0.随机 1.播放第一个之后循环第二个
    int playType;

    public int GetSpeak()
    {
        return speak;
    }

    public void Play(List<string> acts, int playType = 0)
    {
        this.acts = acts;
        this.playType = playType;
        if(playType == 1)
        {
            PlayAct(acts[0], false);
        }
        else
        {
            var index = Random.Range(0, acts.Count);
            PlayAct(acts[index]);
        }
    }

    void PlayAct(string act, bool isLoop = true)
    {
        if (skeleton.AnimationState.GetCurrent(0).Animation.Name.Equals(act)) return;
        Animation animation = skeleton.SkeletonData.FindAnimation(act);
        if (animation != null)
        {
            skeleton.AnimationState.Complete -= Complete;
            skeleton.AnimationState.Complete += Complete;
            skeleton.AnimationState.SetAnimation(0, act, isLoop);
        }
    }

    void Complete(TrackEntry entry)
    {
        skeleton.AnimationState.Complete -= Complete;
        if (playType == 1)
        {
            PlayAct(acts[1]);
        }
        else
        {
            var index = Random.Range(0, acts.Count);
            PlayAct(acts[index]);
        }
    }

    public void SetSpeak(int speak)
    {
        skeleton.color = this.speak == speak ? Color.white : Color.gray;
    }

    public void SetPosX(bool alone = true)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(alone ? 0 : posX, 0);
    }
}