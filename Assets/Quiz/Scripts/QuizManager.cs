using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    
    [SerializeField] private QuizGameUI quizGameUI;
    
    [SerializeField] private List<QuizDataScriptable> quizDataList;
    [SerializeField] private float timeInSeconds;
#pragma warning restore 649

    private string currentCategory = "";
    private int correctAnswerCount = 0;
    
    private List<Question> questions;
    
    private Question selectedQuetion = new Question();
    private int gameScore;
    private int lifesRemaining;
    private float currentTime;
    private QuizDataScriptable dataScriptable;

    private GameStatus gameStatus = GameStatus.NEXT;

    public GameStatus GameStatus { get { return gameStatus; } }

    public List<QuizDataScriptable> QuizData { get => quizDataList; }

    public void StartGame(int categoryIndex, string category)
    {
        currentCategory = category;
        correctAnswerCount = 0;
        gameScore = 0;
        lifesRemaining = 3;
        currentTime = 240;
        
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions);
        
        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
    }

    /// <summary>
    
    /// </summary>
    private void SelectQuestion()
    {
        
        int val = UnityEngine.Random.Range(0, questions.Count);
        
        selectedQuetion = questions[val];
        
        quizGameUI.SetQuestion(selectedQuetion);

        questions.RemoveAt(val);
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            currentTime -= Time.deltaTime;
            SetTime(currentTime);
        }
    }

    void SetTime(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);                       
        quizGameUI.TimerText.text = time.ToString("mm':'ss");   

        if (currentTime <= 0)
        {
            
            GameEnd();
        }
    }

    /// <summary>
    
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        
        bool correct = false;
        
        if (selectedQuetion.correctAns == selectedOption)
        {
            
            correctAnswerCount++;
            correct = true;
            gameScore += 50;
            quizGameUI.ScoreText.text = "Puan:" + gameScore;
        }
        else
        {
            
            lifesRemaining--;
            quizGameUI.ReduceLife(lifesRemaining);

            if (lifesRemaining == 0)
            {
                GameEnd();
            }
        }

        if (gameStatus == GameStatus.PLAYING)
        {
            if (questions.Count > 0)
            {
                
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
        
        return correct;
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.GameOverPanel.SetActive(true);

        
        PlayerPrefs.SetInt(currentCategory, correctAnswerCount); 
    }
}


[System.Serializable]
public class Question
{
    public string questionInfo;         
    public QuestionType questionType;   
    public AudioClip audioClip;   
    public Sprite questionImage;        
        
    public UnityEngine.Video.VideoClip videoClip;   
    public List<string> options;        
    public string correctAns;           
}

[System.Serializable]
public enum QuestionType
{
    TEXT,
    IMAGE,
    AUDIO,
    VIDEO
}

[SerializeField]
public enum GameStatus
{
    PLAYING,
    NEXT
}