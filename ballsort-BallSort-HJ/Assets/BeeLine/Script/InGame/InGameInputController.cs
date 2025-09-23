using System.Collections.Generic;
using Fangtang;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using System.Linq;

public class InGameInputController : ElementBehavior<InGameLineBee>
{
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 wordPoint = Context.GameCamera.ScreenToWorldPoint(Input.mousePosition);

            UpdateTouch(wordPoint);
        }
    }

    private void UpdateTouch(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        DotPoint target = SelectTargetBlock(colliders);
        if (target != null && CanTouchNow())
        {
            Context.EventModel.OnTouchPoint.Invoke(target);
        }
    }

    private bool IsTouchUI()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;
        EventSystem.current.RaycastAll(data, _uiResult);
        if (_uiResult.Count > 0)
        {
            for (int i = 0; i < _uiResult.Count; i++)
            {
                Graphic graphic = _uiResult[i].gameObject.GetComponent<Graphic>();
                if (graphic != null && graphic.raycastTarget)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CanTouchNow()
    {
        if (Context.MatchModel.LinkedList.Count > 1)
            return false;

        if (IsTouchUI())
        {
            return false;
        }

        return true;
    }

    private DotPoint SelectTargetBlock(Collider2D[] colliders)
    {
        DotPoint result = null;
        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            DotPoint block = colliders[i].GetComponent<DotPoint>();
            if (block != null && block.Model.CanTouch)
            {
                result = block;
            }
        }

        return result;
    }

    private void Awake()
    {
        _uiResult = new List<RaycastResult>();
    }

    private List<RaycastResult> _uiResult;
}