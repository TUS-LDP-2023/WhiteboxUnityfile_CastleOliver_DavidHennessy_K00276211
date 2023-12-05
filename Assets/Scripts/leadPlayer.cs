using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class leadPlayer : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public Transform player;
    private int pointIndex = 0;
    private NavMeshAgent agent;
    public float detectionRadius = 5f;
    public float stopLeadingDistance = 10f; // Distance at which AI stops leading the player.
    public float waypointTolerance = 1.0f; // How close AI needs to get to waypoint to consider it "reached".

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false; // Prevent the agent from slowing down as it approaches a waypoint.
        MoveToNextPoint();
    }

    void MoveToNextPoint()
    {
        if (points.Count == 0) return;

        agent.destination = points[pointIndex].position;
    }

    void Update()
    {
        // Check if we're close enough to the current waypoint, if so, proceed to next
        if (!agent.pathPending && agent.remainingDistance < waypointTolerance)
        {
            pointIndex = (pointIndex + 1) % points.Count;
            MoveToNextPoint();
        }

        // Check the distance to the player
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // If the player is within detection radius, lead them to the next point
        if (distanceToPlayer <= detectionRadius)
        {
            LeadPlayer();
        }
        else if (distanceToPlayer > stopLeadingDistance)
        {
            // If the player is too far away, stop leading and wait for them to approach
            agent.isStopped = true;
        }
        else if (agent.isStopped && distanceToPlayer <= stopLeadingDistance)
        {
            // If the player has come back within the leading distance, resume leading
            agent.isStopped = false;
        }
    }

    void LeadPlayer()
    {
        // Check if we're at the last point, in which case don't move further
        if (pointIndex == points.Count - 1 && agent.remainingDistance < waypointTolerance)
        {
            agent.isStopped = true;
            return;
        }

        if (agent.isStopped)
        {
            // If stopped, resume movement
            agent.isStopped = false;
        }

        // Move to the next point in the patrol route
        MoveToNextPoint();
    }
}
