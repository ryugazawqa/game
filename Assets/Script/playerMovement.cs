using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float _movementSpeed;
    private bool _isFacingRight;
    private Animator _animator;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayerMask;
    private bool _isGrounded;

    [Header("Slash Effect")]
    [SerializeField] private GameObject SlashEffect;

    [Header("Hit Effect")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] public float attackRange = 1f;
    [SerializeField] public LayerMask enemyLayers;
    [SerializeField] private GameObject SlideEffect;

    [Header("Dash Effect")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.3f;
    [SerializeField] private GameObject dashEffectPrefab;



    [Header("Mount Settings")]
    [SerializeField] private float mountRange = 2f; // Ne kadar yakında olursa E tuşu çalışır
    private Mount nearbyMount = null; // Yakındaki binek
    private bool isMounted = false; // Binekli mi değil mi


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _isFacingRight = true;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {

        if(isDashing)
        {
            return;
        }

        CheckForMount(); // Yakındaki binek ara
        HandleMountInput(); // E tuşu kontrolü

        if (isMounted) return;

        Move();
        CheckGrounded();
        HandleJump();
        handleAttack();

        if(InputManager.dashPressed && canDash)
        {
            StartCoroutine(Dash());
        }

    }

    private void Move()
    {
        TurnCheck();

        // Koşma animasyonu tetiklemesi
        if (InputManager.Movement.x != 0)
        {
            _animator.SetBool("isRunning", true);
          
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }

        // Yeni sistem: linearVelocity
        rb.linearVelocity = new Vector2(InputManager.Movement.x * _movementSpeed, rb.linearVelocity.y);
    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        
        if (dashEffectPrefab != null)
        {
            GameObject currentDashEffect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);

          
            if (!_isFacingRight)
            {
                currentDashEffect.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            Destroy(currentDashEffect, dashingTime);
        }

        
        float dashDirection = _isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);

        
        yield return new WaitForSeconds(dashingTime);

       
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void TurnCheck()
    {
        if (_isFacingRight && InputManager.Movement.x < 0)
        {
            Turn(false);
        }
        else if (!_isFacingRight && InputManager.Movement.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        // Transform'un dönmesini düzenledim
        if (turnRight)
        {
            _isFacingRight = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            _isFacingRight = false;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

   


    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayerMask);
    }

    private void HandleJump()
    {
        // Zıplama butonuna basılır ve karakter yerdeyse zıplama başlar
        if (InputManager.jumpPressed && _isGrounded)
        {
            Jump();
        }

        // Animator'a doğru bilgiyi gönderiyoruz
        _animator.SetBool("isJumping", !_isGrounded);
    }

    private void Jump()
    {
        // Yeni sistem: linearVelocity
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, _jumpForce);
    }

    private void handleAttack()
    {
        if (InputManager.attackPressed)
        {
            _animator.SetTrigger("isAttack");
            SlashAttack();

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            Debug.Log("Toplam vurulan düşman sayısı: " + hitEnemies.Length);

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("we hit " + enemy.name);
                Debug.Log("SlideEffect null mu? " + (SlideEffect == null));

                if (SlideEffect != null)
                {
                    Vector3 hitPosition = enemy.transform.position + new Vector3(0f, 1f, 0f);
                    GameObject hitEffect = Instantiate(SlideEffect, hitPosition, Quaternion.identity);

                    Debug.Log("Slash oluştu!");
                    Destroy(hitEffect, 1f);
                }
                else
                {
                    Debug.LogError("SlideEffect NULL!");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint ==  null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void SlashAttack()
    {
        if (SlashEffect != null)
        {
            Vector3 effectPosition = transform.position;

           
            float offsetX = _isFacingRight ? 2f : -2f;
            effectPosition.x += offsetX;

           
            GameObject newEffect = Instantiate(SlashEffect, effectPosition, Quaternion.identity);

            ParticleSystem effectPS = newEffect.GetComponent<ParticleSystem>();

            if (effectPS != null)
            {
                var mainModule = effectPS.main;

                if (!_isFacingRight)
                {
                    
                    mainModule.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * 180f);
                }
                else
                {
                   
                    mainModule.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * 0f);
                }
            }

            Destroy(newEffect, 1.0f);
        }
    }

    // Yakındaki binek ara
    private void CheckForMount()
    {
        Mount[] mounts = FindObjectsByType<Mount>(FindObjectsSortMode.None);
        nearbyMount = null;

       
        for (int i = 0; i < mounts.Length; i++)
        {
            Mount mount = mounts[i]; 

            float distance = Vector3.Distance(transform.position, mount.transform.position);
            if (distance <= mountRange)
            {
                nearbyMount = mount;
                break;
            }
        }
    }

    // E tuşu kontrolü
    private void HandleMountInput()
    {

        if (InputManager.interactPressed && nearbyMount != null)
        {
            if (!isMounted)
            {
                // Bineğe binince karakter hareketini durdur
                rb.linearVelocity = Vector2.zero;
                
                isMounted = true;
            }
            else
            {
                isMounted = false;
            }
            Debug.Log("tetiklendi1");
            nearbyMount.ToggleMount(gameObject);
        }
        
    }
}