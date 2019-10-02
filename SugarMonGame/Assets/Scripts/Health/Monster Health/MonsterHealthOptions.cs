//This file was created by Zakir Chaudry on June 14th, 2019 (The Raptors won)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides structural background for monster health related mechanics.
/// </summary>
public class MonsterHealthOptions : HealthOptions
{
    // Start is called before the first frame update
    void Start()
    {
        health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
    }

    public override void UpdateHealth()
    {
        float healthPercent = (float)health / startingHealth;
        this.transform.GetChild(0).GetComponent<MonsterHealthShading>().UpdateHealth(healthPercent);
        this.transform.GetChild(1).GetComponent<MonsterHealthBar>().UpdateHealth(healthPercent);
        this.transform.GetChild(2).GetComponent<MonsterHealthNumbers>().UpdateHealth(health);

    }
}
