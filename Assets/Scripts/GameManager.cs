using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    AllSeason,
}

[Serializable]
public struct PlayerData
{
    public PlayerData(int day = 0,int gold =50)
    {
        GameDay = day;
        this.gold = gold;
        gameDay = new Day();
    }

    public Day gameDay;
    public int GameDay;
    public float gold;
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    public PlayerData playerData;
    public Text goldText;

    public float sfxVolume = 1.0f;

    [Header("EconomySystem")]
    public ecoSystem eco;
    public List<CropSO> cropList = new List<CropSO>();

    public UnityEvent onNextDay = new UnityEvent();
    public UnityEvent onUpdatePrice = new UnityEvent();
    public UnityEvent onSaveGame = new UnityEvent();
    public UnityEvent onLoadGame = new UnityEvent();
    [Header("WeatherSystem")]
    public WeatherSystem weatherSystem;

    public float Gold
    { 
      get => playerData.gold;
      set
        {
            playerData.gold = value;
            goldText.text = Mathf.Floor(value).ToString();
        }
    }

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        eco = new ecoSystem();
        weatherSystem = new WeatherSystem();

        playerData = new PlayerData(0,50);
        playerData.gameDay.onNextProcess.AddListener(UImanager.instance.MoveSunByDayProcess);
        playerData.gameDay.onNextDay.AddListener(weatherSystem.GenerateRandomWeather);

        //onNextDay.AddListener(weatherSystem.GenerateRandomWeather);


        onSaveGame.AddListener(SavePlayerData);
        onLoadGame.AddListener(LoadPlayerData);
    }

    private void LoadPlayerData()
    {
        var data = SaveSystem.LoadSavedObject<PlayerData>("playerData");
        playerData = data;

        UImanager.instance.SetDay();
        goldText.text = Mathf.Floor(playerData.gold).ToString();

    }

    private void SavePlayerData()
    {
        SaveSystem.SaveObject("playerData", playerData);
    }

    private void Start()
    {
        onUpdatePrice.AddListener(eco.UpdatePrice);
        NextDay();
    }
    public void NextDay()
    {
        playerData.gameDay.NextDaySkip();
        //playerData.GameDay += 1;
        //onNextDay.Invoke();
        if (playerData.gameDay.dayNum % 5 ==1)
        {
            onUpdatePrice.Invoke();
        }
    }

    public Season GetSeason()
    { 
        switch((playerData.gameDay.dayNum / 10)%4)
        {
            case 0:
                return Season.Spring;
            case 1:
                return Season.Summer;
            case 2:
                return Season.Autumn;
            case 3:
                return Season.Winter;
        }

        return default(Season);
    }

    public int GetCropNum(CropSO crop)
    {
        for (int i = 0; i < cropList.Count; i++)
        {
            if(cropList[i] ==  crop)
            {
                return i;
            }
        }
        return -1;
    }


    public void SetVolume(float volume)
    {
        sfxVolume = volume;
        AudioManager._instance.SetVolume(sfxVolume);
    }



    public void SaveGame()
    {
        onSaveGame.Invoke();
    }

    public void LoadGame()
    {
        onLoadGame.Invoke();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
