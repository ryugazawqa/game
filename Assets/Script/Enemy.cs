using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EntityVfx enemyVfx; 

    private Rigidbody2D rb;
    private Animator _animator;
    private PlayerMovement nearbyPlayer = null;
    private bool isEnemyFaceRight;
    private bool isAtacking = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

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

        enemyVfx = GetComponent<EntityVfx>();

        lastSpecialAttackTime = -specialAttackCooldown;
        lastSpecialAttackTime2 = -specialAttackCooldown2;
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

    public void TakeHitAndPlayVfx()
    {
        if (enemyVfx != null)
        {
            enemyVfx.PlayOnDamageVfx();
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

        if (audioSource != null && !audioSource.isPlaying)
        {
           
            audioSource.Play();
        }

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
            return;
        }

        if(Time.time >= lastSpecialAttackTime2 + specialAttackCooldown2 && Time.time >= lastSpecialAttackTime + minTimeAfterAttack1)
        {
            lastSpecialAttackTime2 = Time.time;
            SpecialAttack2();
            return;
        }
      
        lastAttackTime = Time.time;
        
    }



    [Header("Special Attack Stats")]
    [SerializeField] private int specialAttackCount = 3; 
    [SerializeField] private float specialTeleportDistance = 2f;
    [SerializeField] private float specialAttackCooldown = 10f;
    [SerializeField] private float specialAttackCooldown2 = 10f;
    [SerializeField] private float minTimeAfterAttack1 = 4;
    private float lastSpecialAttackTime;
    private float lastSpecialAttackTime2;

    private int currentAttackCount = 0;

    private void StartSpecialAttack()
    {
        currentAttackCount = 0;
        isAtacking = true;
       
        rb.linearVelocity = Vector2.zero;
       
        _animator.SetFloat("SpeedEnemy", 0);

        StartCoroutine(SpecialAttackSequence());


    }

    [Header("Special Attack 2 Stats")] 
    [SerializeField] private float specialAttack2Duration = 1.0f;

    private void SpecialAttack2()
    {
        rb.linearVelocity = Vector2.zero;
        _animator.SetFloat("SpeedEnemy", 0);
        isAtacking = true;

        if (nearbyPlayer != null)
        {
            float directionToPlayer = nearbyPlayer.transform.position.x - transform.position.x;
            TurnCheck(directionToPlayer);

            _animator.SetTrigger("specialAttack2");

            StartCoroutine(WaitForAnimation(specialAttack2Duration));

            lastSpecialAttackTime2 = Time.time;
        }

    }

    private IEnumerator WaitForAnimation(float duration)
    {
        
        yield return new WaitForSeconds(duration);

      
        EndAttack();
    }

    private IEnumerator SpecialAttackSequence()
    {
        

        while (currentAttackCount < specialAttackCount)
        {
            
            float direction = isEnemyFaceRight ? 1 : -1;

           
            Vector2 targetPosition = (Vector2)transform.position + new Vector2(direction * specialTeleportDistance,3);


            
            transform.position = targetPosition;

            transform.rotation = Quaternion.Euler(0f, 0f, 45f);

            rb.gravityScale = 0f;

            _animator.SetTrigger("AttackEnemy");

            specialAttackEffect1();

            yield return new WaitForSeconds(0.8f);



            currentAttackCount++;

            rb.gravityScale = 1f;

            

            yield return new WaitForSeconds(1f);
        }
       

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

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
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
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);

        if (nearbyPlayer != null)

        {

            float directionToPlayer = nearbyPlayer.transform.position.x - transform.position.x;

            TurnCheck(directionToPlayer);

        }

       

        isAtacking = false;

    }

}