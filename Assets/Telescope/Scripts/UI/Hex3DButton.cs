using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hex3DButton : MonoBehaviour
{
    void Update()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventDataCurrentPosition, results );
        if ( results.Count == 0 )
        {
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;
            LayerMask layer = 1 << LayerMask.NameToLayer( "HexButton" );
            if ( Physics.Raycast( ray, out hit, 10000000, layer ) )
            {
                if ( Input.GetMouseButtonUp( 0 ) )
                {
                    BoatMover.Instance.transform.position = hit.collider.transform.parent.parent.position;
                    MapData.Instance.DiscoverTile( hit.collider.transform.parent.parent.gameObject );
                }
                if ( Input.GetMouseButtonUp( 1 ) )
                {
                    MapData.Instance.HideTile( hit.collider.transform.parent.parent.gameObject );
                }
            }
        }
    }
}
