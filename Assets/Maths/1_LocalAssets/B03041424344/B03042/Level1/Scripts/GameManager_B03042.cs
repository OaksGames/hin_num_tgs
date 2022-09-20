using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;


public class GameManager_B03042 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject[] TutNumberObjs;
    public GameObject TutNumber;
    public GameObject TutBtn_Okay;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;

    public GameObject LevelHolder;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;

    public GameObject[] NumberObjs;
    public GameObject[] NumberPos;

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
    public int CurrentQuestionOrder;

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
        LevelHolder.gameObject.SetActive(false);

        PlayAudio(Sound_Intro1, 2f);

        float _delay = 0;
        for (int i = 0; i < TutNumberObjs.Length; i++)
        {
            iTween.ScaleFrom(TutNumberObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

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
        Invoke("TutEnableRaycast", Sound_Intro2[VOLanguage].clip.length + 0.5f);
    }

    public void TutEnableRaycast()
    {
        TutNumberObjs[4].gameObject.GetComponent<Image>().raycastTarget = true;
    }

    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutNumberObjs[4].transform.GetComponent<Button>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        TutNumberObjs[4].transform.GetComponent<PopTweenCustom>().StartAnim();

        StopAudio(Sound_Intro2);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro3);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        PlayAudio(Sound_BtnOkClick, 0);
        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);

        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(4, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 1f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 1f);
        TutNumberObjs[4].transform.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 1f);

        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 2f);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 3f);
    }

    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length;
        }

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));

        CurrentItems = new GameObject[NumberPos.Length];

        GenerateLevel();
    }

    int RandAnsrIndex = 0;
    public void GenerateLevel()
    {
        TutorialObj.gameObject.SetActive(false);
        LevelHolder.gameObject.SetActive(true);

        if (CurrentItems.Length >= 0)
        {
            for (int i = 0; i < CurrentItems.Length; i++)
            {
                if (CurrentItems[i] != null)
                    Destroy(CurrentItems[i].gameObject);
            }
        }

        for (int i = 0; i < NumberPos.Length; i++)
        {
            NumberPos[i].GetComponent<PopTweenCustom>().StopAnim();
            NumberPos[i].GetComponent<Button>().interactable = false;
        }

        int tempq = 0;

        QuestionOrderListtemp = new List<int>();

        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }

            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentItem = 0;
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);

            CurrentQuestionOrder = QuestionsOrder1[QuestionOrder1];
            ArrangeNumbers();
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
            CurrentItem = 0;
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);

            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];
            ArrangeNumbers();
            WrongAnsweredQuestionOrder1++;
        }

        Is_OkButtonPressed = false;

        CorrectAnsrIndex = RandAnsrIndex;

        Invoke("PlayQuestionVO", 0.5f);
    }

    public void ArrangeNumbers()
    {
        if (CurrentQuestionOrder < 4)
        {
            RandAnsrIndex = Random.Range(0, (CurrentQuestionOrder + 1));
            Debug.Log("RandAnsrIndex : <" + RandAnsrIndex);
        }
        else
        if (CurrentQuestionOrder == 4)
        {
            RandAnsrIndex = Random.Range(0, (CurrentQuestionOrder));
            Debug.Log("RandAnsrIndex : =" + RandAnsrIndex);
        }
        else
        {
            RandAnsrIndex = Random.Range(CurrentQuestionOrder - 5, 5);
            Debug.Log("RandAnsrIndex : >" + RandAnsrIndex);
        }

        float _delay = 0;
        for (int i = 0; i < NumberPos.Length; i++)
        {
            if (CurrentQuestionOrder == 0 || CurrentQuestionOrder == QuestionsOrder1.Length)
            {
                CurrentItems[i] = Instantiate(NumberObjs[i], NumberPos[i].transform);
                NumberPos[i].GetComponent<Button>().enabled = true;
                iTween.ScaleFrom(NumberPos[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                Debug.Log("Question No : " + CurrentQuestionOrder);
                RandAnsrIndex = 0;
                _delay += 0.2f;
            }
            else
            {
                if (i < RandAnsrIndex)
                {
                    int _index = CurrentQuestionOrder - (RandAnsrIndex - i);
                    CurrentItems[i] = Instantiate(NumberObjs[_index], NumberPos[i].transform);
                    NumberPos[i].GetComponent<Button>().enabled = true;
                    Debug.Log("RandAnsrIndex : i " + _index);
                }
                if (i == RandAnsrIndex)
                {
                    int _index = i;
                    CurrentItems[i] = Instantiate(NumberObjs[CurrentQuestionOrder], NumberPos[i].transform);
                    NumberPos[i].GetComponent<Button>().enabled = true;
                    Debug.Log("RandAnsrIndex : i" + _index);
                }
                if (i > RandAnsrIndex)
                {
                    int _index = CurrentQuestionOrder + (i - RandAnsrIndex);
                    CurrentItems[i] = Instantiate(NumberObjs[_index], NumberPos[i].transform);
                    NumberPos[i].GetComponent<Button>().enabled = true;
                    Debug.Log("RandAnsrIndex : i" + _index);
                }

                iTween.ScaleFrom(NumberPos[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                _delay += 0.1f;
            }
        }
    }

    public void PlayQuestionVO()
    {
        PlayQuestionVoiceOver(CurrentQuestion);
        Invoke("EnableOptionsRaycast", QVOLength + 0.1f);
    }
    void EnableOptionsRaycast()
    {
        for (int i = 0; i < NumberPos.Length; i++)
        {
            NumberPos[i].GetComponent<Button>().enabled = true;
            NumberPos[i].GetComponent<Button>().interactable = true;
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (_Qi)
        {
            case 0:
                QVOLength = Sound_Q0[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q0);
                break;
            case 1:
                QVOLength = Sound_Q1[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q1);
                break;
            case 2:
                QVOLength = Sound_Q2[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q2);
                break;
            case 3:
                QVOLength = Sound_Q3[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q3);
                break;
            case 4:
                QVOLength = Sound_Q4[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q4);
                break;
            case 5:
                QVOLength = Sound_Q5[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q5);
                break;
            case 6:
                QVOLength = Sound_Q6[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q6);
                break;
            case 7:
                QVOLength = Sound_Q7[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q7);
                break;
            case 8:
                QVOLength = Sound_Q8[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q8);
                break;
            case 9:
                QVOLength = Sound_Q9[VOLanguage].clip.length;
                PlayAudioRepeated(Sound_Q9);
                break;
        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        switch (_Ai)
        {
            case 0:
                PlayAudio(Sound_A0, _delay);
                ClipLength = Sound_A0[VOLanguage].clip.length;
                break;
            case 1:
                PlayAudio(Sound_A1, _delay);
                ClipLength = Sound_A1[VOLanguage].clip.length;
                break;
            case 2:
                PlayAudio(Sound_A2, _delay);
                ClipLength = Sound_A2[VOLanguage].clip.length;
                break;
            case 3:
                PlayAudio(Sound_A3, _delay);
                ClipLength = Sound_A3[VOLanguage].clip.length;
                break;
            case 4:
                PlayAudio(Sound_A4, _delay);
                ClipLength = Sound_A4[VOLanguage].clip.length;
                break;
            case 5:
                PlayAudio(Sound_A5, _delay);
                ClipLength = Sound_A5[VOLanguage].clip.length;
                break;
            case 6:
                PlayAudio(Sound_A6, _delay);
                ClipLength = Sound_A6[VOLanguage].clip.length;
                break;
            case 7:
                PlayAudio(Sound_A7, _delay);
                ClipLength = Sound_A7[VOLanguage].clip.length;
                break;
            case 8:
                PlayAudio(Sound_A8, _delay);
                ClipLength = Sound_A8[VOLanguage].clip.length;
                break;
            case 9:
                PlayAudio(Sound_A9, _delay);
                ClipLength = Sound_A9[VOLanguage].clip.length;
                break;
        }
        return ClipLength;
    }

    int UserAnsr;
    public void Check_Answer(int _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (i == _Ansrindex)
            {
                CurrentItems[i].transform.parent.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                CurrentItems[i].transform.parent.GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        PlayAudio(Sound_Selection, 0);
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
        StopRepetedAudio();

        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;

        PlayAudio(Sound_BtnOkClick, 0);
        for (int i = 0; i < NumberPos.Length; i++)
        {
            NumberPos[i].GetComponent<Button>().interactable = false;
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
            //INGAME_COMMON
            //MultiLevelManager.instance.UpdateProgress(1, 1);
            //INGAME_COMMON
            WrongAnsrsCount = 0;

            float LengthDelay = PlayAppreciationVoiceOver(0);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);


            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);
            StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + LengthDelay2 + 1.5f));

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

            Debug.Log("WRONG ANSWER : ");
            iTween.ShakePosition(CurrentItems[UserAnsr].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 0);
                Invoke("HighlightOptions", 1);

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionsOrder1.Length))
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                    //INGAME_COMMON
                    //MultiLevelManager.instance.UpdateProgress(1, 0);
                    //INGAME_COMMON
                }

                StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + 2.25f));

                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Game Over W");

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
                for (int i = 0; i < NumberPos.Length; i++)
                {
                    NumberPos[i].GetComponent<Button>().interactable = true;
                }
            }

            for (int i = 0; i < NumberPos.Length; i++)
            {
                NumberPos[i].GetComponent<PopTweenCustom>().StopAnim();
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

    void HighlightOptions()
    {
        for (int i = 0; i < NumberPos.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                NumberPos[i].transform.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                NumberPos[i].transform.GetComponent<PopTweenCustom>().StopAnim();
            }
        }
    }
    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        Is_CanClick = _IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);
    }

    #endregion

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

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;

    public AudioSource[] Sound_Q0;
    public AudioSource[] Sound_Q1;
    public AudioSource[] Sound_Q2;
    public AudioSource[] Sound_Q3;
    public AudioSource[] Sound_Q4;
    public AudioSource[] Sound_Q5;
    public AudioSource[] Sound_Q6;
    public AudioSource[] Sound_Q7;
    public AudioSource[] Sound_Q8;
    public AudioSource[] Sound_Q9;

    public AudioSource[] Sound_A0;
    public AudioSource[] Sound_A1;
    public AudioSource[] Sound_A2;
    public AudioSource[] Sound_A3;
    public AudioSource[] Sound_A4;
    public AudioSource[] Sound_A5;
    public AudioSource[] Sound_A6;
    public AudioSource[] Sound_A7;
    public AudioSource[] Sound_A8;
    public AudioSource[] Sound_A9;

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
        _audiotorepeat[VOLanguage].PlayDelayed(0);
        yield return new WaitForSeconds(7 + QVOLength);
        StartCoroutine("PlayAudioRepeatedCall");
    }

    public void StopRepetedAudio()
    {
        StopAudio(_audiotorepeat);
        StopCoroutine("PlayAudioRepeatedCall");
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

    #region SETACTIVEWITHDELAY
    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }
    #endregion

    #region SETIMAGERAYCASTWITHDELAY
    public IEnumerator SetImageRaycastTarget(Image _targetImage, float _delay, bool _state)
    {
        yield return new WaitForSeconds(_delay);
        _targetImage.GetComponent<Image>().raycastTarget = _state;
    }
    #endregion
}
