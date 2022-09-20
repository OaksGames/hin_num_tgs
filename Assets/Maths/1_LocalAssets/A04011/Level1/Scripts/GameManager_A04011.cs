using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_A04011 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutHand1, TutHand2;
    public GameObject TutBtn_Okay;
    public GameObject[] Tut_Trains;    
    bool[] Is_TutTrainWheelRot;
    public GameObject[] TutTrain1Wheels;
    public GameObject[] TutTrain2Wheels;
    public GameObject TutSignal;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject LevelsHolder;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;

    public GameObject LevelHolder;
    public GameObject[] Trains;
    public GameObject[] Train1Wheels;
    public GameObject[] Train2Wheels;
    public GameObject[] Items1;
    public GameObject[] Items2;

    public Sprite[] QuestionsSpritesSet1;
    public Sprite[] QuestionsSpritesSet2;

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
    public Sprite[] SignalsSprites;

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
            Is_TrainWheelRot = new bool[2];
            Is_TutTrainWheelRot = new bool[2];

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
        Is_TrainWheelRot = new bool[2];
        Is_TutTrainWheelRot = new bool[2];

        ProgreesBar.SetActive(false);
        btn_Back.SetActive(false);
        SetTutorial(data);
    }

    public void CleanUp()
    {
        // throw new System.NotImplementedException();
    }

    bool[] Is_TrainWheelRot;

    private void Update()
    {
        if (Is_TutTrainWheelRot[0])
        {
            for (int i = 0; i < Train1Wheels.Length; i++)
            {
                TutTrain1Wheels[i].transform.Rotate(new Vector3(0, 0, -180) * Time.deltaTime);
            }
        }

        if (Is_TutTrainWheelRot[1])
        {
            for (int i = 0; i < Train1Wheels.Length; i++)
            {
                TutTrain2Wheels[i].transform.Rotate(new Vector3(0, 0, -180) * Time.deltaTime);
            }
        }

        if (Is_TrainWheelRot[0])
        {
            for (int i = 0; i < Train1Wheels.Length; i++)
            {
                Train1Wheels[i].transform.Rotate(new Vector3(0, 0, -180) * Time.deltaTime);
            }
        }

        if (Is_TrainWheelRot[1])
        {
            for (int i = 0; i < Train1Wheels.Length; i++)
            {
                Train2Wheels[i].transform.Rotate(new Vector3(0, 0, -180) * Time.deltaTime);
            }
        }
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
        LevelsHolder.gameObject.SetActive(false);

        Is_TutTrainWheelRot[0] = true;
        Is_TutTrainWheelRot[1] = true;

        PlayAudio(Sound_TrainMovement,0);

        PlayAudio(Sound_Intro1, 6f);

        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 7f);

        iTween.MoveFrom(Tut_Trains[0].gameObject, iTween.Hash("x", -1000, "islocal", true, "time", 6f, "delay", 0, "easetype", iTween.EaseType.easeInOutSine));
        iTween.MoveFrom(Tut_Trains[1].gameObject, iTween.Hash("x", -1000, "islocal", true, "time", 6f, "delay", 0.25f, "easetype", iTween.EaseType.easeInOutSine));

        Invoke("StopTutTrainMove", 6.5f);
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

    void StopTutTrainMove()
    {
        StopAudio(Sound_TrainMovement);

        TutHand1.gameObject.SetActive(true);

        TutorialObj.GetComponent<Animator>().enabled = true;

        Is_TutTrainWheelRot[0] = false;
        Is_TutTrainWheelRot[1] = false;

        Tut_Trains[0].GetComponent<Animator>().enabled = false;
        Tut_Trains[1].GetComponent<Animator>().enabled = false;
    }
   

    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro2);
    }

    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);

        StopAudio(Sound_Intro2);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro3);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);

        TutSignal.GetComponent<Image>().raycastTarget = false;
        TutSignal.GetComponent<PopTweenCustom>().StartAnim();        
    }

    public void BtnAct_OkTut()
    {
        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);

        TutSignal.transform.GetChild(0).GetComponent<Image>().sprite = SignalsSprites[0];       

        TutBtn_Okay.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(false);        
       
        CurrentItem = 1;

        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(1, LengthDelay);

        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2);

        Invoke("Tut_TrainOut", LengthDelay);

        Invoke("StopTrainMovementSound", LengthDelay + LengthDelay2 + 2f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2);

        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 2f);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 3f);
    }

    void Tut_TrainOut()
    {
        PlayAudio(Sound_TrainMovement, 0);
        iTween.MoveTo(Tut_Trains[0].gameObject, iTween.Hash("x", 1000, "islocal", true, "time", 6f, "delay", 0, "easetype", iTween.EaseType.easeInOutSine));
        Tut_Trains[0].GetComponent<Animator>().enabled = true;
        Is_TutTrainWheelRot[0] = true;
    }

    void StopTrainMovementSound()
    {
        StopAudio(Sound_TrainMovement);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        
        LevelsHolder.gameObject.SetActive(true);
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;
            Debug.Log("NoOfQuestionsToAsk: " + NoOfQuestionsToAsk);
        }
        

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        StopTrainMovementSound();
        WrongAnsrsCount = 0;
        TutorialObj.gameObject.SetActive(false);

        Trains[0].transform.localPosition = new Vector3(-1000, Trains[0].transform.localPosition.y, 0);
        Trains[1].transform.localPosition = new Vector3(-1000, Trains[1].transform.localPosition.y, 0);

        CurrentItems[0].transform.GetChild(0).GetComponent<Image>().sprite = SignalsSprites[2];
        CurrentItems[1].transform.GetChild(0).GetComponent<Image>().sprite = SignalsSprites[2];


        for (int i = 0; i < Items1.Length; i++)
        {
            if (Items1[i] != null)
            {
                Items1[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
        for (int i = 0; i < Items2.Length; i++)
        {
            if (Items2[i] != null)
            {
                Items2[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;

        LevelHolder.transform.gameObject.SetActive(false);

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (CurrentItems[i] != null)
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
                CurrentItems[i].GetComponent<Image>().raycastTarget = false;
            }
        }

        QuestionOrderListtemp = new List<int>();

        CurrentItem = Random.Range(0, 2);
        CorrectAnsrIndex = CurrentItem;

        if (QuestionOrder1 < (QuestionOrderList.Count))
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
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
                Debug.Log("Here");
            }
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }

        LevelHolder.transform.gameObject.SetActive(true);
        iTween.MoveTo(Trains[0].gameObject, iTween.Hash("x", 0, "islocal",true,"time", 6f, "delay", 0, "easetype", iTween.EaseType.easeInOutSine));
        iTween.MoveTo(Trains[1].gameObject, iTween.Hash("x", 0, "islocal", true,"time", 6f, "delay", 0.25f, "easetype", iTween.EaseType.easeInOutSine));
        Trains[0].GetComponent<Animator>().enabled = true;
        Trains[1].GetComponent<Animator>().enabled = true;
        Is_TrainWheelRot[0] = true;
        Is_TrainWheelRot[1] = true;
        PlayAudio(Sound_TrainMovement, 0);


        int _randtemp = RandomNoFromList_Int(new int[] { 3, 4, 5});
        float _delay = 0;
        for (int i = 0; i < Items1.Length; i++)
        {            
            if (CorrectAnsrIndex == 1)
            {
                if (i % 2 == 0)
                {
                    Items2[i].GetComponent<Image>().sprite = QuestionsSpritesSet2[CurrentQuestion];
                }
                else
                {
                    Items2[i].GetComponent<Image>().sprite = QuestionsSpritesSet1[CurrentQuestion];
                }

                if (i % _randtemp == 0)
                {
                    Items1[i].GetComponent<Image>().sprite = QuestionsSpritesSet1[CurrentQuestion];
                }
                else
                    Items1[i].GetComponent<Image>().sprite = QuestionsSpritesSet2[CurrentQuestion];
            }
            else
            if (CorrectAnsrIndex == 0)
            {
                if (i % 2 == 0)
                {
                    Items1[i].GetComponent<Image>().sprite = QuestionsSpritesSet2[CurrentQuestion];
                }
                else
                {
                    Items1[i].GetComponent<Image>().sprite = QuestionsSpritesSet1[CurrentQuestion];
                }

                if (i % _randtemp == 0)
                {
                    Items2[i].GetComponent<Image>().sprite = QuestionsSpritesSet1[CurrentQuestion];
                }
                else
                    Items2[i].GetComponent<Image>().sprite = QuestionsSpritesSet2[CurrentQuestion];
            }

            _delay += 0.1f;
        }
        Is_OkButtonPressed = false;
        Invoke("PlayQuestionVoiceOverWithDelay", 6f);
    }   

    public void PlayQuestionVoiceOverWithDelay()
    {
        PlayQuestionVoiceOver(CorrectAnsrIndex);
        Invoke("EnableOptionsRaycast", QVOLength);
        AutoRotate.Is_Rot = false;
        Trains[0].GetComponent<Animator>().enabled = false;
        Trains[1].GetComponent<Animator>().enabled = false;
        Is_TrainWheelRot[0] = false;
        Is_TrainWheelRot[1] = false;
        StopAudio(Sound_TrainMovement);
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems[i].GetComponent<Image>().raycastTarget = true;
        }
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
        ///UserAnsr = _Ansrindex;
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

        StopRepetedAudio();

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems[i].GetComponent<Image>().raycastTarget = false;
        }

        if (UserAnsr== CorrectAnsrIndex)
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
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay + 0.25f);

            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);

            StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay+ LengthDelay2 + 6.25f));

            if (QuestionOrder1 < (QuestionOrderList.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 6.5f);
            }
            else
            {
                Debug.Log("Game Over C");
                //Invoke("ShowLC", LengthDelay + LengthDelay2 + 3);
                SendResultFinal();
            }

            CurrentItems[UserAnsr].transform.GetChild(0).GetComponent<Image>().sprite = SignalsSprites[0];
            iTween.MoveTo(Trains[UserAnsr].gameObject, iTween.Hash("x", 1000, "islocal", true, "time", 6f, "delay", LengthDelay+ LengthDelay2, "easetype", iTween.EaseType.easeInOutSine));
            Invoke("SetTrainWheelsRot", LengthDelay+ LengthDelay2);

            HighlighCorrectOption();
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

            CurrentItems[UserAnsr].transform.GetChild(0).GetComponent<Image>().sprite = SignalsSprites[1];

            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, 1f);

                Invoke("ClearSignle", 1);
                Invoke("HighlighCorrectOption", 1);

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
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Game Over W");
                    StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2f));
                    // Invoke("ShowLC", LengthDelay + 2.5f);
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
                Invoke("ClearSignle", 1);
                for (int i = 0; i < CurrentItems.Length; i++)
                {
                    CurrentItems[i].GetComponent<Image>().raycastTarget = true;
                    CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    void ClearSignle()
    {
        CurrentItems[0].transform.GetChild(0).GetComponent<Image>().sprite = SignalsSprites[2];
        CurrentItems[1].transform.GetChild(0).GetComponent<Image>().sprite = SignalsSprites[2];
    }

    void HighlighCorrectOption()
    {
        if (CorrectAnsrIndex == 0)
        {
            for (int i = 0; i < Items1.Length; i++)
            {
                if (Items1[i] != null)
                {
                    Items1[i].GetComponent<PopTweenCustom>().StartAnim();
                }
            }
        }
        else
        {
            for (int i = 0; i < Items2.Length; i++)
            {
                if (Items2[i] != null)
                {
                    Items2[i].GetComponent<PopTweenCustom>().StartAnim();
                }
            }
        }
    }

    void SetTrainWheelsRot()
    {
        PlayAudio(Sound_TrainMovement,0);
        Is_TrainWheelRot[UserAnsr] = true;
        Trains[UserAnsr].GetComponent<Animator>().enabled = true;
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
        LevelsHolder.gameObject.SetActive(false);
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO.EN_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.EN_Sound_QO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO.HI_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO.TL_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QO[_Qi]);
                break;
        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
     if (VOLanguage == 0)
      {
        PlayAudio(Sound_AVO.EN_Sound_AO[_Ai], _delay);
        ClipLength = Sound_AVO.EN_Sound_AO[_Ai].clip.length;
      }
      if (VOLanguage == 1)
      {
        PlayAudio(Sound_AVO.HI_Sound_AO[_Ai], _delay);
        ClipLength = Sound_AVO.HI_Sound_AO[_Ai].clip.length;
      }
      if (VOLanguage == 2)
      {
        PlayAudio(Sound_AVO.TL_Sound_AO[_Ai], _delay);
        ClipLength = Sound_AVO.TL_Sound_AO[_Ai].clip.length;
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

    public QVO_AudioSource_A04011 Sound_QVO;

    public AVO_AudioSource_A04011 Sound_AVO;

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

    public AudioSource Sound_TrainMovement;

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

    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
        }

    }
    [System.Serializable]
    public class AVO_AudioSource_A04011
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_A04011
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }

}

