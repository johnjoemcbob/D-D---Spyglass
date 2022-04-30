using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourCharacter : MonoBehaviour
{
	public string Character;

	public Image Image;

	public void Init( string character, Color colour )
	{
		Character = character;

		GetComponent<FlexibleColorPicker>().color = colour;
		Image.color = colour;
	}

    public void OnValueChanged( Color colour )
	{
		Timeline.Instance.SetColour( Character, colour );
		Image.color = colour;
	}
}
