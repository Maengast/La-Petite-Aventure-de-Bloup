using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript: MonoBehaviour
{
    private Slider slider;
    public Gradient Gradient;
    public Image Fill;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }
    

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;
        Fill.color = Gradient.Evaluate(1.0f);
    }

    public void SetValue(float value)
    {
        slider.value = value;
        Fill.color = Gradient.Evaluate(slider.normalizedValue);
    }
}
