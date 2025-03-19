using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXSpawner : MonoBehaviour
{
    public VisualEffect VFX;
    public GameObject VFXPos;
    public Camera Cam;

    private RaycastHit hit;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VFX.SendEvent("Spawn");
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            VFX.SendEvent("StopSpawn");
        }

        if (Physics.Raycast(Cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            VFXPos.transform.position = hit.point;
        }  
    }
}
