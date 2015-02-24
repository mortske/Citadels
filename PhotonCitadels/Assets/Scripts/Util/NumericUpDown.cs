using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class NumericUpDown : MonoBehaviour 
{
    public InputField valueDisplay;
    public Button PlusButton;
    public Button MinusButton;

    public int maxValue;
    public int minValue;
    public int curValue;

    void Start()
    {
        valueDisplay.text = curValue.ToString();
        AdjustToLimits();
        PlusButton.onClick.AddListener(() => { Add(); });
        MinusButton.onClick.AddListener(() => { Negate(); });
        valueDisplay.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>((string value) => { ValueChanged(value); }));
        valueDisplay.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>((string value) => { EndEdit(value); }));
    }

    void Add()
    {
        curValue++;
        AdjustToLimits();
        valueDisplay.text = curValue.ToString();
        
    }

    void Negate()
    {
        curValue--;
        AdjustToLimits();
        valueDisplay.text = curValue.ToString();
    }

    void ValueChanged(string value)
    {
        valueDisplay.text = Regex.Replace(valueDisplay.text, @"[^0-9]", "");
    }

    void EndEdit(string value)
    {
        if (valueDisplay.text != "")
        {
            curValue = int.Parse(valueDisplay.text);
        }
        else
        {
            curValue = minValue;
        }
        AdjustToLimits();
        valueDisplay.text = curValue.ToString();
    }

    void AdjustToLimits()
    {
        PlusButton.interactable = true;
        MinusButton.interactable = true;
        if (curValue <= minValue)
        {
            curValue = minValue;
            MinusButton.interactable = false;
        }

        if (curValue >= maxValue)
        {
            curValue = maxValue;
            PlusButton.interactable = false;
        }
    }

    public void ToggleLock(bool locked)
    {
        valueDisplay.interactable = !locked;
        PlusButton.interactable = !locked;
        MinusButton.interactable = !locked;
    }

    public void SetValue(int val)
    {
        curValue = val;
        AdjustToLimits();
        valueDisplay.text = curValue.ToString();
    }
}
