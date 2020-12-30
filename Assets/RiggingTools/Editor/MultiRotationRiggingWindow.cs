using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MultiRotationRiggingWindow : EditorWindow
{
    public GameObject sourceRig;
    public GameObject targetRig;

    [MenuItem("Animation Rigging/Multirotation Rigging Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MultiRotationRiggingWindow));
    }

    void OnGUI()
    {
        sourceRig = (GameObject)EditorGUILayout.ObjectField("source", sourceRig, typeof(GameObject), true);
        targetRig = (GameObject)EditorGUILayout.ObjectField("target", targetRig, typeof(GameObject), true);
        Transform active = Selection.activeTransform;

        EditorGUI.BeginDisabledGroup(active == null || sourceRig == null || targetRig == null);
        if (GUILayout.Button("Build Blend Rig to " + active.name))
        {

            DeleteChildren(active);
            MultirotateBone(sourceRig.transform, targetRig.transform, active);

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

    void MultirotateBone(Transform source, Transform target, Transform current)
    {
        for (int i = 0; i < source.transform.childCount; i++)
        {
            Transform sourceBone = source.transform.GetChild(i);
            Transform targetBone = target.transform.GetChild(i);

            GameObject newBlend = new GameObject("parent:" + sourceBone.name);
            Undo.RegisterCreatedObjectUndo(newBlend, "created blend object");
            newBlend.transform.parent = current;

            MultiRotationConstraint newConstraint = newBlend.AddComponent<MultiRotationConstraint>();
            newConstraint.data.constrainedObject = sourceBone;

            WeightedTransformArray wta = new WeightedTransformArray(0);
            wta.Add(new WeightedTransform(targetBone, 1));
            newConstraint.data.sourceObjects = wta;

            MultirotateBone(sourceBone, targetBone, newBlend.transform);
        }
    }
}
