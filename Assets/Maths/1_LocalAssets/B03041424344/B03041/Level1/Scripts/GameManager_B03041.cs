using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B03041 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_QuestionsHolder;
    public GameObject TutHand1, TutHand2;
    public GameObject Tut_SidePanel;
    public GameObject Tut_DownPanel;

    [Header("=========== GAMEPLAY CONTENT============")]
    public GameObject LevelObj;
    //public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

    public GameObject[] QuestionItems;
    public GameObject[] QuestionsHolder;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;

    int WrongAnsrsCount;

    int CurrentItem;
    public GameObject[] CurrentItems;

    public GameObject HandObj3;
    public GameObject SidePanel;
    public GameObject DownPanel;

    public int NumberFrom;
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
        //ProgreesBar.SetActive(false);
        btn_Back.SetActive(false);
        SetTutorial(data);
    }

    public void CleanUp()
    {
        // throw new System.NotImplementedException();
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
            MultiLevelManager.instance.LoadProgressMaxValues(QuestionsOrder1.Length);
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

        PlayAudio(Sound_Intro1, 2f);

        Invoke("EnableAnimator", 2);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 2f);
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

    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro2);
    }

    public void Add_TutItem()
    {
        if (Tut_QuestionsHolder.transform.GetChild(0).childCount < 1)
        {
            GameObject _TutTempItem = Instantiate(QuestionItems[0].gameObject, Tut_QuestionsHolder.transform.GetChild(0).transform);
            Tut_SidePanel.transform.GetChild(0).GetComponent<Button>().interactable = true;
            StartCoroutine(PlayAudioAtOneShot(Sound_Selection, 0));
            Tut_DownPanel.transform.GetChild(0).GetComponent<Text>().text = "" + (NumberFrom + _tempOrder + 1);
            iTween.ScaleFrom(_TutTempItem.gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
            iTween.ScaleFrom(Tut_DownPanel.transform.GetChild(0).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
            _tempOrder++;
            BtnAct_OkTut();
        }
        else
        {
            Tut_SidePanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }

        iTween.MoveTo(Tut_SidePanel.gameObject, iTween.Hash("x", 800, "islocal", true, "time", 0.5f, "delay", 0.2f, "easetype", iTween.EaseType.easeInOutElastic));

    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro3);
        StopRepetedAudio();

        CurrentItem = 0;

        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(0, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 1f);

        Invoke("EnableHand2", LengthDelay);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 1f);

        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 2f);

        StartCoroutine(SetActiveWithDelayCall(TutHand2, false, LengthDelay + LengthDelay2 + 2f));

        Invoke("SetGamePlay", LengthDelay + +LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 3f);
    }

    public void EnableHand2()
    {
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }


    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        CurrentItem = 0;

        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);

        LevelsHolder.gameObject.SetActive(true);

        //ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length;
        //ProgreesBar.GetComponent<Slider>().value += 1;

        DownPanel.transform.GetChild(0).GetComponent<Text>().text = "" + NumberFrom;

        QuestionOrderList = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));

        CurrentItems = new GameObject[10];

        CurrentItems[_tempOrder] = Instantiate(QuestionItems[0].gameObject, QuestionsHolder[1].transform.GetChild(0).transform);
        DownPanel.transform.GetChild(0).GetComponent<Text>().text = "" + (NumberFrom + _tempOrder);

        QuestionOrder1++;

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        TutorialObj.gameObject.SetActive(false);

        int RandAnsrIndex = Random.Range(0, 2);
        int tempq = 0;

        LevelsHolder.gameObject.SetActive(false);

        QuestionOrderListtemp = new List<int>();

        Invoke("MakeEnableInput", 2);
        iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 0.5f, "delay", 0f, "easetype", iTween.EaseType.easeInOutElastic));

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
            QuestionOrder1++;
        }

        LevelsHolder.gameObject.SetActive(true);

        HandObj3.gameObject.SetActive(true);

        Is_OkButtonPressed = false;

        int _inttmep = Random.Range(0, 2);
        CorrectAnsrIndex = _inttmep;
        PlayQuestionVoiceOver(_inttmep);
        Debug.Log("CorrectAnsrIndex : " + CorrectAnsrIndex);
    }

    public void MakeEnableInput()
    {
        SidePanel.transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
    }

    int _tempOrder = 0;
    public void Add_Item()
    {
        if (QuestionsHolder[1].transform.GetChild(0).childCount <= 9)
        {
            CurrentItems[_tempOrder] = Instantiate(QuestionItems[0].gameObject, QuestionsHolder[1].transform.GetChild(0).transform);
            SidePanel.transform.GetChild(0).GetComponent<Button>().interactable = true;
            StartCoroutine(PlayAudioAtOneShot(Sound_Selection, 0));
            DownPanel.transform.GetChild(0).GetComponent<Text>().text = "" + (NumberFrom + _tempOrder + 1);
            iTween.ScaleFrom(CurrentItems[_tempOrder].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
            iTween.ScaleFrom(DownPanel.transform.GetChild(0).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
            _tempOrder++;
            BtnAct_Ok();
        }
        else
        {
            SidePanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }

        iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 800, "islocal", true, "time", 0.5f, "delay", 0.2f, "easetype", iTween.EaseType.easeInOutElastic));

    }

    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        Is_CanClick = _IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);
    }


    int UserAnsr;
    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        Is_OkButtonPressed = true;

        HandObj3.gameObject.SetActive(false);

        SidePanel.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;

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
        //INGAME_COMMON
        //MultiLevelManager.instance.UpdateProgress(1, 1);
        //INGAME_COMMON
        WrongAnsrsCount = 0;

        StopRepetedAudio();
        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(QuestionOrder1 - 1, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);

        if (QuestionOrder1 < (QuestionsOrder1.Length) ||
            WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 3);
        }
        else
        {
            Debug.Log("Game Over C");
            //Invoke("ShowLC", LengthDelay + LengthDelay2 + 3);
            SendResultFinal();
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (_Qi)
        {
            case 0:
                QVOLength = Sound_Q1_Tap[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q1_Tap);
                break;
            case 1:
                QVOLength = Sound_Q2_Add[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q2_Add);
                break;
        }
    }

    public float PlayAnswerVoiceOver(int _Ai,float _delay)
    {
        float ClipLength = 0;
        switch (CurrentItem)
        {
            case 0:
                switch (_Ai)
                {
                    case 0: 
                        PlayAudio(Sound_Intro3, _delay);
                        ClipLength = Sound_Intro3[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A82_Items, _delay);
                        ClipLength = Sound_A82_Items[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A83_Items, _delay);
                        ClipLength = Sound_A83_Items[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A84_Items, _delay);
                        ClipLength = Sound_A84_Items[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A85_Items, _delay);
                        ClipLength = Sound_A85_Items[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A86_Items, _delay);
                        ClipLength = Sound_A86_Items[VOLanguage].clip.length;
                        break;
                    case 6:
                        PlayAudio(Sound_A87_Items, _delay);
                        ClipLength = Sound_A87_Items[VOLanguage].clip.length;
                        break;
                    case 7:
                        PlayAudio(Sound_A88_Items, _delay);
                        ClipLength = Sound_A88_Items[VOLanguage].clip.length;
                        break;
                    case 8:
                        PlayAudio(Sound_A89_Items, _delay);
                        ClipLength = Sound_A89_Items[VOLanguage].clip.length;
                        break;
                    case 9:
                        PlayAudio(Sound_A90_Items, _delay);
                        ClipLength = Sound_A90_Items[VOLanguage].clip.length;
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

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
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

    public AudioSource[] Sound_Q1_Tap;
    public AudioSource[] Sound_Q2_Add;

    public AudioSource[] Sound_A82_Items;
    public AudioSource[] Sound_A83_Items;
    public AudioSource[] Sound_A84_Items;
    public AudioSource[] Sound_A85_Items;
    public AudioSource[] Sound_A86_Items;
    public AudioSource[] Sound_A87_Items;
    public AudioSource[] Sound_A88_Items;
    public AudioSource[] Sound_A89_Items;
    public AudioSource[] Sound_A90_Items;

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
