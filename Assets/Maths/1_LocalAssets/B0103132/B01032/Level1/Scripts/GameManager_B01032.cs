using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_B01032 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;
    public GameObject[] Tut_Items;
    public GameObject[] Tut_AnswerObjs;
    public GameObject TutHand1, TutHand2;
    public GameObject TutDownBar;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk1;
    public int NoOfQuestionsToAsk2;
    public int NoOfQuestionsToAsk3;
    public int NoOfQuestionsToAsk4;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject LevelsHolder;
    public GameObject OptionsHolder;

    public GameObject[] QuestionsHolder;
    public GameObject[] QuestionItems;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList1;
    [HideInInspector]
    public List<int> QuestionOrderList2;
    [HideInInspector]
    public List<int> QuestionOrderList3;
    [HideInInspector]
    public List<int> QuestionOrderList4;

    public int[] QuestionsOrder1;
    public int[] TotalNumofQuestions1;
    public int[] RandQuestionsOrder1;
    public int QuestionOrder1=0;
    public int RandQuestionOrder1 = 0;
   
    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public int[] QuestionsOrder2;
    public int[] TotalNumofQuestions2;
    public int[] RandQuestionsOrder2;
    public int QuestionOrder2=0;
    public int RandQuestionOrder2 = 0;

    public List<int> WrongAnsweredQuestions2;
    public int WrongAnsweredQuestionOrder2;

    public int[] QuestionsOrder3;
    public int[] TotalNumofQuestions3;
    public int[] RandQuestionsOrder3;
    public int QuestionOrder3=0;
    public int RandQuestionOrder3 = 0;

    public List<int> WrongAnsweredQuestions3;
    public int WrongAnsweredQuestionOrder3;

    public int[] QuestionsOrder4;
    public int[] TotalNumofQuestions4;
    public int[] RandQuestionsOrder4;
    public int QuestionOrder4=0;
    public int RandQuestionOrder4 = 0;

    public List<int> WrongAnsweredQuestions4;
    public int WrongAnsweredQuestionOrder4;

    [HideInInspector]
    public List<int> QuestionOrderListtemp;

    [HideInInspector]
    public int CorrectAnsrIndex;
    public int CurrentQuestion;
    public int CurrentTempQuestion;
    public int RandCurrentQuestion;

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
            MultiLevelManager.instance.LoadProgressMaxValues(NoOfQuestionsToAsk1 + NoOfQuestionsToAsk2 + NoOfQuestionsToAsk3 + NoOfQuestionsToAsk4);
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
    }
    */

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

        PlayAudio(Sound_Intro1, 2.5f);
        PlayAudio(Sound_Intro2, Sound_Intro1[VOLanguage].clip.length + 2.5f);

        float _delay = 0;
        for (int i = 0; i < Tut_Items.Length; i++)
        {           
            iTween.ScaleFrom(Tut_Items[i].gameObject, iTween.Hash("Scale", Vector3.zero * 1f, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }
        Invoke("EnableAnimator", 1f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length+Sound_Intro2[VOLanguage].clip.length + 2.5f);

        iTween.MoveTo(TutDownBar.gameObject, iTween.Hash("y", 0, "time", 1f, 
            "delay", 10, "easetype", iTween.EaseType.easeOutElastic));
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
        PlayAudioRepeated(Sound_Intro3);
        Invoke("EnableTutOptionRaycast", 2);
    }

    public void EnableTutOptionRaycast()
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

        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        PlayAudioRepeated(Sound_Intro4);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        PlayAudio(Sound_BtnOkClick, 0);
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro4);
        StopRepetedAudio();
        TutHand2.gameObject.SetActive(false);

        CurrentItem = 0;
        float LengthDelay = PlayAppreciationVoiceOver(0.25f);
        float LengthDelay2 = PlayAnswerVoiceOver(3, LengthDelay+0.25f);

        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.25f);

        Invoke("HighlightTutCorrectOption", LengthDelay);

       Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.25f);

        Tut_AnswerObjs[1].gameObject.GetComponent<PopTweenCustom>().Invoke("StopAnim", LengthDelay + LengthDelay2);

        PlayAudio(Sound_Intro5, LengthDelay + LengthDelay2 + 2f);
        iTween.MoveTo(TutDownBar.gameObject, iTween.Hash("y", -250, "time", 1f, "delay", 4, "easetype", iTween.EaseType.easeOutElastic));

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro5[VOLanguage].clip.length + 2f);
    }

    void HighlightTutCorrectOption()
    {
        for (int i = 0; i < Tut_Items.Length; i++)
        {
            if (Tut_Items[i].gameObject.name.Contains((1).ToString()))
            {
                Tut_Items[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                Tut_Items[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        for (int i = 0; i < Tut_AnswerObjs.Length; i++)
        {
            if (i == 1)
            {
                Tut_AnswerObjs[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                Tut_AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }
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
            ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk1 + NoOfQuestionsToAsk2 + NoOfQuestionsToAsk3 + NoOfQuestionsToAsk4;
        }

        RandQuestionsOrder1 = new int[NoOfQuestionsToAsk1];
        RandQuestionsOrder2 = new int[NoOfQuestionsToAsk2];
        RandQuestionsOrder3 = new int[NoOfQuestionsToAsk3];
        RandQuestionsOrder4 = new int[NoOfQuestionsToAsk4];

        for (int i = 0; i < NoOfQuestionsToAsk1; i++)
        {
            RandQuestionsOrder1[i] = i;
        }
        for (int i = 0; i < NoOfQuestionsToAsk2; i++)
        {
            RandQuestionsOrder2[i] = i;
        }
        for (int i = 0; i < NoOfQuestionsToAsk3; i++)
        {
            RandQuestionsOrder3[i] = i;
        }
        for (int i = 0; i < NoOfQuestionsToAsk4; i++)
        {
            RandQuestionsOrder4[i] = i;
        }

        if (Is_NeedRandomizedQuestions)
        {
            RandQuestionsOrder1 = RandomArray_Int(RandQuestionsOrder1);
            RandQuestionsOrder2 = RandomArray_Int(RandQuestionsOrder2);
            RandQuestionsOrder3 = RandomArray_Int(RandQuestionsOrder3);
            RandQuestionsOrder4 = RandomArray_Int(RandQuestionsOrder4);
        }

        QuestionOrderList1 = new List<int>();
        QuestionOrderList2 = new List<int>();
        QuestionOrderList3 = new List<int>();
        QuestionOrderList4 = new List<int>();

        QuestionsOrder1 = new int[NoOfQuestionsToAsk1];
        QuestionsOrder2 = new int[NoOfQuestionsToAsk2];
        QuestionsOrder3 = new int[NoOfQuestionsToAsk3];
        QuestionsOrder4 = new int[NoOfQuestionsToAsk4];

        for (int i = 0; i < NoOfQuestionsToAsk1; i++)
        {
            QuestionsOrder1[i] = TotalNumofQuestions1[RandQuestionsOrder1[i]];
        }
        for (int i = 0; i < NoOfQuestionsToAsk2; i++)
        {
            QuestionsOrder2[i] = TotalNumofQuestions2[RandQuestionsOrder2[i]];
        }
        for (int i = 0; i < NoOfQuestionsToAsk3; i++)
        {
            QuestionsOrder3[i] = TotalNumofQuestions3[RandQuestionsOrder3[i]];
        }
        for (int i = 0; i < NoOfQuestionsToAsk4; i++)
        {
            QuestionsOrder4[i] = TotalNumofQuestions4[RandQuestionsOrder4[i]];
        }

        for (int i = 0; i < NoOfQuestionsToAsk1; i++)
        {
            QuestionOrderList1.Add(RandQuestionsOrder1[i]);
        }
        for (int i = 0; i < NoOfQuestionsToAsk2; i++)
        {
            QuestionOrderList2.Add(RandQuestionsOrder2[i]);
        }
        for (int i = 0; i < NoOfQuestionsToAsk3; i++)
        {
            QuestionOrderList3.Add(RandQuestionsOrder3[i]);
        }
        for (int i = 0; i < NoOfQuestionsToAsk4; i++)
        {
            QuestionOrderList4.Add(RandQuestionsOrder4[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));

        CurrentItems = new GameObject[3];

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;

        LevelsHolder.gameObject.SetActive(true);
        for (int i = 0; i < QuestionsHolder.Length; i++)
        {
            QuestionsHolder[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if(CurrentItems[i]!=null)
            Destroy(CurrentItems[i].gameObject);
        }         

        TutorialObj.gameObject.SetActive(false);

        QuestionOrderListtemp = new List<int>();
        for (int i = 0; i < 20; i++)
        {
            QuestionOrderListtemp.Add(i+1);
        }

        if (QuestionOrder1 < (QuestionOrderList1.Count))
        {
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
        if (QuestionOrder3 < (QuestionOrderList3.Count))
        {
            tempq = QuestionOrder3;
            tempq = RandQuestionOrder3;
            QuestionOrderListtemp.Remove(QuestionsOrder3[QuestionOrder3]);
            CurrentQuestion = QuestionsOrder3[QuestionOrder3];
            RandCurrentQuestion = RandQuestionsOrder3[QuestionOrder3];
            Debug.Log("Question No : " + QuestionOrder3 + " A : " + QuestionsOrder2[QuestionOrder3]);
            CurrentTempQuestion = QuestionOrder3;
            CurrentItem = 2;
            QuestionOrder3++;
        }
        else
        if (QuestionOrder4 < (QuestionOrderList4.Count))
        {
            tempq = QuestionOrder4;
            tempq = RandQuestionOrder4;
            QuestionOrderListtemp.Remove(QuestionsOrder4[QuestionOrder4]);
            CurrentQuestion = QuestionsOrder4[QuestionOrder4];
            RandCurrentQuestion = RandQuestionsOrder4[QuestionOrder4];
            Debug.Log("Question No : " + QuestionOrder4 + " A : " + QuestionsOrder4[QuestionOrder4]);
            CurrentTempQuestion = QuestionOrder4;
            CurrentItem = 3;
            QuestionOrder4++;
        }
        else
        if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            TargetArray = QuestionsOrder1;
            CurrentTempQuestion = FindIndexofElementinArray(CurrentQuestion);
            RandQuestionOrder1 = RandQuestionsOrder1[CurrentTempQuestion];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder1 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            CurrentItem = 0;
            RandCurrentQuestion = RandQuestionsOrder1[CurrentTempQuestion];
            WrongAnsweredQuestionOrder1++;
        }
        else
        if (WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
        {
            tempq = WrongAnsweredQuestionOrder2;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[tempq]);
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            TargetArray = QuestionsOrder2;
            CurrentTempQuestion = FindIndexofElementinArray(CurrentQuestion);
            RandQuestionOrder2 = RandQuestionsOrder2[CurrentTempQuestion];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            CurrentItem = 1;
            RandCurrentQuestion = RandQuestionsOrder2[CurrentTempQuestion];
            WrongAnsweredQuestionOrder2++;
        }
        else
        if (WrongAnsweredQuestionOrder3 < (WrongAnsweredQuestions3.Count))
        {
            tempq = WrongAnsweredQuestionOrder3;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions3[tempq]);
            CurrentQuestion = WrongAnsweredQuestions3[tempq];
            TargetArray = QuestionsOrder3;
            CurrentTempQuestion = FindIndexofElementinArray(CurrentQuestion);
            RandQuestionOrder3 = RandQuestionsOrder3[CurrentTempQuestion];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder3 + " A : " + WrongAnsweredQuestions3[WrongAnsweredQuestionOrder3]);
            CurrentItem = 2;
            RandCurrentQuestion = RandQuestionsOrder3[CurrentTempQuestion];
            WrongAnsweredQuestionOrder3++;
        }
        else
        if (WrongAnsweredQuestionOrder4 < (WrongAnsweredQuestions4.Count))
        {
            tempq = WrongAnsweredQuestionOrder4;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions4[tempq]);
            CurrentQuestion = WrongAnsweredQuestions4[tempq];
            TargetArray = QuestionsOrder4;
            CurrentTempQuestion = FindIndexofElementinArray(CurrentQuestion);
            RandQuestionOrder4 = RandQuestionsOrder4[CurrentTempQuestion];
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder4 + " A : " + WrongAnsweredQuestions4[WrongAnsweredQuestionOrder4]);
            CurrentItem = 3;
            RandCurrentQuestion = RandQuestionsOrder4[CurrentTempQuestion];
            WrongAnsweredQuestionOrder4++;
        }

        float _delay1 = 0;
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                AnswerObjs[i].GetComponent<Text>().text = ""+CurrentQuestion;
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

            iTween.ScaleTo(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.one*0.5f, "time", 1f, "delay", _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay1));
            _delay1 += 0.1f;
        }       

        CurrentItems = new GameObject[20];       
        
        int _tempCurrentItem = 0;
        
        List<int> _tempList = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            _tempList.Add(i);
        }
        _tempList.Remove(CurrentItem);

        //PICKING RANDOM POSITIONS FROM 20 //
        int[] _tempArray = new int[CurrentQuestion];
        int[] _tempArray20 = new int[20];
        for (int i = 0; i < _tempArray20.Length; i++)
        {
            _tempArray20[i] = i;
        }
        _tempArray = RandomArray_Int(_tempArray20, _tempArray.Length);
        //PICKING RANDOM POSITIONS FROM 20 //

        for (int i = 0; i < 20; i++)
        {
            if (_tempArray.Contains(i))
            {
                _tempCurrentItem = CurrentItem;
            }
            else
            {
                _tempCurrentItem = RandomNoFromList_Int(_tempList);
            }          

            if (i<7)
            {
                QuestionsHolder[0].gameObject.SetActive(true);
                CurrentItems[i] = Instantiate(QuestionItems[_tempCurrentItem], QuestionsHolder[0].transform);
            }
            if (i >=7 && i<14)
            {
                QuestionsHolder[1].gameObject.SetActive(true);
                CurrentItems[i] = Instantiate(QuestionItems[_tempCurrentItem], QuestionsHolder[1].transform);
            }
            if (i >=14 && i<20)
            {
                QuestionsHolder[2].gameObject.SetActive(true);
                CurrentItems[i] = Instantiate(QuestionItems[_tempCurrentItem], QuestionsHolder[2].transform);
            }            
        }

        float _delay = 0;
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            iTween.ScaleFrom(CurrentItems[i].gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", _delay, "easetype", iTween.EaseType.easeInOutSine));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        Is_OkButtonPressed = false;

        Invoke("PlayQuestionVoiceOverWithDelay", CurrentItems.Length*0.1f); 
    }

    void PlayQuestionVoiceOverWithDelay()
    {
        PlayQuestionVoiceOver(CurrentItem);
        Invoke("EnableOptionsRaycast", QVOLength);
        iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("y", 0, "time", 1f, "delay", QVOLength, "easetype", iTween.EaseType.easeOutElastic));
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

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength = 0;

        if (VOLanguage == 0)
        {
            ClipLength = Sound_AVO[_Ai].EN_Sound_AO[RandCurrentQuestion].clip.length;
            PlayAudio(Sound_AVO[_Ai].EN_Sound_AO[RandCurrentQuestion], _delay);
            Debug.Log("Sound : " + _Ai);
        }
        if (VOLanguage == 1)
        {
            ClipLength = Sound_AVO[_Ai].HI_Sound_AO[RandCurrentQuestion].clip.length;
            PlayAudio(Sound_AVO[_Ai].HI_Sound_AO[RandCurrentQuestion], _delay);
        }
        if (VOLanguage == 2)
        {
            ClipLength = Sound_AVO[_Ai].TL_Sound_AO[RandCurrentQuestion].clip.length;
            PlayAudio(Sound_AVO[_Ai].TL_Sound_AO[RandCurrentQuestion], _delay);
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
        StartCoroutine("PlayAudioRepeatedSingleCall");
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
            //INGAME_COMMON
            //MultiLevelManager.instance.UpdateProgress(1, 1);
            //INGAME_COMMON
            WrongAnsrsCount = 0;

            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length+0.25f) + Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentItem, LengthDelay + 0.25f);

            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);
            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);

            StartCoroutine(SetActiveWithDelayCall(LevelsHolder, false, LengthDelay + LengthDelay2 + 2f));

            iTween.MoveTo(OptionsHolder.gameObject, iTween.Hash("y", -250, "time", 1f, "delay", LengthDelay + 1.5f, "easetype", iTween.EaseType.easeOutElastic));

            Invoke("HighlightCorrectOption", LengthDelay);

            if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                QuestionOrder2 < (QuestionOrderList2.Count) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count) ||
                QuestionOrder3 < (QuestionOrderList3.Count) ||
                WrongAnsweredQuestionOrder3 < (WrongAnsweredQuestions3.Count) ||
                QuestionOrder4 < (QuestionOrderList4.Count) ||
                WrongAnsweredQuestionOrder4 < (WrongAnsweredQuestions4.Count))
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

            for (int i = 0; i < AnswerObjs.Length; i++)
            {
                if (AnswerObjs[i].name.Contains(UserAnsr.ToString()))
                {
                    iTween.ShakePosition(AnswerObjs[i].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                }
            }

            PlayAudio(Sound_IncorrectAnswer, 0.4f);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentItem, 1f);

                Invoke("HighlightCorrectOption", 1);

                for (int i = 0; i < AnswerObjs.Length; i++)
                {
                    if (AnswerObjs[i] != null)
                    {
                        AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
                    }
                }

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionOrderList1.Count)
                    && CurrentItem == 0 )
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionOrderList2.Count)
                    && CurrentItem == 1)
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions3.Contains(CurrentQuestion) && QuestionOrder3 <= (QuestionOrderList3.Count)
                     && CurrentItem == 2)
                {
                    WrongAnsweredQuestions3.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions4.Contains(CurrentQuestion) && QuestionOrder4<= (QuestionOrderList4.Count) 
                    && CurrentItem == 3)
                {
                    WrongAnsweredQuestions4.Add(CurrentQuestion);
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                    //INGAME_COMMON
                    //MultiLevelManager.instance.UpdateProgress(1, 0);
                    //INGAME_COMMON
                }

                if (QuestionOrder1 < (QuestionOrderList1.Count) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                    QuestionOrder2 < (QuestionOrderList2.Count) ||
                    WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count) ||
                    QuestionOrder3 < (QuestionOrderList3.Count) ||
                    WrongAnsweredQuestionOrder3 < (WrongAnsweredQuestions3.Count) ||
                    QuestionOrder4 < (QuestionOrderList4.Count) ||
                    WrongAnsweredQuestionOrder4 < (WrongAnsweredQuestions4.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + 2.5f);
                }
                else
                {
                    Debug.Log("Game Over W");
                    StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + 2f));
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

    void HighlightCorrectOption()
    {
        for (int i = 0; i < CurrentItems.Length; i++)
        {
            if (CurrentItems[i].gameObject.name.Contains((CurrentItem+1).ToString()))
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StartAnim();
            }
            else
            {
                CurrentItems[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

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

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }
    #region INGAME_COMMON

    public void ShowLC()
    {
        if (!MultiLevelManager.instance.CheckForNextLevel())
        {
            InGameManager.instance.Activity_Finished(MultiLevelManager.instance.Total_Questions, MultiLevelManager.instance.Total_CorrectAnswers);
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

    public QVO_AudioSource_B01032[] Sound_QVO;
    public AVO_AudioSource_B01032[] Sound_AVO;

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

    #region RANDOMIZE AN ARRAY
    public static int[] RandomArray_Int(int[] _SourceArray,int _count)
    {
        int _tempCount = 0;
        int[] _returnArray = new int[_count];
        for (int i = 0; i < _SourceArray.Length; i++)
        {
            int tmp = _SourceArray[i];
            int rand = Random.Range(i, _SourceArray.Length);
            if (_tempCount < _count)
            {
                _tempCount++;
                _SourceArray[i] = _SourceArray[rand];
                _SourceArray[rand] = tmp;
                _returnArray[i] = _SourceArray[i];
            }            
        }
        return _returnArray;
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
    public static int RandomNoFromArray_Int(int[] _SourceList)
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

    [System.Serializable]
    public class AVO_AudioSource_B01032
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_B01032
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }
}
