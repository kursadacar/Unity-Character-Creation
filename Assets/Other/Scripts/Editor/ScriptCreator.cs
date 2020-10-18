using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class ScriptCreator : EditorWindow
{
    private Vector2 scroll;
    public class Createable
    {
        public string @name;
        public string @directory;
        public string @namespace;
        public ClassModifier @classModifier;
        public BaseClassType @baseClass;
        public bool @makeSerializable;
        public string classModifierStr,baseClassStr;
        
        public void Create()
        {
            string actualDir = "";
            string[] pieces = directory.Split('/');
            int l = pieces.Length;
            for (int i = 5; i < l; i++)
            {
                actualDir += pieces[i];
                actualDir += "/";
            }

            actualDir += name + ".cs";
            Debug.Log(actualDir);
            using (StreamWriter outfile = new StreamWriter(actualDir))
            {
                outfile.WriteLine("using UnityEngine;");
                outfile.WriteLine("using System;");
                outfile.WriteLine("using System.Collections;");
                outfile.WriteLine("using System.Collections.Generic;");
                outfile.WriteLine("using Object = UnityEngine.Object;");

                outfile.WriteLine("using GameObject = UnityEngine.GameObject;\n");

                if (!string.IsNullOrEmpty(@namespace))
                {
                    outfile.WriteLine("namespace "+@namespace+" {\n");
                }
            
                if (makeSerializable)
                    outfile.WriteLine("\t[Serializable]");

                string com = baseClassStr.Equals(" ") ? " " : " : ";
                string classBaseName = "\t"+classModifierStr + "class " + name + com + baseClassStr;
                outfile.WriteLine(""+classBaseName + "{");

                outfile.WriteLine(" ");




                outfile.WriteLine("\n");



                outfile.WriteLine("\t}");
            
                if (!string.IsNullOrEmpty(@namespace))
                {
                    outfile.WriteLine("}");
                }
            }
        }

    }

    private List<Createable> scripts;
    public enum ClassModifier
    {
        @static,
        @abstract,
        @none
    }

    public enum  BaseClassType
    {
        MonoBehavior,
        ScriptableObject,
        EditorWindow,
        Custom,
        None
    }
    [MenuItem("Window/Extensions/ScriptCreator")] 
    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(ScriptCreator));
    }
    
    void OnGUI ()
    {
        if(scripts==null)
            scripts=new List<Createable>();
        scroll=GUILayout.BeginScrollView(scroll,EditorStyles.helpBox);

        for (int i = 0; i < scripts.Count; i++)
        {
            Createable script = scripts[i];
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name: ",GUILayout.Width(140));
            script.@name = GUILayout.TextField(script.@name);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Directory: ",GUILayout.Width(140));
            if (GUILayout.Button("Choose a Folder"))
                script.@directory = EditorUtility.OpenFolderPanel("Choose a folder", Application.dataPath, "def");
            GUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(script.directory))
                EditorGUILayout.HelpBox("Directory is not valid",MessageType.Error);
            else
            {
                string path = script.directory + "/" + name + ".cs";
                GUILayout.Label("Chosen Dir: "+path);
            }
            if (File.Exists(script.directory + "/" + name + ".cs"))
            {
                string path = script.directory + "/" + name + ".cs";
                EditorGUILayout.HelpBox("File "+path+" already exists!",MessageType.Error);

            }
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Namespace: ",GUILayout.Width(140));
            script. @namespace = GUILayout.TextField(script.@namespace);
            GUILayout.EndHorizontal();

            
            
            
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Class Modifier: ",GUILayout.Width(140));
            script.@classModifier = (ClassModifier)EditorGUILayout.EnumPopup(script.classModifier);
            GUILayout.EndHorizontal();
            

            switch (script.@classModifier)
            {
                case ClassModifier.@abstract:
                    script.classModifierStr = "public abstract ";
                    break;
                case ClassModifier.@static:
                    script.classModifierStr = "static ";
                    break;
                case ClassModifier.@none:
                    script.classModifierStr = "public ";
                    break;
                
            }
            
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Base Class: ",GUILayout.Width(140));
            script.@baseClass = (BaseClassType)EditorGUILayout.EnumPopup(script.baseClass);
            GUILayout.EndHorizontal();

            
            switch (script.@baseClass)
            {
                case BaseClassType.MonoBehavior:
                    script.baseClassStr = "MonoBehaviour";
                    break;
                case BaseClassType.EditorWindow:
                    script.baseClassStr = "EditorWindow ";
                    break;
                case BaseClassType.ScriptableObject:
                    script.baseClassStr = "ScriptableObject ";
                    break;
                case BaseClassType.None:
                    script.baseClassStr = " ";
                    break;
                case BaseClassType.Custom:
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Custom Base Class: ",GUILayout.Width(140));
                    script.baseClassStr = GUILayout.TextField(script.baseClassStr);
                    GUILayout.EndHorizontal();
                    break;
            }
            
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Serializable? : ",GUILayout.Width(140));
            script.makeSerializable = GUILayout.Toggle(script.makeSerializable,"");
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Script"))
            scripts.Add(new Createable());
        if (GUILayout.Button("Delete Last Script"))
            scripts.RemoveAt(scripts.Count-1);
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Create"))
        {
            foreach (var script in scripts)
                script.Create();
            AssetDatabase.Refresh();
        }
        
    }

}