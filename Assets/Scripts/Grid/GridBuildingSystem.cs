using CodeMonkey.Utils;
using PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Grid
{
    public class GridBuildingSystem : Singleton<GridBuildingSystem>, IDisposable
    {
        public PlaceableObjectSO PlaceableObjectSO;
        public int rowCount = 10;
        public int columnCount = 10;
        public int cellSize = 5;
        public Vector3 StartOrigin = Vector3.zero;
        public bool ShowDebug = false;

        public Action OnSelectChanged;
        public Action<bool> OnBuildingModeChanged;

        public bool IsBuildingMode { get; set; } = false;

        private GridXZ<GridObject> grid;
        private PlaceableObjectSO.Dir dir = PlaceableObjectSO.Dir.Down;

        private GridPathFinding pf;

        public override void Awake()
        {
            base.Awake();

            grid = new GridXZ<GridObject>(rowCount,
                columnCount,
                cellSize,
                StartOrigin,
                (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z),
                ShowDebug);
            pf = new GridPathFinding(grid);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                IsBuildingMode = !IsBuildingMode;
                OnBuildingModeChanged.Invoke(IsBuildingMode);
            }

            if (IsBuildingMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var mousePosition = MousePositionUtils.MouseToTerrainPosition();
                    grid.GetXZ(mousePosition, out int x, out int z);

                    PlaceBuilding(PlaceableObjectSO, x, z, dir);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    GridObject gridObject = grid.GetGridObject(MousePositionUtils.MouseToTerrainPosition());

                    var placedObject = gridObject.PlaceableObject;
                    if (placedObject != null)
                    {
                        placedObject.DestroySelf();
                        var gridPosList = placedObject.GetOffSetList();
                        foreach (var gridPos in gridPosList)
                        {
                            grid.GetGridObject(gridObject.x+(int)gridPos.x, gridObject.z+(int)gridPos.y).PlaceableObject = placedObject;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    dir = PlaceableObjectSO.GetNextDir(dir);
                }
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void PlaceBuilding(PlaceableObjectSO placeableObjectSO, int x, int z, PlaceableObjectSO.Dir direction)
        {
            List<Vector2> offsetList = placeableObjectSO.GetOffsetList(dir);
            var canBuild = true;
            
            foreach (var offset in offsetList)
            {
                if(x + (int)offset.x<0|| x + (int)offset.x>grid.Width|| z + (int)offset.y<0|| z + (int)offset.y>grid.Height)
                {
                    canBuild = false;
                    break;
                }
                if(!grid.GetGridObject(x+(int)offset.x,z+(int)offset.y).CanBuild)
                {
                    canBuild = false;
                    break;
                }
            }

            //var gridPosList = placeableObjectSO.GetGridPositionList(new Vector2Int(x, z), dir);
            //var canBuild = true;
            //foreach (var gridPos in gridPosList)
            //{
            //    if (!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild)
            //    {
            //        canBuild = false;
            //        break;
            //    }
            //}
            if (canBuild)
            {
                //var rotOffset = placeableObjectSO.GetRotationOffset(dir);
                //var placedWorldPos = grid.GetWorldPosition(x, z) + new Vector3(rotOffset.x, 0, rotOffset.y) * grid.CellSize;
                var placedWorldPos = grid.GetWorldPosition(x, z) + new Vector3(0.5f,0,0.5f)* grid.CellSize;
                var placedObject = PlaceableObject.Create(
                    placedWorldPos,
                    new Vector2Int(x, z), dir,
                    placeableObjectSO);
                foreach (var gridPos in offsetList)
                {
                    Debug.Log("占据了:" + (x + (int)gridPos.x) + "," + (z + (int)gridPos.y));
                    var gridTobuild = grid.GetGridObject(x+(int)gridPos.x, z+(int)gridPos.y);
                    gridTobuild.PlaceableObject = placedObject;
                    gridTobuild.PlaceableObjectName = placedObject.placeableObjectSO.NameString;
                    gridTobuild.Direction = placedObject.dir;
       
                }
                return;
            }

            UtilsClass.CreateWorldTextPopup("Cannot build here", grid.GetWorldPosition(x, z));
        }

        public Quaternion GetCurrentBuildingRotation()
        {
            if (PlaceableObjectSO != null)
            {
                return Quaternion.Euler(0, PlaceableObjectSO.GetRotationAngle(dir), 0);
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            var mousePosition = MousePositionUtils.MouseToTerrainPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            if (PlaceableObjectSO != null)
            {
                //var rotationOffset = PlaceableObjectSO.GetRotationOffset(dir);
                return grid.GetWorldPosition(x, z) + new Vector3(0.5f, 0, 0.5f) * grid.CellSize; 
                // return grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize;
            }
            else
            {
                return mousePosition;
            }
        }

        public void SaveGrid()
        {
            SaveSystem.SaveObject("grid", grid);
        }

        public void LoadGrid()
        {
            foreach (var gridObject in grid.GridArray)
            {
                if (gridObject.PlaceableObject != null)
                {
                    gridObject.PlaceableObject.DestroySelf();
                  
                    gridObject.ClearPlaceableObject();
                }
            }
            RebuildGrid();
            pf = new GridPathFinding(grid);
        }

        public void RebuildGrid()
        {
            var g = SaveSystem.LoadSavedObject<GridXZ<GridObject>>("grid");
            this.grid = new GridXZ<GridObject>(g.Width,
                g.Height,
                g.CellSize,
                StartOrigin,
                (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z),
                ShowDebug);
            foreach (var gridObject in g.GridArray)
            {
                var name = gridObject.PlaceableObjectName;
                if (!String.IsNullOrEmpty(name))
                {
                    var placeableObjectSO = Resources.Load<PlaceableObjectSO>($"ScriptableObjects/{name}_SO");
                    PlaceBuilding(placeableObjectSO, gridObject.x, gridObject.z, gridObject.Direction);
                }
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < grid.GridArray.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GridArray.GetLength(1); j++)
                {
                    grid.GridArray[i, j] = null;
                }
            }
            grid.GridArray = null;
            grid = null;
        }
    }
}