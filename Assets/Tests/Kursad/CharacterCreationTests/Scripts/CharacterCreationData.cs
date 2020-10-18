using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Creation Data", menuName = "Character Creation Data")]
public class CharacterCreationData : ScriptableObject
{
    public List<Texture2D> EyeColors = new List<Texture2D>();
    public List<Color> SkinColors = new List<Color>();
    public List<Texture2D> EyeBrows = new List<Texture2D>();
}
