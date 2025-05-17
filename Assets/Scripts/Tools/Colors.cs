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
    
    [Space(5), Header("Borders")]
    [ColorUsage(true, true)]
    public Color playerBorder;
    [ColorUsage(true, true)]
    public Color enemyBorder;
    
    [Space(5), Header("Items")]
    public Color common;
    public Color rare;
    public Color veryRare;
    public Color legendary;
    public Color mythical;
}
