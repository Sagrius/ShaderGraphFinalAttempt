using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayColorChanger : MonoBehaviour
{

    public List<Renderer> paintingObjectsPainters;
    public List<Color> colorPallet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCokor(int index)
    {
        Debug.Log($"Changed To {colorPallet[index]}");
        foreach(Renderer brush in paintingObjectsPainters)
        {
            //brush.gameObject.GetComponent<Painter>().currentColorIndex = index;
            //Debug.Log($"Changed to index {index} while there are {brush.materials.Length} Materials");
            brush.material.SetColor("_DisplayColor", colorPallet[index]);
        }
    }
}
