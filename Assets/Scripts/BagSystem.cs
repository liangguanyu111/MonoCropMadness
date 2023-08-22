using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;

[Serializable]
public struct cropSeed
{
    public cropSeed(CropSO so,int value)
    {
        crop = so;
        soName = so.cropName;
        seedNum = value;
    }
    public cropSeed(String name, int num)
    {
        crop = Resources.Load<CropSO>("ScriptableObjects/Crop/" + name);
        soName = name;
        seedNum = num;
    }
    public string soName;
    public int seedNum;
    [JsonIgnore]
    public CropSO crop;
}
public struct cropPlant
{
    public cropPlant(CropSO so, int value)
    {
        crop = so;
        soName = so.cropName;
        cropNum = value;
    }
    public cropPlant(String name, int num)
    {
        crop = Resources.Load<CropSO>("ScriptableObjects/Crop/" + name);
        soName = name;
        cropNum = num;
    }
    public string soName;
    public int cropNum;
    [JsonIgnore]
    public CropSO crop;
}

public class BagSystem : MonoBehaviour
{

    [JsonIgnore]
    public static BagSystem instance;



    //背包里的种子
    public List<cropSeed> seeds = new List<cropSeed>();

    //背包里的庄稼
    public List<cropPlant> crops = new List<cropPlant>();

    [JsonIgnore]
    public CropSO cropToPlant;

    [JsonIgnore]
    [Header("SeedPanel")]
    public GameObject seedSlot;

    [JsonIgnore]
    public Transform seedPanel;

    [JsonIgnore]
    private List<GameObject> seedUI = new List<GameObject>();

    [JsonIgnore]
    public UnityEvent onHarvest = new UnityEvent();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        GameManager.instance.onSaveGame.AddListener(SaveBag);
        GameManager.instance.onLoadGame.AddListener(LoadBag);
    }

    public void GetSeeds(CropSO cropSO)
    {
        for (int i = 0; i < seeds.Count; i++)
        {
            if (seeds[i].crop == cropSO)
            {
                seeds[i] = new cropSeed(cropSO, seeds[i].seedNum + 1);
                return;
            }
        }
        seeds.Add(new cropSeed(cropSO, 1));
    }

    public bool CheckCrops(CropSO cropSO,int amount=0)
    {
       for(int i = 0; i < crops.Count; i++)
        {
            if (crops[i].crop == cropSO && crops[i].cropNum-amount >=0 && amount!=0)
            {      
                return true;;
            }
        }
        return false;
    }

    public void SellCrop(CropSO cropSO,int amount)
    {
        if(CheckCrops(cropSO,amount))
        {
            GetSellCrops(cropSO, -amount);
            GameManager.instance.eco.SellCrop(cropSO,amount);
            AudioManager._instance.PlaySfx("buttonBuy");
        }
    }

    public void GetSellCrops(CropSO cropSO,int amount)
    {
        for (int i = 0; i < crops.Count; i++)
        {
            if (crops[i].crop == cropSO)
            {
                crops[i] = new cropPlant(cropSO, crops[i].cropNum + amount);
                onHarvest.Invoke();
                return;
            }
        }
        crops.Add(new cropPlant(cropSO, amount));
        onHarvest.Invoke();
    }

    public cropPlant GetCrop(CropSO cropSO)
    {
        for (int i = 0; i < crops.Count; i++)
        {
            if (crops[i].crop == cropSO)
            {
                return crops[i];
            }
        }
        return default(cropPlant);
    }


    public void OpenSeedBag(SoilPlacableObject soil)
    {
        character ch = GameObject.FindGameObjectWithTag("Player").GetComponent<character>();
       
        seedPanel.gameObject.SetActive(true);

        foreach (var item in seedUI)
        {
            Destroy(item.gameObject);
        }
        seedUI.Clear();

        for (int i = 0; i < seeds.Count; i++)
        {
            GameObject seed = Instantiate(seedSlot, seedPanel.transform.GetChild(0));
            seedUI.Add(seed);
            cropSeed corpseed = seeds[i];
            seed.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = corpseed.crop.cropSprite;
            seed.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = corpseed.seedNum.ToString();
            seed.transform.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    ch.agent.SetDestination(soil.transform.position);
                    ch.aniTodo = null;
                    ch.actionTodo = null;
                    ch.aniTodo = () => 
                    {
                        ch.SetAni("Plant");
                    };
                    ch.actionTodo = () => 
                    {
                        UseSeedInBag(corpseed.crop, -1);
                        soil.LoadCropPrefab(corpseed.crop);
                        GameManager.instance.playerData.gameDay.NextProcess();
                    };
                    seedPanel.gameObject.SetActive(false);
                });
        }
    }

    public void UseSeedInBag(CropSO cropSo,int num)
    {
        for (int i = 0; i < seeds.Count; i++)
        {
            if (seeds[i].crop == cropSo)
            {
                cropToPlant = cropSo;
                seeds[i] = new cropSeed(seeds[i].crop, seeds[i].seedNum+num);
                if(seeds[i].seedNum<=0)
                {
                    seeds.Remove(seeds[i]);
                }
            }
        }
    }

    public void SaveBag()
    {
        SaveSystem.SaveObject("seedsBag", seeds);
        SaveSystem.SaveObject("cropsBag", crops);
    }

    public void LoadBag()
    {
        var seeds = SaveSystem.LoadSavedObject<List<cropSeed>>("seedsBag");
        var crops = SaveSystem.LoadSavedObject<List<cropPlant>>("cropsBag");
        if (seeds != null)
        {
            for (int i = 0; i < seeds.Count; i++)
            {
                seeds[i] = new cropSeed(seeds[i].soName,seeds[i].seedNum);
            }
            this.seeds = seeds;
        }
        if (crops != null)
        {
            for (int i = 0; i <crops.Count; i++)
            {
                crops[i]  = new cropPlant(crops[i].soName, crops[i].cropNum);
            }
            this.crops = crops;
        }
    }
}
