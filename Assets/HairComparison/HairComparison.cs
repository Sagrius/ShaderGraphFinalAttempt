using UnityEngine;
using System.Collections;

public class HairComparison : MonoBehaviour
{
    [Header("Texture References")]
    [SerializeField] private Texture2D playerHairTexture;
    [SerializeField] private Texture2D targetHairTexture;
    [SerializeField] private RenderTexture resultRenderTexture;

    [Header("Shader References")]
    [SerializeField] private Material hairComparisonMaterial;

    [Header("Comparison Settings")]
    [SerializeField][Range(0.0f, 1.0f)] private float colorMatchWeight = 0.4f;
    [SerializeField][Range(0.0f, 1.0f)] private float shapeMatchWeight = 0.4f;
    [SerializeField][Range(0.0f, 1.0f)] private float lengthMatchWeight = 0.2f;
    [SerializeField][Range(0.0f, 1.0f)] private float difficultyMultiplier = 0.5f;
    [SerializeField] private bool isHighResolutionMode = true;

    // Results
    private float overallSimilarityScore = 0.0f;
    private float colorSimilarityScore = 0.0f;
    private float shapeSimilarityScore = 0.0f;
    private float lengthSimilarityScore = 0.0f;

    // Cache for analyzed pixels
    private Color32[] playerPixels;
    private Color32[] targetPixels;
    private int width, height;

    // Texture analysis results
    private Texture2D mismatchMapTexture;

    private void Start()
    {
        if (hairComparisonMaterial == null)
        {
            Debug.LogError("Hair comparison material is not assigned!");
            return;
        }

        // Create mismatch map texture
        mismatchMapTexture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
    }

    /// <summary>
    /// Performs hair style comparison between player's hair and target hair
    /// </summary>
    public float CompareHairStyles()
    {
        if (playerHairTexture == null || targetHairTexture == null)
        {
            Debug.LogError("Player or target hair textures are missing!");
            return 0f;
        }

        // Determine resolution mode based on texture size
        isHighResolutionMode = (playerHairTexture.width >= 512 || playerHairTexture.height >= 512);

        // Prepare textures for analysis
        PrepareTexturesForAnalysis();

        // Perform pixel-based comparison
        AnalyzeHairTextures();

        // Generate visualization of comparison results
        GenerateComparisonVisualization();

        // Calculate overall score with difficulty adjustment
        CalculateFinalScore();

        return overallSimilarityScore;
    }

    private void PrepareTexturesForAnalysis()
    {
        // Ensure textures are readable
        if (!playerHairTexture.isReadable || !targetHairTexture.isReadable)
        {
            Debug.LogError("Textures must be marked as readable in import settings!");
            return;
        }

        // Get dimensions (use smallest for comparison)
        width = Mathf.Min(playerHairTexture.width, targetHairTexture.width);
        height = Mathf.Min(playerHairTexture.height, targetHairTexture.height);

        // Get pixel data
        playerPixels = playerHairTexture.GetPixels32();
        targetPixels = targetHairTexture.GetPixels32();
    }

