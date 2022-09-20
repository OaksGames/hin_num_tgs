using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_C11071 : MonoBehaviour
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject []TutNumberObjs;
    public GameObject TutNumObjs;
    public GameObject TutObj;
    public GameObject TutBtn_Okay;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public GameObject LevelObj;
    public GameObject LevelHolder;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;

    public Image[] NumberObjs;
    public Sprite[] NumSprites;

    [HideInInspector]
    public Sprite[] RandNumSprites;

    [HideInInspector]
    public List<int> QuestionOrderList;

    [HideInInspector]
    public bool Is_CanClick;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;

    int WrongAnsrsCount;

    void Start()
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

    #region TUTORIAL

    public void SetTutorial()
    {
        TutorialObj.gameObject.SetActive(true);
        LevelObj.gameObject.SetActive(false);

        PlayAudio(Sound_Intro1, 2f);

        float _delay = 0;
        for (int i = 0; i < TutNumberObjs.Length; i++)
        {
            iTween.ScaleTo(TutNumberObjs[i].gameObject, iTween.Hash("Scale", Vector3.one, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        Invoke("EnableAnimator", 3f);
        Invoke("TutNumberObjsoff", Sound_Intro1[VOLanguage].clip.length+2f);
        //Invoke("Tutobjon", Sound_Intro1[VOLanguage].clip.length + 2f);
        // Invoke("EnableAnimator", 13.5f);
    }

    public void EnableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = true;
    }

    public void DisableAnimator()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
    }
    
    public void TutNumberObjsoff()
    {
        
        for (int i = 0; i < TutNumberObjs.Length; i++)
        {
            TutNumberObjs[i].SetActive(false);
        }
        Invoke("Tutobjon", 0f);
    }
    public void Tutobjon()
    {
        TutNumObjs.gameObject.SetActive(false);
        TutObj.gameObject.SetActive(true);
        PlayAudioRepeated(Sound_Intro2);
        Invoke("EnableTutNoTRaycatTarget", +2.5f);
    }
    public void EnableTutNoTRaycatTarget()
    {
        TutObj.transform.GetChild(2).GetComponent<Image>().raycastTarget = true;
    }

    
    public void Selected_TutNumber()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutObj.transform.GetChild(2).GetComponent<PopTweenCustom>().StartAnim();
        TutObj.transform.GetChild(2).GetComponent<Image>().raycastTarget = false;
        TutBtn_Okay.gameObject.SetActive(true);

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

        float LengthDelay = PlayAppreciationVoiceOver(0);
        
        PlayAudio(Sound_CorrectAnswer, LengthDelay+2.5f );
        PlayAudio(Sound_Intro4, LengthDelay + 0.5f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay +2.5f);

        Invoke("SetGamePlay", LengthDelay + 4.5f);
    }

    #endregion 

    public void SetGamePlay()
    {
        TutorialObj.gameObject.SetActive(false);

        ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length;


        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        RandNumSprites = new Sprite[NumSprites.Length];

        QuestionOrderList = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            RandNumSprites[i] = NumSprites[QuestionsOrder1[i]-1];
        }

        StartCoroutine(SetOk_Button(false, 0f));

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        LevelObj.gameObject.SetActive(true);
        LevelHolder.gameObject.SetActive(true);
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            NumberObjs[i].transform.localScale = Vector3.zero;
            NumberObjs[i].transform.GetComponent<PopTweenCustom>().StopAnim();
        }

        int RandAnsrIndex = Random.Range(0, 4);
        int tempq = 0;

        QuestionOrderListtemp = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            Debug.Log("Here");
        }

        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {
            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[tempq]);
            WrongAnsweredQuestionOrder1++;
        }

        float _delay = 0;
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                NumberObjs[i].sprite = NumSprites[CurrentQuestion-1];
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    NumberObjs[i].sprite = NumSprites[_ixx-1];
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    NumberObjs[i].sprite = NumSprites[_iyy-1];
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    NumberObjs[i].sprite = NumSprites[_izz-1];
                    QuestionOrderListtemp.Remove(_izz);
                }
                else
                if (i == 3)
                {
                    int _iww = RandomNoFromList_Int(QuestionOrderListtemp);
                    NumberObjs[i].sprite = NumSprites[_iww-1];
                    QuestionOrderListtemp.Remove(_iww);
                }
                
            }

            iTween.ScaleFrom(NumberObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        Is_OkButtonPressed = false;

        PlayQuestionVoiceOver(CurrentQuestion-1);
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            NumberObjs[i].GetComponent<Image>().raycastTarget = true;
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
        }
        return ClipLength;
    }

    int UserAnsr;
    public void Check_Answer(int _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            if (i == _Ansrindex)
            {
                NumberObjs[i].transform.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                NumberObjs[i].transform.GetComponent<PopTweenCustom>().StopAnim();
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

    void HighlightOptions()
    {
        for (int i = 0; i < NumberObjs.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                NumberObjs[i].transform.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                NumberObjs[i].transform.GetComponent<PopTweenCustom>().StopAnim();
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

        for (int i = 0; i < NumberObjs.Length; i++)
        {
            NumberObjs[i].GetComponent<Image>().raycastTarget = false;
        }

        StopRepetedAudio();

        if (CorrectAnsrIndex == UserAnsr)
        {
            Debug.Log("CORRECT ANSWER");
            ProgreesBar.GetComponent<Slider>().value += 1;
            Total_CorrectAnswers++;//INGAME_COMMON
            WrongAnsrsCount = 0;

            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length) + Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion-1, LengthDelay + 0.25f);

            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);

            StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + LengthDelay2 + 2));

            if (QuestionOrder1 < (QuestionsOrder1.Length))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                Debug.Log("Questions Finished");
                Invoke("ShowLC", LengthDelay + LengthDelay2 + 2.5f);
            }
            CancelInvoke("RepeatQVOAftertChoosingOption");
        }
        else
        {
            Debug.Log("WRONG ANSWER : ");
            iTween.ShakePosition(NumberObjs[UserAnsr].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;

            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion-1, 1f);

                Invoke("HighlightOptions", 1);

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion))
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                {
                    ProgreesBar.GetComponent<Slider>().value += 1;
                }

                StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + 2f));

                if (QuestionOrder1 < (QuestionsOrder1.Length))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Questions Finished");
                    StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + 2.25f));
                    Invoke("ShowLC", LengthDelay + 2.75f);
                }
                CancelInvoke("RepeatQVOAftertChoosingOption");
                WrongAnsrsCount = 0;
            }
            else
            {
                Is_OkButtonPressed = false;
                //StartCoroutine("PlayAudioRepeatedCall");
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("RepeatQVOAftertChoosingOption", 1);
                for (int i = 0; i < NumberObjs.Length; i++)
                {
                    NumberObjs[i].GetComponent<Image>().raycastTarget = true;
                }
            }

            for (int i = 0; i < NumberObjs.Length; i++)
            {
                NumberObjs[i].transform.GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        Is_CanClick = _IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);
    }

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }

    #region INGAME_COMMON

    public int Total_CorrectAnswers;
    public void ShowLC()
    {
        InGameManager.instance.Activity_Finished(QuestionsOrder1.Length, Total_CorrectAnswers);
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

    public AudioSource[] Sound_Q0;
    public AudioSource[] Sound_Q1;
    public AudioSource[] Sound_Q2;
    public AudioSource[] Sound_Q3;
    public AudioSource[] Sound_Q4;
    public AudioSource[] Sound_Q5;
    
    public AudioSource[] Sound_A0;
    public AudioSource[] Sound_A1;
    public AudioSource[] Sound_A2;
    public AudioSource[] Sound_A3;
    public AudioSource[] Sound_A4;
    public AudioSource[] Sound_A5;
    
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
}
