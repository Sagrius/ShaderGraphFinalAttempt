//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections;

//public class HairComparisonUIManager : MonoBehaviour
//{
//    [Header("References")]
  
//    [Header("UI Elements")]
//    [SerializeField] private RawImage comparisonResultDisplay;
//    [SerializeField] private Slider overallScoreSlider;
//    [SerializeField] private Slider colorScoreSlider;
//    [SerializeField] private Slider shapeScoreSlider;
//    [SerializeField] private Slider lengthScoreSlider;
//    [SerializeField] private TextMeshProUGUI scoreText;
//    [SerializeField] private TextMeshProUGUI feedbackText;
//    [SerializeField] private Image difficultyIndicator;

//    [Header("Comparison Mode Controls")]
//    [SerializeField] private ToggleGroup comparisonModeGroup;
//    [SerializeField] private Toggle normalModeToggle;
//    [SerializeField] private Toggle overlayModeToggle;
//    [SerializeField] private Toggle differenceModeToggle;
//    [SerializeField] private Toggle heatmapModeToggle;

//    [Header("Visual Feedback")]
//    [SerializeField] private GameObject perfectMatchEffect;
//    [SerializeField] private Image mismatchOverlay;
//    [SerializeField] private Animator feedbackAnimator;

//    [Header("Rating Stars")]
//    [SerializeField] private Image[] ratingStars;
//    [SerializeField] private Sprite filledStarSprite;
//    [SerializeField] private Sprite emptyStarSprite;
//    [SerializeField] private Sprite halfFilledStarSprite;

//    [Header("UI Settings")]
//    [SerializeField] private Color goodScoreColor = Color.green;
//    [SerializeField] private Color mediumScoreColor = Color.yellow;
//    [SerializeField] private Color badScoreColor = Color.red;

//    private Material comparisonMaterial;
//    private int currentComparisonMode = 0;

//    private void Start()
//    {
//        if (hairComparisonSystem == null)
//        {
//            Debug.LogError("Hair Comparison System reference is missing!");
//            return;
//        }

//        SetupUIControls();

//        // Initial comparison
//        PerformHairComparison();
//    }

//    private void SetupUIControls()
//    {
//        // Setup mode toggles
//        if (normalModeToggle != null)
//        {
//            normalModeToggle.onValueChanged.AddListener((isOn) => {
//                if (isOn) SetComparisonMode(0);
//            });
//        }

//        if (overlayModeToggle != null)
//        {
//            overlayModeToggle.onValueChanged.AddListener((isOn) => {
//                if (isOn) SetComparisonMode(1);
//            });
//        }

//        if (differenceModeToggle != null)
//        {
//            differenceModeToggle.onValueChanged.AddListener((isOn) => {
//                if (isOn) SetComparisonMode(2);
//            });
//        }

//        if (heatmapModeToggle != null)
//        {
//            heatmapModeToggle.onValueChanged.AddListener((isOn) => {
//                if (isOn) SetComparisonMode(3);
//            });
//        }

//        // Get the comparison material
//        if (comparisonResultDisplay != null && comparisonResultDisplay.material != null)
//        {
//            comparisonMaterial = comparisonResultDisplay.material;
//        }
//    }

//    public void PerformHairComparison()
//    {
//        float overallScore = hairComparisonSystem.CompareHairStyles();

//        // Update UI with results
//        UpdateScoreUI(
//            overallScore,
//            hairComparisonSystem.GetColorScore(),
//            hairComparisonSystem.GetShapeScore(),
//            hairComparisonSystem.GetLengthScore()
//        );

//        // Update comparison visualization
//        UpdateComparisonVisualization();

//        // Show appropriate feedback
//        ShowFeedback(overallScore);
//    }

