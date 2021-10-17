using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public SpellBar spellBar;
    public PowerBar powerBar;
    public FishingBar fishingBar;
    public FishingRod fishingRod;
    public TMPro.TMP_Text ScoreText;

    void Start()
    {
        fishingRod.RodStartDrag += () => {
            fishingBar.gameObject.SetActive(true);
        };

        fishingRod.RodReleased += (drag) => {
            fishingBar.gameObject.SetActive(false);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
