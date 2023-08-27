using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponThrow))]
public class WTCustomInspector : Editor
{
    //get all variables
    SerializedProperty curvePointOffset,
    curveSphereRadius,
    returnPosition,
    ThrowForce,
    ThrowForceDirection,
    ThrowRotationTorque,
    ThrowRotationTorqueDirection,
    ShouldRotate,
    ReturningRotation,
    ReturnRotationSpeed,
    ReceivedRotation,
    LayersToStick;

    void OnEnable()
    {
        //Fetch the objects from the GameObject script to display in the inspector
        returnPosition = serializedObject.FindProperty("returnPosition");
        curvePointOffset = serializedObject.FindProperty("curvePointOffset");
        curveSphereRadius = serializedObject.FindProperty("curveSphereRadius");

        ThrowForce = serializedObject.FindProperty("ThrowForce");
        ThrowForceDirection = serializedObject.FindProperty("ForceDirection");
        ThrowRotationTorque = serializedObject.FindProperty("RotationalTorque");
        ThrowRotationTorqueDirection = serializedObject.FindProperty("TorqueDirection");
        ReceivedRotation = serializedObject.FindProperty("ReceivedRotation");
        ShouldRotate = serializedObject.FindProperty("ShouldRotate");
        ReturningRotation = serializedObject.FindProperty("ReturningRotation");
        ReturnRotationSpeed = serializedObject.FindProperty("ReturnRotationSpeed");
        LayersToStick = serializedObject.FindProperty("LayersToStick");
    }

    public override void OnInspectorGUI(){
        
        var button = GUILayout.Button("Click for more tools");
        if (button) Application.OpenURL("https://assetstore.unity.com/publishers/39163");
        EditorGUILayout.Space(5);

        WeaponThrow script = (WeaponThrow)target;
        
        EditorGUILayout.LabelField("Return Options", EditorStyles.boldLabel);
        //Return Position
        EditorGUILayout.PropertyField(returnPosition, new GUIContent("Return Position", "The return transform position that the weapon would return to."));

        //Curve Point Offset
        EditorGUILayout.PropertyField(curvePointOffset, new GUIContent("Curve Point Offset", "The offset point from the return position where the weapon will curve through before reaching the return position. Shown as a sphere in editing view."));

        EditorGUILayout.PropertyField(curveSphereRadius, new GUIContent("Curve Sphere Radius", "The radius of the sphere for better debugging only."));

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Throw Options", EditorStyles.boldLabel);
        //Throw Force
        EditorGUILayout.PropertyField(ThrowForce, new GUIContent("Throw Force", "The force power of the throw."));

        //Throw force direction
        EditorGUILayout.PropertyField(ThrowForceDirection, new GUIContent("Throw Force Direction", "The axis on which the force should be applied."));

        //Throw rotation torque
        EditorGUILayout.PropertyField(ThrowRotationTorque, new GUIContent("Throw Rotation Torque", "The power of the torque that rotates the weapon on throw."));

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Rotate Options", EditorStyles.boldLabel);
        //Throw rotation torque direction
        EditorGUILayout.PropertyField(ThrowRotationTorqueDirection, new GUIContent("Rotation Torque Direction", "The axis on which the rotation torque should be applied."));

        //Received rotation
        EditorGUILayout.PropertyField(ReceivedRotation, new GUIContent("Received Rotation", "The rotation of the weapon when received."));

        //Should rotate
        EditorGUILayout.PropertyField(ShouldRotate, new GUIContent("Rotate on Return", "Should the weapon rotate when returning."));

        EditorGUI.BeginDisabledGroup (script.ShouldRotate == false);
            EditorGUILayout.PropertyField(ReturningRotation, new GUIContent("Returning Rotation", "The axis to where the rotation should be applied."));
            EditorGUILayout.PropertyField(ReturnRotationSpeed ,new GUIContent("Rotation Speed", "Rotation speed of return."));
        EditorGUI.EndDisabledGroup ();

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Stick Options", EditorStyles.boldLabel);
        //Layers To Stick
        EditorGUILayout.PropertyField(LayersToStick, new GUIContent("Layers To Stick", "The layers that make the weapon stick to it if hit."));

        //apply
        serializedObject.ApplyModifiedProperties();
    }
}