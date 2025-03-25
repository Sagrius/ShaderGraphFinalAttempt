using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class HairStyleComparisonManager : MonoBehaviour
{
    [Serializable]
    public class HairStyleChallenge
    {
        public string challengeName;
        public Texture2D playerHairTexture;
        public Texture2D targetHairTexture;
        public float difficultyLevel;
    }

    [Header("Challenges")]
    public List<HairStyleChallenge> hairChallenges = new List<HairStyleChallenge>();
    public int currentChallengeIndex = 0;

    [Header("UI Components")]
    public Image playerHairImage;
    public Image targetHairImage;
    public Image mismatchOverlay;
    public Slider scoreSlider;
    public Text scoreText;
    public Text challengeNameText;

    [Header("Material Settings")]
    public Material comparisonMaterial;

    [Header("Difficulty Scaling")]
    public float highResolutionThreshold = 2048f;
    public float lowResolutionThreshold = 512f;

    void Start()
    {
        // Validate challenges
        if (hairChallenges.Count == 0)
        {
            Debug.LogError("No hair challenges defined!");
            return;
        }

        // Load initial challenge
        LoadChallenge(currentChallengeIndex);
    }

    public void LoadChallenge(int index)
    {
        if (index < 0 || index >= hairChallenges.Count)
        {
            Debug.LogError($"Invalid challenge index: {index}");
            return;
        }

        HairStyleChallenge currentChallenge = hairChallenges[index];

        // Update UI
        challengeNameText.text = currentChallenge.challengeName;
        playerHairImage.sprite = TextureToSprite(currentChallenge.playerHairTexture);
        targetHairImage.sprite = TextureToSprite(currentChallenge.targetHairTexture);

        // Apply textures to comparison material
        ApplyTexturesToMaterial(currentChallenge);
    }

    private void ApplyTexturesToMaterial(HairStyleChallenge challenge)
    {
        if (comparisonMaterial == null)
        {
            Debug.LogError("Comparison material is not assigned!");
            return;
        }

        // Calculate resolution difficulty
        float resolutionDifficulty = CalculateResolutionDifficulty(
            challenge.playerHairTexture,
            challenge.targetHairTexture
        );

        // Set shader properties
        comparisonMaterial.SetTexture("_PlayerHairTex", challenge.playerHairTexture);
        comparisonMaterial.SetTexture("_TargetHairTex", challenge.targetHairTexture);

        // Adjust difficulty-based thresholds
        comparisonMaterial.SetFloat("_ResolutionFactor", resolutionDifficulty);
        comparisonMaterial.SetFloat("_ColorSimilarityThreshold", 0.1f);
        comparisonMaterial.SetFloat("_ShapeSimilarityThreshold", 0.1f);
        comparisonMaterial.SetFloat("_MismatchIntensity", 0.5f);
    }

    private float CalculateResolutionDifficulty(Texture2D playerTexture, Texture2D targetTexture)
    {
        float playerResolution = Mathf.Max(playerTexture.width, playerTexture.height);
        float targetResolution = Mathf.Max(targetTexture.width, targetTexture.height);

        // High resolution: more precise matching required
        if (playerResolution >= highResolutionThreshold && targetResolution >= highResolutionThreshold)
        {
            return 1f; // Highest difficulty
        }
        // Low resolution: more lenient comparison
        else if (playerResolution <= lowResolutionThreshold && targetResolution <= lowResolutionThreshold)
        {
            return 0.5f; // Lowest difficulty
        }

        // Medium resolution
        return 0.75f;
    }

    // Utility method to convert Texture2D to Sprite
    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    // Navigation Methods
    public void NextChallenge()
    {
        currentChallengeIndex = (currentChallengeIndex + 1) % hairChallenges.Count;
        LoadChallenge(currentChallengeIndex);
    }

    public void PreviousChallenge()
    {
        currentChallengeIndex = (currentChallengeIndex - 1 + hairChallenges.Count) % hairChallenges.Count;
        LoadChallenge(currentChallengeIndex);
    }
}