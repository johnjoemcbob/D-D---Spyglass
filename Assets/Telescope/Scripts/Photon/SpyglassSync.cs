using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SpyglassSync : MonoBehaviourPun, IPunObservable
{
	public float LerpSpeed = 5;

	private Vector3 realPosition = Vector3.zero;
	private Quaternion realRotation = Quaternion.identity;
	private int realPoint = 0;

	void Update()
	{
		var admin = FindObjectOfType<Admin>();
		if ( admin != null )
		{
			// Sync other player on admin
			Camera.main.transform.position = realPosition;
			Camera.main.transform.rotation = Quaternion.Lerp( Camera.main.transform.rotation, realRotation, LerpSpeed );
			admin.IndexText.text = "Current index: " + realPoint.ToString();
			admin.RotationText.text = "Rotation: " + Camera.main.transform.eulerAngles.ToString();
		}
	}

	public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		var admin = FindObjectOfType<Admin>();
		if ( stream.IsWriting )
		{
			// Only send when parented to a mech
			if ( admin == null )
			{
				// Basic Info
				stream.SendNext( Camera.main.transform.position );
				stream.SendNext( Camera.main.transform.rotation );
				stream.SendNext( FindObjectOfType<BoatMover>().CurrentPoint );
			}
		}
		else
		{
			if ( admin != null )
			{
				// Basic Info
				realPosition = (Vector3) stream.ReceiveNext();
				realRotation = (Quaternion) stream.ReceiveNext();
				realPoint = (int) stream.ReceiveNext();
			}
		}
	}

	#region RPC
	[PunRPC]
	void SendAdvanceBoat()
	{
		FindObjectOfType<BoatMover>().MoveBoatToNext();
	}

	[PunRPC]
	void SendResetBoat()
	{
		FindObjectOfType<BoatMover>().MoveBoatToIndex( 0 );
	}

	[PunRPC]
	void SendResetPitch()
	{
		var t = FindObjectOfType<Spyglass>().transform;
		t.eulerAngles = new Vector3( 0, t.eulerAngles.y, t.eulerAngles.z );
	}

	[PunRPC]
	void SendResetView()
	{
		var t = FindObjectOfType<Spyglass>().transform;
		t.eulerAngles = Vector3.zero;
	}
	#endregion
}
