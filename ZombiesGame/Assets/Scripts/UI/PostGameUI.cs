using UnityEngine;
using UnityEngine.SceneManagement;

public class PostGameUI : MonoBehaviour
{
    public int winCondition = 3;
    public Player player;
    public bool isZombie = true;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {   
        if (player.humankills == winCondition && !isZombie) 
        {
            gameObject.SetActive(true);
        }
        if (player.zombiekills == winCondition && isZombie)
        {
            gameObject.SetActive(true);
        }
    }

    public void ClickAndLoad() {
        Invoke("MainMenu", 0.5f);
    }
    
    public void LoadGame() {
        SceneManager.LoadScene("MainMenu");
    }
}
