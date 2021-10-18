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
    public PlayerHealth playerHealth;

    public static ushort Score = 0;

    public static GameManager instance;

    static public void AddScore()
    {
        Score++;
    }

    static public void ResetScore()
    {
        Score++;
    }

    private void Update()
    {
        if (hudManager?.ScoreText)
            hudManager.ScoreText.text = $"Score: {Score}"; 
    }

    void Awake()
    {
        MusicManager.instance?.StartGameMusic();

        Score = 0;

        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        playerHealth.OnDeath += () => {
            SceneLoader.instance.NextScene();
        };

        StartedFishing += () =>
        {
            hudManager.powerBar.gameObject.SetActive(false);
            hudManager.spellBar.gameObject.SetActive(false);

            dragPower.Deactivate();
            cameraManager.GoToFishing();
            player.StartFishing();
        };

        StartedThrowFish += () =>
        {     
            hudManager.powerBar.gameObject.SetActive(true);
            hudManager.fishingBar.gameObject.SetActive(false);

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

        player.UsedSpell += (f, s) =>
        {
            if (!s.Available)
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
