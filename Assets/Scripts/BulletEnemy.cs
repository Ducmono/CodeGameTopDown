using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : Bullet
{
    private void Reset()
    {
        this.minDamage = 1;
        this.maxDamage = 2;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int damage = Random.Range(this.minDamage, this.maxDamage);
            collision.GetComponent<Health>().TakeDam(damage);
            collision.GetComponent<Player>().TakeDamageEffect(damage);
            Destroy(gameObject);
        }
    }
}
