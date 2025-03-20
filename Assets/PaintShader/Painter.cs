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
    private RenderTexture displacementTexture;
    private Material currentMaterial, drawMaterial, displacementMat;

    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        //Material created using the shader
        drawMaterial = new Material(hlslShader);

        displacementMat = new Material(hlslShader);
       
        //The material that is from the shader graph
        currentMaterial = new(GetComponent<MeshRenderer>().material);
        gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
        //The render texture we're drawing into , make a seperate instance for every script holder
        renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        renderTexture.Create();

        displacementTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        displacementTexture.Create();
        //Sets the render texture to the default texture 
        Graphics.Blit(currentMaterial.GetTexture("_MainTexture"), renderTexture);

        //Sets the render texture to the shader graph
        currentMaterial.SetTexture("_RenderTexture", renderTexture);

        currentMaterial.SetTexture("_DisplacementTex", displacementTexture);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject != gameObject) return;

                drawMaterial.SetVector("_Color", PaintColor);
                drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_Strength",strength);
                drawMaterial.SetFloat("_Size",size);

                RenderTexture temp = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(renderTexture, temp);
                Graphics.Blit(temp, renderTexture, drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject != gameObject) return;

                displacementMat.SetVector("_Color", Color.blue);
                displacementMat.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                displacementMat.SetFloat("_Strength", strength);
                displacementMat.SetFloat("_Size", size);

                RenderTexture temp = RenderTexture.GetTemporary(displacementTexture.width, displacementTexture.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(displacementTexture, temp);
                Graphics.Blit(temp, displacementTexture, displacementMat);
                RenderTexture.ReleaseTemporary(temp);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject != gameObject) return;

                displacementMat.SetVector("_Color", Color.black);
                displacementMat.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                displacementMat.SetFloat("_Strength", strength);
                displacementMat.SetFloat("_Size", size);

                RenderTexture temp = RenderTexture.GetTemporary(displacementTexture.width, displacementTexture.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(displacementTexture, temp);
                Graphics.Blit(temp, displacementTexture, displacementMat);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }
}
