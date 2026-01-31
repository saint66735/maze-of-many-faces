using UnityEngine;
using System.Collections;

public class StepRotateLocalY : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float degreesPerStep = 90f;
    [SerializeField] private float rotationDuration = 0.5f;

    [Header("Timing")]
    [SerializeField] private float waitTimeBetweenSteps = 1f;

    private void Start()
    {
        StartCoroutine(RotationLoop());
    }

    private IEnumerator RotationLoop()
    {
        while (true)
        {
            yield return StartCoroutine(RotateStep());
            yield return new WaitForSeconds(waitTimeBetweenSteps);
        }
    }

    private IEnumerator RotateStep()
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, degreesPerStep, 0f);

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;

            // Smooth interpolation
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        // Snap exactly to target at the end to avoid drift
        transform.localRotation = targetRotation;
    }
}
