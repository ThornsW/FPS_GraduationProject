using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;

    public GameObject PausePanel; //暂停页面

    public GameObject OverPanel;  //结束页面 

    public GameObject WinPanel;  //结算页面 

    public PlayerHealthController CurrentHealth; //玩家当前健康状态

    public TimingUI t; //倒计时时间

    private void Awake()
    {
        // 初始化进入游戏时各个组件的状态
        KeepOnGame();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                KeepOnGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (CurrentHealth.CurrentHealth == 0) //玩家死亡
        {
            OverGame(); //结束游戏
        }

        if (t.t < 0)  //时间到
        {
            WinGame();
        }
    }

    private void PauseGame()
    {
        IsPaused = true;
        // 把 time scale 设置为0就可以暂停游戏
        Time.timeScale = 0f;
        // 显示并解锁鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // 显示暂停面板
        PausePanel.SetActive(true);
        // 暂停声音
        AudioListener.pause = true;
    }
    private void OverGame()
    {
        IsPaused = true;
        // 把 time scale 设置为0就可以暂停游戏
        Time.timeScale = 0f;
        // 显示并解锁鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // 显示结束面板
        OverPanel.SetActive(true);
        // 暂停声音
        AudioListener.pause = true;
    }
    private void WinGame()
    {
        IsPaused = true;
        // 把 time scale 设置为0就可以暂停游戏
        Time.timeScale = 0f;
        // 显示并解锁鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // 显示结束面板
        WinPanel.SetActive(true);
        // 暂停声音
        AudioListener.pause = true;
    }

    public void KeepOnGame()
    {
        IsPaused = false;
        // 恢复游戏
        Time.timeScale = 1f;
        // 隐藏并锁定鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // 隐藏暂停面板
        PausePanel.SetActive(false);
        // 播放声音
        AudioListener.pause = false;
    }

    // 回到主菜单
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //重新开始
    public void RePlay()
    {
        KeepOnGame();
        SceneManager.LoadScene("LevelTwo");
    }
}