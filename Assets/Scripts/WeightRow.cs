using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeightRow : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] Slider slider;
    [SerializeField] Toggle enabledToggle;
    [SerializeField] TMP_Text valueText;

    private ColorScript colorScript;

    public void Bind(ColorScript cs)
    {
        colorScript = cs;
        label.text = cs.gameObject.name;

        slider.minValue = 0;
        slider.maxValue = 50;
        slider.wholeNumbers = true;
        slider.value = cs.GetRandomInitWeight();

        enabledToggle.isOn = cs.GetRandomInitWeight() > 0;

        UpdateValue((int)slider.value);

        slider.onValueChanged.AddListener(v =>
        {
            int w = Mathf.RoundToInt(v);
            colorScript.SetRandomInitWeight(w);
            enabledToggle.isOn = w > 0;
            UpdateValue(w);
        });

        enabledToggle.onValueChanged.AddListener(on =>
        {
            if (!on) colorScript.SetRandomInitWeight(0);
            else if (colorScript.GetRandomInitWeight() == 0) colorScript.SetRandomInitWeight(1);

            slider.value = colorScript.GetRandomInitWeight();
            UpdateValue(colorScript.GetRandomInitWeight());
        });
    }

    private void UpdateValue(int w)
    {
        if (valueText) valueText.text = w.ToString();
    }
}
