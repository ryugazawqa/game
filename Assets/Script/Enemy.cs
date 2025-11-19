using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator _animator;
    private PlayerMovement nearbyPlayer = null;
    private bool isEnemyFaceRight;
    private bool isAtacking = false;

    // Son saldýrýdan bu yana geçen süreyi tutmak için
    private float lastAttackTime;

    [Header("Enemy Stats")]
    [SerializeField] private float enemySpeed;
    [SerializeField] private float playerRange = 5f;
    [SerializeField] private float attackRange = 1.5f;

    // Ýki saldýrý arasý bekleme süresi
    [SerializeField] private float attackCooldown = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        isEnemyFaceRight = false;
        lastAttackTime = -attackCooldown; // Ýlk saldýrýnýn hemen gerçekleþmesi için

        lastSpecialAttackTime = -specialAttackCooldown;
    }

    private void Update()
    {
        FindPlayer();

        if (nearbyPlayer != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, nearbyPlayer.transform.position);

            if (distanceToPlayer > attackRange && distanceToPlayer <= playerRange)
            {
                // Saldýrmýyorken veya saldýrý bekleme süresi dolduysa hareket et
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
        // Saldýrý bekleme süresi dolmadýysa veya zaten saldýrýyorsa fonksiyondan çýk
        if (Time.time < lastAttackTime + attackCooldown || isAtacking)
        {
            return;
        }

        if (Time.time < lastSpecialAttackTime + specialAttackCooldown)
        {
            isAtacking = true;
            rb.linearVelocity = Vector2.zero;
            _animator.SetTrigger("AttackEnemy");
            _animator.SetFloat("SpeedEnemy", 0);

            lastAttackTime = Time.time;

            return;
        }   

        StartSpecialAttack();

      
        lastAttackTime = Time.time;
    }

    [Header("Special Attack Stats")]
    [SerializeField] private int specialAttackCount = 3; // Kaç kere ýþýnlanýp saldýracak
    [SerializeField] private float specialTeleportDistance = 2f;
    [SerializeField] private float specialAttackCooldown = 10f;
    private float lastSpecialAttackTime;

    private int currentAttackCount = 0;

    private void StartSpecialAttack()
    {
        isAtacking = true;
        rb.linearVelocity = Vector2.zero;
        _animator.SetFloat("SpeedEnemy", 0);
        currentAttackCount = 0;

        StartCoroutine(SpecialAttackSequence());


    }

    private IEnumerator SpecialAttackSequence()
    {
        
        while (currentAttackCount < specialAttackCount)
        {
            
            float direction = isEnemyFaceRight ? 1 : -1;

           
            Vector2 targetPosition = (Vector2)transform.position + new Vector2(direction * specialTeleportDistance, 0);

            
            transform.position = targetPosition;

            
            _animator.SetTrigger("AttackEnemy");

            
            currentAttackCount++;

           
            yield return new WaitForSeconds(1f);
        }

        lastSpecialAttackTime = Time.time;
        
        EndAttack();
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
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            isEnemyFaceRight = false;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

   
    void EndAttack()

    {
        if (currentAttackCount > 0 && currentAttackCount < specialAttackCount)

        {
            return;
        }

        if (nearbyPlayer != null)

        {

            float directionToPlayer = nearbyPlayer.transform.position.x - transform.position.x;

            TurnCheck(directionToPlayer);

        }

        

        isAtacking = false;

    }

}