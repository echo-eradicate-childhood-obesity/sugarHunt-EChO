//This file was created by Mark Botaish on June 12th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

public class GenerateIconScript : MonoBehaviour
{

    #region STATIC_VARS

    static List<Sprite> sprites;        //The list of sprite icons in the folder

    //This a list of all of the sugar icon locations
    static List<string> iconLocation = new List<string>(new string[] {  
        "Icons/Cane",
        "Icons/Concentrate",
        "Icons/Dextrin",
        "Icons/Obvious",
        "Icons/OSE",
        "Icons/Strange",
        "Icons/Syrup"
    });

    static List<Sprite> maps = new List<Sprite>(new Sprite[] {
        Resources.Load("UI/Map", typeof(Sprite)) as Sprite,
        Resources.Load("UI/Map", typeof(Sprite)) as Sprite,
        Resources.Load("UI/Map", typeof(Sprite)) as Sprite,
        Resources.Load("UI/Map", typeof(Sprite)) as Sprite,
        Resources.Load("UI/Map", typeof(Sprite)) as Sprite,
        Resources.Load("UI/Map", typeof(Sprite)) as Sprite,
        Resources.Load("UI/Map", typeof(Sprite)) as Sprite
    });

    static Color buttonColor = new Color(1, 0, 1, 1);

    static GameObject canvas = GameObject.Find("Canvas");
    static GameObject sugarPanels = GameObject.Find("SugarPanels");

    static GameObject _buttonPrefab = Resources.Load("UI/Button") as GameObject; //Get the prefab    
    static float _mapScale = 4.0f;
    #endregion

    /// <summary>
    /// This function is used to clear all of the panel. Panels can be found under the SugarPanels GameObject
    /// </summary>
    [MenuItem("Tools/Generate UI/Clear All Maps")]
    static void ClearAll()
    {
        print("Clearing...");

        if(sugarPanels != null)
        {
            DestroyImmediate(sugarPanels);
            print("<color=green>Clearing all panels was successful!</color>");
        }
        else
        {
            print("<color=red>Could not find the GameObject <SugarPanels>!</red>");
        }            
    }

    /*
     * The following functions are used to gather all of the sugars in a particular group, then 
     * generate buttons based on those icons. 
    */
    #region INDIVIDUAL_GENERATION
    [MenuItem("Tools/Generate UI/Indv. Generation/Cane Map")]
    static void GenerateCane(){ Mapping(0); print("<color=green>Done!</color>"); }

    [MenuItem("Tools/Generate UI/Indv. Generation/Concentrate Map")]
    static void GenerateConcentrate(){ Mapping(1); print("<color=green>Done!</color>"); }

    [MenuItem("Tools/Generate UI/Indv. Generation/Dextrin Map")]
    static void GenerateDextrin(){ Mapping(2); print("<color=green>Done!</color>"); }

    [MenuItem("Tools/Generate UI/Indv. Generation/Obvious Map")]
    static void GenerateObvious(){ Mapping(3); print("<color=green>Done!</color>"); }

    [MenuItem("Tools/Generate UI/Indv. Generation/OSE Map")]
    static void GenerateOSE(){ Mapping(4); print("<color=green>Done!</color>"); }

    [MenuItem("Tools/Generate UI/Indv. Generation/Strange Map")]
    static void GenerateStrange(){ Mapping(5); print("<color=green>Done!</color>"); }

    [MenuItem("Tools/Generate UI/Indv. Generation/Syrup Map")]
    static void GenerateSyrup(){ Mapping(6); print("<color=green>Done!</color>"); }
    #endregion

    /*
    * The following functions are used clear individual panels within the SugarPanels panel
   */
    #region CLEAR_INDIVIDUAL_PANELS
    [MenuItem("Tools/Generate UI/Indv. Clear/Cane Panel")]
    static void ClearCane() { ClearPanel(0); }

    [MenuItem("Tools/Generate UI/Indv. Clear/Concentrate Buttons")]
    static void ClearConcentrate() { ClearPanel(1);}

    [MenuItem("Tools/Generate UI/Indv. Clear/Dextrin Buttons")]
    static void ClearDextrin() { ClearPanel(2); }

    [MenuItem("Tools/Generate UI/Indv. Clear/Obvious Buttons")]
    static void ClearObvious() { ClearPanel(3); }

    [MenuItem("Tools/Generate UI/Indv. Clear/OSE Buttons")]
    static void ClearOSE() { ClearPanel(4); }

    [MenuItem("Tools/Generate UI/Indv. Clear/Strange Buttons")]
    static void ClearStrange() { ClearPanel(5); }

    [MenuItem("Tools/Generate UI/Indv. Clear/Syrup Buttons")]
    static void ClearSyrup() { ClearPanel(6);}
    #endregion

    /// <summary>
    /// This function is used to generate all of the maps for all of the sugar types.
    /// </summary>
    [MenuItem("Tools/Generate UI/Gerenate Maps")]
    static void GenerateAllMaps()
    {
        print("Generating maps...");
        for (int i = 0; i < iconLocation.Count; i++) //Loop through the sugar groups
            Mapping(i);

        print("<color=green>Generation complete!</color>");
    }

