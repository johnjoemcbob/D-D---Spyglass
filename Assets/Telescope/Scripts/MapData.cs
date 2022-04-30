using System;
using System.IO;
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
    public const int YOFF = 4;

    public static MapData Instance;

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
        public GameObject[] Instances;
	}

    [Header( "Variables" )]
    public HexColumn[] Hexes;
    public bool Regen = false;
    public Vector2 HexSize = new Vector2( 1, 0.8f );
    public int LabelStartOffset = 4;

    [Header( "Assets" )]
    public GameObject[] HexPrefabs;

	private void Awake()
	{
        Instance = this;
    }

    void Start()
    {
        if ( Application.isPlaying )
        {
            // Load file
            string path = Path.Combine( Application.streamingAssetsPath, "Data/hexes_discovered.txt" );

            // Each line is a hex code
            string[] lines = File.ReadAllLines( path );

            // Convert hex code to array indices
            foreach ( var hex in lines )
            {
                int col = (int) ( hex[0] - 'A' );
                HexColumn hexcol = Hexes[col];
                int num = int.Parse( hex.Replace( hex[0].ToString(), "" ) );
                int row = num - hexcol.YOffset - YOFF;

                // Get object and disable the fogofwar child
                hexcol.Instances[row].transform.Find( "HexTileAddons" ).Find( "FogOfWar" ).GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public void DiscoverTile( GameObject obj )
    {
        obj.transform.Find( "HexTileAddons" ).Find( "FogOfWar" ).GetComponent<MeshRenderer>().enabled = false;

        string hexstring = "";
        int x = 0;
        char label_letter = 'A';
        foreach ( var col in Hexes )
        {
            int y = YOFF + col.YOffset;
			foreach ( var row in col.Instances )
			{
                if ( row == obj )
                {
                    hexstring = label_letter + y.ToString();
                    break;
				}
                y++;
            }
            if ( hexstring != "" )
			{
                break;
			}
            x++;
            label_letter++;
        }

        // Store out new line
        string path = Path.Combine( Application.streamingAssetsPath, "Data/hexes_discovered.txt" );
        string[] lines = File.ReadAllLines( path );
		{
            var combine = new List<string>( lines );
            combine.Add( hexstring );
            lines = combine.ToArray();
		}
        File.WriteAllLines( path, lines );
    }

#if UNITY_EDITOR
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
			for ( int col = 0; col < Hexes.Length; col++ )
			{
                HexColumn hexcol = Hexes[col];
                position.z = hexcol.YOffset + ( count % 2 == 0 ? 0 : 0.5f );
                label_number = LabelStartOffset + hexcol.YOffset;

                Hexes[col].Instances = new GameObject[hexcol.Rows.Length];
				for ( int row = 0; row < hexcol.Rows.Length; row++ )
				{
                    Hex hex = hexcol.Rows[row];
                    GameObject obj = PrefabUtility.InstantiatePrefab( HexPrefabs[(int) hex] ) as GameObject;
                    {
                        obj.transform.SetParent( transform.GetChild( 0 ) );
                        obj.transform.localPosition = position;
                        obj.transform.localEulerAngles = new Vector3( 0, -30, 0 );
                        obj.transform.localScale = Vector3.one;

                        foreach ( var text in obj.GetComponentsInChildren<TextMeshProUGUI>() )
                        {
                            text.text = label_letter + label_number.ToString();
                        }
                    }
                    Hexes[col].Instances[row] = obj;

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

    public Vector3 GetHexWorldPos( Vector2 hex )
	{
        var hexcol = Hexes[(int) hex.x];
        float off = hex.y - YOFF - hexcol.YOffset; // wtf
        var obj = hexcol.Instances[(int) off];
        return obj.transform.position;
    }
}
