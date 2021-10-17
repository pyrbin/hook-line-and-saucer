using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action StartedFishing;
    public event Action StartedThrowFish;

    public FishCollectArea fishCollectArea;
    public Transform fishProjectilePosition;
    public CameraManager cameraManager;
    public PlayerThrowFish playerThrowFish;
    public DragPower dragPower;
    public HUDManager hudManager;

    public Player player;    

    void Start()
    {
        StartedFishing += () =>
        {
            hudManager.powerBar.gameObject.SetActive(false);
            hudManager.spellBar.gameObject.SetActive(false);

            cameraManager.GoToFishing();
            player.StartFishing();
        };

        StartedThrowFish += () =>
        {     
            hudManager.powerBar.gameObject.SetActive(true);

            if (playerThrowFish?.Fish?.Parent?.Spell != null)
            {
                hudManager.spellBar.gameObject.SetActive(true);
                hudManager.spellBar.SetFishIcon(playerThrowFish.Fish.Parent.Swimming.Model.sprite);
                hudManager.spellBar.SetFishSpellName(playerThrowFish.Fish.Parent.Spell?.SpellName);
            }

            dragPower.Activate();
            cameraManager.GoToOverview();
        };

        fishCollectArea.FishCollected += (fish) => OnCollectFish(fish);

        player.UsedSpell += (f) =>
        {
            hudManager.spellBar.gameObject.SetActive(false);
        };

        playerThrowFish.FirstThrow += () =>
        {
            cameraManager.GoToTracking(playerThrowFish.Fish.gameObject.transform);
            playerThrowFish.Fish.Parent.Despawned += (stats) =>
            {
                playerThrowFish.Fish = null;
                StartFishing();
            };
        };
    }

    void OnCollectFish(Fish fish) {
        fish.transform.position = fishProjectilePosition.position;
        player.StartThrowFish(fish);

        StartedThrowFish?.Invoke();
    }

    void StartFishing() {
        StartedFishing?.Invoke();
    }
}
