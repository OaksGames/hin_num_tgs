using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_D02012 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;    
    public GameObject Tut_Items;
    public GameObject Tut_Sidepanel;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;
    public int[] QuestionsOrder2;
    public int QuestionOrder2;

    public List<int> WrongAnsweredQuestions2;
    public int WrongAnsweredQuestionOrder2;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int CurrentQuestionOrder;

    int WrongAnsrsCount;

    public GameObject SidePanel;
    public GameObject[] AnswerObjs;

    public GameObject Obj_Hours;
    public GameObject Obj_Minutes;
     int CurrentItem;

    float AddValueInProgress = 0;
    float ValueAdd;
    public GameInputData gameInputData;
    public int TotalQues;
    public string Thisgamekey;
    public int[] Ques_1;

    void Start()
    {
        if (Testing == true && FrameworkOff == true)
        {
            TotalQues = 5;
            Thisgamekey = "na01081";

            SetTutorial(gameInputData);
        }
        /* if (Is_Tutorial)
         {
             SetTutorial();
         }
         else
         {
             SetGamePlay();
         }*/
    }

   

    public void StartGame(GameInputData data)
    {
        ProgreesBar.SetActive(false);
        //btn_Back.SetActive(false);
        SetTutorial(data);
    }

    public void CleanUp()
    {
        // throw new System.NotImplementedException();
    }

    #region TUTORIAL
    public void SetTutorial(GameInputData gameInputData)
    {
        if (FrameworkOff == false && Testing == false)
        {
            this.gameInputData = gameInputData;
            ////////////////What the value should add in progress bar aftereach question//////////////////
            TotalQues = gameInputData.Mechanics.Count;
            //***************************************************
            AddValueInProgress = 1 / (float)QuestionsOrder1.Length;
            Thisgamekey = gameInputData.Key;
        }

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 2f);
        
        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length + 1.5f);
        PlayAudio(Sound_Intro3, Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + 1.5f);
        PlayAudio(Sound_Intro4, Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro3[VOLanguage].clip.length + 3.5f);
        Invoke("EnableAnimator", 2f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro3[VOLanguage].clip.length + Sound_Intro4[VOLanguage].clip.length + 3f);
        Invoke("CallIntro3", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro3[VOLanguage].clip.length + Sound_Intro4[VOLanguage].clip.length + 4f);
        Invoke("Enabletween", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro3[VOLanguage].clip.length + 0.5f);

    }
    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;
    }
    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }
    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro5);
        
    }
    public void Enabletween()
    {
        iTween.MoveTo(Tut_Sidepanel.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 1f, "delay", 0, "easetype", iTween.EaseType.linear));
        float _delay = 0f;
        for (int i = 0; i < Tut_Sidepanel.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Sidepanel.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay+1f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+1f));
            _delay = 0.1f;
        }
    }
    public void CallIntro3()
    {
        
        Tut_Sidepanel.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void Selected_TutAnswer()
    {
        //StopAudio(Sound_Intro5);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_Sidepanel.transform.GetChild(1).GetComponent<Text>().raycastTarget = false;
        Tut_Sidepanel.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();
        //StopAudio(Sound_Intro5);
        //StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro5);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        //StopAudio(Sound_Intro6);
        StopRepetedAudio();
        PlayAudio(Sound_Btn_Ok_Click, 0);
        TutHand2.gameObject.SetActive(false);

        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(16, LengthDelay+0.25f);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 1f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 1f);
        Tut_Sidepanel.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + LengthDelay2 + 3f);
        iTween.MoveTo(Tut_Sidepanel.gameObject, iTween.Hash("x", 5000, "time", 1f, "delay", 8.5f, "easetype", iTween.EaseType.linear));
        PlayAudio(Sound_Intro6, LengthDelay + LengthDelay2 + 2f);
        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro6[VOLanguage].clip.length + 2.5f);
    }
    #endregion
    #region LEVEL
    public void SetGamePlay()
    {
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);
        LevelsHolder.gameObject.SetActive(true);
        ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length + QuestionsOrder2.Length;
        if (Is_NeedRandomizedQuestions)
        {
            QuestionsOrder1 = RandomArray_Int(QuestionsOrder1);
            QuestionsOrder2 = RandomArray_Int(QuestionsOrder2);
        }

        QuestionOrderList = new List<int>();
        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i].ToString();
            QuesKeys.Add(AddKey);

        }

        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));
        GenerateLevel();
    }

    int RandAnsrIndex = 0;
    public void GenerateLevel()
    {
        TutorialObj.gameObject.SetActive(false);
        int tempq = 0;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        QuestionOrderListtemp = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {           
            QuestionOrderListtemp.Add(i + 1);
        }

        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentQuestionOrder = QuestionOrder1;
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
            CurrentItem=0;
        }
        else
        if (QuestionOrder2 < (QuestionsOrder2.Length))
        {
            tempq = QuestionOrder2;
            QuestionOrderListtemp.Remove(QuestionsOrder2[tempq]);
            CurrentQuestion = QuestionsOrder2[tempq];
            CurrentQuestionOrder = QuestionOrder2;
            Debug.Log("Question No : " + QuestionOrder2 + " A : " + QuestionsOrder2[QuestionOrder2]);
            QuestionOrder2++;
             CurrentItem=1;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);           
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            TargetArray = QuestionsOrder1;
            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
             CurrentItem=0;
        }
        else
        if (WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
        {
            tempq = WrongAnsweredQuestionOrder2;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[tempq]);           
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            TargetArray = QuestionsOrder2;
            CurrentQuestionOrder = WrongAnsweredQuestions2[tempq];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            WrongAnsweredQuestionOrder2++;
            CurrentItem=1;
        }

        if(CurrentItem == 0)
        {

            
            RandAnsrIndex = Random.Range(0, 3);
            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                if (RandAnsrIndex == i)
                {
                    if (CurrentQuestion <= 9)
                    { AnswerObjs[i].GetComponent<Text>().text = "0" + CurrentQuestion + ":15"; }
                    else
                    { AnswerObjs[i].GetComponent<Text>().text = "" + CurrentQuestion + ":15"; }
                    CorrectAnsrIndex = RandAnsrIndex;
                }
                else
                {
                    if (i == 0)
                    {
                        int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                        if (_ixx <= 9)
                        { AnswerObjs[i].GetComponent<Text>().text = "0" + _ixx + ":15"; }
                        else
                        { AnswerObjs[i].GetComponent<Text>().text = "" + _ixx + ":15"; }
                        QuestionOrderListtemp.Remove(_ixx);
                    }
                    else
                    if (i == 1)
                    {
                        int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                        if (_iyy <= 9)
                        { AnswerObjs[i].GetComponent<Text>().text = "0" + _iyy + ":15"; }
                        else
                        { AnswerObjs[i].GetComponent<Text>().text = "" + _iyy + ":15"; }
                        QuestionOrderListtemp.Remove(_iyy);
                    }
                    else
                    if (i == 2)
                    {
                        int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                        if (_izz <= 9)
                        { AnswerObjs[i].GetComponent<Text>().text = "0" + _izz + ":15"; }
                        else
                        { AnswerObjs[i].GetComponent<Text>().text = "" + _izz + ":15"; }
                        QuestionOrderListtemp.Remove(_izz);
                    }
                }
            
            }
        }
        else
        {
          
            RandAnsrIndex = Random.Range(0, 3);
            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                if (RandAnsrIndex == i)
                {
                    if (CurrentQuestion <= 9)
                    { AnswerObjs[i].GetComponent<Text>().text = "0" + CurrentQuestion + ":45"; }
                    else
                    { AnswerObjs[i].GetComponent<Text>().text = "" + CurrentQuestion + ":45"; }
                    CorrectAnsrIndex = RandAnsrIndex;
                }
                else
                {
                    if (i == 0)
                    {
                        int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                        if (_ixx <= 9)
                        { AnswerObjs[i].GetComponent<Text>().text = "0" + _ixx + ":45"; }
                        else
                        { AnswerObjs[i].GetComponent<Text>().text = "" + _ixx + ":45"; }
                        QuestionOrderListtemp.Remove(_ixx);
                    }
                    else
                    if (i == 1)
                    {
                        int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                        if (_iyy <= 9)
                        { AnswerObjs[i].GetComponent<Text>().text = "0" + _iyy + ":45"; }
                        else
                        { AnswerObjs[i].GetComponent<Text>().text = "" + _iyy + ":45"; }
                        QuestionOrderListtemp.Remove(_iyy);
                    }
                    else
                    if (i == 2)
                    {
                        int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                        if (_izz <= 9)
                        { AnswerObjs[i].GetComponent<Text>().text = "0" + _izz + ":45"; }
                        else
                        { AnswerObjs[i].GetComponent<Text>().text = "" + _izz + ":45"; }
                        QuestionOrderListtemp.Remove(_izz);
                    }
                }
            
            } 
        }

        LevelsHolder.gameObject.SetActive(true);
        CorrectAnsrIndex = RandAnsrIndex;
        if(CurrentItem == 0)
        {
            SetTime15(CurrentQuestion);
        }
        else
        {
            SetTime45(CurrentQuestion);
        }
        float _delay1 = 0.25f;
        Invoke("QuestionDelay",1.75f);
        iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 512, "time", 0.25f, "islocal", true, "delay", QVOLength, "easetype", iTween.EaseType.linear));
        Debug.Log("CorrectAnsrIndex : " + CurrentQuestion);
        Is_OkButtonPressed = false;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", QVOLength + _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, QVOLength+_delay1));
            _delay1 += 0.1f;
        }
        
    }
    void QuestionDelay()
    {
        float LengthDelay = PlayQuestionVoiceOver(Random.Range(0, 2));
        Invoke("EnableOptionsRaycast", QVOLength);
    }
    void EnableOptionsRaycast()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
            }
        }
    }

    public void SetTime15(int _TimeInput)
    {
         Is_CanClick = false;
        iTween.RotateTo(Obj_Hours.gameObject, iTween.Hash("z", ((-30 * _TimeInput)-10), "time", 1f, "delay", 0f, "easetype", iTween.EaseType.easeOutCirc));

        iTween.ValueTo(this.gameObject, iTween.Hash("from", Obj_Minutes.transform.localEulerAngles.z, "to", -90, "time", 1f, "delay", 0, "easetype", iTween.EaseType.linear,
               "onupdate", "SetMintutesNiddle", "oncompletetarget", this.gameObject));
               ClockHandmovingSet();
    }
    public void SetTime45(int _TimeInput)
    {
         Is_CanClick = false;
        iTween.RotateTo(Obj_Hours.gameObject, iTween.Hash("z", ((-30 * _TimeInput)-20), "time", 1f, "delay", 0f, "easetype", iTween.EaseType.easeOutCirc));

        iTween.ValueTo(this.gameObject, iTween.Hash("from", Obj_Minutes.transform.localEulerAngles.z, "to", -270, "time", 1f, "delay", 0, "easetype", iTween.EaseType.linear,
               "onupdate", "SetMintutesNiddle", "oncompletetarget", this.gameObject));
               ClockHandmovingSet();
    }

    public void SetMintutesNiddle(float _value)
    {
        Obj_Minutes.transform.localEulerAngles = new Vector3(0, 0, _value);
    }
    public void ClockHandmovingSet()
    {
        float _delay1 = 0;
        for (int i = 0; i < 20; i++)
        {
            StartCoroutine(PlayAudioAtOneShot(ClockHandmoving, _delay1));
            _delay1 += 0.05f;
        }
    }


    void SetMinuteAngle(float _v)
    {
        //Obj_Minutes.gameObject.transform.eulerAngles = new Vector3(0, 0, Obj_Minutes.gameObject.transform.eulerAngles.z);

        //Obj_Minutes.transform.RotateAround(Obj_Minutes.transform.position, new Vector3(0,0,-1),3.6f);

       
    }

    float PlayQuestionVoiceOver(int _Qi)
    {
        float ClipLength = 0;
        switch (_Qi)
        {
            case 0:
                PlayAudioRepeated(Sound_Q1);
                QVOLength = Sound_Q1[VOLanguage].clip.length;
                break;
            case 1:
                PlayAudioRepeated(Sound_Q2);
                QVOLength = Sound_Q2[VOLanguage].clip.length;
                break;
            case 2:
                PlayAudioRepeated(Sound_Q3);
                QVOLength = Sound_Q3[VOLanguage].clip.length;
                break;
        }
        return ClipLength;
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        switch(CurrentItem)
        {
            case 0:
                switch (_Ai)
                {
                    case 0:
                     PlayAudio(Sound_A1, _delay);
                        ClipLength = Sound_A1[VOLanguage].clip.length;
                    break;
                    case 1:
                        PlayAudio(Sound_A2, _delay);
                        ClipLength = Sound_A2[VOLanguage].clip.length;
                    break;
                    case 2:
                        PlayAudio(Sound_A3, _delay);
                        ClipLength = Sound_A3[VOLanguage].clip.length;
                    break;
                    case 3:
                        PlayAudio(Sound_A4, _delay);
                        ClipLength = Sound_A4[VOLanguage].clip.length;
                    break;
                    case 4:
                        PlayAudio(Sound_A5, _delay);
                        ClipLength = Sound_A5[VOLanguage].clip.length;
                    break;
                    case 5:
                        PlayAudio(Sound_A6, _delay);
                        ClipLength = Sound_A6[VOLanguage].clip.length;
                    break;
                    case 6:
                        PlayAudio(Sound_A7, _delay);
                        ClipLength = Sound_A7[VOLanguage].clip.length;
                    break;
                    case 7:
                        PlayAudio(Sound_A8, _delay);
                        ClipLength = Sound_A8[VOLanguage].clip.length;
                    break;
                    case 8:
                        PlayAudio(Sound_A9, _delay);
                        ClipLength = Sound_A9[VOLanguage].clip.length;
                    break;
                    case 9:
                        PlayAudio(Sound_A10, _delay);
                        ClipLength = Sound_A10[VOLanguage].clip.length;
                    break;
                    case 10:
                        PlayAudio(Sound_A11, _delay);
                        ClipLength = Sound_A11[VOLanguage].clip.length;
                    break;
                    case 11:
                        PlayAudio(Sound_A12, _delay);
                        ClipLength = Sound_A12[VOLanguage].clip.length;
                    break;
                }
            break;
            case 1:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A13, _delay);
                        ClipLength = Sound_A13[VOLanguage].clip.length;
                    break;
                    case 1:
                        PlayAudio(Sound_A14, _delay);
                        ClipLength = Sound_A14[VOLanguage].clip.length;
                    break;
                    case 2:
                        PlayAudio(Sound_A15, _delay);
                        ClipLength = Sound_A15[VOLanguage].clip.length;
                    break;
                    case 3:
                        PlayAudio(Sound_A16, _delay);
                        ClipLength = Sound_A16[VOLanguage].clip.length;
                    break;
                    case 4:
                        PlayAudio(Sound_A17, _delay);
                        ClipLength = Sound_A17[VOLanguage].clip.length;
                     break; 
                    case 5:
                        PlayAudio(Sound_A18, _delay);
                        ClipLength = Sound_A18[VOLanguage].clip.length;
                    break;
                    case 6:
                        PlayAudio(Sound_A19, _delay);
                        ClipLength = Sound_A19[VOLanguage].clip.length;
                    break;
                    case 7:
                        PlayAudio(Sound_A20, _delay);
                        ClipLength = Sound_A20[VOLanguage].clip.length;
                        break;
                    case 8:
                        PlayAudio(Sound_A21, _delay);
                        ClipLength = Sound_A21[VOLanguage].clip.length;
                    break;
                    case 9:
                        PlayAudio(Sound_A22, _delay);
                        ClipLength = Sound_A22[VOLanguage].clip.length;
                    break; 
                    case 10:
                        PlayAudio(Sound_A23, _delay);
                        ClipLength = Sound_A23[VOLanguage].clip.length;
                    break;  
                    case 11:
                        PlayAudio(Sound_A24, _delay);
                        ClipLength = Sound_A24[VOLanguage].clip.length;
                    break;                           
                }
            break;
        }
        return ClipLength;
    }

    int _appri;
    public float PlayAppreciationVoiceOver(float _delay)
    {
        float ClipLength = 0;
        _appri++;
        switch (_appri)
        {
            case 0:
                PlayAudio(Appriciation_Good, _delay);
                ClipLength = Appriciation_Good[VOLanguage].clip.length;
                break;
            case 1:
                PlayAudio(Appriciation_Great, _delay);
                ClipLength = Appriciation_Great[VOLanguage].clip.length;
                break;
            case 2:
                PlayAudio(Appriciation_Excellent, _delay);
                ClipLength = Appriciation_Excellent[VOLanguage].clip.length;
                break;
            case 3:
                PlayAudio(Appriciation_Nice, _delay);
                ClipLength = Appriciation_Nice[VOLanguage].clip.length;
                break;
            case 4:
                PlayAudio(Appriciation_Splended, _delay);
                ClipLength = Appriciation_Good[VOLanguage].clip.length;
                break;
            case 5:
                PlayAudio(Appriciation_Weldone, _delay);
                ClipLength = Appriciation_Weldone[VOLanguage].clip.length;
                break;
        }
        if (_appri >= 5)
        {
            _appri = 0;
        }
        return ClipLength;
    }

    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        Is_CanClick = _IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);
    }

    int UserAnsr;
    public void Check_Answer(int _UserInput)
    {
        StopRepetedAudio();
        UserAnsr = _UserInput;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (i == _UserInput)
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption", 7);
    }

    void RepeatQVOAftertChoosingOption()
    {
        StartCoroutine("PlayAudioRepeatedCall");
    }

    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;
        PlayAudio(Sound_Btn_Ok_Click, 0);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
        }
        StopRepetedAudio();
        if (CorrectAnsrIndex == UserAnsr)
        {
            /// IF RIGHT ANSWER
            ValueAdd = ValueAdd + AddValueInProgress;
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.UpdateProgress(ValueAdd);
            Debug.Log("progressBar Value Add : " + ValueAdd);

            // update the ResultData in the framework
            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Correct);
                Debug.Log("Add : " + AddKey + ": Correct");
            }

            Debug.Log("CORRECT ANSWER : " + UserAnsr);
            //ProgreesBar.GetComponent<Slider>().value += 1;
            //INGAME_COMMON
            //MultiLevelManager.instance.UpdateProgress(1, 1);
            //INGAME_COMMON
            WrongAnsrsCount = 0;
            StopRepetedAudio();
            float LengthDelay = PlayAppreciationVoiceOver(0.5f);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion-1, LengthDelay+0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);
            iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 5000, "time", 1f, "delay", LengthDelay + LengthDelay2+0.5f, "easetype", iTween.EaseType.linear));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);
             if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                QuestionOrder2 < (QuestionsOrder2.Length) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                Debug.Log("Questions Finished");
                //Invoke("ShowLC", LengthDelay + LengthDelay2 + 4f);
                SendResultFinal();
            }
            CancelInvoke("RepeatQVOAftertChoosingOption");
        }
        else
        {
            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                if (AnswerObjs[i].name.Contains(UserAnsr.ToString()))
                {
                    iTween.ShakePosition(AnswerObjs[i].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                }
            }

            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Wrong);
                Debug.Log("Add : " + AddKey + ": Wrong");
            }

            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion-1, 1f);
                Invoke("HighlightCorrectOption", 1f);
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 5000, "time", 0.25f, "delay", LengthDelay+1.25f, "easetype", iTween.EaseType.linear));
                //TargetList = WrongAnsweredQuestions1;
                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionsOrder1.Length) && CurrentItem==0)
                {                  
                    WrongAnsweredQuestions1.Add(CurrentQuestion);                    
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionsOrder2.Length) && CurrentItem==1)
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                    //INGAME_COMMON
                    //MultiLevelManager.instance.UpdateProgress(1, 0);
                    //INGAME_COMMON
                }

                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                   QuestionOrder2 < (QuestionsOrder2.Length) ||
                   WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                   WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Questions Finished");
                    //Invoke("ShowLC", LengthDelay + 3f);
                    SendResultFinal();
                }
                CancelInvoke("RepeatQVOAftertChoosingOption");
                WrongAnsrsCount = 0;
            }
            else
            {
                Is_OkButtonPressed = false;
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("RepeatQVOAftertChoosingOption", 1);
                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
                    AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }
    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
        }

    }

    void HighlightCorrectOption()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
    }

    int[] TargetArray;
    int FindIndexofElementinArray(int _TargetElement)
    {
        int _returnElement = 0;
        for (int i = 0; i < TargetArray.Length; i++)
        {
            if (TargetArray[i] == _TargetElement)
            {
                _returnElement = i;
            }
        }
        return _returnElement;
    }

    List<int> TargetList;
    int FindIndexofElementinList(int _TargetElement)
    {
        int _returnElement = 0;
        for (int i = 0; i < TargetList.Count; i++)
        {
            if (TargetList[i] != 0)
            {
                if (TargetList[i] == _TargetElement)
                {
                    _returnElement = i;
                }
            }
        }
        return _returnElement;
    }

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }

    #region INGAME_COMMON

    public void ShowLC()
    {
        if (!MultiLevelManager.instance.CheckForNextLevel())
        {
            InGameManager.instance.Activity_Finished(MultiLevelManager.instance.Total_Questions, MultiLevelManager.instance.Total_CorrectAnswers);
        }
    }

    public void BtnAct_Back()
    {
        InGameManager.instance.BtnAct_BackInGame();
    }

    #endregion INGAME_COMMON

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;
    public AudioSource[] Sound_Intro5;
    public AudioSource[] Sound_Intro6;
    

    public AudioSource[] Sound_Q1;
    public AudioSource[] Sound_Q2;
    public AudioSource[] Sound_Q3;

    public AudioSource[] Sound_A1;
    public AudioSource[] Sound_A2;
    public AudioSource[] Sound_A3;
    public AudioSource[] Sound_A4;
    public AudioSource[] Sound_A5;
    public AudioSource[] Sound_A6;
    public AudioSource[] Sound_A7;
    public AudioSource[] Sound_A8;
    public AudioSource[] Sound_A9;
    public AudioSource[] Sound_A10;
    public AudioSource[] Sound_A11;
    public AudioSource[] Sound_A12;
    public AudioSource[] Sound_A13;
    public AudioSource[] Sound_A14;
    public AudioSource[] Sound_A15;
    public AudioSource[] Sound_A16;
    public AudioSource[] Sound_A17;
    public AudioSource[] Sound_A18;
    public AudioSource[] Sound_A19;
    public AudioSource[] Sound_A20;
    public AudioSource[] Sound_A21;
    public AudioSource[] Sound_A22;
    public AudioSource[] Sound_A23;
    public AudioSource[] Sound_A24;

    public AudioSource[] Appriciation_Good;
    public AudioSource[] Appriciation_Excellent;
    public AudioSource[] Appriciation_Great;
    public AudioSource[] Appriciation_Nice;
    public AudioSource[] Appriciation_Splended;
    public AudioSource[] Appriciation_Weldone;

    public AudioSource Sound_CorrectAnswer;
    public AudioSource Sound_IncorrectAnswer;
    public AudioSource Sound_Selection;

    public AudioSource Sound_Ting;
    public AudioSource Sound_Btn_Ok_Click;
     public AudioSource ClockHandmoving;

    public void PlayAudio(AudioSource _audio, float _delay)
    {
        _audio.PlayDelayed(_delay);
    }
    public IEnumerator PlayAudioAtOneShot(AudioSource _audio, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _audio.PlayOneShot(_audio.clip);
    }
    public void PlayAudio(AudioSource[] _audio, float _delay)
    {
        _audio[VOLanguage].PlayDelayed(_delay);
    }
    public void StopAudio(AudioSource[] _audio)
    {
        _audio[VOLanguage].Stop();
    }
    public void PlayAudioRepeated(AudioSource[] _audio)
    {
        _audiotorepeat = _audio;
        StartCoroutine("PlayAudioRepeatedCall");
    }
    AudioSource[] _audiotorepeat;
    float QVOLength;
    IEnumerator PlayAudioRepeatedCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            _audiotorepeat[VOLanguage].PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedCall");
        }
    }

    public void StopRepetedAudio()
    {
        StopAudio(_audiotorepeat);
        StopCoroutine("PlayAudioRepeatedCall");
    }
    #endregion

    #region RANDOMIZE AN ARRAY
    public static int[] RandomArray_Int(int[] _SourceArray)
    {
        for (int i = 0; i < _SourceArray.Length; i++)
        {
            int tmp = _SourceArray[i];
            int rand = Random.Range(i, _SourceArray.Length);
            _SourceArray[i] = _SourceArray[rand];
            _SourceArray[rand] = tmp;
        }
        return _SourceArray;
    }
    #endregion

    #region RANDOM INT FROM A LIST
    public static int RandomNoFromList_Int(List<int> _SourceList)
    {
        int _randreturnno = 0;

        List<int> _templist = _SourceList;

        int _pickrandno = Random.Range(0, _SourceList.Count);

        _randreturnno = _SourceList[_pickrandno];

        return _randreturnno;
    }
    #endregion

    #region RANDOM INT FROM A ARRAY
    public static int RandomNoFromList_Int(int[] _SourceList)
    {
        int _randreturnno = 0;

        int[] _templist = _SourceList;

        int _pickrandno = Random.Range(0, _SourceList.Length);

        _randreturnno = _SourceList[_pickrandno];

        return _randreturnno;
    }
    #endregion
    
}
