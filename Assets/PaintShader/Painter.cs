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

    // Start is called before the first frame update
    void Start()
    {
        //Material created using the shader
        drawMaterial = new Material(hlslShader);
       
        //The material that is from the shader graph
        currentMaterial = new(GetComponent<MeshRenderer>().material);
        gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
        //The material we're drawing into
        renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        renderTexture.Create();

        currentMaterial.SetTexture("_RenderTexture", renderTexture);
    }

    // Update is called once per frame
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

                RenderTexture temp = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(renderTexture, temp);
                Graphics.Blit(temp, renderTexture, drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }
}
