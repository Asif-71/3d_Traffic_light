using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;       // drag Player prefab here
    public Vector3 offset = new Vector3(0, 5, -8);
    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(player);  // always face the player
    }
}
