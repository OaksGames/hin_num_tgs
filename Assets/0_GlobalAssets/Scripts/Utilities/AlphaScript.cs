using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AlphaScript : MonoBehaviour 
{		
	public GameObject CurrentObject;
	public static bool IsNeedObjectFalse;

	public static void AlphFade(GameObject TweenObj,float AlphaTo,float TweenTime,float DelayTime,iTween.EaseType EaseType, bool ObjActive)
	{
		TweenObj.AddComponent<AlphaTween> ();
		TweenObj.GetComponent<AlphaTween>().ApplyAlphaTween(TweenObj,AlphaTo,TweenTime,DelayTime,EaseType,ObjActive);
	}
}
