using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip onClickClip;

    public void ClickAndLoad() {
        audioSource.PlayOneShot(onClickClip);
        Invoke("LoadGame", 0.5f);
    }
    
    public void LoadGame() {
        SceneManager.LoadScene("Game");
    }
}
