#if UNITY_EDITOR  //Not included in Build
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Script {
    public class ValueModifier : MonoBehaviour {
        private List<IValueModifierObserver> observers;
        private static ValueModifier _instance = null;
        public static ValueModifier Instance {
            get {
                if (_instance is null) {
                    _instance = FindObjectOfType<ValueModifier>();
                    if (_instance is null) {
                        GameObject instance = new GameObject("ValueModifier");
                        _instance = instance.AddComponent<ValueModifier>();
                    }
                }
                return _instance;
            }
        }
        
        [Header("Player Move Speed")]
        [SerializeField] private float moveSpeed = 5f; public float MoveSpeed {get => moveSpeed;}
        [SerializeField] private float runSpeed = 3f; public float RunSpeed {get => runSpeed;}



        [Space(5f), Header("Camera")]
        [SerializeField] private Vector2 zoomSpeed = new (-80f, 640f); public Vector2 ZoomSpeed { get => zoomSpeed;}
        [SerializeField] private float camSensitivity = 1f; public float CamSensitivity {get => camSensitivity;}
        [SerializeField] float camAngleMaximum = 40f; public float CamAngleMaximum {get => camAngleMaximum;}
        [SerializeField] float camAngleMinimum = 300f; public float CamAngleMinimum {get => camAngleMinimum;}

        

        private void Awake() {
            if (_instance is null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(_instance != this) {
                Destroy(gameObject);
                Debug.LogWarning("Only one instance of ValueModifier can be active at a time");
            }
            observers = new List<IValueModifierObserver>();
        }

        
        public void AddSubscriber(IValueModifierObserver observer) {
            observers.Add(observer);
        }

        public void ValueUpdateApply() {
            foreach (var ob in observers) {
                ob.ValueModifierUpdated();
            }
        }
    }

    public interface IValueModifierObserver {
        public void ValueModifierUpdated();
    }

//Custom Editor
    [CustomEditor(typeof(ValueModifier))]
    [CanEditMultipleObjects]
    public class ValueModifierEditor : Editor {
        private SerializedProperty moveSpeedProperty;
        private SerializedProperty runSpeedProperty;
        private SerializedProperty zoomSpeedProperty;
        private SerializedProperty camSensitivityProperty;
        private SerializedProperty camAngleMaximumProperty;
        private SerializedProperty camAngleMinimumProperty;
    
        private ValueModifier instance;

        void OnEnable() {
            moveSpeedProperty = serializedObject.FindProperty("moveSpeed");
            runSpeedProperty = serializedObject.FindProperty("runSpeed");
            zoomSpeedProperty = serializedObject.FindProperty("zoomSpeed");
            camSensitivityProperty = serializedObject.FindProperty("camSensitivity");
            camAngleMaximumProperty = serializedObject.FindProperty("camAngleMaximum");
            camAngleMinimumProperty = serializedObject.FindProperty("camAngleMinimum");
            instance = serializedObject.targetObject as ValueModifier;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
        
            GUIStyle style = EditorStyles.helpBox;
            
            GUILayout.BeginVertical(style);
            //Player Movement
            EditorGUILayout.PropertyField(moveSpeedProperty);
            EditorGUILayout.PropertyField(runSpeedProperty);
        
            //Camera Movement
            EditorGUILayout.PropertyField(zoomSpeedProperty);
            EditorGUILayout.PropertyField(camSensitivityProperty);
            EditorGUILayout.PropertyField(camAngleMaximumProperty);
            EditorGUILayout.PropertyField(camAngleMinimumProperty);
            bool changed = serializedObject.ApplyModifiedProperties();
        
            if (changed) {
                instance.ValueUpdateApply();
                changed = false;
            }
            GUILayout.EndVertical();
        }
    }
#endif
}