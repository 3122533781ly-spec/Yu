using UnityEngine;

public interface IUIGuide
{
    /// <summary>
    /// 目标
    /// </summary>
    RectTransform Target { get; }
    /// <summary>
    /// 目标
    /// </summary>
    RectTransform EventTarget { get; }

    /// <summary>
    /// 画布
    /// </summary>
    Canvas CurrentCanvas { get; }

    /// <summary>
    /// 绑定的对象
    /// </summary>
    GameObject gameObject { get; }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="target"></param>
    /// <param name="canvas"></param>
    void SetTarget(RectTransform target, RectTransform eventTarget, Canvas canvas);

    void Full();
}

/// <summary>
/// UI引导形状
/// </summary>
public enum EGuideShape
{
    /// <summary>
    /// 圆形
    /// </summary>
    Circle,
    /// <summary>
    /// 矩形
    /// </summary>
    Rect,
}

/// <summary>
/// 事件引导类型
/// </summary>
public enum EEventGuideType
{
    None,//可扩充，在GuideEvents中添加事件检测，在GuideManager中绑定回调方法
    Skip,
    SingleClick,
    DoubleClick,
}
/// <summary>
/// 手势引导类型
/// </summary>
public enum EGuideGesture
{
    GuideAnim,// 点击
    GuideAnimU,// 上划
    GuideAnimB,// 下划
    GuideAnimL,
    GuideAnimR,
}

/// <summary>
/// 引导数据
/// </summary>
public struct GuideData
{
    public int Type;
    public int Index;
    /// <summary>
    /// 显示的文本
    /// </summary>
    public string Info;
    /// <summary>
    /// 对话框位置
    /// </summary>
    public int DialogY;
    /// <summary>
    /// 是否隐藏箭头
    /// </summary>
    public bool HideArrow;
    /// <summary>
    /// 手势引导类型
    /// </summary>
    public EGuideGesture Gesture;
    /// <summary>
    /// 手势引导对象路径
    /// </summary>
    public string GesturePath;
    /// <summary>
    /// 高亮对象路径
    /// </summary>
    public string Path;
    /// <summary>
    /// 镂空形状
    /// </summary>
    public EGuideShape Shape;
    /// <summary>
    /// 事件类型
    /// </summary>
    public EEventGuideType Event;
    /// <summary>
    /// 事件对象路径
    /// </summary>
    public string EventPath;
    /// <summary>
    /// 对话框角色
    /// </summary>
    public int Role;
    /// <summary>
    /// 对话框角色位置
    /// </summary>
    public Vector2 RolePos;
    /// <summary>
    /// 箭头偏移位置
    /// </summary>
    public Vector2 ArrowOffset;
}
