using UnityEngine;

public class WallDoorController : MonoBehaviour
{
    public Transform wallPivot;              // Pivot point of the wall/door
    public Vector3 closedRotationEuler;      // Euler rotation when wall is closed
    public Vector3 openRotationEuler;        // Euler rotation when wall is open
    public float rotationSpeed = 2f;         // Speed of rotation
    public bool wallIsOpen = false;

    // Reference to the WallLiftTrigger component
    public WallLiftTrigger wallLiftTrigger;

    private const byte TOTAL_DOOR_ACTIVATION_KEY_COUNT = 5;
    private byte keyCounterAct = 0;
    private Coroutine rotateRoutine;

    public void OpenWall()
    {
        keyCounterAct++;
        if ((keyCounterAct >= TOTAL_DOOR_ACTIVATION_KEY_COUNT) && !wallIsOpen)
        {
            /* Protect key detections */
            keyCounterAct = TOTAL_DOOR_ACTIVATION_KEY_COUNT;
            /* Note that the wall is open */
            wallIsOpen = true;
            /* Stop previous routine */
            if (rotateRoutine != null) StopCoroutine(rotateRoutine);
            /* Rotate wall */
            rotateRoutine = StartCoroutine(RotateWall(
                wallPivot.localRotation,
                Quaternion.Euler(openRotationEuler)
            ));
            /* Move the wall on Y axis */
            if (wallLiftTrigger != null)
                wallLiftTrigger.TriggerLift();
        }
    }

    public void CloseWall()
    {
        if (keyCounterAct > 0)
        {
            /* Update key count */
            keyCounterAct--;
            /* Close only if the wall was open */
            if (wallIsOpen == true)
            {
                /* Clear flag */
                wallIsOpen = false;
                /* Stop previous routine */
                if (rotateRoutine != null) StopCoroutine(rotateRoutine);
                /* Rotate wall */
                rotateRoutine = StartCoroutine(RotateWall(
                    wallPivot.localRotation,
                    Quaternion.Euler(closedRotationEuler)
                ));
            }
        }
    }

    private System.Collections.IEnumerator RotateWall(Quaternion from, Quaternion to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            wallPivot.localRotation = Quaternion.Lerp(from, to, t);
            yield return null;
        }
        wallPivot.localRotation = to; // Snap exactly to final rotation at end
    }
}
