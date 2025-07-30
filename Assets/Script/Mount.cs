using UnityEngine;

public class Mount : MonoBehaviour
{
    public Transform mountPoint; // Karakterin oturacağı yer
    public float mountSpeed = 3f;
    public Animator mountAnimator;

    private Rigidbody2D rb;
    private bool isMounted = false;
    private GameObject player;
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
            // Binek hareket etsin
            float horizontal = InputManager.Movement.x;
            rb.linearVelocity = new Vector2(InputManager.Movement.x * mountSpeed, rb.linearVelocity.y);

            // Animasyon
            if (mountAnimator != null)
            {
                mountAnimator.SetBool("isRunning", Mathf.Abs(horizontal) > 0.1f);
            }
        }
    }

    public void ToggleMount(GameObject playerObj)
    {
        if (!isMounted)
        {
            // Bineğe bin
            player = playerObj;
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
            // Binekten in
            player.transform.SetParent(null);
            player.transform.position = transform.position + Vector3.right * 2f;


            // Rigidbody ve Collider'ı tekrar aktif et
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.simulated = true;
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
