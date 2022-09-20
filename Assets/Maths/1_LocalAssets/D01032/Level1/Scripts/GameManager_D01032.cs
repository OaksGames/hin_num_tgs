using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_D01032 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject[] Tut_Items;
    public GameObject Tut_Sets;
    public GameObject TutHand1, TutHand2, TutHand3,TutHand4,TutHand5,TutHand6;
    public GameObject OptionHolder;
    public GameObject Tut_Baket1;
    public GameObject Tut_Baket2;
    public GameObject Tut_Baket3;
    public GameObject[] Tut_AnswerObjs;
    public GameObject GemSack;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject OptionsHolder;
    public GameObject[] Downbar;

    public GameObject[] QuestionsHolder;
    public GameObject[] QuestionItems;
    public GameObject[] QuestionItemsChild;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] TotalNumofQuestions;
    public int[] QuestionsOrder1;
   
    //[HideInInspector]
    public int[] RandQuestionsOrder1;
    public int QuestionOrder1=0;
    public int RandQuestionOrder1 = 0;
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int RandCurrentQuestion;

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
            AddValueInProgress = 1 / (float)NoOfQuestionsToAsk;
            Thisgamekey = gameInputData.Key;

        }
        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);
        iTween.ScaleFrom(GemSack.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0.25f, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0.25f));
        PlayAudio(Sound_IntroBegining, 2f);
        StartCoroutine(SetActiveWithDelayCall(GemSack.gameObject, false, 6.5f));

        for (int i = 0; i < Tut_Items.Length; i++)
        {
            StartCoroutine(SetActiveWithDelayCall(Tut_Items[i].gameObject, true, 0f));
        }

        float _delay = 0.25f;
        for (int i = 0; i < Tut_Items.Length; i++)
        {
            iTween.ScaleFrom(Tut_Items[i].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay+7f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+7f));
            _delay += 0.1f;
        }
        PlayAudio(Sound_Intro1, _delay+8f);

        Invoke("EnableAnimator", 2f);

        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length  + _delay + 9f); 

    }
    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;
    }

    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }
    public void EnableTutRaycast()
    {
        OptionHolder.transform.GetChild(2).gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void EnableTutRaycast2()
    {
        OptionHolder.transform.GetChild(5).gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void EnableTutRaycast3()
    {
        OptionHolder.transform.GetChild(7).gameObject.GetComponent<Text>().raycastTarget = true;
    }

    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro2);

        Invoke("EnableTutRaycast", Sound_Intro2[VOLanguage].clip.length);
    }

    public void Selected_TutAnswer1()
    {
        Basket1Txt();
        TutBtn_Okay.gameObject.SetActive(true);
        StopAudio(Sound_Intro2);
        StopAudio(Sound_Intro1);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro3);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
        OptionHolder.transform.GetChild(2).GetComponent<PopTweenCustom>().StartAnim();
    }
    public void Selected_TutAnswer2()
    {
        Basket2Txt();
        TutBtn_Okay.gameObject.SetActive(true);
        StopAudio(Sound_Intro4);
        StopAudio(Sound_Intro5);
        StopAudio(Sound_Intro3);
        StopAudio(Sound_Intro2);
        StopAudio(Sound_Intro1);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro6);
        TutHand3.gameObject.SetActive(false);
        TutHand4.gameObject.SetActive(true);
        OptionHolder.transform.GetChild(5).GetComponent<PopTweenCustom>().StartAnim();
    }
    public void Selected_TutAnswer3()
    {
        Basket3Txt();
        TutBtn_Okay.gameObject.SetActive(true);
        StopAudio(Sound_Intro8);
        StopAudio(Sound_Intro7);
        StopAudio(Sound_Intro6);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro9);
        TutHand5.gameObject.SetActive(false);
        TutHand6.gameObject.SetActive(true);
        OptionHolder.transform.GetChild(7).GetComponent<PopTweenCustom>().StartAnim();
    }
    public void CallIntro5()
    {
        PlayAudioRepeated(Sound_Intro5);

        Invoke("EnableTutRaycast2", Sound_Intro5[VOLanguage].clip.length + 0.2f);
       
    }
    public void CallIntro8()
    {
        PlayAudioRepeated(Sound_Intro8);

        Invoke("EnableTutRaycast3", Sound_Intro8[VOLanguage].clip.length + 0.5f);

    }
    public void Basket1Txt()
    {
        Tut_Baket1.transform.GetChild(0).gameObject.SetActive(false);
        OptionHolder.transform.GetChild(2).gameObject.GetComponent<Text>().raycastTarget = false;
        Tut_Baket1.transform.GetChild(1).gameObject.SetActive(true);
        iTween.ScaleFrom(Tut_Baket1.transform.GetChild(1).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0));
    }
    public void Basket2Txt()
    {
        Tut_Baket2.transform.GetChild(0).gameObject.SetActive(false);
        OptionHolder.transform.GetChild(5).gameObject.GetComponent<Text>().raycastTarget = false;
        Tut_Baket2.transform.GetChild(1).gameObject.SetActive(true);
        iTween.ScaleFrom(Tut_Baket2.transform.GetChild(1).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0));
    }
    public void Basket3Txt()
    {
        Tut_Baket3.transform.GetChild(0).gameObject.SetActive(false);
        OptionHolder.transform.GetChild(8).gameObject.GetComponent<Text>().raycastTarget = false;
        Tut_Baket3.transform.GetChild(1).gameObject.SetActive(true);
        iTween.ScaleFrom(Tut_Baket3.transform.GetChild(1).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0));
    }

    public void Hand3on()
    {
        TutHand3.gameObject.SetActive(true);
    }
    public void Hand5on()
    {
        TutHand5.gameObject.SetActive(true);
    }
    int _val = 0;

    public void HighlightTut_Answers()
    {
        for (int i = 0; i < Tut_AnswerObjs.Length; i++)
        {
            Tut_AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
        }
    }

    public void BtnAct_OkTut()
    {

        PlayAudio(Sound_BtnOkClick, 0);
        TutBtn_Okay.gameObject.SetActive(false);
        StopRepetedAudio();
        
        if ( _val == 0)
        {
                _val += 1;
            TutHand2.gameObject.SetActive(false);
            StopAudio(Sound_Intro3);
            OptionHolder.transform.GetChild(2).GetComponent<PopTweenCustom>().StopAnim();

            Invoke("Hand3on", 3.5f);
           
            PlayAudio(Sound_Intro4, 2.5f);
            
            Invoke("CallIntro5", 8f);
        }
        else
        if ( _val == 1)
        {
            _val += 1;
            TutHand4.gameObject.SetActive(false);
            OptionHolder.transform.GetChild(5).GetComponent<PopTweenCustom>().StopAnim();
            StopAudio(Sound_Intro6);
            Invoke("Hand5on", 3f);
            PlayAudio(Sound_Intro7, 2.5f);
            Invoke("CallIntro8", 8f);
            
        }
        else
        {
            OptionHolder.transform.GetChild(7).GetComponent<PopTweenCustom>().StopAnim();
            TutHand6.gameObject.SetActive(false);
            StopAudio(Sound_Intro9);
            PlayAudio(Sound_BtnOkClick, 0);
            float LengthDelay = PlayAppreciationVoiceOver(0.2f);
            PlayAudio(Sound_Intro10, LengthDelay + 0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + 4.5f);
            Invoke("HighlightTut_Answers", LengthDelay + 0.5f);
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 4.5f);
            PlayAudio(Sound_Intro11, LengthDelay + 5f);
            Invoke("SetGamePlay", LengthDelay + Sound_Intro9[VOLanguage].clip.length + 7f);
            iTween.ScaleTo(OptionHolder.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", LengthDelay + 4f, "easetype", iTween.EaseType.easeOutElastic));
        }
    }
        
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        CurrentItem = 0;
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);
        LevelsHolder.gameObject.SetActive(true);
        ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;

        RandQuestionsOrder1 = new int[NoOfQuestionsToAsk];
        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            RandQuestionsOrder1[i] = i;
        }

        if (Is_NeedRandomizedQuestions)
        { RandQuestionsOrder1 = RandomArray_Int(RandQuestionsOrder1); }

        QuestionOrderList = new List<int>();

        QuestionsOrder1= new int[NoOfQuestionsToAsk];
        List<string> QuesKeys = new List<string>();


        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionsOrder1[i] = TotalNumofQuestions[RandQuestionsOrder1[i]];
        }

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i].ToString();
            QuesKeys.Add(AddKey);
        }

        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));
        CurrentItems = new GameObject[3];
        GenerateLevel();
    }

    int hundreds;
    int tens;
    int ones;

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;

        _countdigit = 0;
        hundreds = 0;
        tens = 0;
        ones = 0;

        LevelsHolder.gameObject.SetActive(false);
        for (int i = 0; i < QuestionsHolder.Length; i++)
        {
            QuestionsHolder[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "0";
                AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        for (int i = 0; i < Downbar.Length; i++)
        {
            Downbar[i].GetComponent<Text>().raycastTarget = false;
        }

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if(CurrentItems[i]!=null)
            Destroy(CurrentItems[i].gameObject);
        }

        QuestionOrderListtemp = new List<int>();
        TutorialObj.gameObject.SetActive(false);

        if (QuestionOrder1 < (QuestionOrderList.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }
            tempq = QuestionOrder1;
            tempq= RandQuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[QuestionOrder1]; 
            RandCurrentQuestion = RandQuestionsOrder1[QuestionOrder1];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            CurrentTempQuestion = QuestionOrder1;
            CurrentItem = 0;
            QuestionOrder1++;
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
            CurrentTempQuestion = FindIndexofElementinArray(CurrentQuestion);
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            CurrentItem = 0;
            RandCurrentQuestion = RandQuestionsOrder1[CurrentTempQuestion];
            WrongAnsweredQuestionOrder1++;
        }

        LevelsHolder.gameObject.SetActive(true);

        CorrectAnsrIndex = CurrentQuestion;

        hundreds = CurrentQuestion/100;
        tens = (CurrentQuestion-(CurrentQuestion/100)*100)/10;
        ones = (CurrentQuestion - (hundreds * 100))-(tens*10);

        Debug.Log("Hundreds : " + hundreds+": Tens : "+tens+"Ones : "+ones);
        
        CurrentItems = new GameObject[tens + hundreds+ones ];
        int _tempItemCount = 0;
        float _delay = 0.5f;

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems.ToString();
            
            if (i< hundreds)
            {                
                if (i < 10)
                {
                    QuestionsHolder[0].gameObject.SetActive(true);
                    CurrentItems[_tempItemCount] = Instantiate(QuestionItems[0], QuestionsHolder[0].transform);
                    iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                    StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                    _tempItemCount++;
                }               
            }
            
            if (i < tens)
            {
                QuestionsHolder[1].gameObject.SetActive(true);
                CurrentItems[_tempItemCount] = Instantiate(QuestionItems[1], QuestionsHolder[1].transform);
                iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                _tempItemCount++;
            }

            if (i < ones)
            {
                QuestionsHolder[2].gameObject.SetActive(true);
                CurrentItems[_tempItemCount] = Instantiate(QuestionItems[2], QuestionsHolder[2].transform);
                iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                _tempItemCount++;
            }
            _delay += 0.1f;            
        }

        Is_OkButtonPressed = false;

        iTween.ScaleTo(OptionsHolder.gameObject, iTween.Hash("y", 1, "islocal", true, "time", 1f, "delay", QVOLength, "easetype", iTween.EaseType.easeOutElastic));

        float _delay1 = 0;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay",QVOLength + _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting,QVOLength + _delay1));
            _delay1 += 0.1f;
        }    
       
        Invoke("PlayQuestionVoiceOverWithDelay", CurrentItems.Length * 0.1f);
        Debug.Log("CorrectAnsrIndex : " + CorrectAnsrIndex);
    }

    void PlayQuestionVoiceOverWithDelay()
    {
        PlayQuestionVoiceOver(_countdigit);
        Invoke("EnableOptionsRaycast", QVOLength + 0.2f);
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < Downbar.Length; i++)
        {
            Downbar[i].GetComponent<Text>().raycastTarget = true;
        }
    }

    float PlayQuestionVoiceOver(int _Qi)
    {
        int RandomQuestion = Random.Range(0, 2);
        if (VOLanguage == 0)
        {
            QVOLength = Sound_QVO[_Qi].EN_Sound_QVO[RandomQuestion].clip.length;
            PlayAudioRepeated(Sound_QVO[_Qi].EN_Sound_QVO[RandomQuestion]);
        }
        if (VOLanguage == 1)
        {
            QVOLength = Sound_QVO[_Qi].HI_Sound_QVO[RandomQuestion].clip.length;
            PlayAudioRepeated(Sound_QVO[_Qi].HI_Sound_QVO[RandomQuestion]);
        }
        if (VOLanguage == 2)
        {
            QVOLength = Sound_QVO[_Qi].TL_Sound_QVO[RandomQuestion].clip.length;
            PlayAudioRepeated(Sound_QVO[_Qi].TL_Sound_QVO[RandomQuestion]);
        }
        return QVOLength;
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        if (VOLanguage == 0)
        {
            ClipLength = Sound_AVO.EN_Sound_AVO[_Ai].clip.length;
            PlayAudio(Sound_AVO.EN_Sound_AVO[_Ai], _delay);
        }
        if (VOLanguage == 1)
        {
            ClipLength = Sound_AVO.HI_Sound_AVO[_Ai].clip.length;
            PlayAudio(Sound_AVO.HI_Sound_AVO[_Ai], _delay);
        }
        if (VOLanguage == 2)
        {
            ClipLength = Sound_AVO.TL_Sound_AVO[_Ai].clip.length;
            PlayAudio(Sound_AVO.TL_Sound_AVO[_Ai], _delay);
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
        //StartCoroutine("PlayAudioRepeatedCall");
        PlayQuestionVoiceOver(_countdigit);
    }

    int _countdigit;
    int _UserAnsrHundred;
    int _UserAnsrTen;
    int _UserAnsrOnes;
    int OptionIndex;
    public void AddCount(int _value)
    {
        OptionIndex = _value - 1;
        for (int i = 0; i < Downbar.Length; i++)
        {
            if (Downbar[i].name.Contains(OptionIndex.ToString()))
            {
                Downbar[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                Downbar[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        if (_countdigit < 3)
        {
            StopRepetedAudio();

            AnswerObjs[_countdigit].GetComponent<Text>().text = "" + _value;
            iTween.ScaleFrom(AnswerObjs[_countdigit].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutBounce));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0));
            
            if (_countdigit >= 2)
            {
                _UserAnsrOnes = _value;
                StartCoroutine(SetOk_Button(true, 0));
                UserAnsr = (_UserAnsrHundred * 100) + (_UserAnsrTen * 10)+ _UserAnsrOnes;
            }
            else
            if (_countdigit >= 1)
            {
                _UserAnsrTen = _value;
                StartCoroutine(SetOk_Button(true, 0));
                UserAnsr = (_UserAnsrHundred * 100) + (_UserAnsrTen * 10);
            }
            else
            {
                _UserAnsrHundred = _value;
                StartCoroutine(SetOk_Button(true, 0));
            }

            CancelInvoke("RepeatQVOAftertChoosingOption");
            Invoke("RepeatQVOAftertChoosingOption", 7);
        }
    }

    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        for (int i = 0; i < Downbar.Length; i++)
        {
            if (Downbar[i].name.Contains(OptionIndex.ToString()))
            {
                Downbar[i].GetComponent<PopTweenCustom>().StopAnim();
            }

        }
        Invoke("RepeatQVOAftertChoosingOption", 0.25f);
        if (_countdigit >= 2)
        {
            Is_OkButtonPressed = true;

            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
            }
            for (int i = 0; i < Downbar.Length; i++)
            {
                Downbar[i].GetComponent<Text>().raycastTarget = false;
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
                //ProgreesBar.GetComponent<Slider>().value += 1;
                WrongAnsrsCount = 0;

                float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length + 0.25f) + Sound_BtnOkClick.clip.length;
                float LengthDelay2 = PlayAnswerVoiceOver(RandCurrentQuestion, LengthDelay + 0.5f);

                Invoke("HighlightOptions", 0);
                // PlayAnswerVOItemName(CurrentItem,LengthDelay+LengthDelay2+0.2f);

                iTween.ScaleTo(OptionsHolder.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 1f, "delay", LengthDelay + LengthDelay2 + 2f, "easetype", iTween.EaseType.easeOutElastic));
                Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);
                PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
                StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2.5f));

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 4f);
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
                        iTween.ShakePosition(AnswerObjs[i].gameObject, iTween.Hash("x", 10f, "time", 0.25f));
                    }
                }
                PlayAudio(Sound_IncorrectAnswer, 0);
                WrongAnsrsCount++;
                if (WrongAnsrsCount >= 2)
                {
                    float Length = PlayAnswerVoiceOver(RandCurrentQuestion, 0);
                    Invoke("HighlightOptions", Sound_IncorrectAnswer.clip.length);
                    iTween.ScaleTo(OptionsHolder.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 1f, "delay", Sound_IncorrectAnswer.clip.length + Length, "easetype", iTween.EaseType.easeOutElastic));
                    for (int i = 0; i < AnswerObjs.Length; i++)
                    {
                        if (AnswerObjs[i] != null)
                        {
                            AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                        }
                    }
                    if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList.Count))
                    {
                        WrongAnsweredQuestions1.Add(CurrentQuestion);
                    }
                    else
                    {
                       // ProgreesBar.GetComponent<Slider>().value += 1;
                    }

                    if (QuestionOrder1 < (QuestionOrderList.Count) ||
                        WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))

                    {
                        Invoke("GenerateLevel", Length + 2f);
                    }
                    else
                    {
                        Debug.Log("Questions Finished");
                        //StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, Length + 1.5f));
                        //Invoke("ShowLC", Length+2.5f);
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
                        if (AnswerObjs[i] != null)
                        {
                            AnswerObjs[i].GetComponent<Text>().text = "0";
                            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
                            AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                        }
                    }
                    for (int i = 0; i < Downbar.Length; i++)
                    {
                        Downbar[i].GetComponent<Text>().raycastTarget = true;
                    }
                }
                _countdigit = 0;
            }
        }
        else
        _countdigit++;

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

    void HighlightOptions()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
        }

        AnswerObjs[0].GetComponent<Text>().text = "" + hundreds;
        AnswerObjs[1].GetComponent<Text>().text = "" + tens;
        AnswerObjs[2].GetComponent<Text>().text = "" + ones;
    }

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }

    public void ShowLC()
    {
        LCObj.gameObject.SetActive(true);
    }

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_IntroBegining;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;
    public AudioSource[] Sound_Intro5;
    public AudioSource[] Sound_Intro6;
    public AudioSource[] Sound_Intro7;
    public AudioSource[] Sound_Intro8;
    public AudioSource[] Sound_Intro9;
    public AudioSource[] Sound_Intro10;
    public AudioSource[] Sound_Intro11;

    public QO_AudioSource_D01032[] Sound_QVO;
    public AO_AudioSource_D01032 Sound_AVO;
    
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

    #region FIND INDEX OF ELEMENT IN A ARRAY
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
    #endregion
}

[System.Serializable]
public class AO_AudioSource_D01032
{
    public AudioSource[] EN_Sound_AVO;
    public AudioSource[] HI_Sound_AVO;
    public AudioSource[] TL_Sound_AVO;
}

[System.Serializable]
public class QO_AudioSource_D01032
{
    public AudioSource[] EN_Sound_QVO;
    public AudioSource[] HI_Sound_QVO;
    public AudioSource[] TL_Sound_QVO;
}
