using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMove : MonoBehaviour
{
    // max_amplitude_ defines the height of the sine wave; period_ is the duration of 360 deg through it.
    public float max_amplitude_;
    public float period_;

    // This effectively determines the wavelength 
    public float x_speed_;

    // Acts as the vertical/horizontal midpoints of the wave (taken from start position
    private float midpoint_y_pos_;
    private float midpoint_x_pos_;

    // Used to control where in the sine wave this object starts at.
    public float start_time_in_osc_ = 0f;

    private float time_through_current_osc_ = 0f;

    // The max distance from the starting position this object is allowed to wander
    // If 0, keep moving in one direction forever; else go back and forth about start
    public float max_x_dist_;

    // Set to 1 or -1 to reverse direction of movement
    private int direction_ = 1;

    // Use to temporarily disable movement
    private bool is_paused_ = false;

    void Start()
    {
        midpoint_y_pos_ = transform.position.y - Mathf.Sin((start_time_in_osc_ / period_) * 2 * Mathf.PI) * max_amplitude_;
        midpoint_x_pos_ = transform.position.x;
        time_through_current_osc_ = start_time_in_osc_;

        SetStartPos();
    }

    void Update()
    {
        if(!is_paused_)
            Move();
    }

    private void Move()
    {
        if (Mathf.Abs(transform.position.x - midpoint_x_pos_) >= max_x_dist_)
            direction_ *= -1;

        time_through_current_osc_ += Time.deltaTime;
        if (time_through_current_osc_ >= period_)
            time_through_current_osc_ = 0f;
        float y_target = midpoint_y_pos_ + max_amplitude_ * Mathf.Sin((time_through_current_osc_ / period_) * 2f * Mathf.PI);
        //transform.position = new Vector3(transform.position.x, y, transform.position.z);

        transform.Translate(new Vector3(x_speed_ * direction_ * Time.deltaTime, y_target - transform.position.y, 0f));
    }

    // Set the y position according to the phase of oscillation this object starts at, relative to its y position in the editor.
    private void SetStartPos()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + max_amplitude_ * Mathf.Sin((time_through_current_osc_ / period_) * 2f * Mathf.PI));
    }

    public void Pause()
    {
        is_paused_ = true;
    }

    public void Unpause()
    {
        is_paused_ = false;
    }
}
