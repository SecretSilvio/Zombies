using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class PostGameUI : MonoBehaviour
{
    public int winCondition = 3;
    public Player player;
    public GameObject screen; 
    public bool isZombie = true;
    public Transform victoryCamPosition;
    public float camMoveSpeed = 2f;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {   
        Debug.Log(player.zombiekills);
        if (player.humankills == winCondition) 
        {   
            // StartCoroutine(MoveCameraToVictory());
            Debug.Log($"Human Win");
            screen.SetActive(true);
        }
        if (player.zombiekills == winCondition)
        {
            // StartCoroutine(MoveCameraToVictory());
            Debug.Log($"Zombie Win");
            screen.SetActive(true);
        }
        if (player.Health <= 0)
        {
            screen.SetActive(true);
        }
    }

    IEnumerator MoveCameraToVictory()
    {
        Transform cam = Camera.main.transform;
        float t = 0f;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * camMoveSpeed;
            cam.position = Vector3.Lerp(startPos, victoryCamPosition.position, t);
            cam.rotation = Quaternion.Slerp(startRot, victoryCamPosition.rotation, t);
            yield return null;
        }
    }

    public void ClickAndLoad() {
        Invoke("MainMenu", 0.5f);
    }
    
    public void LoadGame() {
        SceneManager.LoadScene("MainMenu");
    }
}
