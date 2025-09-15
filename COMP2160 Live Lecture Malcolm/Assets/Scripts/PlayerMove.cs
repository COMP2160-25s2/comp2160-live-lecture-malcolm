using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Vector3 move = Vector3.up;
    [SerializeField] private float speed = 5.0f;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        transform.Translate(move * speed * Time.deltaTime);
    }
}
