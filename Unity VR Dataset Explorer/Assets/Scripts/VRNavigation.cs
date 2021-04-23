using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRNavigation : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller leftController, rightController;

    public Rigidbody NaviBase;

    public GameObject controlerObject;
    public float thrustForce;
    public float maxSpeed;
    private float speedUp = 1;
    private float movingSpeed;

    public bool movementAllowed;
    public bool spaceMovement;

    public GameObject filterMenu;

    public bool somethingGrabed;

    public bool flatNavigation; 
    public GameObject centerEye;

    private void Start()
    {
        somethingGrabed = false;
    }

    void FixedUpdate()
    {
        if (somethingGrabed == false)
        {
            movingSpeed = thrustForce;
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController))
            {
                movingSpeed = thrustForce * speedUp;
                if (speedUp < maxSpeed)
                {
                    speedUp += 0.2f;
                }
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController) == false)
                {
                    NaviBase.drag = 4;
                }

            }
            else
            {
                speedUp = 1;
                if (filterMenu.activeSelf == false)
                {
                    NaviBase.drag = 0.5f;
                }
                else
                {
                    NaviBase.drag = 4;
                }
            }

            // add force
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                if (movementAllowed == true)
                {
                    if (spaceMovement == true)
                    {
                        NaviBase.AddForce(controlerObject.transform.rotation * Vector3.forward * movingSpeed);
                    }
                    else
                    {
                        if (flatNavigation == false) 
                        {
                            Quaternion flatDirection = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                            NaviBase.AddForce(flatDirection * Vector3.forward * movingSpeed);
                        }
                    }
                    NaviBase.maxAngularVelocity = 2f;
                }
            }
        }
        if (somethingGrabed == true)
        {
            movingSpeed = thrustForce;
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, leftController))
            {
                movingSpeed = thrustForce * speedUp;
                if (speedUp < maxSpeed)
                {
                    speedUp += 0.2f;
                }
                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController) == false)
                {
                    NaviBase.drag = 4;
                }

            }
            else
            {
                speedUp = 1;
                NaviBase.drag = 0.5f;
            }

            // add force
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController))
            {
                if (movementAllowed == true)
                {
                    NaviBase.AddForce(controlerObject.transform.rotation * Vector3.forward * movingSpeed);
                    NaviBase.maxAngularVelocity = 2f;
                }
            }
        }

        //2D Navigation

        if(flatNavigation == true && spaceMovement == false)
        {
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, rightController).x != 0)
            {
                NaviBase.drag = 1;
                Vector2 touchPadVec = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, rightController);
                float thisSpeed = Vector2.Distance(new Vector2(0, 0), touchPadVec);
                float flatYDirection = euler_angle(touchPadVec.x, touchPadVec.y);
                float centerEyeYRotation = centerEye.transform.rotation.eulerAngles.y;
                flatYDirection = centerEyeYRotation - flatYDirection;
                Quaternion flatDirection = Quaternion.Euler(0, flatYDirection, 0);
                NaviBase.AddForce(flatDirection * new Vector3(0, 0, thisSpeed) * movingSpeed * 3);
            } else
            {
                NaviBase.drag = 12;
            }

        }

    }

    float euler_angle(float x, float y)
    {
        float rad = Mathf.Atan(y / x);
        float deg = rad * 180 / Mathf.PI;
        if (x < 0) deg += 180;
        float eul = (270 + deg) % 360;
        return eul;
    }
}