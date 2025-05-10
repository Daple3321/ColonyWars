using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    [SerializeReference] public Weapon currentWeapon;
    [Space(10)] public WeaponWorldItem weaponWorld;

    public AnimationCurve concentrationCurve;
    public float concentraion = 0;
    public bool canAttack;
    public bool isAiming;
    
    void Awake()
    {
        enabled = false;
    }

    private Player player;
    private PlayerInventory playerInventory;
    private PlayerAnimation playerAnimation;
    private Controls controls;
    public void Init(Player player, PlayerAnimation playerAnimation)
    {
        this.player = player;
        this.playerInventory = player.playerInventory;
        this.playerAnimation = playerAnimation;
        controls = GameAssets.controls;
        playerInventory.OnItemSelected += PlayerInventory_OnItemSelected;
        playerInventory.OnItemDropped += PlayerInventory_OnItemDropped;
        PlayerAiming.OnAim += PlayerAiming_OnAim;
        enabled = true;
    }

    private void PlayerInventory_OnItemSelected(object sender, PlayerInventory.OnItemSelectedEventArgs e)
    {
        if (e.selectedItem != null && e.selectedItem is Weapon wp)
        {
            currentWeapon = wp;
            weaponWorld = e.worldItem as WeaponWorldItem;
            SetupWeapon(wp);

            //PlayerAiming.OnAim += PlayerAiming_OnAim;
        }
        else
        {
            ClearWeapon();
            //PlayerAiming.OnAim -= PlayerAiming_OnAim;
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
        
        //Debug.Log($"Selected weapon: {currentWeapon.itemName}.");
    }

    private void ClearWeapon()
    {
        currentWeapon = null; // не считается нулевым почему-то
        concentraion = 0f;
        weaponWorld = null;
        
        playerAnimation.OnWeaponClear();
    }

    private void HandleWeapon()
    {
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
        }

        currentWeapon.UpdateWeapon();
    }

    private void HandleAutomaticAttacks()
    {
        if (currentWeapon.CanAttack())
        {
            if(!isAiming){
                player.playerMovement.RotateToMouse();
            }
            
            currentWeapon.Attack();
            weaponWorld.Attack(concentraion);
            StartCoroutine(PlayerCameraController.CameraShake(1, 0.15f));

            currentWeapon.mouseReleased = false;
        }
    }
    private void HandleSingleAttacks()
    {
        if (currentWeapon.CanAttack())
        {
            if(!isAiming){
                player.playerMovement.RotateToMouse();
            }
            
            currentWeapon.Attack();
            weaponWorld.Attack(concentraion);
            StartCoroutine(PlayerCameraController.CameraShake(1, 0.15f));

            currentWeapon.mouseReleased = false;
        }
    }
    private void HandleChargeAttacks()
    {
        if(!currentWeapon.IsCharged()){
            currentWeapon.Charge();
        }
        
        if (currentWeapon.CanAttack())
        {
            if(!isAiming){
                player.playerMovement.RotateToMouse();
            }
            
            currentWeapon.Attack();
            weaponWorld.Attack(concentraion);
            StartCoroutine(PlayerCameraController.CameraShake(2, 0.25f));

            currentWeapon.mouseReleased = false;
        }
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
    private void PlayerAiming_OnAim(bool isAiming)
    {
        if (weaponWorld != null)
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
