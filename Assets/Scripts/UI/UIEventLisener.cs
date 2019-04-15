using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public sealed class UIEventLisener : EventTrigger
{
    public delegate void OnClickDelgate(GameObject go);
    public delegate void OnDragDelegate(GameObject go, PointerEventData eventData);

    public event OnClickDelgate OnClickEvent;
    public event OnClickDelgate OnPress;
    public event OnClickDelgate OnUp;
    public event OnClickDelgate OnEnter;
    public event OnClickDelgate OnExit;

    public event OnDragDelegate OnDragBegin;
    public event OnDragDelegate OnDragIng;
    public event OnDragDelegate OnDragEnd;


    public static UIEventLisener Get(GameObject go)
    {
        UIEventLisener li = go.GetComponent<UIEventLisener>();
        if (li == null)
        {
            li = go.AddComponent<UIEventLisener>();
        }
        return li;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickEvent != null)
        {
            OnClickEvent(gameObject);
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (OnPress != null)
            OnPress(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (OnUp != null)
            OnUp(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnter != null)
            OnEnter(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (OnExit != null)
            OnExit(gameObject);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (OnDragBegin != null)
            OnDragBegin(gameObject,eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (OnDragIng != null)
            OnDragIng(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (OnDragEnd != null)
            OnDragEnd(gameObject, eventData);
    }

}
