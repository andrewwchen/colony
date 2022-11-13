using UnityEngine;

[CreateAssetMenu(fileName = "LightingSet", menuName = "ScriptableObjects/LightingSet")]
public class LightingSet : ScriptableObject
{
    public string id;
    // the material of the skybox
    public Material skyboxMat;
    // the main directional light
    public Light light;
    // the lightmap data
    public Texture2D[] lightmapColor;
    public Texture2D[] lightmapDir;
}
