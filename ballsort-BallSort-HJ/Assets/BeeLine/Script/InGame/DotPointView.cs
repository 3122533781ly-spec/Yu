using System.Collections.Generic;
using _02.Scripts.LevelEdit;
using DG.Tweening;
using Lei31.SElement;
using ProjectSpace.BubbleMatch.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class DotPointView : SimpleElementComponent<DotPoint>
{
    [SerializeField] public float ScaleDuration = 0.3f;
    [SerializeField] public GameObject Lightobj;
    [SerializeField] private SpriteRenderer icon;

    private void OnEnable()
    {
        SetIcon();
    }

    public void PlayFlipDogAnim()
    {
        Lightobj.SetActive(false);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(Context.Transform.DOScaleX(0, ScaleDuration));
        sequence.AppendCallback(ChangeDog);
        sequence.Append(Context.Transform.DOScaleX(1, ScaleDuration));
    }

    public void RefreshView(DotPointData data)
    {
        if (data == null)
        {
            Debug.Log(data.IsBee);
        }
        _beeObject.SetActive(false);

        if (data.Value == 2)
        {
            _beeObject.SetActive(true);
            DotPointViewConfig target = _idToConfig[1];
            Context.Model.SetDogAnimator(target.Animator);
            _flipDog = target.Object;
            //            target.Object.SetActive(true);
        }
        else
        {
            DotPointViewConfig target = _idToConfig[data.Value];
            Context.Model.SetDogAnimator(target.Animator);
            target.Object.SetActive(true);
        }
    }

    public void SetIcon()
    {
        icon.sprite = SpriteManager.Instance.GetLineBallIcon();
    }

    private void ChangeDog()
    {
        _beeObject.SetActive(false);
        _flipDog.SetActive(true);
    }

    private void InitDic()
    {
        _idToConfig = new Dictionary<int, DotPointViewConfig>();

        for (int i = 0; i < Configs.Count; i++)
        {
            _idToConfig.Add(Configs[i].Id, Configs[i]);
            Configs[i].Object.SetActive(false);
        }
    }

    protected override void OnInit()
    {
        InitDic();
    }

    [SerializeField] private GameObject _beeObject;
    [SerializeField] private List<DotPointViewConfig> Configs;
    private Dictionary<int, DotPointViewConfig> _idToConfig;

    private GameObject _flipDog;
}

[System.Serializable]
public class DotPointViewConfig
{
    public int Id;
    public GameObject Object;
    public Animator Animator;
}