using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TimingUI : MonoBehaviour
{
    private float startTime; // 开始时间
    public TMP_Text TimingText; // 时间显示文本
    public float t;

    private void Awake()
    {
        // 初始化进入游戏时各个组件的状态
        t = 300;
    }

    void Update()
    {
        t = t - Time.deltaTime;
        string minutes = ((int)t / 60).ToString("00");
        string seconds = (t % 60).ToString("00");
        TimingText.text = minutes + " : " + seconds;
    }


    public void ReTime() 
    {
        t = 300;
    }
}