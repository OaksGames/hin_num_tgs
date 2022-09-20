using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B08032 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject[] Tut_Items;
    public GameObject Tut_Items1;
    public GameObject Tut_Wardrob;
    public GameObject TutHand1, TutHand2,TutHand3;
    public GameObject Tut_SeaSaw_MidRod;
    public GameObject Tut_SeaSaw_Tray_L;
    public GameObject Tut_SeaSaw_Tray_R;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;

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

    public Sprite[] Items_Baskets;

    public Sprite[] Items0;
    public Sprite[] Items1;

    public GameObject SeaSaw_MidRod;
    public GameObject SeaSaw_Tray_L;
    public GameObject SeaSaw_Tray_R;
    public GameObject[] BasketItemsHolder;

    public int[] ItemWeights;

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

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);

        PlayAudio(Sound_Intro1, 2f);
        
        float _delay = 0.25f;
      
        for (int i = 0; i < Tut_Items.Length; i++)
        {
            iTween.ScaleFrom(Tut_Items[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.5f;
        }

        iTween.RotateTo(Tut_SeaSaw_MidRod.gameObject, iTween.Hash("z", -15, "time", 0.15f, "delay", 0.5f, "easetype", iTween.EaseType.easeOutCirc));

        Invoke("EnableAnimator", 3.5f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 3f);
        
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
    public void EnableTutRaycast()
    {
        Tut_Wardrob.transform.GetChild(1).gameObject.GetComponent<Image>().raycastTarget = true;
    }
    public void DisableTutRaycast()
    {
        Tut_Wardrob.transform.GetChild(1).gameObject.GetComponent<Image>().raycastTarget = false;
    }
    public void CallIntro2()
    {
        PlayAudioRepeated(Sound_Intro2);
        Invoke("EnableTutRaycast", Sound_Intro2[VOLanguage].clip.length + 0.5f);
    }
    public void CallIntro3()
    {
        TutHand1.gameObject.SetActive(false);
        StopRepetedAudio();
        StopAudio(Sound_Intro2);
        PlayAudio(Sound_Intro3,0.2f);
        Invoke("DisableTutRaycast",  0.5f);
        Invoke("CallIntro4", Sound_Intro3[VOLanguage].clip.length +2f);
    }
    public void CallIntro4()
    {
        StopAudio(Sound_Intro3);
        PlayAudioRepeated(Sound_Intro4);
        Invoke("EnableTutRaycast", Sound_Intro4[VOLanguage].clip.length + 0.5f);
    }

    bool _IsDone;
    public void AddTut_Item()
    {
        TutHand1.gameObject.SetActive(false);
        if (_IsDone==false)
        {
            _IsDone = true;
            CallIntro3();
        }

        Tut_Set_SeaSaw();
      
    }

    int tempChildCount = 1;
    float _MaxAngle = 15;
    public void Tut_Set_SeaSaw()
    {
        if (tempChildCount < 6)
        {
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0));
            iTween.ScaleTo(Tut_Items1.transform.GetChild(tempChildCount).gameObject, iTween.Hash("Scale", Vector3.one, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
            tempChildCount += 1;
            
            float _TargetAngle = (Tut_SeaSaw_MidRod.transform.localEulerAngles.z - 360) + ((_MaxAngle / 5) * 1);
            Mathf.Clamp(_TargetAngle, -_MaxAngle, _MaxAngle);
            iTween.RotateTo(Tut_SeaSaw_MidRod.gameObject, iTween.Hash("z", _TargetAngle, "time", 0.15f, "delay", 0, "easetype", iTween.EaseType.easeOutCirc));

            Debug.Log("ANGLE : " + _TargetAngle + " " + (Tut_SeaSaw_MidRod.transform.localEulerAngles.z - 360));
            if (tempChildCount == 6)
            {
                StopAudio(Sound_Intro3);
                TutHand3.gameObject.SetActive(true);
                StopAudio(Sound_Intro4);
                StopRepetedAudio();
                PlayAudio(Sound_Intro5,0.5f);
                Invoke("CallIntro6", Sound_Intro5[VOLanguage].clip.length + 1.5f);
                //TutHand1.gameObject.SetActive(false);
                
            }
            
        }
    }

    //public void SeaSaw_Tray()
    //{
    //    Tut_SeaSaw_Tray_L.gameObject.transform.localEulerAngles = new Vector3(0, 0, -(Tut_SeaSaw_MidRod.transform.localEulerAngles.z - 360));
    //    Tut_SeaSaw_Tray_R.gameObject.transform.localEulerAngles = new Vector3(0, 0, -(Tut_SeaSaw_MidRod.transform.localEulerAngles.z - 360));
    //}
    public void CallIntro6()
    {
        TutHand3.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
        StopAudio(Sound_Intro5);
        PlayAudio(Sound_Intro6, 0.5f);
        TutBtn_Okay.gameObject.SetActive(true);

    }
    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);

        Tut_Items1.transform.GetChild(0).gameObject.GetComponent<Image>().raycastTarget = false;
        Tut_Items1.transform.GetChild(0).GetComponent<PopTweenCustom>().StartAnim();

        StopAudio(Sound_Intro4);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro5);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        PlayAudio(Sound_BtnOkClick, 0);
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro6);
        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);

        CurrentItem = 0;
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(8, LengthDelay+0.25f);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);
        Tut_Items1.transform.GetChild(0).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + LengthDelay2 + 1f);

        PlayAudio(Sound_Intro7, LengthDelay + LengthDelay2 + 2f);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro7[VOLanguage].clip.length + 3f);
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
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk+1;
        }

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));

        CurrentItems = new GameObject[2];

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 2);
        int tempq = 0;

        LevelsHolder.gameObject.SetActive(false);
        //QuestionsHolder[CurrentItem].gameObject.SetActive(false);        

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

        LevelsHolder.gameObject.SetActive(true);

        //QuestionsHolder[CurrentItem].gameObject.SetActive(true);

        CurrentItems[0] = SeaSaw_Tray_L.transform.GetChild(0).gameObject;
        CurrentItems[1] = SeaSaw_Tray_R.transform.GetChild(0).GetChild(0).gameObject;

        _tempChildCount = 0;
        for (int i = 0; i < CurrentItems[0].transform.childCount; i++)
        {
            CurrentItems[0].transform.GetChild(i).gameObject.transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < BasketItemsHolder.Length; i++)
        {
            BasketItemsHolder[i].GetComponent<Button>().enabled = false;
        }

        //int _CurrentBasket = Random.Range(0, BasketItemsHolder.Length);
        int _CurrentBasket = 0;
        BasketItemsHolder[_CurrentBasket].GetComponent<Image>().sprite = Items_Baskets[CurrentQuestion-1];
        CurrentItems[1].GetComponent<Image>().sprite = Items0[CurrentQuestion - 1];
        //BasketItemsHolder[_CurrentBasket].GetComponent<Button>().enabled = true;

        int _ii = Random.Range(0, 2);
        BasketItemsHolder[_CurrentBasket].transform.parent.gameObject.GetComponent<GridLayoutGroup>().startAxis = (GridLayoutGroup.Axis)_ii;

        int _jj = Random.Range(0, 5);
        BasketItemsHolder[_CurrentBasket].transform.parent.gameObject.GetComponent<GridLayoutGroup>().startCorner = (GridLayoutGroup.Corner)_jj;

        float _delay = 0;
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            iTween.ScaleFrom(CurrentItems[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        iTween.RotateTo(SeaSaw_MidRod.gameObject, iTween.Hash("z", -15, "time", 0.15f, "delay", 0.5f, "easetype", iTween.EaseType.easeOutCirc));

        Is_OkButtonPressed = false;
        int _inttmep = Random.Range(0, 2);

        CorrectAnsrIndex = ItemWeights[CurrentQuestion-1];
        
        PlayQuestionVoiceOver(CurrentQuestion-1);

        Debug.Log("CorrectAnsrIndex : " + CorrectAnsrIndex);
        Invoke("EnableOptionsRaycast", QVOLength+0.1f);
    }

    int _inttempq;
    void EnableOptionsRaycast()
    {
        int _CurrentBasket = 0;
        BasketItemsHolder[_CurrentBasket].GetComponent<Button>().enabled = true;
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems[i].GetComponent<Image>().raycastTarget = true;
        }
    }

   
    public void Add_Item()
    {
        StartCoroutine(SetOk_Button(true, 0));

        Set_SeaSaw();
    }

    int _tempChildCount = 0;
    float MaxAngle = 15;
    public void Set_SeaSaw()
    {
        if (_tempChildCount < ItemWeights[CurrentQuestion-1])
        {
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 0));
            iTween.ScaleTo(CurrentItems[0].transform.GetChild(_tempChildCount).gameObject, iTween.Hash("Scale", Vector3.one, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
            CurrentItems[0].transform.GetChild(_tempChildCount).GetComponent<Image>().sprite = Items1[CurrentQuestion - 1];
            _tempChildCount += 1;
            UserAnsr = _tempChildCount;            
        

            float _TargetAngle = (SeaSaw_MidRod.transform.localEulerAngles.z - 360)+((MaxAngle/ItemWeights[CurrentQuestion-1])*1);
            Mathf.Clamp(_TargetAngle, -MaxAngle, MaxAngle);
            iTween.RotateTo(SeaSaw_MidRod.gameObject, iTween.Hash("z", _TargetAngle, "time", 0.15f, "delay", 0, "easetype", iTween.EaseType.easeOutCirc));
        
            Debug.Log("ANGLE : " + _TargetAngle+" "+ (SeaSaw_MidRod.transform.localEulerAngles.z-360));
        }
    }

    private void Update()
    {
        SeaSaw_Tray_L.gameObject.transform.localEulerAngles = new Vector3(0, 0, -(SeaSaw_MidRod.transform.localEulerAngles.z - 360));
        SeaSaw_Tray_R.gameObject.transform.localEulerAngles = new Vector3(0, 0, -(SeaSaw_MidRod.transform.localEulerAngles.z - 360));
        Tut_SeaSaw_Tray_L.gameObject.transform.localEulerAngles = new Vector3(0, 0, -(Tut_SeaSaw_MidRod.transform.localEulerAngles.z - 360));
        Tut_SeaSaw_Tray_R.gameObject.transform.localEulerAngles = new Vector3(0, 0, -(Tut_SeaSaw_MidRod.transform.localEulerAngles.z - 360));
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO.EN_Sound_QVO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO.EN_Sound_QVO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO.HI_Sound_QVO[_Qi - 1].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QVO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO.TL_Sound_QVO[_Qi - 1].clip.length;
                PlayAudioRepeated(Sound_QVO.HI_Sound_QVO[_Qi]);
                break;
        }
        
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO.EN_Sound_AVO[_Ai], _delay);
        ClipLength = Sound_AVO.EN_Sound_AVO[_Ai].clip.length;

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
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (CurrentItems[i].name.Contains(UserAnsr.ToString()))
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
    int _CurrentBasket = 0;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;

        PlayAudio(Sound_BtnOkClick, 0);

       
        BasketItemsHolder[_CurrentBasket].GetComponent<Button>().enabled = false;

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems[i].GetComponent<Image>().raycastTarget = false;
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

            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length) + 0.5f;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion-1, LengthDelay +1.2f);

            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 1f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 1f);

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
            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Wrong);
                Debug.Log("Add : " + AddKey + ": Wrong");
            }

            Debug.Log("WRONG ANSWER : " + UserAnsr);

            for (int i = 0; i < CurrentItems.Length; i++)
            {
                if (CurrentItems[i].name.Contains(UserAnsr.ToString()))
                {
                    iTween.ShakePosition(CurrentItems[i].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                }
            }

            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion-1, 1f);

                Invoke("HighlightCorrectOption", 1f);

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
                for (int i = 0; i < CurrentItems.Length; i++)
                {
                    CurrentItems[i].GetComponent<Image>().raycastTarget = true;
                    //CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
                }
                
                BasketItemsHolder[_CurrentBasket].GetComponent<Button>().enabled = true;
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }
    
    void HighlightCorrectOption()
    {
        float _delayans = 0;
        if (UserAnsr < ItemWeights[CurrentQuestion - 1])
        {
            for (int i = 0; i < ItemWeights[CurrentQuestion - 1] - UserAnsr; i++)
            {
                Invoke("Set_SeaSaw", _delayans);
                _delayans += 0.1f;
            }
        }
        else
        {
            for (int i = ItemWeights[CurrentQuestion - 1]; i<UserAnsr; i++)
            {
                CurrentItems[0].transform.GetChild(i).gameObject.transform.localScale = Vector3.zero;
            }
        }

        iTween.RotateTo(SeaSaw_MidRod.gameObject, iTween.Hash("z", 0, "time", 0.15f, "delay", 0, "easetype", iTween.EaseType.easeOutCirc));
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
    public AudioSource[] Sound_Intro5;
    public AudioSource[] Sound_Intro6;
    public AudioSource[] Sound_Intro7;

    public QO_AudioSource_B08032 Sound_QVO;
    public AO_AudioSource_B08032 Sound_AVO;

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
}

[System.Serializable]
public class AO_AudioSource_B08032
{
    public AudioSource[] EN_Sound_AVO;
    public AudioSource[] HI_Sound_AVO;
    public AudioSource[] TL_Sound_AVO;
}

[System.Serializable]
public class QO_AudioSource_B08032
{
    public AudioSource[] EN_Sound_QVO;
    public AudioSource[] HI_Sound_QVO;
    public AudioSource[] TL_Sound_QVO;
}
