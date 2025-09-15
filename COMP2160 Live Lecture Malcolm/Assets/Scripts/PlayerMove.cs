using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Vector3 move = Vector3.down;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(move * Time.deltaTime);        
    }
}
