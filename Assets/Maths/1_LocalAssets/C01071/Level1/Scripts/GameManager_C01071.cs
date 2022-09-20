using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;


public class GameManager_C01071 : MonoBehaviour, IOAKSGame
{

    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject Tut_Items;
     public GameObject Tut_Numbers;
    public GameObject TutBtn_Okay;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject LevelsHolder;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;

    public GameObject[] LevelHolder;
    public GameObject[] Items1;

    public Image BG;
    public Sprite[] BG_Sprites;

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
            AddValueInProgress = 1 / (float)NoOfQuestionsToAsk;
            Thisgamekey = gameInputData.Key;
        }

        

        TutorialObj.gameObject.SetActive(true);
        LevelsHolder.gameObject.SetActive(false);
        PlayAudio(Sound_Intro1, 2f);
        float _delay = 0;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            iTween.MoveFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("y", 1500, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }
        for(int i=0;i<Tut_Numbers.transform.childCount;i++)
        {
            iTween.ScaleFrom(Tut_Numbers.transform.GetChild(i).gameObject,iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
             _delay += 0.1f;
        }
        Invoke("EnableAnimator", 2);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 2f);
        Invoke("CallIntro3", 22f);
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
    }    public void CallIntro3()
    {        
        Tut_Items.transform.GetChild(1).GetChild(0).GetComponent<Image>().raycastTarget = true;
    }

    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        Tut_Items.transform.GetChild(1).GetChild(0).GetComponent<Image>().raycastTarget = false;
        Tut_Items.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();               
        StopRepetedAudio();
        StopAudio(Sound_Intro2);
        StopAudio(Sound_Intro1);
        PlayAudioRepeated(Sound_Intro3);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        StopAudio(Sound_Intro3);
        PlayAudio(Sound_BtnOkClick, 0);
        TutBtn_Okay.gameObject.SetActive(false);       
        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);
        CurrentItem = 1;
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(1, LengthDelay);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);
        Tut_Items.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + LengthDelay2 + 1f);
        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 2f);
        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 3f);
    }

    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelsHolder.gameObject.SetActive(true);
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;
        }


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
        CurrentItems = new GameObject[Items1.Length];
        GenerateLevel();
    }
     int p=0;
    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;
        LevelHolder[0].transform.parent.gameObject.SetActive(false);      
        for (int i = 0; i < Items1.Length; i++)
        {
            if (CurrentItems[i] != null)
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
                if (CurrentItem == 0)
                {
                    CurrentItems[i].transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
                }
                else
                {
                    CurrentItems[i].transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
                }
            }
        }
        QuestionOrderListtemp = new List<int>();
        TutorialObj.gameObject.SetActive(false);
        if (QuestionOrder1 < (QuestionOrderList.Count))
        {
            for (int i = 10; i < QuestionsOrder1.Length; i++)
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
            CurrentItems = Items1;
        }


        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            for (int i = 10; i < QuestionsOrder1.Length; i++)
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
            CurrentItems = Items1;
        }

        BG.sprite = BG_Sprites[CurrentItem];
        LevelHolder[CurrentItem].transform.parent.gameObject.SetActive(true);
        float _delay = 0;

        Is_OkButtonPressed = false;
        if (p==0)
        {                
            iTween.ScaleFrom(LevelHolder[0].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
            Invoke("PlayQuestionVoiceOverWithDelay", 2.5f);
            p++;
        }   
        else
        {
            PlayQuestionVoiceOver(CurrentQuestion);            
            Invoke("EnableOptionsRaycast", QVOLength);
        }
    }
    void EnableOptionsRaycast()
    {
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems[i].transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
        }
    }

    public void PlayQuestionVoiceOverWithDelay()
    {
        PlayQuestionVoiceOver(CurrentQuestion);
        Invoke("EnableOptionsRaycast", QVOLength);
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

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (i == _Ansrindex)
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
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
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems[i].transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
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
            WrongAnsrsCount = 0;
            StopRepetedAudio();
            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length+0.25f) + Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay + 0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.7f);
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.7f);            
            StartCoroutine(SetActiveWithDelayCall(LevelHolder[CurrentItem].transform.parent.gameObject, false, LengthDelay + LengthDelay2 + 2f));   

            if (QuestionOrder1 < (QuestionOrderList.Count) || WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
              
                StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 3f));
               // Invoke("ShowLC", LengthDelay + LengthDelay2 + 3f);
                Debug.Log("Game Over C");
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
            iTween.ShakePosition(CurrentItems[UserAnsr].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 1);
                Invoke("HighlightCorrectOption", 1);

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList.Count))
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                     WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                 
                    StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2.5f));
                    //Invoke("ShowLC", LengthDelay + 2.5f);
                    Debug.Log("Game Over W");
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
                for (int i = 0; i < CurrentItems.Length; i++)
                {
                    CurrentItems[i].transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
                    CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
                    Debug.Log("...............");
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }
    void HighlightCorrectOption()
    {
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
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

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }
    public IEnumerator SetImageRaycastTarget(Image _targetImage, float _delay, bool _state)
    {
        yield return new WaitForSeconds(_delay);
        _targetImage.GetComponent<Image>().raycastTarget = _state;
    }
    public void ShowLC()
    {
        LCObj.gameObject.SetActive(true);
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

    public float PlayAnswerVoiceOver(int _Ai,float _delay)
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

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;

    public QO_AudioSource_C01071 Sound_QVO;
    public AO_AudioSource_C01071 Sound_AVO;

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

}


[System.Serializable]
public class AO_AudioSource_C01071
{
    public AudioSource[] EN_Sound_AVO;
    public AudioSource[] HI_Sound_AVO;
    public AudioSource[] TL_Sound_AVO;
}

[System.Serializable]
public class QO_AudioSource_C01071
{
    public AudioSource[] EN_Sound_QVO;
    public AudioSource[] HI_Sound_QVO;
    public AudioSource[] TL_Sound_QVO;
}