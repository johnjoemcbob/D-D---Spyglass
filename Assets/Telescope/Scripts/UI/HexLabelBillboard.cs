using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexLabelBillboard : MonoBehaviour
{
    void Update()
    {
        if ( Compass.Instance != null )
        {
            //transform.LookAt( Compass.Instance.Camera.transform );
            transform.LookAt( transform.position - Compass.Instance.Camera.transform.forward );
            transform.eulerAngles = new Vector3( 90, transform.eulerAngles.y - 180, 0 );
        }
	}
}
