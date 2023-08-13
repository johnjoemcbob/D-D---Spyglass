using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GyroSpeedSlider : MonoBehaviour
{
	public static float GyroModifier = 1;

	private void Start()
	{
		var slider = GetComponent<Slider>();
		OnValueChanged( slider.value );
	}

	public void OnValueChanged( float value )
	{
		GetComponentInChildren<TextMeshProUGUI>().text = ( Mathf.Round( value * 10 ) / 10 ).ToString();
		GyroModifier = value;
	}
}
