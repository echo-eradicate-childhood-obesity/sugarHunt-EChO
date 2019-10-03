//This file was created by Zakir Chaudry on June 17th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Provides structural background for gold related mechanics.
/// </summary>
public class PlayerGold : MonoBehaviour
{
    /// <summary>
    /// How much gold the player currently has
    /// </summary>
    [Tooltip("How much gold the player currently has")]
    public int gold = 100;

    /// <summary>
    /// Adds specified amount of gold to current player amount
    /// </summary>
    /// <param name="newGold">How much gold is being added</param>
    
    void AddGold (int newGold)
    {
        gold += newGold;
        UpdateGold();
    }
    
    /// <summary>
    /// Adds 10 gold to current player amount
    /// </summary>
    [ContextMenu("Add 10 Gold")]
    public void AddTenGold ()
    {
        AddGold(10);
    }

    /// <summary>
    /// Add 50 gold to current player amount
    /// </summary>
    [ContextMenu("Add 50 Gold")]
    public void AddFiftyGold ()
    {
        AddGold(50);
    }

    /// <summary>
    /// Adds 100 gold to current player amount
    /// </summary>
    [ContextMenu("Add 100 Gold")]
    public void AddOneHundredGold()
    {
        AddGold(100);
    }

    /// <summary>
    /// Spends specified amount of gold
    /// </summary>
    /// <param name="spentGold">How much gold is being spent</param>
    public void SpendGold(int spentGold)
    {
        if (spentGold > gold)
        {
            throw new System.InvalidOperationException("spentGold must not be greater than current player gold");
        } else if (spentGold < 0)
        {
            throw new System.InvalidOperationException("Cannot spend a negative amount of gold. Try using AddGold() instead");
        } else
        {
            gold -= spentGold;
        }
        UpdateGold();
    }

    /// <summary>
    /// Updates anything that relies on current player gold amount
    /// </summary>
    [ContextMenu("Update Gold")]
    public void UpdateGold()
    {
        if (this.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            this.transform.GetChild(0).GetComponent<LevelUpgradeScreen>().UpdateScreen();

        } else
        {
            this.transform.GetChild(1).GetComponent<NumberUpgradeScreen>().UpdateScreen();

        }

    }

    [ContextMenu("Set Gold to Zero")]
    public void SetGoldToZero() 
    {
        gold = 0;
        UpdateGold();
    }
}
