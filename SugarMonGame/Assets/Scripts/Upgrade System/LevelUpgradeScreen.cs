//This file was created by Zakir Chaudry on June 17th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Background for the level based upgrade screen
/// </summary>
public class LevelUpgradeScreen : MonoBehaviour
{
    /// <summary>
    /// Dictionary of Levels to Costs 
    /// </summary>
    Dictionary<string, int> levelToCost = new Dictionary<string, int>();

    /// <summary>
    /// Dictionary of Items to Item Descriptions
    /// </summary>
    Dictionary<string, string> itemToDescription = new Dictionary<string, string>();

    /// <summary>
    /// Dictionary of Items to File Locations
    /// </summary>
    Dictionary<string, string> itemToFile = new Dictionary<string, string>();

    /// <summary>
    /// Current Selected Item
    /// </summary>
    string curr;

    /// <summary>
    /// The image that displays the current item
    /// </summary>
    Image itemImage;

    /// <summary>
    /// The text that holds the current item name
    /// </summary>
    TextMeshProUGUI itemName;

    /// <summary>
    /// The text that holds the item description
    /// </summary>
    TextMeshProUGUI itemDescription;

    /// <summary>
    /// The text that holds the upgrade cost
    /// </summary>
    TextMeshProUGUI itemCost;

    /// <summary>
    /// The text that holds the item level status
    /// </summary>
    TextMeshProUGUI itemStatus;

    /// <summary>
    /// The text that holds how much gold the player has
    /// </summary>
    TextMeshProUGUI goldNumber;

    /// <summary>
    /// The button that upgrades the current item (if enough gold)
    /// </summary>
    Button upgradeButton;

    /// <summary>
    /// Dictionary of items to their level status
    /// </summary>
    Dictionary<string, string> itemToStatus = new Dictionary<string, string>();

    /// <summary>
    /// The different upgrade levels each item can achieve
    /// </summary>
    string[] levels;

    /// <summary>
    /// Dictionary of upgrade levels to their respective index in levels
    /// </summary>
    Dictionary<string, int> levelToIndex = new Dictionary<string, int>();

    /// <summary>
    /// How much gold the player has
    /// </summary>
    int gold;

    // Start is called before the first frame update
    void Start()
    {
        Upgrades upgrades = this.gameObject.GetComponentInParent<Upgrades>();
        levelToCost = upgrades.levelToCost;
        itemToDescription = upgrades.itemToDescription;
        itemToFile = upgrades.itemToFile;
        curr = upgrades.curr;

        levels = upgrades.levels;
        levelToIndex = upgrades.levelToIndex;

        itemImage = this.transform.Find("Item Image").gameObject.GetComponent<Image>();
        itemName = this.transform.Find("Item Name").gameObject.GetComponent<TextMeshProUGUI>();
        itemDescription = this.transform.Find("Item Description").gameObject.GetComponent<TextMeshProUGUI>();
        itemCost = this.transform.Find("Item Cost").gameObject.GetComponent<TextMeshProUGUI>();
        itemStatus = this.transform.Find("Item Status").gameObject.GetComponent<TextMeshProUGUI>();
        upgradeButton = this.transform.Find("Upgrade Button").gameObject.GetComponent<Button>();
        goldNumber = this.transform.Find("Gold").gameObject.GetComponent<TextMeshProUGUI>();

        gold = this.gameObject.GetComponentInParent<PlayerGold>().gold;

        foreach (string item in upgrades.items) {
            itemToStatus.Add(item, upgrades.levels[0]);
        }

        UpdateScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Updates the upgrade screen
    /// </summary>
    public void UpdateScreen()
    {
        curr = this.gameObject.GetComponentInParent<Upgrades>().curr;

        itemImage.sprite = Resources.Load<Sprite>(itemToFile[curr]);
        itemName.text = curr;
        itemDescription.text = itemToDescription[curr];
        
        itemCost.text = "Cost: " + levelToCost[levels[Mathf.Min(levelToIndex[itemToStatus[curr]] + 1, levels.Length - 1)]].ToString();
        itemStatus.text = "Current: " + itemToStatus[curr];

        gold = this.gameObject.GetComponentInParent<PlayerGold>().gold;
        goldNumber.text = "Gold: " + gold.ToString();
        if (itemToStatus[curr] == levels[levels.Length - 1])
        {
            upgradeButton.interactable = false;
            upgradeButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Max Level";
            itemCost.text = "Cost: N/A";
        }
        else if (gold < int.Parse(itemCost.text.Substring(6)))
        {
            upgradeButton.interactable = false;
            upgradeButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Not Enough Gold";
        } else
        {
            upgradeButton.interactable = true;
            upgradeButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade?";
        }
    }
    /// <summary>
    /// Upgrade the selected item
    /// </summary>
    public void Upgrade()
    {
        this.gameObject.GetComponentInParent<PlayerGold>().SpendGold(int.Parse(itemCost.text.Substring(6)));
        itemToStatus[curr] = levels[Mathf.Min(levelToIndex[itemToStatus[curr]] + 1, levels.Length - 1)];
        Debug.Log("current item status should be " + itemToStatus[curr]);
        UpdateScreen();
    }
}
