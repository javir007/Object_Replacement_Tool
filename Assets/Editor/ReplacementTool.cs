using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplacementTool : EditorWindow
{
    static List<GameObject> replacement = new List<GameObject>();

    [Tooltip("Assign here the new Objects that you want to replace")]
    public List<GameObject> replacementObject = new List<GameObject>();

    public bool modifyTransform = false;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotation;
    public Vector3 scale = new Vector3(1,1,1);

    

    [MenuItem("Javir Urro/Replacement Tool")]
    public static void ShowEditorWindow(){
        EditorWindow window = GetWindow<ReplacementTool>("Replacement Objects Tool");
        Vector2 minSize = new Vector2(200f,450f);
        window.minSize = minSize; 
    }
    
    private void OnGUI(){
        //Creation of Labels and Layout
        GUILayout.Label("Objects to Replace",EditorStyles.boldLabel);

        //Serialization to use a GameObject List inside an Editor Window
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty elementsList = so.FindProperty("replacementObject");
        EditorGUILayout.PropertyField(elementsList,true);
        so.ApplyModifiedProperties();
        replacement = replacementObject;
        GUILayout.Space(10f);
        
        EditorGUILayout.HelpBox("How to use the tool" + "\n \n"+ "1. Assign the new Objects inside the Replacement Object List"
        +"\n"+"2. Select in the Hierarchy Tab all the Objects that you want to Replace"+"\n"
        +"3. Click on the Button Replace"+"\n\n"
        +"Note: The Replace Button will appear when the new Objects are assigned",MessageType.Info);
        GUILayout.Space(10f);
        //Creation of Transform Vectors on Editor Window
        modifyTransform = EditorGUILayout.Toggle ("Modify Transform", modifyTransform);
        if(modifyTransform){
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            GUILayout.Space(5f);
            positionOffset = EditorGUILayout.Vector3Field("Position",positionOffset);
            GUILayout.Space(5f);
            rotation = EditorGUILayout.Vector3Field("Rotation",rotation);
            GUILayout.Space(5f);
            scale = EditorGUILayout.Vector3Field("Scale",scale);
            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }
        //Check if there is not empty elements on the List
        GUILayout.Space(10f);
        if(!CheckButton())
            EditorGUILayout.HelpBox("One or more objects are Empty inside the List",MessageType.Warning);
        
        if(CheckButton()&&replacementObject.Count>0){
            if(GUILayout.Button("Replace")){
                Replace();
            }
        }
    }

    bool CheckButton(){
        foreach(GameObject go in replacementObject){
           if(go == null)
               return false;
       }
       return true;
    }

    void Replace(){
    if(replacement == null)
        return;

    //Get the Selection from the Assets selected on Hierarchy
    Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
    int listLenght = replacement.Count;
    if(transforms.Length>0){
        foreach(Transform t in transforms){
         
        int index = listLenght>1 ? Random.Range(0,listLenght):0;
        GameObject newGo = null;
        PrefabAssetType pref = PrefabUtility.GetPrefabAssetType(replacement[index]);
        newGo = CheckPrefabType(pref,index);
        SetTransform(newGo,t);

        UnityEngine.Object.DestroyImmediate(t.gameObject);
        Undo.RegisterCreatedObjectUndo(newGo,"Replace Selection");
        }
    }
}
//Check the Type of the prefab
private GameObject CheckPrefabType(PrefabAssetType p, int index){

    if(p == PrefabAssetType.Regular || p == PrefabAssetType.Model){
            return((GameObject)PrefabUtility.InstantiatePrefab(replacement[index]));
        }
        else{
            return((GameObject)Editor.Instantiate(replacement[index]));
        }
}
//Modify the transform of the new elements
void SetTransform(GameObject go,Transform t){
        
        if(!modifyTransform){
            go.transform.position = t.position;
            go.transform.rotation = t.rotation;
            go.transform.localScale = t.localScale;
        }else{
            go.transform.position = t.position + positionOffset;
            go.transform.Rotate(rotation);
            go.transform.localScale =  scale;
        }

        go.transform.parent = t.parent;
}
  
}
