using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLocationPie : MonoBehaviour
{
	public GameObject PieSegmentPrefab;

	[HideInInspector]
	public int CurrentCharacter = -1;
	[HideInInspector]
	public Vector3 CurrentPos;

	private List<Image> Segments = new List<Image>();
	private List<int> Values = new List<int>();

	private void Start()
	{
		Reset();
	}

	public void Reset()
	{
		CurrentCharacter = -1;
		CurrentPos = Vector3.zero;

		// Hide for now
		transform.position = Vector3.one * 10000;

		// Reset values
		for ( int v = 0; v < Values.Count; v++ )
		{
			Values[v] = 0;
		}
	}

	public void Initialise( List<Color> colours )
	{
		// Create a number of segments equal to the number of characters in the game
		for ( int i = 0; i < colours.Count; i++ )
		{
			GameObject seg = Instantiate( PieSegmentPrefab, transform );
			var img = seg.GetComponent<Image>();
			img.color = colours[i];
			Segments.Add( img );
			Values.Add( 0 );
		}
	}

	public void SetPos( Vector3 pos )
	{
		CurrentPos = pos;

		transform.position = pos + Vector3.up * 15;// Camera.main.WorldToScreenPoint( pos );
	}

	public void SetValue( int character, int value )
	{
		Values[character] = value;

		UpdateValues();
	}

	void UpdateValues()
	{
		// Calculate the number of characters currently in this location
		float total = 0;
		for ( int i = 0; i < Values.Count; i++ )
		{
			total += Values[i];
		}

		// For each character, add their segment and offset the next
		float current = 1;
		for ( int seg = 0; seg < Segments.Count; seg++ )
		{
			float add = Values[seg] / total;
			if ( add != 0 )
			{
				Segments[seg].fillAmount = current;
				current -= add;
			}
			else
			{
				Segments[seg].fillAmount = add;
			}

			foreach ( Transform child in Segments[seg].transform )
			{
				child.gameObject.SetActive( child.name == Timeline.Instance.Characters[seg] );
			}
		}
	}
}
