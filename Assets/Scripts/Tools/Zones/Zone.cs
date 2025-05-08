using PrimeTween;
using UnityEngine;

public enum ZoneShape : byte
{
    Sphere,
    Cylinder,
    Capsule,
    Box,
    CustomMesh
}

public class Zone : MonoBehaviour
{
    public float radius = 1f;
    public float height = 5f;
    
    public Color color;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Material mat;
    
    
    public ZoneShape shape;
    
    public void Init(Color color, float radius = 1f, ZoneShape shape = ZoneShape.Cylinder, float height = 5f)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        
        this.color = color;
        mat.SetColor("_BaseColor", color);
        //mat.SetColor("_GlowColor", glowColor);
        SetScale(radius, height);
    }
    public void SetNoiseEffect(int state)
    {
        mat.SetInt("_Noise", state);
    }
    
    public virtual void SetScale(float newRadius, float newHeight)
    {
        Tween scaleTween = Tween.Scale(transform, new Vector3(newRadius, newHeight, newRadius), 1f, Ease.OutCubic);
        radius = newRadius;
    }
}
