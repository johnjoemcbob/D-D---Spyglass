using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMover : MonoBehaviour
{
	public static BoatMover Instance;

	public Transform[] PathPoints;

	public int CurrentPoint = 0;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Alpha1 ) )
		{
			MoveBoatToNext();
		}
	}

	public void MoveBoatToIndex( int index )
	{
		transform.position = PathPoints[index].position;
		CurrentPoint = index;
	}
	public void MoveBoatToNext()
	{
		MoveBoatToIndex( CurrentPoint + 1 );
	}
}
