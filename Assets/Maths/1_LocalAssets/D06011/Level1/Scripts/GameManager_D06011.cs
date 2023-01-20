using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_D06011 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    public static GameManager_D06011 Instance;
    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_CartHolder;
    public GameObject[] Tut_GemsHolder;
    public GameObject Tut_OptionHolder;
    public GameObject[] Tut_Carts;
    public GameObject[] TutorialGems;
    public GameObject TutHand1, TutHand2,TutHand3;
   

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject GemHolder;
    public GameObject GemObj;
    public GameObject[] CurrentGems;
    public GameObject[] Carts;
    public float[] CartTargetPositions;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] TotalQuestionsOrder;

    public int[] QuestionsOrder1;
    public int[] QuestionsOrder1_2;
    public int QuestionOrder1;

    public List<int> WrongAnsweredQuestions1;
    public List<int> WrongAnsweredQuestions1_2;
    public int WrongAnsweredQuestionOrder1;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;
    [HideInInspector]
    public List<int> QuestionCountListttemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int CurrentQuestion2;
    public int CurrentQuestionOrder;

    public int EachCartMaxCount;

    int WrongAnsrsCount;

    public GameObject SidePanel;
    public GameObject[] AnswerObjs;
    float AddValueInProgress = 0;
    float ValueAdd;
    public GameInputData gameInputData;
    public int TotalQues;
    public string Thisgamekey;
    public int[] Ques_1;

    void Start()
    {
        Instance=this;
        if (Testing == true && FrameworkOff == true)
        {
            TotalQues = 5;
            Thisgamekey = "na01081";

            SetTutorial(gameInputData);
        }
        /* if (Is_Tutorial)
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
        //btn_Back.SetActive(false);
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

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 1f);
              
        float _delay = 0;
        for (int i = 0; i < Tut_GemsHolder.Length; i++)
        {
            iTween.ScaleFrom(Tut_GemsHolder[i].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
           
        }
        
        Invoke("EnableAnimator", 3f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 1f);

       
        for (int i = 0; i < 3; i++)
        {
            Tut_Carts[i].gameObject.SetActive(true);
            Tut_Carts[i].transform.GetChild(1).GetComponent<Tut_ItemDrop_D06011>().EmptySlots = 3;
        }

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
        Tut_OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = true;
    }
    public void CallIntro2()
    {
        PlayAudio(Sound_Intro2, 0.5f);
        iTween.MoveTo(Tut_CartHolder.gameObject, iTween.Hash("x", -200, "islocal", true, "time", 3f, "delay", 0f, "easetype", iTween.EaseType.linear));
        Invoke("CallIntro3", Sound_Intro2[VOLanguage].clip.length + 0.5f);
       
    }
    public void CallIntro3()
    {
        StopAudio(Sound_Intro2);
        PlayAudio(Sound_Intro3, 0.5f);
        Invoke("CallIntro4", Sound_Intro3[VOLanguage].clip.length + 0.5f);
        
    }
    public void CallIntro4()
    {
        
        StopAudio(Sound_Intro3);
        TutHand1.gameObject.SetActive(false);
        PlayAudioRepeated(Sound_Intro4);
       

        for (int i = 0; i < 9; i++)
        {
            Tut_GemsHolder[i].GetComponent<ItemDrag_D06011>().GetInitProperties();
            Tut_GemsHolder[i].transform.gameObject.GetComponent<Image>().raycastTarget = true;
        }
        Invoke("Tut_CheckAllCartsareFilled", 0.5f);
    }
    public void Tut_CheckAllCartsareFilled()
    {
        bool _IsFilledCart = false;
        for (int i = 0; i < 3; i++)
        {
            if (Tut_Carts[i].transform.GetChild(1).GetComponent<Tut_ItemDrop_D06011>().EmptySlots == 0)
            {
                PlayAudio(Sound_Selection, 0);
                _IsFilledCart = true;
                Debug.Log("FILL : " + _IsFilledCart);
               
            }
            else
            {
                _IsFilledCart = false;
                Debug.Log("FILL : " + _IsFilledCart);
                return;
            }
        }

        Debug.Log("FILL : " + _IsFilledCart);
        if (_IsFilledCart == true)
        {
            StopRepetedAudio();
           // StopAudio(Sound_Intro4);
            // StartCoroutine(SetOk_Button(true, 0));
           
            Invoke("CallIntro5", 0.5f);
        }
       
    }
    public void CallHand3()
    {
        TutHand3.gameObject.SetActive(true);
    }
    public void CallIntro5()
    {
        StopAudio(Sound_Intro4);
        //PlayAudio(Sound_Intro5, 0.5f);
        Invoke("CallIntro6", Sound_Intro5[VOLanguage].clip.length+ 0.2f);
    }
    public void CallIntro6()
    {
        //StopAudio(Sound_Intro5);
        PlayAudio(Sound_Intro6, 0.5f);
        Invoke("CallHand3", 4.5f);
        Invoke("CallIntro7", Sound_Intro6[VOLanguage].clip.length + 0.5f);
       
        Invoke("EnableTutRaycast", Sound_Intro6[VOLanguage].clip.length );
    }
    public void CallIntro7()
    {
        StopAudio(Sound_Intro6);
        PlayAudioRepeated(Sound_Intro7);
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 530, "islocal", true, "time", 1f, "delay", 0f, "easetype", iTween.EaseType.linear));
    }

    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        PlayAudio(Sound_Selection, 0);
        Tut_OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = false;
        Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();
        StopRepetedAudio();
        StopAudio(Sound_Intro7);
        PlayAudioRepeated(Sound_Intro8);
        TutHand3.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutBtn_Okay.gameObject.SetActive(false);
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        PlayAudio(Sound_CorrectAnswer, LengthDelay + 1f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 1f);
        Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 1.5f);
        PlayAudio(Sound_Intro9, LengthDelay + 1.5f);
        Invoke("SetGamePlay", LengthDelay + Sound_Intro7[VOLanguage].clip.length+ 3.5f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        TutorialObj.gameObject.SetActive(false);
        LevelObj.gameObject.SetActive(true);

        //ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;

        if (Is_NeedRandomizedQuestions)
        { QuestionsOrder1 = RandomArray_Int(QuestionsOrder1); }

        QuestionOrderList = new List<int>();
        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < NoOfQuestionsToAsk; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList[i].ToString();
            QuesKeys.Add(AddKey);
        }
        for (int i = 0; i < QuestionOrderList.Count; i++)
        {
            Debug.Log("QuesOrdList:::" + QuestionOrderList[i].ToString());
        }

        //--------Forcefully Removing Repeated Elements from QuesKeys--------------
        QuesKeys = QuesKeys.Distinct().ToList();

        for (int i = 0; i < QuesKeys.Count; i++)
        {
            Debug.Log("QuesKeys:::" + QuesKeys[i].ToString());
        }

        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));

        GenerateLevel();
    }
    
    int RandAnsrIndex = 0;
    public void GenerateLevel()
    {
        TutorialObj.gameObject.SetActive(false);

        TotalQuestionsOrder = new int[NoOfQuestionsToAsk];
        for (int i = 0; i < TotalQuestionsOrder.Length; i++)
        {
            TotalQuestionsOrder[i] = i;
        }

        GemHolder.GetComponent<GridLayoutGroup>().enabled = true;

        iTween.MoveTo(Carts[0].transform.parent.gameObject, iTween.Hash("x", 800, "islocal", true, "time", 0f, "delay", 0f, "easetype", iTween.EaseType.linear));

        for (int i = 0; i < CurrentGems.Length; i++)
        {
            Destroy(CurrentGems[i]);
        }

        CurrentGems = new GameObject[0];

        int tempq = 0;
        int RandAnsrIndex = Random.Range(0, 3);

        //Is_PressedOkOnce = false;
        Is_OkButtonPressed = false;

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        for (int i = 0; i < Carts.Length; i++)
        {
            Carts[i].gameObject.SetActive(false);
        }

        QuestionOrderListtemp = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length-1; i++)
        {
            // LIST IS FOR SELECTING OPTION NUMBERS
            QuestionOrderListtemp.Add(i+1);
        }

        if (QuestionOrder1 < (QuestionOrderList.Count))
        {
            tempq = QuestionOrder1;
            
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentQuestion2 = QuestionsOrder1_2[tempq];
            CurrentQuestionOrder = QuestionOrder1;
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = QuestionsOrder1[WrongAnsweredQuestions1[tempq]];
            CurrentQuestion2 = QuestionsOrder1_2[WrongAnsweredQuestions1[tempq]];
            TargetArray = QuestionsOrder1;

            CurrentQuestionOrder = WrongAnsweredQuestions1[tempq];

            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }

        CurrentGems = new GameObject[CurrentQuestion];
        float _delay3 = 0;
        for (int i = 0; i < CurrentQuestion; i++)
        {
            CurrentGems[i]=Instantiate(GemObj.gameObject, Vector3.zero, Quaternion.identity,GemHolder.transform);
            
            //iTween.ScaleFrom(CurrentGems[i].gameObject, iTween.Hash("Scale", 0.5f, "time", 1f, "delay", _delay3, "easetype", iTween.EaseType.easeOutElastic));

            _delay3 += 0.1f;

            if (i == (CurrentQuestion - 1))
            {
                //GemHolder.GetComponent<GridLayoutGroup>().enabled = false;
            }
        }

        EachCartMaxCount = CurrentQuestion / CurrentQuestion2;
        Debug.Log("EachCartMaxCount : " + EachCartMaxCount);

        Invoke("GemsPoping", 0);
        for (int i = 0; i < CurrentQuestion2; i++)
        {
            
            Carts[i].gameObject.SetActive(true);
            Carts[i].transform.GetChild(1).GetComponent<ItemDrop_D06011>().EmptySlots = EachCartMaxCount;
        }        

        iTween.MoveTo(Carts[0].transform.parent.gameObject, iTween.Hash("x", CartTargetPositions[CurrentQuestion2-1],"islocal",true, "time", 3f, "delay", 0.1f, "easetype", iTween.EaseType.linear));

        Is_OkButtonPressed = false;

        

        PlayQuestionVO1(CurrentQuestion-1,3f);
        
        Debug.Log("Question  : " + CurrentQuestion+" Cart Count : "+CurrentQuestion2);
    }

    public void GemsPoping()
    {
        float _delayx = 0;
        for (int i = 0; i < CurrentGems.Length; i++)
        {
            iTween.ScaleFrom(CurrentGems[i].gameObject, iTween.Hash("scale", Vector3.zero, "islocal", true, "time", 0.5f, "delay", _delayx, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delayx));
            _delayx += 0.1f;
        }
    }

    int _tempQopt;
    public IEnumerator SetAnswerOptions(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        RandAnsrIndex = Random.Range(0, 3);

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        QuestionOrderListtemp.Remove(_tempQopt);

        float _delay1 = 0.5f;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = "" + _tempQopt;
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _ixx;
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _iyy;
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    AnswerObjs[i].GetComponent<Text>().text = "" + _izz;
                    QuestionOrderListtemp.Remove(_izz);
                }
            }

            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay1));
            _delay1 += 0.1f;
        }
    }

    public void CheckAllCartsareFilled()
    {
        bool _IsFilled=false;
        for (int i = 0; i < CurrentQuestion2; i++)
        {
            PlayAudio(Sound_Selection, 0);
            if (Carts[i].transform.GetChild(1).GetComponent<ItemDrop_D06011>().EmptySlots == 0)
            {
                _IsFilled = true;               
            }
            else
            {
                _IsFilled = false;
                Debug.Log("FILL : " + _IsFilled);
                return;
            }
        }

        Debug.Log("FILL : " + _IsFilled);
        if (_IsFilled == true)
        {
           // StopRepetedAudio();
           // StartCoroutine(SetOk_Button(true, 0));
            CancelInvoke("PlayQuestionVO3");
            StopRepetedAudio();
            Invoke("PlayQuestionVO4", 0.5f);

            _tempQopt = EachCartMaxCount;
            StartCoroutine("SetAnswerOptions", 0);

            iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 525, "islocal", true, "time", 0.5f, "delay", 0f, "easetype", iTween.EaseType.linear));
            
            CancelInvoke("RepeatQVOAftertChoosingOption");
        }
    }

    public void StopAudioRepeateForOnDrag()
    {
        if (!Is_RepeatedVOPlaying)
            return;

        StopRepetedAudio();
        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption", 7);
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
        }

        Debug.Log("EnableOptionsRaycast Here");

        GemHolder.GetComponent<GridLayoutGroup>().enabled = false;

        for (int i = 0; i < CurrentQuestion; i++)
        {
            CurrentGems[i].GetComponent<ItemDrag_D06011>().GetInitProperties();
        }
    }

    void PlayQuestionVO1(int _Qi, float _delay)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO1.EN_Sound_QO[_Qi].clip.length;
                PlayAudio(Sound_QVO1.EN_Sound_QO[_Qi], _delay);
                break;
            case 1:
                QVOLength = Sound_QVO1.HI_Sound_QO[_Qi].clip.length;
                PlayAudio(Sound_QVO1.HI_Sound_QO[_Qi], _delay);
                break;
            case 2:
                QVOLength = Sound_QVO1.TL_Sound_QO[_Qi].clip.length;
                PlayAudio(Sound_QVO1.TL_Sound_QO[_Qi], _delay);
                break;
        }

        PlayQuestionVO2(CurrentQuestionOrder, QVOLength+3f);
    }

    void PlayQuestionVO2(int _Qi, float _delay)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO2.EN_Sound_QO[_Qi].clip.length;
                PlayAudio(Sound_QVO2.EN_Sound_QO[_Qi], _delay);
                break;
            case 1:
                QVOLength = Sound_QVO2.HI_Sound_QO[_Qi].clip.length;
                PlayAudio(Sound_QVO2.HI_Sound_QO[_Qi], _delay);
                break;
            case 2:
                QVOLength = Sound_QVO2.TL_Sound_QO[_Qi].clip.length;
                PlayAudio(Sound_QVO2.HI_Sound_QO[_Qi], _delay);
                break;
        }

        Invoke("EnableOptionsRaycast", QVOLength);
        //Invoke("PlayQuestionVO3", QVOLength + _delay+7);
    }

    void PlayQuestionVO3()
    {
        PlayQuestionVO3(0);
        Debug.Log("PlayQuestionVO3_");
    }

    void PlayQuestionVO3(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO3.EN_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO3.EN_Sound_QO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO3.HI_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO3.HI_Sound_QO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO3.TL_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO3.HI_Sound_QO[_Qi]);
                break;
        }
    }
    void PlayQuestionVO4()
    {
        PlayQuestionVO4(Random.Range(0, 2));
        Invoke("EnableOptionsRaycast", QVOLength);

    }
    void PlayQuestionVO4(int _Qi)
    {
        StopRepetedAudio();

        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO4.EN_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO4.EN_Sound_QO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO4.HI_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO4.HI_Sound_QO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO4.TL_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO4.HI_Sound_QO[_Qi]);
                break;
        }

        Debug.Log("_Qi : " + _Qi);
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO.EN_Sound_AO[_Ai-1], _delay);
        ClipLength = Sound_AVO.EN_Sound_AO[_Ai-1].clip.length;

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
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (i ==_Ansrindex)
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
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
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (i == CorrectAnsrIndex)
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
    }

    bool Is_OkButtonPressed = false;
    int CountOk;
    float LengthDelay;

    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
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
           // ProgreesBar.GetComponent<Slider>().value += 1;
            Total_CorrectAnswers++;//INGAME_COMMON
            WrongAnsrsCount = 0;

            Invoke("HighlightOptions", 0.5f);
            float LengthDelay = PlayAppreciationVoiceOver(0.25f);
            float LengthDelay2 = PlayAnswerVoiceOver(EachCartMaxCount, LengthDelay+0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);
            iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 5000, "time", 1f, "delay", LengthDelay + LengthDelay2 + 2.5f, "easetype", iTween.EaseType.linear));
            iTween.MoveTo(Carts[0].transform.parent.gameObject, iTween.Hash("x", -1200, "islocal", true, "time", 3f, "delay", 0.1f, "easetype", iTween.EaseType.linear));
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);

            if (QuestionOrder1 < (QuestionOrderList.Count) ||
            WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
            }
            else
            {
                Debug.Log("Questions Finished");
                // StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + LengthDelay2+3f));
                // Invoke("ShowLC", LengthDelay + LengthDelay2 + 4f);
                SendResultFinal();
            }
            CancelInvoke("RepeatQVOAftertChoosingOption");
        }
        else
        {
            iTween.ShakePosition(AnswerObjs[UserAnsr].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            if (FrameworkOff == false)
            {
                string AddKey = "" + Thisgamekey + "_Q" + CurrentQuestion.ToString();
                GameFrameworkInterface.Instance.AddResult(AddKey, Tpix.UserData.QAResult.Wrong);
                Debug.Log("Add : " + AddKey + ": Wrong");
            }
            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;

            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(EachCartMaxCount, 1f);
                
                Invoke("HighlightOptions", 1f);
                iTween.MoveTo(Carts[0].transform.parent.gameObject, iTween.Hash("x", -1200, "islocal", true, "time", 3f, "delay", 0.1f, "easetype", iTween.EaseType.linear));
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 5000, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.linear));
                StopRepetedAudio();
                TargetList = WrongAnsweredQuestions1;
                if (!WrongAnsweredQuestions1.Contains(TotalQuestionsOrder[CurrentQuestionOrder]) && QuestionOrder1 <= (QuestionOrderList.Count))
                {
                    if (WrongAnsweredQuestionOrder1 <= 0)
                    {
                        WrongAnsweredQuestions1.Add(TotalQuestionsOrder[CurrentQuestionOrder]);
                    }
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2f);
                }
                else
                {
                    Debug.Log("Questions Finished");
                    // StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + 2f));
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
                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
                    AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                }
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

    int[] TargetArray;
    int FindIndexofElementinArray(int _TargetElement)
    {
        int _returnElement = 0;
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
    public AudioSource[] Sound_Intro6;
    public AudioSource[] Sound_Intro7;
    public AudioSource[] Sound_Intro8;
    public AudioSource[] Sound_Intro9;

    public QVO_AudioSource_D06011 Sound_QVO1;
    public QVO_AudioSource_D06011 Sound_QVO2;
    public QVO_AudioSource_D06011 Sound_QVO3;
    public QVO_AudioSource_D06011 Sound_QVO4;
    public AVO_AudioSource_D06011 Sound_AVO;

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

    bool Is_RepeatedVOPlaying;
    public void PlayAudioRepeated(AudioSource _audio)
    {        
        Debug.Log("PlayAudioRepeated : " + _audio.name);
        _audiotorepeat = _audio;
        StartCoroutine("PlayAudioRepeatedSingleCall");
    }

    AudioSource _audiotorepeat;
    float QVOLength;
    public IEnumerator PlayAudioRepeatedSingleCall()
    {
        yield return new WaitForSeconds(0);
        if (!Is_OkButtonPressed)
        {
            Is_RepeatedVOPlaying = true;
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
            Is_RepeatedVOPlaying = true;
            _audiotorepeatarray[VOLanguage].PlayDelayed(0);
            yield return new WaitForSeconds(7 + QVOLength);
            StartCoroutine("PlayAudioRepeatedCall");
        }
    }

    public void StopRepetedAudio()
    {
        Is_RepeatedVOPlaying = false;

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
    public class AVO_AudioSource_D06011
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_D06011
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }

}

