using UnityEngine;

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
        if (isMounted) return;

        Move();
        CheckGrounded();
        HandleJump();
        handleAttack();
        CheckForMount(); // Yakındaki binek ara
        HandleMountInput(); // E tuşu kontrolü
        Debug.Log(rb.linearVelocity.ToString());
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
        }
    }

    // Yakındaki binek ara
    private void CheckForMount()
    {
        Mount[] mounts = FindObjectsByType<Mount>(FindObjectsSortMode.None);
        nearbyMount = null;

        foreach (Mount mount in mounts)
        {
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

            nearbyMount.ToggleMount(gameObject);
        }
    }
}