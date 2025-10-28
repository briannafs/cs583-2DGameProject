using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour {
    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("References")]
    public Animator animator;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [Header("Footstep Sounds")]
    public AudioClip[] grassSteps;
    public float stepInterval = 0.4f;

    private Vector2 input;
    private Vector2 velocity;
    private Vector2 lastMoveDir = Vector2.down;
    private float stepTimer;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update() {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        velocity = input.normalized * moveSpeed;

        if (input.sqrMagnitude > 0.0001f)
            lastMoveDir = input.normalized;

        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("MoveY", input.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);
        animator.SetFloat("LastMoveX", lastMoveDir.x);
        animator.SetFloat("LastMoveY", lastMoveDir.y);

        HandleFootsteps();
    }

    void FixedUpdate() => rb.linearVelocity = velocity;

    void HandleFootsteps() {
        if(velocity.sqrMagnitude > 0.1f) {
            stepTimer -= Time.deltaTime;
            if(stepTimer <= 0f) {
                PlayFootstep();
                stepTimer = stepInterval;
            }

        } else {
            stepTimer = 0f;
        }
    }

    void PlayFootstep() {
        if(grassSteps.Length == 0) 
        return;
        int index = Random.Range(0, grassSteps.Length);
        audioSource.PlayOneShot(grassSteps[index], 0.9f);
    }
}
