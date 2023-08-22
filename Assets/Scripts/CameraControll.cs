using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public float MoveSpeed;

    private Vector3 newPos = new Vector3();
    private void LateUpdate()
    {
        Vector2 mousePos = Input.mousePosition;

        newPos.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if(mousePos.x>Screen.width * 0.95f &&mousePos.x<Screen.width)
        {
            newPos.z = 1;
        }
        if(mousePos.x < Screen.width * 0.05f && mousePos.x > 0)
        {
            newPos.z = -1;
        }
        if(mousePos.y > Screen.height * 0.95f && mousePos.y < Screen.height)
        {
            newPos.x = -1;
        }
        if(mousePos.y < Screen.height * 0.05f && mousePos.y> 0)
        {
            newPos.x = 1;
        }


        Vector3 nextPos = transform.position + newPos.normalized * MoveSpeed * Time.deltaTime;
        if (nextPos.z<=0 && nextPos.z>=-30 &&nextPos.x<=10 && nextPos.x>=-10 )
        {
            transform.position = nextPos;
        }
    }
}
