using UnityEngine;
using static EntityStatType;

public class UnitMovement : MonoBehaviour
{
    private Unit owner;
    private CharacterController characterController;
    private Animator animator;
    public void Init(Unit owner)
    {
        this.owner = owner;
        this.animator = owner.animator;
        characterController = GetComponent<CharacterController>();
        
        currentSpeed = owner.stats[runSpeed].Value;
        speedMod = new ModVar(currentSpeed);
    }
    
    [Space(5), Header("Movement")]
    public float currentSpeed;
    public ModVar speedMod;
    public float rotationSpeed;
    public float fallSpeed;
    
    [Tooltip("Сила гравитации")]
    public float gravity = 20.0f;
    
    [Header("Ground Check")]
    [Tooltip("Радиус проверки нахождения на земле")]
    public float groundCheckRadius = 0.3f;
    [Tooltip("Смещение точки проверки нахождения на земле относительно центра объекта")]
    public Vector3 groundCheckOffset = new Vector3(0, -0.8f, 0); // Подстройте под вашего персонажа
    [Tooltip("Маска слоев, которые считаются землей")]
    public LayerMask groundLayer;
    
    [Header("External Forces")]
    [Tooltip("Коэффициент затухания внешней силы (как быстро игрок остановится после толчка)")]
    public float externalForceDamping = 5.0f;
    
    private Vector3 _unitVelocity; // Скорость ЮНИТА, включая гравитацию и прыжок
    private Vector3 _externalForce;  // Внешние силы, действующие на игрока (например, отталкивание)
    private bool _isGrounded;
    //private float _turnSmoothVelocity;

    void Update()
    {
        //HandleMovement();
        HandleGravity();
        ApplyExternalForces();
        ApplyMovement();
    }
    void FixedUpdate(){
        CheckIfGrounded();
    }

    public virtual void MoveTo(Vector3 target)
    {
        Vector3 dirToTarget = (target - transform.position).normalized;
        Vector3 inputDirection = new Vector3(dirToTarget.x, 0, dirToTarget.z);
        _unitVelocity.x = inputDirection.x * currentSpeed;
        _unitVelocity.z = inputDirection.z * currentSpeed;
    }
    
    // Vector3 moveDir = new Vector3(0, 0, 0);
    // public virtual void MoveTo(Vector3 target)
    // {
    //     moveDir = (target - transform.position).normalized;
    //     if (!characterController.isGrounded)
    //     {
    //         moveDir.y = -fallSpeed;
    //     }
        
    //     characterController.Move(moveDir * currentSpeed * Time.deltaTime);
    // }
    public virtual void RotateTo(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Vector3 lookDir = Quaternion.AngleAxis(-90, Vector3.up) * Vector3.Cross(Vector3.up, dir);
        Quaternion lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        
        Debug.DrawRay(transform.position, dir*8, Color.red);
        //Debug.DrawRay(transform.position, lookDir*10, Color.cyan);    
    }
    public virtual void UpdateAnimationParams()
    {
        float speedX = Mathf.Clamp(characterController.velocity.x, -1, 1);
        float speedZ = Mathf.Clamp(characterController.velocity.z, -1, 1);
        animator.SetFloat("Speed X", speedX, 0.1f, Time.deltaTime);
        animator.SetFloat("Speed Z", speedZ, 0.1f, Time.deltaTime);
    }
    
    private void CheckIfGrounded()
    {
        Vector3 spherePosition = transform.position + groundCheckOffset;
        _isGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }
    private void HandleGravity()
    {
        if (_isGrounded)
        {
            // Если на земле, сбрасываем вертикальную скорость (чтобы не накапливалась гравитация)
            // Небольшое отрицательное значение помогает "приклеить" к земле
            _unitVelocity.y = -2f;
        }
        else
        {
            // Применяем гравитацию, если не на земле
            _unitVelocity.y -= gravity * Time.deltaTime;
        }
    }
    private void ApplyExternalForces()
    {
        if (_externalForce.magnitude > 0.01f)
        {
            characterController.Move(_externalForce * Time.deltaTime);
            // Затухание внешней силы
            _externalForce = Vector3.Lerp(_externalForce, Vector3.zero, externalForceDamping * Time.deltaTime);
        }
    }

    private void ApplyMovement()
    {
        // Объединяем движение от ввода, гравитацию/прыжок и внешние силы
        Vector3 finalVelocity = new Vector3(_unitVelocity.x, _unitVelocity.y, _unitVelocity.z);
        characterController.Move(finalVelocity * Time.deltaTime);
    }
    
    public void ResetVelocity(){
        _unitVelocity = Vector3.zero;
    }
    
    /// <summary>
    /// Применяет внешнюю силу к игроку (например, отталкивание).
    /// </summary>
    /// <param name="force">Вектор силы.</param>
    public void AddForce(Vector3 force)
    {
        // Если сила направлена в основном вверх (например, подбрасывание),
        // то добавляем ее к вертикальной скорости, чтобы она сочеталась с гравитацией.
        // Иначе, добавляем как горизонтальную внешнюю силу.
        if (Mathf.Abs(force.y) > Mathf.Abs(force.x) + Mathf.Abs(force.z)) // Проверяем, доминирует ли Y-компонента
        {
            _unitVelocity.y += force.y; // Добавляем к текущей вертикальной скорости (влияет на прыжок/падение)
            _externalForce += new Vector3(force.x, 0, force.z); // Горизонтальную часть добавляем как обычно
        }
        else
        {
            _externalForce += force;
        }
    }
    
    public void UpdateSpeed(){
        currentSpeed = speedMod.Get();
    }
    public void ResetSpeed(){
        currentSpeed = owner.stats[runSpeed].Value;
    }
    // public void ResetVelocity(){
    //     characterController.SimpleMove(Vector3.zero);
    // }
    
#if UNITY_EDITOR
    // Отрисовка сферы для проверки земли в редакторе для удобства настройки
    void OnDrawGizmosSelected()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 spherePosition = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
    }
#endif
}
