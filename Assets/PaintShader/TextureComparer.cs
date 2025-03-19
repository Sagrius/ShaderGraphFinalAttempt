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
    private Texture2D covertedRenderTexture;
    // Start is called before the first frame update
    void Start()
    {
        


        Invoke(nameof(InitializeTextures), 3f);
    }

    // Update is called once per frame
    void Update()
    {

        if (covertedRenderTexture != null)
        {
            correctCounter = 0;
            for (int i = 0; i < goalTexture.GetPixels().Length; i++)
            {
                Debug.Log(i);
                if (goalTexture.GetPixels()[i] == covertedRenderTexture.GetPixels()[i])
                {
                    correctCounter++;
                    precentageText.text = $"{(int)correctCounter / goalTexture.GetPixels().Length}%";
                }
            }
        }
       
    }

    public void InitializeTextures()
    {
        RenderTexture shorterName = myPainter.splatmap;
        covertedRenderTexture.ReadPixels(new Rect(0, 0, shorterName.width, shorterName.height), 0, 0);
        covertedRenderTexture.Apply();
        if (shorterName != null)
        {
            Debug.Log("UNITY IS STUPID");
        }
    }
}
