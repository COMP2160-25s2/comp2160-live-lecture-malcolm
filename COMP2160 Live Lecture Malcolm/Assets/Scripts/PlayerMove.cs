using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Vector3 move;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(move * Time.deltaTime);        
    }
}
