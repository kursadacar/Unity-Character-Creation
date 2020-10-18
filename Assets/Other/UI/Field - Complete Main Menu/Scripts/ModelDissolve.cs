using UnityEngine;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class ModelDissolve : MonoBehaviour
    {
        [Header("VARIABLES")]
        public Material dissolveMaterial;
        [Range(0.0f, 1.0f)] public float dissolveValue = 1.0f;
        [Range(0.1f, 2.5f)] public float animationSpeed = 0.5f;

        [Header("SETTINGS")]
        public bool playAtStart;
        private bool playing;

        ParticleSystem ps;

        void Start()
        {
            ps = GetComponentInChildren<ParticleSystem>();

            if (playAtStart)
            {
                dissolveValue = 1;
                Dissolve();
            }

            else
            {
                dissolveValue = 1;
            }
        }

        public void Disable()
        {
            if (playing)
            {
                animationSpeed -= 0.4f;
            }
            playing = false;
        }

        public void Dissolve()
        {
            if (playing == false)
            {
                animationSpeed += 0.4f;
            }
            playing = true;
        }

        void Update()
        {
            if (playing)
            {
                if (dissolveValue == 0 || dissolveValue >= 0)
                {
                    dissolveValue -= Time.deltaTime / animationSpeed;
                    ps.Play();
                }
            }

            else
            {
                if (dissolveValue == 1 || dissolveValue <= 1)
                {
                    dissolveValue += Time.deltaTime / animationSpeed;
                }
            }

            dissolveMaterial.SetFloat("_cutoff", dissolveValue);
        }
    }
}