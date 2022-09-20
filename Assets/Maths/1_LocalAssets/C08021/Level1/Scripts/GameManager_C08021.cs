using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

[System.Serializable]
public class Question_Sprites
{
    public Sprite[] Question_Items;
}

public class GameManager_C08021 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject TutHand1, TutHand2;
    public GameObject Tut_Board;
    public GameObject Tut_HangingBoard;
    public GameObject Tut_QuestionItemsLeft;
    public GameObject Tut_QuestionItemsRight;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsType1Ask;
    public int NoOfQuestionsType2Ask;

    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject Hanging_Board;
    public GameObject Selected_Option;
    public GameObject[] QuestionsHolderLeft;
    public GameObject[] QuestionsHolderRight;
    public GameObject QuestionItem;

    [HideInInspector]
    public bool Is_CanClick;

    public GameObject LeftHandSide_Numbers;
    public GameObject RightHandSide_Numbers;
    public GameObject OptionObj;
    public GameObject[] CurrentQuestionItems;
    public int[] ItemsOrder;
    public List<int> RandomOption;

   // [HideInInspector]
    public List<int> QuestionOrderList;
    public List<int> QuestionOrderList1;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public int[] QuestionsOrder2;
    public int QuestionOrder2;
    public List<int> WrongAnsweredQuestions2;
    public int WrongAnsweredQuestionOrder2;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;

    public int CurrentQuestion;
    int WrongAnsrsCount;
    int CurrentItem;

    public GameObject[] CurrentItemsLeft;
    public GameObject[] CurrentItemsRight;

    public int[] Left_Numbers1;
    public int[] Left_Numbers2;
    public int[] Right_Numbers1;
    public int[] Right_Numbers2;

    public float LeftNum_Digit;
    public float RightNum_Digit;

    float AddValueInProgress = 0;
    float ValueAdd;
    public GameInputData gameInputData;
    public int TotalQues;
    public string Thisgamekey;
    public int[] Ques_1;
    public GameObject btn_Back;

    public Question_Sprites[] QuestionsItems;

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
    public void SetTutorial(GameInputData gameInputDat)
    {
        if (FrameworkOff == false && Testing == false)
        {
            this.gameInputData = gameInputData;
            ////////////////What the value should add in progress bar aftereach question//////////////////
            TotalQues = gameInputData.Mechanics.Count;
            //***************************************************
            AddValueInProgress = 1 / (float)(NoOfQuestionsType1Ask + NoOfQuestionsType2Ask);
            Thisgamekey = gameInputData.Key;
        }

        

        TutorialObj.gameObject.SetActive(true);

        float delay = 0.5f;
        for(int i=0;i< Tut_QuestionItemsLeft.transform.childCount;i++)
        {
            iTween.ScaleFrom(Tut_QuestionItemsLeft.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, delay));
            delay += 0.1f;
        }
        for (int i = 0; i < Tut_QuestionItemsRight.transform.childCount; i++)
        {
            iTween.ScaleFrom(Tut_QuestionItemsRight.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, delay));
            delay += 0.1f;
        }

        PlayAudio(Sound_Intro1, 3f);
        LevelsHolder.gameObject.SetActive(false);

        float _delay = 0;
        for (int i = 0; i < Tut_Items.transform.childCount; i++)
        {
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+delay+2.5f));
            iTween.ScaleFrom(Tut_Items.transform.GetChild(i).gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay+ delay + 2.5f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+delay+2.5f));
            _delay += 0.1f;
        }

        iTween.MoveTo(Tut_HangingBoard.gameObject, iTween.Hash("y", 208, "time", 0.25f, "delay", 3.25f, "islocal", true, "easetype", iTween.EaseType.linear));
        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length+ 3.5f);
        PlayAudio(Sound_Intro3, Sound_Intro1[VOLanguage].clip.length+Sound_Intro2[VOLanguage].clip.length + 4f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length+ Sound_Intro3[VOLanguage].clip.length + 3.5f);
        Invoke("EnableAnimator", 3);
        Invoke("EnableTutNoTRaycatTarget", Sound_Intro1[VOLanguage].clip.length+Sound_Intro2[VOLanguage].clip.length+ Sound_Intro3[VOLanguage].clip.length+5.5f);
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
        Tut_Items.transform.GetChild(0).GetComponent<Text>().raycastTarget = true;
    }

    public void CallIntro2()
    {
        
        PlayAudioRepeated(Sound_Intro4);
    }

    public void Selected_TutAnswer()
    {
        StopRepetedAudio();
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_Items.transform.GetChild(0).GetComponent<Text>().raycastTarget = false;
        Tut_Items.transform.GetChild(0).GetComponent<PopTweenCustom>().StartAnim();
        Tut_Board.transform.GetChild(0).GetComponent<Text>().text = "<";
        PlayAudioRepeated(Sound_Intro5);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        Tut_Board.transform.GetChild(0).GetComponent<PopTweenCustom>().StartAnim();
        Tut_Items.transform.GetChild(0).GetComponent<PopTweenCustom>().StopAnim();
        TutBtn_Okay.gameObject.SetActive(false);
        StopRepetedAudio();
        PlayAudio(Sound_Btn_Ok_Click, 0);
        TutHand2.gameObject.SetActive(false);
        float LengthDelay = PlayAppreciationVoiceOver(0.5f);
        PlayAudio(Sound_Intro6, LengthDelay+0.5f);
        iTween.MoveTo(Tut_HangingBoard.gameObject, iTween.Hash("y", 460, "time", 0.25f, "delay", LengthDelay+Sound_Intro6[VOLanguage].clip.length + 0.25f, "islocal", true, "easetype", iTween.EaseType.linear));
        PlayAudio(Sound_CorrectAnswer, Sound_Intro6[VOLanguage].clip.length+LengthDelay + 0.5f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + Sound_Intro6[VOLanguage].clip.length + 0.5f);
        PlayAudio(Sound_Intro7, LengthDelay+ Sound_Intro6[VOLanguage].clip.length+2.5f);
        Invoke("SetGamePlay", LengthDelay + Sound_Intro6[VOLanguage].clip.length+ Sound_Intro7[VOLanguage].clip.length + 3f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        LevelsHolder.gameObject.SetActive(true);
        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsType1Ask + NoOfQuestionsType2Ask;
        }

        if (Is_NeedRandomizedQuestions)
        {
            QuestionsOrder1 = RandomArray_Int(QuestionsOrder1);
            QuestionsOrder2 = RandomArray_Int(QuestionsOrder2);
        }

        QuestionOrderList = new List<int>();
        QuestionOrderList1 = new List<int>();

        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < NoOfQuestionsType1Ask; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            //------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i].ToString();
            QuesKeys.Add(AddKey);
        }
        for (int i = 0; i < NoOfQuestionsType2Ask; i++)
        {
            QuestionOrderList1.Add(QuestionsOrder2[i]);
            //------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + (QuestionOrderList1[i] + QuestionOrderList.Count).ToString();
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

        TutorialObj.gameObject.SetActive(false);
        Selected_Option.GetComponent<Text>().text = "";
        Selected_Option.GetComponent<PopTweenCustom>().StopAnim();

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (CurrentQuestionItems[i] != null)
            {
                CurrentQuestionItems[i].GetComponent<Text>().raycastTarget = false;
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        for (int i = 0; i < CurrentItemsLeft.Length; i++)
        {
            if (CurrentItemsLeft[i] != null)
                Destroy(CurrentItemsLeft[i].gameObject);
        }
        for (int i = 0; i < CurrentItemsRight.Length; i++)
        {
            if (CurrentItemsRight[i] != null)
                Destroy(CurrentItemsRight[i].gameObject);
        }

        QuestionOrderListtemp = new List<int>();

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

            LeftNum_Digit = Left_Numbers1[QuestionsOrder1[QuestionOrder1]];
            RightNum_Digit = Right_Numbers1[QuestionsOrder1[QuestionOrder1]];

            Debug.Log("Left Side" + LeftNum_Digit);
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
            CurrentItem = 0;
        }
        else
        if (QuestionOrder2 < (QuestionOrderList1.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = QuestionOrder2;
            QuestionOrderListtemp.Remove(QuestionsOrder2[tempq]);
            CurrentQuestion = QuestionsOrder2[tempq];
            CorrectAnsrIndex = QuestionsOrder2[tempq];

            LeftNum_Digit = Left_Numbers2[QuestionsOrder2[QuestionOrder2]];
            RightNum_Digit = Right_Numbers2[QuestionsOrder2[QuestionOrder2]];

            Debug.Log("Left Side" + LeftNum_Digit);
            Debug.Log("Question No : " + QuestionOrder2 + " A : " + QuestionsOrder2[QuestionOrder2]);
            QuestionOrder2++;
            CurrentItem = 1;
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
            CorrectAnsrIndex = WrongAnsweredQuestions1[tempq];

            LeftNum_Digit = Left_Numbers1[WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]];
            RightNum_Digit = Right_Numbers1[WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
            CurrentItem = 0;
        }
        else
        if (WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
                Debug.Log("Here");
            }
            tempq = WrongAnsweredQuestionOrder2;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[tempq]);
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            CorrectAnsrIndex = WrongAnsweredQuestions2[tempq];

            LeftNum_Digit = Left_Numbers2[WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]];
            RightNum_Digit = Right_Numbers2[WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            WrongAnsweredQuestionOrder2++;
            CurrentItem = 1;
        }

        LevelsHolder.gameObject.SetActive(true);
        OptionObj.gameObject.SetActive(true);

        RandomOption = new List<int>();
        for (int i = 0; i < QuestionsItems[CurrentQuestion].Question_Items.Length; i++)
        {
            RandomOption.Add(i);
        }

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i] = OptionObj.transform.GetChild(i).gameObject;
        }

        if (LeftNum_Digit < RightNum_Digit)
        {
            Currect_Ansr = 0;
        }
        if (LeftNum_Digit == RightNum_Digit)
        {
            Currect_Ansr = 1;
        }
        if (LeftNum_Digit > RightNum_Digit)
        {
            Currect_Ansr = 2;
        }

        if (CurrentItem == 0)
        {
            CurrentItemsLeft = new GameObject[Left_Numbers1[CurrentQuestion]];
            CurrentItemsRight = new GameObject[Right_Numbers1[CurrentQuestion]];
        }
        if (CurrentItem == 1)
        {
            CurrentItemsLeft = new GameObject[Left_Numbers2[CurrentQuestion]];
            CurrentItemsRight = new GameObject[Right_Numbers2[CurrentQuestion]];
        }

        float delay = 0;
        for (int i = 0; i < CurrentItemsLeft.Length; i++)
        {
            if (i < 3)
            {
                QuestionsHolderLeft[0].gameObject.SetActive(true);
                CurrentItemsLeft[i] = Instantiate(QuestionItem, QuestionsHolderLeft[0].transform);
            }
            if (i >= 3 && i < 6)
            {
                QuestionsHolderLeft[1].gameObject.SetActive(true);
                CurrentItemsLeft[i] = Instantiate(QuestionItem, QuestionsHolderLeft[1].transform);
            }
            if (i >= 6)
            {
                QuestionsHolderLeft[2].gameObject.SetActive(true);
                CurrentItemsLeft[i] = Instantiate(QuestionItem, QuestionsHolderLeft[2].transform);
            }

            iTween.ScaleFrom(CurrentItemsLeft[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, delay));
            delay += 0.1f;
        }
        for (int i = 0; i < CurrentItemsRight.Length; i++)
        {
            if (i < 3)
            {
                QuestionsHolderRight[0].gameObject.SetActive(true);
                CurrentItemsRight[i] = Instantiate(QuestionItem, QuestionsHolderRight[0].transform);
            }
            if (i >= 3 && i < 6)
            {
                QuestionsHolderRight[1].gameObject.SetActive(true);
                CurrentItemsRight[i] = Instantiate(QuestionItem, QuestionsHolderRight[1].transform);
            }
            if (i >= 6)
            {
                QuestionsHolderRight[2].gameObject.SetActive(true);
                CurrentItemsRight[i] = Instantiate(QuestionItem, QuestionsHolderRight[2].transform);
            }

            iTween.ScaleFrom(CurrentItemsRight[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, delay));
            delay += 0.1f;
        }

        int _ixx = RandomNoFromList_Int(RandomOption);
        for (int i = 0; i < CurrentItemsLeft.Length; i++)
        {

            CurrentItemsLeft[i].GetComponent<Image>().sprite = QuestionsItems[CurrentQuestion].Question_Items[_ixx];
            RandomOption.Remove(_ixx);
        }

        int _iyy = RandomNoFromList_Int(RandomOption);
        for (int i = 0; i < CurrentItemsRight.Length; i++)
        {
            CurrentItemsRight[i].GetComponent<Image>().sprite = QuestionsItems[CurrentQuestion].Question_Items[_iyy];
            RandomOption.Remove(_iyy);
        }

        Invoke("PlayQuestion", delay + 0.5f);
        Is_OkButtonPressed = false;
    }

    void PlayQuestion()
    {
        PlayQuestionVoiceOver(CurrentQuestion);
        Invoke("EnableOptionsRaycast", QVOLength);
        iTween.MoveTo(Hanging_Board.gameObject, iTween.Hash("y", 208f, "time", 0.25f, "delay",QVOLength, "islocal", true, "easetype", iTween.EaseType.linear));
        float _delay = 0;
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            iTween.ScaleFrom(CurrentQuestionItems[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay + QVOLength + 0.25f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+QVOLength+0.25f));
            _delay += 0.1f;
        }
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i].GetComponent<Text>().raycastTarget = true;
        }
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

    public IEnumerator SetOk_Button(bool _IsSet, float _delay)
    {
        Is_CanClick = _IsSet;
        yield return new WaitForSeconds(_delay);
        Btn_Ok.gameObject.SetActive(_IsSet);
        Btn_Ok_Dummy.gameObject.SetActive(!_IsSet);
    }

    int UserAnsr;
    int UserAnseri;
    int Currect_Ansr;
    public void Check_Answer(int _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (UserAnsr == i)
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StartAnim();
                UserAnseri = i;
                Selected_Option.GetComponent<Text>().text = "" + CurrentQuestionItems[i].GetComponent<Text>().text;
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

    void HighlightOptions()
    {
        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            if (UserAnsr == Currect_Ansr)
            {
                Selected_Option.GetComponent<Text>().text = CurrentQuestionItems[i].GetComponent<Text>().text;
                Selected_Option.GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
                Selected_Option.GetComponent<Text>().text = "" + CurrentQuestionItems[Currect_Ansr].GetComponent<Text>().text;
                Selected_Option.GetComponent<PopTweenCustom>().StartAnim();
            }
        }
    }

    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;
        PlayAudio(Sound_Btn_Ok_Click, 0);

        for (int i = 0; i < CurrentQuestionItems.Length; i++)
        {
            CurrentQuestionItems[i].GetComponent<Text>().raycastTarget = false;
        }

        StopRepetedAudio();

        if (UserAnsr == Currect_Ansr)
        {
            Selected_Option.GetComponent<PopTweenCustom>().StartAnim();

            for (int i = 0; i < CurrentQuestionItems.Length; i++)
            {
                CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }

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
            float LengthDelay = PlayAppreciationVoiceOver(0.25f);
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay + 0.25f);
            iTween.MoveTo(Hanging_Board.gameObject, iTween.Hash("y", 460f, "time", 0.25f, "delay", LengthDelay+LengthDelay2, "islocal", true, "easetype", iTween.EaseType.linear));
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 1.5f));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);

            if (QuestionOrder1 < (QuestionOrderList.Count) ||
                QuestionOrder2 < (QuestionOrderList1.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 3f);
            }
            else
            {
                //Invoke("ShowLC", LengthDelay + LengthDelay2 + 3);
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

            Debug.Log("WRONG ANSWER : " + UserAnseri);
            iTween.ShakePosition(CurrentQuestionItems[UserAnseri].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;

            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, Sound_IncorrectAnswer.clip.length+0.5f);
                Invoke("HighlightOptions", 0.5f);
                iTween.MoveTo(Hanging_Board.gameObject, iTween.Hash("y", 460f, "time", 0.25f, "delay", LengthDelay, "islocal", true, "easetype", iTween.EaseType.linear));

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList.Count) && CurrentItem == 0)
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionOrderList1.Count) && CurrentItem == 1)
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }
                else
                {
                   // ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                    QuestionOrder2 < (QuestionOrderList1.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                    WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))

                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + 2f));
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
                for (int i = 0; i < CurrentQuestionItems.Length; i++)
                {
                    CurrentQuestionItems[i].GetComponent<Text>().raycastTarget = true;
                    CurrentQuestionItems[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;
        if (VOLanguage == 0)
        {
            ClipLength = Sound_AVO[_Ai].EN_Sound_AO[CurrentItem].clip.length;
            PlayAudio(Sound_AVO[_Ai].EN_Sound_AO[CurrentItem], _delay);
            Debug.Log("Sound : " + _Ai);
        }
        if (VOLanguage == 1)
        {
            ClipLength = Sound_AVO[_Ai].HI_Sound_AO[CurrentItem].clip.length;
            PlayAudio(Sound_AVO[_Ai].HI_Sound_AO[CurrentItem], _delay);
        }
        if (VOLanguage == 2)
        {
            ClipLength = Sound_AVO[_Ai].TL_Sound_AO[CurrentItem].clip.length;
            PlayAudio(Sound_AVO[_Ai].TL_Sound_AO[CurrentItem], _delay);
        }
        return ClipLength;
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
    public AudioSource[] Sound_Intro6;
    public AudioSource[] Sound_Intro7;

    public QVO_AudioSource_C08021 Sound_QVO;
    public AVO_AudioSource_C08021[] Sound_AVO;

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
    public class AVO_AudioSource_C08021
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_C08021
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }
}
