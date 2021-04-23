using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyCam : MonoBehaviour
{
    public GameObject target;

    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    public float RotateSmoothTime = 0.1f;
    private float AngularVelocity = 0.0f;

    private Transform targetTransform;
    private Quaternion targetQuaternion;

    private void Start()
    {
        targetTransform = target.transform;
        targetQuaternion = target.transform.rotation;
    }

    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = targetTransform.TransformPoint(new Vector3(0, 0, -0.18f));

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        //Vector3 rotationVec = targetQuaternion.eulerAngles;
        //transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, target.transform.rotation.eulerAngles, ref velocity, smoothTime * Time.deltaTime));
        //transform.rotation = target.transform.rotation;
        //transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, rotationVec, ref velocity, smoothTime));

        //var target_rot = Quaternion.LookRotation(targetTransform.position - transform.position);
        var target_rot = targetTransform.rotation;
        var delta = Quaternion.Angle(transform.rotation, target_rot);
        if (delta > 0.0f)
        {
            var t = Mathf.SmoothDampAngle(delta, 0.0f, ref AngularVelocity, RotateSmoothTime);
            t = 1.0f - t / delta;
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, t);
        }
    }



}

