using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // private Rigidbody rb;
    private CharacterController controller;

    public float moveSpeed = 15;
    public float jumpForce = 25;
    private Vector3 velocity;
    [SerializeField] private float friction = 10;
    [SerializeField] private float gravity = 9.81f;
    private float groggyStrafe = 0f;

    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode attack = KeyCode.E;

    private bool isAttacking = false;
    public float attackCooldown = 1f;
    public float attackRange = 3f;
    public float attackWidth = 1f;
    public float attackHeight = 2f;

    public int Health;
    public int humankills = 0;
    public int zombiekills = 0;

    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        StartCoroutine(GroggyRoutine());
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        if (moveForward != KeyCode.None && Input.GetKey(moveForward)) move += transform.forward;
        if (moveBackward != KeyCode.None && Input.GetKey(moveBackward)) move -= transform.forward;
        if (moveLeft != KeyCode.None && Input.GetKey(moveLeft)) move -= transform.right;
        if (moveRight != KeyCode.None && Input.GetKey(moveRight)) move += transform.right;

        move += transform.right * groggyStrafe;

        if (Input.GetKey(attack)) StartCoroutine(Attacking());

        // Gravity
        if (!controller.isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f; // Stick to ground slightly
        }

        // Combine vertical velocity (gravity) with horizontal movement
        move.y = velocity.y;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    IEnumerator GroggyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f)); // Random wait
            groggyStrafe = Random.Range(0, 2) == 0 ? -1f : 1f; // Left or right

            yield return new WaitForSeconds(0.2f); // Groggy effect duration
            groggyStrafe = 0f; // Reset
        }
    }

    // void FixedUpdate() {
    //     rb.AddForce(moveDirection.normalized *moveSpeed/10,ForceMode.Impulse);
    //     rb.linearVelocity = new Vector3(
    //         rb.linearVelocity.x*(100-friction)/100,
    //         rb.linearVelocity.y-gravity/10,
    //         rb.linearVelocity.z*(100-friction)/100
    //     );
    // }

    public IEnumerator Attacking()
    {
        isAttacking = true;
        AttackNearbyTargets();
        yield return new WaitForSeconds(attackCooldown);
    }

    void AttackNearbyTargets()
    {
        Vector3 boxCenter = transform.position + transform.forward * attackRange / 2;
        Vector3 boxSize = new Vector3(attackWidth, attackHeight, attackRange);

        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, transform.rotation);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Zombie") || hit.CompareTag("Human"))
            {
                Destroy(hit.gameObject);
                Debug.Log("Hit target: " + hit.name);
                if (hit.CompareTag("Zombie"))
                {
                    // logic for incrementing score
                    zombiekills += 1;
                }
                else
                {
                    // logic for incrementing score
                    humankills += 1;
                }
                break; // Only hit one
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + transform.forward * attackRange / 2;
        Vector3 boxSize = new Vector3(attackWidth, attackHeight, attackRange);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

    public void takeDamage(int damage)
    {
        Health -= damage;
    }
}
