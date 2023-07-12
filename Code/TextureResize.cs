using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextureResize : MonoBehaviour 
{

    public float scaleFactor = 5.0f;
    Material mat;
    void Update () 
    {
        ScaleTexture();
    }
    private void OnValidate() {
        ScaleTexture();
    }
    void ScaleTexture()
    {
        if (transform.hasChanged && Application.isEditor && !Application.isPlaying) 
        {
            GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2 (transform.localScale.x / scaleFactor , transform.localScale.z / scaleFactor);
            transform.hasChanged = false;
        }
    }
}
