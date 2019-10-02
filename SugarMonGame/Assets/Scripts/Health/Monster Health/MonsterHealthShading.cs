//This file was created by Zakir Chaudry on June 14th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The background for the shading based monster health mechanic
/// </summary>
public class MonsterHealthShading : MonoBehaviour
{
    /// <summary>
    /// The percentage health the monster currently has
    /// </summary>
    [Tooltip("The percentage health the monster currently has")]
    public float healthPercent;
    /// <summary>
    /// The monster image that the shading is happening to
    /// </summary>
    [Tooltip("The monster image that the shading is happening to")]
    public Image monster;
    // Start is called before the first frame update
    void Start()
    {
        healthPercent = 1;
        monster.color = new Color(1, 1, 1);
    }

    /// <summary>
    /// Updates the shading of the monster based on current monster health
    /// </summary>
    /// <param name="newHealthPercent">The new percentage of monster health</param>
    public void UpdateHealth(float newHealthPercent)
    {
        healthPercent = newHealthPercent;
        float shadingPercent = healthPercent;
        if (shadingPercent > 0 && shadingPercent < 1)
        {
            shadingPercent = ((90 * shadingPercent) + 10)/100;
        }
        monster.color = new Color(shadingPercent, shadingPercent, shadingPercent); ///Color(1,1,1) = white, Color(0,0,0) = black. Any decimal is somewhere between.

    }
}
