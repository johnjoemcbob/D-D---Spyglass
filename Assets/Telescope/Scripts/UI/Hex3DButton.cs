using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex3DButton : MonoBehaviour
{
    void Update()
    {
        if ( Input.GetMouseButtonUp( 0 ) )
		{
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;
            LayerMask layer = 1 << LayerMask.NameToLayer( "HexButton" );
            if ( Physics.Raycast( ray, out hit, 10000000, layer ) )
			{
                BoatMover.Instance.transform.position = hit.collider.transform.parent.parent.position;
			}
		}
    }
}
