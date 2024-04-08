using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class JumpMainMenu : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private float delyJump = 17f;
    private float delayPlayer = 3f;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(PlayVideoAfterDelay());
        Invoke("LoadMainMenu", delyJump);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // 加载指定的场景
    }

    IEnumerator PlayVideoAfterDelay()
    {
        yield return new WaitForSeconds(delayPlayer);
        videoPlayer.Play();
    }
}