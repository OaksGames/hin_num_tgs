using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiLevelManager : MonoBehaviour
{
    public static MultiLevelManager instance;

    private int CurrentLevel;
    private int TotalLevels;
    public Slider Progressbar;
    public GameObject[] AllLevels;

    public int Total_CorrectAnswers;
    public int Total_Attempts;
    public int Total_Questions;

    private void Awake()
    {
        instance = this;
        
    }

    void Start()
    {
        Progressbar.value = 0;
        Progressbar.maxValue = 0;
        for (int i = 0; i < AllLevels.Length; i++)
        {
            AllLevels[i].gameObject.SetActive(false);
        }

        CurrentLevel = 0;
        TotalLevels = AllLevels.Length;
        CheckForNextLevel();
    }

    public void LoadProgressMaxValues(int _value)
    {
        Total_Questions += _value;
        Progressbar.maxValue+= _value;
    }

    public void UpdateProgress(int _value, int _correctAnsValue)
    {
        Total_Attempts+= _value;
        Progressbar.value+= _value;
        Total_CorrectAnswers += _correctAnsValue;
    }
    
    public bool CheckForNextLevel()
    {
        if (CurrentLevel < AllLevels.Length)
        {
            CurrentLevel++;
            GenerateLevel();
            Debug.Log("CurrentLevel : "+ CurrentLevel);
            return true;
        }
        else
        {
            Debug.Log("ALL LEVELS COMPLETED");
            return false;
        }
    }

    void GenerateLevel()
    {
        AllLevels[CurrentLevel - 1].gameObject.SetActive(true);
        if (CurrentLevel - 2>= 0)
        {
            AllLevels[CurrentLevel - 2].gameObject.SetActive(false);
        }
        Debug.Log("GenerateLevel : " + CurrentLevel);
    }
}
