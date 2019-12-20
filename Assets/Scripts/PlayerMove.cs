using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float move_speed_ = 1f;

    private InputRetriever input_;

    private void Start()
    {
        input_ = GetComponent<InputRetriever>();
    }

    void Update()
    {
        float hor_move = input_.GetAxis("Horizontal") * move_speed_ * Time.deltaTime;
        float ver_move = input_.GetAxis("Vertical") * move_speed_ * Time.deltaTime;

        transform.Translate(hor_move, ver_move, 0f);
    }
}