    private void AnalyzeHairTextures()
    {
        // Analysis counters
        int totalPixels = width * height;
        int matchingColorPixels = 0;
        int matchingShapePixels = 0;
        int playerHairPixels = 0;
        int targetHairPixels = 0;

        // Settings adjusted by difficulty
        float colorThreshold = isHighResolutionMode ? 0.1f : 0.2f;
        colorThreshold *= (2.0f - difficultyMultiplier); // Higher difficulty = lower threshold

        // Prepare result texture
        Color32[] mismatchMapPixels = new Color32[totalPixels];

        // Analyze each pixel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                // Get pixels from both textures
                Color32 playerPixel = playerPixels[index];
                Color32 targetPixel = targetPixels[index];

                // Check if this is a hair pixel in either texture
                bool isPlayerHair = playerPixel.a > 128; // Alpha > 0.5
                bool isTargetHair = targetPixel.a > 128; // Alpha > 0.5

                // Count hair pixels for shape analysis
                if (isPlayerHair) playerHairPixels++;
                if (isTargetHair) targetHairPixels++;

                // Color comparison
                float colorDifference = ColorDifference(playerPixel, targetPixel);
                bool isColorMatch = colorDifference < colorThreshold;

                // Count matching pixels
                if (isPlayerHair && isTargetHair)
                {
                    matchingShapePixels++;
                    if (isColorMatch) matchingColorPixels++;
                }

                // Set pixel in mismatch map
                if (isPlayerHair || isTargetHair)
                {
                    if (isPlayerHair && isTargetHair && isColorMatch)
                    {
                        // Good match - green
                        mismatchMapPixels[index] = new Color32(0, 255, 0, 255);
                    }
                    else if (isPlayerHair && isTargetHair)
                    {
                        // Shape match but color mismatch - yellow
                        mismatchMapPixels[index] = new Color32(255, 255, 0, 255);
                    }
                    else if (isPlayerHair)
                    {
                        // Player hair only - red
                        mismatchMapPixels[index] = new Color32(255, 0, 0, 255);
                    }
                    else
                    {
                        // Target hair only - blue
                        mismatchMapPixels[index] = new Color32(0, 0, 255, 255);
                    }
                }
                else
                {
                    // No hair - transparent
                    mismatchMapPixels[index] = new Color32(0, 0, 0, 0);
                }
            }
        }

        // Calculate similarity scores
        int maxHairPixels = Mathf.Max(playerHairPixels, targetHairPixels);
        if (maxHairPixels > 0)
        {
            // Shape similarity based on overlap (Dice coefficient)
            shapeSimilarityScore = (2.0f * matchingShapePixels) / (playerHairPixels + targetHairPixels);

            // Color similarity among matching shape pixels
            colorSimilarityScore = matchingShapePixels > 0 ?
                (float)matchingColorPixels / matchingShapePixels : 0;

            // Length similarity (estimate based on total hair pixels)
            float pixelRatio = (float)Mathf.Min(playerHairPixels, targetHairPixels) /
                               Mathf.Max(playerHairPixels, targetHairPixels);
            lengthSimilarityScore = pixelRatio;
        }

        // Update mismatch map texture
        mismatchMapTexture.SetPixels32(mismatchMapPixels);
        mismatchMapTexture.Apply();
    }

    private float ColorDifference(Color32 a, Color32 b)
    {
        // Simple color difference calculation
        return (Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b)) / (3.0f * 255.0f);
    }

    private void GenerateComparisonVisualization()
    {
        // Set shader properties for visualization
        hairComparisonMaterial.SetTexture("_PlayerHairTex", playerHairTexture);
        hairComparisonMaterial.SetTexture("_TargetHairTex", targetHairTexture);
        hairComparisonMaterial.SetTexture("_MismatchMapTex", mismatchMapTexture);
        hairComparisonMaterial.SetFloat("_OverallScore", overallSimilarityScore);
        hairComparisonMaterial.SetFloat("_ColorScore", colorSimilarityScore);
        hairComparisonMaterial.SetFloat("_ShapeScore", shapeSimilarityScore);

        // Render the result to a render texture
        Graphics.Blit(null, resultRenderTexture, hairComparisonMaterial);
    }

    private void CalculateFinalScore()
    {
        // Calculate weighted score
        float weightSum = colorMatchWeight + shapeMatchWeight + lengthMatchWeight;
        float rawScore = (colorSimilarityScore * colorMatchWeight +
                         shapeSimilarityScore * shapeMatchWeight +
                         lengthSimilarityScore * lengthMatchWeight) / weightSum;

        // Apply difficulty scaling
        if (isHighResolutionMode)
        {
            // High resolution mode is more demanding
            float difficultyFactor = 0.7f + (0.3f * difficultyMultiplier);
            rawScore *= difficultyFactor;
        }
        else
        {
            // Low resolution mode is more forgiving
            float leniencyFactor = 0.8f + (0.2f * (1.0f - difficultyMultiplier));
            rawScore = Mathf.Min(1.0f, rawScore * leniencyFactor);
        }

        overallSimilarityScore = Mathf.Clamp01(rawScore);
    }

    // Getters for UI display
    public float GetOverallScore() => overallSimilarityScore;
    public float GetColorScore() => colorSimilarityScore;
    public float GetShapeScore() => shapeSimilarityScore;
    public float GetLengthScore() => lengthSimilarityScore;
    public Texture2D GetMismatchMapTexture() => mismatchMapTexture;
    public bool IsHighResMode() => isHighResolutionMode;
}