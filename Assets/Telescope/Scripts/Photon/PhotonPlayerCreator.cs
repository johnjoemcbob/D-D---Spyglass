using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PhotonPlayerCreator : MonoBehaviourPun
{
	public static Vector3 Spawn = new Vector3( 0, 0, 0 );

	public void OnJoinedRoom()
    {
        CreatePlayerObject();
	}

    void CreatePlayerObject()
    {
		// Find or default spawn position
		Vector3 spawn = GetSpawn();

		// Create
        GameObject newPlayerObject = PhotonNetwork.Instantiate( "NetworkPlayer", spawn, Quaternion.identity, 0 );
	}

	public static Vector3 GetSpawn()
	{
		var spawn = Spawn;
		var point = GameObject.Find( "SpawnPoint" );
		if ( point != null )
		{
			spawn = point.transform.position;
		}
		return Spawn;
	}
}
