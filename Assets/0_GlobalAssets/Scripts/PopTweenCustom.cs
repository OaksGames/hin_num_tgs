using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PopTweenCustom : MonoBehaviour 
{
    public enum AnimationsAt
    {
        AtAwake,
        AtStart,
        AtOnEnable,
        AtCall
    }
    public AnimationsAt AnimationAt;

    public float ScaleTo=1.1f;
    public float AnimTime=0.25f;
    [HideInInspector]
    public Vector3 ObjScale;

	void Awake()
	{
        ObjScale = this.gameObject.transform.localScale;
        if (AnimationAt == AnimationsAt.AtAwake)
        {
            iTween.ScaleTo(this.gameObject, iTween.Hash("x", ScaleTo, "y", ScaleTo, "time", AnimTime, "islocal", true, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));
        }
	}

    void Start()
    {
        if (AnimationAt == AnimationsAt.AtStart)
        {
            iTween.ScaleTo(this.gameObject, iTween.Hash("x", ScaleTo, "y", ScaleTo, "time", AnimTime, "islocal", true, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));
        }
    }

    void OnEnable()
    {
        if (AnimationAt == AnimationsAt.AtOnEnable)
        {
            iTween.ScaleTo(this.gameObject, iTween.Hash("x", ScaleTo, "y", ScaleTo, "time", AnimTime, "islocal", true, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));
        }
    }

    public void StartAnim()
    {
        if (AnimationAt == AnimationsAt.AtCall)
        {
            iTween.ScaleTo(this.gameObject, iTween.Hash("x", ScaleTo, "y", ScaleTo, "time", AnimTime, "islocal", true, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));
        }
    }

    public void StopAnim()
    {
        Destroy(this.gameObject.GetComponent<iTween>());
        this.gameObject.transform.localScale = ObjScale;
    }
}
