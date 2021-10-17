using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TMPro.TMP_Text Text;

    // Start is called before the first frame update
    void Awake()
    {
        MusicManager.instance.StartDeathMusic();
        Text.text = $"SCORE {GameManager.Score}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
