using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

class SteppedMotionConstraintWindow : EditorWindow {
    public GameObject constrainedRig;
    public GameObject targetRig;

    [MenuItem("Animation Rigging/Stepped Motion Chain Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SteppedMotionConstraintWindow));
    }

    void OnGUI()
    {
        constrainedRig = (GameObject)EditorGUILayout.ObjectField("target rig", constrainedRig, typeof(GameObject), true);
        targetRig = (GameObject)EditorGUILayout.ObjectField("Source A", targetRig, typeof(GameObject), true);
        Transform active = Selection.activeTransform;

        EditorGUI.BeginDisabledGroup(active == null || targetRig == null || constrainedRig == null);
        if (GUILayout.Button(constrainedRig == null ? "Slect Object to apply Rig" : "Build Blend Rig to " + active.name))
        {

            DeleteChildren(active);
            SteppedMotionBone(constrainedRig.transform, targetRig.transform, active);

            if (active.GetComponent<Rig>() == null)
            {
                Undo.AddComponent(active.gameObject, typeof(Rig));
            }
        }
        EditorGUI.EndDisabledGroup();
    }

    void DeleteChildren(Transform target)
    {
        int childCount = target.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Undo.DestroyObjectImmediate(target.GetChild(0).gameObject);
        }
    }

    void SteppedMotionBone(Transform currentReference, Transform targetReference, Transform current)
    {
        for (int i = 0; i < currentReference.transform.childCount; i++)
        {
            Transform curBone = currentReference.transform.GetChild(i);
            Transform curA = targetReference.transform.GetChild(i);

            GameObject newBlend = new GameObject("stepped:" + curBone.name);
            Undo.RegisterCreatedObjectUndo(newBlend, "created stepped constraint object");

            newBlend.transform.parent = current;

            SteppedMotionConstraint newConstraint = newBlend.AddComponent<SteppedMotionConstraint>();
            newConstraint.data.source = curBone;
            newConstraint.data.target = curA;

            SteppedMotionBone(curBone, curA, newBlend.transform);
        }
    }
}
