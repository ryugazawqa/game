using UnityEngine;

public class Mount : MonoBehaviour
{
    public Transform mountPoint; // Karakterin oturacağı yer
    public float mountSpeed = 3f;
    public Animator mountAnimator;

    private Rigidbody2D rb;
    private bool isMounted = false;
    private GameObject player;
    private bool isMountfaceRight = true;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayerMask;
    private bool _isGrounded;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayerMask);
    }

    void Update()
    {
        if (isMounted)
        {
            Move();
        }
    }

    private void Move()
    {
        TurnCheck();

        float horizontal = InputManager.Movement.x;
        rb.linearVelocity = new Vector2(horizontal * mountSpeed, rb.linearVelocity.y);

        // Animasyon (PlayerMovement'teki mantığa uygun)
        if (mountAnimator != null)
        {
            mountAnimator.SetBool("isRunning", Mathf.Abs(horizontal) > 0.1f);
        }
    }

    private void TurnCheck()
    {
        float horizontal = InputManager.Movement.x;

        if (isMountfaceRight && horizontal < 0)
        {
            Turn(false);
        }
        else if (!isMountfaceRight && horizontal > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isMountfaceRight = true;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            isMountfaceRight = false;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void ToggleMount(GameObject playerObj)
    {
        Debug.Log("tETİKLENDİ");
        if (!isMounted)
        {

            // Bineğe bin
            player = playerObj;

            player.transform.rotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            player.transform.SetParent(mountPoint);
            player.transform.localPosition = Vector3.zero;

            // Rigidbody ve Collider'ı devre dışı bırak
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.simulated = false;
            }

            Collider2D playerCol = player.GetComponent<Collider2D>();
            if (playerCol != null)
            {
                playerCol.enabled = false;
            }

            // Karakter animasyonlarını durdur
            Animator playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isRunning", false);
            }

            // Left Leg'i aktif et
            Transform leftLeg = player.transform.Find("Left Leg");
            if (leftLeg != null)
            {
                leftLeg.gameObject.SetActive(true);
                Debug.Log("Left Leg aktif edildi!");
            }

            isMounted = true;
            Debug.Log("Bineğe binildi!");
        }
        else
        {

            //Burayı düzenle aşağı düşmesin 
            // Binekten in
            // ÖNCE pozisyonu mount'un yanına al (collider çakışmasını önle)
            Vector3 dismountPos = new Vector3(
                transform.position.x + 2f,
                transform.position.y + 1f, // Biraz yukarı da koy
                transform.position.z
            );

            player.transform.SetParent(null);
            player.transform.position = dismountPos;

            // Rigidbody ve Collider'ı tekrar aktif et
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.simulated = true;
                playerRb.linearVelocity = Vector2.zero;
            }



            Collider2D playerCol = player.GetComponent<Collider2D>();
            if (playerCol != null)
            {
                playerCol.enabled = true;
            }

            // Left Leg'i pasif et
            Transform leftLeg = player.transform.Find("Left Leg");
            if (leftLeg != null)
            {
                leftLeg.gameObject.SetActive(false);
                Debug.Log("Left Leg pasif edildi!");
            }

            // Binek animasyonunu durdur
            if (mountAnimator != null)
            {
                mountAnimator.SetBool("isRunning", false);
            }

            isMounted = false;
            Debug.Log("Binekten inildi!");
        }
    }
}
