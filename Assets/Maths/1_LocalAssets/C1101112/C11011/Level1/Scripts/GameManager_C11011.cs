using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_C11011 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsType1Ask;
    public int NoOfQuestionsType2Ask;

    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

    [HideInInspector]
    public bool Is_CanClick;

    public GameObject QuestionObj;
    public Sprite[] QuestionObj_Type1;
    public Sprite[] QuestionObj_Type2;
    public GameObject[] CurrentQuestionItems;
    public int[] ItemsOrder;

    [HideInInspector]
    public List<int> QuestionOrderList;
    public List<int> QuestionOrderList1;

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
    public List<int> OptionsOrderTempList1;

    [HideInInspector]
    public List<int> OptionsOrderTempList2;

    [HideInInspector]
    public int CorrectAnsrIndex;

    public int CurrentQuestion;
    int WrongAnsrsCount;
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

            TotalQues = 6;
            Thisgamekey = "nc08071";

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

    /* bool Init;
     private void OnEnable()
     {
         if (Init)
         {
             if (Is_Tutorial)
             {
                 SetTutorial();
             }
             else
             {
                 SetGamePlay();
             }
         }
     }

     private void OnDisable()
     {
         if (!Init)
         {
             MultiLevelManager.instance.LoadProgressMaxValues(NoOfQuestionsType1Ask + NoOfQuestionsType2Ask);
             Init = true;
         }
     }

     private void Update()
     {
         // TEST
         if (Input.GetKeyDown(KeyCode.S))
         {
             ShowLC();
         }
     }*/

    public void StartGame(GameInputData data)
    {
        ProgreesBar.SetActive(false);
        // btn_Back.SetActive(false);
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
            AddValueInProgress = 1 / (float)(NoOfQuestionsType1Ask + NoOfQuestionsType2Ask);
            Thisgamekey = gameInputData.Key;

        }

        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 3.5f);
        LevelsHolder.gameObject.SetActive(false);

        float _delay = 0.75f;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay+0.75f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+0.75f));
            _delay += 0.2f;
        }

        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length+ 3.8f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + 4f);
        Invoke("EnableAnimator", 3);
        Invoke("EnableTutNoTRaycatTarget", 11.5f);
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
        Tut_Items.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;
    }

    public void CallIntro2()
    { 
        PlayAudioRepeated(Sound_Intro3);
    }

    public void Selected_TutAnswer()
    {
        StopRepetedAudio();
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_Items.transform.GetChild(1).GetComponent<Image>().raycastTarget = false;
        Tut_Items.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();
        PlayAudioRepeated(Sound_Intro4);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopRepetedAudio();
        PlayAudio(Sound_Btn_Ok_Click, 0);
        TutHand2.gameObject.SetActive(false);
        CurrentItem = 1;
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(0, LengthDelay+0.25f);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);
        PlayAudio(Sound_Intro5, LengthDelay+ LengthDelay2 + 2f);
        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 2f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelsHolder.gameObject.SetActive(true);
        //ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsType1Ask + NoOfQuestionsType2Ask;

        if (Is_NeedRandomizedQuestions)
        {
            QuestionsOrder1 = RandomArray_Int(QuestionsOrder1);
            QuestionsOrder2 = RandomArray_Int(QuestionsOrder2);
        }

        QuestionOrderList = new List<int>();
        QuestionOrderList1 = new List<int>();

        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < NoOfQuestionsType1Ask; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            //------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i];
            QuesKeys.Add(AddKey);
        }
        for (int i = 0; i < NoOfQuestionsType2Ask; i++)
        {
            QuestionOrderList1.Add(QuestionsOrder2[i]);
           
        }

        Debug.Log("ThisGameKey:" + Thisgamekey);

        //--------------------DEBUGGING-------------------------------
        for (int i = 0; i < QuestionOrderList.Count; i++)
        {
            Debug.Log("OrderList:" + QuestionOrderList[i].ToString());
        }
        for (int i = 0; i < QuestionOrderList1.Count; i++)
        {
            Debug.Log("OrderList1:////////" + QuestionOrderList1[i].ToString());
        }
        //-------------------------------------------------------------------

        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);

        int tempq = 0;

        LevelsHolder.gameObject.SetActive(true);
        QuestionObj.SetActive(true);

        QuestionObj.transform.Find("Item_1").transform.SetSiblingIndex(0);
        QuestionObj.transform.Find("Item_3_Answer").transform.SetSiblingIndex(2);

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i] != null)
            {
                CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = false;
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        QuestionOrderListtemp = new List<int>();

        OptionsOrderTempList1 = new List<int>();
        OptionsOrderTempList2 = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            OptionsOrderTempList1.Add(QuestionsOrder1[i]);
        }
        for (int i = 0; i < QuestionsOrder2.Length; i++)
        {
            OptionsOrderTempList2.Add(QuestionsOrder2[i]);
        }

        TutorialObj.gameObject.SetActive(false);
        if (QuestionOrder1 < (QuestionOrderList.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CorrectAnsrIndex = QuestionsOrder1[tempq];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
            CurrentItem = 0;
        }
        else
        if (QuestionOrder2 < (QuestionOrderList1.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = QuestionOrder2;
            QuestionOrderListtemp.Remove(QuestionsOrder2[tempq]);
            CurrentQuestion = QuestionsOrder2[tempq];
            CorrectAnsrIndex = QuestionsOrder2[tempq];
            Debug.Log("Question No : " + QuestionOrder2 + " A : " + QuestionsOrder2[QuestionOrder2]);
            QuestionOrder2++;
            CurrentItem = 1;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
                Debug.Log("Here");
            }
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            CorrectAnsrIndex = WrongAnsweredQuestions1[tempq];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
            CurrentItem = 0;
        }
        else
        if (WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
                Debug.Log("Here");
            }
            tempq = WrongAnsweredQuestionOrder2;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[tempq]);
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            CorrectAnsrIndex = WrongAnsweredQuestions2[tempq];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            WrongAnsweredQuestionOrder2++;
            CurrentItem = 1;
        }

        if (CurrentItem == 0)
        {
            float _delay = 0;
            for (int i = 0; i < QuestionObj.transform.childCount; i++)
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(OptionsOrderTempList2);
                    QuestionObj.transform.GetChild(0).GetComponent<Image>().sprite = QuestionObj_Type2[_ixx];
                    OptionsOrderTempList2.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(OptionsOrderTempList2);
                    QuestionObj.transform.GetChild(1).GetComponent<Image>().sprite = QuestionObj_Type2[_iyy];
                    OptionsOrderTempList2.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    QuestionObj.transform.GetChild(2).GetComponent<Image>().sprite = QuestionObj_Type1[CurrentQuestion];
                    QuestionOrderListtemp.Remove(_izz);
                }

                iTween.ScaleFrom(QuestionObj.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                _delay += 0.1f;
            }
        }
        else
        {
            float _delay = 0;
            for (int i = 0; i < QuestionObj.transform.childCount; i++)
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(OptionsOrderTempList1);
                    QuestionObj.transform.GetChild(0).GetComponent<Image>().sprite = QuestionObj_Type1[_ixx];
                    OptionsOrderTempList1.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(OptionsOrderTempList1);
                    QuestionObj.transform.GetChild(1).GetComponent<Image>().sprite = QuestionObj_Type1[_iyy];
                    OptionsOrderTempList1.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    QuestionObj.transform.GetChild(2).GetComponent<Image>().sprite = QuestionObj_Type2[CurrentQuestion];
                    QuestionOrderListtemp.Remove(_izz);
                }

                iTween.ScaleFrom(QuestionObj.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                _delay += 0.1f;
            }
        }

        ItemsOrder = RandomArray_Int(ItemsOrder);

        int _randi = RandomNoFromList_Int(ItemsOrder);
        QuestionObj.transform.GetChild(2).SetSiblingIndex(_randi);
        if (_randi != 2)
        { QuestionObj.transform.GetChild(_randi + 1).SetSiblingIndex(2); }

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i] = QuestionObj.transform.GetChild(ItemsOrder[i]).gameObject;
        }

        PlayQuestionVoiceOver(CurrentItem);
        // Debug.Log("////////////" + PlayQuestionVoiceOver(CurrentItem));
        Invoke("EnableOptionsRaycast", QVOLength);
        Is_OkButtonPressed = false;
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = true;
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO[_Qi].EN_Sound_QO[Random.Range(0, 3)].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].EN_Sound_QO[Random.Range(0, 3)]);
                break;
            case 1:
                QVOLength = Sound_QVO[_Qi].HI_Sound_QO[Random.Range(0, 3)].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].HI_Sound_QO[Random.Range(0, 3)]);
                break;
            case 2:
                QVOLength = Sound_QVO[_Qi].TL_Sound_QO[Random.Range(0, 3)].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].HI_Sound_QO[Random.Range(0, 3)]);
                break;
        }
    }



    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        Is_CanClick = _IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);
    }
    string UserAnsr;
    int UserAnseri;
    public void Check_Answer(GameObject _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex.name;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i].name == _Ansrindex.name)
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StartAnim();
                UserAnseri = i;
            }
            else
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption", 7);
    }
    void RepeatQVOAftertChoosingOption()
    {
        StartCoroutine("PlayAudioRepeatedSingleCall");
    }
    void HighlightOptions()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i].name.Contains("Answer"))
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
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
        PlayAudio(Sound_Btn_Ok_Click, 0);
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = false;
        }
        StopRepetedAudio();
        if (UserAnsr.Contains("Answer"))
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
           // ProgreesBar.GetComponent<Slider>().value += 1;
            //INGAME_COMMON
            //MultiLevelManager.instance.UpdateProgress(1, 1);
            //INGAME_COMMON
            WrongAnsrsCount = 0;
            float LengthDelay = PlayAppreciationVoiceOver(0.25f);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentItem, LengthDelay + 0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 1.5f));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);

            if (QuestionOrder1 < (QuestionOrderList.Count) ||
                QuestionOrder2 < (QuestionOrderList1.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                Debug.Log("Questions Finished");
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
            Debug.Log("WRONG ANSWER : " + UserAnseri);
            iTween.ShakePosition(CurrentQuestionItems[UserAnseri].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentItem, 1f);
                Invoke("HighlightOptions", 0.5f);

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList.Count) && CurrentItem==0)
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionOrderList1.Count) && CurrentItem==1)
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

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                   QuestionOrder2 < (QuestionOrderList1.Count) ||
                   WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                   WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
                {
                    Invoke("GenerateLevel", Sound_IncorrectAnswer.clip.length + LengthDelay + 1.25f);
                }
                else
                {
                    Debug.Log("Questions Finished");
                    //StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2.5f));
                   // Invoke("ShowLC", Sound_IncorrectAnswer.clip.length + LengthDelay + 3f);
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
                for (int i = 0; i < CurrentQuestionItems.Length; i++)
                {
                    CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = true;
                    CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
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

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO.EN_Sound_AO[_Ai], _delay);
        ClipLength = Sound_AVO.EN_Sound_AO[_Ai].clip.length;

        return ClipLength;
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

    public QVO_AudioSource_C11011[] Sound_QVO;
    public AVO_AudioSource_C11011 Sound_AVO;

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

    [System.Serializable]
    public class AVO_AudioSource_C11011
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_C11011
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }
}
