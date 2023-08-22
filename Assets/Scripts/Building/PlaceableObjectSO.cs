using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlaceableObjectSO : ScriptableObject
{
    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right,
    }

    public string NameString;
    public Transform Prefab;
    public int Width;
    public int Height;
    public List<Vector2> offsetList;

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, Width);
            case Dir.Up: return new Vector2Int(Width, Height);
            case Dir.Right: return new Vector2Int(Height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir)
        {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

    public List<Vector2> GetOffsetList(PlaceableObjectSO.Dir dir)
    {
        List<Vector2> offsets = new List<Vector2>(offsetList.ToArray());
        foreach (var offset in offsets)
        {
            switch (dir)
            {
                case PlaceableObjectSO.Dir.Down:
                    {
                        break;
                    }
                case PlaceableObjectSO.Dir.Up:
                    {
                        offsets.Add(new Vector2(-offset.x, -offset.y));
                        break;
                    }
                case PlaceableObjectSO.Dir.Left:
                    {
                        if (offset.x == 0)
                        {
                            offsets.Add(new Vector2(offset.y, 0));
                        }
                        else if (offset.y == 0)
                        {
                            offsets.Add(new Vector2(0, -offset.x));
                        }
                        else if ((offset.x < 0 && offset.y < 0) || (offset.x > 0 && offset.y > 0))
                        {
                            offsets.Add(new Vector2(offset.y, -offset.x));
                        }
                        else if ((offset.x > 0 && offset.y < 0) || (offset.x < 0 && offset.y > 0))
                        {
                            offsets.Add(new Vector2(-offset.y, offset.x));
                        }
                        break;
                    }
                case PlaceableObjectSO.Dir.Right:
                    {
                        if (offset.x == 0)
                        {
                            offsets.Add(new Vector2(-offset.y, 0));
                        }
                        else if (offset.y == 0)
                        {
                            offsets.Add(new Vector2(0, offset.x));
                        }
                        else if ((offset.x < 0 && offset.y < 0) || (offset.x > 0 && offset.y > 0))
                        {
                            offsets.Add(new Vector2(-offset.y, offset.x));
                        }
                        else if ((offset.x > 0 && offset.y < 0) || (offset.x < 0 && offset.y > 0))
                        {
                            offsets.Add(new Vector2(offset.y, -offset.x));
                        }
                        break;
                    }
            }
        }
        offsets.Add(new Vector2(0, 0));
        return offsets;
    }
}