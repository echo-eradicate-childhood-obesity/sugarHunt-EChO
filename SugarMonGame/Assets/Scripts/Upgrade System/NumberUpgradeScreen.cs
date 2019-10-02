//This file was created by Zakir Chaudry on June 17th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Background for the number based upgrade screen
/// </summary>
public class NumberUpgradeScreen : MonoBehaviour
{
    /// <summary>
    /// Info for the number upgrades
    /// </summary>
    Dictionary<string, Dictionary<string, float>> costs;

    /// <summary>
    /// How much the upgrade cost is increased by after each upgrade
    /// </summary>
    int addedCost;

    /// <summary>
    /// Dictionary of Items to Item Descriptions
    /// </summary>
    Dictionary<string, string> itemToDescription = new Dictionary<string, string>();

    /// <summary>
    /// Dictionary of Items to File Locations
    /// </summary>
    public Dictionary<string, string> itemToFile = new Dictionary<string, string>();

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
    /// How much gold the player has
    /// </summary>
    int gold;

    // Start is called before the first frame update
    void Start()
    {
        Upgrades upgrades = this.gameObject.GetComponentInParent<Upgrades>();
        itemToDescription = upgrades.itemToDescription;
        itemToFile = upgrades.itemToFile;
        curr = upgrades.curr;
        costs = upgrades.numCosts;
        addedCost = upgrades.addedCost;

        itemImage = this.transform.Find("Item Image").gameObject.GetComponent<Image>();
        itemName = this.transform.Find("Item Name").gameObject.GetComponent<TextMeshProUGUI>();
        itemDescription = this.transform.Find("Item Description").gameObject.GetComponent<TextMeshProUGUI>();
        itemCost = this.transform.Find("Item Cost").gameObject.GetComponent<TextMeshProUGUI>();
        itemStatus = this.transform.Find("Item Status").gameObject.GetComponent<TextMeshProUGUI>();
        upgradeButton = this.transform.Find("Upgrade Button").gameObject.GetComponent<Button>();
        goldNumber = this.transform.Find("Gold").gameObject.GetComponent<TextMeshProUGUI>();

        gold = this.gameObject.GetComponentInParent<PlayerGold>().gold;

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
        Debug.Log("curr is " + curr);
        foreach (KeyValuePair<string, string> entry in itemToFile)
        {
            Debug.Log(entry.Key + entry.Value);
        }
        itemImage.sprite = Resources.Load<Sprite>(itemToFile[curr]);
        
        itemName.text = curr;
        itemDescription.text = itemToDescription[curr];

        itemCost.text = "Cost: " + costs[curr]["cost"].ToString();
        itemStatus.text = "Current: " + costs[curr]["current"].ToString();

        gold = this.gameObject.GetComponentInParent<PlayerGold>().gold;
        goldNumber.text = "Gold: " + gold.ToString();
        if (gold < int.Parse(itemCost.text.Substring(6)))
        {
            upgradeButton.interactable = false;
            upgradeButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Not Enough Gold";
        }
        else
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
        costs[curr]["current"] += costs[curr]["added"];
        costs[curr]["cost"] += addedCost;
        UpdateScreen();
    }
}
