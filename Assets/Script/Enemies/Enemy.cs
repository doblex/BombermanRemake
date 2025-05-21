using UnityEngine;

public class Enemy : PG
{
    public bool isDead = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthController>().DoDamage(1);
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        isDead = true;
    }
}
