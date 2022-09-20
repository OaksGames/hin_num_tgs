using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B09051 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject TutHand1, TutHand2;

    public AudioClip[] Tut_AudioClips_EN;
    public AudioClip[] Tut_AudioClips_TE;
    public AudioClip[] Tut_AudioClips_HI;
    public AudioSource Tut_audioSource;

    public Text[] Tut_Txt_options;
    public GameObject Tut_Ans;
    public GameObject TutorialObj2;
    public GameObject Image_Days;
    public GameObject Day_Text;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk1;
    public int NoOfQuestionsToAsk2;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList1;

    [HideInInspector]
    public List<int> QuestionOrderList2;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;
    public int[] QuestionsOrder2;
    public int QuestionOrder2;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;
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
    public Image Ans_Img;
    public Sprite[] Ans_Sprites;
    public string[] Ans_Items;
    int CurrentItem;

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

        TutorialObj.gameObject.SetActive(true);       

        float _delay = 0;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        Invoke("EnableAnimator", 3);
        StartCoroutine(_Play_Tut_Audio());
        Invoke("CallIntro2", 30f);
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

    public IEnumerator _Play_Tut_Audio()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < Tut_AudioClips_EN.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            Day_Text.GetComponent<Text>().text = "" + Ans_Items[i];
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
            
            Image_Days.GetComponent<Image>().sprite = Ans_Sprites[i];
            iTween.ScaleTo(Tut_Txt_options[i].gameObject, iTween.Hash("x", 0.58f, "y", 0.58f, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong));
            Tut_Txt_options[i].GetComponent<Text>().color = Color.black;

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
        StartCoroutine(SetActiveWithDelayCall(TutorialObj2, true, 0.75f));
        PlayAudio(Sound_Intro8,0.5f);
        PlayAudio(Sound_Intro9, Sound_Intro8[VOLanguage].clip.length + 0.5f);
        Invoke("CallIntro3", Sound_Intro8[VOLanguage].clip.length + Sound_Intro9[VOLanguage].clip.length + 0.5f);
    }

    public void CallIntro3()
    {
        PlayAudioRepeated(Sound_Intro10);
    }

    public void Selected_TutAnswer()
    {
        StopRepetedAudio();
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
       // Tut_Ans.GetComponent<Text>().raycastTarget = false;
        PlayAudioRepeated(Sound_Intro11);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopRepetedAudio();
        PlayAudio(Sound_Btn_Ok_Click, 0);
        TutHand2.gameObject.SetActive(false);
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(2, LengthDelay);
        iTween.ScaleTo(Tut_Txt_options[2].gameObject, iTween.Hash("x", 0.5f, "y", 0.5f, "time", 0.25f, "islocal", true, "easetype", iTween.EaseType.linear));
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);
        Tut_Ans.transform.GetComponent<PopTweenCustom>().Invoke("StartAnim", 0);
        PlayAudio(Sound_Intro12, LengthDelay + LengthDelay2 + 2f);
        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro12[VOLanguage].clip.length + 2f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);
        QuestionsOrder2 = RandomArray_Int(QuestionsOrder2);
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk1 + NoOfQuestionsToAsk2;
        }

        if (Is_NeedRandomizedQuestions)
        {
            QuestionsOrder1 = RandomArray_Int(QuestionsOrder1);
            QuestionsOrder2 = RandomArray_Int(QuestionsOrder2);
        }

        QuestionOrderList1 = new List<int>();
        QuestionOrderList2 = new List<int>();

        for (int i = 0; i < NoOfQuestionsToAsk1; i++)
        {
            QuestionOrderList1.Add(QuestionsOrder1[i]);
        }
        for (int i = 0; i < NoOfQuestionsToAsk2; i++)
        {
            QuestionOrderList2.Add(QuestionsOrder2[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));
        GenerateLevel();
    }

    int CurrentChildQuestion = 0;
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
             
        if (QuestionOrder1 < (QuestionOrderList1.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }

            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentQuestionOrder = QuestionOrder1;
            CurrentItem = 0;
            CurrentChildQuestion = Random.Range(0, QuestionsOrder1.Length);

            Debug.Log("Question1 No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }
        else
        if (QuestionOrder2 < (QuestionOrderList2.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }

            tempq = QuestionOrder2;
            QuestionOrderListtemp.Remove(QuestionsOrder2[tempq]);
            CurrentQuestion = QuestionsOrder2[tempq];
            CurrentQuestionOrder = CurrentQuestion;
            CurrentItem = 1;
            CurrentChildQuestion = CurrentQuestion+1;
            Debug.Log("Question2 No : " + QuestionOrder2 + " A : " + QuestionsOrder2[QuestionOrder2]);
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
            TargetArray = QuestionsOrder1;
            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];
            CurrentItem = 0;
            CurrentChildQuestion = Random.Range(0, QuestionsOrder1.Length);
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
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
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            TargetArray = QuestionsOrder2;
            CurrentQuestionOrder = WrongAnsweredQuestions2[tempq];
            CurrentItem = 1;
            CurrentChildQuestion = CurrentQuestion + 1;
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            WrongAnsweredQuestionOrder2++;
        }

        float _delay1 = 0;
        RandAnsrIndex = Random.Range(0, 3);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[CurrentQuestion+ CurrentItem];
                Ans_Img.sprite = Ans_Sprites[CurrentQuestion+ CurrentItem];
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log("" + _ixx);
                    AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[_ixx+ CurrentItem];
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log("" + _iyy);
                    AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[_iyy+ CurrentItem];
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    Debug.Log("" + _izz);
                    AnswerObjs[i].GetComponent<Text>().text = "" + Ans_Items[_izz+ CurrentItem];
                    QuestionOrderListtemp.Remove(_izz);
                }
            }
        }

        CorrectAnsrIndex = RandAnsrIndex;
       
        if(CurrentItem== 1)
        {
            PlayQuestionVoiceOver(CurrentQuestionOrder+2);
        }
        if(CurrentItem== 0)
        {
            PlayQuestionVoiceOver(Random.Range(0,2));
        }           
       
        iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x",417, "time", 0.25f, "islocal", true, "delay", QVOLength, "easetype", iTween.EaseType.easeOutElastic));
        Debug.Log("CorrectAnsrIndex : " + CurrentQuestion);

        Is_OkButtonPressed = false;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", QVOLength + _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, QVOLength + _delay1));
            _delay1 += 0.1f;
        }

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

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO.EN_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.EN_Sound_QO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO.HI_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO.TL_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QO[_Qi]);
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
        StartCoroutine("PlayAudioRepeatedSingleCall");
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
            if (Testing)
            {
                ProgreesBar.GetComponent<Slider>().value += 1;
            }
            Total_CorrectAnswers++;//INGAME_COMMON
            WrongAnsrsCount = 0;

            float LengthDelay=PlayAppreciationVoiceOver(Sound_Btn_Ok_Click.clip.length + 0.25f) + Sound_Btn_Ok_Click.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver((CurrentQuestion)+(CurrentItem*7), LengthDelay+0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);
            iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 1500, "time", 0.25f, "delay", LengthDelay + LengthDelay2+ 0.75f,"islocal",true, "easetype", iTween.EaseType.easeOutElastic));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);

            if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                QuestionOrder2 < (QuestionOrderList2.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                Debug.Log("Game Over C");
                StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + LengthDelay2 + 2.5f));
                //Invoke("ShowLC", LengthDelay + LengthDelay2 + 3f);
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
                float LengthDelay = PlayAnswerVoiceOver((CurrentQuestion) + (CurrentItem * 7), Sound_IncorrectAnswer.clip.length);
                Invoke("HighlightCorrectOption", Sound_IncorrectAnswer.clip.length);
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 1500, "time", 0.25f, "delay", LengthDelay+ 0.75f, "islocal", true,"easetype", iTween.EaseType.easeOutElastic));
                TargetList = WrongAnsweredQuestions1;

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList1.Count) && CurrentItem == 0)
                {
                        WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionOrderList2.Count) && CurrentItem == 1)
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }
                else
                {
                    ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                    QuestionOrder2 < (QuestionOrderList2.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                    WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
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

    public AudioSource[] Sound_Intro8;
    public AudioSource[] Sound_Intro9;
    public AudioSource[] Sound_Intro10;
    public AudioSource[] Sound_Intro11;
    public AudioSource[] Sound_Intro12;

    public QVO_AudioSource_B09051 Sound_QVO;
    public AVO_AudioSource_B09051 Sound_AVO;

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
    public class AVO_AudioSource_B09051
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_B09051
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }
}
