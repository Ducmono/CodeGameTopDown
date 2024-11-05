using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.TextCore.Text;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public bool roaming = true;
    public float moveSpeed = 2f;
    public float nextWayPointDistance = 2f;
    public float repeatTimeUpdatePath = 0.5f;
    public SpriteRenderer characterSR;
    public Animator animator;
    public int minDamage;
    public int maxDamage;

    //shoot
    public bool isShoot=false;
    public GameObject bullet;
    public float bulletSpeed;
    public float timeBtwFire;
    private float fireCooldown;

    Path path;
    Seeker seeker;
    Rigidbody2D rb;
    Health PlayerHealth;

    Coroutine moveCoroutine;

    // Part 10
    public float freezeDurationTime;
    float freezeDuration;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        freezeDuration = 0;
        target = FindObjectOfType<Player>().transform;

        InvokeRepeating("CalculatePath", 0f, repeatTimeUpdatePath);
    }

    private void FixedUpdate()
    {
        if(isShoot==true) TimeBullet();
    }
    void TimeBullet()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown < 0)
        {
            fireCooldown = timeBtwFire;
            EnemyFireBullet();
        }
    }
    void EnemyFireBullet()
    {
        GameObject bulletTmp = Instantiate(bullet, transform.position, Quaternion.identity);

        Rigidbody2D rb = bulletTmp.GetComponent<Rigidbody2D>();
        Vector3 playerPos = FindObjectOfType<Player>().transform.position;
        Vector3 direction = playerPos - transform.position;
        rb.AddForce(direction.normalized * bulletSpeed, ForceMode2D.Impulse);
    }
    void CalculatePath()
    {
        Vector2 target = FindTarget();
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target, OnPathCompleted);
    }

    void OnPathCompleted(Path p)
    {
        if (!p.error)
        {
            path = p;
            MoveToTarget();
        }
    }

    Vector2 FindTarget()
    {
        Vector3 playerPos=FindObjectOfType<Player>().transform.position;
        if(roaming == true)
        {
            return (Vector2)playerPos + (Random.Range(8f, 13f) * new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized);
        }
        else return playerPos;
    }
    void MoveToTarget()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    public void FreezeEnemy()
    {
        freezeDuration = freezeDurationTime;
    }

    IEnumerator MoveToTargetCoroutine()
    {
        int currentWP = 0;

        while (currentWP < path.vectorPath.Count)
        {
            while (freezeDuration > 0)
            {
                freezeDuration -= Time.deltaTime;
                yield return null;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWP] - rb.position).normalized;
            Vector2 force = direction * moveSpeed * Time.deltaTime;
            transform.position += (Vector3)force;

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWP]);
            if (distance < nextWayPointDistance)
                currentWP++;

            if (force.x != 0)
                if (force.x < 0)
                    characterSR.transform.localScale = new Vector3(-1, 1, 0);
                else
                    characterSR.transform.localScale = new Vector3(1, 1, 0);

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth = collision.GetComponent<Health>();
            InvokeRepeating("DamagePlayer", 0, 1f);
        }
        if (collision.CompareTag("FireRange"))
        {
            FindObjectOfType<WeaponManager>().AddEnemyToFireRange(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CancelInvoke("DamagePlayer");
            PlayerHealth = null;
        }
        if (collision.CompareTag("FireRange"))
        {
            FindObjectOfType<WeaponManager>().RemoveEnemyToFireRange(this.transform);
        }
    }

    void DamagePlayer()
    {
        int damage = Random.Range(minDamage, maxDamage);
        PlayerHealth.TakeDam(damage);
        //
        PlayerHealth.GetComponent<Player>().TakeDamageEffect(damage);
    }
}
