using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugGyroUI : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 forw = FindObjectOfType<Spyglass>().Gyro.forward;
        Vector3 ang = FindObjectOfType<Spyglass>().Gyro.eulerAngles;
        Vector3 test = new Vector3( ang.x * forw.x, ang.y * forw.y, ang.z * forw.z );
        GetComponent<Text>().text = Input.gyro.attitude.eulerAngles.ToString() + "\n" + ang + " : " + FindObjectOfType<Spyglass>().LastRoll;
    }
}
