using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Slider slider;
    public Gradient Gradient;
    public Image Fill;
    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMaxStamina(float stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
        Fill.color = Gradient.Evaluate(1.0f);
    }

    public void SetStamina(float health)
    {
        slider.value = health;
        Fill.color = Gradient.Evaluate(slider.normalizedValue);
    }
}
