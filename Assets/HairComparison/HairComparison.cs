using UnityEngine;
using TMPro;

public class TextureComparison : MonoBehaviour
{
    [SerializeField] private Painter painterScript;        // Reference to the Painter script
    [SerializeField] private Texture2D targetTexture;      // The target/reference texture
    [SerializeField] private TextMeshProUGUI scoreText;    // Text to display score percentage

    // Temporary texture for converting 
    private Texture2D painterTexture2D;

    // Result percentage
    private float matchPercentage = 0.0f;

    private void Start()
    {
        if (painterScript == null)
        {
            Debug.LogError("Painter script reference is missing!");
            return;
        }

        // Create texture for reading pixels
        painterTexture2D = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
    }

    /// <summary>
    /// Compares pixels between painter's current material texture and target texture
    /// Returns a match percentage (0-100)
    /// </summary>
    public float CompareTextures()
    {
        if (painterScript == null || targetTexture == null)
        {
            Debug.LogError("Painter script or target texture is missing!");
            return 0f;
        }

        // Get the current material from the Painter script
        Material currentMaterial = painterScript.currentMaterial;
        if (currentMaterial == null)
        {
            Debug.LogError("Could not access painter's current material!");
            return 0f;
        }

        // Get the render texture from the current material
        Texture renderTexture = currentMaterial.GetTexture("_RenderTexture");
        if (renderTexture == null)
        {
            Debug.LogError("Current material doesn't have a _RenderTexture!");
            return 0f;
        }

        // Convert texture to Texture2D for comparison
        ConvertTextureToTexture2D(renderTexture, painterTexture2D);

        // Compare pixels and get percentage
        matchPercentage = ComparePixels(painterTexture2D, targetTexture);

        return matchPercentage;
    }

    /// <summary>
    /// Converts any texture to a Texture2D
    /// </summary>
    private void ConvertTextureToTexture2D(Texture sourceTexture, Texture2D texture2D)
    {
        // Create a temporary RenderTexture
        RenderTexture tempRT = RenderTexture.GetTemporary(
            sourceTexture.width,
            sourceTexture.height,
            0,
            RenderTextureFormat.ARGB32
        );

        // Copy source texture to the temporary RenderTexture
        Graphics.Blit(sourceTexture, tempRT);

        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // Set the temp render texture as active and read its pixels
        RenderTexture.active = tempRT;
        texture2D.Resize(tempRT.width, tempRT.height);
        texture2D.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        texture2D.Apply();

        // Restore previously active render texture
        RenderTexture.active = currentActiveRT;

        // Release the temporary render texture
        RenderTexture.ReleaseTemporary(tempRT);
    }

    /// <summary>
    /// Compares pixels between two textures and returns match percentage
    /// </summary>
    private float ComparePixels(Texture2D texture1, Texture2D texture2)
    {
        // Ensure target texture is readable
        if (!targetTexture.isReadable)
        {
            Debug.LogError("Target texture must be marked as readable in import settings!");
            return 0f;
        }

        // Get dimensions (use smallest for comparison)
        int width = Mathf.Min(texture1.width, texture2.width);
        int height = Mathf.Min(texture1.height, texture2.height);

        // Get pixel data
        Color32[] pixels1 = texture1.GetPixels32();
        Color32[] pixels2 = texture2.GetPixels32();

        int totalPixels = width * height;
        int matchingPixels = 0;
        int coloredPixels = 0;

        // Compare each pixel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculate indices in the original textures
                int index1 = y * texture1.width + x;
                int index2 = y * texture2.width + x;

                // Make sure we don't go out of bounds
                if (index1 >= pixels1.Length || index2 >= pixels2.Length)
                {
                    continue;
                }

                // Get pixels from both textures
                Color32 pixel1 = pixels1[index1];
                Color32 pixel2 = pixels2[index2];

                // Check if this is a colored pixel in target texture (not transparent)
                bool isTargetColored = pixel2.a > 128;

                if (isTargetColored)
                {
                    coloredPixels++;

                    // Simple color comparison - consider match if colors are very close
                    if (ColorDifference(pixel1, pixel2) < 0.1f)
                    {
                        matchingPixels++;
                    }
                }
            }
        }

        // Calculate match percentage
        float percentage = (coloredPixels > 0) ? ((float)matchingPixels / coloredPixels * 100f) : 0f;

        return percentage;
    }

    private float ColorDifference(Color32 a, Color32 b)
    {
        // Simple color difference calculation
        float rDiff = Mathf.Abs(a.r - b.r) / 255f;
        float gDiff = Mathf.Abs(a.g - b.g) / 255f;
        float bDiff = Mathf.Abs(a.b - b.b) / 255f;

        return (rDiff + gDiff + bDiff) / 3f;
    }

    private void Update()
    {
        // Check for Z key press to perform comparison
        if (Input.GetKeyDown(KeyCode.Z))
        {
            float percentage = CompareTextures();

            // Display score as percentage
            if (scoreText != null)
            {
                scoreText.text = percentage.ToString("F1") + "%";

                // Color based on match percentage
                if (percentage >= 90f)
                    scoreText.color = Color.green;
                else if (percentage >= 70f)
                    scoreText.color = Color.yellow;
                else
                    scoreText.color = Color.red;
            }

            Debug.Log($"Texture comparison: {percentage:F1}% match");
        }
    }
}