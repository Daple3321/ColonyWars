using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    
    public void Initialize(ItemData data)
    {
        itemData = data;
        GetComponent<SpriteRenderer>().sprite = data.icon;
        name = data.itemName;

        // Если это оружие, можем сделать дополнительные настройки
        // if (data is WeaponData weaponData)
        // {
        //     Debug.Log($"Dropped weapon: {weaponData.itemName}, Damage: {weaponData.damage}");
        //     // Дополнительная логика для оружия (например, менять цвет, отображать доп.эффекты и т.д.)
        // }
    }

    public ItemData GetItemData() => itemData;
}
