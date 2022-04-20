using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float LiveTime = 1;
	public float MinScale = 0.001f;
	public float MaxScale = 1;


	void Start()
    {
		StartCoroutine( PulseOut() );
    }

	IEnumerator PulseOut()
	{
		int steps = (int) LiveTime * 60;
		for ( int step = 0; step < steps; step++ )
		{
			transform.localScale = Vector3.Lerp( Vector3.one * MinScale, Vector3.one * MaxScale, (float) step / steps );
			yield return new WaitForSeconds( LiveTime / steps );
		}

		Destroy( gameObject );
	}
}
