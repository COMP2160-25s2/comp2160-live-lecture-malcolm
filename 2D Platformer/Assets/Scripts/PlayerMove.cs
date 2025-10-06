/**
 * 
 * Handles movement of the player avatar
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{

#region Parameters
    [SerializeField] private float gravity = -10f;  // m/s/s
    [SerializeField] private float maxFallSpeed = -20f; // m/s 
    [SerializeField] private float horizontalSpeed = 10f;  // m/s/s
#endregion 

#region Connected Objects
#endregion

#region Components
    private Rigidbody2D rigidbody;
#endregion

#region State
    private Actions actions;
    private InputAction moveAction;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        actions = new Actions();
        moveAction = actions.PlayerMove.Move;
    }

    void OnEnable() 
    {
        actions.PlayerMove.Enable();
    }

    void OnDisable() 
    {
        actions.PlayerMove.Disable();
    }
#endregion 

#region Update
    void Update()
    {
    }
#endregion

// Time     Update      FixedUpdate     pos (v=10m/s)
// 0.0      1           1               0
// 0.03     1           1               0.2m
// 0.06     1           2               0.6m
// 0.09     1           1               0.8m
// 0.12     1           2               1.2m

#region FixedUpdate
    void FixedUpdate()
    {
        Vector3 v = rigidbody.linearVelocity;

        // vertical movement
        v.y += gravity * Time.fixedDeltaTime;
        v.y = Mathf.Max(v.y, maxFallSpeed);     // clamp to maxFallSpeed

        // horizontal movement
        v.x = moveAction.ReadValue<float>() * horizontalSpeed;

        rigidbody.linearVelocity = v;
    }
#endregion

#region Gizmos
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Handles.color = Color.white;
            Handles.Label(transform.position, $"rigidbody.linearVelocity = {rigidbody.linearVelocity}"); 
            return;
        }
    }
#endregion
}
