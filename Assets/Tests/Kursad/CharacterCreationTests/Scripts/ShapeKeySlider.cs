using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace scripts.characters.creation
{
    public class ShapeKeySlider : MonoBehaviour
    {
        public enum PropertyType
        {
            ShapeKey,
            Limbs
        }

        private GameObject BaseCharacter;
        private SkinnedMeshRenderer meshToEdit;
        private BonePair bonePairToScale;
        private int shapeID;

        private bool adjustNormals = false;

        private string _shapeName;

        public PropertyType propertyType;
        public string shapeName
        {
            get
            {
                return _shapeName;
            }
            set
            {
                _shapeName = value;
                GetComponentInChildren<Text>().text = value;
            }
        }


        public void SetupShapeKey(GameObject baseChar,int shapeKeyID,string shapeKeyName)
        {
            propertyType = PropertyType.ShapeKey;

            BaseCharacter = baseChar;

            shapeID = shapeKeyID;

            //Debug.Log(shapeKeyName);

            shapeKeyName = shapeKeyName.Replace('_', ' ');

            if (shapeKeyName[shapeKeyName.Length - 1] == 'N')
            {
                adjustNormals = true;
                shapeKeyName = shapeKeyName.Remove(shapeKeyName.Length - 2, 2);
            }

            if (shapeKeyName[shapeKeyName.Length-1] == 'Q')
            {
                GetComponentInChildren<Slider>().minValue = -25;
                shapeKeyName = shapeKeyName.Remove(shapeKeyName.Length - 2,2);
            }
            else if(shapeKeyName[shapeKeyName.Length-1] == 'H')
            {
                GetComponentInChildren<Slider>().minValue = -50;
                shapeKeyName = shapeKeyName.Remove(shapeKeyName.Length - 2, 2);
            }
            shapeName = shapeKeyName;

            meshToEdit = BaseCharacter.GetComponentInChildren<SkinnedMeshRenderer>();
            SetShapeKey(0);
        }

        public void SetupLimb(GameObject baseChar,BonePair bonePair)
        {
            propertyType = PropertyType.Limbs;

            BaseCharacter = baseChar;
            bonePairToScale = bonePair;
            shapeName = bonePair.pairName;
            GetComponentInChildren<Slider>().minValue = .8f;
            GetComponentInChildren<Slider>().maxValue= 1.2f;
        }

        public void SetShapeKey(float value)
        {
            if(propertyType == PropertyType.ShapeKey)
            {
                meshToEdit.SetBlendShapeWeight(shapeID,value);
            }
            else if(propertyType == PropertyType.Limbs)
            {
                bonePairToScale.SetExpansionRate(value);
            }
        }
    }
}
