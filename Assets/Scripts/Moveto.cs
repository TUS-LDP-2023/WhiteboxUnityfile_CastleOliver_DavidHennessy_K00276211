using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moveto : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public Transform player;
    private int pointIndex = 0;
    private NavMeshAgent agent;
    public float detectionRadius = 5f;
    public float chaseDistance = 10f;
    public float resumePatrolDistance = 15f;
    public Color fieldOfViewColor = Color.yellow;
    public float fieldOfViewAngle = 60f;
    public float chaseDuration = 5.0f; // Adjust this duration as needed.
    public float chaseSpeedIncrease = 2.0f; // Adjustable chase speed increase.

    private Vector3 lastKnownPlayerPosition;
    private bool playerDetected = false;
    
    private bool headingToLastKnownPosition = false; // Flag to indicate if heading to last known position.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GotoNextPoint();
    }

    void GotoNextPoint()
    {
        if (points.Count == 0)
            return;

        agent.destination = points[pointIndex].position;
        pointIndex = (pointIndex + 1) % points.Count;
    }

    void Update()
    {
        if (!playerDetected)
        {
            // Reset the speed to the default value when not chasing the player.
            agent.speed = 2.0f; // Set to your default speed value (adjust as needed).

            DetectPlayer();

            if (!playerDetected)
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    GotoNextPoint();
            }
        }
        else
        {
            Vector3 directionToPlayer = player.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer <= chaseDistance)
            {
                if (CanSeePlayer())
                {
                    // Player is within chase distance and in the AI's field of view.
                    agent.speed = 2.0f + chaseSpeedIncrease; // Adjustable chase speed.
                    agent.SetDestination(player.position);
                    lastKnownPlayerPosition = player.position;

                    // Rotate the AI to face the player
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

                    
                   
                    headingToLastKnownPosition = false; // Player is in sight, not heading to last known position.
                }
                else if (!headingToLastKnownPosition)
                {
                    // Player is outside of the field of view, start heading to the last known position.
                    agent.SetDestination(lastKnownPlayerPosition);
                    headingToLastKnownPosition = true; // Set the flag to indicate heading to last known position.
                }
                else if (distanceToPlayer > resumePatrolDistance)
                {
                    // Player is out of range, set the flag to resume patrolling.
                    playerDetected = false;
                    headingToLastKnownPosition = false; // Player is out of range, no longer heading to last known position.
                }
            }
        }

        // Check if the AI patrol has reached the last known player position.
        if (headingToLastKnownPosition && Vector3.Distance(transform.position, lastKnownPlayerPosition) < 1.0f)
        {
            // The AI patrol has reached the last known position.
            agent.speed = 2.0f; // Reset to the default speed.
            playerDetected = false;
            headingToLastKnownPosition = false; // Resume patrolling.
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer <= fieldOfViewAngle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.collider.gameObject == player.gameObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (CanSeePlayer())
            {
                playerDetected = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the field of view cone in the scene view.
        Gizmos.color = fieldOfViewColor;
        Gizmos.DrawRay(transform.position, transform.forward * chaseDistance);
        Vector3 leftFOVDirection = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightFOVDirection = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftFOVDirection * chaseDistance);
        Gizmos.DrawRay(transform.position, rightFOVDirection * chaseDistance);

        // Draw the detection radius in the scene view.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw the last known player position as a pink circle.
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(lastKnownPlayerPosition, 1.0f);

        // Visualize the raycast when detecting the player.
        Gizmos.color = Color.green;
        if (playerDetected)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
            {
                Gizmos.DrawLine(transform.position, hit.point);
            }
        }
    }
}


