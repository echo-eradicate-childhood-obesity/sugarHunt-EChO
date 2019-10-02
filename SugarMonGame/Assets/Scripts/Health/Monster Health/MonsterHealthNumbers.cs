//This file was created by Zakir Chaudry on June 14th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterHealthNumbers : HealthNumbers
{
    // Start is called before the first frame update
    public override void Start()
    {
        text = this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        totalHealth = this.transform.parent.gameObject.GetComponent<MonsterHealthOptions>().startingHealth; //Gets starting health from HealthOptions
        currentHealth = totalHealth;
        UpdateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
