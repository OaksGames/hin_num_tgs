using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B03082 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject Tut_Midpanel;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

    public GameObject[] QuestionItems;
    public GameObject[] QuestionsHolder;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] TotalNumofQuestions1;
    public int[] RandQuestionsOrder1;
    public int[] QuestionsOrder1;
    public int QuestionOrder1 = 0;
    public int RandQuestionOrder1 = 0;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int CurrentQuestionOrder;

    int WrongAnsrsCount;

    int CurrentItem;
    public GameObject[] CurrentItems;

    public GameObject CircleImg;
    public GameObject MidPanel;
    public Color ColorQstnText;
    public Color ColorNormalText;

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

        LevelsHolder.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 2.2f);
        float _delay = 0.25f;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }
        Invoke("EnableAnimator", 2);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length - 2.5f);
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
        Tut_Items.transform.GetChild(5).gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void CallIntro2()
    {
        StopAudio(Sound_Intro1);
        PlayAudioRepeated(Sound_Intro2);
        Invoke("EnableTutNoTRaycatTarget", Sound_Intro2[VOLanguage].clip.length +0.1f);
    }
    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        Tut_Items.transform.GetChild(5).GetComponent<Text>().raycastTarget = false;
        Tut_Items.transform.GetChild(5).GetComponent<PopTweenCustom>().StartAnim();
        Tut_Items.transform.GetChild(10).gameObject.SetActive(true);
        iTween.ScaleTo(Tut_Midpanel.gameObject, iTween.Hash("Scale", Vector3.one, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
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
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);
        CurrentItem = 0;
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(6, LengthDelay + 0.25f);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);
        Tut_Items.transform.GetChild(5).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 1f);
        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 2.5f);
        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 3f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        CurrentItem = 0;
        TutorialObj.gameObject.SetActive(false);
        LevelsHolder.gameObject.SetActive(true);
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;
        }

        RandQuestionsOrder1 = new int[NoOfQuestionsToAsk];
        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            RandQuestionsOrder1[i] = i;
        }

        if (Is_NeedRandomizedQuestions)
        {
            RandQuestionsOrder1 = RandomArray_Int(RandQuestionsOrder1);
        }

        QuestionOrderList = new List<int>();
        QuestionsOrder1 = new int[NoOfQuestionsToAsk];

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionsOrder1[i] = TotalNumofQuestions1[RandQuestionsOrder1[i]];
        }

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionOrderList.Add(RandQuestionsOrder1[i]);
        }
        StartCoroutine(SetOk_Button(false, 0f));
        CurrentItems = new GameObject[2];
        GenerateLevel();
    }

    int RandAnsrIndex = 0;
    public void GenerateLevel()
    {
        TutorialObj.gameObject.SetActive(false);
        int tempq = 0;
        CurrentItem = 0;
        QuestionsHolder[CurrentItem].gameObject.SetActive(false);
        CircleImg.gameObject.SetActive(false);
        for (int i = 0; i < QuestionItems.Length; i++)
        {
            if (QuestionItems[i] != null)
            {
                QuestionItems[i].GetComponent<Text>().raycastTarget = false;
                QuestionItems[i].GetComponent<Text>().color = ColorNormalText;
                QuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        MidPanel.transform.localScale = Vector3.zero;
        if (QuestionOrder1 < (QuestionOrderList.Count))
        {
            tempq = QuestionOrder1;
            RandQuestionOrder1 = RandQuestionsOrder1[QuestionOrder1];
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentQuestionOrder = QuestionOrder1;
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            ArrangeNumbers();
            QuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            TargetArray = QuestionsOrder1;
            CurrentQuestionOrder = FindIndexofElementinArray(CurrentQuestion);
            RandQuestionOrder1 = RandQuestionsOrder1[CurrentQuestionOrder];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            ArrangeNumbers();
            WrongAnsweredQuestionOrder1++;
        }      
        
        CurrentItem = CurrentQuestion;
        LevelsHolder.gameObject.SetActive(true);
        QuestionsHolder[0].gameObject.SetActive(true);

        float _delay = 0;
        for (int i = 0; i < QuestionItems.Length; i++)
        {
            iTween.ScaleFrom(QuestionItems[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));            
            _delay += 0.1f;
        }

        Is_OkButtonPressed = false;
        CorrectAnsrIndex = RandAnsrIndex-1;
        Invoke("PlayQuestionVO", 0.5f);
        Debug.Log("CorrectAnsrIndex : " + CurrentQuestion);
    }

    void PlayQuestionVO()
    {
        PlayQuestionVoiceOver(RandQuestionOrder1);
        Invoke("EnableOptionsRaycast", QVOLength + 0.1f);
    }

    void EnableOptionsRaycast()
    {       
        for (int i = 0; i < QuestionItems.Length; i++)
        {
            QuestionItems[i].GetComponent<Text>().raycastTarget = true;
        }
    }
    public void ArrangeNumbers()
    {
        RandAnsrIndex = Random.Range(1, (QuestionItems.Length));
        for (int i = 0; i < QuestionItems.Length; i++)
        {
            if (i < RandAnsrIndex)
            {
                int _index = CurrentQuestion - (RandAnsrIndex - i);
                QuestionItems[i].GetComponent<Button>().enabled = true;
                QuestionItems[i].GetComponent<Text>().color = ColorNormalText;
                QuestionItems[i].GetComponent<Text>().text = "" + _index;
                QuestionItems[i].transform.localScale = Vector3.one * 0.5f;
                QuestionItems[i].GetComponent<PopTweenCustom>().ObjScale = Vector3.one * 0.5f;
                Debug.Log("RandAnsrIndex : i " + _index);
            }
            if (i == RandAnsrIndex)
            {
                int _index = CurrentQuestion;
                QuestionItems[i].GetComponent<Button>().enabled = false;
                QuestionItems[i].GetComponent<Text>().color = ColorQstnText;
                QuestionItems[i].GetComponent<Text>().text = "" + _index;
                QuestionItems[i].transform.localScale = Vector3.one * 0.65f;
                QuestionItems[i].GetComponent<PopTweenCustom>().ObjScale= Vector3.one * 0.65f;
                Debug.Log("RandAnsrIndex : i" + _index);
            }
            if (i > RandAnsrIndex)
            {
                int _index = CurrentQuestion + (i - RandAnsrIndex);
                QuestionItems[i].GetComponent<Button>().enabled = true;
                QuestionItems[i].GetComponent<Text>().color = ColorNormalText;
                QuestionItems[i].GetComponent<Text>().text = "" + _index;
                QuestionItems[i].transform.localScale = Vector3.one * 0.5f;
                QuestionItems[i].GetComponent<PopTweenCustom>().ObjScale = Vector3.one * 0.5f;
                Debug.Log("RandAnsrIndex : i" + _index);
            }
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        if (VOLanguage == 0)
        {
            QVOLength = Sound_QVO.EN_Sound_QVO[_Qi].clip.length;
            PlayAudioRepeated(Sound_QVO.EN_Sound_QVO[_Qi]);
        }
        if (VOLanguage == 1)
        {
            QVOLength = Sound_QVO.HI_Sound_QVO[_Qi].clip.length;
            PlayAudioRepeated(Sound_QVO.HI_Sound_QVO[_Qi]);
        }
        if (VOLanguage == 2)
        {
            QVOLength = Sound_QVO.TL_Sound_QVO[_Qi].clip.length;
            PlayAudioRepeated(Sound_QVO.TL_Sound_QVO[_Qi]);
        }

    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {

        float ClipLength = 0;
        if (VOLanguage == 0)
        {
            ClipLength = Sound_AVO.EN_Sound_AVO[_Ai].clip.length;
            PlayAudio(Sound_AVO.EN_Sound_AVO[_Ai], _delay);
            Debug.Log("Sound : " + CurrentItem);
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
        iTween.ScaleTo(MidPanel.gameObject, iTween.Hash("Scale", Vector3.one, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleFrom(MidPanel.transform.GetChild(0).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
        MidPanel.transform.GetChild(0).GetComponent<Text>().text= ""+int.Parse(QuestionItems[_UserInput].gameObject.GetComponent<Text>().text);
        CircleImg.gameObject.SetActive(true);
        CircleImg.transform.localPosition = QuestionItems[_UserInput].gameObject.transform.localPosition;
        StopRepetedAudio();
        UserAnsr = _UserInput;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);
        for (int i = 0; i < QuestionItems.Length; i++)
        {
            if (i==_UserInput)
            {
                QuestionItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                QuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
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
        PlayAudio(Sound_BtnOkClick, 0);
        
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

            }

            for (int i = 0; i < QuestionItems.Length; i++)
            {
                QuestionItems[i].GetComponent<Text>().raycastTarget = false;
            }
            Debug.Log("CORRECT ANSWER : " + UserAnsr);
            if (Testing)
            {
                ProgreesBar.GetComponent<Slider>().value += 1;
            }
            WrongAnsrsCount = 0;
            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length+0.25f) + Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(RandQuestionOrder1, LengthDelay + 0.25f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);            
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);
            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2f));

            if (QuestionOrder1 < (QuestionOrderList.Count) ||
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
            CircleImg.gameObject.SetActive(false);
            for (int i = 0; i < QuestionItems.Length; i++)
            {
                if (i==UserAnsr)
                {
                    iTween.ShakePosition(QuestionItems[i].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                    Debug.Log("SHAKE WRONG ANSWER : " + i);

                    if (FrameworkOff == false)
                    {
                        string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                        GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Wrong);
                        Debug.Log("Add : " + AddKey + ": Wrong");
                    }
                }
            }
            iTween.ScaleTo(MidPanel.gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                for (int i = 0; i < QuestionItems.Length; i++)
                {
                    QuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
                    QuestionItems[i].GetComponent<Text>().raycastTarget = false;
                }
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestionOrder, 1f);
                Invoke("HighlightCorrectOption", 1);
                TargetList = WrongAnsweredQuestions1;
                if (!WrongAnsweredQuestions1.Contains(FindIndexofElementinList(CurrentQuestion)) && QuestionOrder1 <= (QuestionOrderList.Count))
                {
                    if(WrongAnsweredQuestionOrder1<=0)
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                {
                   // ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Game Over W");
                    StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2f));
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
                for (int i = 0; i < QuestionItems.Length; i++)
                {                   
                    QuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    void HighlightCorrectOption()
    {
        for (int i = 0; i < QuestionItems.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                QuestionItems[i].GetComponent<PopTweenCustom>().StartAnim();
                CircleImg.gameObject.SetActive(true);
                CircleImg.transform.localPosition = QuestionItems[i].gameObject.transform.localPosition;
            }
            else
            {
                QuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
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

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }
    public void ShowLC()
    {
        LCObj.gameObject.SetActive(true);
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
    
    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;

    public QO_AudioSource_B03082 Sound_QVO;
    public AO_AudioSource_B03082 Sound_AVO;

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
public class AO_AudioSource_B03082
{
    public AudioSource[] EN_Sound_AVO;
    public AudioSource[] HI_Sound_AVO;
    public AudioSource[] TL_Sound_AVO;
}

[System.Serializable]
public class QO_AudioSource_B03082
{
    public AudioSource[] EN_Sound_QVO;
    public AudioSource[] HI_Sound_QVO;
    public AudioSource[] TL_Sound_QVO;
}
