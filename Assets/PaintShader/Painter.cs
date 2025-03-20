using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Painter : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private Shader hlslShader;

    [SerializeField] private  Color PaintColor = Color.cyan;

    [SerializeField] [Range(1, 500)] private float size = 20;
    [SerializeField] [Range(0,1)] private float strength = 0.35f;

    private RenderTexture renderTexture;
    private Material currentMaterial, drawMaterial;

    private RaycastHit hit;

    void Start()
    {
        //Material created using the shader
        drawMaterial = new Material(hlslShader);
       
        //The material that is from the shader graph
        currentMaterial = new(GetComponent<MeshRenderer>().material);
        gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
        //The render texture we're drawing into , make a seperate instance for every script holder
        renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        renderTexture.Create();
        //Sets the render texture to the default texture 
        Graphics.Blit(currentMaterial.GetTexture("_MainTexture"),renderTexture);
        //Sets the render texture to the shader graph
        currentMaterial.SetTexture("_RenderTexture", renderTexture);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject != gameObject) return;

                currentMaterial.SetFloat("_BrushPower",strength);

                drawMaterial.SetVector("_Color", PaintColor);
                drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_Strength",strength);
                drawMaterial.SetFloat("_Size",size);

                //The act saving the changes
                RenderTexture temp = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(renderTexture, temp);
                Graphics.Blit(temp, renderTexture, drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }
}
