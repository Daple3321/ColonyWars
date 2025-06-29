using UnityEngine;

[CreateAssetMenu(fileName = "Throwable", menuName = "Scriptable Objects/Items/Throwable")]
public class ThrowableData : UsableData
{
    public float throwForce;
    public GameObject prefab;
    
    public override Item CreateItemInstance(){
        return new Throwable(this);
    }
}
