using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
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

    private bool isAttacking = false;
    public float attackCooldown = 1f;
    public float attackRange = 3f;
    public float attackWidth = 1f;
    public float attackHeight = 2f;

    public int humankills = 0;
    public int zombiekills = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horzMove = 0f;
        float vertMove = 0f;

        if (Input.GetKey(moveLeft)) horzMove -= 1f;
        if (Input.GetKey(moveRight)) horzMove += 1f;

        if (Input.GetKey(moveForward)) vertMove += 1f;
        if (Input.GetKey(moveBackward)) vertMove -= 1f;

        if (Input.GetKey(attack)) StartCoroutine(Attacking());

        moveDirection = (transform.forward * vertMove + transform.right * horzMove).normalized;
    }

    void FixedUpdate() {
        rb.AddForce(moveDirection.normalized *moveSpeed/10,ForceMode.Impulse);
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x*(100-friction)/100,
            rb.linearVelocity.y-gravity/10,
            rb.linearVelocity.z*(100-friction)/100
        );
    }

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
}