//    private void UpdateScoreUI(float overall, float color, float shape, float length)
//    {
//        // Update sliders
//        if (overallScoreSlider != null) overallScoreSlider.value = overall;
//        if (colorScoreSlider != null) colorScoreSlider.value = color;
//        if (shapeScoreSlider != null) shapeScoreSlider.value = shape;
//        if (lengthScoreSlider != null) lengthScoreSlider.value = length;

//        // Update score text
//        if (scoreText != null)
//        {
//            scoreText.text = $"Overall Score: {overall * 100:F0}%";
//            scoreText.color = GetScoreColor(overall);
//        }

//        // Update star rating (5-star system)
//        UpdateStarRating(overall);

//        // Update difficulty indicator
//        if (difficultyIndicator != null)
//        {
//            difficultyIndicator.color = hairComparisonSystem.IsHighResMode() ?
//                new Color(1f, 0.5f, 0f) : new Color(0.5f, 0.8f, 0.2f);
//        }
//    }

//    private void UpdateStarRating(float score)
//    {
//        if (ratingStars == null || ratingStars.Length == 0) return;

//        // Calculate star rating (0-5 stars based on score)
//        float starRating = score * 5f;

//        for (int i = 0; i < ratingStars.Length; i++)
//        {
//            if (starRating >= i + 1)
//            {
//                // Full star
//                ratingStars[i].sprite = filledStarSprite;
//            }
//            else if (starRating > i && starRating < i + 1)
//            {
//                // Half star
//                ratingStars[i].sprite = halfFilledStarSprite;
//            }
//            else
//            {
//                // Empty star
//                ratingStars[i].sprite = emptyStarSprite;
//            }
//        }
//    }

//    private void UpdateComparisonVisualization()
//    {
//        if (comparisonMaterial != null)
//        {
//            // Set current mode
//            comparisonMaterial.SetInt("_ComparisonMode", currentComparisonMode);

//            //// Update mismatch visualization
//            //if (mismatchOverlay != null)
//            //{
//            //    mismatchOverlay.mainTexture = hairComparisonSystem.GetMismatchMapTexture();
//            //    mismatchOverlay.enabled = currentComparisonMode == 3; // Only visible in heatmap mode
//            //}
//        }
//    }

//    private void ShowFeedback(float score)
//    {
//        if (feedbackText != null)
//        {
//            string feedback;

//            if (score >= 0.95f)
//            {
//                feedback = "Perfect Match! Amazing work!";
//                if (perfectMatchEffect != null) perfectMatchEffect.SetActive(true);
//            }
//            else if (score >= 0.85f)
//            {
//                feedback = "Excellent! Very close to the target style.";
//            }
//            else if (score >= 0.75f)
//            {
//                feedback = "Great job! The style is quite similar.";
//            }
//            else if (score >= 0.65f)
//            {
//                feedback = "Good effort! Getting closer to the style.";
//            }
//            else if (score >= 0.5f)
//            {
//                feedback = "Not bad. Try adjusting the hair a bit more.";
//            }
//            else if (score >= 0.3f)
//            {
//                feedback = "Needs more work. Check the highlighted areas.";
//            }
//            else
//            {
//                feedback = "Try again. The style is quite different from the target.";
//            }

//            feedbackText.text = feedback;
//            feedbackText.color = GetScoreColor(score);

//            // Animate feedback text
//            if (feedbackAnimator != null)
//            {
//                feedbackAnimator.SetTrigger("ShowFeedback");
//            }
//        }

//        // Hide perfect match effect if score is not perfect
//        if (perfectMatchEffect != null && score < 0.95f)
//        {
//            perfectMatchEffect.SetActive(false);
//        }
//    }

//    public void SetComparisonMode(int mode)
//    {
//        currentComparisonMode = mode;
//        UpdateComparisonVisualization();
//    }

//    private Color GetScoreColor(float score)
//    {
//        if (score >= 0.75f) return goodScoreColor;
//        if (score >= 0.45f) return mediumScoreColor;
//        return badScoreColor;
//    }
//}