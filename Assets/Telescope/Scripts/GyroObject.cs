using UnityEngine;

public class GyroObject : MonoBehaviour
{
    private Quaternion _origin = Quaternion.identity;

    private void getOrigin()
    {
        _origin = Input.gyro.attitude;
    }

    private void Start()
    {
        Input.gyro.enabled = true;
        getOrigin();
    }

    void Update()
    {
        transform.rotation = ConvertRightHandedToLeftHandedQuaternion( Input.gyro.attitude );
    }

    private Quaternion ConvertRightHandedToLeftHandedQuaternion( Quaternion rightHandedQuaternion )
    {
        return new Quaternion( -rightHandedQuaternion.x,
            -rightHandedQuaternion.z,
            -rightHandedQuaternion.y,
            rightHandedQuaternion.w );
    }

    private static Quaternion GyroToUnity( Quaternion q )
    {
        return new Quaternion( q.x, q.y, -q.z, -q.w );
    }
}