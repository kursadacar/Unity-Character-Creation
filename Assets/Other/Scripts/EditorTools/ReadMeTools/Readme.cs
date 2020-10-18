using System;
using UnityEngine;

[Serializable,CreateAssetMenu]
public class Readme : ScriptableObject
{
    [TextArea(4,40)]
    public string Header = "Header";
    [TextArea(4,40)]
    public string info = "Write here";
}
