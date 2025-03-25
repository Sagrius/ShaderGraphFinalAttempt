using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayColorChanger : MonoBehaviour
{

    public List<Painter> paintingObjectsPainters;
    public List<Color> colorPallet;
    public Slider sizeSlider;
    public Slider strengrthSlider;
    // Start is called before the first frame update
    void Start()
    {
        ChangeSize();
        ChangeStrength();

    }

    // Update is called once per frame
   
    public void ChangeCokor(int index)
    {
        Debug.Log($"Changed To {colorPallet[index]}");
        foreach(var brush in paintingObjectsPainters)
        {
            brush.PaintColor = colorPallet[index];
        }
    }

    public void ChangeSize()
    {
        foreach (var brush in paintingObjectsPainters)
        {
            brush.size = sizeSlider.value;
        }
    }

    public void ChangeStrength()
    {
        foreach (var brush in paintingObjectsPainters)
        {
            brush.strength = strengrthSlider.value;
        }
    }

}
