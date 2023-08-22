using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


[Flags]
public enum ToolState
{
    Default=0,
    Hxe=1,
    Seed=2,
    Glove=4,
    Shovel=8,
    Pesticides=16,
    fertilizer =32,
    
    Soil = Seed|Shovel|fertilizer|Pesticides,
}

[Serializable]
public struct ToolStatesCheck
{
    public ToolState State;
    public bool StateStatus;

    public ToolStatesCheck(ToolState state,bool status)
    {
        State = state;
        StateStatus = status;
    }
}

public class BuildManager : MonoBehaviour
{
        public static BuildManager instance;

        public bool isBuildMode = false;
        public UnityEvent<bool> onBuildMode = new UnityEvent<bool>();
        public ToolState toolState = ToolState.Glove;

        public CropSO cropTest;

        [Header("Grid")]
        public FarmPlacableObjectSO PlaceableObjectSO;
        public FarmGrid grid;


        public int rowCount;
        public int columnCount;
        public float cellSize;
        public Vector3 StartOrigin;
        [Header("Cursor")]
        public List<Texture2D> cursorTextures = new List<Texture2D>();



        EPOOutline.Outlinable outlineable;
        EPOOutline.Outlinable nextOutlineable;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            GameManager.instance.onSaveGame.AddListener(SaveGrid);
            GameManager.instance.onLoadGame.AddListener(LoadGrid);
        }

        private void Start()
        {

             grid = new FarmGrid(rowCount,
             columnCount,
             cellSize,
             StartOrigin);
        }

        private void Update()
        {
            BuildMode();
        }

        void BuildMode()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                isBuildMode = !isBuildMode;
                onBuildMode.Invoke(isBuildMode);
                if(!isBuildMode)
                {
                    toolState = ToolState.Default;
                    Cursor.SetCursor(cursorTextures[0], new Vector2(0, 0), CursorMode.Auto);
            }
            }
            if(isBuildMode)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    toolState = ToolState.Hxe;
                    Cursor.SetCursor(cursorTextures[1],new Vector2(0,0),CursorMode.Auto);
                }
                else if(Input.GetKeyDown(KeyCode.Alpha2))
                {
                    toolState = ToolState.Seed;
                    //BagSystem.instance.OpenSeedBag();
                    Cursor.SetCursor(cursorTextures[2], new Vector2(0, 0), CursorMode.Auto);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    toolState = ToolState.Glove;
                    Cursor.SetCursor(cursorTextures[3], new Vector2(0, 0), CursorMode.Auto);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    toolState = ToolState.Shovel;
                    Cursor.SetCursor(cursorTextures[4], new Vector2(0, 0), CursorMode.Auto);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    toolState = ToolState.Pesticides;
                    Cursor.SetCursor(cursorTextures[5], new Vector2(0, 0), CursorMode.Auto);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    toolState = ToolState.fertilizer;
                    Cursor.SetCursor(cursorTextures[6], new Vector2(0, 0), CursorMode.Auto);
                }
        }
            Interact();
            //DefaultMode();
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
        

        public void Interact()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            IInteractable interactable;

            if((Physics.Raycast(ray, out hit, 1000f,LayerMask.GetMask("Interactable")) && hit.collider.TryGetComponent<EPOOutline.Outlinable>(out nextOutlineable)))
            {

            if (!nextOutlineable.TryGetComponent<IInteractable>(out interactable))
            {
                interactable = nextOutlineable.transform.GetComponentInParent<IInteractable>();
            }
            if(interactable.CanInteract(toolState))
            {

                    if(Input.GetMouseButtonDown(0))
                    {
                        interactable.Interact(toolState);
                    }
                    if(outlineable!=null&&outlineable!=nextOutlineable)
                    {
                        outlineable.enabled = false;
                    }
                    nextOutlineable.enabled = true;
                    outlineable = nextOutlineable;               
            }
            else if(nextOutlineable!=null&&outlineable!=null)
            {
                    outlineable.enabled = false;
                    nextOutlineable.enabled = false;
            }
        }

        //if (toolState==ToolState.Hxe&&Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.gameObject.layer == 7)
        //        {
        //            grid.GetXZ(hit.point, out int x, out int z);
        //            UImanager.instance.GridPick(x, z);
        //        }
        //    }
        //    else if ((Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Ground")) && hit.collider.TryGetComponent<IInteractable>(out interactable)))
        //    {
        //            interactable.Interact(toolState);
        //    }
        }
        void DefaultMode()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                IInteractable interactable;
                if (toolState == ToolState.Hxe)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.layer == 7)
                        {
                            grid.GetXZ(hit.point, out int x, out int z);
                            UImanager.instance.GridPick(x, z);
                        }
                    }
                    if (Input.GetMouseButtonDown(0) && grid.CheckPosition(hit.point))
                    {
                        grid.GetXZ(hit.point, out int x, out int z);
                        FarmGridObject gridObject = grid.GetGridObject(x, z);
                        if (grid.GetGridObject(x, z).canBuild)
                        {
                            FarmPlacableObject placableObject = PlaceBuilding(PlaceableObjectSO, x, z, new SoilInfo());
                            SoilPlacableObject soilPlaceableObject;
                            if (placableObject.TryGetComponent<SoilPlacableObject>(out soilPlaceableObject))
                            {
                                soilPlaceableObject.OnSoilChange.AddListener(gridObject.SetSoilInfo);
                            }
                        }
                    }
                    return;
                }

                if ((Physics.Raycast(ray, out hit,1000f,LayerMask.GetMask("Ground"))&&hit.collider.TryGetComponent<IInteractable>(out interactable)))
                {
                    interactable.Interact(toolState);
                }
                else
                {
                 //UImanager.instance.ActiveSoilMenu(false, Vector3.zero);
                 //UImanager.instance.soil = null;
                }

            }
        }
        //void HxeMode()
        //{
        //    if (toolState == ToolState.Hxe)
        //    {
        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;
        //        if (Physics.Raycast(ray, out hit))
        //        {
        //            if (hit.collider.gameObject.layer == 7)
        //            {
        //                grid.GetXZ(hit.point,out int x, out int z);
        //                UImanager.instance.GridPick(x, z);
        //            }
        //        }
        //        if(Input.GetMouseButtonDown(0)&&grid.CheckPosition(hit.point))
        //        {
        //            grid.GetXZ(hit.point, out int x, out int z);
        //            if(grid.GetGridObject(x, z).canBuild)
        //            {
        //                PlaceBuilding(PlaceableObjectSO, x, z);
        //            }
        //        }
        //    }
        //}

        public FarmPlacableObject PlaceBuilding(FarmPlacableObjectSO placeableObjectSO, int x, int z,SoilInfo soilInfo)
        {
        
                //var rotOffset = placeableObjectSO.GetRotationOffset(dir);
                //var placedWorldPos = grid.GetWorldPosition(x, z) + new Vector3(rotOffset.x, 0, rotOffset.y) * grid.CellSize;
            var placedWorldPos = grid.GetWorldPosition(x, z) + new Vector3(0.5f,0,0.5f)*cellSize;
            var placedObject = FarmPlacableObject.Create(
                    placedWorldPos,
                    new Vector2Int(x, z),
                    placeableObjectSO,soilInfo);

            SoilPlacableObject soilObject;

            if(placedObject.TryGetComponent<SoilPlacableObject>(out soilObject))
            {
                soilObject.onPlant.AddListener(CheckComparePlant);
            }
    
            var gridTobuild = grid.GetGridObject(x, z);


            gridTobuild.canBuild = false;
            gridTobuild.PlaceableObject = placedObject;
            gridTobuild.PlaceableObjectName = placedObject.FarmplaceableObjectSO.NameString;



            return placedObject;

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
        }

    public string BuildMode(int toolIndex)
    {

        if(nextOutlineable!=null)
        {
            nextOutlineable.enabled = false;
            nextOutlineable = null;
        }

        if(outlineable!=null)
        {
            outlineable.enabled = false;
            outlineable = null;
        }
        switch (toolIndex)
        {
            case 0:
                toolState = ToolState.Hxe;
                return "Hxe";
            case 1:
                toolState = ToolState.Seed;
                return "Seed";
            case 2:
                toolState = ToolState.Glove;
                return "Glove";
            case 3:
                toolState = ToolState.Shovel;
                return "Shovel";
            case 4:
                toolState = ToolState.Pesticides;
                return "Pesticides";
            case 5:
                toolState = ToolState.fertilizer;
                return "Fertilizer";
        }

        return " ";
    }
    public void RebuildGrid()
    {
        var g = SaveSystem.LoadSavedObject<FarmGrid>("grid");
        this.grid = new FarmGrid(g.Width,
            g.Height,
            g.CellSize,
            StartOrigin);
        foreach (var gridObject in g.GridArray)
        {
            var name = gridObject.PlaceableObjectName;
            SoilInfo soilInfo = gridObject.soilInfo;
            if (!String.IsNullOrEmpty(name))
            {
                var placeableObjectSO = Resources.Load<FarmPlacableObjectSO>($"ScriptableObjects/{name}_SO");
                gridObject.PlaceableObject = PlaceBuilding(placeableObjectSO, gridObject.x, gridObject.z,soilInfo);

                SoilPlacableObject soilPlaceableObject;
                if(gridObject.PlaceableObject.TryGetComponent<SoilPlacableObject>(out soilPlaceableObject))
                {
                    soilPlaceableObject.OnSoilChange.AddListener(gridObject.SetSoilInfo);
                }
            }
        }
    }

    public void CheckComparePlant(SoilPlacableObject soilObject, int x,int z)
    {
       List<FarmGridObject> farms = grid.GetAdjacentedGrid(x, z);

        foreach (var item in farms)
        {
            SoilPlacableObject soilPlaceableObject;
            if (item.PlaceableObject!=null && item.PlaceableObject.TryGetComponent<SoilPlacableObject>(out soilPlaceableObject)&&soilPlaceableObject.cropInfo!=null)
            {
                if(soilObject.cropInfo.compareType.HasFlag(soilPlaceableObject.cropInfo.myType))
                {
                    if(!soilObject.soilInfo.isSick)
                    {
                        soilObject.soilInfo.isUpgrade = true;
                        soilObject.Upgrade();
                    }

                    if(!soilPlaceableObject.soilInfo.isSick && !soilPlaceableObject.soilInfo.isUpgrade)
                    {
                        soilPlaceableObject.soilInfo.isUpgrade = true;
                        soilPlaceableObject.Upgrade();
                    }
                }
            }
        }
     


    }
}

