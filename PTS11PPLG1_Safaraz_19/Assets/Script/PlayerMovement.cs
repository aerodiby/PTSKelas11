using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    public float kecepatan = 5f;
    public int hp = 100;
    private Vector2 arahGerak;
    private Rigidbody2D rb;

    InputAction moveAction;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
    }
    
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (moveAction != null)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            arahGerak = moveValue.normalized;
            rb.linearVelocity = arahGerak * kecepatan;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            }
            Destroy(other.gameObject);
        }

    }
    public void KenaDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Player Kena Damage: " + damage + ", sisa HP: " + hp);
        if (hp <= 0)
        {
            Debug.Log("Player Mati");
        }
    }
}
