using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Newtonsoft.Json;

public class FarmGrid
{
    public FarmGridObject[,] GridArray;
    public int Width;
    public int Height;
    public float CellSize;
    [JsonIgnore]
    private Vector3 originPosition;


    public FarmGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.Width = width;
        this.Height = height;
        this.CellSize = cellSize;
        this.originPosition = originPosition;

        GridArray = new FarmGridObject[width, height];

        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int z = 0; z < GridArray.GetLength(1); z++)
            {
                GridArray[x, z] = new FarmGridObject(this, x, z);
            }
        }

    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * CellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / CellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / CellSize);
    }

    public void SetGridObject(int x, int z, FarmGridObject value)
    {
        if (x >= 0 && z >= 0 && x < Width && z < Height)
        {
            GridArray[x, z] = value;
        }
    }
    public void SetGridObject(Vector3 worldPosition, FarmGridObject value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public FarmGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < Width && z < Height)
        {
            return GridArray[x, z];
        }
        else
        {
            return default(FarmGridObject);
        }
    }

    public FarmGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, Width - 1),
            Mathf.Clamp(gridPosition.y, 0, Height - 1)
        );
    }

    public bool CheckPosition(Vector3 worldPosition)
    {
        GetXZ(worldPosition, out int x, out int z);
        if (x >= 0 && x < Width && z >= 0 && z < Height)
        {
            return true;
        }
        return false;
    }
    public bool CheckPosition(int x, int z)
    {
        if(x>=0&&x<Height&&z>=0&&z<Width)
        {
            return true;
        }
        return false;
    }

    public List<FarmGridObject> GetAdjacentedGrid(int x,int z)
    {
        List<FarmGridObject> AdjacentedList = new List<FarmGridObject>();
        if (x+1<Width)
        {
            AdjacentedList.Add(GridArray[x + 1, z]);
        }
        if(x-1>=0)
        {
            AdjacentedList.Add(GridArray[x - 1, z]);
        }

        if (z + 1 < Height)
        {
            AdjacentedList.Add(GridArray[x , z+1]);
        }
        if (z - 1 >= 0)
        {
            AdjacentedList.Add(GridArray[x, z-1]);
        }

        return AdjacentedList;
    }
}
