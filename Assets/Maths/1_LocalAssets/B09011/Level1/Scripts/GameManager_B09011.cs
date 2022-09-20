using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B09011 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Item;
    public GameObject Tut_HoursHand;
    public Text Tut_Txt_DigitalTime;
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

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int CurrentQuestionOrder;

    int WrongAnsrsCount;
    
    public GameObject[] AnswerObjs;

    public GameObject Obj_Hours;
    public GameObject Obj_Minites;

    public Text Txt_DigitalTime;

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
            Thisgamekey = "na08041";

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
            AddValueInProgress = 1 / (float)TotalQues;
            Thisgamekey = gameInputData.Key;
        }

        SetQues(TotalQues, Thisgamekey);

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);

        PlayAudio(Sound_Intro1,2f);
        PlayAudio(Sound_Intro2, 2f+ Sound_Intro1[VOLanguage].clip.length);     
        
        Invoke("EnableAnimator", 2);
        Invoke("CallIntro3", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + 2);
    }

    void SetQues(int TotalQues, string Thisgamekey)
    {

        int[] QuesTemp = new int[TotalQues];
        Ques_1 = new int[TotalQues];
        int j = 0;
        int random = Random.Range(0, 5);
        if (random == 0)
            j = 0;
        else if (random == 1)
            j = 2;
        else if (random == 2)
            j = 4;
        else if (random == 3)
            j = 6;
        else if (random == 4)
            j = 8;

        //Debug.Log("Picked Ques from : " + j);
        int p = 0;
        for (int i = j; i < (j + TotalQues); i++)
        {
            QuesTemp[p] = Ques_1[p];
            p++;
        }

        System.Array.Copy(QuesTemp, Ques_1, QuesTemp.Length);

        // create a list of questions being posed in the game
        List<string> QuesKeys = new List<string>();


        // Add the questions keys to the list
        for (int i = 0; i < TotalQues; i++)
        {
            // example key A05011_Q01
            string AddKey = "" + Thisgamekey + "_Q" + Ques_1[i];
            QuesKeys.Add(AddKey);
            Debug.Log("Add : " + AddKey);
        }
        // send the list of questions to initialize the 
        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

    }


    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;
    }

    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }

    public void CallIntro3()
    {
        //StopAudio(Sound_Intro2);
        //PlayAudioRepeated(Sound_Intro3);
        Tut_Item.transform.gameObject.GetComponent<Text>().raycastTarget = true;
    }

    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);

        Tut_Item.transform.GetComponent<Text>().raycastTarget = false;
        Tut_Item.transform.GetComponent<PopTweenCustom>().StartAnim();

        iTween.RotateTo(Tut_HoursHand.gameObject, iTween.Hash("z",-30, "time", 0.15f, "delay", 0, "easetype", iTween.EaseType.easeOutCirc));

        StopAudio(Sound_Intro2);
        StopRepetedAudio();
        PlayAudio(Sound_Selection, 0);
        PlayAudioRepeated(Sound_Intro3);

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

        Tut_Txt_DigitalTime.text= "01:00";

        float LengthDelay = Sound_Intro4[VOLanguage].clip.length;
        PlayAudio(Sound_Intro4,0);
        PlayAppreciationVoiceOver(LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + 1f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 1f);
        Tut_Txt_DigitalTime.transform.GetComponent<PopTweenCustom>().Invoke("StartAnim", 1f);

        Tut_Txt_DigitalTime.transform.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 2f);
        Tut_Item.transform.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 2f);

        PlayAudio(Sound_Intro5, LengthDelay + 2f);

        Invoke("SetGamePlay", LengthDelay + Sound_Intro5[VOLanguage].clip.length + 2.5f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);

        LevelsHolder.gameObject.SetActive(true);

        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length - 1;
        }
        

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));

        GenerateLevel();
    }

    int RandAnsrIndex = 0;
    public void GenerateLevel()
    {
        TutorialObj.gameObject.SetActive(false);
        Txt_DigitalTime.GetComponent<PopTweenCustom>().StopAnim();

        int tempq = 0;
        
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                //AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        SetTime(1);

        QuestionOrderListtemp = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            // LIST IS FOR SELECTING OPTION NUMBERS
            QuestionOrderListtemp.Add(i+1);
        }

        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {            
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentQuestionOrder = QuestionOrder1;
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            //CurrentQuestion = WrongAnsweredQuestions1[WrongAnsweredQuestions1[tempq]];
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            TargetArray = QuestionsOrder1;
            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }
       
        LevelsHolder.gameObject.SetActive(true);

        CorrectAnsrIndex = CurrentQuestion;

        SetValueTo(CurrentQuestion);

        Is_OkButtonPressed = false;

        float LengthDelay=PlayQuestionVoiceOver(CurrentQuestion - 1);

        Invoke("EnableOptionsRaycast", QVOLength);

        Debug.Log("CorrectAnsrIndex : " + CurrentQuestion);
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
    public void SetTime(int _TimeInput)
    {
        iTween.RotateTo(Obj_Hours.gameObject, iTween.Hash("z", (-30 * _TimeInput), "time", 0.15f, "delay", 0, "easetype", iTween.EaseType.easeOutCirc));        
    }

    public void SetValueTo(int _value)
    {
        if (_value.ToString().Length > 1)
        {
            Txt_DigitalTime.text = "" + (int)_value + ":00";
        }
        else
        {
            Txt_DigitalTime.text = "0" + (int)_value + ":00";
        }
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
            case 3:
                PlayAudioRepeated(Sound_Q4);
                QVOLength = Sound_Q4[VOLanguage].clip.length;
                break;
            case 4:
                PlayAudioRepeated(Sound_Q5);
                QVOLength = Sound_Q5[VOLanguage].clip.length;
                break;
            case 5:
                PlayAudioRepeated(Sound_Q6);
                QVOLength = Sound_Q6[VOLanguage].clip.length;
                break;
            case 6:
                PlayAudioRepeated(Sound_Q7);
                QVOLength = Sound_Q7[VOLanguage].clip.length;
                break;
            case 7:
                PlayAudioRepeated(Sound_Q8);
                QVOLength = Sound_Q8[VOLanguage].clip.length;
                break;
            case 8:
                PlayAudioRepeated(Sound_Q9);
                QVOLength = Sound_Q9[VOLanguage].clip.length;
                break;
            case 9:
                PlayAudioRepeated(Sound_Q10);
                QVOLength = Sound_Q10[VOLanguage].clip.length;
                break;
            case 10:
                PlayAudioRepeated(Sound_Q11);
                QVOLength = Sound_Q11[VOLanguage].clip.length;
                break;
            case 11:
                PlayAudioRepeated(Sound_Q12);
                QVOLength = Sound_Q12[VOLanguage].clip.length;
                break;
        }
        return ClipLength;
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
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
                ClipLength = Appriciation_Good[VOLanguage].clip.length+_delay;
                break;
            case 1:
                PlayAudio(Appriciation_Great, _delay);
                ClipLength = Appriciation_Great[VOLanguage].clip.length + _delay;
                break;
            case 2:
                PlayAudio(Appriciation_Excellent, _delay);
                ClipLength = Appriciation_Excellent[VOLanguage].clip.length + _delay;
                break;
            case 3:
                PlayAudio(Appriciation_Nice, _delay);
                ClipLength = Appriciation_Nice[VOLanguage].clip.length + _delay;
                break;
            case 4:
                PlayAudio(Appriciation_Splended, _delay);
                ClipLength = Appriciation_Good[VOLanguage].clip.length + _delay;
                break;
            case 5:
                PlayAudio(Appriciation_Weldone, _delay);
                ClipLength = Appriciation_Weldone[VOLanguage].clip.length + _delay;
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
            if (i==(_UserInput-1))
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        SetTime(_UserInput);

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

            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion-1, LengthDelay+0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);

            //StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 0.75f));
            
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);

            Invoke("HighlightDigitalText", 1);

            if (QuestionOrder1 < (QuestionsOrder1.Length) ||
            WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
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
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion-1, Sound_IncorrectAnswer.clip.length);

                Invoke("HighlightCorrectOption", Sound_IncorrectAnswer.clip.length);

                TargetList = WrongAnsweredQuestions1;
                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionsOrder1.Length))
                {
                    if (WrongAnsweredQuestionOrder1 <= 0)
                    {
                        WrongAnsweredQuestions1.Add(CurrentQuestion);
                    }
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay+1f);
                }
                else
                {
                    Debug.Log("Game Over W");
                    StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2f));
                    // Invoke("ShowLC", LengthDelay + 2.5f);
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

    void HighlightCorrectOption()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (i == (CorrectAnsrIndex-1))
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        SetTime(CurrentQuestion);

        Invoke("HighlightDigitalText", 1);
    }

    void HighlightDigitalText()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
        }

        Txt_DigitalTime.GetComponent<PopTweenCustom>().StartAnim(); 
    }

    int[] TargetArray;
    int FindIndexofElementinArray(int _TargetElement)
    {
        int _returnElement=0;
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

    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
        }

    }

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
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

    public AudioSource[] Sound_Q1;
    public AudioSource[] Sound_Q2;
    public AudioSource[] Sound_Q3;
    public AudioSource[] Sound_Q4;
    public AudioSource[] Sound_Q5;
    public AudioSource[] Sound_Q6;
    public AudioSource[] Sound_Q7;
    public AudioSource[] Sound_Q8;
    public AudioSource[] Sound_Q9;
    public AudioSource[] Sound_Q10;
    public AudioSource[] Sound_Q11;
    public AudioSource[] Sound_Q12;

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
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedCall");
        }
    }

    public void StopRepetedAudio()
    {
        if(_audiotorepeat!=null)
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
