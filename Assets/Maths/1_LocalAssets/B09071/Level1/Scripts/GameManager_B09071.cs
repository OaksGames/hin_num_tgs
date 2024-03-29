﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B09071 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    //public GameObject Tut_Shape;
    public GameObject Tut_Items;
    public GameObject Month_Image;
    //public GameObject Tut_Sidepanel;
    public GameObject TutHand1, TutHand2;

    public AudioClip[] Tut_AudioClips_EN;
    public AudioClip[] Tut_AudioClips_TE;
    public AudioClip[] Tut_AudioClips_HI;

    public AudioSource Tut_audioSource;
    public Text[] Tut_Txt_options;
    public GameObject Tut_Ans;
    public GameObject TutorialObj2;
    public float tutActiveTime;

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

    public GameObject SidePanel;
    public GameObject[] AnswerObjs;

    public Image Ans_Img;
    public Sprite[] Ans_Sprites;
    public string[] Ans_Items;

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
            AddValueInProgress = 1 / (float)(QuestionsOrder1.Length);
            Thisgamekey = gameInputData.Key;
        }

        //SetQues(TotalQues, Thisgamekey);

        TutorialObj.gameObject.SetActive(true);

        float _delay = 0;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        PlayAudio(Sound_Intro1, 2f);
        //PlayAudio(Sound_Intro2, 
        //    Sound_Intro1[VOLanguage].clip.length+2);
        //PlayAudio(Sound_Intro3, 
        //    Sound_Intro1[VOLanguage].clip.length+Sound_Intro2[VOLanguage].clip.length + 2);

        Invoke("EnableAnimator", 5);
        StartCoroutine(_Play_Tut_Audio());
        Invoke("CallIntro2", 50f);
    }

    

    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;
    }

    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }

    public IEnumerator _Play_Tut_Audio()
    {
        yield return new WaitForSeconds(Sound_Intro1[0].clip.length + 1.5f);
        for (int i = 0; i < Tut_AudioClips_EN.Length; i++)
        {
            if (VOLanguage == 0)
            {
                Tut_audioSource.PlayOneShot(Tut_AudioClips_EN[i]);
                Debug.Log("VO SELECTED : " + VOLanguage);
            }
            else
            if (VOLanguage == 1)
            {
                Tut_audioSource.PlayOneShot(Tut_AudioClips_TE[i]);
                Debug.Log("VO SELECTED : " + VOLanguage);
            }
            else
            if (VOLanguage == 2)
            {
                Tut_audioSource.PlayOneShot(Tut_AudioClips_HI[i]);
                Debug.Log("VO SELECTED : " + VOLanguage);
            }

            Month_Image.GetComponent<Image>().sprite = Ans_Sprites[i];
            //Tut_Txt_options[i].GetComponentInChildren<Text>().color = Color.black;
            iTween.ScaleTo(Tut_Txt_options[i].gameObject, iTween.Hash("x", 0.58f, "y", 0.58f, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));
            Tut_Txt_options[i].GetComponent<Text>().color = Color.black;
            //Tut_Items.transform.GetChild(i).GetComponent<PopTweenCustom>().StartAnim();
            while (Tut_audioSource.isPlaying)
                yield return null;

            for (int j = 0; j < Tut_Items.transform.childCount; j++)
            {
                iTween.ScaleTo(Tut_Txt_options[j].gameObject, iTween.Hash("x", 0.5f, "y", 0.5f, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.linear));
                Tut_Txt_options[j].GetComponent<Text>().color = Color.grey;
            }
        }
    }

    public void CallIntro2()
    {
        StartCoroutine(SetActiveWithDelayCall(TutorialObj2, true, tutActiveTime));
        PlayAudio(Sound_Intro14, 5f);
        PlayAudio(Sound_Intro15, Sound_Intro14[VOLanguage].clip.length + 5f);
        Invoke("CallIntro3", Sound_Intro14[VOLanguage].clip.length + Sound_Intro15[VOLanguage].clip.length + 5f);
    }

    public void CallIntro3()
    {
        PlayAudioRepeated(Sound_Intro16);
    }

    public void Selected_TutAnswer()
    {
        StopRepetedAudio();
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        //Tut_Items.transform.GetChild(6).GetComponent<Image>().raycastTarget = false;
        //Tut_Txt_options[7].GetComponentInChildren<Text>().color = Color.black;
        //iTween.ScaleTo(Tut_Txt_options[7].gameObject, iTween.Hash("x", 0.55f, "y", 0.55f, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));

        PlayAudioRepeated(Sound_Intro17);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        //StopAudio(Sound_Intro3);
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);

        //CurrentItem = 1;
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(5, LengthDelay+0.25f);
        ///iTween.ScaleTo(Tut_Txt_options[1].gameObject, iTween.Hash("x", 0.5f, "y", 0.5f, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.linear));
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);

        Tut_Ans.transform.GetComponent<PopTweenCustom>().Invoke("StartAnim", 0);

        ///PlayAudio(Sound_Intro12, LengthDelay + LengthDelay2 + 2f);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro17[VOLanguage].clip.length + 2f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);

        //LevelsHolder.gameObject.SetActive(true);
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length;
        }

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();

        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
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
                AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        QuestionOrderListtemp = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            // LIST IS FOR SELECTING OPTION NUMBERS
            QuestionOrderListtemp.Add(QuestionsOrder1[i]);
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
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            TargetArray = QuestionsOrder1;
            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];
         
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }

        
        RandAnsrIndex = Random.Range(0, 3);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[CurrentQuestion];
                Ans_Img.sprite = Ans_Sprites[CurrentQuestion];
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log("" + _ixx);
                    AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[_ixx];
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log(""+ _iyy);
                    AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[_iyy];
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log("" + _izz);
                    AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[_izz];
                    QuestionOrderListtemp.Remove(_izz);
                }
            }
        }

        //LevelsHolder.gameObject.SetActive(true);
        Is_OkButtonPressed = false;
        CorrectAnsrIndex = RandAnsrIndex;

        float LengthDelay=PlayQuestionVoiceOver(CurrentQuestion);
        iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 412, "time", 0.25f,"islocal",true ,"delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));
        float _delay1 = 0;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", LengthDelay+_delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, LengthDelay+_delay1));
            _delay1 += 0.1f;
        }
        Invoke("EnableOptionsRaycast", QVOLength);
        Debug.Log("CorrectAnsrIndex : " + CurrentQuestion);
    }
    void EnableOptionsRaycast()
    {
       // iTween.MoveTo(SidePanel.gameObject, iTween.Hash("y", 0, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
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
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
        }

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


            float LengthDelay = PlayAppreciationVoiceOver(0.25f);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay+0.25f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);

            //StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 0.75f));

            iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 612, "time", 1f, "delay", LengthDelay + LengthDelay2+0.25f, "islocal", true , "easetype", iTween.EaseType.easeOutElastic));

            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);

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

            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 1f);
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 612, "time", 0.25f, "delay", LengthDelay+0.25f, "islocal", true , "easetype", iTween.EaseType.easeOutElastic));
                Invoke("HighlightOptions", 0.5f);

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
                    Invoke("GenerateLevel", LengthDelay+2.5f);
                }
                else
                {
                    Debug.Log("Game Over W");
                    StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + 2f));
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

    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
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
    public AudioSource[] Sound_Intro14;
    public AudioSource[] Sound_Intro15;
    public AudioSource[] Sound_Intro16;
    public AudioSource[] Sound_Intro17;

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
