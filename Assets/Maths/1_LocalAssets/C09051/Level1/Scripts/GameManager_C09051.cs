using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_C09051 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject TutHand1, TutHand2;
    public GameObject TutHand3, TutHand4;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

    [HideInInspector]
    public bool Is_CanClick;

    public GameObject[] QuestionObjs;
    public int[] QNoOfMarks;
    public GameObject[] CurrentQuestionItems;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

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
        } */      
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
            AddValueInProgress = 1 / (float)NoOfQuestionsToAsk;
            Thisgamekey = gameInputData.Key;
        }

        TutorialObj.gameObject.SetActive(true);
        LevelsHolder.gameObject.SetActive(false);

        PlayAudio(Sound_Intro1, 2f);

        float _delay = 0;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        Invoke("EnableAnimator", 2);
        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length + 2.2f);
        Invoke("CallIntro3", Sound_Intro2[VOLanguage].clip.length + Sound_Intro1[VOLanguage].clip.length + 2.2f);
    }

    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;
    }

    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }

    public void CallIntro3()
    {
        QVOLength = Sound_Intro3[VOLanguage].clip.length;
        PlayAudioRepeated(Sound_Intro3);
    }

    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        
        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        QVOLength = Sound_Intro4[VOLanguage].clip.length;
        PlayAudioRepeated(Sound_Intro4);

        TutHand4.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    int TutFindCount;
    public void Tut_FoundChange(GameObject _change)
    {
        _change.gameObject.transform.parent.GetComponent<Button>().enabled = false;
        TutFindCount++;
        PlayAudio(Sound_Ting, 0);
        iTween.ScaleTo(_change.gameObject, iTween.Hash("Scale", Vector3.one, "time", 1f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));

        if (TutFindCount == 1)
        {
            TutHand1.gameObject.SetActive(false);
            TutHand3.gameObject.SetActive(true);
            Tut_Items.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Image>().raycastTarget = true;
        }
        else
        if (TutFindCount == 2)
        {
            TutHand3.gameObject.SetActive(false);
            TutHand4.gameObject.SetActive(true);
            Tut_Items.transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<Image>().raycastTarget = true;
        }
        else
        if (TutFindCount >= 3)
        {
            Selected_TutAnswer();
        }
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro4);
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);

        CurrentItem = 1;
        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(0, LengthDelay);
     
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);        

        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            Tut_Items.transform.GetChild(i).GetComponent<PopTweenCustom>().Invoke("StartAnim", LengthDelay);
            Tut_Items.transform.GetChild(i).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + LengthDelay2 + 2f);
        }

        PlayAudio(Sound_Intro5, LengthDelay+ LengthDelay2 + 2f);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro5[VOLanguage].clip.length + 3f);
    }

    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        TutorialObj.gameObject.SetActive(false);
        LevelsHolder.gameObject.SetActive(true);

        ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();
        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            //------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i];
            QuesKeys.Add(AddKey);
        }

        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;
        FindCount = 0;
        LevelsHolder.gameObject.SetActive(false);

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i] != null)
            {
                CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = false;
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
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
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CorrectAnsrIndex = QuestionsOrder1[tempq];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }

        LevelsHolder.gameObject.SetActive(true);
        for (int i = 0; i < QuestionObjs.Length; i++)
        {
            if (i == CurrentQuestion)
            {
                QuestionObjs[i].gameObject.SetActive(true);
            }
            else
            {
                QuestionObjs[i].gameObject.SetActive(false);
            }
        }

        float _delay = 0;
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i] = QuestionObjs[CurrentQuestion].transform.GetChild(i).gameObject;
            iTween.ScaleFrom(CurrentQuestionItems[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));   
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        Is_OkButtonPressed = false;
        CurrentItem = Random.Range(0, 2);
        PlayQuestionVoiceOver(CurrentQuestion);
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i] != null)
            {
                CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = true;
            }
        }
    }

    int FindCount;
    public void FoundChange(GameObject _change)
    {
        _change.gameObject.transform.parent.GetComponent<Button>().enabled = false;
        FindCount++;
        PlayAudio(Sound_Ting, 0);
        iTween.ScaleTo(_change.gameObject, iTween.Hash("Scale", Vector3.one, "time", 1f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));

        if (FindCount >= QNoOfMarks[CurrentQuestion])
        {
            StartCoroutine(SetOk_Button(true, 0));
        }

        StopRepetedAudio();
        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption", 7);
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO[_Qi].EN_Sound_QO[Random.Range(0, 2)].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].EN_Sound_QO[Random.Range(0, 2)]);
                break;
            case 1:
                QVOLength = Sound_QVO[_Qi].HI_Sound_QO[Random.Range(0, 2)].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].HI_Sound_QO[Random.Range(0, 2)]);
                break;
            case 2:
                QVOLength = Sound_QVO[_Qi].TL_Sound_QO[Random.Range(0, 2)].clip.length;
                PlayAudioRepeated(Sound_QVO[_Qi].HI_Sound_QO[Random.Range(0, 2)]);
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
            if (CurrentQuestionItems[i].name== _Ansrindex.name)
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

    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;

        PlayAudio(Sound_BtnOkClick, 0);

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i].GetComponent<Image>().raycastTarget = false;
        }

        StopRepetedAudio();

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
        ProgreesBar.GetComponent<Slider>().value += 1;
        WrongAnsrsCount = 0;

        float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length) + Sound_BtnOkClick.clip.length;
        float LengthDelay2 = PlayAnswerVoiceOver(0, LengthDelay+0.25f);

        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2+0.5f);
            
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);
        StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2f));

        Invoke("HighlightCorrectOption", LengthDelay);

        if (QuestionOrder1 < (QuestionOrderList.Count) ||
            WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
        }
        else
        {
            Debug.Log("Questions Finished");
           // Invoke("ShowLC", LengthDelay + LengthDelay2 + 3f);
            SendResultFinal();
        }
        CancelInvoke("RepeatQVOAftertChoosingOption");
        
        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    void HighlightCorrectOption()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (!CurrentQuestionItems[i].name.Contains("Answer"))
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().Invoke("StartAnim", 0);
            }
            else
            {
                iTween.ScaleTo(CurrentQuestionItems[i].gameObject,
                        iTween.Hash("Scale", Vector3.zero, "time", 0.25f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
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

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO.EN_Sound_AO[_Ai], _delay);
        ClipLength = Sound_AVO.EN_Sound_AO[_Ai].clip.length;

        return ClipLength;
    }

    public void ShowLC()
    {
        LCObj.gameObject.SetActive(true);
    }

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;
    public AudioSource[] Sound_Intro5;

    public QVO_AudioSource_C09051[] Sound_QVO;
    public AVO_AudioSource_C09051 Sound_AVO;

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
    public class AVO_AudioSource_C09051
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_C09051
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }
}
