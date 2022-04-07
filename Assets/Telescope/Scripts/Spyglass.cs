using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spyglass : MonoBehaviour
{
    public static Spyglass Instance;

    public float Speed = 5;
    public float ZoomSpeed = 2.5f;
    public float ZoomDeadzone = 0.5f;
    public float MaxZoom = 2;
    public bool Newer = false;

    public Transform Gyro;

    Gyroscope m_Gyro;

    [HideInInspector]
    public float Zoom = 1;
    [HideInInspector]
    public Vector3 GyroRotation;
    [HideInInspector]
    public float LastRoll = 0;

    private float InitialBend;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Screen.sleepTimeout = (int) SleepTimeout.NeverSleep;

        //Set up and enable the gyroscope (check your device has one)
        m_Gyro = Input.gyro;
        m_Gyro.enabled = true;

        InitialBend = BendingManager.Instance.bendingAmount;
    }

    void Update()
    {
        GyroModifyCamera();

        // Invert if accidentally upsidedown?
        if ( !Newer )
        {
            if ( transform.up.y < 0 )
            {
                transform.eulerAngles = new Vector3( -transform.eulerAngles.x, transform.eulerAngles.y, 0 );
            }
        }
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.y, 0 );

        // Try zoom in/out with roll
        float roll = GyroToUnity( m_Gyro.rotationRate ).y;
        if ( !Newer )
        {
            roll = GyroToUnity( m_Gyro.rotationRate ).z;
        }
        if ( roll >= ZoomDeadzone )
        {
            Zoom = MaxZoom;
            transform.eulerAngles = new Vector3( 0, transform.eulerAngles.y, 0 );
        }
        if ( roll <= -ZoomDeadzone )
        {
            Zoom = 1;
            transform.eulerAngles = new Vector3( 0, transform.eulerAngles.y, 0 );
        }
        // Zoom separate slightly for sync network reasons
        if ( Zoom == MaxZoom )
        {
            BendingManager.Instance.bendingAmount = InitialBend * 1.5f;
        }
        else
        {
            BendingManager.Instance.bendingAmount = InitialBend;
        }
        LastRoll = roll;
        var child = transform.GetChild( 0 );
        child.localPosition = Vector3.Lerp( child.localPosition, Vector3.forward * Zoom, Time.deltaTime * ZoomSpeed );
    }

    void GyroModifyCamera()
    {
        if ( Newer )
        {
            // Rotation via rotation rate
            GyroRotation += GyroToUnity( m_Gyro.rotationRate ) * Speed;

            // Actual camera
            transform.rotation = GyroToUnity( m_Gyro.attitude );
            transform.Rotate( Vector3.right, 90 );
            transform.eulerAngles = new Vector3( transform.eulerAngles.x, -GyroRotation.x, 0 );
        }
        else
        {
            transform.eulerAngles += GyroToUnity( m_Gyro.rotationRate ) * Speed;
        }
    }

    private static Quaternion GyroToUnity( Quaternion q )
    {
        return new Quaternion( -q.x, -q.z, -q.y, q.w );
    }

    private static Vector3 GyroToUnity( Vector3 q )
    {
        if ( Spyglass.Instance.Newer )
        {
            return new Vector3( q.y, q.z, q.x );
        }
        else
        {
            return new Vector3( -q.x, -q.y, q.z );
        }
    }
}