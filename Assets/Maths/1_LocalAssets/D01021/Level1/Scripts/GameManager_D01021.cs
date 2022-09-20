using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_D01021 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject Tut_Sets;
    //public GameObject Tut_AnswerObjs;
    public GameObject TutHand1, TutHand2;
    public GameObject OptionHolder;
    public GameObject Tut_Downbar;
  //  public GameObject ItemSet;

   [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject OptionsHolder;
    public GameObject Downbar;
    
    public GameObject[] QuestionsHolder;
    public GameObject[] QuestionItems;
    public GameObject[] QuestionItemsChild;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int QuestionOrder1=0;
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

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
        PlayAudio(Sound_Intro2, 8f);
        PlayAudio(Sound_Intro3, 10f);

        float _delay = 0;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
             iTween.ScaleFrom(Tut_Items.transform.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay+16f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+16f));
            _delay += 0.2f;            
        }
        Invoke("EnableTut_Sets", Sound_Intro1[VOLanguage].clip.length +2.2f);

        Invoke("EnableAnimator", 2.7f);
        
        Invoke("CallIntro4", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro3[VOLanguage].clip.length + 4.5f);
        Invoke("EnableAnimator", 2f);
        
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
        OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void EnableTut_Sets()
    {
        Tut_Items.gameObject.SetActive(false);
        Tut_Sets.gameObject.SetActive(true);
    }
    public void CallIntro4()
    {
        PlayAudioRepeated(Sound_Intro4);
        Invoke("EnableTutRaycast", Sound_Intro4[VOLanguage].clip.length+0.25f);
        iTween.ScaleTo(OptionHolder.gameObject, iTween.Hash("x", 1, "islocal", true, "time", 1f, "delay", 0.5f, "easetype", iTween.EaseType.easeOutElastic));
    }

    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        iTween.ScaleTo(Tut_Downbar.gameObject, iTween.Hash("y", 1, "islocal", true, "time", 0.5f, "delay", 0.1f, "easetype", iTween.EaseType.easeOutElastic));
        Tut_Downbar.transform.GetChild(0).GetComponent<Text>().text = "400";
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget=false;
        OptionHolder.transform.GetChild(1).gameObject.GetComponent<PopTweenCustom>().StartAnim();
        StopAudio(Sound_Intro4);
        StopAudio(Sound_Intro3);
        StopAudio(Sound_Intro2);
        StopAudio(Sound_Intro1);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro5);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        PlayAudio(Sound_BtnOkClick, 0);
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro5);
        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);
        PlayAppreciationVoiceOver(0.5f);
       // PlayAudio(Sound_Intro6, 0);
        float LengthDelay = PlayAnswerVoiceOver(3,1.2f);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + 2.5f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 2.5f);
        PlayAudio(Sound_Intro7, LengthDelay +4f);
        OptionHolder.transform.GetChild(1).gameObject.GetComponent< PopTweenCustom>().Invoke("StopAnim", LengthDelay + 2f);
        Invoke("SetGamePlay", LengthDelay + Sound_Intro7[VOLanguage].clip.length+5f);
        iTween.ScaleTo(OptionHolder.gameObject, iTween.Hash("x", 0, "islocal", true, "time", 1f, "delay", LengthDelay + Sound_Intro7[VOLanguage].clip.length + 1.5f, "easetype", iTween.EaseType.easeOutElastic));
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        CurrentItem = 0;
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);
        LevelsHolder.gameObject.SetActive(true);
        ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length;

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

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
            WrongAnsweredQuestionOrder1++;
        }

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = "" +( CurrentQuestion*100);
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _ixx * 100;
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _iyy * 100;
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _izz * 100;
                    QuestionOrderListtemp.Remove(_izz);
                }
            }           
        }

        LevelsHolder.gameObject.SetActive(true);
        int ones =0;
        int hundreds = 0;

        if (CurrentQuestion >= 0)
        {
            hundreds = CurrentQuestion;
            Debug.Log("----String----" + ones + hundreds);
        }
        else
        {
            hundreds = ((CurrentQuestion * 100) % 100 - ones) / 10;
        }
        
        CurrentItems = new GameObject[ones+ hundreds];
        int _tempItemCount = 0;
        float _delay = 0;

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems.ToString();
            
            if (i< hundreds)
            {                
                if (i < 10)
                {
                    QuestionsHolder[0].gameObject.SetActive(true);
                    CurrentItems[_tempItemCount] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[0].transform);
                    iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                    StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                    _tempItemCount++;
                }
                else
                {
                    QuestionsHolder[1].gameObject.SetActive(true);
                    CurrentItems[_tempItemCount] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[1].transform);
                    iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                    StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                    _tempItemCount++;
                }               
            }
            if (i < ones)
            {
                QuestionsHolder[2].gameObject.SetActive(true);
                CurrentItems[_tempItemCount] = Instantiate(QuestionItemsChild[CurrentItem], QuestionsHolder[2].transform);
                iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                _tempItemCount++;
            }
            _delay += 0.1f;            
        }

        Is_OkButtonPressed = false;

        PlayQuestionVoiceOver1(CurrentQuestion - 1);
        iTween.ScaleTo(OptionsHolder.gameObject, iTween.Hash("x", 1, "islocal", true, "time", 1f, "delay", QVOLength2, "easetype", iTween.EaseType.easeOutElastic));
        Invoke("PlayQuestionVoiceOverWithDelay", QVOLength2);
        Debug.Log("CorrectAnsrIndex : " + CorrectAnsrIndex);
        float delay = 0.25f;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", QVOLength2 + delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, QVOLength2 + delay));
            delay += 0.1f;
        }
    }

    void PlayQuestionVoiceOverWithDelay()
    {
        PlayQuestionVoiceOver2(Random.RandomRange(0, 2));
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void EnableOptionsRaycast()
    {       
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
        }
    }   

    void PlayQuestionVoiceOver1(int _Qi)
    {
        switch (_Qi)
        {
            case 0:
                QVOLength2 = Sound_Q1[VOLanguage].clip.length;
                PlayAudio(Sound_Q1, 0);
                break;
            case 1:
                QVOLength2 = Sound_Q2[VOLanguage].clip.length;
                PlayAudio(Sound_Q2, 0);
                break;
            case 2:
                QVOLength2 = Sound_Q3[VOLanguage].clip.length;
                PlayAudio(Sound_Q3, 0);
                break;
            case 3:
                QVOLength2 = Sound_Q4[VOLanguage].clip.length;
                PlayAudio(Sound_Q4, 0);
                break;
            case 4:
                QVOLength2 = Sound_Q5[VOLanguage].clip.length;
                PlayAudio(Sound_Q5, 0);
                break;
            case 5:
                QVOLength2 = Sound_Q6[VOLanguage].clip.length;
                PlayAudio(Sound_Q6, 0);
                break;
            case 6:
                QVOLength2 = Sound_Q7[VOLanguage].clip.length;
                PlayAudio(Sound_Q7, 0);
                break;
            case 7:
                QVOLength2 = Sound_Q8[VOLanguage].clip.length;
                PlayAudio(Sound_Q8, 0);
                break;
            case 8:
                QVOLength2 = Sound_Q9[VOLanguage].clip.length;
                PlayAudio(Sound_Q9, 0);
                break;
            case 9:
                QVOLength2 = Sound_Q10[VOLanguage].clip.length;
                PlayAudio(Sound_Q10, 0);
                break;
        }

        //Invoke("PlayQuestionVoiceOver2", QVOLength2);
    }

    void PlayQuestionVoiceOver2(int _Qi)
    {
        switch (_Qi)
        {
            case 0:
                PlayAudioRepeated(Sound_QHalf_1);
                QVOLength = Sound_QHalf_1[VOLanguage].clip.length;
                break;
            case 1:
                PlayAudioRepeated(Sound_QHalf_2);
                QVOLength = Sound_QHalf_2[VOLanguage].clip.length;
                break;
        }       

        //Invoke("PlayQuestionVoiceOver2", QVOLength + 7);
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
    public void Check_Answer(int _Ansrindex)
    {
        StopRepetedAudio();
        //StopAudio(CurrentAudio);
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);
        iTween.ScaleTo(Downbar.gameObject, iTween.Hash("y", 1, "islocal", true, "time", 0.5f, "delay", 0.1f, "easetype", iTween.EaseType.easeOutElastic));
        Downbar.transform.GetChild(0).GetComponent<Text>().text = AnswerObjs[_Ansrindex].GetComponent<Text>().text;

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
                Downbar.transform.GetChild(0).GetComponent<Text>().text = AnswerObjs[i].GetComponent<Text>().text;
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
        PlayAudio(Sound_BtnOkClick, 0);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = false;          
        }
        
        StopRepetedAudio();
        StopAudio(CurrentAudio);

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
            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length+0.25f)+ Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion-1, LengthDelay + 0.5f);
           // PlayAnswerVOItemName(CurrentItem,LengthDelay+LengthDelay2+0.2f);
            
            iTween.ScaleTo(OptionsHolder.gameObject, iTween.Hash("x", 0,"islocal",true, "time", 1f, "delay", LengthDelay+LengthDelay2 + 2f, "easetype", iTween.EaseType.easeOutElastic));
            iTween.ScaleTo(Downbar.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.5f, "delay", LengthDelay + LengthDelay2 + 2f, "easetype", iTween.EaseType.easeOutElastic));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 1f); 
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 1f);
            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2.5f));
            
            if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))                
            {
                Invoke("GenerateLevel", LengthDelay+LengthDelay2 +4f);
            }
            else
            {
                Debug.Log("Questions Finished");
                //Invoke("ShowLC", LengthDelay+LengthDelay2 + 4f);
                SendResultFinal();
            }
            //CancelInvoke("PlayQuestionVoiceOver1");
            //CancelInvoke("PlayQuestionVoiceOver2");
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
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float Length = PlayAnswerVoiceOver(CurrentQuestion-1,1f);
                Invoke("HighlightOptions", Sound_IncorrectAnswer.clip.length);
                iTween.ScaleTo(Downbar.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.5f, "delay", Length+0.25f, "easetype", iTween.EaseType.easeOutElastic));
                iTween.ScaleTo(OptionsHolder.gameObject, iTween.Hash("x", 0, "islocal", true, "time", 1f, "delay", Sound_IncorrectAnswer.clip.length+ Length+0.25f, "easetype", iTween.EaseType.easeOutElastic));
                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    if (AnswerObjs[i] != null)
                    {
                        AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                    }
                }
                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionsOrder1.Length))
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }               
                else
                {
                   // ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                   
                {
                    Invoke("GenerateLevel", Length+2.5f);
                }
                else
                {
                    Debug.Log("Questions Finished");
                    // StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, Length + 2f));
                    //Invoke("ShowLC", Length + 2.5f);
                    SendResultFinal();
                }
                //CancelInvoke("PlayQuestionVoiceOver1");
                //CancelInvoke("PlayQuestionVoiceOver2");
                CancelInvoke("RepeatQVOAftertChoosingOption");
                WrongAnsrsCount = 0;
            }
            else
            {
                Is_OkButtonPressed = false;
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("RepeatQVOAftertChoosingOption", 1);
                //Invoke("PlayQuestionVoiceOver1", 1);
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
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;
    public AudioSource[] Sound_Intro5;
    public AudioSource[] Sound_Intro6;
    public AudioSource[] Sound_Intro7;
    public AudioSource[] Sound_Intro8;
    public AudioSource[] Sound_Intro9;

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

    public AudioSource[] Sound_QHalf_1;
    public AudioSource[] Sound_QHalf_2;

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

    AudioSource[] CurrentAudio;
    public void PlayAudio(AudioSource[] _audio, float _delay)
    {
        CurrentAudio = _audio;
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
    float QVOLength, QVOLength2;
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
