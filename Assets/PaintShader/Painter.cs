using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Painter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Shader hlslShader;

    private RenderTexture renderTexture;
    private Material currentMaterial, drawMaterial;
    private RaycastHit hit;

    [SerializeField] [Range(1, 500)] private float size;
    [SerializeField] [Range(0,1)] private float strength;
    // Start is called before the first frame update
    void Start()
    {
        //Material created using the shader
        drawMaterial = new Material(hlslShader);
        drawMaterial.SetVector("_Color", Color.red);
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
