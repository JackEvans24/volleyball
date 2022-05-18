using UnityEngine;

public class ResetBall : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private Vector3 origin;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            ball.position = origin;
        }
    }
}
