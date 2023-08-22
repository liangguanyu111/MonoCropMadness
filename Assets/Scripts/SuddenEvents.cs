using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenEvents : MonoBehaviour
{
    public static SuddenEvents instance;

    public GameObject cockroachEffect;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SoilSickEvent(SoilPlacableObject soil)
    {
        Vector3 pos = soil.transform.position + new Vector3(0, 0.5f, -0.6f);
       soil.cockroachEffect = Instantiate(cockroachEffect,pos,Quaternion.identity);
    }
}
