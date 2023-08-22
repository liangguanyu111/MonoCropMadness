using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
[System.Serializable]
public class SoilInfo
{
    public float nutritionalLevel;
    public float healthLevel;

    public int cropNum = -1;
    public int cropCountDay = 0;


    public bool isSick;
    public bool isUpgrade;

    public SoilInfo()
    {
        nutritionalLevel = 100;
        healthLevel = 100;
        cropNum = -1;
        isSick = false;
        isUpgrade = false;
        cropCountDay = 0;
    }

    public SoilInfo(int cropNum)
    {
        nutritionalLevel = 100;
        healthLevel = 100;
        this.cropNum = cropNum;
    }

    public void UpdateSoilInfo(CropSO crop)
    {
        this.cropNum = GameManager.instance.GetCropNum(crop);
    }
}
