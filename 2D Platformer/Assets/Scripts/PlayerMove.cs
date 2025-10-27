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
    [Header("Horizontal movement")]
    [SerializeField] private float horizontalSpeed = 10f;  // m/s/s

    [Header("Vertical movement")]
    [SerializeField] private float gravity = -10f;  // m/s/s
    [SerializeField] private float jumpHeight = 2.5f;  // m
    [SerializeField] private float maxFallSpeed = -20f; // m/s 
    [SerializeField] private float maxOnGroundAngle = 45f;  // degrees
    [SerializeField] private float jumpBufferDuration = 0.1f;  // s
    [SerializeField] private float hoverDuration = 0.1f;  // s

    [Header("Gizmos")]
    [SerializeField] private float maxHistoryDuration = 10f;  // s
#endregion 

#region Connected Objects
#endregion

#region Components
    private new Rigidbody2D rigidbody;
#endregion

#region State
    public enum JumpState { OnGround, RisingWithGravity, Hovering, FallingWithGravity, FallingWithoutGravity };
    private JumpState jumpState = JumpState.FallingWithGravity;
    public JumpState State
    {
        get { return jumpState; }
    }

    private Actions actions;
    private InputAction moveAction;
    private List<ContactPoint2D> contacts;
    private float lastJumpPressedTime = float.NegativeInfinity;
    private bool isJumpHeld;
    private float hoverTimer = 0;

    private struct HistoryItem
    {
        public float time;
        public Vector3 position;
        public JumpState jumpState;
    }
    private Queue<HistoryItem> history;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0; // we're doing our own gravity

        actions = new Actions();
        moveAction = actions.PlayerMove.Move;

        contacts = new List<ContactPoint2D>();

        HistoryItem now = new HistoryItem();
        now.time = Time.time;
        now.position = transform.position;
        now.jumpState = jumpState;

        history = new Queue<HistoryItem>();
        history.Enqueue(now);
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
            isJumpHeld = true;
        }        

        if (actions.PlayerMove.Jump.WasReleasedThisFrame())
        {
            isJumpHeld = false;
        }        

        HistoryItem now = new HistoryItem();
        now.time = Time.time;
        now.position = transform.position;
        now.jumpState = jumpState;
        history.Enqueue(now);

        // remove old stuff from the history
        while (history.Peek().time < Time.time - maxHistoryDuration)
        {        
            history.Dequeue();
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
        Vector3 v = rigidbody.linearVelocity;

        v.y = UpdateJumpState(v.y);

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

#region FSM
    private float UpdateJumpState(float vy)
    {
        bool isOnGround = IsOnGround();
            
        switch (jumpState)
        {
            case JumpState.OnGround:
                if (!isOnGround)
                {
                    jumpState = JumpState.FallingWithGravity;
                    Debug.Log("[PlayerMove.UpdateJumpState] jumpState: OnGround -> FallingWithGravity");
                }
                else if (Time.time - lastJumpPressedTime < jumpBufferDuration)
                {
                    // the player has pressed jump within the jumpBuffer time
                    jumpState = JumpState.RisingWithGravity;
                    Debug.Log("[PlayerMove.StartJump] jumpState: OnGround -> RisingWithGravity");

                    // Calculate jump speed to guarantee jump height
                    // v^2 = u^2 + 2as
                    // At the top of the jump, v = 0, a = gravity, s = jumpHeight
                    // u^2 = sqrt(-2as)

                    vy = Mathf.Sqrt(-2 * gravity * jumpHeight);

                    // forget the jump request by setting it to infinitely into the past
                    lastJumpPressedTime = float.NegativeInfinity;
                }
                else
                {
                    // nothing to do
                }

                break;

            case JumpState.RisingWithGravity:

                vy += gravity * Time.fixedDeltaTime;        

                if (!isJumpHeld)
                {
                    jumpState = JumpState.FallingWithGravity;
                    Debug.Log("[PlayerMove.UpdateJumpState] jumpState: RisingWithGravity -> FallingWithGravity");
                }
                else if (vy <= 0)
                {
                    jumpState = JumpState.Hovering;
                    vy = 0;
                    hoverTimer = hoverDuration;
                    Debug.Log("[PlayerMove.UpdateJumpState] jumpState: RisingWithGravity -> Hovering");
                }
                break;

            case JumpState.Hovering:
                hoverTimer -= Time.deltaTime;

                if (!isJumpHeld)
                {
                    jumpState = JumpState.FallingWithGravity;
                    Debug.Log("[PlayerMove.UpdateJumpState] jumpState: Hovering -> FallingWithGravity");
                }
                else if (hoverTimer < 0)
                {
                    jumpState = JumpState.FallingWithGravity;
                    Debug.Log("[PlayerMove.UpdateJumpState] jumpState: Hovering -> FallingWithGravity");
                }
                break;

            case JumpState.FallingWithGravity:
                if (isOnGround)
                {
                    jumpState = JumpState.OnGround;
                    Debug.Log("[PlayerMove.UpdateJumpState] jumpState: Gravity -> OnGround");
                }
                else
                {
                    vy += gravity * Time.fixedDeltaTime;        
  
                    if (vy < maxFallSpeed)
                    {
                        jumpState = JumpState.FallingWithoutGravity;
                        Debug.Log("[PlayerMove.UpdateJumpState] jumpState: Gravity -> FallingNoGravity");
                        vy = maxFallSpeed;
                    }  
                }
                break;

            case JumpState.FallingWithoutGravity:
                if (isOnGround)
                {
                    jumpState = JumpState.OnGround;
                    Debug.Log("[PlayerMove.UpdateJumpState] jumpState: FallingNoGravity -> OnGround");
                }
                else
                {
                    vy = maxFallSpeed;   
                }
                break;
        }
        return vy;

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

        DrawTrailGizmo();
    }


    private void DrawTrailGizmo()
    {
        Gizmos.color = Color.white;
        Vector3? oldPos = null; // nullable type https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references

        foreach (HistoryItem h in history)
        {            
            if (oldPos != null)
            {
                switch (h.jumpState)
                {
                    case JumpState.OnGround:
                        Gizmos.color = Color.white;
                        break;

                    case JumpState.RisingWithGravity:
                        Gizmos.color = Color.red;
                        break;

                    case JumpState.Hovering:
                        Gizmos.color = Color.yellow;
                        break;

                    case JumpState.FallingWithGravity:
                        Gizmos.color = Color.green;
                        break;

                    case JumpState.FallingWithoutGravity:
                        Gizmos.color = Color.blue;
                        break;

                }
                Gizmos.DrawLine(oldPos.Value, h.position);
            }
            oldPos = h.position;                
        }
    }
#endregion
}
