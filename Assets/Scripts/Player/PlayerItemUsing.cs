using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerItemUsing : MonoBehaviour
{
    void Awake(){enabled = false;}
    
    private PlayerInventory playerInventory;
    private Player p;
    private PlayerAnimation playerAnimation;
    private Controls controls;
    public void Init(Player player)
    {
        this.p = player;
        this.playerAnimation = p.playerAnimation;
        this.playerInventory = p.playerInventory;
        controls = GameAssets.controls;
        
        playerInventory.OnItemSelected += PlayerInventory_OnItemSelected;
        playerInventory.OnItemDropped += PlayerInventory_OnItemDropped;
        
        enabled = true;
    }
    
    [SerializeReference] public Usable currentItem;
    [Space(10)] public WorldItem itemWorld;
    
    private void PlayerInventory_OnItemSelected(object sender, PlayerInventory.OnItemSelectedEventArgs e)
    {
        if (e.selectedItem != null && e.selectedItem is Usable u)
        {
            currentItem = u;
            itemWorld = e.worldItem;
            SetupItem(u);
        }
        else if(e.selectedItem == null && itemWorld != null)
        {
            ClearItem();
        }
    }
    private void PlayerInventory_OnItemDropped(object sender, PlayerInventory.OnItemDroppedEventArgs e)
    {
        if (e.droppedItem == currentItem)
        {
            ClearItem();
        }
    }
    
    void Update()
    {
        if (itemWorld != null)
        {
            HandleUsing();
        }
    }
    
    private void SetupItem(Usable u)
    {
        //playerAnimation.OnWeaponSetup(u);
        
        if(!u.usableData.isInfinite){
            p.ui.SwitchAmmoCounter(true);
            //p.ui.ammoCounter.gameObject.SetActive(true);
            p.ui.SetAmmoIcon(u.icon);
            p.ui.UpdateAmmoCount(u.uses);
        }
    }

    private void ClearItem()
    {
        currentItem = null;
        itemWorld = null;
        
        p.ui.SwitchAmmoCounter(false);
        playerAnimation.OnWeaponClear();
    }

    private void HandleUsing()
    {
        if (controls.Player.Attack.IsPressed() && currentItem.CheckUses() && !EventSystem.current.IsPointerOverGameObject())
        {
            switch (currentItem.usableData.useType)
            {
                case UseType.HOLD_ACTIVE:
                    HandleActiveHold();
                    break;
                case UseType.HOLD_INSTANT:
                    HandleHoldInstant();
                    break;
            }
        }
        
        if(controls.Player.Attack.WasPressedThisFrame() && currentItem.CheckUses() && !EventSystem.current.IsPointerOverGameObject())
        {
            switch (currentItem.usableData.useType)
            {
                case UseType.INSTANT:
                    HandleInstantUse();
                    break;
            }
        }

        if (controls.Player.Attack.WasReleasedThisFrame())
        {
            currentItem.mouseReleased = true;
            currentItem.charge = 0f;
            p.ui.SwitchChargeBar(false);
        }

        //currentItem.HandleUsing();
    }

    private void HandleActiveHold()
    {
        if (currentItem.CanUse())
        {
            currentItem.Use(Time.deltaTime);
            //itemWorld.Attack(concentraion);

            currentItem.mouseReleased = false;
            
            int selectedSlot = playerInventory.selectedSlotId;
            if(!currentItem.CheckUses()){
                if(playerInventory.hotbar.DeleteItem(playerInventory.selectedSlotId, 1) && currentItem != null)
                { // если ещё есть в стаке и не удалился
                    currentItem.uses = currentItem.usableData.uses;
                    playerInventory.SelectItem(playerInventory.hotbar, selectedSlot);
                }
                else{
                    currentItem = null;
                    itemWorld = null;
                    p.ui.SwitchChargeBar(false);
                }
            }
        }
    }
    private void HandleInstantUse()
    {
        if (currentItem.CanUse())
        {
            currentItem.Use(1f);
            //weaponWorld.Attack(concentraion);
            //StartCoroutine(AttackRoutine());

            currentItem.mouseReleased = false;
            
            int selectedSlot = playerInventory.selectedSlotId;
            if(!currentItem.CheckUses()){
                if(playerInventory.hotbar.DeleteItem(playerInventory.selectedSlotId, 1) && currentItem != null){ // если ещё есть в стаке и не удалился
                    currentItem.uses = currentItem.usableData.uses;
                    playerInventory.SelectItem(playerInventory.hotbar, selectedSlot);
                }
                else{
                    currentItem = null;
                    itemWorld = null;
                    p.ui.SwitchChargeBar(false);
                }
            }
        }
    }
    private void HandleHoldInstant()
    {
        if(!currentItem.IsCharged()){
            p.ui.SwitchChargeBar(true);
            currentItem.Charge();
            
            p.ui.UpdateChargeBar(currentItem.charge);
        }
        
        if (currentItem.CanUse())
        {
            currentItem.Use(1f);
            //weaponWorld.Attack(concentraion);

            currentItem.mouseReleased = false;
        }
        
        int selectedSlot = playerInventory.selectedSlotId;
        if(!currentItem.CheckUses()){
            if(playerInventory.hotbar.DeleteItem(playerInventory.selectedSlotId, 1) && currentItem != null)
            {
                currentItem.uses = currentItem.usableData.uses;
                playerInventory.SelectItem(playerInventory.hotbar, selectedSlot);
            }
            else{
                currentItem = null;
                itemWorld = null;
                p.ui.SwitchChargeBar(false);
            }
        }
    }
}