    /// <summary>
    /// This function is used to generate a single map.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    static void Mapping(int index)
    {
       //Determine if the sugarPanel exists
        if (sugarPanels == null)
        {
            sugarPanels = new GameObject("SugarPanels");
            sugarPanels.transform.SetParent(canvas.transform, false);
        }
        else
        {
            ClearPanel(index); //Clear the panel if it exists
        }
       
        Color changeButtonColor = maps[index].texture.GetPixel(0, 0);                           //The bottom left pixel on the map will be the color to replace the button color on the map

        Vector3 size = new Vector3(maps[index].rect.width, maps[index].rect.height, 1);         // Get the size of the map
        Vector2 offset = (new Vector2((size.x - 1.0f), (size.y - 1.0f)) / 2.0f) * _mapScale;    // Calculate the offset of position/pixel
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;                    // Get the size of the canvas

        string groupName = iconLocation[index].Substring(6);                                    // Get the name of the sugar group and remove the path locations
        string mapName = groupName + "Map";                                                     // Creates the string <SugarName>Map
        string scrollerName = groupName + "Scroller";                                           // Creates the string <SugarName>Scroller

        GameObject scroller = CreateScroller(scrollerName, sugarPanels.transform, new Vector2(canvasSize.x, canvasSize.y)); //Create a scroller
        GameObject panel = CreatePanel(mapName, size * _mapScale, scroller.transform);                                      //Create the panel for the scroller to be a child of

        Texture2D copyMap = new Texture2D(maps[index].texture.width, maps[index].texture.height, maps[index].texture.format, true); //Create a texture to copy the maps to

        scroller.GetComponent<ScrollRect>().content = panel.GetComponent<RectTransform>(); //Set the contents of the scroll
        CreateText(groupName + " - 0/0", scroller.transform); //Create the text that is in the top left of the screen
        copyMap.filterMode = FilterMode.Point;  //Ensures that the new texture is no distorted in anyway. 
        panel.GetComponent<Image>().sprite = Sprite.Create(copyMap, maps[index].rect, maps[index].pivot, maps[index].pixelsPerUnit); //Create a sprite that holds the newly created texture
        sprites = Resources.LoadAll(iconLocation[index], typeof(Sprite)).Cast<Sprite>().ToList(); //Load all the sprites from the folder

        int spriteIndex = 0;

        //Loop through all of the pixels in the map (Top left to bottom right)
        for (int i = (int)size.y-1; i >= 0; i--)
        {
            for (int j = 0; j < size.x; j++)
            {
                Color colr = maps[index].texture.GetPixel(j, i);
                //If the pixel on the map is equal to the button pixel and the copy texture does not equal changeButtonColor
                //Then gather the pixel group with that color and set it to the changeButtonColor and spawn a button
                if(colr == buttonColor && copyMap.GetPixel(j, i) != changeButtonColor) 
                {                    
                    copyMap.SetPixel(j,i, changeButtonColor);                                
                    
                    //Get the size and position of the button based on the map
                    Vector2 center = SetButtonColor(j, i, maps[index].texture, copyMap, buttonColor ,changeButtonColor) * _mapScale;
                    Vector2 position = new Vector2(j * _mapScale - offset.x + (center.x / 2.0f) - (0.5f * _mapScale), i * _mapScale - offset.y - (center.y / 2.0f) - (0.5f * _mapScale));
                    if(spriteIndex < sprites.Count)   //If there is a sprite to spawn create a button                
                    {
                        GameObject button = CreateButton(sprites[spriteIndex], position, center, panel.transform);
                        button.name = Regex.Replace(sprites[spriteIndex].name, @"(^\w)|(\s\w)", m => m.Value.ToUpper()) + " Button"; // Each words starts with an uppercase
                        button.tag = "LevelButton";
                        button.transform.localScale = button.transform.localScale;
                    }

                    spriteIndex++;                    
                }
                else
                {
                    //If the color does not equal the button color copy the map texture to the created texture
                    if (colr != buttonColor)
                    {
                        copyMap.SetPixel(j, i, maps[index].texture.GetPixel(j, i));
                    }
                }
                
            }           
        }

        //Create an error if not all of the buttons could fit on the map
        if (spriteIndex < sprites.Count)
            print("<color=red>Could not fit all buttons on the " + mapName + " !</color>");

        copyMap.Apply(); //Apply the changes to the create texture
    }

    /// <summary>
    /// This function is used to create the various buttons of each sugar group.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="trans"></param>
    /// <returns></returns>
    static GameObject CreateButton(Sprite sprite, Vector2 position, Vector2 size, Transform trans)
    {
        GameObject button = new GameObject();
        button.AddComponent<Image>().sprite = sprite;
        button.AddComponent<Button>().interactable = false;
        button.GetComponent<RectTransform>().sizeDelta = size * 1.5f;
        button.GetComponent<RectTransform>().anchoredPosition = position;
        button.transform.SetParent(trans, false);

        ColorBlock colorBlock = button.GetComponent<Button>().colors;
        colorBlock.highlightedColor = Color.red;
        colorBlock.disabledColor = Color.black;
        button.GetComponent<Button>().colors = colorBlock;

        return button;
    }

