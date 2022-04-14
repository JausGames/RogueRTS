using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent updateArmyEvent;


    [SerializeField] Vector2 move = Vector2.zero;
    [SerializeField] Vector2 look = Vector2.zero;
    [SerializeField] float speed = 50f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] Rigidbody body;
    [SerializeField] AnimationCurve accelerationCurve;

    internal void SetSpeed(float speed)
    {
        maxSpeed = 3f * speed;
        this.speed = maxSpeed * 3.333f;
    }

    

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        if (updateArmyEvent == null)
            updateArmyEvent = new UnityEvent();
    }

    private void Update()
    {
        var currSpeed = body.velocity.sqrMagnitude;

        Debug.Log("Playercontroller, Update : currSpeed = " + currSpeed);

        var v3Move = move.x * Vector3.right + move.y * Vector3.forward;
        var v3Look = look.x * Vector3.right + look.y * Vector3.forward;
        if (move.magnitude > 0.1f && currSpeed < maxSpeed)
        {
            body.velocity = body.velocity.magnitude * v3Move;
            var speedFactor = accelerationCurve.Evaluate(currSpeed / maxSpeed);
            body.AddForce(v3Move * speed * speedFactor * Time.deltaTime, ForceMode.Impulse);
            Debug.Log("Playercontroller, Update : normal acc = " + v3Move * speed * speedFactor * Time.deltaTime);
            Debug.Log("Playercontroller, Update : normal acc speedFactor = " + speedFactor);
        }
        else if (move.magnitude > 0.1f && body.velocity.sqrMagnitude >= maxSpeed)
        {
            body.velocity = Mathf.Sqrt(maxSpeed) * v3Move;
            Debug.Log("Playercontroller, Update : max speed move = " + Mathf.Sqrt(maxSpeed) * v3Move);
        }
        else
        {
            body.velocity /= 10f;
        }

        if (look.magnitude > 0.1f)
        {
            var angle = Vector3.SignedAngle(transform.forward, v3Look, transform.up);
            transform.Rotate(transform.up * angle);
        }
        else body.angularVelocity /= 10f;


        updateArmyEvent.Invoke();
    }

    public void SetMove(Vector2 move)
    {
        this.move = move;
    }

    public void SetLook(Vector2 look)
    {
        this.look = look;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)move);
    }

    void ForceForwardDirection(Vector3 localVelocity)
    {
        //moslty have forward speed
        var driftSpeedRatio = Mathf.Pow(localVelocity.x, 2f) / Mathf.Pow(Mathf.Abs(localVelocity.x) + Mathf.Abs(localVelocity.y), 2f);
        //var breakRatio = carSettings.stopDriftingCurve.Evaluate(driftSpeedRatio);
        var breakRatio = 0.2f;
        localVelocity.x *= breakRatio;
        //body.velocity = Vector3.Lerp(body.velocity, transform.TransformDirection(localVelocity), carSettings.driftSpeedBoostRatio * Time.deltaTime);
        body.velocity = Vector3.Lerp(body.velocity, transform.TransformDirection(localVelocity), 1.5f * Time.deltaTime);
        
    }
}
