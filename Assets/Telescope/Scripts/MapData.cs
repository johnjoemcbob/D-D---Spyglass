using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MapData : MonoBehaviour
{
    [Serializable]
    public enum Hex
	{
        Ocean,
        Shallows,
        Grass,
        Forest,
        Mountain,
        DeathWastes,
        Sand
	}

    [Serializable]
    public struct HexColumn
	{
        public int YOffset;
        public Hex[] Rows;
	}

    [Header( "Variables" )]
    public HexColumn[] Hexes;
    public bool Regen = false;
    public Vector2 HexSize = new Vector2( 1, 0.8f );
    public int LabelStartOffset = 4;

    [Header( "Assets" )]
    public GameObject[] HexPrefabs;

#if UNITY_EDITOR
    void Start()
    {
        
    }

    void Update()
    {
        if ( Regen )
        {
            // Move old and queue to delete
            List<GameObject> todelete = new List<GameObject>();
			foreach ( Transform child in transform.GetChild( 0 ) )
			{
                todelete.Add( child.gameObject );
			}
			foreach ( var del in todelete )
			{
                DestroyImmediate( del );
            }

            // Create new
            Vector3 position = Vector3.zero;
            int count = 0;
            char label_letter = 'A';
            int label_number = 0;
			foreach ( var hexcol in Hexes )
			{
                position.z = hexcol.YOffset + ( count % 2 == 0 ? 0 : 0.5f );
                label_number = LabelStartOffset + hexcol.YOffset;

                foreach ( var hex in hexcol.Rows )
				{
                    GameObject obj = PrefabUtility.InstantiatePrefab( HexPrefabs[(int) hex] ) as GameObject;
                    obj.transform.SetParent( transform.GetChild( 0 ) );
                    obj.transform.localPosition = position;
                    obj.transform.localEulerAngles = new Vector3( 0, -30, 0 );
                    obj.transform.localScale = Vector3.one;

                    obj.GetComponentInChildren<TextMeshProUGUI>().text = label_letter + label_number.ToString();

                    label_number++;
                    position.z += HexSize.y;
				}
                position.x -= HexSize.x;

                count++;
                label_letter++;
			}

            Regen = false;
        }
    }
#endif
}
