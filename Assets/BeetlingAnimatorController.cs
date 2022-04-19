using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetlingAnimatorController : MonoBehaviour
{
    [SerializeField] Animator animator;


    internal void SetController(float velocity, Minion.Direction direction)
    {
        Debug.Log("BeetlingAnimatorController,  SetController : velocity = " + velocity);
        switch (direction)
        {
            case Minion.Direction.Up:
                animator.SetBool("Up", true);
                animator.SetBool("Down", false);
                animator.SetBool("Left", false);
                animator.SetBool("Right", false);
                animator.SetFloat("Speed", velocity); 
                break;
            case Minion.Direction.Down:
                animator.SetBool("Up", false);
                animator.SetBool("Down", true);
                animator.SetBool("Left", false);
                animator.SetBool("Right", false);
                animator.SetFloat("Speed", velocity);
                break;
            case Minion.Direction.Left:
                animator.SetBool("Up", false);
                animator.SetBool("Down", false);
                animator.SetBool("Left", true);
                animator.SetBool("Right", false);
                animator.SetFloat("Speed", velocity);
                break;
            case Minion.Direction.Right:
                animator.SetBool("Up", false);
                animator.SetBool("Down", false);
                animator.SetBool("Left", false);
                animator.SetBool("Right", true);
                animator.SetFloat("Speed", velocity);
                break;
            case Minion.Direction.Null:
                animator.SetFloat("Speed", velocity);
                break;
            default:
                break;
        }
    }
}
