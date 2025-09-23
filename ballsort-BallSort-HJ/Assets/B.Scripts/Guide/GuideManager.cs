using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// 引导管理器
/// </summary>
public class GuideManager : MonoSingleton<GuideManager>
{
    [SerializeField] private Canvas _guideCanvas; //
    [SerializeField] private Canvas _guideCanvas1;
    [SerializeField] private GuideUICircle _circleUIGuide; //圆形UI引导
    [SerializeField] private GuideUIRect _rectUIGuide; //矩形UI引导
    [SerializeField] private RectTransform _guideArrow; //引导箭头

    [SerializeField] private Text _guideText; //引导文本

    //[SerializeField] private Image _guideRole;//引导人
    //[SerializeField] private Image _guideName;//引导人名字
    [SerializeField] private RectTransform _guideDialog; //引导对话框
    [SerializeField] private Button _guideSkip; //跳过

    private SpriteAtlas _guideAtlas; //引导人图集
    private List<GuideData> _guideDatas; //引导数据列表
    private int _curGuideIndex = -1; //当前引导索引
    private int _curGuideType = 0; //当前引导类型
    private IUIGuide _curUIGuide; //当前UI引导
    private GameObject _curGuideObj; //当前引导对象
    private Button _curGuideBtn; //当前引导按钮对象
    private GameObject _gestureObj; //手势引导对象
    private GameObject _eventObj; //事件引导对象

    public Func<int, int, bool> OnGuideStepEnd;

    /// <summary>
    /// 设置引导数据
    /// </summary>
    /// <param name="guides">引导数据</param>
    /// <param name="atlas">引导人图集</param>
    public void InitGuideData(List<GuideData> guides, SpriteAtlas atlas = null, bool isStartGuide = true)
    {
        _guideAtlas = atlas;
        _guideDatas = guides;

        if (_guideDatas != null && isStartGuide) StartGuide(0); //开始引导
    }

    /// <summary>
    /// 开始引导
    /// </summary>
    public void StartGuide(int index)
    {
        if (_guideDatas == null) return;
        _curGuideIndex = index;
        GuideData guide = _guideDatas[index];
        _curGuideType = guide.Type;
        SetUIGuide(guide);
    }

    /// <summary>
    /// 下一个引导
    /// </summary>
    public void NextGuide(bool isOut = true)
    {
        if (_guideDatas == null) return;
        _curGuideIndex++;
        if (_curGuideIndex < _guideDatas.Count)
        {
            if (isOut) EndGuide(false);
            StartGuide(_curGuideIndex);
        }
        else EndGuide();
    }

    /// <summary>
    /// 结束引导
    /// </summary>
    public void EndGuide(bool isEnd = true)
    {
        HideTip();
        if (_curUIGuide != null) _curUIGuide.gameObject.SetActive(false); //关闭形状遮罩
        _guideArrow.gameObject.SetActive(false); //关闭箭头
        if (_curGuideBtn) _curGuideBtn.onClick.RemoveListener(EndUIGuide);
        if (_curGuideObj) Destroy(_curGuideObj.GetComponent<GuideEvents>());
        if (_guideSkip)
        {
            _guideSkip.SetActive(false);
            _guideSkip.onClick.RemoveListener(EndUIGuide);
        }

        if (_eventObj) Destroy(_eventObj.GetComponent<GuideEvents>());
        _curGuideObj = null;
        _curGuideBtn = null;
        _gestureObj = null;
        _eventObj = null;

        if (isEnd)
        {
            _curGuideIndex = -1;
            _curGuideType = 0;
            _guideDatas = null;
        }
    }

    /// <summary>
    /// 得到当前引导下标
    /// </summary>
    public int GetGuideIndex()
    {
        return _curGuideIndex;
    }

    /// <summary>
    /// 得到当前引导类型
    /// </summary>
    public int GetGuideType()
    {
        return _curGuideType;
    }

    /// <summary>
    /// 引导是否已结束
    /// </summary>
    public bool IsGuideEnd()
    {
        if (_guideDatas == null) return true;
        return _curGuideIndex >= _guideDatas.Count;
    }

    #region 引导内容

