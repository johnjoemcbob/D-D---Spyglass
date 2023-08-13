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
	private float realZoom = 1;

	void Update()
	{
		if ( !photonView.IsMine )
		{
			var admin = FindObjectOfType<Admin>();
			if ( admin != null )
			{
				// Sync other player on admin
				Camera cam = Compass.Instance.Camera;
				cam.transform.parent.rotation = Quaternion.Lerp( cam.transform.parent.rotation, realRotation, LerpSpeed );
				Spyglass.Instance.Zoom = realZoom;
				admin.RotationText.text = "Rotation: " + cam.transform.parent.eulerAngles.ToString();
			}
			else
			{
				BoatMover.Instance.transform.position = realPosition;
			}
		}
	}

	public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		var admin = FindObjectOfType<Admin>();
		if ( stream.IsWriting )
		{
			// Client controls rotation, admin position
			if ( admin == null )
			{
				// Basic Info
				Camera cam = Compass.Instance.Camera;
				stream.SendNext( cam.transform.parent.rotation );
				stream.SendNext( Spyglass.Instance.Zoom );
			}
			else
			{
				// Basic Info
				stream.SendNext( BoatMover.Instance.transform.position );
				stream.SendNext( KalerolsTower.Instance.CurrentFloorHeight );
				stream.SendNext( GyroSpeedSlider.GyroModifier );
			}
		}
		else
		{
			// Client controls rotation, admin position
			if ( admin != null )
			{
				// Basic Info
				realRotation = (Quaternion) stream.ReceiveNext();
				realZoom = (float) stream.ReceiveNext();
			}
			else
			{
				realPosition = (Vector3) stream.ReceiveNext();
				KalerolsTower.Instance.CurrentFloorHeight = (float) stream.ReceiveNext();
				GyroSpeedSlider.GyroModifier = (float) stream.ReceiveNext();
			}
		}
	}

	#region RPC
	[PunRPC]
	void SendAdvanceBoat()
	{
		BoatMover.Instance.MoveBoatToNext();
	}

	[PunRPC]
	void SendResetBoat()
	{
		BoatMover.Instance.MoveBoatToIndex( 0 );
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
