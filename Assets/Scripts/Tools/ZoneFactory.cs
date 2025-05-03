using UnityEngine;

public static class ZoneFactory
{
    
    
    public static Zone CreateZone(Vector3 pos, Color color, Color glowColor, ZoneShape shape = ZoneShape.Cylinder, float radius = 1f)
    {
        GameObject go = null;
        
        switch (shape){
            case ZoneShape.Cylinder:
                go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            break;
            case ZoneShape.Box:
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            break;
            case ZoneShape.Sphere:
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            break;
            case ZoneShape.Capsule:
                go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            break;
            case ZoneShape.CustomMesh:
                
            break;
        }
        go.name = "Zone";
        go.GetComponent<MeshRenderer>().material = GameAssets.intersectionMaterial;
        GameObject.Destroy(go.GetComponent<Collider>());
        
        Zone zone = go.AddComponent<Zone>();
        radius *= 2;
        zone.Init(color, glowColor, radius, shape);
        
        //Debug.Log("ZONE CREATED");
        return zone;
    }
    
    public static Zone CreateTriggerZone(Vector3 pos, Color color, Color glowColor, ZoneShape shape = ZoneShape.Cylinder, float radius = 1f)
    {
        GameObject go = null;
        
        switch (shape){
            case ZoneShape.Cylinder:
                go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            break;
            case ZoneShape.Box:
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            break;
            case ZoneShape.Sphere:
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            break;
            case ZoneShape.Capsule:
                go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            break;
            case ZoneShape.CustomMesh:
                
            break;
        }
        
        Zone zone = go.AddComponent<Zone>();
        zone.Init(color, glowColor, radius, shape);
        return zone;
    }
}
