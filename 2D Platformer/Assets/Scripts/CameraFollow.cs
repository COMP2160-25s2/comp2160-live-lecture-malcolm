/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;

public class CameraFollow : MonoBehaviour
{

#region Parameters
    [SerializeField] private int fixedFrameRate = 100; // fps
    [SerializeField] private float followDecay = 0.05f;
#endregion 

#region Connected Objects
    [SerializeField] private Transform target;
#endregion

#region Components
#endregion

#region State
#endregion

#region Init & Destroy
    void Awake()
    {
    }
#endregion 

#region Update
    void LateUpdate()
    {
        // check out Freya Holmer's video about lerp to know why this is wrong
        // https://www.youtube.com/watch?v=LSNQuFEDOyQ

        // do this at the end of the update cycle to guarantee everything has already moved

        for (float time = 0; time < Time.deltaTime; time += 1f / fixedFrameRate)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, followDecay);            
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
