using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent( typeof( Camera ) )]
public class FogCameraController : MonoBehaviour
{
    public bool AllowFog = false;

    private bool FogOn;

	private void Start()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += OnEndcameraRendering;
    }

	private void OnDestroy()
	{
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= OnEndcameraRendering;
    }

	private void OnPreRender()
    {
        FogOn = RenderSettings.fog;
        RenderSettings.fog = AllowFog;
    }

	private void OnPostRender()
    {
        RenderSettings.fog = FogOn;
    }

    void OnBeginCameraRendering( ScriptableRenderContext context, Camera camera )
    {
        if ( camera == GetComponent<Camera>() )
		{
            RenderSettings.fog = FogOn;
		}
    }

    private void OnEndcameraRendering( ScriptableRenderContext context, Camera camera )
    {
        if ( camera == GetComponent<Camera>() )
        {
            RenderSettings.fog = !FogOn;
        }
    }
}
