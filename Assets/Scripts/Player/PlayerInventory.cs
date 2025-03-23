using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();

    public List<ItemData> itemDatas = new List<ItemData>();

    public GameObject itemPrefab;

    void Awake()
    {
        //enabled = false;
    }


    void Start()
    {
        inventory = new List<Item>()
        {
            new MeleeWeapon(itemDatas[0]),
            new RangedWeapon(itemDatas[1]),
            new Item(itemDatas[2]),
            new Item(itemDatas[2]),
            new Item(itemDatas[2]),
        };

        for (int i = 0; i < inventory.Count; i++)
        {
            Debug.Log($"Item: {inventory[i].itemName}");
            if (inventory[i] is MeleeWeapon meleeWeapon)
            {
                Debug.Log($"Attack Charge: {meleeWeapon.attackCharge}");
            }
        }

        GameObject droppedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        WorldItem worldItem = droppedItem.GetComponent<WorldItem>();
        worldItem.Initialize(inventory[0].itemData, inventory[0]);
        
        // for (int i = 0; i < itemDatas.Count; i++)
        // {
        //     Debug.Log($"ID: {itemDatas[i].id}");
        // }
    }
}
