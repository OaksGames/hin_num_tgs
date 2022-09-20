using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B02011 : MonoBehaviour, IOAKSGame
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
    public GameObject TutNumber;
    public GameObject Tut_DownBar;
    public GameObject SelectedTut_Option;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk1;
    public int NoOfQuestionsToAsk2;

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
    public List<int> QuestionOrderList1;

    [HideInInspector]
    public List<int> QuestionOrderList2;

    public int[] QuestionsOrder1;
    public int QuestionOrder1=0;
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public int[] QuestionsOrder2;
    public int QuestionOrder2=0;
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
    public GameObject SelectedOptHolder;
    public GameObject[] RackItem;

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
        PlayAudio(Sound_Intro1, 2.5f);
        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length + 2.4f);
        PlayAudio(Sound_Intro3, Sound_Intro1[VOLanguage].clip.length+ Sound_Intro2[VOLanguage].clip.length+2.5f);

        float _delay = 0.25f;
        for (int i = 0; i < Tut_Items.Length; i++)
        {
            iTween.ScaleFrom(Tut_Items[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay+0.25f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+0.25f));
            _delay += 0.1f;
        }

        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length 
            + Sound_Intro2[VOLanguage].clip.length +Sound_Intro3[VOLanguage].clip.length + 3.5f);
        Invoke("EnableAnimator", 2);
        Invoke("EnableTutNoTRaycatTarget", 13.5f);
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

    public void EnableTutNoTRaycatTarget()
    {
        TutNumber.GetComponent<Text>().raycastTarget = true;
    }

    public void CallIntro2()
    {       
        PlayAudioRepeated(Sound_Intro4);
        iTween.MoveTo(Tut_DownBar.gameObject, iTween.Hash("y", 0, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
    }

    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_AnswerObjs[2].gameObject.GetComponent<Text>().raycastTarget = false;
        Tut_AnswerObjs[2].gameObject.GetComponent<PopTweenCustom>().StartAnim();
        iTween.MoveTo(SelectedTut_Option.gameObject, iTween.Hash("x", 411, "time", 0.3f, "delay", CurrentQuestion*0.1f,"islocal",true, "easetype", iTween.EaseType.linear));
        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro5);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro5);
        StopRepetedAudio();
        PlayAudio(Sound_Btn_Ok_Click, 0);
        TutHand2.gameObject.SetActive(false);

        CurrentItem = 0;                
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        PlayAudio(Sound_Intro6, LengthDelay+0.25f);
        float LengthDelay2 = Sound_Intro6[VOLanguage].clip.length+0.25f;
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);
        Tut_AnswerObjs[1].gameObject.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay+ LengthDelay2 + 1f);
        iTween.MoveTo(Tut_DownBar.gameObject, iTween.Hash("y", -400, "time", 1f, "delay", LengthDelay+ LengthDelay2, "easetype", iTween.EaseType.easeOutElastic));
        PlayAudio(Sound_Intro7, LengthDelay + LengthDelay2 + 2f);
        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro5[VOLanguage].clip.length + 3f);
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
            if(CurrentItems[i]!=null)
            Destroy(CurrentItems[i].gameObject);
        }

        QuestionOrderListtemp = new List<int>();
        TutorialObj.gameObject.SetActive(false);

        if (QuestionOrder1 < (QuestionOrderList1.Count))
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
        if (QuestionOrder2 < (QuestionOrderList2.Count))
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
                AnswerObjs[i].GetComponent<Text>().text = ""+CurrentQuestion*10;
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _ixx*10;
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _iyy*10;
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _izz*10;
                    QuestionOrderListtemp.Remove(_izz);
                }
                else
                if(i==3)
                {
                    int _iqq = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _iqq * 10;
                    QuestionOrderListtemp.Remove(_iqq);
                }
            }

            iTween.ScaleTo(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.one*0.5f, "time", 1f, "delay", _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay1));
            _delay1 += 0.1f;
        }       

        CurrentItems = new GameObject[CurrentQuestion];
        LevelsHolder.gameObject.SetActive(true);

        for (int i = 0; i < CurrentItems.Length; i++)
        { 
            if (i<5)
            {
                QuestionsHolder[0].gameObject.SetActive(true);
                RackItem[0].gameObject.SetActive(true);
                RackItem[1].gameObject.SetActive(false);
                CurrentItems[i] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[0].transform);   
            }
            if (i >=5)
            {
                QuestionsHolder[1].gameObject.SetActive(true);
                RackItem[1].gameObject.SetActive(true);
                CurrentItems[i] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[1].transform);
            }          
        }

        float _delay = 0;
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            iTween.ScaleTo(CurrentItems[i].gameObject, iTween.Hash("scale", Vector3.one, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            CurrentItems[i].GetComponent<Image>().raycastTarget = true;
            _delay += 0.1f;
        }

        Is_OkButtonPressed = false;
        Invoke("PlayQuestionVoiceOverWithDelay", CurrentItems.Length * 0.1f);
    }

    void PlayQuestionVoiceOverWithDelay()
    {
        PlayQuestionVoiceOver(Random.RandomRange(0, 2));
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void EnableOptionsRaycast()
    {
        iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("y", 0, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        if (VOLanguage == 0)
        {
            QVOLength = Sound_QVO[CurrentItem].EN_Sound_QVO[_Qi].clip.length;
            PlayAudioRepeated(Sound_QVO[CurrentItem].EN_Sound_QVO[_Qi]);
        }
        if (VOLanguage == 1)
        {
            QVOLength = Sound_QVO[CurrentItem].HI_Sound_QVO[_Qi].clip.length;
            PlayAudioRepeated(Sound_QVO[CurrentItem].HI_Sound_QVO[_Qi]);
        }
        if (VOLanguage == 2)
        {
            QVOLength = Sound_QVO[CurrentItem].TL_Sound_QVO[_Qi].clip.length;
            PlayAudioRepeated(Sound_QVO[CurrentItem].TL_Sound_QVO[_Qi]);
        }
        
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        if (VOLanguage == 0)
        {
            ClipLength = Sound_AVO[CurrentItem].EN_Sound_AVO[_Ai].clip.length;
            PlayAudio(Sound_AVO[CurrentItem].EN_Sound_AVO[_Ai], _delay);
            Debug.Log("Sound : " + CurrentItem);
        }
        if (VOLanguage == 1)
        {
            ClipLength = Sound_AVO[CurrentItem].HI_Sound_AVO[_Ai].clip.length;
            PlayAudio(Sound_AVO[CurrentItem].HI_Sound_AVO[_Ai], _delay);
        }
        if (VOLanguage == 2)
        {
            ClipLength = Sound_AVO[CurrentItem].TL_Sound_AVO[_Ai].clip.length;
            PlayAudio(Sound_AVO[CurrentItem].TL_Sound_AVO[_Ai], _delay);
        }
       
        return ClipLength;
    }

    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);   
    }

    int UserAnsr;
    public GameObject AnswerDisplay;
    public void Check_Answer(int _Ansrindex)
    {
        iTween.MoveTo(SelectedOptHolder.gameObject, iTween.Hash("x", 411, "time", 0.3f, "delay", 0, "islocal", true, "easetype", iTween.EaseType.linear));
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

        AnswerDisplay.GetComponent<Text>().text = "" + AnswerObjs[UserAnsr].GetComponent<Text>().text;
        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption", 7);
    }

    void RepeatQVOAftertChoosingOption()
    {
        StartCoroutine("PlayAudioRepeatedSingleCall");
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

    bool Is_OkButtonPressed=false;
    public void BtnAct_Ok()
    {
        if (Is_CanClick)
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

            float LengthDelay = PlayAppreciationVoiceOver(Sound_Btn_Ok_Click.clip.length+0.25f) + Sound_Btn_Ok_Click.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion-1, LengthDelay + 0.25f);
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);

            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2f));

            iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("y", -400, "time", 0.25f, "delay", LengthDelay + LengthDelay2, "easetype", iTween.EaseType.easeOutElastic));
            iTween.MoveTo(SelectedOptHolder.gameObject, iTween.Hash("x", 617, "time", 0.25f, "delay", LengthDelay + LengthDelay2 + 2f, "islocal", true, "easetype", iTween.EaseType.linear));
            
            if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                QuestionOrder2 < (QuestionOrderList2.Count) ||
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

            iTween.MoveTo(SelectedOptHolder.gameObject, iTween.Hash("x", 617, "time", 0.3f, "delay", 0, "islocal", true, "easetype", iTween.EaseType.linear));
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
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion-1, 1f);
                iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("y", -400, "time", 0.25f, "delay", LengthDelay+1f, "easetype", iTween.EaseType.easeOutElastic));
                Invoke("HighlightOptions", 0.5f);

                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    if (AnswerObjs[i] != null)
                    {
                        AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                    }
                }

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList1.Count)
                    && CurrentItem == 0)
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionOrderList2.Count) && CurrentItem==1)
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }               
                else
                {
                    ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                    QuestionOrder2 < (QuestionOrderList2.Count) ||
                    WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
                   
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
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
                iTween.MoveTo(SelectedOptHolder.gameObject, iTween.Hash("x", 617, "time", 0.3f, "delay", 0.2f, "islocal", true, "easetype", iTween.EaseType.linear));//CS
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


    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }
    #endregion
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
    public AudioSource[] Sound_Intro6;
    public AudioSource[] Sound_Intro7;

    public QO_AudioSource_B02011[] Sound_QVO;
    public AO_AudioSource_B02011[] Sound_AVO;

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
    float QVOLength2;
    IEnumerator PlayAudioRepeatedSingleCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            _audiotorepeat.PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength + QVOLength2);
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

[System.Serializable]
public class AO_AudioSource_B02011
{
    public AudioSource[] EN_Sound_AVO;
    public AudioSource[] HI_Sound_AVO;
    public AudioSource[] TL_Sound_AVO;
}

[System.Serializable]
public class QO_AudioSource_B02011
{
    public AudioSource[] EN_Sound_QVO;
    public AudioSource[] HI_Sound_QVO;
    public AudioSource[] TL_Sound_QVO;
}
