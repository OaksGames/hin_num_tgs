using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tween_TickMark : MonoBehaviour
{
    public static Tween_TickMark myScript;

    public GameObject AlphaBG;
    public GameObject Obj_RoundBg;
    public GameObject Obj_TickMark;
    public GameObject[] Obj_Dots;

    private void Awake()
    {
        myScript = this;
        //Tween_In();
    }

    public void Tween_In()
    {
        float _delay = 0.15f;

        //AudioManager.myScript.PlayAudio(AudioManager.myScript.Audio_CorrectAnsr);        

        AlphaScript.AlphFade(AlphaBG.gameObject, 0.95f, 0.25f, 0, iTween.EaseType.linear, true);

        iTween.ScaleTo(Obj_RoundBg.gameObject, iTween.Hash("Scale", Vector3.one, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));

        for (int i = 0; i < Obj_Dots.Length; i++)
        {
            iTween.ScaleTo(Obj_Dots[i].gameObject, iTween.Hash("Scale", Vector3.one, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            _delay += 0.05f;
        }

        iTween.ValueTo(this.gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.25, "delay", 0.25f, "easetype", iTween.EaseType.linear,
                "onupdate", "FillMarker", "oncomplete", "OnTextFadeComplete", "oncompletetarget", this.gameObject));

        iTween.ScaleTo(Obj_TickMark.gameObject, iTween.Hash("Scale", Vector3.one, "time", 0.25f, "delay", 0.25f, "easetype", iTween.EaseType.easeOutBounce));

        Invoke("Tween_Out", 1.35f);
    }

    public void FillMarker(float _value)
    {
        Obj_TickMark.GetComponent<Image>().fillAmount = _value;
    }

    public void Tween_Out()
    {
        AlphaScript.AlphFade(AlphaBG.gameObject, 0, 0.2f, 0, iTween.EaseType.linear, false);

        iTween.ScaleTo(Obj_RoundBg.gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.2f, "delay", 0, "easetype", iTween.EaseType.linear));

        for (int i = 0; i < Obj_Dots.Length; i++)
        {
            iTween.ScaleTo(Obj_Dots[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.2f, "delay", 0, "easetype", iTween.EaseType.linear));
        }

        Obj_TickMark.GetComponent<Image>().fillAmount = 0;

        iTween.ScaleTo(Obj_TickMark.gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.2f, "delay", 0, "easetype", iTween.EaseType.linear));


        //GameManager.myScript.Invoke("SetLevelTarget",0.25f);
    }
}
