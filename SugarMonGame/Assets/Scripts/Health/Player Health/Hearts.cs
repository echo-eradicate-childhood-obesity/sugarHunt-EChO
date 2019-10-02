//This file was created by Zakir Chaudry on June 12, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The background for the heart-based health system.
/// </summary>
public class Hearts : MonoBehaviour
{
    /// <summary>
    /// The number of hearts the player starts with.
    /// </summary>
    [Tooltip("How many hearts the player starts with")]
    public int totalNumberOfHearts = 5;
    /// <summary>
    /// The number of hearts the player currently has
    /// </summary>
    [Range(0,5), Tooltip("How many hearts the player currently has")]
    public int numberOfHearts;
    /// <summary>
    /// List containing the heart images
    /// </summary>
    [Tooltip("List of heart objects")]
    public List<Image> hearts;
    /// <summary>
    /// The Heart Prefab to instantiate
    /// </summary>
    [Tooltip("The Heart Prefab to instantiate")]
    public Image heartPrefab;

    // Start is called before the first frame update
    void Start()
    {
        numberOfHearts = totalNumberOfHearts;
        for (int i = 0; i < totalNumberOfHearts; i++)
        {
            Image heart = Instantiate(heartPrefab, transform);
            heart.rectTransform.position = new Vector2(i * heart.rectTransform.rect.width, 100);
            hearts.Add(heart);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Updates the number of hearts the player has based on current Player health
    /// </summary>
    /// <param name="healthPercentage">Percentage of starting health the player currently has</param>
    public void UpdateHealth(float healthPercentage)
    {
        if (healthPercentage >= 1)
        {
            numberOfHearts = totalNumberOfHearts;
        }
        else
        {
            if (healthPercentage <= 0)
            {
                numberOfHearts = 0;
            }
            else
            {
                numberOfHearts = ((int)(healthPercentage * totalNumberOfHearts) + 1);
            }
            //Debug.Log("i should be less than number of hearts: " + numberOfHearts);
        }
        for (int i = 0; i < numberOfHearts; i++)
        {
            //Debug.Log("Trying to get [" + i + "]");
            hearts[i].GetComponent<Image>().color = Color.white;
        }
        for (int i = numberOfHearts; i < totalNumberOfHearts; i++)
        {
            //Debug.Log("Now we're doing black hearts");
            hearts[i].GetComponent<Image>().color = Color.black;
        }
    }
}
