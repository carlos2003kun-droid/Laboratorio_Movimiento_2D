using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movimiento Horizontal")]
    public float speed = 7f;
    private float horizontalInput;

    [Header("Mecánica de Salto")]
    public float jumpForce = 10.5f;
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb2d;
    private Animator animator;

    private int coins;
    public TMP_Text textCoins;

    public AudioSource audioSource;

    public AudioClip coinClip;
    public AudioClip barrelClip;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetFloat("VerticalVelocity", rb2d.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);

        if (Input.GetButtonDown("Jump") && isGrounded && rb2d.linearVelocity.y <= 0.01f)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        }

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1f, 1f);
        }
    }

    void FixedUpdate()
    {
        rb2d.linearVelocity = new Vector2(horizontalInput * speed, rb2d.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(coinClip);
            Destroy(collision.gameObject);
            coins++;
            textCoins.text = coins.ToString();
        }
        if (collision.transform.CompareTag("Spikes"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (collision.transform.CompareTag("Barrel"))
        {
            audioSource.PlayOneShot(barrelClip);
            Vector2 KnockbackDir = (rb2d.position - (Vector2)collision.transform.position).normalized;
            rb2d.linearVelocity = Vector2.zero;
            rb2d.AddForce(KnockbackDir * 2f, ForceMode2D.Impulse);

            BoxCollider2D[] colliders = collision.gameObject.GetComponents<BoxCollider2D>();

            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f);
        }
    }
}

