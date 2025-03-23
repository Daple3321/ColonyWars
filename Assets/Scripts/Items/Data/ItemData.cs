using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
    public Rarity rarity;
    public bool IsStackable;

    public int id => GetInstanceID();

    public int maxStackSize = 1;


    public Mesh itemMesh;
    public Material[] itemMaterials;
    public GameObject customPrefab;
}
