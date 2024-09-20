using UnityEngine;
using UnityEngine.AI;

// Use AI for Enemy 
public class EnemyController : MonoBehaviour {
    // Enemy Properties
    public float idleTime = 2f;
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;
    public float sightDistance = 10f;

    // AI Agents Settings
    // Waypoint for enemy to run
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private float idleTimer = 0f;
    private Transform player;

    // Enemy State
    private enum EnemyState {Idle, Walk, Chase}
    private EnemyState currentState = EnemyState.Idle;

    // Audio Source
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;

    // Background Audio Source
    private AudioSource audioSource;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        SetDestinationToWaypoint();
    }

    private void Update() {
        if(currentState == EnemyState.Idle) {
            idleTimer += Time.deltaTime;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsChasing", false);
            PlaySound(idleSound);

            if (idleTimer >= idleTime)
                NextWaypoint();

            CheckForPlayerDetection();
        }
        else {
            if(currentState == EnemyState.Walk) {
                idleTimer = 0f;
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsChasing", false);
                PlaySound(walkingSound);

                if (agent.remainingDistance <= agent.stoppingDistance)
                    currentState = EnemyState.Idle;

                CheckForPlayerDetection();
            }
            else {
                if(currentState == EnemyState.Chase) {
                    idleTimer = 0f;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                    animator.SetBool("IsChasing", true);
                    PlaySound(chasingSound);

                    if (Vector3.Distance(transform.position, player.position) > sightDistance) {
                        currentState = EnemyState.Walk;
                        agent.speed = walkSpeed;
                    }
                }
            } 
        }
    }

    // Check if player in scope of zombie's detection
    private void CheckForPlayerDetection() {
        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;
        if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, sightDistance)) {
            if (hit.collider.CompareTag("Player")) {
                currentState = EnemyState.Chase;
            }
        }
    }

    private void PlaySound(AudioClip soundClip) {
        if (!audioSource.isPlaying || audioSource.clip != soundClip) {
            audioSource.clip = soundClip;
            audioSource.Play();
        }
    }

    private void NextWaypoint() {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint() {
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = EnemyState.Walk;
        agent.speed = walkSpeed;
        animator.enabled = true;
    }
}
