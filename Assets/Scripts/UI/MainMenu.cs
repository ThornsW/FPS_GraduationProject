using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        // 避免从游戏中退出后没有动画和声音
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    // 开始游戏，默认进行关卡一
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelTwo");
    }

    // 退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }
}