using UnityEngine;

public class HealthController : MonoBehaviour
{
    public delegate void OnDeath();
    public delegate void OnDamage(float MaxHp, float currentHp);

    public OnDeath onDeath;
    public OnDamage onDamage;

    [Header("HP options")]
    [SerializeField] int maxHitPoints;
    [SerializeField] int currentHp;
    [SerializeField] bool invincible;

    [Header("Death Animation")]
    [SerializeField] GameObject deathEffect;

    public void SetInvicible(bool invincible) { this.invincible = invincible; }
    public int CurrentHp { get => currentHp; }
    public int MaxHitPoints { get => currentHp; }


    private void Start()
    {
        currentHp = maxHitPoints;
        onDamage?.Invoke(maxHitPoints, currentHp);
    }

    public void DoDamage(int damage)
    {
        if (invincible)
            return;

        currentHp -= damage;
        onDamage?.Invoke(maxHitPoints, currentHp);

        if (currentHp <= 0)
        {
            if (deathEffect != null)
            {
                GameObject clone = Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(clone, clone.GetComponent<ParticleSystem>().main.duration);
            }

            onDeath?.Invoke();
        }
    }

    public void ResetHealth()
    {
        currentHp = maxHitPoints;
        onDamage?.Invoke(maxHitPoints, currentHp);
    }

    public void RestoreHealth(int amount)
    {
        currentHp += amount;
        currentHp = Mathf.Min(currentHp, maxHitPoints);
        onDamage?.Invoke(maxHitPoints, currentHp);
    }
}