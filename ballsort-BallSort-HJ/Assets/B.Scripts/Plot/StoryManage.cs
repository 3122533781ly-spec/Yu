using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum StoryType
{
    Guide,
    Role
}

public class StoryManage : MonoSingleton<StoryManage>
{
    [SerializeField] private RectTransform storyRoot;
    [SerializeField] private Canvas _guideCanvas;

    [SerializeField] private Canvas _bgCanvas;
    [SerializeField] private Image _bg;

    PlotUI storyUI;
    PlotRoleUI roleUI;
    StoryType type;
    int rolePlotId;

    Action<StoryType> onPlotHide;

    Func<int, bool> onStepEnd;
    Action onStepHide;

    /// <summary>
    /// 剧情对话之后要接的引导类型
    /// </summary>
    public int NextGuide;

    public void StartStory(Action<StoryType> onPlotHide = null)
    {
        this.type = StoryType.Guide;
        this.onPlotHide = onPlotHide;

        _guideCanvas.worldCamera = UICamera.Instance.Camera;
        _guideCanvas.sortingLayerName = "UI";

        _bgCanvas.gameObject.SetActive(false);

        GuideStory();
    }

    async void GuideStory()
    {
        if (storyUI == null)
        {
            // var handle = YooAssets.LoadAssetAsync<GameObject>("PlotUI");
            // await handle.Task;
            // GameObject prefab = handle.AssetObject as GameObject;
            // GameObject go = Instantiate(prefab);
            // handle.Release();
            // storyUI = go.GetComponent<PlotUI>();
            // storyUI.transform.localScale = Vector3.one;
            // storyUI.transform.localPosition = Vector3.zero;
            // storyUI.transform.SetParent(storyRoot, false);
        }
        storyUI.SetActive(true);
        storyUI.Rest();
        storyUI.StartPlay(onEnd);
    }

    void onEnd()
    {
        if (storyUI != null) storyUI.SetActive(false);
        onPlotHide?.Invoke(type);
    }

    public void StartRoleStory(int id, Func<int, bool> onStepEnd = null, Action onStepHide = null)
    {
        this.type = StoryType.Role;
        this.onStepEnd = onStepEnd;
        this.onStepHide = onStepHide;
        rolePlotId = id;

        _guideCanvas.worldCamera = UICamera.Instance.Camera;
        _guideCanvas.sortingLayerName = "UI";

        _bgCanvas.worldCamera = UICamera.Instance.Camera;
        _bgCanvas.sortingLayerName = "UI";
        _bgCanvas.gameObject.SetActive(true);

        RoleStory();
        NetRoleData();
    }

    void NextRoleStory()
    {
        if (rolePlotId == 0) return;
        rolePlotId++;
        NetRoleData();
    }

    async void RoleStory()
    {
        if (roleUI == null)
        {
            // var handle = YooAssets.LoadAssetAsync<GameObject>("PlotRoleUI");
            // await handle.Task;
            // GameObject prefab = handle.AssetObject as GameObject;
            // GameObject go = Instantiate(prefab);
            // handle.Release();
            // roleUI = go.GetComponent<PlotRoleUI>();
            // roleUI.transform.localScale = Vector3.one;
            // roleUI.transform.localPosition = Vector3.zero;
            // roleUI.transform.SetParent(storyRoot, false);
            // roleUI.SetData(_bg, onRoleEnd);
        }
        roleUI.SetActive(true);
    }

    void NetRoleData()
    {
        RolePlotData d;
        var has = RolePlotConfig.Instance.TryGetConfigByID(rolePlotId, out d);
        if (has) roleUI.SetSpeak(d);
        else onRoleHide();
    }

    void onRoleEnd()
    {
        bool isNext = true;
        if (onStepEnd != null) isNext = onStepEnd(rolePlotId);
        if (isNext) NextRoleStory();
        else onRoleHide();
    }

    void onRoleHide()
    {
        if (roleUI != null) roleUI.SetActive(false);
        _bgCanvas.gameObject.SetActive(false);
        onStepHide?.Invoke();
    }
}