    /// <summary>
    /// This function is used to change different pixels to a certain color. When the mapping reads the 
    /// target color, this function takes that pixel and gets all of the pixels near by of that same color
    /// and changes that color so the change color. This ensures that squares are not counted more than once
    /// and the dimensions of the buttons is gathers correctly.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="map"></param>
    /// <param name="copyMap"></param>
    /// <param name="buttonColor"></param>
    /// <param name="changeButtonColor"></param>
    /// <returns></returns>
    static Vector2 SetButtonColor(int x, int y, Texture2D map, Texture2D copyMap, Color buttonColor, Color changeButtonColor)
    {
        int widthIndex = 0;
        int heightIndex = 0;

        while(map.GetPixel(x, y - heightIndex) == buttonColor)
        {
            widthIndex = 0;
            while(map.GetPixel(x + widthIndex,y - heightIndex) == buttonColor)
            {
                copyMap.SetPixel(x + widthIndex, y - heightIndex, changeButtonColor);
                widthIndex++;
            }
            heightIndex++;
        }

        return new Vector2(widthIndex, heightIndex);
    }

    /// <summary>
    /// This function is used to clear a panel given a specific sugar group index.
    /// <param name="index"></param>
    static void ClearPanel(int index)
    {
        string name = iconLocation[index].Substring(6) + "Scroller";
        GameObject obj = GameObject.Find(name);

        if (obj != null)
        {
            print("Clearing...");
            print(name);
            DestroyImmediate(obj);
            print("<color=green>Clearing of the <" + name + "> panel was successful!</color>");
        }
    }

    /// <summary>
    /// The function is used to clear a certain panel. The panel must be given in the parameters
    /// </summary>
    /// <param name="index"></param>
    static void ClearPanel(Transform panel)
    {
        int size = panel.childCount;

        for (int i = size - 1; i >= 0; i--)
        {
            DestroyImmediate(panel.GetChild(i).gameObject);
        }
    }

    #region CREATE_UI_ELEMENTS

    /// <summary>
    /// This function is used to create the scroller GameObject
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    static GameObject CreateScroller(string name, Transform transform, Vector3 size)
    {
        //create the gameobject
        GameObject scroll = new GameObject(name);
        scroll.AddComponent<CanvasRenderer>();
        scroll.AddComponent<RectTransform>();
        scroll.AddComponent<Image>().color = Color.black;
        scroll.AddComponent<Mask>();

        //Init the size and position. Might need to change in the future 
        scroll.GetComponent<RectTransform>().sizeDelta = size;
        
        //Apply scroller settings
        ScrollRect rect = scroll.AddComponent<ScrollRect>();
        rect.movementType = ScrollRect.MovementType.Clamped;
        rect.scrollSensitivity = 30.0f;

        //Set the parent
        scroll.transform.SetParent(transform, false);
        return scroll;
    }

    /// <summary>
    /// This function is used to create a panel for the sugar buttons to be attached to.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="size"></param>
    /// <param name="transform"></param>
    /// <param name="isGrid"></param>
    /// <returns></returns>
    static GameObject CreatePanel(string name, Vector3 size, Transform transform, bool isGrid = false)
    {
        //Create the panel
        GameObject panel = new GameObject(name);
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<RectTransform>();
        panel.AddComponent<Image>();

        //Set the size
        panel.GetComponent<RectTransform>().sizeDelta = size;

        //Add a grid component if needed
        if (isGrid)
            panel.AddComponent<GridLayoutGroup>();
        
       //Set the parent
        panel.transform.SetParent(transform, false);

        return panel;
    }

    /// <summary>
    /// This function is used to create the text for the header of each of the sugar groups
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    static void CreateText(string name, Transform parent)
    {
        //Create the object
        GameObject text = new GameObject("Text");
        text.AddComponent<Text>().color = Color.red;
        text.transform.SetParent(parent, false);

        //Set the text settings
        Text textSettings = text.GetComponent<Text>();
        textSettings.text = name;
        textSettings.fontSize = 27;
        textSettings.alignment = TextAnchor.MiddleLeft;
        textSettings.horizontalOverflow = HorizontalWrapMode.Overflow;

        //Set the position and size settings
        RectTransform rectSettings = text.GetComponent<RectTransform>();
        RectTransform parentSettings = parent.GetComponent<RectTransform>();
        //rectSettings.position = new Vector3(-parentSettings.sizeDelta.x/.0f, parentSettings.sizeDelta.y, 0);
        rectSettings.sizeDelta = new Vector2(160, 30);
        Vector2 pos = new Vector2(-parentSettings.sizeDelta.x / 2.0f, (parentSettings.sizeDelta.y / 2.0f));
        pos.x += (rectSettings.sizeDelta.x/2.0f);
        pos.y -= (rectSettings.sizeDelta.y/2.0f);

        rectSettings.anchoredPosition = new Vector3(pos.x, pos.y, 0);
      
    }

    #endregion

    private void Start()
    {
        Debug.LogError("This script should not be attached to any object! Current Object =  <" + gameObject.name + ">");
    }
}
