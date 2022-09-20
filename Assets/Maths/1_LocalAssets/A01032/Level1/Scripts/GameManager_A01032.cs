using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Tpix;
using Tpix.ResourceData;

public class GameManager_A01032 : MonoBehaviour, IOAKSGame
{
    public bool FrameworkOff = false;
    public bool Testing = false;

    [Header("=========== TUTORIAL CONTENT============")]
    public bool Is_Tutorial = true;
    public GameObject TutorialObj;
    public GameObject[] TutTrayObjs;
    public GameObject TutBtn_Okay;
    public GameObject TutHand1, TutHand2;

    [Header("=========== GAMEPLAY CONTENT============")]
    public bool Is_NeedRandomizedQuestions;
    public GameObject LevelObj;
    public GameObject LevelHolder;
    public GameObject ProgreesBar;
    public GameObject Btn_Ok, Btn_Ok_Dummy;
    public GameObject LCObj;

    public GameObject[] Trays;
    public Sprite[] TraysItemsSprires;
    public GameObject[] TraysItems;
    public GameObject TrayItemsPrefab;

    [HideInInspector]
    public bool Is_CanClick;

    [HideInInspector]
    public List<int> QuestionOrderList;

    public int[] QuestionsOrder1;
    public int QuestionOrder1;

    public int[] QuestionsOrder2;
    public int QuestionOrder2;

    public List<int> WrongAnsweredQuestions1;
    public int WrongAnsweredQuestionOrder1;

    public List<int> WrongAnsweredQuestions2;
    public int WrongAnsweredQuestionOrder2; 

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

            TotalQues = 6;
            Thisgamekey = "na01032";

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

        TutorialObj.gameObject.SetActive(true);
        LevelObj.gameObject.SetActive(false);       

        float _delay = 0;
        for (int i = 0; i < TutTrayObjs.Length; i++)
        {
            iTween.ScaleFrom(TutTrayObjs[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            StartCoroutine(PlayAudioAtOneShot(Sound_Ting, _delay));
            _delay += 0.1f;
        }

        PlayAudio(Sound_Intro1, 2f);
        Invoke("EnableAnimator", 6);
        Invoke("CallIntro2", Sound_Intro1[VOLanguage].clip.length+2f);
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
    }

    public void Selected_TutAnswer()
    {
        TutorialObj.GetComponent<Animator>().enabled = false;
        TutTrayObjs[1].transform.parent.GetComponent<Image>().raycastTarget = false;
        TutBtn_Okay.gameObject.SetActive(true);
        TutTrayObjs[1].transform.parent.GetComponent<PopTweenCustom>().StartAnim();

        StopAudio(Sound_Intro2);
        StopRepetedAudio();
        PlayAudio(Sound_Selection, 0);
        PlayAudioRepeated(Sound_Intro3);

        TutHand1.gameObject.SetActive(false);
        TutHand2.gameObject.SetActive(true);
    }

    public void BtnAct_OkTut()
    {
        TutTrayObjs[1].transform.parent.GetComponent<PopTweenCustom>().StopAnim();
        TutBtn_Okay.gameObject.SetActive(false);
        StopAudio(Sound_Intro3);
        StopRepetedAudio();
        PlayAudio(Sound_BtnOkClick, 0);
        TutHand2.gameObject.SetActive(false);

        float LengthDelay = PlayAppreciationVoiceOver(0);
        float LengthDelay2 = PlayAnswerVoiceOver(2, LengthDelay);

        PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2);

        Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2);
        TutTrayObjs[1].transform.parent.GetComponent<PopTweenCustom>().Invoke("StopAnim", 0);

        Invoke("Tut_CupCakeAnim", LengthDelay);

        PlayAudio(Sound_Intro4, LengthDelay + LengthDelay2 + 2f);

