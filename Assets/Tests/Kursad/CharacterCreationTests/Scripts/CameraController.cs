using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes.Editor;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine.EventSystems;

namespace scripts.characters.creation
{
    public enum CameraMode { Face,Body};

    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private GameObject character;
        [SerializeField]private Vector3 bodyPosition = new Vector3();
        [SerializeField]private Quaternion bodyRotation = new Quaternion();

        [SerializeField]private Vector3 facePosition = new Vector3();
        [SerializeField]private Quaternion faceRotation = new Quaternion();

        [SerializeField]private Vector3 targetPos = new Vector3();
        [SerializeField]private Quaternion targetRot = new Quaternion();

        [SerializeField]private CameraMode mode;

        [SerializeField] private float rotationValue;

        private bool rotateCharacter;

        void Start()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
            if (targetCamera == null)
                targetCamera = FindObjectOfType<Camera>();
            SetCameraMode(CameraMode.Body);

            rotationValue = character.transform.rotation.eulerAngles.y;
        }

        void Update()
        {
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetPos, 0.2f);
            targetCamera.transform.rotation = Quaternion.Lerp(targetCamera.transform.rotation, targetRot, 0.2f);

            character.transform.rotation =
                Quaternion.Lerp(character.transform.rotation,
                Quaternion.Euler(character.transform.rotation.eulerAngles.x, rotationValue, character.transform.rotation.eulerAngles.z), 0.1f);

            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                rotateCharacter = true;
                if(EventSystem.current.IsPointerOverGameObject())
                {
                    rotateCharacter = false;
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                rotateCharacter = false;
            }

            //-----------

            if (rotateCharacter)
            {
                rotationValue -= Input.GetAxis("Mouse X") * 3f;
            }

            if(!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    float curPerc = (targetPos - bodyPosition).magnitude / (bodyPosition - facePosition).magnitude;

                    targetPos = Vector3.Lerp(bodyPosition, facePosition, curPerc + 0.125f * Input.mouseScrollDelta.y);
                    targetRot = Quaternion.Lerp(bodyRotation, faceRotation, curPerc + 0.125f * Input.mouseScrollDelta.y);
                }
            }
        }

        [Button("Set Body Transforms")]
        private void SetBodyTransforms()
        {
            bodyPosition = SceneView.lastActiveSceneView.camera.transform.position;
            bodyRotation = SceneView.lastActiveSceneView.camera.transform.rotation;
        }

        [Button("Set Face Transforms")]
        private void SetFaceTransforms()
        {
            facePosition = SceneView.lastActiveSceneView.camera.transform.position;
            faceRotation = SceneView.lastActiveSceneView.camera.transform.rotation;
        }

        public void SetCameraMode(CameraMode mode)
        {
            this.mode = mode;
            if(mode == CameraMode.Body)
            {
                targetPos = bodyPosition;
                targetRot = bodyRotation;
            }
            else if(mode == CameraMode.Face)
            {
                targetPos = facePosition;
                targetRot = faceRotation;
            }
        }
    }
}


