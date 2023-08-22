using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool canInteract { get; set; } 

    bool CanInteract(ToolState toolState);
    void Interact(ToolState toolState);
}
