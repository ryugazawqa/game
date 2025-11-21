using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator _animator;
    private PlayerMovement nearbyPlayer = null;
    private bool isEnemyFaceRight;
    private bool isAtacking = false;

    [Header("effect")]
    [SerializeField] private GameObject firstSpeacialEffect;

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
        
        if (Time.time < lastAttackTime + attackCooldown || isAtacking)
        {
            return;
        }

        if (Time.time >= lastSpecialAttackTime + specialAttackCooldown)
        {
            lastSpecialAttackTime = Time.time;
            StartSpecialAttack();
        }
      
      
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
        currentAttackCount = 0;
        isAtacking = true;
       
        rb.linearVelocity = Vector2.zero;
       
        _animator.SetFloat("SpeedEnemy", 0);

        StartCoroutine(SpecialAttackSequence());


    }

    private IEnumerator SpecialAttackSequence()
    {
        

        while (currentAttackCount < specialAttackCount)
        {
            
            float direction = isEnemyFaceRight ? 1 : -1;

           
            Vector2 targetPosition = (Vector2)transform.position + new Vector2(direction * specialTeleportDistance, 4);


            
            transform.position = targetPosition;

            transform.rotation = Quaternion.Euler(0f, 0f, 45f);

            rb.gravityScale = 0f;

            _animator.SetTrigger("AttackEnemy");

            specialAttackEffect1();

            yield return new WaitForSeconds(0.8f);


            Debug.Log($"Saldýrý Tetiklendi: {currentAttackCount + 1}. Toplam hedef: {specialAttackCount}");


            currentAttackCount++;

            rb.gravityScale = 1f;



            yield return new WaitForSeconds(1f);
        }
        Debug.Log("Özel Saldýrý Dizisi Bitti!"); 

        lastSpecialAttackTime = Time.time;
        
        EndAttack();
    }

    private void specialAttackEffect1()
    {
        if(firstSpeacialEffect != null)
        {
            Vector3 effectPosition = transform.position;
            effectPosition.y -= 3f;
            effectPosition.z -= 3f;
            Instantiate(firstSpeacialEffect, effectPosition, Quaternion.identity);
        }
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
       

        if (nearbyPlayer != null)

        {

            float directionToPlayer = nearbyPlayer.transform.position.x - transform.position.x;

            TurnCheck(directionToPlayer);

        }

       

        isAtacking = false;

    }

}