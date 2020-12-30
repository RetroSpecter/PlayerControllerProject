using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

class BlendRiggingWindow : EditorWindow {
    public GameObject constrainedRig;
    public GameObject sourceA;
    public GameObject sourceB;

    [MenuItem("Animation Rigging/Blend Rigging Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BlendRiggingWindow));
    }

    void OnGUI()
    {
        constrainedRig = (GameObject)EditorGUILayout.ObjectField("target rig", constrainedRig, typeof(GameObject), true);
        sourceA = (GameObject)EditorGUILayout.ObjectField("Source A", sourceA, typeof(GameObject), true);
        sourceB = (GameObject)EditorGUILayout.ObjectField("Source B", sourceB, typeof(GameObject), true);
        Transform active = Selection.activeTransform;

        EditorGUI.BeginDisabledGroup(active == null || sourceA == null || sourceB == null || constrainedRig == null);
        if (GUILayout.Button("Build Blend Rig to " + active.name)) {

            DeleteChildren(active);
            BlendRigBone(constrainedRig.transform, sourceA.transform, sourceB.transform, active);

            if (active.GetComponent<Rig>() == null) {
                Undo.AddComponent(active.gameObject, typeof(Rig));
            }
        }
        EditorGUI.EndDisabledGroup();
    }

    void DeleteChildren(Transform target) {
        int childCount = target.childCount;
        for (int i = 0; i < childCount; i++) {
            Undo.DestroyObjectImmediate(target.GetChild(0).gameObject);
        }
    }

    void BlendRigBone(Transform currentReference, Transform A, Transform B, Transform current) {
        for (int i = 0; i < currentReference.transform.childCount; i++) {
            Transform curBone = currentReference.transform.GetChild(i);
            Transform curA = A.transform.GetChild(i);
            Transform curB = B.transform.GetChild(i);

            GameObject newBlend = new GameObject("blend:" + curBone.name);
            Undo.RegisterCreatedObjectUndo(newBlend, "created blend object");

            newBlend.transform.parent = current;

            BlendConstraint newConstraint = newBlend.AddComponent<BlendConstraint>();
            newConstraint.data.constrainedObject = curBone;
            newConstraint.data.sourceObjectA = curA;
            newConstraint.data.sourceObjectB = curB;

            BlendRigBone(curBone, curA, curB, newBlend.transform);
        }
    }
}
