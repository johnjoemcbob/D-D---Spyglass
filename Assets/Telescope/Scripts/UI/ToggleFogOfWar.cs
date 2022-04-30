using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFogOfWar : MonoBehaviour
{
    public void OnValueChanged( bool toggle )
	{
		Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer( "FogOfWar" );
	}
}
