using System.Collections;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioSource audioSource;
    private float startTime = 35f;
    private float endTime = 44f;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        StartCoroutine(DecreaseVolume());
    }

    IEnumerator DecreaseVolume()
    {
        yield return new WaitForSeconds(startTime);

        float duration = endTime - startTime;
        float startVolume = audioSource.volume;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration); // 平滑地减少音量
            yield return null;
        }

        audioSource.volume = 0f;
    }
}
