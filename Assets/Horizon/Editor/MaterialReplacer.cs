using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MaterialReplacer : MonoBehaviour
{
    void Update()
    {
		// For every mesh renderer in child
		foreach ( MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>() )
        {
            int ind = 0;
            foreach ( var mat in renderer.sharedMaterials )
            {
                // If it has a material which is not of the correct flatkit shader
                //Debug.Log( mat.shader.name );
                if ( mat.shader.name == "Universal Render Pipeline/Lit" )
                {
                    Material material = mat;

                    string matname = mat.name.Replace( " (Instance)", "" ) + " AutoConvert";
                    string name = matname;
                    string dir = "Assets/Telescope/Materials/Auto/";
                    var assets = AssetDatabase.FindAssets( name );
                    if ( assets.Length > 0 )
					{
                        // Load existing material for this model + part
                        GUID id;
                        if ( GUID.TryParse( assets[0], out id ) )
                        {
                            material = AssetDatabase.LoadAssetAtPath<Material>( AssetDatabase.GUIDToAssetPath( id ) );
                        }
					}
					else
                    {
                        // Create new one with the same name in a folder with the model name
                        material = new Material( Shader.Find( "NotSlot/Bending Master" ) );
                        {
                            material.name = matname;

                            material.SetColor( "_Color", mat.color );
                        }
                        AssetDatabase.CreateAsset( material, dir + name + ".mat" );
                    }

                    // Assign it
                    Material[] mats = renderer.sharedMaterials;
                    {
                        mats[ind] = material;
                    }
                    renderer.sharedMaterials = mats;

                    ind++;
                }
            }
        }
    }
}
