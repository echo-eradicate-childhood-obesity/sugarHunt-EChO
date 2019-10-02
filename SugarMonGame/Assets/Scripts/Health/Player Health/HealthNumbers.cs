//This file was created by Zakir Chaudry on June 14th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthNumbers : MonoBehaviour
{
    /// <summary>
    /// The text object the health is displayed in
    /// </summary>
    [Tooltip("The text object that the health is displayed in")]
    public TextMeshProUGUI text;
    /// <summary>
    /// The total amount of health the player starts with
    /// </summary>
    [Tooltip("The total amount of health the player starts with")]
    public int totalHealth;
    /// <summary>
    /// The current amount of health the player has
    /// </summary>
    [Tooltip("The current amount of health the player has")]
    public int currentHealth;
    // Start is called before the first frame update
    public virtual void Start()
    {
        text = this.transform.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        totalHealth = this.transform.parent.gameObject.GetComponent<HealthOptions>().startingHealth; //Gets starting health from HealthOptions
        currentHealth = totalHealth;
        UpdateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Updates the current player health, then updates the health numbers based on that
    /// </summary>
    /// <param name="newHealth">The new amount of health the player has</param>
    public void UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        UpdateHealth();
    }
    /// <summary>
    /// Updates the health numbers with current player health
    /// </summary>
    public void UpdateHealth()
    {
        text.text = currentHealth.ToString() + "/" + totalHealth.ToString();
    }
}
