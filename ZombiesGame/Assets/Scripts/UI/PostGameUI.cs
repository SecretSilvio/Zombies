using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class PostGameUI : MonoBehaviour
{
    public void ClickAndLoad() {
        Invoke("LoadGame", 0f);
    }
    
    public void LoadGame() {
        SceneManager.LoadScene("MainMenu");
    }
}
