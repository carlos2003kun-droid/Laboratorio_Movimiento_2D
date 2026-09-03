using UnityEngine;

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

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Lectura de movimiento horizontal sin inercia flotante
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // SEGURIDAD: Solo permite saltar si toca el suelo Y el personaje no está subiendo en el aire
        if (Input.GetButtonDown("Jump") && isGrounded && rb2d.linearVelocity.y <= 0.01f)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        }

        // Voltea el sprite del personaje de forma inmediata
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1f, 1f);
        }
    }

    void FixedUpdate()
    {
        // Aplicamos el movimiento horizontal seco
        rb2d.linearVelocity = new Vector2(horizontalInput * speed, rb2d.linearVelocity.y);

        // Comprobación circular en las patitas
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    // Dibuja el radio de detección en rojo dentro del editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}

