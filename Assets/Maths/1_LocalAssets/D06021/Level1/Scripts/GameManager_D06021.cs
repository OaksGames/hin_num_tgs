using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_D06021 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    public static GameManager_D06021 Instance;
    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject TutBtn_Okay;

    public GameObject Tut_GemHolder;
    public GameObject Tut_GemObj;
    public GameObject Tut_ShovelObj;
    public GameObject Tut_EquationHolder;

    public int Tut_Question;
    public int Tut_Question1;

    public GameObject[] Tut_CurrentGems;
    public GameObject[] Tut_GemSlots;
    public float[] Tut_CartTargetPositions;

    public GameObject Tut_OptionHolder;
    public GameObject TutHand1, TutHand2, TutHand3, TutHand4;

    public int Tut_EachCartMaxCount;
    public GameObject[] Tut_AnswerObjs;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public int NoOfQuestionsToAsk;

    public GameObject LevelObj;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;
    public GameObject GemHolder;
    public GameObject GemObj;
    public GameObject ShovelObj;
    public GameObject EquationHolder;

    public GameObject[] CurrentGems;
    public GameObject[] GemSlots;
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
            AddValueInProgress = 1 / (float)(NoOfQuestionsToAsk * 3);
            Thisgamekey = gameInputData.Key;
        }

        LevelObj.gameObject.SetActive(false);
        TutorialObj.gameObject.SetActive(true);
        PlayAudio(Sound_Intro1, 1f);
        Tut_CurrentGems = new GameObject[Tut_Question];
        float _delay = 0;
        for (int i = 0; i < Tut_Question; i++)
        {
            Tut_CurrentGems[i] = Instantiate(Tut_GemObj.gameObject, Vector3.zero, Quaternion.identity, Tut_GemHolder.transform);

            iTween.ScaleFrom(Tut_CurrentGems[i].gameObject, iTween.Hash("Scale", 0.5f, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));

            _delay += 0.1f;
        }
        Invoke("Tut_GemsPoping", 0);
        Invoke("EnableAnimator", 6f);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length + 0.5f);
        Is_CanTapTut_Shovel = true;
        Tut_EachCartMaxCount = Tut_Question / Tut_Question1;
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
    public void DisableTutRaycast()
    {
        Tut_OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = false;
    }
    void Tut_ShovelEnable()
    {
        Tut_ShovelObj.GetComponent<Image>().raycastTarget = true;
    }
    public void Tut_GemsPoping()
    {
        float _delay2 = 0;
        for (int i = 0; i < Tut_CurrentGems.Length; i++)
        {
            iTween.ScaleFrom(Tut_CurrentGems[i].gameObject, iTween.Hash("scale", Vector3.zero, "islocal", true, "time", 0.5f, "delay", _delay2, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay2));
            _delay2 += 0.1f;
        }
    }
    public void CallIntro2()
    {
        PlayAudio(Sound_Intro2, 0.5f);
        Invoke("CallIntro3", Sound_Intro2[VOLanguage].clip.length + 0.5f);      
    }
    public void CallIntro3()
    {
        StopAudio(Sound_Intro2);
        PlayAudioRepeated(Sound_Intro3);  
        Tut_AnswerObjs[0].GetComponent<Text>().text = "" + 4;
        Tut_AnswerObjs[1].GetComponent<Text>().text = "" + 8;
        Tut_AnswerObjs[2].GetComponent<Text>().text = "" + 6;
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 1f, "delay", 0f, "easetype", iTween.EaseType.linear));
        Invoke("EnableTutRaycast", Sound_Intro3[VOLanguage].clip.length);
    }
    public void CallIntro4()
    {
        StopAudio(Sound_Intro3);
        PlayAudio(Sound_Intro4, 0.1f);
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 1512, "islocal", true, "time", 1f, "delay", 0f, "easetype", iTween.EaseType.linear));
        Invoke("CallIntro5", Sound_Intro4[VOLanguage].clip.length + 1f);
        Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", 0.5f);
        Invoke("DisableTutRaycast", 0.2f);
        
        TutHand2.gameObject.SetActive(true);
    }
    public void CallIntro5()
    {
        StopAudio(Sound_Intro4);
        PlayAudioRepeated(Sound_Intro5);
        Tut_AnswerObjs[0].GetComponent<Text>().text = "" + 2;
        Tut_AnswerObjs[1].GetComponent<Text>().text = "" + 4;
        Tut_AnswerObjs[2].GetComponent<Text>().text = "" + 3;
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 1f, "delay", 0f, "easetype", iTween.EaseType.linear));      
        Invoke("EnableTutRaycast", Sound_Intro5[VOLanguage].clip.length);
        
    }
    public void CallIntro6()
    {
        StopAudio(Sound_Intro5);
        PlayAudio(Sound_Intro6, 0.1f);
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 1512, "islocal", true, "time", 1f, "delay", 0f, "easetype", iTween.EaseType.linear));
        Invoke("CallIntro7", Sound_Intro6[VOLanguage].clip.length + 0.5f);
        Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", 0.5f);
        Invoke("DisableTutRaycast", 0.2f);
        TutHand3.gameObject.SetActive(true);
    }
    public void CallIntro7()
    {
        StopAudio(Sound_Intro6);
        PlayAudioRepeated(Sound_Intro7);
        Invoke("CallIntro8", Sound_Intro7[VOLanguage].clip.length + 0.5f);
        Invoke("Tut_ShovelEnable", Sound_Intro7[VOLanguage].clip.length);

    }
    public void CallIntro8()
    {
        StopRepetedAudio();
        StopAudio(Sound_Intro7);
        PlayAudioRepeated(Sound_Intro8);
        TutHand3.gameObject.SetActive(false);
    }
    public void CallIntro9()
    {
        StopRepetedAudio();
        StopAudio(Sound_Intro8);
        PlayAudio(Sound_Intro9,0.5f);
        Invoke("CallIntro10", Sound_Intro9[VOLanguage].clip.length + 0.2f);
        TutHand4.gameObject.SetActive(true);
    }
    public void CallIntro10()
    {
        StopAudio(Sound_Intro7);
        PlayAudioRepeated(Sound_Intro10);
        Tut_AnswerObjs[0].GetComponent<Text>().text = "" + 3;
        Tut_AnswerObjs[1].GetComponent<Text>().text = "" + 2;
        Tut_AnswerObjs[2].GetComponent<Text>().text = "" + 4;
        iTween.MoveTo(Tut_OptionHolder.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 1f, "delay", 0f, "easetype", iTween.EaseType.linear));
        Invoke("EnableTutRaycast", Sound_Intro10[VOLanguage].clip.length);
    }
    public void CallIntro11()
    {
        StopRepetedAudio();
        StopAudio(Sound_Intro10);
        PlayAudio(Sound_Intro11,0.2f);
        Invoke("CallIntro12", Sound_Intro11[VOLanguage].clip.length + 0.2f);
    }
    public void CallIntro12()
    {
        StopRepetedAudio();
        StopAudio(Sound_Intro11);
        PlayAudio(Sound_Intro12, 0.2f);
        PlayAudio(Sound_CorrectAnswer, Sound_Intro12[VOLanguage].clip.length + 1f);
        iTween.ScaleTo(Tut_EquationHolder.transform.GetChild(0).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay", LengthDelay + 1.25f, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleTo(Tut_EquationHolder.transform.GetChild(0).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay", LengthDelay + 1.5f, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleTo(Tut_EquationHolder.transform.GetChild(1).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay", LengthDelay + 2f, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleTo(Tut_EquationHolder.transform.GetChild(1).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay", LengthDelay + 2.25f, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleTo(Tut_EquationHolder.transform.GetChild(2).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay", LengthDelay + 2.75f, "easetype", iTween.EaseType.easeOutElastic));
        iTween.ScaleTo(Tut_EquationHolder.transform.GetChild(2).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay", LengthDelay + 3f, "easetype", iTween.EaseType.easeOutElastic));
        Tween_TickMark.myScript.Invoke("Tween_In", Sound_Intro12[VOLanguage].clip.length + 1f);
        Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().Invoke("StopAnim", Sound_Intro12[VOLanguage].clip.length + 1.5f);
        PlayAudio(Sound_Intro13, Sound_Intro12[VOLanguage].clip.length + 1.5f);
        Invoke("SetGamePlay", Sound_Intro12[VOLanguage].clip.length + Sound_Intro13[VOLanguage].clip.length+ 2.5f);
    }

    int _tempgemcount1;
    int _slotCount1;
    GameObject _tempGem1;
    bool Is_CanTapTut_Shovel;
    public void Tut_ShovelTap()
    {
        if (Is_CanTapTut_Shovel)
            StartCoroutine("Tut_ShovelTapAct");
    }
    public IEnumerator Tut_ShovelTapAct()
    {
        Is_CanTapTut_Shovel = false;
        PlayAudio(Sound_Selection, 0);
        yield return new WaitForSeconds(0);
        PlayAudio(Sound_Selection, 0);
        if (_tempgemcount1 < Tut_Question)
        {
            _tempGem1 = Tut_CurrentGems[_tempgemcount1];

            iTween.MoveTo(Tut_CurrentGems[_tempgemcount1].gameObject, iTween.Hash("position", Tut_GemSlots[_slotCount1].transform.position, "time", 0.25f, "delay", 0, "easetype", iTween.EaseType.linear));
            Tut_CurrentGems[_tempgemcount1].GetComponent<Image>().enabled = true;
            Invoke("Tut_Set_TempGemParent", 0.3f);

            _tempgemcount1++;

            if (_tempgemcount1 == Tut_Question)
            {
                StopRepetedAudio();
                Invoke("CallIntro9", 0.1f);
                Tut_ShovelObj.GetComponent<Image>().raycastTarget = false;
            }
            yield return new WaitForSeconds(0.3f);
            Is_CanTapTut_Shovel = true;
        }
    }
    
    void Tut_Set_TempGemParent()
    {
       
       _tempGem1.transform.parent = Tut_GemSlots[_slotCount1].transform.GetChild(0);
        Tut_CurrentGems[_tempgemcount1-1].GetComponent<Image>().enabled = false;

        if (_tempgemcount1 % Tut_EachCartMaxCount == 0)
        {
            _slotCount1++;
        }
    }
    int TutAnswer;
    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        PlayAudio(Sound_Selection, 0);
        if(TutAnswer==0)
        {
            TutHand1.gameObject.SetActive(false);
            StopRepetedAudio();
            StopAudio(Sound_Intro3);
            TutAnswer++;
            Tut_OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = false;
            Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();
            Tut_EquationHolder.transform.GetChild(0).gameObject.SetActive(true);
            Invoke("CallIntro4", 0.1f);
        }
        else
        if (TutAnswer ==1)
        {
            TutHand2.gameObject.SetActive(false);
            StopRepetedAudio();
            StopAudio(Sound_Intro3);
            TutAnswer++;
            Tut_OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = false;
            Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();
            Tut_EquationHolder.transform.GetChild(1).gameObject.SetActive(true);
            Invoke("CallIntro6",  0.1f);
        }
        else
        if (TutAnswer == 2)
        {
            StopRepetedAudio();
            StopAudio(Sound_Intro10);
            Tut_OptionHolder.transform.GetChild(1).gameObject.GetComponent<Text>().raycastTarget = false;
            Tut_OptionHolder.transform.GetChild(1).GetComponent<PopTweenCustom>().StartAnim();
            Tut_EquationHolder.transform.GetChild(2).gameObject.SetActive(true);
            Invoke("CallIntro11",0.2f);
            TutHand4.gameObject.SetActive(false);
        }
    }
       
    #endregion

    #region LEVEL
    public void SetGamePlay()
    {
        TutorialObj.gameObject.SetActive(false);
        LevelObj.gameObject.SetActive(true);

       // ProgreesBar.GetComponent<Slider>().maxValue = NoOfQuestionsToAsk;

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
        //-------------------------DEBUGGING--------------------------------------
        for (int i = 0; i < QuestionOrderList.Count; i++)
        {
            Debug.Log("QuesOrdList:::" + QuestionOrderList[i].ToString());
        }
        //------------------------------------------------------------------------

        //--------Forcefully Removing Repeated Elements from QuesKeys--------------
            QuesKeys = QuesKeys.Distinct().ToList();
        //-------------------------------------------------------------------------


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
        GemSlots[0].transform.parent.GetComponent<GridLayoutGroup>().enabled = true;
        ShovelObj.GetComponent<Image>().raycastTarget = false;

        for (int i = 0; i < EquationHolder.transform.childCount; i++)
        {
            EquationHolder.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < CurrentGems.Length; i++)
        {
            Destroy(CurrentGems[i]);
        }

        CurrentGems = new GameObject[0];

        int tempq = 0;
        CountOk = 0;
        _tempgemcount=0;
        _slotCount=0;

        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            if (AnswerObjs[i] != null)
            {
                AnswerObjs[i].GetComponent<Text>().text = "";
                AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
                AnswerObjs[i].GetComponent<PopTweenCustom>().StopAnim();
            }
        }

        for (int i = 0; i < GemSlots.Length; i++)
        {
            GemSlots[i].gameObject.SetActive(false);
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
            _delay3 += 0.1f;
        }

        EachCartMaxCount = CurrentQuestion / CurrentQuestion2;
        Debug.Log("EachCartMaxCount : " + EachCartMaxCount);

        EquationHolder.transform.GetChild(0).GetComponent<Text>().text = "" + CurrentQuestion;
        EquationHolder.transform.GetChild(1).GetComponent<Text>().text = "" + CurrentQuestion2;
        EquationHolder.transform.GetChild(2).GetComponent<Text>().text = "" + EachCartMaxCount;

        _tempQopt = CurrentQuestion;
        StartCoroutine("SetAnswerOptions", 1);
        Invoke("GemsPoping", 0);
        iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.linear));

        for (int i = 0; i < CurrentQuestion2; i++)
        {
            GemSlots[i].gameObject.SetActive(true);
        }

        Is_OkButtonPressed = false;

        Invoke("PlayQuestionVO1", 1);

        Invoke("EnableOptionsRaycast", QVOLength);
        Debug.Log("Question  : " + CurrentQuestion+" Cart Count : "+CurrentQuestion2);

        Is_CanTapShovel = true;
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
        float _delay1 = 0;
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

            iTween.ScaleFrom(AnswerObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero,"time", 1f, "delay", _delay1, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay1));
            _delay1 += 0.1f;
        }
    }

    void EnableShovel()
    {
        ShovelObj.GetComponent<Image>().raycastTarget = true;
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = true;
        }
        Debug.Log("EnableOptionsRaycast Here");
    }
    void DisbleOptionsRaycast()
    {
        for (int i = 0; i < AnswerObjs.Length; i++)
        {
            AnswerObjs[i].GetComponent<Text>().raycastTarget = false;
        }
        Debug.Log("EnableOptionsRaycast Here");
    }

    void PlayQuestionVO1()
    {
        PlayQuestionVO1(Random.Range(0, 2));
       
    }

    void PlayQuestionVO1(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO1.EN_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO1.EN_Sound_QO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO1.HI_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO1.HI_Sound_QO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO1.TL_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO1.TL_Sound_QO[_Qi]);
                break;
        }
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void PlayQuestionVO2()
    {
        PlayQuestionVO2(Random.Range(0,2));
    }
    void PlayQuestionVO2(int _Qi)
    {
        switch (VOLanguage)
        {
            case 0:
                QVOLength = Sound_QVO2.EN_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO2.EN_Sound_QO[_Qi]);
                break;
            case 1:
                QVOLength = Sound_QVO2.HI_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO2.HI_Sound_QO[_Qi]);
                break;
            case 2:
                QVOLength = Sound_QVO2.TL_Sound_QO[_Qi].clip.length;
                PlayAudioRepeated(Sound_QVO2.HI_Sound_QO[_Qi]);
                break;
        }
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void PlayQuestionVO3()
    {
        PlayQuestionVO3(0);
        Invoke("EnableShovel", QVOLength);

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

    public float PlayAnswerVoiceOver1(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO1.EN_Sound_AO[_Ai-1], _delay);
        ClipLength = Sound_AVO1.EN_Sound_AO[_Ai-1].clip.length;

        return ClipLength;
    }
    public float PlayAnswerVoiceOver2(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO2.EN_Sound_AO[_Ai - 1], _delay);
        ClipLength = Sound_AVO2.EN_Sound_AO[_Ai - 1].clip.length;

        return ClipLength;
    }
    public float PlayAnswerVoiceOver3(int _Ai, float _delay)
    {
        float ClipLength = 0;

        PlayAudio(Sound_AVO3.EN_Sound_AO[_Ai ], _delay);
        ClipLength = Sound_AVO3.EN_Sound_AO[_Ai ].clip.length;

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
            Invoke("DisbleOptionsRaycast", 0.1f);
            WrongAnsrsCount = 0;
            if (CountOk == 0)
            {
                CountOk++;

                Invoke("HighlightOptions", 0.5f);
                LengthDelay = PlayAnswerVoiceOver1(CurrentQuestion, 0);
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 1000, "islocal", true, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.linear));

                EquationHolder.transform.GetChild(0).gameObject.SetActive(true);
                iTween.ScaleFrom(EquationHolder.transform.GetChild(0).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));

                Is_OkButtonPressed = false;
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("PlayQuestionVO2", LengthDelay+2f);
               
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 1f, "delay", LengthDelay + 1, "easetype", iTween.EaseType.easeOutElastic));

                _tempQopt = CurrentQuestion2;
                StartCoroutine("SetAnswerOptions", LengthDelay+1.2f);

            }
            else
            if (CountOk == 1)
            {
                CountOk++;

                LengthDelay = PlayAnswerVoiceOver2(CurrentQuestion2, 0);
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 1000, "islocal", true, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.linear));

                EquationHolder.transform.GetChild(1).gameObject.SetActive(true);
                iTween.ScaleFrom(EquationHolder.transform.GetChild(1).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));

                Is_OkButtonPressed = false;
                CancelInvoke("RepeatQVOAftertChoosingOption");
                Invoke("PlayQuestionVO3", LengthDelay);
                Invoke("DisbleOptionsRaycast", 0.1f);

                GemHolder.GetComponent<GridLayoutGroup>().enabled = false;
                GemSlots[0].transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
               
            }
         
            else
            {
                //ProgreesBar.GetComponent<Slider>().value += 1;
                Total_CorrectAnswers++;//INGAME_COMMON


                EquationHolder.transform.GetChild(2).gameObject.SetActive(true);
                iTween.ScaleFrom(EquationHolder.transform.GetChild(2).gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));

                Invoke("HighlightOptions", 0.5f);
                LengthDelay = PlayAppreciationVoiceOver(0.25f);

                iTween.ScaleTo(EquationHolder.transform.GetChild(0).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay", LengthDelay + 1f, "easetype", iTween.EaseType.easeOutElastic));
                iTween.ScaleTo(EquationHolder.transform.GetChild(0).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay", LengthDelay + 1.25f, "easetype", iTween.EaseType.easeOutElastic));
                iTween.ScaleTo(EquationHolder.transform.GetChild(1).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay", LengthDelay + 1.75f, "easetype", iTween.EaseType.easeOutElastic));
                iTween.ScaleTo(EquationHolder.transform.GetChild(1).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay", LengthDelay + 2f, "easetype", iTween.EaseType.easeOutElastic));
                iTween.ScaleTo(EquationHolder.transform.GetChild(2).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay", LengthDelay + 2.5f, "easetype", iTween.EaseType.easeOutElastic));
                iTween.ScaleTo(EquationHolder.transform.GetChild(2).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay", LengthDelay + 2.75f, "easetype", iTween.EaseType.easeOutElastic));

                float LengthDelay2 = PlayAnswerVoiceOver3(CurrentQuestionOrder, LengthDelay + 0.5f);
                PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.75f);
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 1000, "islocal", true, "time", 1f, "delay", LengthDelay + LengthDelay2 + 2.5f, "easetype", iTween.EaseType.linear));
                Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.75f);

                if (QuestionOrder1 < (QuestionOrderList.Count) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
                {
                    Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 2.5f);
                }
                else
                {
                    Debug.Log("Questions Finished");
                    // StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + LengthDelay2 + 3f));
                    // Invoke("ShowLC", LengthDelay + LengthDelay2 + 4f);
                    SendResultFinal();
                }
                CancelInvoke("RepeatQVOAftertChoosingOption");
            }

            Debug.Log("CountOk : " + CountOk);
           
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
                Invoke("DisbleOptionsRaycast", 0.1f);
                if (CountOk == 0)
                {
                    LengthDelay = PlayAnswerVoiceOver1(CurrentQuestion, 1f);
                }
                else
                if (CountOk == 1)
                {
                    LengthDelay = PlayAnswerVoiceOver2(CurrentQuestion2, 1f);
                }
                else
                if (CountOk == 2)
                {
                    EquationHolder.transform.GetChild(2).gameObject.SetActive(true);
                    LengthDelay = PlayAnswerVoiceOver3(CurrentQuestionOrder, 1f);
                    iTween.ScaleTo(EquationHolder.transform.GetChild(0).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay", 1.25f, "easetype", iTween.EaseType.easeOutElastic));
                    iTween.ScaleTo(EquationHolder.transform.GetChild(0).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay",  1.5f, "easetype", iTween.EaseType.easeOutElastic));
                    iTween.ScaleTo(EquationHolder.transform.GetChild(1).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay",  2f, "easetype", iTween.EaseType.easeOutElastic));
                    iTween.ScaleTo(EquationHolder.transform.GetChild(1).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay",  2.25f, "easetype", iTween.EaseType.easeOutElastic));
                    iTween.ScaleTo(EquationHolder.transform.GetChild(2).gameObject, iTween.Hash("scale", new Vector3(1.25f, 1.25f, 1f), "time", 0.5f, "delay",  2.75f, "easetype", iTween.EaseType.easeOutElastic));
                    iTween.ScaleTo(EquationHolder.transform.GetChild(2).gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay",  3f, "easetype", iTween.EaseType.easeOutElastic));
                }

                Invoke("HighlightOptions", 1f);
                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 1000, "islocal", true, "time", 0.25f, "delay", LengthDelay, "easetype", iTween.EaseType.linear));
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
                    //StartCoroutine(SetActiveWithDelayCall(LevelObj, false, LengthDelay + 2f));
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

    int _tempgemcount;
    int _slotCount;
    GameObject _tempGem;
    bool Is_CanTapShovel;
    public void ShovelTap()
    {
        if (Is_CanTapShovel)
            StartCoroutine("ShovelTapAct");
    }

    public IEnumerator ShovelTapAct()
    {
        PlayAudio(Sound_Selection, 0);
        Is_CanTapShovel = false;
        yield return new WaitForSeconds(0);
        if (_tempgemcount < CurrentQuestion)
        {
            _tempGem = CurrentGems[_tempgemcount];
            iTween.MoveTo(CurrentGems[_tempgemcount].gameObject, iTween.Hash("position", GemSlots[_slotCount].transform.position, "time", 0.25f, "delay", 0, "easetype", iTween.EaseType.linear));
            CurrentGems[_tempgemcount].GetComponent<Image>().enabled = true;
            Invoke("Set_TempGemParent", 0.3f);
            _tempgemcount++;

            if (_tempgemcount == CurrentQuestion)
            {

                //StartCoroutine(SetOk_Button(true, 0));
                StopRepetedAudio();
                ShovelObj.GetComponent<Image>().raycastTarget = false;
                Invoke("PlayQuestionVO4",1f);

                iTween.MoveTo(SidePanel.gameObject, iTween.Hash("x", 512, "islocal", true, "time", 1f, "delay", 0, "easetype", iTween.EaseType.easeOutElastic));
                _tempQopt = EachCartMaxCount;
                StartCoroutine("SetAnswerOptions", 0.3f);
            }
            yield return new WaitForSeconds(0.3f);
            Is_CanTapShovel = true;

        }
    }

    void Set_TempGemParent()
    {
        _tempGem.transform.parent = GemSlots[_slotCount].transform.GetChild(0);
        CurrentGems[_tempgemcount-1].GetComponent<Image>().enabled = false;

        if (_tempgemcount % EachCartMaxCount == 0)
        {
            _slotCount++;
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
    public AudioSource[] Sound_Intro10;
    public AudioSource[] Sound_Intro11;
    public AudioSource[] Sound_Intro12;
    public AudioSource[] Sound_Intro13;

    public QVO_AudioSource_D06021 Sound_QVO1;
    public QVO_AudioSource_D06021 Sound_QVO2;
    public QVO_AudioSource_D06021 Sound_QVO3;
    public QVO_AudioSource_D06021 Sound_QVO4;

    public AVO_AudioSource_D06021 Sound_AVO1;
    public AVO_AudioSource_D06021 Sound_AVO2;
    public AVO_AudioSource_D06021 Sound_AVO3;

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
        Debug.Log("PlayAudioRepeated : " + _audio.name);
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
    public class AVO_AudioSource_D06021
    {
        public AudioSource[] EN_Sound_AO;
        public AudioSource[] HI_Sound_AO;
        public AudioSource[] TL_Sound_AO;
    }

    [System.Serializable]
    public class QVO_AudioSource_D06021
    {
        public AudioSource[] EN_Sound_QO;
        public AudioSource[] HI_Sound_QO;
        public AudioSource[] TL_Sound_QO;
    }

}

