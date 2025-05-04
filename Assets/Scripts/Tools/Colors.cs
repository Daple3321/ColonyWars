using UnityEngine;

[CreateAssetMenu(fileName = "Colors Preset", menuName = "Scriptable Objects/ColorsPreset")]
public class Colors : ScriptableObject
{
    public Gradient friendlyHealthbar;
    public Gradient enemyHealthbar;
    public Gradient playerStaminaBar;
    public Gradient playerHealthbar;
    
    public Gradient playerBulletTrail;
    public Gradient enemyBulletTrail;
    
    public Color availableColor;
    public Color blockedColor;
    
    [Space(5), Header("Zones")]
    [ColorUsage(true, true)]
    public Color playerBuildZone;
    [ColorUsage(true, true)]
    public Color buildingOverlap;
    [ColorUsage(true, true)]
    public Color gatherRadius;
}
