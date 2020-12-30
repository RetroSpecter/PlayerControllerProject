using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DampChainWindow : EditorWindow
{
    public GameObject headBone;
    public GameObject tailBone;

    [MenuItem("Animation Rigging/Damp Chain Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DampChainWindow));
    }

    void OnGUI()
    {
        headBone = (GameObject)EditorGUILayout.ObjectField("Head Bone", headBone, typeof(GameObject), true);
        tailBone = (GameObject)EditorGUILayout.ObjectField("Tail Bone", tailBone, typeof(GameObject), true);
        Transform active = Selection.activeTransform;


        EditorGUI.BeginDisabledGroup(active == null || headBone == null || headBone == null);
        if (GUILayout.Button("Build Damp Rig to " + active.name)){
            DeleteChildren(active);

            if (active.GetComponent<Rig>() == null) {
                Undo.AddComponent(active.gameObject, typeof(Rig));
            }
            AutoRig(active.gameObject, headBone, tailBone);
        }
        EditorGUI.EndDisabledGroup();
    }

    void DeleteChildren(Transform target) {
        int childCount = target.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Undo.DestroyObjectImmediate(target.GetChild(0).gameObject);
        }
    }

    public void AutoRig(GameObject target, GameObject headBone, GameObject tailBone)
    {
        List<GameObject> intermediateBones = new List<GameObject>();
        intermediateBones.Add(tailBone);

        while (intermediateBones[intermediateBones.Count - 1] != headBone
                && intermediateBones[intermediateBones.Count - 1].transform.parent != null) {
            intermediateBones.Add(intermediateBones[intermediateBones.Count - 1].transform.parent.gameObject);
        }

        if (intermediateBones[intermediateBones.Count - 1] != headBone) {
            Debug.LogError("head bone and tail bone not connected");
            return;
        }

        for (int i = intermediateBones.Count - 1; i > 0; i--)
        {
            GameObject newDamp = new GameObject("damp" + (intermediateBones.Count - i));
            Undo.RegisterCreatedObjectUndo(newDamp, "created blend object");

            newDamp.transform.parent = target.transform;
            DampedTransform dampSettings = newDamp.AddComponent(typeof(DampedTransform)) as DampedTransform;
            dampSettings.data.constrainedObject = intermediateBones[i - 1].transform;
            dampSettings.data.sourceObject = intermediateBones[i].transform;
        }
    }
}
