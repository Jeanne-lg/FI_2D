using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int money = 0;
    public float maxHealth = 100f;
    public float curHealth = 30f;
    public Rigidbody2D rb;
    public int atk = 10;
    public int def = 5;
    public Text moneyText;
    public Slider healthBar;
    public Player_Combat playerCombat;

    public static PlayerStats Instance; // Singleton-Referenz

    void Update()
    {
        if (Input.GetButtonDown("Slash"))
        {
            playerCombat.Attack();
        }
    }
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Falls ein zweites PlayerStats-Objekt existiert
    }

    void Start()
    {
        UpdateMoneyUI();
        UpdateHealthUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false; // Nicht genug Geld
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }

    // ====== HEALTH HANDLING ======
    public void TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(1f, damage - def); // simple def calculation
        curHealth -= actualDamage;
        if (curHealth < 0) curHealth = 0;
        UpdateHealthUI();

        if (curHealth <= 0)
        {
            GameOver();
        }
    }

    public void Heal(float amount)
    {
        curHealth = Mathf.Min(curHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = curHealth;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // TODO: Trigger GameOver Screen / Scene Reload / Disable Player Movement
    }
}