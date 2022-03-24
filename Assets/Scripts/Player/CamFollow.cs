using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] Transform follow;
    [SerializeField] Vector3 offsetPosition;
    //[SerializeField] Quaternion offsetRotation;
    [SerializeField] float positionClampSpeed;
    //[SerializeField] float rotationClampSpeed;

    // Update is called once per frame
    void Update()
    {
        if(follow) transform.position = Vector3.Lerp(transform.position, follow.position + offsetPosition, positionClampSpeed);
        //.rotation = Quaternion.Lerp(transform.rotation, follow.rotation, rotationClampSpeed);
    }
}
