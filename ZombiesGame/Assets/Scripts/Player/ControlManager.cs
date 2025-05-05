using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControlManager : MonoBehaviour
{
    public float changeInterval = 5f;
    public Player player;

    private List<ControlScheme> controlSchemes = new List<ControlScheme>();
    private ControlScheme currentScheme;

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
        int index = Random.Range(0, keyList.Count);
        KeyCode selected = keyList[index];
        keyList.RemoveAt(index);
        return selected;
    }
}

