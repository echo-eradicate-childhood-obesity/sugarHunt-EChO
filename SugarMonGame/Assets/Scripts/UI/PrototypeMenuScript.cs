using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeMenuScript : MonoBehaviour
{
    public void GoToShootingScene(){SceneManager.LoadScene("SampleScene");}
    public void GotToLevelScene(){SceneManager.LoadScene("UIScene");}
    public void GoToMainMenu(){ SceneManager.LoadScene("TempMenuScene");}
    public void GoToPlayerHealthScene(){ SceneManager.LoadScene("PlayerHealthPrototypes");}
    public void GoToEnemyHealthScene(){ SceneManager.LoadScene("MonsterHealthPrototypes");}
    public void GoToUpgradeScene(){ SceneManager.LoadScene("UpgradeScreenPrototypes");}
}
