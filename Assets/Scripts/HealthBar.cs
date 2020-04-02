using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    private Slider slider;
    public Gradient Gradient;
    public Image Fill;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        Fill.color = Gradient.Evaluate(1.0f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        Fill.color = Gradient.Evaluate(slider.normalizedValue);
    }
}
