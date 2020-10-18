using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EyesLookAt : MonoBehaviour
{
    [SerializeField, RequiredAttribute] private GameObject LeftEyeBone;
    [SerializeField, RequiredAttribute] private GameObject RightEyeBone;
    [SerializeField, RequiredAttribute] private GameObject target;

    [SerializeField] float rotationClamp;

    private void LateUpdate()
    {
        LeftEyeBone.transform.LookAt(target.transform);
        RightEyeBone.transform.LookAt(target.transform);

        LeftEyeBone.transform.Rotate(90, 0, 0, Space.Self);
        RightEyeBone.transform.Rotate(90, 0, 0, Space.Self);
    }

    public void SetTargetPosition(Vector3 pos)
    {
        target.transform.position = pos;
    }
}