        Invoke("SetGamePlay", LengthDelay + LengthDelay2 + Sound_Intro4[VOLanguage].clip.length + 3f);
    }

    void Tut_CupCakeAnim()
    {
        TutTrayObjs[1].transform.GetChild(0).transform.GetChild(0).GetComponent<PopTweenCustomWithDelay>().Invoke("StartAnim", 0.1f);
        TutTrayObjs[1].transform.GetChild(0).transform.GetChild(1).GetComponent<PopTweenCustomWithDelay>().Invoke("StartAnim",1);
    }


    #endregion
    public IEnumerator SetImageRaycastTarget(Image _targetImage, float _delay, bool _state)
    {
        yield return new WaitForSeconds(_delay);
        _targetImage.GetComponent<Image>().raycastTarget = _state;
    }

    #region LEVEL
    public void SetGamePlay()
    {
        TutorialObj.gameObject.SetActive(false);

        if (Testing)
        {
            ProgreesBar.GetComponent<Slider>().maxValue = QuestionsOrder1.Length;
            Debug.Log("QuestionsOrder1.Length: " + QuestionsOrder1.Length);
        }

        if (Is_NeedRandomizedQuestions)
        {
            QuestionsOrder1 = RandomArray_Int(QuestionsOrder1);
            QuestionsOrder2 = RandomArray_Int(QuestionsOrder2);
        }

        QuestionOrderList = new List<int>();

        for (int i = 0; i < QuestionsOrder1.Length; i++)
        {
            QuestionOrderList.Add(QuestionsOrder1[i]);
        }

        StartCoroutine(SetOk_Button(false, 0f));

        TraysItems = new GameObject[Trays.Length];

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        LevelObj.gameObject.SetActive(true);
        LevelHolder.gameObject.SetActive(true);

        TraysItems = new GameObject[Trays.Length];
        for (int i = 0; i < Trays.Length; i++)
        {
            TraysItems[i] = Instantiate(TrayItemsPrefab, Trays[i].transform);
        }

        int RandAnsrIndex = Random.Range(0, 3);
        int tempq = 0;

        QuestionOrderListtemp = new List<int>();

        if (QuestionOrder1 < (QuestionsOrder1.Length))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }

            tempq = QuestionOrder1;
            QuestionOrderListtemp.Remove(QuestionsOrder1[tempq]);
            CurrentQuestion = QuestionsOrder1[tempq];
            CurrentItem = 0;
            Debug.Log("Question No : " + QuestionOrder1 + " A : " + QuestionsOrder1[QuestionOrder1]);
            QuestionOrder1++;
        }
        else if (QuestionOrder2 < (QuestionsOrder2.Length))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = QuestionOrder2;
            QuestionOrderListtemp.Remove(QuestionsOrder2[tempq]);
            CurrentQuestion = QuestionsOrder2[tempq];
            CurrentItem = Random.Range(1,5);
            Debug.Log("Question2 No : " + QuestionOrder2 + " A : " + QuestionsOrder2[QuestionOrder2]);
            QuestionOrder2++;
        }
        else if (WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count))
        {
            for (int i = 0; i < QuestionsOrder1.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder1[i]);
            }
            tempq = WrongAnsweredQuestionOrder1;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions1[tempq]);
            CurrentQuestion = WrongAnsweredQuestions1[tempq];
            CurrentItem = 0;
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions1[WrongAnsweredQuestionOrder1]);
            WrongAnsweredQuestionOrder1++;
        }
        else if (WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
        {
            for (int i = 0; i < QuestionsOrder2.Length; i++)
            {
                QuestionOrderListtemp.Add(QuestionsOrder2[i]);
            }
            tempq = WrongAnsweredQuestionOrder2;
            QuestionOrderListtemp.Remove(WrongAnsweredQuestions2[tempq]);
            CurrentQuestion = WrongAnsweredQuestions2[tempq];
            CurrentItem = Random.Range(1, 5);
            Debug.Log("MissedQuestion No : " + WrongAnsweredQuestionOrder2 + " A : " + WrongAnsweredQuestions2[WrongAnsweredQuestionOrder2]);
            WrongAnsweredQuestionOrder2++;
        }

        int[] OptionOrder=new int[3]; 
        float _delay = 0;
        for (int i = 0; i < Trays.Length; i++)
        {
            if (RandAnsrIndex == i)
            {
                OptionOrder[i] = CurrentQuestion;
                CorrectAnsrIndex = RandAnsrIndex;
            }
            else
            {
                if (i == 0)
                {
                    int _ixx = RandomNoFromList_Int(QuestionOrderListtemp);
                    OptionOrder[i] = _ixx;
                    QuestionOrderListtemp.Remove(_ixx);
                }
                else
                if (i == 1)
                {
                    int _iyy = RandomNoFromList_Int(QuestionOrderListtemp);
                    OptionOrder[i] = _iyy;
                    QuestionOrderListtemp.Remove(_iyy);
                }
                else
                if (i == 2)
                {
                    int _izz = RandomNoFromList_Int(QuestionOrderListtemp);
                    OptionOrder[i] = _izz;
                    QuestionOrderListtemp.Remove(_izz);                    
                }
            }
            Debug.Log("Options Order : " + OptionOrder[i]);

            TraysItems[i].transform.GetChild(OptionOrder[i]).gameObject.SetActive(true);
            SetItemsSprites(TraysItems[i].transform.GetChild(OptionOrder[i]).gameObject);

            iTween.ScaleFrom(TraysItems[i].gameObject, iTween.Hash("Scale", Vector3.zero, "time", 1f, "delay", _delay, "easetype", iTween.EaseType.easeOutElastic));
            //iTween.MoveFrom(TraysItems[i].gameObject, iTween.Hash("y", 300, "time", 0.25f, "islocal",true,"delay", _delay, "easetype", iTween.EaseType.spring));
            _delay += 0.1f;
        }

        Is_OkButtonPressed = false;
        PlayQuestionVoiceOver(CurrentQuestion);
        Invoke("EnableOptionsRaycast", QVOLength);
    }

    void EnableOptionsRaycast()
    {
        for (int i = 0; i < Trays.Length; i++)
        {
            Trays[i].GetComponent<Image>().raycastTarget = true;
        }
    }

    public void SetItemsSprites(GameObject _ItemsParent)
    {
        for (int i = 0; i < _ItemsParent.transform.childCount; i++)
        {
            _ItemsParent.transform.GetChild(i).GetComponent<Image>().sprite = TraysItemsSprires[CurrentItem];
        }
    }

    void PlayQuestionVoiceOver(int _Qi)
    {
        switch (CurrentItem)
        {
            case 0:
                switch (_Qi)
                {
                    case 0:
                        QVOLength= Sound_Q0_Cupcake[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q0_Cupcake);
                        break;
                    case 1:
                        QVOLength = Sound_Q1_Cupcake[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Cupcake);
                        break;
                    case 2:
                        QVOLength = Sound_Q2_Cupcake[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Cupcake);
                        break;
                    case 3:
                        QVOLength = Sound_Q3_Cupcake[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_Cupcake);
                        break;
                    case 4:
                        QVOLength = Sound_Q4_Cupcake[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q4_Cupcake);
                        break;
                    case 5:
                        QVOLength = Sound_Q5_Cupcake[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q5_Cupcake);
                        break;
                }
                break;
            case 1:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q0_Donut[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q0_Donut);
                        break;
                    case 1:
                        QVOLength = Sound_Q1_Donut[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Donut);
                        break;
                    case 2:
                        QVOLength = Sound_Q2_Donut[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Donut);
                        break;
                    case 3:
                        QVOLength = Sound_Q3_Donut[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_Donut);
                        break;
                    case 4:
                        QVOLength = Sound_Q4_Donut[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q4_Donut);
                        break;
                    case 5:
                        QVOLength = Sound_Q5_Donut[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q5_Donut);
                        break;
                }
                break;
            case 2:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q0_IceCream[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q0_IceCream);
                        break;
                    case 1:
                        QVOLength = Sound_Q1_IceCream[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_IceCream);
                        break;
                    case 2:
                        QVOLength = Sound_Q2_IceCream[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_IceCream);
                        break;
                    case 3:
                        QVOLength = Sound_Q3_IceCream[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_IceCream);
                        break;
                    case 4:
                        QVOLength = Sound_Q4_IceCream[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q4_IceCream);
                        break;
                    case 5:
                        QVOLength = Sound_Q5_IceCream[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q5_IceCream);
                        break;
                }
                break;
            case 3:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q0_Cookie[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q0_Cookie);
                        break;
                    case 1:
                        QVOLength = Sound_Q1_Cookie[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Cookie);
                        break;
                    case 2:
                        QVOLength = Sound_Q2_Cookie[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Cookie);
                        break;
                    case 3:
                        QVOLength = Sound_Q3_Cookie[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_Cookie);
                        break;
                    case 4:
                        QVOLength = Sound_Q4_Cookie[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q4_Cookie);
                        break;
                    case 5:
                        QVOLength = Sound_Q5_Cookie[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q5_Cookie);
                        break;
                }
                break;
            case 4:
                switch (_Qi)
                {
                    case 0:
                        QVOLength = Sound_Q0_Pastry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q0_Pastry);
                        break;
                    case 1:
                        QVOLength = Sound_Q1_Pastry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q1_Pastry);
                        break;
                    case 2:
                        QVOLength = Sound_Q2_Pastry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q2_Pastry);
                        break;
                    case 3:
                        QVOLength = Sound_Q3_Pastry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q3_Pastry);
                        break;
                    case 4:
                        QVOLength = Sound_Q4_Pastry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q4_Pastry);
                        break;
                    case 5:
                        QVOLength = Sound_Q5_Pastry[VOLanguage].clip.length;
                        PlayAudioRepeated(Sound_Q5_Pastry);
                        break;
                }
                break;
        }
    }

    public float PlayAnswerVoiceOver(int _Ai, float _delay)
    {
        float ClipLength=0;
        switch (CurrentItem)
        {
            case 0:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A0_CupCake, _delay);
                        ClipLength = Sound_A0_CupCake[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A1_CupCake, _delay);
                        ClipLength = Sound_A1_CupCake[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A2_CupCake, _delay);
                        ClipLength = Sound_A2_CupCake[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A3_CupCake, _delay);
                        ClipLength = Sound_A3_CupCake[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A4_CupCake, _delay);
                        ClipLength = Sound_A4_CupCake[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A5_CupCake, _delay);
                        ClipLength = Sound_A5_CupCake[VOLanguage].clip.length;
                        break;
                }
                break;
            case 1:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A0_Donut, _delay);
                        ClipLength = Sound_A0_Donut[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A1_Donut, _delay);
                        ClipLength = Sound_A1_Donut[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A2_Donut, _delay);
                        ClipLength = Sound_A2_Donut[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A3_Donut, _delay);
                        ClipLength = Sound_A3_Donut[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A4_Donut, _delay);
                        ClipLength = Sound_A4_Donut[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A5_Donut, _delay);
                        ClipLength = Sound_A5_Donut[VOLanguage].clip.length;
                        break;
                }
                break;
            case 2:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A0_IceCream, _delay);
                        ClipLength = Sound_A0_IceCream[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A1_IceCream, _delay);
                        ClipLength = Sound_A1_IceCream[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A2_IceCream, _delay);
                        ClipLength = Sound_A2_IceCream[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A3_IceCream, _delay);
                        ClipLength = Sound_A3_IceCream[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A4_IceCream, _delay);
                        ClipLength = Sound_A4_IceCream[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A5_IceCream, _delay);
                        ClipLength = Sound_A5_IceCream[VOLanguage].clip.length;
                        break;
                }
                break;
            case 3:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A0_Cookie, _delay);
                        ClipLength = Sound_A0_Cookie[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A1_Cookie, _delay);
                        ClipLength = Sound_A1_Cookie[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A2_Cookie, _delay);
                        ClipLength = Sound_A2_Cookie[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A3_Cookie, _delay);
                        ClipLength = Sound_A3_Cookie[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A4_Cookie, _delay);
                        ClipLength = Sound_A4_Cookie[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A5_Cookie, _delay);
                        ClipLength = Sound_A5_Cookie[VOLanguage].clip.length;
                        break;
                }
                break;
            case 4:
                switch (_Ai)
                {
                    case 0:
                        PlayAudio(Sound_A0_Pastry, _delay);
                        ClipLength = Sound_A0_Pastry[VOLanguage].clip.length;
                        break;
                    case 1:
                        PlayAudio(Sound_A1_Pastry, _delay);
                        ClipLength = Sound_A1_Pastry[VOLanguage].clip.length;
                        break;
                    case 2:
                        PlayAudio(Sound_A2_Pastry, _delay);
                        ClipLength = Sound_A2_Pastry[VOLanguage].clip.length;
                        break;
                    case 3:
                        PlayAudio(Sound_A3_Pastry, _delay);
                        ClipLength = Sound_A3_Pastry[VOLanguage].clip.length;
                        break;
                    case 4:
                        PlayAudio(Sound_A4_Pastry, _delay);
                        ClipLength = Sound_A4_Pastry[VOLanguage].clip.length;
                        break;
                    case 5:
                        PlayAudio(Sound_A5_Pastry, _delay);
                        ClipLength = Sound_A5_Pastry[VOLanguage].clip.length;
                        break;
                }
                break;
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

        for (int i = 0; i < Trays.Length; i++)
        {
            if(i== _Ansrindex)
            Trays[i].GetComponent<PopTweenCustom>().StartAnim();
            else
            Trays[i].GetComponent<PopTweenCustom>().StopAnim();
        }

        PlayAudio(Sound_Selection, 0);

        CancelInvoke("RepeatQVOAftertChoosingOption");
        Invoke("RepeatQVOAftertChoosingOption", 7);
    }
    void RepeatQVOAftertChoosingOption()
    {
        StartCoroutine("PlayAudioRepeatedCall");
    }

    bool Is_OkButtonPressed = false;
    public void BtnAct_Ok()
    {
        if (!Is_CanClick)
            return;

        Is_OkButtonPressed = true;

        PlayAudio(Sound_BtnOkClick, 0);

        for (int i = 0; i < Trays.Length; i++)
        {
            Trays[i].GetComponent<Image>().raycastTarget = false;
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

            Debug.Log("CORRECT ANSWER");
            if (Testing)
            {
                ProgreesBar.GetComponent<Slider>().value += 1;
            }
            WrongAnsrsCount = 0;            

            float LengthDelay = PlayAppreciationVoiceOver(Sound_BtnOkClick.clip.length) + Sound_BtnOkClick.clip.length;
            float LengthDelay2 = PlayAnswerVoiceOver(CurrentQuestion, LengthDelay + 0.25f);

            PlayAudio(Sound_CorrectAnswer, LengthDelay + LengthDelay2 + 0.5f);
            Tween_TickMark.myScript.Invoke("Tween_In", LengthDelay + LengthDelay2 + 0.5f);

            if (CurrentItem == 0)
            { Invoke("CupCakeAnim", LengthDelay + 0.5f);}
            else
            {Invoke("CupCakeAnim2", LengthDelay + 0.5f);}


            if (TraysItems.Length >= 0)
            {
                for (int i = 0; i < Trays.Length; i++)
                {
                    Destroy(TraysItems[i].gameObject, LengthDelay + LengthDelay2 + 2);
                    //Trays[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }
            //StartCoroutine(SetActiveWithDelayCall(LevelHolder, false, LengthDelay + LengthDelay2 + 2));

            Trays[UserAnsr].transform.GetComponent<PopTweenCustom>().Invoke("StopAnim", 0);

            if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                QuestionOrder2 < (QuestionsOrder2.Length) ||
                WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count) ||
                WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
            {
                Invoke("GenerateLevel", LengthDelay + LengthDelay2 + 3);
            }
            else
            {
                Debug.Log("Game Over");
                //Invoke("ShowLC", LengthDelay+ LengthDelay2 + 3);
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

            Debug.Log("WRONG ANSWER : ");
            iTween.ShakePosition(Trays[UserAnsr].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
            PlayAudio(Sound_IncorrectAnswer, 0);
            WrongAnsrsCount++;
            if (WrongAnsrsCount >= 2)
            {
                float LengthDelay = PlayAnswerVoiceOver(CurrentQuestion, Sound_IncorrectAnswer.clip.length + 0.25f);
    
                if (CurrentItem == 0)
                { Invoke("CupCakeAnim", Sound_IncorrectAnswer.clip.length + 0.25f);}
                else
                { Invoke("CupCakeAnim2", Sound_IncorrectAnswer.clip.length + 0.25f);}

                if (TraysItems.Length >= 0)
                {
                    for (int i = 0; i < Trays.Length; i++)
                    {
                        Destroy(TraysItems[i].gameObject, LengthDelay + Sound_IncorrectAnswer.clip.length + 1);
                        Trays[i].GetComponent<PopTweenCustom>().StopAnim();
                    }
                }

                if (!WrongAnsweredQuestions1.Contains(CurrentQuestion) && QuestionOrder1 <= (QuestionsOrder1.Length) && QuestionOrder2 == 0)
                {
                    WrongAnsweredQuestions1.Add(CurrentQuestion);
                }
                else
                if (!WrongAnsweredQuestions2.Contains(CurrentQuestion) && QuestionOrder2 <= (QuestionsOrder2.Length))
                {
                    WrongAnsweredQuestions2.Add(CurrentQuestion);
                }
                else
                {
                    //ProgreesBar.GetComponent<Slider>().value += 1;
                }

                if (QuestionOrder1 < (QuestionsOrder1.Length) ||
                    QuestionOrder2 < (QuestionsOrder2.Length) ||
                    WrongAnsweredQuestionOrder1 < (WrongAnsweredQuestions1.Count)||
                    WrongAnsweredQuestionOrder2 < (WrongAnsweredQuestions2.Count))
                {
                    Invoke("GenerateLevel", Sound_IncorrectAnswer.clip.length + LengthDelay +1.25f);
                }
                else
                {
                    Debug.Log("Game Over");
                    // Invoke("ShowLC", Sound_IncorrectAnswer.clip.length + LengthDelay + 1.25f);
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
                for (int i = 0; i < Trays.Length; i++)
                {
                    Trays[i].GetComponent<Image>().raycastTarget = true;
                    Trays[i].GetComponent<PopTweenCustom>().StopAnim();
                }
            }            
        }
        StartCoroutine(SetOk_Button(false, 0.25f));
    }

    void CupCakeAnim()
    {
        float _delay = 0.1f;
        for (int i = 0; i < Trays[CorrectAnsrIndex].transform.GetChild(0).GetChild(CurrentQuestion).transform.childCount; i++)
        {
            Trays[CorrectAnsrIndex].transform.GetChild(0).GetChild(CurrentQuestion).transform.GetChild(i).GetComponent<PopTweenCustom>().Invoke("StartAnim", _delay);
            _delay += 1;
            Trays[CorrectAnsrIndex].transform.GetChild(0).GetChild(CurrentQuestion).transform.GetChild(i).GetComponent<PopTweenCustom>().Invoke("StopAnim", _delay);
        }
    }

    void CupCakeAnim2()
    {
        for (int i = 0; i < Trays[CorrectAnsrIndex].transform.GetChild(0).GetChild(CurrentQuestion).transform.childCount; i++)
        {
            Trays[CorrectAnsrIndex].transform.GetChild(0).GetChild(CurrentQuestion).transform.GetChild(i).
                GetComponent<PopTweenCustom>().Invoke("StartAnim", 0);
        }
    }

    #endregion

    IEnumerator SetActiveWithDelayCall(GameObject _obj, bool _state, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _obj.gameObject.SetActive(_state);
    }
    public void ShowLC()
    {
        for (int i = 0; i < Trays.Length; i++)
        {
            Trays[i].GetComponent<PopTweenCustom>().StopAnim();
        }
        LCObj.gameObject.SetActive(true);
    }

    #region AUDIO VO
    [Header("=========== AUDIO VO CONTENT============")]
    public int VOLanguage;
    public AudioSource[] Sound_Intro1;
    public AudioSource[] Sound_Intro2;
    public AudioSource[] Sound_Intro3;
    public AudioSource[] Sound_Intro4;

    public AudioSource[] Sound_Q1_Cupcake;
    public AudioSource[] Sound_Q2_Cupcake;
    public AudioSource[] Sound_Q3_Cupcake;
    public AudioSource[] Sound_Q4_Cupcake;
    public AudioSource[] Sound_Q5_Cupcake;
    public AudioSource[] Sound_Q0_Cupcake;

    public AudioSource[] Sound_Q1_Donut;
    public AudioSource[] Sound_Q2_Donut;
    public AudioSource[] Sound_Q3_Donut;
    public AudioSource[] Sound_Q4_Donut;
    public AudioSource[] Sound_Q5_Donut;
    public AudioSource[] Sound_Q0_Donut;

    public AudioSource[] Sound_Q1_IceCream;
    public AudioSource[] Sound_Q2_IceCream;
    public AudioSource[] Sound_Q3_IceCream;
    public AudioSource[] Sound_Q4_IceCream;
    public AudioSource[] Sound_Q5_IceCream;
    public AudioSource[] Sound_Q0_IceCream;

    public AudioSource[] Sound_Q1_Cookie;
    public AudioSource[] Sound_Q2_Cookie;
    public AudioSource[] Sound_Q3_Cookie;
    public AudioSource[] Sound_Q4_Cookie;
    public AudioSource[] Sound_Q5_Cookie;
    public AudioSource[] Sound_Q0_Cookie;

    public AudioSource[] Sound_Q1_Pastry;
    public AudioSource[] Sound_Q2_Pastry;
    public AudioSource[] Sound_Q3_Pastry;
    public AudioSource[] Sound_Q4_Pastry;
    public AudioSource[] Sound_Q5_Pastry;
    public AudioSource[] Sound_Q0_Pastry;

    public AudioSource[] Sound_A0_CupCake;
    public AudioSource[] Sound_A1_CupCake;
    public AudioSource[] Sound_A2_CupCake;
    public AudioSource[] Sound_A3_CupCake;
    public AudioSource[] Sound_A4_CupCake;
    public AudioSource[] Sound_A5_CupCake;

    public AudioSource[] Sound_A0_Donut;
    public AudioSource[] Sound_A1_Donut;
    public AudioSource[] Sound_A2_Donut;
    public AudioSource[] Sound_A3_Donut;
    public AudioSource[] Sound_A4_Donut;
    public AudioSource[] Sound_A5_Donut;

    public AudioSource[] Sound_A0_IceCream;
    public AudioSource[] Sound_A1_IceCream;
    public AudioSource[] Sound_A2_IceCream;
    public AudioSource[] Sound_A3_IceCream;
    public AudioSource[] Sound_A4_IceCream;
    public AudioSource[] Sound_A5_IceCream;

    public AudioSource[] Sound_A0_Cookie;
    public AudioSource[] Sound_A1_Cookie;
    public AudioSource[] Sound_A2_Cookie;
    public AudioSource[] Sound_A3_Cookie;
    public AudioSource[] Sound_A4_Cookie;
    public AudioSource[] Sound_A5_Cookie;

    public AudioSource[] Sound_A0_Pastry;
    public AudioSource[] Sound_A1_Pastry;
    public AudioSource[] Sound_A2_Pastry;
    public AudioSource[] Sound_A3_Pastry;
    public AudioSource[] Sound_A4_Pastry;
    public AudioSource[] Sound_A5_Pastry;


    public AudioSource[] Sound_A11_;

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

    void SendResultFinal()
    {
        ///////////////////////////////Set final result output///////////////////
        if (Testing == false)
        {
            if (FrameworkOff == false)
                GameFrameworkInterface.Instance.SendResultToFramework();
        }

    }
}
