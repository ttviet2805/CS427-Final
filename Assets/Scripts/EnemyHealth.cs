using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour {
    public int maxHealth = 100;
    private int currentHealth;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount) {
        currentHealth -= damageAmount;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    private void Die() {
        gameObject.GetComponent<Animator>().SetBool("Death", true);
    }
}
