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
    [SerializeField] Rigidbody2D body;
    [SerializeField] AnimationCurve accelerationCurve;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (updateArmyEvent == null)
            updateArmyEvent = new UnityEvent();
    }

    private void Update()
    {
        var currSpeed = body.velocity.sqrMagnitude;

        Debug.Log("Playercontroller, Update : currSpeed = " + currSpeed);

        if (move.magnitude > 0.1f && currSpeed < maxSpeed)
        {
            body.velocity = body.velocity.magnitude * move;
            var speedFactor = accelerationCurve.Evaluate(currSpeed / maxSpeed);
            body.AddForce(move * speed * speedFactor * Time.deltaTime, ForceMode2D.Impulse);
        }
        else if (move.magnitude > 0.1f && body.velocity.sqrMagnitude >= maxSpeed)
        {
            body.velocity = body.velocity.magnitude * move;
        }
        else
        {
            body.velocity /= 10f;
        }

        if (look.magnitude > 0.1f)
        {
            var angle = Vector3.SignedAngle(transform.up, look, transform.forward);
            transform.Rotate(transform.forward * angle);
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
