using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonClickEffect : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        iTween.ScaleTo(this.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.05f, "delay", 0.01f, "easetype", iTween.EaseType.linear));
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        iTween.ScaleTo(this.gameObject, iTween.Hash("scale", new Vector3(0.9f, 0.9f, 0.9f), "time", 0.01f, "delay", 0f, "easetype", iTween.EaseType.easeOutElastic));  
    }
}
