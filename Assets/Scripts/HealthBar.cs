using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour 
{
    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private float flashDuration = 0.3f;

    [Header("Animation Settings")]
    [SerializeField] private float smoothSpeed = 5f;

    private Slider slider;
    private Image fillImage;
    private float targetValue;
    private float lastDamageTime;

    void Awake()
    {
        slider = GetComponent<Slider>();
        fillImage = slider.fillRect.GetComponent<Image>();
        ResetToNormalColor();
        InitializeHealth();
    }

    void InitializeHealth()
    {
        slider.value = 1f;
        targetValue = 1f;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float newValue = Mathf.Clamp01(currentHealth / maxHealth);
        
        // Check if we took damage (value decreased)
        if (newValue < targetValue)
        {
            fillImage.color = damageColor;
            lastDamageTime = Time.time;
        }
        
        targetValue = newValue;
    }

    void Update()
    {
        // Smooth value animation
        slider.value = Mathf.Lerp(slider.value, targetValue, smoothSpeed * Time.deltaTime);
        
        // Return to normal color after flash duration
        if (Time.time > lastDamageTime + flashDuration)
        {
            ResetToNormalColor();
        }
    }

    void ResetToNormalColor()
    {
        fillImage.color = normalColor;
    }
}