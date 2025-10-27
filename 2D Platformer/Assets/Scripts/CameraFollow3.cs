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
    [Header("Debug")]
    [SerializeField] private bool capFrameRate = true;
    [SerializeField] private int targetFrameRate = 30;
#endregion 

#region Connected Objects
    [Header("Connected Objects")]
    [SerializeField] private PlayerMove player;
#endregion

#region Components
#endregion

#region State
    private Rigidbody2D playerRigidbody;
    private Transform playerTransform;
    private Vector3 targetPosition;
#endregion

#region Init & Destroy
    void Awake()
    {
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        playerTransform = player.transform;

        targetPosition = playerTransform.position;

        // For QA: we can cap the frame rate to see how this runs
        // TODO: We should migrate this to a general QA Manager class.

    }
#endregion 

#region Update
    void LateUpdate()
    {
        if (capFrameRate)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        }
        else
        {
            // ignore Application.targetFrameRate, instead use refresh rate of the monitor
            QualitySettings.vSyncCount = 1;            
        }
 
        // update horizontal positio
        targetPosition.x = playerTransform.position.x;

        // horizontal movement is in front of player
        if (playerRigidbody.linearVelocity.x > 0)
        {
            targetPosition.x += offset;            
        }
        else if (playerRigidbody.linearVelocity.x < 0)
        {
            targetPosition.x -= offset;            
        }

        // reset desired vertical position only when the player is on the grounfd

        if (player.State == PlayerMove.JumpState.OnGround)
        {
            targetPosition.y = playerTransform.position.y;
        }

        // lerp towards desired position at fixed frame rate
        Debug.Log($"deltaTime = {Time.deltaTime}, FPS = {1f / Time.deltaTime}");

        for (float time = 0; time < Time.deltaTime; time += 1f / fixedFrameRate)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followDecay);            
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

        // show where the camera is heading
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(targetPosition, 0.1f);
    }
#endregion
}
