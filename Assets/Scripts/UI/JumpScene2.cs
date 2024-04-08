using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpScene2 : MonoBehaviour
{
    private float delayInSeconds = 44f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadScene", delayInSeconds);
    }

    void LoadScene()
    {
        SceneManager.LoadScene("Scene2"); // 加载指定的场景
    }
}