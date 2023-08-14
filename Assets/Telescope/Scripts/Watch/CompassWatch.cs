using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassWatch : MonoBehaviour
{
    public static float Rotation = 0;

    public Transform Needle;

    void Update()
    {
        Needle.localEulerAngles = new Vector3( 0, 0, Rotation );
    }
}
