using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Helper
{
    public static string Serialize<T>(this T toSerialize)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();
        xml.Serialize(writer, toSerialize);
        return writer.ToString();
    }

    public static T Deserialize<T>(this string toDeserialize)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(toDeserialize);
        return (T)xml.Deserialize(reader);
    }

    public static T RandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));
        int random = UnityEngine.Random.Range(0, values.Length);
        return (T)values.GetValue(random);
    }
    public static T RandomEnumValue<T>(int maxExclusive)
    {
        var values = Enum.GetValues(typeof(T));
        int random = UnityEngine.Random.Range(0, maxExclusive);
        return (T)values.GetValue(random);
    }

    public static Vector3 GetRandPointOnUnitSphereCap(Quaternion targetDirection, float angle)
    {
        float angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        Vector2 PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        Vector3 V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * V;
    }
    public static Vector3 GetRandPointOnUnitSphereCap(Vector3 targetDirection, float angle)
    {
        return GetRandPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
    }

    public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
    {
        var angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        var PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * V;
    }
    public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
    {
        return GetRandPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
    }

    public static Vector3 PointOnConeEdgeRandom(Vector3 center, float radius, float minAngle, float maxAngle)
    {
        Quaternion randAng = Quaternion.Euler(0, 0, UnityEngine.Random.Range(minAngle, maxAngle + 1));
        return center + randAng * Vector3.forward * radius;
    }
    
    public static GameObject FindClosestObject(Vector3 fromPos, Collider[] colliders)
    {
        GameObject closest = null; 
        float distance = Mathf.Infinity;
        Vector3 position = fromPos;
        foreach (Collider go in colliders)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go.gameObject;
                distance = curDistance;
                // if (distance < TargetRange /*&& !go.transform.GetComponent<FriendlyUnit_AI>().isDead*/)
                //     OptimalTarget = go.transform;
            }
        }
        return closest;
    }
    public static GameObject FindClosestObject(Vector3 fromPos, GameObject[] objects)
    {
        GameObject closest = null; 
        float distance = Mathf.Infinity;
        Vector3 position = fromPos;
        foreach (GameObject go in objects)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
                // if (distance < TargetRange /*&& !go.transform.GetComponent<FriendlyUnit_AI>().isDead*/)
                //     OptimalTarget = go.transform;
            }
        }
        return closest;
    }
    
    public static void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public static void SpawnHitEffect(Transform pos, HitType hitType) // OBJECT POOL!!
    {
        GameObject hit = null;
        switch (hitType)
        {
            case HitType.UNIT:
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            
            case HitType.PLAYER:
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            
            case HitType.GROUND:
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            
            case HitType.BUILDING:
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
        }
        
        GameObject.Destroy(hit, 5);
    }
    public static void SpawnHitEffect(Transform pos, int objLayer) // OBJECT POOL!!
    {
        GameObject hit = null;
        switch (objLayer)
        {
            case 11: // enemy unit
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            case 12: // player unit
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            
            case 6: // player
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            
            case 7: // ground
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            
            case 13: // enemy building
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
            case 14: // player building
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos.position, Quaternion.LookRotation(-pos.up, Vector3.right));
                break;
        }
        
        GameObject.Destroy(hit, 5);
    }
    public static void SpawnHitEffect(Vector3 pos, Vector3 hitNormal, int objLayer) // OBJECT POOL!!
    {
        GameObject hit = null;
        switch (objLayer)
        {
            case 11: // enemy unit
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos, Quaternion.LookRotation(hitNormal, Vector3.right));
                break;
            case 12: // player unit
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos, Quaternion.LookRotation(hitNormal, Vector3.right));
                break;
            
            case 6: // player
            hit = GameObject.Instantiate(GameAssets.unitHitParts, pos, Quaternion.LookRotation(hitNormal, Vector3.right));
                break;
            
            case 7: // ground
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos, Quaternion.LookRotation(hitNormal, Vector3.right));
                break;
            
            case 13: // enemy building
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos, Quaternion.LookRotation(hitNormal, Vector3.right));
                break;
            case 14: // player building
            hit = GameObject.Instantiate(GameAssets.groundHitParts, pos, Quaternion.LookRotation(hitNormal, Vector3.right));
                break;
        }
        
        GameObject.Destroy(hit, 5);
    }
}
