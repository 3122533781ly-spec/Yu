using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Fangtang;
using UnityEngine;

public class InGameLineView : ElementBehavior<InGameLineBee>
{
    public void PlayViewHint(List<DotPoint> path)
    {
        List<Vector3> lines = iTween.GetSmoothPoints(path.Select(o => o.Transform.position).ToArray(), _smooth);
        _hintLineRenderer.positionCount = lines.Count;
        _hintLineRenderer.SetPositions(lines.ToArray());
        _hintLineRenderer.gameObject.SetActive(true);
        HintNumber.SetActive(true);
        Color form = new Color(1f, 1f, 1f, 0f);
        Color to = new Color(1f, 1f, 1f, 1f);

        DOTween.Kill(_hintLineRenderer);
        _hintLineRenderer.startColor = form;
        _hintLineRenderer.DOColor(new Color2(form, form), new Color2(to, to), 1f)
            .SetLoops(4, LoopType.Yoyo)
            .OnComplete(() =>
            {
                _hintLineRenderer.gameObject.SetActive(false);
                HintNumber.SetActive(false);
            });
    }

    private void Update()
    {
        if (Context.MatchModel.LinkedList.Count <= 0)
        {
            _linePoints.Clear();
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                UpdateTouching();
            }
        }

        UpdateLine();
    }

    private void UpdateTouching()
    {
        Vector3 mousePoint = Context.GameCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < Context.MatchModel.LinkedList.Count; i++)
        {
            points.Add(Context.MatchModel.LinkedList[i].Transform.position);
        }

        points.Add(mousePoint);
        _linePoints = iTween.GetSmoothPoints(points.ToArray(), _smooth);
    }

    private void UpdateLine()
    {
        _lineRenderer.positionCount = _linePoints.Count;
        _lineRenderer.SetPositions(_linePoints.ToArray());
    }

    protected override void OnInit()
    {
        _linePoints = new List<Vector3>();
    }

    [SerializeField] private LineRenderer _lineRenderer = null;
    [SerializeField] private int _smooth = 8;

    [SerializeField] private LineRenderer _hintLineRenderer = null;
    [SerializeField] private GameObject HintNumber;
    private List<Vector3> _linePoints;
}