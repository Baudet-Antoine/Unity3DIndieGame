using System.Collections;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{
    public Transform Opened;
    public Transform Closed;
    public float moveSpeed = 2f;

    private bool isOpening = false;
    private bool isClosing = false;
    private bool isAnimating = false;
    private Vector3 targetPosition;
    private Coroutine currentAnimation = null;

    void Update()
    {
        if (isOpening && !isAnimating)
        {
            StartAnimation(Opened.position);
            isOpening = false;
        }
        else if (isClosing && !isAnimating)
        {
            StartAnimation(Closed.position);
            isClosing = false;
        }
    }

    public void OpenDoor()
    {
        if (!isAnimating || (currentAnimation != null && targetPosition == Closed.position))
        {
            isOpening = true;
        }
    }

    public void CloseDoor()
    {
        if (!isAnimating || (currentAnimation != null && targetPosition == Opened.position))
        {
            isClosing = true;
        }
    }

    private void StartAnimation(Vector3 newTargetPosition)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        targetPosition = newTargetPosition;
        currentAnimation = StartCoroutine(MoveDoor(targetPosition));
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        isAnimating = true;
        Vector3 startingPosition = transform.position;
        float distance = Vector3.Distance(startingPosition, targetPosition);
        float journeyLength = distance / moveSpeed;
        float startTime = Time.time;

        while (Time.time - startTime < journeyLength)
        {
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distCovered / distance;
            transform.position = Vector3.Lerp(startingPosition, targetPosition, fractionOfJourney);
            yield return null;
        }

        transform.position = targetPosition;
        isAnimating = false;
        currentAnimation = null;
    }
}
