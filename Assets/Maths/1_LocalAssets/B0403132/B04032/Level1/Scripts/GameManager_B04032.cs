using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B04032 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Shape;
    public GameObject Tut_Items;
    public GameObject TutHand1, TutHand2;
    public GameObject TutNumber;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject[] QuestionItems;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int[] AnswerOrder1;
    public int[] AnswerOrderTemp;
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
    public GameObject DownBar;
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

    /*bool Init;
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
            MultiLevelManager.instance.LoadProgressMaxValues(NoOfQuestionsToAsk);
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

       

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 2.5f);
        PlayAudio(Sound_Intro2, 2.5f+ Sound_Intro1[VOLanguage].clip.length);
        iTween.ScaleFrom(Tut_Shape.gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", 0.25f, "easetype", iTween.EaseType.easeOutElastic));

        float _delay = 1.0f;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay+0.5f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+0.5f));
            _delay += 0.1f;
        }

        iTween.MoveTo(Tut_Items.gameObject, iTween.Hash("y", 0, "time", 1f, "delay", 10, "easetype", iTween.EaseType.easeOutElastic));
        PlayAudio(Sound_Intro3, Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + 2.5f);
        Invoke("CallIntro3", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + 6);
        Invoke("EnableAnimator", 2);
        Invoke("EnableTutNoTRaycatTarget", 13.5f);
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

    public void CallIntro3()
    {
        StopAudio(Sound_Intro3);
        PlayAudioRepeated(Sound_Intro5);     
    }

    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_Items.transform.GetChild(1).GetComponent<Text>().raycastTarget = false;
        Tut_Items.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();
        StopAudio(Sound_Intro5);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro6);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro6);
        StopRepetedAudio();
        PlayAudio(Sound_Btn_Ok_Click, 0);
        TutHand2.gameObject.SetActive(false);
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        PlayAudio(Sound_Intro4,LengthDelay+0.25f);       
        PlayAudio(Sound_CorrectAnswer, LengthDelay + 3f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 3f);
        Tut_Items.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 2f);

        StartCoroutine(SetActiveWithDelayCall(Tut_Shape.transform.GetChild(0).gameObject, true, 0.5f));
        StartCoroutine(SetActiveWithDelayCall(Tut_Shape.transform.GetChild(1).gameObject, true, 0.5f));

        iTween.ScaleFrom(Tut_Shape.transform.GetChild(0).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", 0.5f, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleFrom(Tut_Shape.transform.GetChild(1).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", 0.5f, "easetype", iTween.EaseType.easeOutElastic));
        iTween.MoveTo(Tut_Items.gameObject, iTween.Hash("y", -400, "time", 1f, "delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));
        PlayAudio(Sound_Intro7, LengthDelay + 5.25f);
        Invoke("SetGamePlay", LengthDelay + Sound_Intro7[VOLanguage].clip.length + 6.5f);
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
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;
        }

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            AnswerOrder1[i] = AnswerOrderTemp[QuestionsOrder1[i]];
        }

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

        for (int i = 0; i < QuestionItems.Length; i++)
        {
            QuestionItems[i].gameObject.SetActive(false);
            QuestionItems[CurrentQuestionOrder].transform.GetChild(0).gameObject.SetActive(false);
            QuestionItems[CurrentQuestionOrder].transform.GetChild(1).gameObject.SetActive(false);
        }

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
            QuestionOrderListtemp.Add(i+1);
        }

        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {            
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(AnswerOrder1[tempq]);
            CurrentQuestion = AnswerOrder1[tempq];
            CurrentQuestionOrder = QuestionsOrder1[QuestionOrder1];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;

            QuestionOrderListtemp.Remove(AnswerOrder1[WrongAnsweredQuestions1[tempq]]);
            
            TargetArray = QuestionsOrder1;
            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];

            CurrentQuestion = AnswerOrderTemp[CurrentQuestionOrder];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }

        float _delay1 = 0;
        RandAnsrIndex = Random.Range(0, 3);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = "" + CurrentQuestion;
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _ixx;
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _iyy;
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _izz;
                    QuestionOrderListtemp.Remove(_izz);
                }
            }
        }

        LevelsHolder.gameObject.SetActive(true);
        QuestionItems[CurrentQuestionOrder].gameObject.SetActive(true);

        float _delay = 0;
        for (int i = 0; i < QuestionItems.Length; i++)
        {
            if (i == CurrentQuestionOrder)
            {
                iTween.ScaleFrom(QuestionItems[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            }
        }

        CorrectAnsrIndex = RandAnsrIndex;
        float LengthDelay=PlayQuestionVoiceOver(CurrentQuestionOrder);
        iTween.MoveTo(DownBar.gameObject, iTween.Hash("y", 0, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", LengthDelay + _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, LengthDelay + _delay1));
            _delay1 += 0.1f;
        }

        Is_OkButtonPressed = false;
        Debug.Log("CorrectAnsrIndex : " + CurrentQuestion);
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
        StartCoroutine(SetActiveWithDelayCall(QuestionItems[CurrentQuestionOrder].transform.GetChild(0).gameObject, true, LengthDelay));
        StartCoroutine(SetActiveWithDelayCall(QuestionItems[CurrentQuestionOrder].transform.GetChild(1).gameObject, true, LengthDelay));
        iTween.ScaleFrom(QuestionItems[CurrentQuestionOrder].transform.GetChild(0).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleFrom(QuestionItems[CurrentQuestionOrder].transform.GetChild(1).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));

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

    float LengthDelay;
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
            //INGAME_COMMON
            //MultiLevelManager.instance.UpdateProgress(1, 1);
            //INGAME_COMMON
            WrongAnsrsCount = 0;
            WrongAnsrsCount = 0;
            float LengthDelay = PlayAppreciationVoiceOver(0.25f);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestionOrder, LengthDelay+0.25f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);

            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 0.75f));
            StartCoroutine(SetActiveWithDelayCall(QuestionItems[CurrentQuestionOrder].transform.GetChild(0).gameObject, true, LengthDelay));
            StartCoroutine(SetActiveWithDelayCall(QuestionItems[CurrentQuestionOrder].transform.GetChild(1).gameObject, true, LengthDelay));

            iTween.ScaleFrom(QuestionItems[CurrentQuestionOrder].transform.GetChild(0).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));
            iTween.ScaleFrom(QuestionItems[CurrentQuestionOrder].transform.GetChild(1).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));
            iTween.MoveTo(DownBar.gameObject, iTween.Hash("y", -400, "time", 1f, "delay", LengthDelay + 1.5f, "easetype", iTween.EaseType.easeOutElastic));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);

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
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestionOrder, 1f);
                Invoke("HighlightOptions", 0.5f);
                iTween.MoveTo(DownBar.gameObject, iTween.Hash("y", -400, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.easeOutElastic));
                TargetList = WrongAnsweredQuestions1;

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestionOrder) && QuestionOrder1 <= (QuestionsOrder1.Length))
                {
                    if (WrongAnsweredQuestionOrder1 <= 0)
                    {
                        WrongAnsweredQuestions1.Add(CurrentQuestionOrder);
                    }
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                    //INGAME_COMMON
                    //MultiLevelManager.instance.UpdateProgress(1, 0);
                    //INGAME_COMMON
                    WrongAnsrsCount = 0;
                }

                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
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
    public AudioSource[] Sound_Intro6;
    public AudioSource[] Sound_Intro7;

    public AudioSource[] Sound_Q1;
    public AudioSource[] Sound_Q2;
    public AudioSource[] Sound_Q3;
    public AudioSource[] Sound_Q4;
    public AudioSource[] Sound_Q5;
    public AudioSource[] Sound_Q6;
    public AudioSource[] Sound_Q7;

    public AudioSource[] Sound_A1;
    public AudioSource[] Sound_A2;
    public AudioSource[] Sound_A3;
    public AudioSource[] Sound_A4;
    public AudioSource[] Sound_A5;
    public AudioSource[] Sound_A6;
    public AudioSource[] Sound_A7;

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
