using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AddMeshColliders : MonoBehaviour
{
    void Update()
    {
		foreach ( var mesh in GetComponentsInChildren<MeshRenderer>() )
		{
			if ( mesh.GetComponent<MeshCollider>() == null )
			{
				mesh.gameObject.AddComponent<MeshCollider>();
			}
		}
    }
}
