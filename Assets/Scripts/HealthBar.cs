using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class HealthBar : MonoBehaviour 
{
    [Header("Core Settings")]
    [SerializeField] private Image healthFill;
    [SerializeField] private float updateSpeed = 5f;
    
    [Header("Visual Feedback")]
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadedAlpha = 0.4f;

    private CanvasGroup canvasGroup;
    private float targetFill = 1f; // Start full (1 = full health)

    void Awake()
    {
        // Auto-get references if not assigned
        if (healthFill == null) healthFill = GetComponentInChildren<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Initialize to full health
        healthFill.fillAmount = 1f;
        healthFill.color = colorGradient.Evaluate(1f);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        // Convert to 0-1 range (1 = full, 0 = empty)
        targetFill = currentHealth / maxHealth;
        
        // Visual feedback
        healthFill.color = colorGradient.Evaluate(targetFill);
        ShowHealthBar();
    }

    void Update()
    {
        // Smooth fill animation
        healthFill.fillAmount = Mathf.Lerp(
            healthFill.fillAmount,
            targetFill,
            updateSpeed * Time.deltaTime
        );
    }

    void ShowHealthBar()
    {
        canvasGroup.alpha = 1f;
        CancelInvoke(nameof(FadeOut));
        Invoke(nameof(FadeOut), fadeDelay);
    }

    void FadeOut() => canvasGroup.alpha = fadedAlpha;
}