using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spyglass : MonoBehaviour
{
    public float Speed = 5;

    Gyroscope m_Gyro;

    void Start()
    {
        Screen.sleepTimeout = (int) SleepTimeout.NeverSleep;

        //Set up and enable the gyroscope (check your device has one)
        m_Gyro = Input.gyro;
        m_Gyro.enabled = true;
    }

    void Update()
    {
        GyroModifyCamera();

        if ( transform.up.y < 0 )
		{
            transform.eulerAngles = new Vector3( -transform.eulerAngles.x, transform.eulerAngles.y, 0 );
		}
    }

    void GyroModifyCamera()
    {
        //transform.rotation = GyroToUnity( Input.gyro.attitude );
        transform.eulerAngles += GyroToUnity( m_Gyro.rotationRate ) * Speed;
    }

    private static Quaternion GyroToUnity( Quaternion q )
    {
        return new Quaternion( q.x, q.y, -q.z, -q.w );
    }

    private static Vector3 GyroToUnity( Vector3 q )
    {
        return new Vector3( -q.x, -q.y, 0 );
    }
}