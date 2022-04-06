using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public static Compass Instance;

    public float Offset = -90;
    public Transform ArrowUIObject;
    public Camera Camera;

	private void Awake()
	{
        Instance = this;
    }

	void Start()
    {
        
    }

    void Update()
    {
        var targetAngle = Camera.transform.eulerAngles.y + Offset;
        ArrowUIObject.eulerAngles = new Vector3( 0, 0, targetAngle );
    }
}
