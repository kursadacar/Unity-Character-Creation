using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraBehaviour : MonoBehaviour
{
    public Vector3 cameraOffset;
    //private ICharacter target;

    void Start()
    {

    }
    void Update()
    {
        //if(target == null)
        //{
        //    target = NetworkUtility.myPlayer;
        //    return;
        //}
        //transform.position = Vector3.Lerp(transform.position, target.Position + cameraOffset, Time.deltaTime *2);
        //transform.rotation = Quaternion.LookRotation(-cameraOffset, Vector3.up);
    }

    //void ChangeTarget(Player _target)
    //{
    //    target = _target;
    //}
}