    /// <summary>
    /// UI引导
    /// </summary>
    /// <param name="uiPath"></param>
    /// <param name="shape"></param>
    /// <param name="info"></param>
    private void SetUIGuide(GuideData guide)
    {
        if (guide.Info != "-1")
        {
            if (guide.Path != null && guide.Path != "") StartCoroutine(WaitLoad(guide));
            else
            {
                if (guide.Info != null && guide.Info != "")
                {
                    ShowTip(guide.Info, guide.DialogY);
                    FillGuideRole(guide.Role, guide.RolePos);
                }

                SetGuideMask(null, null, guide.Shape);
                if (guide.Event == EEventGuideType.Skip)
                {
                    _guideSkip.SetActive(true);
                    _guideSkip.onClick.AddListener(EndUIGuide);
                }
                else _guideSkip.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(WaitLoadArrow(guide));
        }
    }

    /// <summary>
    /// 等待加载UI
    /// 避免动态加载的报错
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="shape"></param>
    /// <returns></returns>
    private IEnumerator WaitLoad(GuideData guide)
    {
        yield return new WaitUntil(() => (_curGuideObj = GameObject.Find(guide.Path)) != null);
        if (guide.Info != null && guide.Info != "")
        {
            ShowTip(guide.Info, guide.DialogY);
            FillGuideRole(guide.Role, guide.RolePos);
        }

        if (_curGuideObj == null)
        {
            _curGuideObj = GameObject.Find(guide.Path);
        }

        RectTransform target = _curGuideObj.GetComponent<RectTransform>();
        if (guide.Event == EEventGuideType.Skip)
        {
            _guideSkip.SetActive(true);
            _guideSkip.onClick.AddListener(EndUIGuide);

            SetGuideMask(target, target, guide.Shape);
        }
        else
        {
            _guideSkip.SetActive(false);
            RectTransform eventT = null;
            if (guide.Event != EEventGuideType.None)
            {
                if (guide.EventPath != null && guide.EventPath != "")
                {
                    _eventObj = GameObject.Find(guide.EventPath);
                    if (_eventObj != null)
                    {
                        eventT = _eventObj.GetComponent<RectTransform>();
                        _curGuideBtn = _eventObj.GetComponent<Button>();
                        if (_curGuideBtn) _curGuideBtn.onClick.AddListener(EndUIGuide);
                        else
                        {
                            if (guide.Event == EEventGuideType.DoubleClick)
                                _eventObj.AddComponent<GuideEvents>().DoubleClick += EndUIGuide;
                            else _eventObj.AddComponent<GuideEvents>().SingleClick += EndUIGuide;
                        }
                    }
                }
                else
                {
                    _curGuideBtn = _curGuideObj.GetComponent<Button>();
                    if (_curGuideBtn) _curGuideBtn.onClick.AddListener(EndUIGuide);
                    else
                    {
                        if (guide.Event == EEventGuideType.DoubleClick)
                            _curGuideObj.gameObject.AddComponent<GuideEvents>().DoubleClick += EndUIGuide;
                        else _curGuideObj.gameObject.AddComponent<GuideEvents>().SingleClick += EndUIGuide;
                    }
                }
            }

            SetGuideMask(target, eventT == null ? target : eventT, guide.Shape);
            if (!guide.HideArrow)
            {
                if (guide.GesturePath != null && guide.GesturePath != "")
                {
                    _gestureObj = GameObject.Find(guide.GesturePath);
                    if (_gestureObj == null)
                        SetGuideArrow(eventT == null ? target : eventT, guide.Gesture, guide.ArrowOffset);
                    else
                    {
                        RectTransform gestureT = _gestureObj.GetComponent<RectTransform>();
                        SetGuideArrow(gestureT, guide.Gesture, guide.ArrowOffset);
                    }
                }
                else SetGuideArrow(eventT == null ? target : eventT, guide.Gesture, guide.ArrowOffset);
            }
        }
    }

    IEnumerator WaitLoadArrow(GuideData guide)
    {
        if (!guide.HideArrow)
        {
            if (guide.GesturePath != null && guide.GesturePath != "")
            {
                yield return new WaitUntil(() => (_gestureObj = GameObject.Find(guide.GesturePath)) != null);
                RectTransform gestureT = _gestureObj.GetComponent<RectTransform>();
                SetGuideArrow(gestureT, guide.Gesture, guide.ArrowOffset);
            }
        }
    }

    /// <summary>
    /// 设置引导遮罩
    /// </summary>
    /// <param name="target"></param>
    /// <param name="maskShape"></param>
    private void SetGuideMask(RectTransform target, RectTransform eventTarget, EGuideShape maskShape)
    {
        switch (maskShape)
        {
            case EGuideShape.Circle:
                _curUIGuide = _circleUIGuide;
                break;
            case EGuideShape.Rect:
                _curUIGuide = _rectUIGuide;
                break;
        }

        _guideCanvas.worldCamera = UICamera.Instance.Camera;
        _guideCanvas.sortingLayerName = "UI";
        _curUIGuide.SetTarget(target, eventTarget, _guideCanvas);
        _curUIGuide.gameObject.SetActive(true);
        if (target == null) _curUIGuide.Full();
    }

    /// <summary>
    /// 设置引导箭头
    /// </summary>
    /// <param name="target"></param>
    private void SetGuideArrow(RectTransform target, EGuideGesture gesture, Vector2 offset)
    {
        _guideArrow.gameObject.SetActive(true);
        _guideCanvas1.worldCamera = UICamera.Instance.Camera;
        _guideCanvas1.sortingLayerName = "UI";
        Vector2 tarPos = WorldToCanvasPos(_guideCanvas1, target.position);
        if (gesture == EGuideGesture.GuideAnim)
        {
            var half = target.rect.width / 3;
            tarPos.x += half;
        }

        _guideArrow.anchoredPosition = tarPos + offset;

        Animator anim = _guideArrow.GetComponentInChildren<Animator>();
        anim.Play(gesture.ToString());
    }

    private Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world,
            canvas.GetComponent<Camera>(), out position);
        return position;
    }

    /// <summary>
    /// 结束UI遮罩引导
    /// </summary>
    private void EndUIGuide()
    {
        EndGuide(false);
        bool isNext = true;
        if (OnGuideStepEnd != null) isNext = OnGuideStepEnd(_curGuideType, _curGuideIndex);
        if (isNext) NextGuide(false);
    }

    #endregion

    #region 提示信息

    /// <summary>
    /// 显示Tip
    /// </summary>
    /// <param name="info"></param>
    private void ShowTip(string info, int posY)
    {
        _guideDialog.SetActive(true);
        _guideDialog.anchoredPosition = new Vector2(0, posY);
        _guideText.text = info;
    }

    /// <summary>
    /// 隐藏Tip
    /// </summary>
    private void HideTip()
    {
        _guideDialog.SetActive(false);
    }

    /// <summary>
    /// 设置引导人
    /// </summary>
    void FillGuideRole(int role, Vector2 pos)
    {
        if (_guideAtlas == null || role == 0) return;
        // _guideRole.sprite = _guideAtlas.GetSprite("role_" + role);
        // _guideRole.SetNativeSize();
        // _guideRole.rectTransform.anchoredPosition = pos;
        // _guideName.sprite = _guideAtlas.GetSprite("roleName_" + role);
        // _guideName.SetNativeSize();
    }

    #endregion
}