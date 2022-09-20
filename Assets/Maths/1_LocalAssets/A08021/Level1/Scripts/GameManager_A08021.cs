using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_A08021 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject[] Tut_Items;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

    

    public GameObject[] AllItems;
    public GameObject[] QuestionsHolder;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    
     public int[] QuestionOrder;
    public int QuestionOrder1;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public string CorrectAnserString;

    int WrongAnsrsCount;

    int CurrentItem;
    public GameObject[] CurrentItems;

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
            Thisgamekey = "na01081";

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

        Invoke("CallIntro1", 0f);

        float _delay = 0;
        for (int i = 0; i < Tut_Items.Length; i++)
        {
            iTween.MoveFrom(Tut_Items[i].gameObject, iTween.Hash("y", 1500, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 0.2f);
    }
    void CallIntro1()
    {
        PlayAudio(Sound_Intro1, 0f);
    }
    void SetQues(int TotalQues, string Thisgamekey)
    {

        // create a list of questions being posed in the game
        List<string> QuesKeys = new List<string>();


        // Add the questions keys to the list
        for (int i = 0; i < TotalQues; i++)
        {
            // example key A05011_Q01
            string AddKey = "" + Thisgamekey + "_Q" + i;
            QuesKeys.Add(AddKey);
            Debug.Log("Add : " + AddKey);
        }
        // send the list of questions to initialize the 
        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

    }

    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro2);
        Tut_Items[7].gameObject.GetComponent<Image>().raycastTarget = true;
    }

    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);

        Tut_Items[7].gameObject.GetComponent<Image>().raycastTarget = false;
        Tut_Items[7].gameObject.GetComponent<PopTweenCustom>().StartAnim();

        StopAudio(Sound_Intro2);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro3);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);

        PlayAudio(Sound_Selection, 0);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);

        CurrentItem = 0;

        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(1, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2);
        Tut_Items[7].gameObject.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 1f);

        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 2f);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 3f);
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
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionOrder.Length;
        }
       

        if (Is_NeedRandomizedQuestions)
        { QuestionOrder = RandomArray_Int(QuestionOrder); }

        //RandNumSprites = new Sprite[NumSprites.Length];

        QuestionOrderList = new List<int>();

        for (int i = 0; i < QuestionOrder.Length; i++)
        {
            QuestionOrderList.Add(QuestionOrder[i]);
           // RandNumSprites[i] = NumSprites[QuestionOrder[i]];
        }

        StartCoroutine(SetOk_Button(false, 0f));

        CurrentItems = new GameObject[2];

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;

        LevelsHolder.gameObject.SetActive(false);

        //QuestionsHolder[CurrentItem].gameObject.SetActive(false);

        for (int i = 0; i < AllItems.Length; i++)
        {
            if (AllItems[i] != null)
            {
                AllItems[i].GetComponent<Image>().raycastTarget = false;
                AllItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        QuestionOrderListtemp = new List<int>();

        TutorialObj.gameObject.SetActive(false);

        if (QuestionOrder1 < (QuestionOrder.Length))
        {
            for (int i = 0; i < QuestionOrder.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionOrder[i]);
            }
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionOrder[tempq]);
            CurrentQuestion = QuestionOrder[tempq];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionOrder[QuestionOrder1]);
            QuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            for (int i = 0; i < QuestionOrder.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionOrder[i]);
                Debug.Log("Here");
            }
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }
        
        LevelsHolder.gameObject.SetActive(true);
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems[i] = QuestionsHolder[CurrentQuestion].transform.GetChild(i).gameObject;
        }

        float _delay = 0;
        for (int i = 0; i < AllItems.Length; i++)
        {
            iTween.MoveFrom(AllItems[i].gameObject, iTween.Hash("y", 1500, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
            //CurrentItems[i].GetComponent<Image>().raycastTarget = true;
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));            
            _delay += 0.1f;
        }

        int _inttmep = Random.Range(0, 2);
        CorrectAnsrIndex = _inttmep;
        string[] _tempstring= QuestionsHolder[CurrentQuestion].gameObject.name.Split("_"[0]);

        CorrectAnserString = _tempstring[0] + "_" + _tempstring[1];

        Is_OkButtonPressed = false;
        PlayQuestionVoiceOver(CorrectAnsrIndex);

        Invoke("EnableAllitemsRaycast", QVOLength+0.25f);
        Debug.Log("CorrectAnsrIndex : " + CorrectAnsrIndex +": "+ CorrectAnserString);
    }

    void EnableAllitemsRaycast()
    {
        for (int i = 0; i < AllItems.Length; i++)
        {
            if (AllItems[i] != null)
            {
                AllItems[i].GetComponent<Image>().raycastTarget = true;
            }
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (CurrentQuestion)
        {
            case 0:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Building_Yellow_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Building_Yellow_Right);                        
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Building_Yellow_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Building_Yellow_Left);
                        break;
                }
                break;
            case 1:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Building_Red_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Building_Red_Right);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Building_Red_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Building_Red_Left);
                        break;
                }
                break;
            case 2:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Building_Blue_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Building_Blue_Right);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Building_Blue_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Building_Blue_Left);
                        break;
                }
                break;
            case 3:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Tree_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Tree_Right);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Tree_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Tree_Left);
                        break;
                }
                break;
            case 4:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Boat_Red_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Boat_Red_Right);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Boat_Red_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Boat_Red_Left);
                        break;
                }
                break;
            case 5:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Boat_Blue_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Boat_Blue_Right);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Boat_Blue_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Boat_Blue_Left);
                        break;
                }
                break;
            case 6:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Boat_Green_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Boat_Green_Right);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Boat_Green_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Boat_Green_Left);
                        break;
                }
                break;
            case 7:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q1_Boat_Yellow_Right[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Boat_Yellow_Right);
                        break;
                    case 1:
                        QVOLength = Sound_Q2_Boat_Yellow_Left[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Boat_Yellow_Left);
                        break;
                }
                break;
            case 8:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q3_Boat_Red_Middle[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_Boat_Red_Middle);
                        break;
                    case 1:
                        QVOLength = Sound_Q3_Boat_Blue_Middle[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_Boat_Blue_Middle);
                        break;
                    case 2:
                        QVOLength = Sound_Q3_Boat_Green_Middle[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_Boat_Green_Middle);
                        break;
                }
                break;

        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        switch (CurrentItem)
        {
            case 0:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A1_Building_Right, _delay);
                        ClipLength = Sound_A1_Building_Right[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A2_Building_Left, _delay);
                        ClipLength = Sound_A2_Building_Left[VOLanguage].clip.length;
                        break;
                }
                break;
            case 1:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A1_Tree_Right, _delay);
                        ClipLength = Sound_A1_Tree_Right[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A2_Tree_Left, _delay);
                        ClipLength = Sound_A2_Tree_Left[VOLanguage].clip.length;
                        break;
                }
                break;
            case 2:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A1_Boat_Right, _delay);
                        ClipLength = Sound_A1_Boat_Right[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A2_Boat_Left, _delay);
                        ClipLength = Sound_A2_Boat_Left[VOLanguage].clip.length;
                        break;
                }
                break;
            case 3:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A3_Boat_Middle, _delay);
                        ClipLength = Sound_A3_Boat_Middle[VOLanguage].clip.length;
                        break;
                }
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
    string _UserInput;
    public void Check_Answer(GameObject _AnsrObj)
    {
        StopRepetedAudio();
        string _itemName = _AnsrObj.gameObject.name;
        _UserInput = _itemName;
        UserAnsr = int.Parse(_itemName.Substring(_itemName.Length-1));
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);

        for (int i = 0; i < AllItems.Length; i++)
        {
            if (AllItems[i].name.Contains(_itemName))
            {
                AllItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AllItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

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

        for (int i = 0; i < AllItems.Length; i++)
        {
            AllItems[i].GetComponent<Image>().raycastTarget = false;
        }

        StopRepetedAudio();

        if (CurrentQuestion <= 2)
        {CurrentItem = 0;}
        else
        if (CurrentQuestion == 3)
        {CurrentItem = 1;}
        else
        if (CurrentQuestion > 3)
        {CurrentItem = 2;}

        if (_UserInput.Contains(CorrectAnserString) && UserAnsr== CorrectAnsrIndex)
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
            WrongAnsrsCount = 0;

            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length) + Sound_BtnOkClick.clip.length;           
           
            float LengthDelay2 = PlayAnswerVoiceOver(UserAnsr, LengthDelay + 0.25f);
            
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);

            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + +LengthDelay2 + 0.5f);

            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2));
            
            if (QuestionOrder1 < (QuestionOrder.Length) ||
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

            for (int i = 0; i < AllItems.Length; i++)
            {
                if (AllItems[i].name.Contains(_UserInput))
                {
                    iTween.ShakePosition(AllItems[i].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                }
            }

            for (int i = 0; i < AllItems.Length; i++)
            {
                if (AllItems[i] != null)
                {
                    AllItems[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }

            PlayAudio(Sound_IncorrectAnswer, 0);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CorrectAnsrIndex, 1f);

                Invoke("HighlightCorrectOption", 1);

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrder.Length))
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                {
                    ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrder.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Game Over W");
                    SendResultFinal();
                   // Invoke("ShowLC", LengthDelay + 2.5f);
                }
                CancelInvoke("RepeatQVOAftertChoosingOption");
                WrongAnsrsCount = 0;
            }
            else
            {
                Is_OkButtonPressed = false;
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("RepeatQVOAftertChoosingOption", 1);
                for (int i = 0; i < AllItems.Length; i++)
                {
                    AllItems[i].GetComponent<Image>().raycastTarget = true;
                    AllItems[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    void HighlightCorrectOption()
    {
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (CurrentItems[i].gameObject.name.Contains(CorrectAnserString) && i == CorrectAnsrIndex)
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
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

    public AudioSource[] Sound_Q1_Building_Yellow_Right;
    public AudioSource[] Sound_Q2_Building_Yellow_Left;

    public AudioSource[] Sound_Q1_Building_Red_Right;
    public AudioSource[] Sound_Q2_Building_Red_Left;

    public AudioSource[] Sound_Q1_Building_Blue_Right;
    public AudioSource[] Sound_Q2_Building_Blue_Left;

    public AudioSource[] Sound_Q1_Tree_Right;
    public AudioSource[] Sound_Q2_Tree_Left;

    public AudioSource[] Sound_Q1_Boat_Red_Right;
    public AudioSource[] Sound_Q2_Boat_Red_Left;
    public AudioSource[] Sound_Q3_Boat_Red_Middle;

    public AudioSource[] Sound_Q1_Boat_Blue_Right;
    public AudioSource[] Sound_Q2_Boat_Blue_Left;
    public AudioSource[] Sound_Q3_Boat_Blue_Middle;

    public AudioSource[] Sound_Q1_Boat_Green_Right;
    public AudioSource[] Sound_Q2_Boat_Green_Left;
    public AudioSource[] Sound_Q3_Boat_Green_Middle;

    public AudioSource[] Sound_Q1_Boat_Yellow_Right;
    public AudioSource[] Sound_Q2_Boat_Yellow_Left;

    public AudioSource[] Sound_A1_Building_Right;
    public AudioSource[] Sound_A2_Building_Left;

    public AudioSource[] Sound_A1_Tree_Right;
    public AudioSource[] Sound_A2_Tree_Left;

    public AudioSource[] Sound_A1_Boat_Right;
    public AudioSource[] Sound_A2_Boat_Left;
    public AudioSource[] Sound_A3_Boat_Middle;

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
