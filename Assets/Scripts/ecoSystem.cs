using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct cropStruct
{
    public cropStruct(CropSO crop,Queue<float> pastPrice, Queue<float> pastSeedPrice, int amount, float latestCrop,float latestSeed)
    {
        this.crop = crop;
        this.pastPrice = pastPrice;
        this.pastSeedPrice = pastSeedPrice;
        latestCropPrice = latestCrop;
        latestSeedPrice = latestSeed;
        SellAmount =amount;
    }
    public CropSO crop;
    public Queue<float> pastPrice;
    public float latestCropPrice;
    public Queue<float> pastSeedPrice;
    public float latestSeedPrice;
    public int SellAmount;
    
}
public class ecoSystem
{
    public List<cropStruct> crops;
    public ecoSystem()
    {
        crops = new List<cropStruct>();
        foreach (var item in GameManager.instance.cropList)
        {
            crops.Add(new cropStruct(item, new Queue<float>(), new Queue<float>(),0,item.cropPrice,item.seedPrice));
        }
    }


    public Queue<float> GetPrice(CropSO crop)
    {
        foreach (var item in crops)
        {
            if(crop==item.crop)
            {
                return item.pastPrice;
            }
        }
        return null;
    }

    public void SellCrop(CropSO crop,int amount)
    {
        GameManager.instance.Gold += GetLastestCropPrice(crop) * amount;
        for (int i = 0; i < crops.Count; i++)
        {
            if (crop == crops[i].crop)
            {
                crops[i] = new cropStruct(crop, crops[i].pastPrice,crops[i].pastSeedPrice, crops[i].SellAmount + amount, crops[i].latestCropPrice,crops[i].latestSeedPrice);
            }
        }
    }


    public float CaculatePrice(CropSO crop)
    {
        
        float price = crop.cropPrice * Random.Range(0.9f, 1.1f);
        float a = 1 / ((GetCropStruct(crop).SellAmount / crop.marketRequirement + Random.Range(0.65f, 0.95f)));
        a = a > 0.5f ? a : 0.5f;
        a = a < 2.0f ? a : 2.0f;
        float factor1 =  a * Random.Range(0.9f, 1.1f);
        if(crop.SuitSeason == GameManager.instance.GetSeason()|| crop.SuitSeason == Season.AllSeason)
        {
            float factor2 = Random.Range(1.0f, 1.25f);
            price *= factor2;
        }
        price *= factor1;
        return price;
    }
    public float CaculateSeedPrice(CropSO crop)
    {

        float price = crop.seedPrice * Random.Range(0.9f, 1.1f);
        float a = 1 / ((GetCropStruct(crop).SellAmount / crop.marketRequirement + Random.Range(0.65f, 0.95f)));
        a = a > 0.5f ? a : 0.5f;
        a = a < 2.0f ? a : 2.0f;
        float factor1 = a * Random.Range(0.9f, 1.1f);
        if (crop.SuitSeason == GameManager.instance.GetSeason() || crop.SuitSeason == Season.AllSeason)
        {
            float factor2 = Random.Range(0.85f, 1f);
            price *= factor2;
        }
        price *= factor1;
        return price;
    }
    public cropStruct GetCropStruct(CropSO crop)
    {
        for (int i = 0; i < crops.Count; i++)
        {
            if (crop == crops[i].crop)
            {
                return crops[i];
            }
        }
        return default(cropStruct);
    }

    //Call it every 5
    public void UpdatePrice()
    {
        for (int i = 0; i < crops.Count; i++)
        {
            float latestCropPrice = CaculatePrice(crops[i].crop);
            float latestSeedPrice = CaculateSeedPrice(crops[i].crop);
            crops[i] = new cropStruct(crops[i].crop, crops[i].pastPrice, crops[i].pastSeedPrice, crops[i].SellAmount,latestCropPrice,latestSeedPrice) ;

            crops[i].pastPrice.Enqueue(latestCropPrice);
            crops[i].pastSeedPrice.Enqueue(latestSeedPrice);
        }
        Debug.Log("Update Price!");
    }
    public Queue<float> GetCropPrices(CropSO crop)
    {

        for (int i = 0; i < crops.Count; i++)
        {
            if (crop == crops[i].crop)
            {
                return crops[i].pastPrice;
            }
        }
        return null;
    }

    public float GetLastestCropPrice(CropSO crop)
    {
        for (int i = 0; i < crops.Count; i++)
        {
            if (crop == crops[i].crop)
            {
                return crops[i].latestCropPrice;
            }
        }
        return crop.cropPrice;
    }

    public float GetLastestSeedPrice(CropSO crop)
    {
        for (int i = 0; i < crops.Count; i++)
        {
            if (crop == crops[i].crop)
            {
                return crops[i].latestSeedPrice;
            }
        }
        return crop.seedPrice;
    }



    public Queue<float> GetSeedPrices(CropSO crop)
    {

        for (int i = 0; i < crops.Count; i++)
        {
            if (crop == crops[i].crop)
            {
                return crops[i].pastSeedPrice;
            }
        }
        return null;
    }


}
