using UnityEngine;

public class ControlScheme : MonoBehaviour
{
    public KeyCode moveForward;
    public KeyCode moveBackward;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode attack;

    public ControlScheme(KeyCode forward, KeyCode backward, KeyCode left, KeyCode right, KeyCode attack)
    {
        moveForward = forward;
        moveBackward = backward;
        moveLeft = left;
        moveRight = right;
        this.attack = attack;
    }

}
