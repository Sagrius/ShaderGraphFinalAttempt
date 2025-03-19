using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Painter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Shader drawShader;

    private RenderTexture splatmap;
    private Material currentMaterial, drawMaterial;
    private RaycastHit hit;

    [SerializeField] [Range(1, 500)] private float size;
    [SerializeField] [Range(0,1)] private float strength;
    // Start is called before the first frame update
    void Start()
    {
        drawMaterial = new Material(drawShader);
        drawMaterial.SetVector("_Color", Color.red);

        currentMaterial = GetComponent<MeshRenderer>().material;

        splatmap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        currentMaterial.SetTexture("_SplatMap",splatmap);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_Strength",strength);
                drawMaterial.SetFloat("_Size",size);
                RenderTexture temp = RenderTexture.GetTemporary(splatmap.width, splatmap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(splatmap,temp);
                Graphics.Blit(temp,splatmap,drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }
}
