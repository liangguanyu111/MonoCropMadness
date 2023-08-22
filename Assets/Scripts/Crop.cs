using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct cropInfo
{
   public GameObject cropModel;
   public int day;
}

public enum CropState
{
    Growing,
    Matured,
    withered,
}

public class Crop : MonoBehaviour,IInteractable
{
    [SerializeField]
    public List<cropInfo> cropList = new List<cropInfo>();

    public CropState cropState;
    public GameObject cropModel;

    public string cropName;
    private SoilPlacableObject soil;
    public bool canInteract { get; set; } = true;

    public ToolState toolState;

    public void CropInit(Vector3 pos,SoilPlacableObject soil)
    {
        this.soil = soil;
        LoadModelByCountDays();
        this.transform.position = pos;
        //cropState = CropState.Growing;
        GameManager.instance.playerData.gameDay.onNextDay.AddListener(NextDay); 
    }
    

    public void CropLoad(Vector3 pos)
    {
        
        
    }

    public void NextDay()
    {
        soil.soilInfo.cropCountDay += 1;

        LoadModelByCountDays();
    }

    public void LoadModelByCountDays()
    {
        for (int i = cropList.Count - 1; i >= 0; i--)
        {
            if (soil.soilInfo.cropCountDay >= cropList[i].day)
            {
                //Counsume we only have four state
                switch (i)
                {
                    case 2:
                        cropState = CropState.Matured;
                        break;
                    case 3:
                        cropState = CropState.withered;
                        break;
                }

                if(cropModel)
                {
                    Destroy(cropModel.gameObject);
                }
                cropModel = Instantiate(Resources.Load<GameObject>("FarmCrops/Prefabs/Crops/"+cropName+"_0"+(i+1).ToString()), this.transform);
                break;
            }
        }
    }

    bool IInteractable.CanInteract(ToolState toolState)
    {
        return this.toolState == toolState && canInteract&& cropState==CropState.Matured;
    }

    void IInteractable.Interact(ToolState toolState)
    {
        character ch = GameObject.FindGameObjectWithTag("Player").GetComponent<character>();
        ch.aniTodo = null;
        ch.actionTodo = null;
        ch.aniTodo = () => { ch.SetAni("Harvest"); };
        ch.actionTodo = () =>
        {
            canInteract = false;
            soil.Harvest();
        };
    }
}
