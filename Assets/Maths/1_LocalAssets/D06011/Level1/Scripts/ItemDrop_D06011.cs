using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDrop_D06011 : MonoBehaviour, IDropHandler
{
    public GameObject GemParent;
    public int EmptySlots;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && EmptySlots>0)
        {
           
            EmptySlots--;
            eventData.pointerDrag.gameObject.transform.parent = GemParent.transform;
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            GameManager_D06011.Instance.CheckAllCartsareFilled();
            //GameManager_D06011.Instance.Tut_CheckAllCartsareFilled();
            eventData.pointerDrag.GetComponent<ItemDrag_D06011>().enabled = false;
            Debug.Log("Here : "+ EmptySlots);
        }
    }
}
