using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayColorChanger : MonoBehaviour
{

    public List<Painter> paintingObjectsPainters;
    public List<Color> colorPallet;
    // Start is called before the first frame update
   

    public void ChangeCokor(int index)
    {
        Debug.Log($"Changed To {colorPallet[index]}");
        foreach(var brush in paintingObjectsPainters)
        {

            brush.PaintColor = colorPallet[index];

            //    Debug.Log($"there are {brush.materials.Length} materials");
            //    brush.gameObject.GetComponent<Painter>().currentColorIndex = index;
            //    //Debug.Log($"Changed to index {index} while there are {brush.materials.Length} Materials");
            //    brush. SetColor("_DisplayColor", colorPallet[index]);
        }
    }
}
