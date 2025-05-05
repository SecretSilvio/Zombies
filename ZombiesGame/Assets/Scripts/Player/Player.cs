using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public ControlScheme currentScheme;
    public ControlManager controlManager;
    private Rigidbody rb;

    public float moveSpeed = 15;
    public float jumpForce = 25;
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private float friction = 10;
    [SerializeField] private float gravity = 3;

    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode attack = KeyCode.E;

    private bool jumpPressed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetControls(ControlScheme newScheme)
    {
        currentScheme = newScheme;
        moveForward = currentScheme.moveForward;
        moveBackward = currentScheme.moveBackward;
        moveRight = currentScheme.moveRight;
        moveLeft = currentScheme.moveLeft;
        attack = currentScheme.attack;
        Debug.Log("New controls: Forward=" + currentScheme.moveForward + ", Backward=" + currentScheme.moveBackward + ", Left=" + newScheme.moveLeft + ", Right=" + newScheme.moveRight + ", Attack=" + newScheme.attack);
    }

    void Update()
    {
        float horzMove = 0f;
        float vertMove = 0f;

        if (Input.GetKey(moveLeft)) horzMove -= 1f;
        if (Input.GetKey(moveRight)) horzMove += 1f;

        if (Input.GetKey(moveForward)) vertMove += 1f;
        if (Input.GetKey(moveBackward)) vertMove -= 1f;

        moveDirection = (transform.forward * vertMove + transform.right * horzMove).normalized;
    }

    void FixedUpdate() {
        rb.AddForce(moveDirection.normalized *moveSpeed/10,ForceMode.Impulse);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x*(100-friction)/100,rb.linearVelocity.y-gravity/10,rb.linearVelocity.z*(100-friction)/100);
    }
}
