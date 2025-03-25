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
        //Paint particles
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            VFX.SendEvent("SpawnPaint");
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            VFX.SendEvent("StopPaint");
        }

        //Grow particles
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            VFX.SendEvent("SpawnGrow");
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            VFX.SendEvent("StopGrow");
        }

        //Shrink particles
        if (Input.GetKeyDown(KeyCode.Space))
        {
            VFX.SendEvent("SpawnCut");
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            VFX.SendEvent("StopCut");
        }

        if (Physics.Raycast(Cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            VFXPos.transform.position = hit.point;
        }  
    }
}
