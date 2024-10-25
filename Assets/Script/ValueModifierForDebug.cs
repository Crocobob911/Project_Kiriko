using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ValueModifierForDebug : MonoBehaviour {

    private static ValueModifierForDebug _instance = null;
    public static ValueModifierForDebug Instance {
        get {
            if (_instance is null) {
                _instance = FindObjectOfType<ValueModifierForDebug>();
                if (_instance is null) {
                    GameObject instance = new GameObject("ValueModifierForDebug");
                    _instance = instance.AddComponent<ValueModifierForDebug>();
                }
            }
            return _instance;
        }
    }
    
    private List<IValueModifierObserver> observers;

    [Header("Player Move Speed")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed {get => moveSpeed;}
    [SerializeField] private float runSpeed = 3f;
    public float RunSpeed {get => runSpeed;}
    
    [Space(5f), Header("Camera")]
    [SerializeField] private Vector2 zoomSpeed = new (-80f, 640f);
    public Vector2 ZoomSpeed {get => zoomSpeed;}
    [SerializeField, Range(0, 10)] private float camSensitivity = 1f;
    public float CamSensitivity {get => camSensitivity;}
    [SerializeField, Range(0f, 60f)] float camAngleMaximum = 40f;
    public float CamAngleMaximum {get => camAngleMaximum;}
    [SerializeField, Range(270f, 361f)] float camAngleMinimum = 300f;
    public float CamAngleMinimum {get => camAngleMinimum;}
    
    private void Awake() {
        if (_instance is null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(_instance != this) {
            Destroy(gameObject);
        }
        observers = new List<IValueModifierObserver>();
    }

    public void AddThisSubscriber(IValueModifierObserver observer) {
        observers.Add(observer);
    }

    public void ValueUpdateApply() {
        foreach (var ob in observers) {
            ob.ValueModified_Debug();
        }
    }
}


public interface IValueModifierObserver {
    public void ValueModified_Debug();
}

/*
[CustomEditor(typeof(ValueModifierForDebug))]
public class ValueModifierForDebug_Editor : Editor {
    SerializedProperty valueModifiers;

    void OnEnable() {
        valueModifiers = serializedObject.FindProperty("valueModifiers");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(valueModifiers);
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Apply")) {
            ValueModifierForDebug.Instance.ValueUpdateApply();
        }
    }
}*/

