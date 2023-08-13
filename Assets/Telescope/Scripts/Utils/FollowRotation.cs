using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public Vector3 Axis;
    public Vector3 Offset;

    public Transform ToFollow;
    
    void Update()
    {
        transform.eulerAngles = Offset + new Vector3(
            ToFollow.eulerAngles.x * Axis.x,
            ToFollow.eulerAngles.y * Axis.y,
            ToFollow.eulerAngles.z * Axis.z
        );
    }
}
