using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator _animator;
    private PlayerMovement nearbyPlayer = null;
    private bool isEnemyFaceRight;
    private bool isAtacking = false;

    // Son sald�r�dan bu yana ge�en s�reyi tutmak i�in
    private float lastAttackTime;

    [Header("Enemy Stats")]
    [SerializeField] private float enemySpeed;
    [SerializeField] private float playerRange = 5f;
    [SerializeField] private float attackRange = 1.5f;

    // �ki sald�r� aras� bekleme s�resi
    [SerializeField] private float attackCooldown = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        isEnemyFaceRight = false;
        lastAttackTime = -attackCooldown; // �lk sald�r�n�n hemen ger�ekle�mesi i�in
    }

    private void Update()
    {
        FindPlayer();

        if (nearbyPlayer != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, nearbyPlayer.transform.position);

            if (distanceToPlayer > attackRange && distanceToPlayer <= playerRange)
            {
                // Sald�rm�yorken veya sald�r� bekleme s�resi dolduysa hareket et
                if (!isAtacking)
                {
                    MoveToPlayer();
                }
            }
            else if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
        }
        else
        {
            StopMoving();
        }
    }

    private void FindPlayer()
    {
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            nearbyPlayer = player;
        }
        else
        {
            nearbyPlayer = null;
        }
    }

    private void MoveToPlayer()
    {
        Vector2 direction = (nearbyPlayer.transform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * enemySpeed, rb.linearVelocity.y);
        TurnCheck(direction.x);
        _animator.SetFloat("SpeedEnemy", Mathf.Abs(rb.linearVelocity.x));
    }

    private void Attack()
    {
        // Sald�r� bekleme s�resi dolmad�ysa veya zaten sald�r�yorsa fonksiyondan ��k
        if (Time.time < lastAttackTime + attackCooldown || isAtacking)
        {
            return;
        }

        isAtacking = true;
        rb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("AttackEnemy");
        _animator.SetFloat("SpeedEnemy", 0);
        lastAttackTime = Time.time;
    }

    private void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        _animator.SetFloat("SpeedEnemy", 0);
    }

    private void TurnCheck(float horizontalDirection)
    {
        if (horizontalDirection > 0 && !isEnemyFaceRight)
        {
            Turn(true);
        }
        else if (horizontalDirection < 0 && isEnemyFaceRight)
        {
            Turn(false);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isEnemyFaceRight = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            isEnemyFaceRight = false;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    // Bu metot, animasyon event'i ile �a�r�l�r
    public void EndAttack()
    {
        isAtacking = false;
    }
}