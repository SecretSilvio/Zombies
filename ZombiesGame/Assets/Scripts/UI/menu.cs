using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void ClickAndLoad() {
        Invoke("LoadGame", 0.5f);
    }
    
    public void LoadGame() {
        SceneManager.LoadScene("Game");
    }
}
