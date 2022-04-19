using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public struct CharacterEventKeyframe
{
    public string EventName;
    public DateTime Date;
    public string PrettyDate;
    public bool Teleport;
    public Vector2 Hex;
}

// From https://github.com/tiago-peres/blog/blob/master/csvreader/CSVReader.cs
public class Timeline : MonoBehaviour
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    [Header( "Variables" )]
    public Color[] Colours;
    public float CharacterLineOffset = 5;

    [Header( "References" )]
    public Slider TimelineSlider;
    public Text CurrentDateText;

    [Header( "Assets" )]
    public GameObject LinePrefab;

    // List of all character name instances in loaded
    public List<string> Characters = new List<string>();
    Dictionary<string, List<CharacterEventKeyframe>> CharacterTimelines = new Dictionary<string, List<CharacterEventKeyframe>>();
    Dictionary<string, LineRenderer> TimelineLines = new Dictionary<string, LineRenderer>();
    List<string> UniqueDates = new List<string>();

    void Start()
    {
        LoadAllCharacters();
        SetupSlider();
        OnTimelineSliderValueChanged( 0 );
    }

    void Update()
    {

    }

	#region LoadCSV
	void LoadAllCharacters()
	{
        DirectoryInfo dir = new DirectoryInfo( Application.streamingAssetsPath );
        FileInfo[] info = dir.GetFiles( "*.csv" );
        foreach ( FileInfo f in info )
		{
            string character = f.Name.Replace( ".csv", "" );
            string path = Path.Combine( Application.streamingAssetsPath, character + ".csv" );
            Characters.Add( character );
            CharacterTimelines.Add( character, Parse( File.ReadAllText( path ) ) );
        }
        SortUniqueDates();

        // Add lines after sorting
		foreach ( var character in Characters )
		{
            CreateTimelineLine( character, CharacterTimelines[character] );
        }
    }

    public List<CharacterEventKeyframe> Parse( string csvfile )
    {
        List<CharacterEventKeyframe> events = new List<CharacterEventKeyframe>();
        {
            CharacterEventKeyframe last = new CharacterEventKeyframe();
            var lines = Regex.Split( csvfile, LINE_SPLIT_RE );
            var header = Regex.Split( lines[0], SPLIT_RE );
            for ( var line = 1; line < lines.Length; line++ )
            {
                CharacterEventKeyframe frame = new CharacterEventKeyframe();
                {
                    var split = Regex.Split( lines[line], SPLIT_RE );
                    string date = split[0];
                    if ( date.Contains( "+" ) )
					{
                        // Add to last date instead
                        frame.Date = last.Date.AddDays( double.Parse( date.Replace( "+", "" ) ) );
                    }
					else
					{
                        frame.Date = DateTime.Parse( date );
                    }
                    string prettydate = frame.Date.ToString( "dd/MM/yyyy" );
                    if ( !UniqueDates.Contains( prettydate ) )
					{
                        UniqueDates.Add( prettydate );
					}
                    frame.PrettyDate = prettydate;

                    string hex = split[1];
                    float x = hex[0] - 'A';
                    float y = float.Parse( hex.Substring( 1 ).ToString() );
                    frame.Hex = new Vector2( x, y );

                    frame.EventName = split[2];
                    frame.Teleport = split[3] == "1";
				}
                events.Add( frame );
                last = frame;
            }
        }
        return events;
    }

    void SortUniqueDates()
	{
        //List<string> sorted = new List<string>();
		{
            // Convert to real dates?
            UniqueDates.Sort( delegate ( string a, string b )
            {
                // E.g.
                return ( DateTime.Parse( a ) > DateTime.Parse( b ) ) ? 1 : -1;
            } );
        }
        //UniqueDates = sorted;
    }
	#endregion

	#region Lines
	void CreateTimelineLine( string character, List<CharacterEventKeyframe> events )
    {
        GameObject obj = Instantiate( LinePrefab, transform );
        obj.name = character;

        List<Vector3> positions = new List<Vector3>();
        // For all unique dates,
        Vector3 lastpos = MapData.Instance.GetHexWorldPos( events[0].Hex );
		foreach ( var date in UniqueDates )
        {
            // Count max number of occurances of this date through all timelines
            int occurances = 0;
			foreach ( var timeline in CharacterTimelines )
			{
                int currentoccur = 0;
				foreach ( var frame in timeline.Value )
				{
                    if ( frame.PrettyDate == date )
					{
                        currentoccur++;
					}
				}
                if ( currentoccur > occurances )
				{
                    occurances = currentoccur;
				}
			}

            // Check if there is an entry for this date in the actual data
            // If there are entries then add them all now
            foreach ( var frame in events )
            {
                if ( frame.PrettyDate == date )
                {
                    // Find the hex by the x,y
                    var hex = frame.Hex;
                    var pos = MapData.Instance.GetHexWorldPos( hex );

                    // Offset in hex by character index for running ease of view
                    pos += new Vector3( 1, 0, 1 ) * Characters.IndexOf( character ) * CharacterLineOffset;

                    // Add that point to the line
                    positions.Add( pos );
                    lastpos = pos;

                    occurances--;
                }
            }
            // Any remaining missing occurances, just add a duplicate (need all line renderers to have the same count)
		    for ( int i = 0; i < occurances; i++ )
            {
                // If not, duplicate the previous point
                var pos = lastpos;
                pos += new Vector3( 1, 0, 1 ) * Characters.IndexOf( character ) * CharacterLineOffset;
                positions.Add( pos );
            }
        }
        var line = obj.GetComponentInChildren<LineRenderer>();
        {
            line.positionCount = positions.Count;
            line.SetPositions( positions.ToArray() );
            line.material.color = Colours[Characters.IndexOf( character )];
            line.startColor = Colours[Characters.IndexOf( character )];
            line.endColor = Colours[Characters.IndexOf( character )];
        }
        TimelineLines.Add( character, line );
    }
	#endregion

	#region Slider
    void SetupSlider()
	{
        TimelineSlider.maxValue = UniqueDates.Count - 1;
    }

    public void OnTimelineSliderValueChanged( float value )
	{
        // Get date by UniqueDates[(int) value]
        string prettydate = UniqueDates[(int) value];
        DateTime currentdate = DateTime.Parse( prettydate );

        // Show this date on the current date text display
        CurrentDateText.text = prettydate;

		// For each CSV, find that date's last index occurance
		foreach ( var timeline in CharacterTimelines )
		{
            int index = 0;
			foreach ( var frame in timeline.Value )
            {
                if ( frame.Date < currentdate )
				{
                    index++;
				}
			}

            // For each line renderer, use that point index to generate new width curve
            var line = TimelineLines[timeline.Key];
            float time = (float) index / line.positionCount;
            Keyframe[] keys = new Keyframe[2];
			{
                keys[0] = new Keyframe( 0, 0.5f );
                keys[1] = new Keyframe( time, 1 );
                //keys[2] = new Keyframe( 0, 1 );
            }
            var curve = line.widthCurve;
            curve.keys = keys;
            line.widthCurve = curve;

            // Future points should have alpha transparency
            Color col = Colours[Characters.IndexOf( timeline.Key )];
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey( col, 0.0f ) },
                new GradientAlphaKey[] { new GradientAlphaKey( 0.5f, 0.0f ), new GradientAlphaKey( 1, time ), new GradientAlphaKey( 0.5f, time + 0.01f ), new GradientAlphaKey( 0, 1 ) }
            );
            line.colorGradient = gradient;
        }
    }
    #endregion
}
