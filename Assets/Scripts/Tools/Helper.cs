using System;
using System.IO;
using System.Xml.Serialization;
using Unity.Collections;
using UnityEngine;

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
}
