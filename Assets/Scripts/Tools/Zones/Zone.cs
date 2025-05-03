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
    
    [ColorUsage(true, true)]
    public Color glowColor;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Material mat;
    
    
    public ZoneShape shape;
    
    public void Init(Color color, Color glowColor, float radius = 1f, ZoneShape shape = ZoneShape.Cylinder)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        
        this.color = color;
        mat.SetColor("_BaseColor", color);
        //mat.SetColor("_GlowColor", glowColor);
        SetRadius(radius);
    }
    
    public virtual void SetRadius(float newRadius)
    {
        Tween scaleTween = Tween.Scale(transform, new Vector3(newRadius, height, newRadius), 1f, Ease.OutCubic);
        radius = newRadius;
    }
}
