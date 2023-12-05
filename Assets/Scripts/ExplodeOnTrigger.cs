using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnTrigger : MonoBehaviour
{
    public GameObject building;
    public float explosionDistanceMax = 5f;
    public float explosionSpeed = 1f;
    public float resetSpeed = 2f;
    public Vector3 randomnessRange = new Vector3(1f, 1f, 1f);

    private Dictionary<Transform, Vector3> originalPositions;
    private Dictionary<Transform, Vector3> explosionDirections;
    private bool isExploded = false;
    private bool isExploding = false;
    private bool resetInProgress = false;

    void Start()
    {
        originalPositions = new Dictionary<Transform, Vector3>();
        explosionDirections = new Dictionary<Transform, Vector3>();
        if (building != null)
        {
            AddChildrenRecursive(building.transform);
        }
    }

    void AddChildrenRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child != parent)
            {
                originalPositions[child] = child.position;
                explosionDirections[child] = GetRandomDirection() * explosionDistanceMax;
                AddChildrenRecursive(child);
            }
        }
    }

    Vector3 GetRandomDirection()
    {
        return new Vector3(
            Random.Range(-randomnessRange.x, randomnessRange.x),
            Random.Range(-randomnessRange.y, randomnessRange.y),
            Random.Range(-randomnessRange.z, randomnessRange.z)
        ).normalized;
    }

    void Update()
    {
        if (isExploding)
        {
            foreach (var kvp in originalPositions)
            {
                Transform child = kvp.Key;
                Vector3 explosionDirection = explosionDirections[child];
                Vector3 targetPosition = originalPositions[child] + explosionDirection;

                // Explode only to the max distance
                if ((child.position - originalPositions[child]).magnitude < explosionDistanceMax)
                {
                    child.position = Vector3.Lerp(child.position, targetPosition, explosionSpeed * Time.deltaTime);
                }
            }
        }
        else if (resetInProgress) // Reset positions when not exploding
        {
            foreach (var kvp in originalPositions)
            {
                Transform child = kvp.Key;
                // Lerp the position of the child back to its original position
                child.position = Vector3.Lerp(child.position, kvp.Value, resetSpeed * Time.deltaTime);
            }

            // Check if all children are close enough to their original positions to consider the reset complete
            bool allReset = true;
            foreach (var kvp in originalPositions)
            {
                if ((kvp.Key.position - kvp.Value).sqrMagnitude > 0.001f)
                {
                    allReset = false;
                    break;
                }
            }

            // If all children are reset, allow for a new explosion
            if (allReset)
            {
                resetInProgress = false;
                isExploded = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isExploded && !resetInProgress)
        {
            isExploding = true;
            isExploded = true; // Set exploded state for this trigger
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isExploding = false;
            resetInProgress = true; // Start resetting the building once the player exits the collider
        }
    }

    // This function could be called to manually reset the explosion from another script if needed
    public void ResetExplosion()
    {
        resetInProgress = true;
        isExploded = false;
    }
}