using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public void DrawTexture(Texture2D texture)
    {

        //set texture while in editor
        textureRender.sharedMaterial.mainTexture = texture;
        //set size of plane 
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);

    }
}
