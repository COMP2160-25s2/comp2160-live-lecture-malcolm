/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;

public class CameraFollow3 : MonoBehaviour
{

#region Parameters
    [SerializeField] private int fixedFrameRate = 100; // fps
    [SerializeField] private float followDecay = 0.05f;
    [SerializeField] private float offset = 5; // m
#endregion 

#region Connected Objects
    [SerializeField] private PlayerMove target;
#endregion

#region Components
#endregion

#region State
    private Rigidbody2D targetRigidbody;
    private Transform targetTransform;
    private Vector3 desiredPosition;
#endregion

#region Init & Destroy
    void Awake()
    {
        targetRigidbody = target.GetComponent<Rigidbody2D>();
        targetTransform = target.transform;

        desiredPosition = targetTransform.position;
    }
#endregion 

#region Update
    void LateUpdate()
    {
        // update horizontal positio
        desiredPosition.x = targetTransform.position.x;

        // horizontal movement is in front of player
        if (targetRigidbody.linearVelocity.x > 0)
        {
            desiredPosition.x += offset;            
        }
        else if (targetRigidbody.linearVelocity.x < 0)
        {
            desiredPosition.x -= offset;            
        }

        // reset desired vertical position only when the player is on the grounfd

        if (target.State == PlayerMove.JumpState.OnGround)
        {
            desiredPosition.y = targetTransform.position.y;
        }

        // lerp towards desired position at fixed frame rate
        for (float time = 0; time < Time.deltaTime; time += 1f / fixedFrameRate)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followDecay);            
        }
    }
#endregion

#region Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Don't run in the editor
            return;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
#endregion
}
