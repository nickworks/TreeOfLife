using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script gives a game object health and the ability to take damage or to heal.
/// </summary>
public class HealthController : MonoBehaviour {
    /// <summary>
    /// The max health of this object.
    /// </summary>
    public int maxHealth = 3;
    /// <summary>
    /// The current health of this object.
    /// </summary>
    private int health = 3;
    /// <summary>
    /// Whether or not this object is dead.
    /// </summary>
    public bool isDead { get; private set; }
    /// <summary>
    /// How long to wait (seconds) after taking damage until this object can take damage again.
    /// </summary>
    public float invulnerableTime = 1f;
    /// <summary>
    /// This tracks how long this object must wait before being able to take damage again. Measured in seconds.
    /// </summary>
    private float timeUntilVulnerable = 0;
    
    /// <summary>
    /// This runs once per frame, and counts down our invulnerability timer.
    /// </summary>
    void Update()
    {
        if (timeUntilVulnerable > 0) timeUntilVulnerable -= Time.deltaTime;
    }
    /// <summary>
    /// This method fully heals this object and makes it not dead.
    /// </summary>
    public void FullHeal()
    {
        health = maxHealth;
        isDead = false;
    }
    /// <summary>
    /// This method hurts this object.
    /// </summary>
    /// <param name="amt">How much to hurt this object.</param>
    /// <returns>Whether or not this object was successfully hurt.</returns>
    public bool TakeDamage(int amt)
    {
        if(amt < 0)
        {
            throw new System.Exception("Damage amt cannot be negative. If you are trying to heal, use Heal() instead.");
        }
        if (timeUntilVulnerable > 0) return false;
        health -= amt;
        timeUntilVulnerable = invulnerableTime;
        if (health < 0) isDead = true;
        return true;
    }
    /// <summary>
    /// This method heals this object.
    /// </summary>
    /// <param name="amt">How much to heal this object.</param>
    /// <returns>Whether or not the player was healed.</returns>
    /// 
    public bool Heal(int amt)
    {
        if (amt < 0)
        {
            throw new System.Exception("Heal amount cannot be negative. If you are trying to hurt, use TakeDamage() instead.");
        }
        health += amt;
        if (health > maxHealth) health = maxHealth;
        return true;
    }

}
