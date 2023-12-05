using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTriggerScript : MonoBehaviour
{
    public GameObject panelPrefab; // Assign your panel prefab in the inspector
    public Transform instantiationPoint; // Assign an empty GameObject as the instantiation point
    public float targetScale = 1f; // Set the target scale in the inspector
    public float expansionDuration = 2f; // Duration of the expansion in seconds
    public float existenceDuration = 5f; // How long the panel exists before disappearing
    public float bounceIntensity = 1.1f; // How much the panel will overshoot its target scale
    public float bounceDuration = 0.25f; // How long the bounce effect will last

    private GameObject instantiatedPanel;
    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {
            isPlayerInside = true;
            if (instantiatedPanel == null)
            {
                instantiatedPanel = Instantiate(panelPrefab, instantiationPoint.position, instantiationPoint.rotation);
                instantiatedPanel.transform.localScale = Vector3.zero; // Start from a small point
                StartCoroutine(ScaleAndBounce());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            // Start coroutine to lerp scale to zero then destroy the panel
            StartCoroutine(ShrinkAndDestroy(instantiatedPanel, expansionDuration));
        }
    }

    private IEnumerator ScaleAndBounce()
    {
        // Scale up to the target scale
        yield return StartCoroutine(ScaleOverTime(instantiatedPanel.transform, Vector3.zero, new Vector3(targetScale, targetScale, targetScale), expansionDuration));

        // Bounce effect
        yield return StartCoroutine(ScaleOverTime(instantiatedPanel.transform, instantiatedPanel.transform.localScale, new Vector3(targetScale * bounceIntensity, targetScale * bounceIntensity, targetScale * bounceIntensity), bounceDuration / 2));
        yield return StartCoroutine(ScaleOverTime(instantiatedPanel.transform, instantiatedPanel.transform.localScale, new Vector3(targetScale, targetScale, targetScale), bounceDuration / 2));

        // Start countdown to auto-destroy if player doesn't leave collider
        StartCoroutine(DestroyAfterTime(existenceDuration));
    }

    private IEnumerator ScaleOverTime(Transform targetTransform, Vector3 startScale, Vector3 endScale, float time)
    {
        float currentTime = 0.0f;
        do
        {
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }

    private IEnumerator ShrinkAndDestroy(GameObject panel, float time)
    {
        yield return StartCoroutine(ScaleOverTime(panel.transform, panel.transform.localScale, Vector3.zero, time));
        Destroy(panel);
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (instantiatedPanel != null && !isPlayerInside)
        {
            StartCoroutine(ShrinkAndDestroy(instantiatedPanel, expansionDuration));
        }
    }
}
