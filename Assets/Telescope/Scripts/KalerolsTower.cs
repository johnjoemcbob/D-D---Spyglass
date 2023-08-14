using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalerolsTower : MonoBehaviour
{
    public static KalerolsTower Instance;

    public static float CurrentFloorHeight;

    public float Speed = 5;
    public float YawSpeed = 1;
    public float FloorRotateSpeed = 1;
    public float FloorHeightMin = -2.25f;
    public float FloorHeightMax = 5;
    public float HeightLerpSpeed = 0.2f;

    public Transform FloorsParent;
    public Transform[] Skulls;


	private void Awake()
	{
        Instance = this;
    }

	void Start()
    {
        CurrentFloorHeight = FloorHeightMin;
    }

    void Update()
    {
		foreach ( var skull in Skulls )
		{
            float current = ( Skulls.Length / (float) skull.GetSiblingIndex() );
            float speed = Speed * current;
            skull.localEulerAngles = new Vector3( Mathf.Sin( Time.time * speed ), Time.deltaTime * YawSpeed * current, Mathf.Cos( Time.time * speed ) );
		}
        FloorsParent.localEulerAngles += new Vector3( 0, Time.deltaTime * FloorRotateSpeed, 0 );
        FloorsParent.localPosition = Vector3.Lerp( FloorsParent.localPosition, Vector3.up * CurrentFloorHeight, Time.deltaTime * HeightLerpSpeed );
    }
}
