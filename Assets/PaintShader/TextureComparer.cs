using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextureComparer : MonoBehaviour
{
    public Painter myPainter;
    public Texture2D goalTexture;
    public TMP_Text precentageText;
    private float correctCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        correctCounter = 0;
        foreach(var pixel in goalTexture.GetPixels())
        {
            //if(pixel)
        }
    }
}
