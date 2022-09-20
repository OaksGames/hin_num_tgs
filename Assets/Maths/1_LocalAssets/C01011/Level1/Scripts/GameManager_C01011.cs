using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;


public class GameManager_C01011 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject[] Tut_Items;
    public GameObject[] Tut_AnswerObjs;
    public GameObject TutHand1, TutHand2;
    public GameObject Tut_OptionHolder;
    public GameObject Tut_ItemSet;
    
    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject OptionsHolder;
   
    public GameObject[] QuestionsHolder;
    public GameObject[] QuestionItems;
    

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int QuestionOrder1 = 0;
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public int[] QuestionsOrder2;
    public int QuestionOrder2 = 0;
    public List<int> WrongAnsweredQuestions2;
    public int WrongAnsweredQuestionOrder2;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int CurrentTempQuestion;

    int WrongAnsrsCount;

    int CurrentItem;
    public GameObject[] CurrentItems;

    public GameObject[] AnswerObjs;

    float AddValueInProgress = 0;
    float ValueAdd;
    public GameInputData gameInputData;
    public int TotalQues;
    public string Thisgamekey;
    public int[] Ques_1;
    public GameObject btn_Back;

    void Start()
    {
        if (Testing == true && FrameworkOff == true)
        {

            TotalQues = 9;
            Thisgamekey = "nc01g011";

            SetTutorial(gameInputData);
        }
        /*if (Is_Tutorial)
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
        btn_Back.SetActive(false);
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
        PlayAudio(Sound_Intro1, 2.5f);
        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length + 2.1f);
        iTween.ScaleFrom(Tut_Items[0].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 2f, "easetype", iTween.EaseType.easeOutElastic)); 
        iTween.ScaleFrom(Tut_Items[1].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 6f, "easetype", iTween.EaseType.easeOutElastic)); 
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 2f)); 
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 6f));
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 0,"islocal" , true, "time", 1f, "delay", Sound_Intro1[VOLanguage].clip.length + 7f, "easetype", iTween.EaseType.linear));
        float _delay = 0;
        for (int i = 0; i < Tut_AnswerObjs.Length; i++)
        {            
           iTween.ScaleFrom(Tut_AnswerObjs[i].gameObject, iTween.Hash("scale", Vector3.zero,"time", 0.5f, "delay", _delay+10f, "easetype", iTween.EaseType.easeOutElastic));
           StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+10f));
            _delay += 0.2f;           
        }
        Invoke("EnableAnimator", 2f);
        Invoke("CallIntro3", Sound_Intro3[VOLanguage].clip.length + 8f);
        
    }

   
    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;
    }
    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }
    public void EnableTutNoTRaycatTarget()
    {
        Tut_AnswerObjs[1].gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void CallIntro3()
    {        
        PlayAudioRepeated(Sound_Intro3);
        Invoke("EnableTutNoTRaycatTarget",Sound_Intro3[VOLanguage].clip.length+0.5f);               
    }
    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_AnswerObjs[1].gameObject.GetComponent<Text>().raycastTarget = false;
        Tut_AnswerObjs[1].gameObject.GetComponent<PopTweenCustom>().StartAnim();       
        StopAudio(Sound_Intro3);
        StopAudio(Sound_Intro2);
        StopAudio(Sound_Intro1);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro4);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }
    public void BtnAct_OkTut()
    {        
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro4);
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);
        CurrentItem = 0;       
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2= PlayAnswerVoiceOver(1, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);                      
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 2f);        
        Tut_AnswerObjs[1].gameObject.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 1f);
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 2000,"islocal",true, "time", 1f, "delay",  1.75f, "easetype", iTween.EaseType.linear));
        Invoke("SetGamePlay", LengthDelay + Sound_Intro5[VOLanguage].clip.length + 2f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        CurrentItem = 0;
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);
        LevelsHolder.gameObject.SetActive(true);
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length + QuestionsOrder2.Length;
        }
        if(Is_NeedRandomizedQuestions)
        { 
            QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); 
            QuestionsOrder2 = RandomArray_Int(QuestionsOrder2); 
        }
        List<string> QuesKeys = new List<string>();

        QuestionOrderList = new List<int>();
        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            //---------------------------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i];
            QuesKeys.Add(AddKey);
        }

        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));
        CurrentItems = new GameObject[3];
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;
        LevelsHolder.gameObject.SetActive(false);
        for (int i = 0; i < QuestionsHolder.Length; i++)
        {
            QuestionsHolder[i].gameObject.SetActive(false);
        }        
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (CurrentItems[i] != null)
               Destroy(CurrentItems[i].gameObject);
        }

        QuestionOrderListtemp = new List<int>();
        TutorialObj.gameObject.SetActive(false);
        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            CurrentTempQuestion = QuestionOrder1;
            CurrentItem = 0;
            QuestionOrder1++;
        }
        else
        if (QuestionOrder2 < (QuestionsOrder2.Length))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = QuestionOrder2;
            QuestionOrderListtemp.Remove(QuestionsOrder2[tempq]);
            CurrentQuestion = QuestionsOrder2[tempq];
            Debug.Log("Question No : " + QuestionOrder2 + " A : " + QuestionsOrder2[QuestionOrder2]);
            CurrentTempQuestion = QuestionOrder2;
            CurrentItem = 1;
            QuestionOrder2++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            CurrentTempQuestion = CurrentQuestion-1;
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            CurrentItem = 0;
            WrongAnsweredQuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = WrongAnsweredQuestionOrder2;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[tempq]);
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            CurrentTempQuestion = CurrentQuestion-1;
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            CurrentItem = 1;
            WrongAnsweredQuestionOrder2++;
        }

        float _delay1 = 0;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = "" + CurrentQuestion * 10;
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _ixx * 10;
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _iyy *10;
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _izz * 10;
                    QuestionOrderListtemp.Remove(_izz);
                }
            }
            
        }
        CurrentItems = new GameObject[CurrentQuestion];
        LevelsHolder.gameObject.SetActive(true);
        iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("x",512,"islocal",true , "time", 1f, "delay",  QVOLength + _delay1+0.75f, "easetype", iTween.EaseType.linear));       
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (i < 5)
            {
                QuestionsHolder[0].gameObject.SetActive(true);
                CurrentItems[i] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[0].transform);
            }
            if (i >= 5)
            {
                QuestionsHolder[1].gameObject.SetActive(true);
                 CurrentItems[i] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[1].transform);

            }
        }

        float _delay = 0;
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            iTween.ScaleTo(CurrentItems[i].gameObject, iTween.Hash("scale", Vector3.one, "time", 0.5f, "delay",_delay+0.75f, "easetype", iTween.EaseType.easeInOutSine));
           StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+0.75f));
            _delay += 0.1f;
        }
       
        Is_OkButtonPressed = false;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay",QVOLength + _delay1+0.75f, "easetype", iTween.EaseType.easeOutElastic));
            //StartCoroutine(PlayAudioAtOneShot(Sound_Ting,QVOLength + _delay1+0.75f));
            _delay1 += 0.1f;
        }
        PlayQuestionVoiceOver(Random.RandomRange(0, 2));
        Invoke("PlayQuestionVoiceOverWithDelay", CurrentItems.Length * 0.1f);
    }

    void PlayQuestionVoiceOverWithDelay()
    {
       //PlayQuestionVoiceOver(Random.RandomRange(0, 2));
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void EnableOptionsRaycast()
    {       
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (CurrentItem)
        {
            case 0:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Cookies[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Cookies);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Cookies[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Cookies);
                        break;
                }
                break;
            case 1:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Eggs[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Eggs);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Eggs[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Eggs);
                        break;
                }
                break;

        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        switch (CurrentItem)
        {
            case 0:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A1_Cookies, _delay);
                        ClipLength = Sound_A1_Cookies[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A2_Cookies, _delay);
                        ClipLength = Sound_A2_Cookies[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A3_Cookies, _delay);
                        ClipLength = Sound_A3_Cookies[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A4_Cookies, _delay);
                        ClipLength = Sound_A4_Cookies[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A5_Cookies, _delay);
                        ClipLength = Sound_A5_Cookies[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A6_Cookies, _delay);
                        ClipLength = Sound_A6_Cookies[VOLanguage].clip.length;
                        break;
                    case 6:
                        PlayAudio(Sound_A7_Cookies, _delay);
                        ClipLength = Sound_A7_Cookies[VOLanguage].clip.length;
                        break;
                    case 7:
                        PlayAudio(Sound_A8_Cookies, _delay);
                        ClipLength = Sound_A8_Cookies[VOLanguage].clip.length;
                        break;
                    case 8:
                        PlayAudio(Sound_A9_Cookies, _delay);
                        ClipLength = Sound_A9_Cookies[VOLanguage].clip.length;
                        break;
                    case 9:
                        PlayAudio(Sound_A10_Cookies, _delay);
                        ClipLength = Sound_A10_Cookies[VOLanguage].clip.length;
                        break;
                }
                break;
            case 1:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A1_Eggs, _delay);
                        ClipLength = Sound_A1_Eggs[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A2_Eggs, _delay);
                        ClipLength = Sound_A2_Eggs[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A3_Eggs, _delay);
                        ClipLength = Sound_A3_Eggs[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A4_Eggs, _delay);
                        ClipLength = Sound_A4_Eggs[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A5_Eggs, _delay);
                        ClipLength = Sound_A5_Eggs[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A6_Eggs, _delay);
                        ClipLength = Sound_A6_Eggs[VOLanguage].clip.length;
                        break;
                    case 6:
                        PlayAudio(Sound_A7_Eggs, _delay);
                        ClipLength = Sound_A7_Eggs[VOLanguage].clip.length;
                        break;
                    case 7:
                        PlayAudio(Sound_A8_Eggs, _delay);
                        ClipLength = Sound_A8_Eggs[VOLanguage].clip.length;
                        break;
                    case 8:
                        PlayAudio(Sound_A9_Eggs, _delay);
                        ClipLength = Sound_A9_Eggs[VOLanguage].clip.length;
                        break;
                    case 9:
                        PlayAudio(Sound_A10_Eggs, _delay);
                        ClipLength = Sound_A10_Eggs[VOLanguage].clip.length;
                        break;
                }
                break;

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
    public void Check_Answer(int _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i].name.Contains(UserAnsr.ToString()))
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

    void HighlightOptions()
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
    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        StopRepetedAudio();
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;

        PlayAudio(Sound_BtnOkClick, 0);

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
            if (Testing)
            {
                ProgreesBar.GetComponent<Slider>().value += 1;
            }
            Total_CorrectAnswers++;//INGAME_COMMON
            WrongAnsrsCount = 0;
            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length+0.25f)+ Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion-1, LengthDelay +0.5f);            
            iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("x", 2000,"islocal",true, "time", 1f, "delay", LengthDelay + 1.5f, "easetype", iTween.EaseType.linear));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);
            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 1.5f));
            
            if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                QuestionOrder2 < (QuestionsOrder2.Length) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))

            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                Debug.Log("Game Over C");
                //Invoke("ShowLC", LengthDelay + LengthDelay2 + 3);
                SendResultFinal();
            }
            CancelInvoke("RepeatQVOAftertChoosingOption");
        }
        else
        {

            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Wrong);
                Debug.Log("Add : " + AddKey + ": Wrong");

            }
            Debug.Log("WRONG ANSWER : " + UserAnsr);
            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                if (AnswerObjs[i].name.Contains(UserAnsr.ToString()))
                {

                    iTween.ShakePosition(AnswerObjs[i].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                }
            }
            PlayAudio(Sound_IncorrectAnswer, 0);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion-1, 1);
                iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("x", 2000, "islocal",true,"time", 1f, "delay", LengthDelay + 1.5f, "easetype", iTween.EaseType.linear));
                Invoke("HighlightOptions", 0.5f);
                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    if (AnswerObjs[i] != null)
                    {
                        AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                    }
                }

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionsOrder1.Length)
                    && QuestionOrder2 == 0)
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionsOrder2.Length))
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                }
                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                    QuestionOrder2 < (QuestionsOrder2.Length) ||
                    WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))

                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Game Over W");
                    StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + 2f));
                    //Invoke("ShowLC", LengthDelay + 2.5f);
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

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
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

    #region INGAME_COMMON

    public int Total_CorrectAnswers;
    public void ShowLC()
    {
        InGameManager.instance.Activity_Finished(QuestionsOrder1.Length, Total_CorrectAnswers);
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

    public AudioSource[] Sound_Q1_Cookies;
    public AudioSource[] Sound_Q2_Cookies;

    public AudioSource[] Sound_Q1_Eggs;
    public AudioSource[] Sound_Q2_Eggs;

    public AudioSource[] Sound_A1_Cookies;
    public AudioSource[] Sound_A2_Cookies;
    public AudioSource[] Sound_A3_Cookies;
    public AudioSource[] Sound_A4_Cookies;
    public AudioSource[] Sound_A5_Cookies;
    public AudioSource[] Sound_A6_Cookies;
    public AudioSource[] Sound_A7_Cookies;
    public AudioSource[] Sound_A8_Cookies;
    public AudioSource[] Sound_A9_Cookies;
    public AudioSource[] Sound_A10_Cookies;

    public AudioSource[] Sound_A1_Eggs;
    public AudioSource[] Sound_A2_Eggs;
    public AudioSource[] Sound_A3_Eggs;
    public AudioSource[] Sound_A4_Eggs;
    public AudioSource[] Sound_A5_Eggs;
    public AudioSource[] Sound_A6_Eggs;
    public AudioSource[] Sound_A7_Eggs;
    public AudioSource[] Sound_A8_Eggs;
    public AudioSource[] Sound_A9_Eggs;
    public AudioSource[] Sound_A10_Eggs;

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
    public AudioSource Sound_BtnOkClick;

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
            yield return new WaitForSeconds(7+QVOLength);
            StartCoroutine("PlayAudioRepeatedCall");
        }
    }

    public void StopRepetedAudio()
    {
        StopAudio(_audiotorepeat);
        StopCoroutine("PlayAudioRepeatedCall");
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



