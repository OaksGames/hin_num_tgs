using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaTween : MonoBehaviour 
{
	public static AlphaTween myScript;
	public bool IsNeedObjectFalse=true;

	public void ApplyAlphaTween(GameObject TweenObj,float AlphaTo,float TweenTime,float DelayTime,iTween.EaseType EaseType, bool IsObjActive)
	{
		GameObject TempObj = TweenObj;
		TempObj.gameObject.SetActive (true);
		IsNeedObjectFalse = IsObjActive;

		if (TweenObj.GetComponent<Text>()!=null) 
		{
			iTween.ValueTo (TweenObj.gameObject, iTween.Hash ("from", TweenObj.gameObject.GetComponent<Text> ().color.a, "to", AlphaTo, "time", TweenTime, "delay", DelayTime, "easetype", EaseType,
				"onupdate", "TextFade", "oncomplete", "OnTextFadeComplete", "oncompletetarget", TweenObj.gameObject));
		} 
		else
		if (TweenObj.GetComponent<Image>()!=null) 
		{
			iTween.ValueTo (TweenObj.gameObject, iTween.Hash ("from", TweenObj.gameObject.GetComponent<Image> ().color.a, "to", AlphaTo, "time", TweenTime, "delay", DelayTime, "easetype", EaseType, 
					"onupdate", "ImageFade", "oncomplete", "OnImageFadeComplete", "oncompletetarget", TweenObj.gameObject));
		}
		else
		if (TweenObj.GetComponent<SpriteRenderer> () != null)
		{
			iTween.ValueTo (TweenObj.gameObject, iTween.Hash ("from", TweenObj.gameObject.GetComponent<SpriteRenderer> ().color.a, "to", AlphaTo, "time", TweenTime, "delay", DelayTime, "easetype",EaseType, 
						"onupdate", "SpriteRendererFade", "oncomplete", "OnSpriteFadeComplete", "oncompletetarget", TweenObj.gameObject));
		}
	}

	public void TextFade (float value)
	{
		this.gameObject.GetComponent<Text> ().color = new Color (this.gameObject.GetComponent<Text> ().color.r, this.gameObject.GetComponent<Text> ().color.g, this.gameObject.GetComponent<Text> ().color.b, value);
	}
	public void ImageFade (float value)
	{
		this.gameObject.GetComponent<Image> ().color = new Color (this.gameObject.GetComponent<Image> ().color.r, this.gameObject.GetComponent<Image> ().color.g, this.gameObject.GetComponent<Image> ().color.b, value);
	}
	public void SpriteRendererFade (float value)
	{
		this.gameObject.GetComponent<SpriteRenderer> ().color = new Color (this.gameObject.GetComponent<SpriteRenderer> ().color.r, this.gameObject.GetComponent<SpriteRenderer> ().color.g, this.gameObject.GetComponent<SpriteRenderer> ().color.b, value);
	}

	public void OnTextFadeComplete ()
	{
		if (this.GetComponent<AlphaTween> () != null)
		{
			Destroy (this.GetComponent<AlphaTween> ());
		}
		if(!IsNeedObjectFalse)
		{
			this.gameObject.SetActive(false);
		}
	}

	public void OnImageFadeComplete ()
	{
		if (this.GetComponent<AlphaTween> () != null)
		{
			Destroy (this.GetComponent<AlphaTween> ());
		}
		if(!IsNeedObjectFalse)
		{
			this.gameObject.SetActive(false);
		}
	}

	public void OnSpriteFadeComplete ()
	{
		if (this.GetComponent<AlphaTween> () != null)
		{
			Destroy (this.GetComponent<AlphaTween> ());
		}
		if(!IsNeedObjectFalse)
		{
			this.gameObject.SetActive(false);
		}
	}

	void OnDisable()
	{
		if (this.GetComponent<AlphaTween> () != null)
		{
			Destroy (this.GetComponent<AlphaTween> ());
		}
	}
}
