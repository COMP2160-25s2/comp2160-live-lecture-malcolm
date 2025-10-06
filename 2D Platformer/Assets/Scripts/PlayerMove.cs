/**
 * 
 * Handles movement of the player avatar
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{

#region Parameters
    [SerializeField] private float gravity = -10f;  // m/s/s
    [SerializeField] private float maxFallSpeed = -20f; // m/s 
#endregion 

#region Connected Objects
#endregion

#region Components
    private Rigidbody2D rigidbody;
#endregion

#region State
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
#endregion 

#region Update
    void Update()
    {
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
    {
        Vector3 v = rigidbody.linearVelocity;
        v.y += gravity * Time.fixedDeltaTime;
        v.y = Mathf.Max(v.y, maxFallSpeed);     // clamp to maxFallSpeed
        rigidbody.linearVelocity = v;
    }
#endregion

#region Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Handles.color = Color.white;
            Handles.Label(transform.position, $"rigidbody.linearVelocity = {rigidbody.linearVelocity}"); 
            return;
        }
    }
#endregion
}
