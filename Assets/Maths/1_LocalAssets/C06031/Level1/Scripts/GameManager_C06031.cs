using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_C06031 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject Tut_Items2;
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

    public GameObject SidePanel;
    public GameObject[] AnswerObjs;

    public Image Ans_Img;
    public Sprite[] Ans_Sprites;

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
            AddValueInProgress = 1 / (float)QuestionsOrder1.Length;
            Thisgamekey = gameInputData.Key;
        }

       

        TutorialObj.gameObject.SetActive(true);

        PlayAudio(Sound_Intro1, 0.5f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length +0.8f);
    
        float _delay = 0;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }
        
        iTween.MoveTo(Tut_Sidepanel.gameObject, iTween.Hash("x",512, "time", 1f, "islocal", true, "delay", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + 1, "easetype", iTween.EaseType.easeInOutQuad));
        Invoke("EnableAnimator",0.5f);
        Invoke("CallIntro3", Sound_Intro1[VOLanguage].clip.length+ Sound_Intro2[VOLanguage].clip.length+0.8f);
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
        Tut_Sidepanel.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void CallIntro2()
    {
        StopAudio(Sound_Intro1);
        PlayAudio(Sound_Intro2, 0.5f);
        Tut_Items.transform.gameObject.SetActive(false);
        Tut_Items2.transform.gameObject.SetActive(true);
    }

    public void CallIntro3()
    {
       
        StopAudio(Sound_Intro2);
        PlayAudioRepeated(Sound_Intro3);
        Invoke("EnableTutNoTRaycatTarget", Sound_Intro3[VOLanguage].clip.length + 0.1f);
    }

    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);

        Tut_Sidepanel.transform.GetChild(1).GetComponent<Text>().raycastTarget = false;
        Tut_Sidepanel.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();

        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro4);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro4);
        PlayAudio(Sound_BtnOkClick, 0);

        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);

        float LengthDelay = PlayAnswerVoiceOver(0,0);
        PlayAppreciationVoiceOver(LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + 1f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 1f);
        Tut_Sidepanel.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 1f);

        PlayAudio(Sound_Intro5, LengthDelay + 2f);

        Invoke("SetGamePlay", LengthDelay + Sound_Intro4[VOLanguage].clip.length + 2.5f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);

        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length;
        }

        if(Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        List<int> QuestionOrderList = new List<int>();
        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {

            QuestionOrderList.Add(QuestionsOrder1[i]);
            //------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i];
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

        for (int i = 2; i < 8; i++)
        {
            QuestionOrderListtemp.Add(i);
        }

        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {       
            
            tempq = QuestionOrder1;
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
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            TargetArray = QuestionsOrder1;
            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }

        if (CurrentQuestion < 11)
        {
            QuestionOrderListtemp.Remove(2);
        }
        else
        if (CurrentQuestion >= 11 && CurrentQuestion <= 21)
        {
            QuestionOrderListtemp.Remove(4);
        }
        else
        if (CurrentQuestion > 21)
        {
            QuestionOrderListtemp.Remove(4);
        }

        float _delay1 = 0;
        RandAnsrIndex = Random.Range(0, 3);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                if (CurrentQuestion < 11)
                {
                    AnswerObjs[i].GetComponent<Text>().text = "" + 2;
                    AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 1;
                }
                else
                if (CurrentQuestion >=11 && CurrentQuestion <=21)
                {
                    AnswerObjs[i].GetComponent<Text>().text = "" + 4;
                    AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 1;
                }
                else
                if (CurrentQuestion > 21)
                {
                    AnswerObjs[i].GetComponent<Text>().text = "" + 4;
                    AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 3;
                }
               
                Ans_Img.sprite = Ans_Sprites[CurrentQuestion];
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log("" + _ixx);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _ixx;
                    AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 1;
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log(""+ _iyy);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _iyy;
                    AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 1;
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log("" + _izz);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _izz;
                    AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 1;
                    QuestionOrderListtemp.Remove(_izz);
                }
            }
           
            iTween.ScaleTo(AnswerObjs[i].transform.parent.gameObject, iTween.Hash("Scale", Vector3.one * 0.5f, "time", 1f, "delay", _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting,0));
            
        }
        Is_OkButtonPressed = false;

        CorrectAnsrIndex = RandAnsrIndex;
        float LengthDelay=PlayQuestionVoiceOver(CurrentQuestion);
        iTween.MoveFrom(SidePanel.gameObject, iTween.Hash("x", 2000, "time", 0.5f,"islocal",true ,"delay", LengthDelay, "easetype", iTween.EaseType.easeInOutQuad));

        Invoke("EnableOptionsRaycast", QVOLength);
        Debug.Log("CorrectAnsrIndex : " + CurrentQuestion);
    }
    
    void EnableOptionsRaycast()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
        }
    }
    float PlayQuestionVoiceOver(int _Qi)
    {
        
        
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
            case 12:
                PlayAudioRepeated(Sound_Q13);
                QVOLength = Sound_Q13[VOLanguage].clip.length;
                break;
            case 13:
                PlayAudioRepeated(Sound_Q14);
                QVOLength = Sound_Q14[VOLanguage].clip.length;
                break;
            case 14:
                PlayAudioRepeated(Sound_Q15);
                QVOLength = Sound_Q15[VOLanguage].clip.length;
                break;
            case 15:
                PlayAudioRepeated(Sound_Q16);
                QVOLength = Sound_Q16[VOLanguage].clip.length;
                break;
            case 16:
                PlayAudioRepeated(Sound_Q17);
                QVOLength = Sound_Q17[VOLanguage].clip.length;
                break;
            case 17:
                PlayAudioRepeated(Sound_Q18);
                QVOLength = Sound_Q18[VOLanguage].clip.length;
                break;
            case 18:
                PlayAudioRepeated(Sound_Q19);
                QVOLength = Sound_Q19[VOLanguage].clip.length;
                break;
            case 19:
                PlayAudioRepeated(Sound_Q20);
                QVOLength = Sound_Q20[VOLanguage].clip.length;
                break;
            case 20:
                PlayAudioRepeated(Sound_Q21);
                QVOLength = Sound_Q21[VOLanguage].clip.length;
                break;
            case 21:
                PlayAudioRepeated(Sound_Q22);
                QVOLength = Sound_Q22[VOLanguage].clip.length;
                break;
            case 22:
                PlayAudioRepeated(Sound_Q23);
                QVOLength = Sound_Q23[VOLanguage].clip.length;
                break;
            case 23:
                PlayAudioRepeated(Sound_Q24);
                QVOLength = Sound_Q24[VOLanguage].clip.length;
                break;
            case 24:
                PlayAudioRepeated(Sound_Q25);
                QVOLength = Sound_Q25[VOLanguage].clip.length;
                break;
            case 25:
                PlayAudioRepeated(Sound_Q26);
                QVOLength = Sound_Q26[VOLanguage].clip.length;
                break;
            case 26:
                PlayAudioRepeated(Sound_Q27);
                QVOLength = Sound_Q27[VOLanguage].clip.length;
                break;

        }
        return QVOLength;
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
            case 12:
                PlayAudio(Sound_A13, _delay);
                ClipLength = Sound_A13[VOLanguage].clip.length;
                break;
            case 13:
                PlayAudio(Sound_A14, _delay);
                ClipLength = Sound_A14[VOLanguage].clip.length;
                break;
            case 14:
                PlayAudio(Sound_A15, _delay);
                ClipLength = Sound_A15[VOLanguage].clip.length;
                break;
            case 15:
                PlayAudio(Sound_A16, _delay);
                ClipLength = Sound_A16[VOLanguage].clip.length;
                break;
            case 16:
                PlayAudio(Sound_A17, _delay);
                ClipLength = Sound_A17[VOLanguage].clip.length;
                break;
            case 17:
                PlayAudio(Sound_A18, _delay);
                ClipLength = Sound_A18[VOLanguage].clip.length;
                break;
            case 18:
                PlayAudio(Sound_A19, _delay);
                ClipLength = Sound_A19[VOLanguage].clip.length;
                break;
            case 19:
                PlayAudio(Sound_A20, _delay);
                ClipLength = Sound_A20[VOLanguage].clip.length;
                break;
            case 20:
                PlayAudio(Sound_A21, _delay);
                ClipLength = Sound_A21[VOLanguage].clip.length;
                break;
            case 21:
                PlayAudio(Sound_A22, _delay);
                ClipLength = Sound_A22[VOLanguage].clip.length;
                break;
            case 22:
                PlayAudio(Sound_A23, _delay);
                ClipLength = Sound_A23[VOLanguage].clip.length;
                break;
            case 23:
                PlayAudio(Sound_A24, _delay);
                ClipLength = Sound_A24[VOLanguage].clip.length;
                break;
            case 24:
                PlayAudio(Sound_A25, _delay);
                ClipLength = Sound_A25[VOLanguage].clip.length;
                break;
            case 25:
                PlayAudio(Sound_A26, _delay);
                ClipLength = Sound_A26[VOLanguage].clip.length;
                break;
            case 26:
                PlayAudio(Sound_A27, _delay);
                ClipLength = Sound_A27[VOLanguage].clip.length;
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
            if (i==_UserInput)
            {
                AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        PlayAudio(Sound_Selection, 0);
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

            float LengthDelay = PlayAppreciationVoiceOver(0);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);

            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);
           
            if (QuestionOrder1 < (QuestionsOrder1.Length) ||
            WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                //Invoke("ShowLC", LengthDelay + LengthDelay2 + 3);
                Debug.Log("Game Over C");
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

            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                if (AnswerObjs[i].name.Contains(UserAnsr.ToString()))
                {
                    iTween.ShakePosition(AnswerObjs[i].transform.parent.gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                }
            }            

            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 0);
                
                Invoke("HighlightOptions", 1);
                //iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 5000, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.easeInOutQuad));
                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    if (AnswerObjs[i] != null)
                    {
                        AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StopAnim();
                    }
                }
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
                    ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay+1f);
                }
                else
                {
                    Debug.Log("Game Over W");
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
                    AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StopAnim();
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }
    void HighlightOptions()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StopAnim();
            }
        }
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

    #endregion

    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
        }

    }
   

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
    public AudioSource[] Sound_Q13;
    public AudioSource[] Sound_Q14;
    public AudioSource[] Sound_Q15;
    public AudioSource[] Sound_Q16;
    public AudioSource[] Sound_Q17;
    public AudioSource[] Sound_Q18;
    public AudioSource[] Sound_Q19;
    public AudioSource[] Sound_Q20;
    public AudioSource[] Sound_Q21;
    public AudioSource[] Sound_Q22;
    public AudioSource[] Sound_Q23;
    public AudioSource[] Sound_Q24;
    public AudioSource[] Sound_Q25;
    public AudioSource[] Sound_Q26;
    public AudioSource[] Sound_Q27;


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
    public AudioSource[] Sound_A25;
    public AudioSource[] Sound_A26;
    public AudioSource[] Sound_A27;


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
        _audiotorepeat[VOLanguage].PlayDelayed(0);
        yield return new WaitForSeconds(7 + QVOLength);
        StartCoroutine("PlayAudioRepeatedCall");
                  
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
