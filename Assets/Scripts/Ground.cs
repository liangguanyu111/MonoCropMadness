using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour, IInteractable
{
    public ToolState toolState = ToolState.Hxe;

    public bool canInteract { get; set; } = true;

  
    public void Interact(ToolState toolState)
    {
        character ch = GameObject.FindGameObjectWithTag("Player").GetComponent<character>();
        ch.aniTodo = null;
        ch.actionTodo = null;
        ch.agent.SetDestination(this.transform.position);
        ch.aniTodo = ()=> { ch.SetAni("Dig"); };

        ch.actionTodo = () => {
            FarmGrid grid = BuildManager.instance.grid;
            grid.GetXZ(this.transform.position, out int x, out int z);
            FarmGridObject gridObject = grid.GetGridObject(x, z);
            if (grid.GetGridObject(x, z).canBuild)
            {
                FarmPlacableObject placableObject = BuildManager.instance.PlaceBuilding(BuildManager.instance.PlaceableObjectSO, x, z, new SoilInfo());
                SoilPlacableObject soilPlaceableObject;
                if (placableObject.TryGetComponent<SoilPlacableObject>(out soilPlaceableObject))
                {
                    soilPlaceableObject.OnSoilChange.AddListener(gridObject.SetSoilInfo);
                }

                GameManager.instance.playerData.gameDay.NextProcess();
            }

            canInteract = false;
        };
    }

    bool IInteractable.CanInteract(ToolState toolState)
    {
        return this.toolState == toolState && canInteract;
    }
}
