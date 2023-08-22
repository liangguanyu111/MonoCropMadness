using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FarmPlacableObject:MonoBehaviour,IInteractable
{
    
    public ToolState toolState;
    public FarmPlacableObjectSO FarmplaceableObjectSO;
    protected Vector2Int origin;

    [SerializeField]
    public List<ToolStatesCheck> statesCheck = new List<ToolStatesCheck>();

    public bool canInteract { get; set; } = true;

    public static FarmPlacableObject Create(Vector3 worldPosition, Vector2Int origin, FarmPlacableObjectSO placeableObjectSO,SoilInfo soilInfo)
    {
        var placedObjectTransform = Instantiate(
            placeableObjectSO.Prefab,
            worldPosition,
            Quaternion.identity);
        var placeableObject = placedObjectTransform.GetComponent<FarmPlacableObject>();

        placeableObject.FarmplaceableObjectSO = placeableObjectSO;
        placeableObject.origin = origin;

        // Build effects
        placedObjectTransform.DOShakeScale(.5f, .2f, 10, 90, true);

        SoilPlacableObject soil;
        if(placedObjectTransform.TryGetComponent<SoilPlacableObject>(out soil))
        {
            soil.soilInfo = soilInfo;

            if(soilInfo.cropNum!= -1)
            {
                soil.LoadCropBySoilInfo(soilInfo);
            }
            GameManager.instance.playerData.gameDay.onNextDay.AddListener(soil.OnNextDay);
        }

        return placeableObject;
    }

    public bool CanInteract(ToolState toolState)
    {
        if(this.toolState.HasFlag(toolState))
        {
            foreach (var item in statesCheck)
            {
                if(item.State == toolState)
                {
                    return item.StateStatus;
                }
            }
        }
        return false;
    }

    public void SetToolStateStatus(ToolState state,bool value)
    {
        for (int i = 0; i < statesCheck.Count; i++)
        {
            if (statesCheck[i].State == state)
            {
                statesCheck[i] = new ToolStatesCheck(state, value);
            }
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public virtual void Interact(ToolState toolState)
    {

    }

}
