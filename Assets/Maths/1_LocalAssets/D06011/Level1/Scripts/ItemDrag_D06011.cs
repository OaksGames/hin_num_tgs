using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDrag_D06011 : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas_UI;
    private CanvasGroup canvas_Group;

    public Vector3 ItemInitial_Pos, ItemDragEnd_Pos;

    public bool Is_Init = false;

    public void GetInitProperties()
    {
        rectTransform = this.GetComponent<RectTransform>();
        canvas_UI = this.transform.GetComponentInParent<Canvas>();
        canvas_Group = this.GetComponent<CanvasGroup>();

        ItemInitial_Pos = this.transform.localPosition;//original pos

        Is_Init = true;

        Debug.Log("GetInitProperties");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!Is_Init)
            return;

        canvas_Group.alpha = 0.8f;
        canvas_Group.blocksRaycasts = false;
        GameManager_D06011.Instance.StopAudioRepeateForOnDrag();
        Debug.Log("onBegindrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!Is_Init)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas_UI.scaleFactor;
        Debug.Log("ondrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!Is_Init)
            return;

        canvas_Group.alpha = 1f;
        canvas_Group.blocksRaycasts = true;
        
        if (ItemDragEnd_Pos == transform.localPosition)
        {
            Debug.Log("start & end are equal :" + ItemDragEnd_Pos);
            //Task is done
        }
        else
        {
            this.transform.localPosition = ItemInitial_Pos;
        }

        Debug.Log("start & end are equal :" + ItemInitial_Pos);
    }
}
