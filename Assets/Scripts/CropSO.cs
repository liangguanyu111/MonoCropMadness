using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

[Flags]
public enum compareType
{
    Cabbage = 1,
    Carrot= 2,
    Corn = 4,
    Cucumber = 8,
    Garlic = 16,
    Onion = 32,
    Potato = 64,
    Pumpkin = 128,
    Radish = 256,
    RedBeet = 512,
    Sunflower = 1024,
    Tomato = 2048,
    Watermelon = 4096,
    Wheat = 8192,
}

[CreateAssetMenu()]
public class CropSO : ScriptableObject
{
    public Season SuitSeason;

    public compareType myType;
    public compareType compareType;

    [JsonIgnore]
    public Sprite cropSprite;

    public string cropName;
    [JsonIgnore]
    public Transform plantSeed;

    public float seedPrice;
    public float cropPrice;
    public int expectProduce; //预期产量
    public int marketRequirement; //当上个月的销量超过需求量时 该植物的膨胀系数上升
}
