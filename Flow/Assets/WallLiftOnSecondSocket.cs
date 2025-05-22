using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WallLiftOnSecondSocket : MonoBehaviour
{
    public Transform wallToMove;
    public float liftHeight = 2f;
    public float liftSpeed = 1f;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    private int activationCount = 0;
    private bool wallIsLifted = false;
    private Coroutine moveRoutine;

    private void OnEnable()
    {
        socket.selectEntered.AddListener(OnSocketActivated);
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnSocketActivated);
    }

    private void OnSocketActivated(SelectEnterEventArgs args)
    {
        activationCount++;

        if (activationCount == 2 && !wallIsLifted)
        {
            wallIsLifted = true;

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(MoveWallUpward());
        }
    }

    private System.Collections.IEnumerator MoveWallUpward()
    {
        Vector3 startPos = wallToMove.position;
        Vector3 endPos = startPos + Vector3.up * liftHeight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * liftSpeed;
            wallToMove.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        wallToMove.position = endPos; // snap to final position
    }
}