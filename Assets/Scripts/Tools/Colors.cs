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
}
