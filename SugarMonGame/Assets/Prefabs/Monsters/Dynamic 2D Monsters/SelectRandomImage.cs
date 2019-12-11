using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SelectRandomImage : MonoBehaviour
{
    public List<Sprite> monsterImages; 

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = monsterImages[Random.Range(0,monsterImages.Count)];
    }
}
