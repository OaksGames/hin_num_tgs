using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_C06051 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_QtnText1;
    public GameObject Tut_QtnText2;
    public GameObject Tut_Items1;
    public GameObject Tut_Items2;
    public GameObject Tut_Options;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

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

    public GameObject SidePanel;
    public GameObject[] AnswerObjs;
    public GameObject[] QuestionObjs;
    public GameObject Questionobj;

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
            AddValueInProgress = 1 / (float)NoOfQuestionsToAsk;
            Thisgamekey = gameInputData.Key;
        }

        

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);

        PlayAudio(Sound_Intro1, 0.5f);

        float _delay = 0;
        
        iTween.ScaleFrom(Tut_Items1.gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0));
        
        Invoke("EnableAnimator", 1.5f);
        Invoke("Tut_ImgOn", Sound_Intro1[VOLanguage].clip.length - 5f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 1f);
    }

    

    public void Tut_ImgOn()
    {
       Tut_Items1.gameObject.SetActive(false);
       Tut_Items2.gameObject.SetActive(true);
       Tut_QtnText1.gameObject.SetActive(false);
       Tut_QtnText2.gameObject.SetActive(true);
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
        Tut_Options.transform.GetChild(0).gameObject.GetComponent<Text>().raycastTarget = true;
    }

    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro2);
        Invoke("EnableTutNoTRaycatTarget", Sound_Intro2[VOLanguage].clip.length + 0.1f);
    }

    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);

        Tut_Options.transform.GetChild(0).GetComponent<Text>().raycastTarget = false;
        Tut_Options.transform.GetChild(0).GetComponent<PopTweenCustom>().StartAnim();

        StopAudio(Sound_Intro2);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro3);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro3);
        PlayAudio(Sound_BtnOkClick, 0);

        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);

        float LengthDelay = PlayAnswerVoiceOver(8, 0);
        PlayAppreciationVoiceOver(LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + 1f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 1f);
        Tut_Options.transform.GetChild(0).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 1f);

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
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;
        }


        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();
        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
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
    int _randq;
    public void GenerateQuestion()
    {
        _randq = Random.Range(0, 2);
        CorrectAnsrIndex = _randq;
        switch (_randq)
        {
            case 0: // False
                if (CurrentQuestion < 2)
                {
                    int _randwrongasnr = Random.Range(0, 4);
                    if (_randwrongasnr == 0)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 1)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 2;
                    }
                    else if (_randwrongasnr == 2)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 3)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 3;
                    }
                }
                else                
                if (CurrentQuestion >= 2 && CurrentQuestion<=3)
                {
                    int _randwrongasnr = Random.Range(0, 4);
                    if (_randwrongasnr == 0)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 2;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 1)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 2;
                    }
                    else if (_randwrongasnr == 2)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 3)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 3;
                    }
                }                
                else
                if (CurrentQuestion >=4 && CurrentQuestion <= 5)
                {
                    int _randwrongasnr = Random.Range(0, 4);
                    if (_randwrongasnr == 0)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 2;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 1)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 2)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 3)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 3;
                    }
                }
                else
                if (CurrentQuestion >= 6 && CurrentQuestion <= 7)
                {
                    int _randwrongasnr = Random.Range(0, 4);
                    if (_randwrongasnr == 0)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 2;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 1)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 2)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 2;
                    }
                    else if (_randwrongasnr == 3)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 3;
                    }
                }
                else
                if (CurrentQuestion >= 8 && CurrentQuestion <10)
                {
                    int _randwrongasnr = Random.Range(0, 4);
                    if (_randwrongasnr == 0)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 2;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 1)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                    else if (_randwrongasnr == 2)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 2;
                    }
                    else if (_randwrongasnr == 3)
                    {
                        QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                        QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    }
                }
                break;
            case 1://true
                if (CurrentQuestion < 2)
                {
                    QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 2;
                    QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                    
                }
                else
                if (CurrentQuestion >= 2 && CurrentQuestion <= 3)
                {
                    QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                    QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                }
                else
                if (CurrentQuestion >= 4 && CurrentQuestion <= 5)
                {
                    QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
                    QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 2;
                }
                else
                if (CurrentQuestion >= 6 && CurrentQuestion <= 7)
                {
                    QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                    QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
                }
                else
                if (CurrentQuestion >= 8 && CurrentQuestion <= 9)
                {
                    QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
                    QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 3;
                }
                break;
        }
      
    }

    int RandAnsrIndex = 0;
    public int _iCorrectAns;
    public void GenerateLevel()
    {        
        int tempq = 0;
        
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                QuestionObjs[0].transform.gameObject.GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        if (QuestionOrder1 < (QuestionOrderList.Count))
        {            
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentQuestionOrder = QuestionOrder1;
            Ans_Img.sprite = Ans_Sprites[CurrentQuestion];
            GenerateQuestion();
            Questionobj.gameObject.SetActive(true);
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;            
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            Ans_Img.sprite = Ans_Sprites[CurrentQuestion];
            TargetArray = QuestionsOrder1;
            GenerateQuestion();
            Questionobj.gameObject.SetActive(true);
          
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }

        float _delay1 = 0;
        
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (i == 0)
            {
                AnswerObjs[i].GetComponent<Text>().text = "False";
                AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 1;
            }
            else
            if (i == 1)
            {
                AnswerObjs[i].GetComponent<Text>().text = "True";
                AnswerObjs[i].transform.parent.GetComponent<Text>().text = "" + 1;
            }            

            iTween.ScaleTo(AnswerObjs[i].transform.parent.gameObject, iTween.Hash("Scale", Vector3.one * 0.5f, "time", 1f, "delay", _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting,0));
           
        }
        if (CurrentQuestion ==0|| CurrentQuestion == 2|| CurrentQuestion ==4 || CurrentQuestion == 6 || CurrentQuestion == 8)
        {
            PlayQuestionVoiceOver(0);
        }
        else
        if (CurrentQuestion == 1 || CurrentQuestion == 3 || CurrentQuestion == 5 )
        {
            PlayQuestionVoiceOver(1);
        }
        else
        if (CurrentQuestion == 7 || CurrentQuestion == 9 )
        {
            PlayQuestionVoiceOver(2);
        }
        Is_OkButtonPressed = false;
        Invoke("EnableOptionsRaycast", QVOLength);
        Debug.Log("CorrectAnsrIndex : " + CorrectAnsrIndex);
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
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO[_Qi].EN_Sound_QO[0].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].EN_Sound_QO[0]);
                break;
            case 1:
                QVOLength = Sound_QVO[_Qi].HI_Sound_QO[0].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].HI_Sound_QO[0]);
                break;
            case 2:
                QVOLength = Sound_QVO[_Qi].TL_Sound_QO[0].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].HI_Sound_QO[0]);
                break;
        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO.EN_Sound_AO[_Ai], _delay);
        ClipLength = Sound_AVO.EN_Sound_AO[_Ai].clip.length;

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
        StartCoroutine("PlayAudioRepeatedSingleCall");
    }

    public void CorrectAnswer()
    {
        if (CurrentQuestion < 2)
        {
            QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 2;
            QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;

        }
        else
        if (CurrentQuestion >= 2 && CurrentQuestion <= 3)
        {
            QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
            QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
        }
        else
        if (CurrentQuestion >= 4 && CurrentQuestion <= 5)
        {
            QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 3;
            QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 2;
        }
        else
        if (CurrentQuestion >= 6 && CurrentQuestion <= 7)
        {
            QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
            QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 1;
        }
        else
        if (CurrentQuestion >= 8 && CurrentQuestion <= 9)
        {
            QuestionObjs[0].transform.GetChild(2).GetComponent<Text>().text = "" + 4;
            QuestionObjs[0].transform.GetChild(0).GetComponent<Text>().text = "" + 3;
        }
        Invoke("HighlightOptions", 1);
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

            CorrectAnswer();
            Invoke("HighlightOptions", 1);

            float LengthDelay = PlayAppreciationVoiceOver(0);

            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);

            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);

            if (QuestionOrder1 < ((QuestionOrderList.Count)) ||
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
                    iTween.ShakePosition(AnswerObjs[UserAnsr].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                }
            }            

            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                 CorrectAnswer();
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 0);
                Invoke("HighlightOptions", 1);
                TargetList = WrongAnsweredQuestions1;
                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList.Count))
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

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay+1f);
                }
                else
                {

                    StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2f));
                    //Invoke("ShowLC", LengthDelay + 2.5f);
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
                    QuestionObjs[0].transform.gameObject.GetComponent<PopTweenCustom>().StopAnim();
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
                AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StopAnim();
                QuestionObjs[0].transform.gameObject.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].transform.parent.gameObject.GetComponent<PopTweenCustom>().StopAnim();
                QuestionObjs[0].transform.gameObject.GetComponent<PopTweenCustom>().StopAnim();
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

    public QVO_AudioSource_C06051[] Sound_QVO;
    public AVO_AudioSource_C06051 Sound_AVO;

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

    public void StopAudio(AudioSource _audio)
    {
        _audio.Stop();
    }

    public void PlayAudioRepeated(AudioSource _audio)
    {
        _audiotorepeat = _audio;
        StartCoroutine("PlayAudioRepeatedSingleCall");
    }
    AudioSource _audiotorepeat;
    float QVOLength;
    IEnumerator PlayAudioRepeatedSingleCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            _audiotorepeat.PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedSingleCall");
        }
    }

    public void PlayAudioRepeated(AudioSource[] _audio)
    {
        _audiotorepeatarray = _audio;
        StartCoroutine("PlayAudioRepeatedCall");
    }
    AudioSource[] _audiotorepeatarray;
    IEnumerator PlayAudioRepeatedCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            _audiotorepeatarray[VOLanguage].PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedCall");
        }
    }

    public void StopRepetedAudio()
    {
        if (_audiotorepeatarray != null)
        { StopAudio(_audiotorepeatarray); }

        if (_audiotorepeat != null)
        { StopAudio(_audiotorepeat); }

        StopCoroutine("PlayAudioRepeatedCall");
        StopCoroutine("PlayAudioRepeatedSingleCall");
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

    [System.Serializable]
    public class AVO_AudioSource_C06051
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_C06051
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }
}
