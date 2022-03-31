using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Admin : MonoBehaviourPun
{
	public Text IndexText;
	public Text RotationText;

	#region RPCs
	public void ButtonAdvance()
	{
		FindObjectOfType<SpyglassSync>().photonView.RPC( "SendAdvanceBoat", RpcTarget.Others );
	}

	public void ButtonResetBoat()
	{
		FindObjectOfType<SpyglassSync>().photonView.RPC( "SendResetBoat", RpcTarget.Others );
	}

	public void ButtonResetPitch()
	{
		FindObjectOfType<SpyglassSync>().photonView.RPC( "SendResetPitch", RpcTarget.Others );
	}

	public void ButtonResetView()
	{
		FindObjectOfType<SpyglassSync>().photonView.RPC( "SendResetView", RpcTarget.Others );
	}
	#endregion
}
