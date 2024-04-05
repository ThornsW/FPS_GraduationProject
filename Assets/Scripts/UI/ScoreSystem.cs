using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public static int score; // 本局得分
    private int speed = 50; // 分数变化速度
    public TMP_Text ScoreText; // 分数显示文本
    public Text NewScore; //最终得分显示


    private void Start()
    {
        score = 0;
    }

    private void Update()
    {
        int currentScore = int.Parse(ScoreText.text);
        if (currentScore == score) return;
        int updateScore = (int) Mathf.MoveTowards(currentScore, score, speed);
        ScoreText.text = updateScore.ToString();
        NewScore.text = ScoreText.text;
    }

    public static void AddScore(int value)
    {
        score += value;
        if (score > PlayerPrefs.GetInt("MaximumScore", 0))  //如果当前分数大于历史最高分
        {
            PlayerPrefs.SetInt("MaximumScore", score); //刷新最高分
        }
    }

}