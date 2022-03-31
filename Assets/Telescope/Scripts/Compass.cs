using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public float Offset = -90;
    public Transform ArrowUIObject;

    void Start()
    {
        
    }

    void Update()
    {
        var targetAngle = Camera.main.transform.eulerAngles.y + Offset;
        ArrowUIObject.eulerAngles = new Vector3( 0, 0, targetAngle );
    }
}
