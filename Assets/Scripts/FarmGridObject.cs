using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.Events;

public class FarmGridObject
{
    //This is the GridSlot
    public bool canBuild;

    public int x;
    public int z;
    public string PlaceableObjectName;

    public SoilInfo soilInfo;

    private FarmGrid grid;
    private FarmPlacableObject placeableObject;



    [JsonIgnore]
    public FarmPlacableObject PlaceableObject
    {
        get => placeableObject;
        set
        {
            placeableObject = value;
        }
    }



    public FarmGridObject(FarmGrid grid, int x, int z)
    {
        canBuild = true;
        this.grid = grid;
        this.x = x;
        this.z = z;

        soilInfo = new SoilInfo();
    }

    public void SetSoilInfo(SoilInfo soilInfo)
    {
        this.soilInfo = soilInfo;
    }
    public override string ToString()
    {
        return $"{x}, {z}";
    }
    public void ClearPlaceableObject()
    {
        placeableObject = null;
        PlaceableObjectName = null;
    }
}
