using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    public Camera Camera;
    public RenderTexture SpyglassTexture;

    private Thread _udpServerThread;
    private UdpClient _udpClient;
    private int _port = 7691;
    private int _width = 160;
    private int _height = 160;

    RenderTexture rt;
    byte[] CamData;
    int CompassRotation;
    bool RecenterRequested = false;

    void Start()
    {
        if ( Camera == null )
        {
            Camera = Camera.main;
        }

        rt = new RenderTexture( _width, _height, 24 );
        SpyglassTexture.width = _width;
        SpyglassTexture.height = _height;

        _udpServerThread = new Thread( new ThreadStart( ListenForMessages ) );
        _udpServerThread.IsBackground = true;
        _udpServerThread.Start();
    }


    private void Update()
	{
        CamData = GetImageBytes();
        CompassRotation = (int) Spyglass.Instance.transform.eulerAngles.y;
        if ( RecenterRequested )
		{
            Spyglass.Instance.Recenter();
            RecenterRequested = false;
        }
    }

	private void ListenForMessages()
    {
        _udpClient = new UdpClient( _port );

        while ( true )
        {
            IPEndPoint remoteEndPoint = new IPEndPoint( IPAddress.Any, _port );
            byte[] data = _udpClient.Receive( ref remoteEndPoint );
            string message = Encoding.ASCII.GetString( data );
            if ( message == "c" )
            {
                byte[] bytes = BitConverter.GetBytes( CompassRotation );
                Array.Reverse( bytes );
                _udpClient.Send( bytes, bytes.Length, remoteEndPoint );
            }
            else if ( message == "r" )
			{
                RecenterRequested = true;
			}
            else
            {
                Spyglass.Instance.ReceiveGyro( message );

                if ( CamData != null )
                {
                    // Send an image back
                    byte[] bytes = BitConverter.GetBytes( CamData.Length );
                    Array.Reverse( bytes );
                    byte[] combinedArray = new byte[bytes.Length + CamData.Length];
                    Array.Copy( bytes, 0, combinedArray, 0, bytes.Length );
                    Array.Copy( CamData, 0, combinedArray, bytes.Length, CamData.Length );
                    _udpClient.Send( combinedArray, combinedArray.Length, remoteEndPoint );
                }
            }
        }
    }

    byte[] GetImageBytes()
    {
        Texture2D screenshot;

        Camera.targetTexture = rt;
        {
            screenshot = new Texture2D( _width, _height, TextureFormat.RGB24, false );
            Camera.Render();
            RenderTexture.active = rt;
            {
                screenshot.ReadPixels( new Rect( 0, 0, _width, _height ), 0, 0 );
            }
            RenderTexture.active = null;
        }
        Camera.targetTexture = SpyglassTexture;

        byte[] bytes = screenshot.EncodeToJPG();
        Destroy( screenshot );
        return bytes;
    }

    private void OnApplicationQuit()
    {
        _udpServerThread.Abort();
        _udpClient.Close();
    }
}
