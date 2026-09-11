using UnityEngine;
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 100;
    public float ms = 2f;

    protected Transform player;
    [SerializeField] private int damageSaatTabrakan = 20;

    [Header("Pengaturan State Machine")]
    [SerializeField] private float jarakDeteksi = 6f;
    [SerializeField] private float jarakSerang = 1.2f;
    [SerializeField] private float jedaSerang = 1f;
    public static event System.Action<Enemy> OnZombieMati;

    [Header("State Zombie")]
    private StateZombie state = StateZombie.IDLE;
    private float waktuSerangTerakhir;

    [SerializeField] private float radiusPatrol = 3f;
    private Vector2 titikAwal;
    private Vector2 tujuanPatrol;



    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        titikAwal = transform.position;
        PilihTujuanPatrolBaru();
    }

    void Update()
    {
        PeriksaTransisi();

        switch (state)
        {
            case StateZombie.IDLE:
                PerilakuIdle();
                break;

            case StateZombie.PATROL:
                PerilakuPatrol();
                break;

            case StateZombie.CHASE:
                PerilakuChase();
                break;

            case StateZombie.ATTACK:
                PerilakuAttack();
                break;
        }
    }

    public void Kejar()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            ms * Time.deltaTime
        );
    }

    public virtual void Serang()
    {
        Debug.Log("Enemy Serang");
    }

    public float JarakKePlayer()
    {
        if (player == null) return Mathf.Infinity;

        return Vector2.Distance(transform.position, player.position);
    }

    void PeriksaTransisi()
    {
        float jarak = JarakKePlayer();

        if (jarak <= jarakSerang)
        {
            state = StateZombie.ATTACK;
        }
        else if (jarak <= jarakDeteksi)
        {
            state = StateZombie.CHASE;
        }
        else
        {
            state = StateZombie.PATROL;
        }
    }

    void PerilakuIdle()
    {

    }

    void PerilakuPatrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, tujuanPatrol, ms * 0.5f * Time.deltaTime);
        if (Vector2.Distance(transform.position, tujuanPatrol) < 0.1f)
            PilihTujuanPatrolBaru();
    }

    void PerilakuChase() { Kejar(); }   

    void PerilakuAttack()
    {         
        if (Time.time >= waktuSerangTerakhir + jedaSerang)
        {
            Serang();        
            waktuSerangTerakhir = Time.time;
        }
    }
    void PilihTujuanPatrolBaru()
    {
        Vector2 acak = Random.insideUnitCircle * radiusPatrol;
        tujuanPatrol = titikAwal + acak;
    }

    public void KenaDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Enemy Kena Damage: " + damage + ", sisa HP: " + hp);
        if (hp <= 0)
        {
            Mati();
        }
    }

    protected virtual void Mati()
    {
        Debug.Log("Enemy Mati");
        OnZombieMati?.Invoke(this);
        Destroy(gameObject);
    }

     void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enemy Tabrak: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy Tabrak Player");
            IDamageable playerScript = other.GetComponent<IDamageable>();
            if (playerScript != null)
            {
                Debug.Log("Enemy Serang Player");
                playerScript.KenaDamage(damageSaatTabrakan);
            }
            Debug.Log("Enemy Tabrak Player Selesai");
        }
    }
}
