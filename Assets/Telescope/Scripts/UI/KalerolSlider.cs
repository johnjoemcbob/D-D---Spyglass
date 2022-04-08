using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KalerolSlider : MonoBehaviour
{
    void Start()
    {
        var slider = GetComponent<Slider>();
        slider.minValue = KalerolsTower.Instance.FloorHeightMin;
        slider.maxValue = KalerolsTower.Instance.FloorHeightMax;
        slider.value = slider.minValue;
    }

    void Update()
    {
        
    }

    public void SliderValueChanged( float value )
	{
        KalerolsTower.Instance.CurrentFloorHeight = value;
	}
}
