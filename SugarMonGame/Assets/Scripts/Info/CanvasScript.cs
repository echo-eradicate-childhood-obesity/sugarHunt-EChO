//This file was create by Mark Botaish on July 9th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
    public static CanvasScript instance;

    public GameObject _warningIndicationPrefab;         // A reference to the warning UI image prefab 

    private Vector2 dim;                                // The dimensions of the screen
    private Vector2 offset = new Vector2(10, 20);       // The offset of the screen so the UI image is not cut off

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        dim = new Vector2(Screen.width, Screen.height) - offset;

    }

    public void CreateWarningFromObject(GameObject obj) { StartCoroutine(CreateWarning(obj)); } 

    /// <summary>
    /// Creates the warning on screen for the object. 
    /// If the object is on screen, then display the warning, else dont. 
    /// When the object is destroyed, destroy the warning.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IEnumerator CreateWarning(GameObject obj)
    { 
        GameObject _warningIndication = Instantiate(_warningIndicationPrefab, transform);
        while (obj != null)
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(obj.transform.position);
            if(pos.x < dim.x && pos.x > offset.x && pos.y < dim.y && pos.y > offset.y)
            {
                _warningIndication.SetActive(false);
            }
            else
            {
                if(!_warningIndication.activeSelf)
                    _warningIndication.SetActive(true);
            }

            pos.x = Mathf.Clamp(pos.x, offset.x, dim.x);
            pos.y = Mathf.Clamp(pos.y, offset.y, dim.y);
            _warningIndication.transform.position = pos;

            yield return null;
        }
        Destroy(_warningIndication);
    }

    public void PlayAgain() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void GoToMenu() { SceneManager.LoadScene("UIScene"); }
}
