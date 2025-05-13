using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ControlManager : MonoBehaviour
{
    public float changeInterval = 5f;
    public Player player;
    public Transform playerbody;
    public Transform[] rtp;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 5.0f;
    [SerializeField] private bool fadeIn = false;



    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    private List<ControlScheme> controlSchemes = new List<ControlScheme>();
    private ControlScheme currentScheme;

    [Range(0f, 1f)]
    public float unbindChance = 0.2f;

    private KeyCode[] validKeys;

    void Start()
    {
        validKeys = System.Enum.GetValues(typeof(KeyCode))
            .Cast<KeyCode>()
            .Where(k => IsValidKey(k))
            .ToArray();

        StartCoroutine(RandomizeControls());
    }

    
    public IEnumerator RandomizeControls()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval);
            List<KeyCode> availableKeys = new List<KeyCode>(validKeys);

            player.moveForward = GetAndRemoveRandomKey(availableKeys);
            player.moveBackward = GetAndRemoveRandomKey(availableKeys);
            player.moveLeft = GetAndRemoveRandomKey(availableKeys);
            player.moveRight = GetAndRemoveRandomKey(availableKeys);
            player.attack = GetAndRemoveRandomKey(availableKeys);
            
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
            yield return StartCoroutine(TeleportToRandomPoint());
            yield return new WaitForSeconds(.5f);
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
            Debug.Log($"Forward: {player.moveForward}, Backward: {player.moveBackward}, Left: {player.moveLeft}, Right: {player.moveRight}");
            // yield return new WaitForSeconds(changeInterval);
        }
    }

    bool IsValidKey(KeyCode key)
    {
        // Exclude mouse buttons, joystick buttons, None, etc.
        return
            key >= KeyCode.A && key <= KeyCode.Z || // letters
            key == KeyCode.Space ||
            key == KeyCode.LeftShift || key == KeyCode.RightShift ||
            key == KeyCode.LeftControl || key == KeyCode.RightControl ||
            key == KeyCode.UpArrow || key == KeyCode.DownArrow ||
            key == KeyCode.LeftArrow || key == KeyCode.RightArrow;
    }

    private KeyCode GetAndRemoveRandomKey(List<KeyCode> keyList)
    {
        if (Random.value < unbindChance)
            return KeyCode.None;

        int index = Random.Range(0, keyList.Count);
        KeyCode chosen = keyList[index];
        keyList.RemoveAt(index);
        return chosen;
        // int index = Random.Range(0, keyList.Count);
        // KeyCode selected = keyList[index];
        // keyList.RemoveAt(index);
        // return selected;
    }

    // IEnumerator Fade(float startAlpha, float endAlpha)
    // {
    //     Debug.Log("Fading!");
    //     float elapsed = 0f;
    //     Color color = fadeImage.color;

    //     while (elapsed < fadeDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
    //         color.a = alpha;
    //         fadeImage.color = color;
    //         yield return null;
    //     }

    //     color.a = endAlpha;
    //     fadeImage.color = color;
    //     Debug.Log("Fading end!");
    // }


    public IEnumerator TeleportToRandomPoint()
    {
        if (rtp.Length == 0) 
            yield break;

        int index = Random.Range(0, rtp.Length);
        Transform targetPoint = rtp[index];
        Debug.Log(targetPoint.position);
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        transform.position = targetPoint.position;

        yield return null;

        if (controller != null)
            controller.enabled = true;
        yield break;
    }
}

