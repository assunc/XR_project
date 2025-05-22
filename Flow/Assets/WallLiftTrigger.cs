using UnityEngine;

public class WallLiftTrigger : MonoBehaviour
{
    public Transform wallToLift;       // The wall that moves vertically
    public float liftHeight = 2f;      // How high it moves
    public float liftSpeed = 1f;       // Speed of movement

    private bool hasMoved = false;
    private Coroutine liftRoutine;

    /* Call this method to move the wall up once */
    public void TriggerLift()
    {
        if (hasMoved || wallToLift == null)
            return;

        hasMoved = true;

        if (liftRoutine != null)
            StopCoroutine(liftRoutine);

        liftRoutine = StartCoroutine(LiftWallCoroutine());
    }

    private System.Collections.IEnumerator LiftWallCoroutine()
    {
        Vector3 startPos = wallToLift.position;
        Vector3 endPos = startPos + Vector3.up * liftHeight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * liftSpeed;
            wallToLift.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        wallToLift.position = endPos; // Snap to final position
    }
}
