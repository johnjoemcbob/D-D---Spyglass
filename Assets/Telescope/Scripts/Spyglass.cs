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
    public float PushbackMult = 0.8f;
    public bool Newer = false;
    public bool Manual = false;
    public bool OrientationSent = false;

    public Transform Gyro;

    Gyroscope m_Gyro;

    [HideInInspector]
    public float Zoom = 1;
    [HideInInspector]
    public Vector3 GyroRotation;
    [HideInInspector]
    public float LastRoll = 0;

    private float InitialBend;
    private Vector3 ManualGyro;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Screen.sleepTimeout = (int) SleepTimeout.NeverSleep;

        if ( !Manual )
        {
            //Set up and enable the gyroscope (check your device has one)
            m_Gyro = Input.gyro;
            m_Gyro.enabled = true;
        }

        InitialBend = BendingManager.Instance.bendingAmount;
    }

    void Update()
    {
        var child = transform.GetChild( 0 );

        if ( Input.GetKeyDown( KeyCode.Space ) )
		{
            transform.eulerAngles = new Vector3( 0, transform.eulerAngles.y, 0 );
		}

        if ( Manual )
		{
            if ( OrientationSent )
            {
                transform.eulerAngles = GyroToUnity( ManualGyro );
                transform.eulerAngles = new Vector3( transform.eulerAngles.x, -transform.eulerAngles.y, transform.eulerAngles.z );
            }
            else
            {
                transform.eulerAngles += GyroToUnity( ManualGyro ) * Speed;
            }
            ManualGyro = Vector3.zero;
        }
        else if ( !Manual )
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
            child.localPosition = Vector3.Lerp( child.localPosition, Vector3.forward * Zoom, Time.deltaTime * ZoomSpeed );
        }

        // Camera raycast collision pushback
        Ray ray = new Ray( Compass.Instance.Camera.transform.parent.position, Compass.Instance.Camera.transform.forward );
        RaycastHit hit;
        LayerMask layer = 1 << LayerMask.NameToLayer( "Default" );
        if ( Physics.Raycast( ray, out hit, Zoom, layer ) )
		{
            child.localPosition = Vector3.forward * hit.distance * PushbackMult;
		}
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

    public void ReceiveGyro( string gyro )
	{
        Vector3 vector = new Vector3();
		{
            string[] split = gyro.Split( '|' );
            if ( split.Length == 3 )
            {
                float.TryParse( split[1], out vector.x );
                float.TryParse( split[0], out vector.y );
                vector.x *= -1;
            }
        }
        ReceiveGyro( vector );
    }

    public void ReceiveGyro( Vector3 gyro )
    {
        ManualGyro = gyro;
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
            return new Vector3( q.x, q.y, q.z );
        }
    }
}