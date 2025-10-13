/**
 * 
 * Handles movement of the player avatar
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using System.Collections.Generic;
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
    [SerializeField] private float jumpSpeed = 10f;  // m/s
    [SerializeField] private float maxOnGroundAngle = 45f;  // degrees
    [SerializeField] private float jumpBufferTime = 0.1f;  // s
#endregion 

#region Connected Objects
#endregion

#region Components
    private Rigidbody2D rigidbody;
#endregion

#region State
    private Actions actions;
    private InputAction moveAction;
    private List<ContactPoint2D> contacts;

    private float lastJumpPressedTime = float.NegativeInfinity;
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

        contacts = new List<ContactPoint2D>();
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
        // remember when jump was last pressed
        if (actions.PlayerMove.Jump.WasPressedThisFrame())
        {
            lastJumpPressedTime = Time.time;
        }        
    }
#endregion

// Input happens on Update frame but is processed on FixedUpdate, 
// so we can't guarantee that WasPressedThisFrame will be registered if we test in FixedUpdate
//
// 0     Update   press space     Was Pressed = true          jumpPressed = true
// 0.01  Update   space held      Was Pressed = false         jumpPressed = true
// 0.02  Fixed    ...             Was Pressed = false         jumpPressed = false

#region FixedUpdate
    void FixedUpdate()
    {
        // get all the contact points
        // note: this method only exists in 2D physics.
        // in 3D physics you will need to collect these points yourself using OnCollisionStay() 
        rigidbody.GetContacts(contacts);

        // jump buffer time allows jumps that are performed a fraction of a second too early
        // remember that jump was pressed and process it once we hit the ground

        if (Time.time - lastJumpPressedTime < jumpBufferTime && IsOnGround())
        {
            // impulse = mass * deltaVelocity
            // so we need to scale impulse by mass
            // in 3D we could use ForceMode.DeltaVelocity but that doesn't exist in 2D

            rigidbody.AddForce(Vector2.up * jumpSpeed * rigidbody.mass, ForceMode2D.Impulse);                

            // forget the jump request by setting it to infinitely into the past
            lastJumpPressedTime = float.NegativeInfinity;
        }

        Vector3 v = rigidbody.linearVelocity;

        // vertical movement
        v.y += gravity * Time.fixedDeltaTime;
        v.y = Mathf.Max(v.y, maxFallSpeed);     // clamp to maxFallSpeed

        // horizontal movement
        v.x = moveAction.ReadValue<float>() * horizontalSpeed;

        rigidbody.linearVelocity = v;
    }

    private bool IsOnGround()
    {        
        // check if any of the contact points is pointing upwards
        // i.e. the angle between the normal and the up vector is less than maxOnGroundAngle

        foreach (ContactPoint2D cp in contacts)
        {
            float angle = Vector2.Angle(cp.normal, Vector2.up);
            if (angle < maxOnGroundAngle)
            {
                return true;
            }
        }

        return false;
    }
#endregion

#region Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        // Draw both the rigidbody position and the transform position
        // these may be different if rigidbody interpolation is turned on

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(rigidbody.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.05f);

        // Use handles to write the velocity at the rigidbody location (and down a bit)
        Handles.color = Color.white;
        Handles.Label(rigidbody.position + Vector2.down * 0.1f, $"rigidbody.linearVelocity = {rigidbody.linearVelocity}"); 

        // draw all the contact points and normals

        Gizmos.color = Color.blue;
        foreach (ContactPoint2D cp in contacts)
        {
            Gizmos.DrawSphere(cp.point, 0.05f);
            Gizmos.DrawLine(cp.point, cp.point + cp.normal);
        }

    }
#endregion
}
