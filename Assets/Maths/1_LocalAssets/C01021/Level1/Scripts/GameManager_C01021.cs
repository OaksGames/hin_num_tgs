using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_C01021 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject Tut_Items;
    public GameObject[] Tut_AnswerObjs;
    public GameObject TutHand1, TutHand2;
    public GameObject OptionHolder;
    public GameObject ItemSet;
   

   [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;

  //  [HideInInspector]
    public int NoOfQuestionsType1Ask;
    public int NoOfQuestionsType2Ask;


    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject OptionsHolder;
    
    public GameObject[] QuestionsHolder;
    public GameObject[] QuestionItems;
    public GameObject[] QuestionItemsChild;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList1;
    public List<int> QuestionOrderList2;

    public int[] TotalNumofQuestions1;
    public int[] RandQuestionsOrder1;
    public int[] QuestionsOrder1;
    public int QuestionOrder1=0;
    public int RandQuestionOrder1 = 0;
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public int[] TotalNumofQuestions2;
    public int[] RandQuestionsOrder2;
    public int[] QuestionsOrder2;
    public int QuestionOrder2=0;
    public int RandQuestionOrder2 = 0;
    public List<int> WrongAnsweredQuestions2;
    public int WrongAnsweredQuestionOrder2;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int RandCurrentQuestion;
   
    public int CurrentTempQuestion;
    int WrongAnsrsCount;
    int CurrentItem;
    public GameObject[] CurrentItems;
    public GameObject[] AnswerObjs;

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
            AddValueInProgress = 1 / (float)(NoOfQuestionsType1Ask + NoOfQuestionsType2Ask);
            Thisgamekey = gameInputData.Key;
        }

        

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 2f);
        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length + 2f);
        PlayAudio(Sound_Intro3, Sound_Intro2[VOLanguage].clip.length + Sound_Intro1[VOLanguage].clip.length + 2f);
        PlayAudio(Sound_Intro4, Sound_Intro3[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro1[VOLanguage].clip.length + 2f);
        PlayAudio(Sound_Intro5, Sound_Intro4[VOLanguage].clip.length + Sound_Intro3[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro1[VOLanguage].clip.length + 2f);
        PlayAudio(Sound_Intro6, Sound_Intro5[VOLanguage].clip.length + Sound_Intro4[VOLanguage].clip.length + Sound_Intro3[VOLanguage].clip.length + Sound_Intro2[VOLanguage].clip.length + Sound_Intro1[VOLanguage].clip.length + 2f);  

        iTween.ScaleFrom(Tut_Items.transform.GetChild(0).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 2.1f, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 2.1f));
        iTween.ScaleFrom(Tut_Items.transform.GetChild(1).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 8f, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 8f));
        iTween.ScaleFrom(Tut_Items.transform.GetChild(2).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 8.1f, "easetype", iTween.EaseType.easeOutElastic));
         StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 8.1f));
        iTween.ScaleFrom(Tut_Items.transform.GetChild(3).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 8.15f, "easetype", iTween.EaseType.easeOutElastic));
         StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 8.15f));
        iTween.ScaleFrom(Tut_Items.transform.GetChild(4).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 8.2f, "easetype", iTween.EaseType.easeOutElastic));
         StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 8.2f));
        iTween.ScaleFrom(Tut_Items.transform.GetChild(5).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 8.25f, "easetype", iTween.EaseType.easeOutElastic));
        StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 8.25f)); 
        iTween.ScaleFrom(Tut_Items.transform.GetChild(6).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 8.3f, "easetype", iTween.EaseType.easeOutElastic));
         StartCoroutine(PlayAudioAtOneShot(Sound_Ting, 8.3f));
        float _delay = 0;
        for (int i = 0; i < OptionHolder.transform.childCount; i++)
        {
             iTween.ScaleFrom(OptionHolder.transform.GetChild(i).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay+ Sound_Intro1[VOLanguage].clip.length + 19f, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay+ Sound_Intro1[VOLanguage].clip.length+19f));
            _delay += 0.2f;            
        }
        iTween.MoveTo(OptionHolder.transform.gameObject, iTween.Hash("x", 512,"islocal" , true, "time", 1f, "delay", Sound_Intro1[VOLanguage].clip.length + 18f, "easetype", iTween.EaseType.easeOutElastic));
        Invoke("CallIntro7", Sound_Intro7[VOLanguage].clip.length + 22f);
        Invoke("EnableAnimator", 2f);
    }

    

    public void CallIntro7()
    {
        PlayAudioRepeated(Sound_Intro7);
        Invoke ("EnableTutNoTRaycatTarget",2.2f);
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
        Tut_AnswerObjs[1].gameObject.GetComponent<Text>().raycastTarget = true;
    } 
    public void Selected_TutAnswer()
    {
        PlayAudio(Sound_Selection, 0);
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutBtn_Okay.gameObject.SetActive(true);
        Tut_AnswerObjs[1].gameObject.GetComponent<Text>().raycastTarget = false;
        Tut_AnswerObjs[1].gameObject.GetComponent<PopTweenCustom>().StartAnim();
        StopAudio(Sound_Intro7);
        StopAudio(Sound_Intro6);
        StopAudio(Sound_Intro4); StopAudio(Sound_Intro5);
        StopAudio(Sound_Intro3);
        StopAudio(Sound_Intro2);
        StopAudio(Sound_Intro1);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro8);
        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        PlayAudio(Sound_BtnOkClick, 0);
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro9);
        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);
        CurrentItem = 0; 
        PlayAppreciationVoiceOver(0.5f);
        float LengthDelay = PlayAnswerVoiceOver(6,1.2f);
        PlayAudio(Sound_Strwaberry,1.75f);
        iTween.MoveTo(OptionHolder.transform.gameObject, iTween.Hash("x", 2000, "time", 1f, "delay", LengthDelay+3f, "easetype", iTween.EaseType.easeOutElastic));
        PlayAudio(Sound_CorrectAnswer, LengthDelay + 2.5f);
        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + 2.5f);
        Tut_AnswerObjs[1].gameObject.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + 2f);
        Invoke("SetGamePlay", LengthDelay + Sound_Intro9[VOLanguage].clip.length+2f);
    }
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
       // CurrentItem = 0;
        LevelObj.gameObject.SetActive(true);
        TutorialObj.gameObject.SetActive(false);
        LevelsHolder.gameObject.SetActive(true);

        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsType1Ask + NoOfQuestionsType2Ask;
        }


        RandQuestionsOrder1 = new int[NoOfQuestionsType1Ask];
        RandQuestionsOrder2 = new int[NoOfQuestionsType2Ask];

        for (int i = 0; i < NoOfQuestionsType1Ask; i++)
        {
            RandQuestionsOrder1[i] = i;
        }

        for (int i = 0; i < NoOfQuestionsType2Ask; i++)
        {
            RandQuestionsOrder2[i] = i;
        }

        if (Is_NeedRandomizedQuestions)
        {
            RandQuestionsOrder1 = RandomArray_Int(RandQuestionsOrder1);
            RandQuestionsOrder2 = RandomArray_Int(RandQuestionsOrder2);
        }

        QuestionOrderList1 = new List<int>();
        QuestionOrderList2 = new List<int>();

        QuestionsOrder1 = new int[NoOfQuestionsType1Ask];
        QuestionsOrder2 = new int[NoOfQuestionsType2Ask];

        List<string> QuesKeys = new List<string>();

        for (int i = 0; i < NoOfQuestionsType1Ask; i++)
        {
            QuestionsOrder1[i] = TotalNumofQuestions1[RandQuestionsOrder1[i]];
        }
        for (int i = 0; i < NoOfQuestionsType2Ask; i++)
        {
              QuestionsOrder2[i] = TotalNumofQuestions2[RandQuestionsOrder2[i]];
        }

        for (int i = 0; i < NoOfQuestionsType1Ask; i++)
        {
            QuestionOrderList1.Add(RandQuestionsOrder1[i]);
            //------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + QuestionOrderList1[i].ToString();
            QuesKeys.Add(AddKey);
        }

        for (int i = 0; i < NoOfQuestionsType2Ask; i++)
        {
            QuestionOrderList2.Add(RandQuestionsOrder2[i]);
            //------------------------------------------
            string AddKey = "" + Thisgamekey + "_Q" + (QuestionOrderList2[i] + QuestionOrderList1.Count).ToString();
            QuesKeys.Add(AddKey);
        }
        if (FrameworkOff == false)
            GameFrameworkInterface.Instance.ReplaceQuestionKeys(QuesKeys);

        StartCoroutine(SetOk_Button(false, 0f));
        CurrentItems = new GameObject[3];
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;
      
        LevelsHolder.gameObject.SetActive(false);
        for (int i = 0; i < QuestionsHolder.Length; i++)
        {
            QuestionsHolder[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if(CurrentItems[i]!=null)
            Destroy(CurrentItems[i].gameObject);
        }
        QuestionOrderListtemp = new List<int>();
        TutorialObj.gameObject.SetActive(false);

        if (QuestionOrder1 < (QuestionOrderList1.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }
            tempq = QuestionOrder1;
            tempq = RandQuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[QuestionOrder1]);
            CurrentQuestion = QuestionsOrder1[QuestionOrder1];
            RandCurrentQuestion = RandQuestionsOrder1[QuestionOrder1];
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            CurrentTempQuestion = QuestionOrder1;
            CurrentItem = 0;
            QuestionOrder1++;
        }
        else
        if (QuestionOrder2 < (QuestionOrderList2.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = QuestionOrder2;
            tempq = RandQuestionOrder2;
            QuestionOrderListtemp.Remove(QuestionsOrder2[QuestionOrder2]);
            CurrentQuestion = QuestionsOrder2[QuestionOrder2];
            RandCurrentQuestion = RandQuestionsOrder2[QuestionOrder2];
            Debug.Log("Question No : " + QuestionOrder2 + " A : " + QuestionsOrder2[QuestionOrder2]);
            CurrentTempQuestion = QuestionOrder2;
            CurrentItem = 1;
            QuestionOrder2++;
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
            TargetArray = QuestionsOrder1;
            CurrentTempQuestion = FindIndexofElementinArray(CurrentQuestion);
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            CurrentItem = 0;
            RandCurrentQuestion = RandQuestionsOrder1[CurrentTempQuestion]; 
            WrongAnsweredQuestionOrder1++;
        }
        else
       if (WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = WrongAnsweredQuestionOrder2;

            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[tempq]);
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            TargetArray = QuestionsOrder2;
            CurrentTempQuestion = FindIndexofElementinArray(CurrentQuestion);
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            CurrentItem = 1;
            //int Ab = FindIndexofElementinArray(Ab);
            RandCurrentQuestion = RandQuestionsOrder2[CurrentTempQuestion];
            WrongAnsweredQuestionOrder2++;
        }

            float _delay1 = 0;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = "" + CurrentQuestion ;                               
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
           
        }
        LevelsHolder.gameObject.SetActive(true);
        int ones = CurrentQuestion % 10;
        int tens = 0;
        if (CurrentQuestion >= 100)
        {
            tens = 10;
        }
        else
        {
            tens = (CurrentQuestion % 100 - ones) / 10;
        }
        Debug.Log("----String----" + ones + tens);
        CurrentItems = new GameObject[ones+tens];
        int _tempItemCount = 0; 
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            CurrentItems.ToString();
            float _delay = 0;
            if (i< tens)
            {                
                if (i < 5)
                {
                    QuestionsHolder[0].gameObject.SetActive(true);
                    CurrentItems[_tempItemCount] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[0].transform);
                    iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                    StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                    _tempItemCount++;
                }
                else
                {
                    QuestionsHolder[1].gameObject.SetActive(true);
                    CurrentItems[_tempItemCount] = Instantiate(QuestionItems[CurrentItem], QuestionsHolder[1].transform);
                    iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                    StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                    _tempItemCount++;
                }               
            }
            if (i < ones)
            {
                QuestionsHolder[2].gameObject.SetActive(true);
                CurrentItems[_tempItemCount] = Instantiate(QuestionItemsChild[CurrentItem], QuestionsHolder[2].transform);
                iTween.ScaleFrom(CurrentItems[_tempItemCount].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
                StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
                _tempItemCount++;
            }
            _delay += 0.1f;            
        }

        Is_OkButtonPressed = false;
        iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("X", 450, "islocal", true, "time", 1f, "delay", QVOLength, "easetype", iTween.EaseType.easeOutElastic));
       for (int i = 0; i < AnswerObjs.Length; i++)
        {
            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay",QVOLength + _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting,QVOLength + _delay1));
            _delay1 += 0.1f;
        }        
        Invoke("PlayQuestionVoiceOverWithDelay", CurrentItems.Length * 0.1f);
        Debug.Log("CorrectAnsrIndex : " + CorrectAnsrIndex);
    }
     void PlayQuestionVoiceOverWithDelay()
    {
        PlayQuestionVoiceOver(Random.RandomRange(0, 2));        
        Invoke("EnableOptionsRaycast", QVOLength + 1f);
    }
    void EnableOptionsRaycast()
    {       
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        float ClipLength = 0;
        switch (CurrentItem)
        {
            case 0:
                switch (_Qi)
                {
                    case 0:
                        QVOLength =Sound_Q1_Apple[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Apple);
                        break;
                    case 1:
                        QVOLength =Sound_Q2_Apple[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Apple);
                        break;
                }
                break;
            case 1:
                switch (_Qi)
                {
                    case 0:
                         QVOLength =Sound_Q1_Strwaberry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Strwaberry);
                        break;
                    case 1:
                        QVOLength =Sound_Q2_Strwaberry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Strwaberry);
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
                        PlayAudio(Sound_A1_Apple, _delay);
                        ClipLength = Sound_A1_Apple[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A2_Apple, _delay);
                        ClipLength = Sound_A2_Apple[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A3_Apple, _delay);
                        ClipLength = Sound_A3_Apple[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A4_Apple, _delay);
                        ClipLength = Sound_A4_Apple[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A5_Apple, _delay);
                        ClipLength = Sound_A5_Apple[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A6_Apple, _delay);
                        ClipLength = Sound_A6_Apple[VOLanguage].clip.length;
                        break;
                    case 6:
                        PlayAudio(Sound_A7_Apple, _delay);
                        ClipLength = Sound_A7_Apple[VOLanguage].clip.length;
                        break;
                    case 7:
                        PlayAudio(Sound_A8_Apple, _delay);
                        ClipLength = Sound_A8_Apple[VOLanguage].clip.length;
                        break;
                    case 8:
                        PlayAudio(Sound_A9_Apple, _delay);
                        ClipLength = Sound_A9_Apple[VOLanguage].clip.length;
                        break;
                    case 9:
                        PlayAudio(Sound_A10_Apple, _delay);
                        ClipLength = Sound_A10_Apple[VOLanguage].clip.length;
                        break;
                }
                PlayAudio(Sound_Strwaberry,ClipLength+_delay);
                break;
            case 1:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A1_Strwaberry , _delay);                        
                        ClipLength = Sound_A1_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A2_Strwaberry, _delay);
                        ClipLength = Sound_A2_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A3_Strwaberry, _delay);
                        ClipLength = Sound_A3_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A4_Strwaberry, _delay);
                        ClipLength = Sound_A4_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A5_Strwaberry, _delay);
                        ClipLength = Sound_A5_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A6_Strwaberry, _delay);
                        ClipLength = Sound_A6_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 6:
                        PlayAudio(Sound_A7_Strwaberry, _delay);
                        ClipLength = Sound_A7_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 7:
                        PlayAudio(Sound_A8_Strwaberry, _delay);
                        ClipLength = Sound_A8_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 8:
                        PlayAudio(Sound_A9_Strwaberry, _delay);
                        ClipLength = Sound_A9_Strwaberry[VOLanguage].clip.length;
                        break;
                    case 9:
                        PlayAudio(Sound_A10_Strwaberry, _delay);
                        ClipLength = Sound_A10_Strwaberry[VOLanguage].clip.length;
                        break;
                }
             PlayAudio(Sound_Apple, ClipLength+_delay);
                break;
           
        }
        return ClipLength;
    }
    //  public float PlayAnswerVOItemName(int _Aj, float _delay)
    // {
    //     float ClipLength = 0;
    //     switch (CurrentItem)
    //     {
    //         case 0:
    //             switch (_Aj)
    //             {
    //             case 0:
    //                  PlayAudio(Sound_Strwaberry, _delay);
    //                  ClipLength = Sound_Strwaberry[VOLanguage].clip.length;
    //                  break;
    //             }
    //         break;
    //         case 1:
    //             switch (_Aj)
    //             {
    //             case 0:
    //                  PlayAudio(Sound_Apple, _delay);
    //                  ClipLength = Sound_Apple[VOLanguage].clip.length;
    //                  break;
    //             }
    //         break;
    //     }
    //     return ClipLength;
    // }

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
    public void Check_Answer(int _Ansrindex)
    {
        StopRepetedAudio();
        UserAnsr = _Ansrindex;
        StartCoroutine(SetOk_Button(true, 0));
        PlayAudio(Sound_Selection, 0);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i].name.Contains(UserAnsr.ToString()))
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
        StartCoroutine("PlayAudioRepeatedCall");
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
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;
        PlayAudio(Sound_BtnOkClick, 0);
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = false;          
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
            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length+0.25f)+ Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(RandCurrentQuestion, LengthDelay + 0.5f);
           // PlayAnswerVOItemName(CurrentItem,LengthDelay+LengthDelay2+0.2f);
            
            iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("x", 1000,"islocal",true, "time", 1f, "delay", LengthDelay+LengthDelay2 + 2f, "easetype", iTween.EaseType.easeOutElastic));            
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 2f); 
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 2f);
            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2.5f));
            
            if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                QuestionOrder2 < (QuestionOrderList2.Count) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))                
            {
                Invoke("GenerateLevel", LengthDelay+LengthDelay2 +4f);
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
            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                if (AnswerObjs[i].name.Contains(UserAnsr.ToString()))
                {                    
                    iTween.ShakePosition(AnswerObjs[i].gameObject, iTween.Hash("x", 10f, "time", 0.25f));
                }
            }
            PlayAudio(Sound_IncorrectAnswer, 0);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float Length = PlayAnswerVoiceOver(RandCurrentQuestion, 0);
                Invoke("HighlightOptions", Sound_IncorrectAnswer.clip.length);
                iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("x", 1000, "islocal", true, "time", 1f, "delay", Sound_IncorrectAnswer.clip.length+ Length, "easetype", iTween.EaseType.easeOutElastic));
                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    if (AnswerObjs[i] != null)
                    {
                        AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                    }
                }
                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList1.Count) && CurrentItem == 0)
                   
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionOrderList2.Count) && CurrentItem == 1)
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }                
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                    QuestionOrder2 < (QuestionOrderList2.Count) ||
                    WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
                   
                {
                    Invoke("GenerateLevel", Length+2f);
                }
                else
                {
                   
                    StartCoroutine(SetActiveWithDelayCall(LevelObj, false, Length + 2f));
                    //Invoke("ShowLC", 2.5f);
                    Debug.Log("Questions Finished");
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


    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }

    public void ShowLC()
    {
        LCObj.gameObject.SetActive(true);
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
    public AudioSource[] Sound_Intro8;
    public AudioSource[] Sound_Intro9;

    public AudioSource[] Sound_Q1_Apple;
    public AudioSource[] Sound_Q2_Apple;

    public AudioSource[] Sound_Q1_Strwaberry;
    public AudioSource[] Sound_Q2_Strwaberry;
    
    public AudioSource[] Sound_A1_Apple;
    public AudioSource[] Sound_A2_Apple;
    public AudioSource[] Sound_A3_Apple;
    public AudioSource[] Sound_A4_Apple;
    public AudioSource[] Sound_A5_Apple;
    public AudioSource[] Sound_A6_Apple;
    public AudioSource[] Sound_A7_Apple;
    public AudioSource[] Sound_A8_Apple;
    public AudioSource[] Sound_A9_Apple;
    public AudioSource[] Sound_A10_Apple;

    public AudioSource[] Sound_A1_Strwaberry;
    public AudioSource[] Sound_A2_Strwaberry;
    public AudioSource[] Sound_A3_Strwaberry;
    public AudioSource[] Sound_A4_Strwaberry;
    public AudioSource[] Sound_A5_Strwaberry;
    public AudioSource[] Sound_A6_Strwaberry;
    public AudioSource[] Sound_A7_Strwaberry;
    public AudioSource[] Sound_A8_Strwaberry;
    public AudioSource[] Sound_A9_Strwaberry;
    public AudioSource[] Sound_A10_Strwaberry;
public AudioSource[] Sound_Apple;
public AudioSource[] Sound_Strwaberry;
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

    #region FIND INDEX OF ELEMENT IN A ARRAY
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
    #endregion
}
