using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
//using NaughtyAttributes.Editor;

namespace scripts.characters.creation
{
    public class ExpandLimbs : MonoBehaviour
    {
        [SerializeField,RequiredAttribute("Assign the root bone!")] private GameObject rootBone;
        [SerializeField,RequiredAttribute("Assign a foot end bone!")] private GameObject footEnd;
        [SerializeField] public List<BonePair> bonePairs = new List<BonePair>();

        private float ArmatureOffset;
    }
    [System.Serializable]
    public class BonePair
    {
        [SerializeField] public string pairName;
        [SerializeField,Tooltip("Second Bone to scale"),] public List<GameObject> bones = new List<GameObject>();
        [SerializeField,Tooltip("Whether or not child bones should keep their scales")] public bool preserveChildScale;
        [SerializeField, OnValueChanged("Expand"), AllowNesting] private float expandAmount = 1;
        private void Expand(float oldVal, float newVal)
        {
            foreach(var bone in bones)
            {
                if (bone != null)
                {
                    bone.transform.localScale = new Vector3(newVal, newVal, newVal);
                }

                if (preserveChildScale)
                {
                    foreach (var child in bone.GetComponentsInChildren<Transform>())
                    {
                        if (child.parent == bone.transform)
                        {
                            child.localScale = new Vector3(1 / newVal, 1 / newVal, 1 / newVal);
                        }
                    }
                }
            }
        }

        public void SetExpansionRate(float expansionRate)
        {
            foreach (var bone in bones)
            {
                if (bone != null)
                {
                    bone.transform.localScale = new Vector3(expansionRate, expansionRate, expansionRate);
                }

                if (preserveChildScale)
                {
                    foreach (var child in bone.GetComponentsInChildren<Transform>())
                    {
                        if (child.parent == bone.transform)
                        {
                            child.localScale = new Vector3(1 / expansionRate, 1 / expansionRate, 1 / expansionRate);
                        }
                    }
                }
            }
        }
    }
}
