using UnityEngine;

[CreateAssetMenu(fileName = "UsableData", menuName = "Scriptable Objects/Items/Usable Data")]
public class UsableData : ItemData
{
    [Space(6), Header("Usable data")]
    public UseType useType;
    public float uses = 1f;
    public bool isInfinite = false;
    public float chargeRate = 1f;
    
    public override Item CreateItemInstance(){
        return new Usable(this);
    }
}

public enum UseType
{
    INSTANT, // без чарджа
    HOLD_INSTANT, // активируется при окончании чарджа
    HOLD_ACTIVE, // активен во время чарджа
    
}