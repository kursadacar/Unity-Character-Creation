using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace scripts.characters.creation
{
    public class PropertyList : MonoBehaviour
    {
        [SerializeField] private GameObject GroupHolderPrefab;
        [SerializeField] private GameObject KeyEditorPrefab;
        [SerializeField] private GameObject ItemSelectorPrefab;
        [SerializeField] private GameObject BaseCharacter;
        [SerializeField] private CharacterCreationData CharacterCreationData;

        [SerializeField] private Transform ListHolder;

        void Start()
        {
            if (CharacterCreationData.SkinColors.Count > 0)
            {
                var SkinSelector = Instantiate(ItemSelectorPrefab, ListHolder).GetComponent<ItemSelector>();
                SkinSelector.name = "Skin Selector";
                SkinSelector.GetComponentInChildren<Text>().text = "Skin Tone";

                foreach (var skinColor in CharacterCreationData.SkinColors)
                {
                    var newTex = new Texture2D(32, 32);

                    //set color of the texture
                    var colors = newTex.GetPixels();
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = skinColor;
                    }
                    newTex.SetPixels(colors);
                    var newObj = SkinSelector.AddItem(newTex, delegate { SetSkinTone(skinColor); });
                }
            }

            if(CharacterCreationData.EyeColors.Count > 0)
            {
                var EyeSelector = Instantiate(ItemSelectorPrefab, ListHolder).GetComponent<ItemSelector>();
                EyeSelector.name = "Eye Selector";
                EyeSelector.GetComponentInChildren<Text>().text = "Eye Color";

                foreach (var eyeColor in CharacterCreationData.EyeColors)
                {
                    var newObj = EyeSelector.AddItem(eyeColor, delegate { SetEyeColor(eyeColor); });
                }
            }

            if (CharacterCreationData.EyeBrows.Count > 0)
            {
                var EyebrowSelector = Instantiate(ItemSelectorPrefab, ListHolder).GetComponent<ItemSelector>();
                EyebrowSelector.name = "Eyebrow Selector";
                EyebrowSelector.GetComponentInChildren<Text>().text = "Eyebrow";


                foreach (var eyebrow in CharacterCreationData.EyeBrows)
                {
                    var newObj = EyebrowSelector.AddItem(eyebrow, delegate { SetEyebrows(eyebrow); });
                }
            }

            //foreach (var limb in BaseCharacter.GetComponent<ExpandLimbs>().bonePairs)
            //{
            //    var newObj = Instantiate(KeyEditorPrefab, ListHolder).GetComponent<ShapeKeySlider>();
            //    newObj.SetupLimb(BaseCharacter,limb);
            //}


            var mesh = BaseCharacter.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            var keyCount = mesh.blendShapeCount;

            List<GameObject> parents = new List<GameObject>();
            GameObject activeParent = null;

            for (int i = 0; i < keyCount; i++)
            {
                string blendshapeName = mesh.GetBlendShapeName(i);
                string header = blendshapeName.Split('_')[0];
                if(parents.Where(x=> x.name == header).Count() == 0)
                {
                    GameObject newgo = Instantiate(GroupHolderPrefab, ListHolder);
                    newgo.name = header;
                    newgo.GetComponentInChildren<Text>().text = header;
                    parents.Add(newgo);
                }
                activeParent = parents.Where(x => x.name == header).ElementAt(0);
                //var wordList = blendshapeName.Split('_').ToList();
                //wordList.RemoveAt(0);
                //blendshapeName = string.Join("_", wordList);

                var newObj = Instantiate(KeyEditorPrefab, activeParent.transform).GetComponent<ShapeKeySlider>();
                newObj.SetupShapeKey(BaseCharacter,i, blendshapeName);
                newObj.propertyType = ShapeKeySlider.PropertyType.ShapeKey;
            }

        }

        void SetEyeColor(Texture2D eyeColor)
        {
            BaseCharacter.GetComponent<CharacterMaterial>().Eyes.SetTexture("_BaseColorMap", eyeColor);

            Debug.Log("Set Eye color..");
        }

        void SetSkinTone(Color skinTone)
        {
            foreach(var mat in BaseCharacter.GetComponent<CharacterMaterial>().bodyMats)
            {
                mat.SetColor("_BaseColor", skinTone);
            }
            Debug.Log("Set Skin tone..");
        }

        void SetEyebrows(Texture2D eyebrows)
        {
            BaseCharacter.GetComponent<CharacterMaterial>().Eyebrows.SetTexture("_BaseColorMap", eyebrows);
            Debug.Log("Set Eyebrows..");
        }
    }

}
