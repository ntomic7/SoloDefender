using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider slider;

    [SerializeField] TextMeshProUGUI textMeshPro;

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetCurrentValue(int currentValue)
    {
        slider.value = currentValue;
    }

    public void SetValue(int amount)
    {
        slider.value += amount;

        // delete later
        if(this.gameObject.name == "EnemyHealthBar")
        textMeshPro.text = slider.value.ToString() + " / " + slider.maxValue.ToString();

    }
}
