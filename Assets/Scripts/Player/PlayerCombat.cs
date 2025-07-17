using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    [SerializeReference] public Weapon currentWeapon;
    [Space(10)] public WeaponWorldItem weaponWorld;
    
    public ItemData fistsData;
    private Tool fists;
    
    [Space(5), Header("Aiming")]
    public AnimationCurve concentrationCurve;
    public float concentraion = 0;
    public bool isAiming;
    public bool canAttack;
    public bool toolEquiped = false;
    
    [Space(6), Header("Blocking")]
    public float blockTime = 0.8f;
    public float blockResetTime = 1.5f;
    public bool isBlocking;
    public bool canBlock = true;
    
    void Awake()
    {
        enabled = false;
    }

    private Player p;
    private PlayerInventory playerInventory;
    private PlayerAnimation playerAnimation;
    private Controls controls;
    public void Init(Player player, PlayerAnimation playerAnimation)
    {
        this.p = player;
        this.playerInventory = player.playerInventory;
        this.playerAnimation = playerAnimation;
        controls = GameAssets.controls;
        playerInventory.OnItemSelected += PlayerInventory_OnItemSelected;
        playerInventory.OnItemDropped += PlayerInventory_OnItemDropped;
        PlayerAiming.OnAim += PlayerAiming_OnAim;
        
        player.animationEvents.PerformAttack += PerformAttackOnEvent;
        player.animationEvents.OnAttackEnded += OnAttackEnded;
        
        fists = fistsData.CreateItemInstance() as Tool;
        
        EventBus.i.PlayerRespawn += OnPlayerRespawn;
        
        enabled = true;
    }
    
    private void OnPlayerRespawn()
    {
        canBlock = true;
    }

    private void PlayerInventory_OnItemSelected(object sender, PlayerInventory.OnItemSelectedEventArgs e)
    {
        if (e.selectedItem != null && e.selectedItem is Weapon wp)
        {
            ClearWeapon();
            
            currentWeapon = wp;
            weaponWorld = e.worldItem as WeaponWorldItem;
            SetupWeapon(wp);
            
            if(currentWeapon is Tool){
                toolEquiped = true;
            }
            //PlayerAiming.OnAim += PlayerAiming_OnAim;
        }
        // Пока что САМАЯ худшая строчка кода за всю карьеру.
        else if((e.selectedItem == null || e.selectedItem is not Weapon) && weaponWorld != null && e.selectedItem is not Usable) // 
        { 
            ClearWeapon();
            //PlayerAiming.OnAim -= PlayerAiming_OnAim;
        }
        
        if (e.selectedItem == null){ // EMPTY HANDS
            ClearWeapon();
            
            currentWeapon = fists;
            weaponWorld = fists.SpawnItem(p.rightHand.position, 1) as ToolWorldItem;
            weaponWorld.Attach(p.rightHand);
            SetupWeapon(fists);
            
            toolEquiped = true;
        }
    }
    private void PlayerInventory_OnItemDropped(object sender, PlayerInventory.OnItemDroppedEventArgs e)
    {
        if (e.droppedItem == currentWeapon)
        {
            //PlayerAiming.OnAim -= PlayerAiming_OnAim;
            ClearWeapon();
        }
    }

    void Update()
    {
        if (weaponWorld != null)
        {
            HandleWeapon();
            HandleAiming();
        }
    }
    
    private void SetupWeapon(Weapon wp)
    {
        playerAnimation.OnWeaponSetup(wp);
        
        if(wp.needsAmmo){
            p.ui.SwitchAmmoCounter(true);
            p.ui.SetAmmoIcon(wp.ammoType.icon);
            p.ui.UpdateAmmoCount(wp.GetAmmoInfo());
        }
        
        wp.isAttacking = false;
        
        //Debug.Log($"Selected weapon: {currentWeapon.itemName}.");
    }

    private void ClearWeapon()
    {
        currentWeapon = null; // не считается нулевым почему-то
        concentraion = 0f;
        if(weaponWorld != null){
            Destroy(weaponWorld.gameObject);
        }
        weaponWorld = null;
        
        if(toolEquiped == true){
            toolEquiped = false;
        }
        
        p.ui.SwitchAmmoCounter(false);
        playerAnimation.OnWeaponClear();
    }

    private void HandleWeapon()
    {
        //if(controls.Player.Attack.WasPressedThisFrame() && EventSystem.current.IsPointerOverGameObject()) return;
        
        if (controls.Player.Attack.IsPressed() && !EventSystem.current.IsPointerOverGameObject())
        {
            switch (currentWeapon.attackType)
            {
                case AttackType.AUTOMATIC:
                    HandleAutomaticAttacks();
                    break;
                case AttackType.SINGLE:
                    HandleSingleAttacks();
                    break;
                case AttackType.CHARGE:
                    HandleChargeAttacks();
                    break;
            }
        }

        if (controls.Player.Attack.WasReleasedThisFrame())
        {
            currentWeapon.mouseReleased = true;
            currentWeapon.attackCharge = 0f;
            p.ui.SwitchChargeBar(false);
        }

        currentWeapon.UpdateWeapon();
    }

    private void HandleAutomaticAttacks()
    {
        if (currentWeapon.CanAttack())
        {
            if(!isAiming){
                p.movement.RotateToMouse();
            }
            
            currentWeapon.Attack();
            if(!currentWeapon.weaponData.attackTriggeredByAnimation){
                weaponWorld.Attack(concentraion);
                ApplyRecoil(currentWeapon.recoilForce.Value);
            }
            
            if(attackRoutine != null){
                StopCoroutine(attackRoutine);
                p.movement.curSpeed.Remove();
                p.movement.ResetSpeed();
                p.movement.runningAllowed = true;
            }
            attackRoutine = StartCoroutine(AttackRoutine());
            //weaponWorld.Attack(concentraion);
            
            //ApplyRecoil(currentWeapon.recoilForce.Value);
            //StartCoroutine(PlayerCameraController.CameraShake(1, 0.15f));

            currentWeapon.mouseReleased = false;
        }
    }
    private void HandleSingleAttacks()
    {
        if (currentWeapon.CanAttack())
        {
            if(!isAiming){
                p.movement.RotateToMouse();
            }
            
            currentWeapon.Attack();
            if(!currentWeapon.weaponData.attackTriggeredByAnimation){
                weaponWorld.Attack(concentraion);
                ApplyRecoil(currentWeapon.recoilForce.Value);
            }
            
            if(attackRoutine != null){
                StopCoroutine(attackRoutine);
                p.movement.curSpeed.Remove();
                p.movement.ResetSpeed();
                p.movement.runningAllowed = true;
            }
            attackRoutine = StartCoroutine(AttackRoutine());
            
            //ApplyRecoil(currentWeapon.recoilForce.Value);
            //StartCoroutine(PlayerCameraController.CameraShake(1, 0.15f));

            currentWeapon.mouseReleased = false;
        }
    }
    private void HandleChargeAttacks()
    {
        if(!currentWeapon.IsCharged()){
            p.ui.SwitchChargeBar(true);
            currentWeapon.Charge();
            
            p.ui.UpdateChargeBar(currentWeapon.attackCharge);
        }
        
        if (currentWeapon.CanAttack())
        {
            if(!isAiming){
                p.movement.RotateToMouse();
            }
            
            currentWeapon.Attack();
            if(!currentWeapon.weaponData.attackTriggeredByAnimation){
                weaponWorld.Attack(concentraion);
                ApplyRecoil(currentWeapon.recoilForce.Value);
            }
            
            if(attackRoutine != null){
                StopCoroutine(attackRoutine);
                p.movement.curSpeed.Remove();
                //p.movement.ResetSpeed();
                p.movement.UpdateSpeed();
                p.movement.runningAllowed = true;
            }
            attackRoutine = StartCoroutine(AttackRoutine());
            
            //ApplyRecoil(currentWeapon.recoilForce.Value);
            //StartCoroutine(PlayerCameraController.CameraShake(2, 0.25f));

            currentWeapon.mouseReleased = false;
        }
    }
    
    public void PerformAttackOnEvent(){
        weaponWorld.Attack(concentraion);
        ApplyRecoil(currentWeapon.recoilForce.Value);
    }
    public void OnAttackEnded(){
        if(currentWeapon != null){
            currentWeapon.isAttacking = false;
        }
    }
    
    private Coroutine attackRoutine;
    private IEnumerator AttackRoutine()
    {
        p.movement.runningAllowed = false;
        p.movement.curSpeed.Add(-currentWeapon.weaponData.slowingAmount);
        p.movement.currentSpeed = p.movement.curSpeed.Get();
        
        float timeLeft = currentWeapon.attackRate;
        if(currentWeapon is MeleeWeapon melee){
            timeLeft = melee.attackSequence.CurrentAttack().duration;
        }
        while(timeLeft > 0)
        {
            p.movement.RotateToMouse();
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        
        if(currentWeapon != null){
            currentWeapon.isAttacking = false;
        }
        p.movement.curSpeed.Remove();
        //p.movement.ResetSpeed();
        p.movement.UpdateSpeed();
        p.movement.runningAllowed = true;
    }
    private void ApplyRecoil(float recoilForce)
    {
        p.movement.AddForce(-transform.forward.normalized * recoilForce);
    }

    private void HandleAiming()
    {
        if (isAiming) // Можно взять с PlayerAiming.isAiming 
        {
            Vector3 mousePos = Input.mousePosition; // БРАТЬ С ДРУГОГО МЕСТА. КАЖДЫЙ РАЗ ПЕРЕСЧИТЫВАТЬ ТРЕШ.
            Ray cameraRay = Camera.main.ScreenPointToRay(mousePos);
            float planeY = transform.position.y;
            float t = (planeY - cameraRay.origin.y) / cameraRay.direction.y;
            Vector3 worldMousePos = cameraRay.origin + t * cameraRay.direction;
            worldMousePos.y = transform.position.y;
            Debug.DrawLine(weaponWorld.transform.position, worldMousePos, Color.red);
        }
    }
    
    private Coroutine _aimRoutine;
    private Coroutine blockRoutine;
    private Coroutine blockReset;
    private void PlayerAiming_OnAim(bool isAiming)
    {
        if (weaponWorld != null && currentWeapon is RangedWeapon)
        {
            if (isAiming)
            {
                _aimRoutine = StartCoroutine(BeginAim(0.6f));
                this.isAiming = true;
            }
            else
            {
                if (_aimRoutine != null)
                {
                    StopCoroutine(_aimRoutine);
                }
                concentraion = 0;
                EventBus.i.PlayerConcentrationChanged?.Invoke(concentraion);
                this.isAiming = false;
            }
        }
        
        if(weaponWorld != null && currentWeapon is MeleeWeapon)
        {
            if (isAiming && canBlock){
                if (blockReset != null){
                    StopCoroutine(blockReset);
                }
                
                blockRoutine = StartCoroutine(BeginBlock());
            }
            
            if(!isAiming && isBlocking)
            {
                if (blockRoutine != null){
                    StopCoroutine(blockRoutine);
                }
                
                blockReset = StartCoroutine(BlockReset());
                CrosshairManager.i.SetCrosshair(CrosshairManager.CrosshairType.Aim);
                this.isBlocking = false;
            }
        }
    }
    
    public IEnumerator BlockReset()
    {
        canBlock = false;
        
        float timeLeft = blockResetTime;
        while (timeLeft > 0)
        {
            float normalizedProgess = timeLeft / blockResetTime;

            timeLeft -= Time.deltaTime;
            yield return null;
        }
        
        canBlock = true;
    }
    public IEnumerator BeginBlock()
    {
        this.isBlocking = true;
        CrosshairManager.i.SetCrosshair(CrosshairManager.CrosshairType.Block);
        
        float timeLeft = blockTime;
        while (timeLeft > 0)
        {
            float normalizedProgess = timeLeft / blockTime;

            timeLeft -= Time.deltaTime;
            yield return null;
        }
        
        this.isBlocking = false;
        CrosshairManager.i.SetCrosshair(CrosshairManager.CrosshairType.Aim);
        blockReset = StartCoroutine(BlockReset());
    }
    
    public IEnumerator BeginAim(float concentrationTime) // перенести в weapon?
    {
        float progress = 0;
        while (progress < concentrationTime)
        {
            float normalizedProgess = progress / concentrationTime;
            concentraion = concentrationCurve.Evaluate(normalizedProgess);
            EventBus.i.PlayerConcentrationChanged?.Invoke(concentraion);

            progress += Time.deltaTime;
            yield return null;
        }
        concentraion = 1;
    }
}
