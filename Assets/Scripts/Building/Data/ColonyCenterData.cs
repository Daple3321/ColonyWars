using UnityEngine;

[CreateAssetMenu(fileName = "Colony Center", menuName = "Scriptable Objects/Buildings/Colony Center")]
public class ColonyCenterData : BuildingData
{
    public bool canOverlapOtherColonies = false;
    public float colonyRadius = 20f;
    
    public override bool CheckBuildConditions(Vector3 projectionPos)
    {
        if(!base.CheckBuildConditions(projectionPos)){
            return false;
        }
        
        
        return true;
    }
}
