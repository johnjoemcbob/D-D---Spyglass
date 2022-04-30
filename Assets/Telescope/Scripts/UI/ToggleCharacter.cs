using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCharacter : MonoBehaviour
{
	public string Character;

	public void OnValueChanged( bool toggle )
	{
		Timeline.Instance.ToggleCharacter( Character, toggle );
	}
}
