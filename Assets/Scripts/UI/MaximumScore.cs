using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaximumScore : MonoBehaviour
{

    public Text Scoretext;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Scoretext.text = PlayerPrefs.GetInt("MaximumScore", 0).ToString();
    }

    public void ReMaximumScore()
    {
        PlayerPrefs.SetInt("MaximumScore", 0);
    }
}
