#if UNITY_EDITOR  //Not included in Build
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        
        // 근데 이 쪼끄만 클래스 하나에서
        // 프로젝트 내의 모든 가변적인 변수들을 가지고 있다는 게 말이 되나?
        // 각 객체에서 가지고 와야하지 않을까?
        
        [Header("플레이어 움직임")]
        [SerializeField] private float moveSpeed; public float MoveSpeed => moveSpeed;
        [SerializeField] private float sprintSpeed; public float SprintSpeed => sprintSpeed;



        [Space(5f), Header("카메라")]
        [SerializeField] private float camSensitivity; public float CamSensitivity => camSensitivity;
        [SerializeField] private float zoomSpeed; public float ZoomSpeed => zoomSpeed;
        [SerializeField] private float zoomMin;  public float ZoomMin => zoomMin;
        [SerializeField] private float zoomMax;   public float ZoomMax => zoomMax;
        [SerializeField] private float camAngleMaximum;   public float CamAngleMaximum => camAngleMaximum;
        [SerializeField] private float camAngleMinimum;  public float CamAngleMinimum => camAngleMinimum;
        

        //....이 방식이 맞나? 이게 맞음?
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
        private SerializedProperty sprintSpeedProperty;
        private SerializedProperty camSensitivityProperty;
        private SerializedProperty zoomSpeedProperty;
        private SerializedProperty zoomMinProperty;
        private SerializedProperty zoomMaxProperty;
        private SerializedProperty camAngleMaximumProperty;
        private SerializedProperty camAngleMinimumProperty;
    
        private ValueModifier instance;

        void OnEnable() {
            moveSpeedProperty = serializedObject.FindProperty("moveSpeed");
            sprintSpeedProperty = serializedObject.FindProperty("sprintSpeed");
            camSensitivityProperty = serializedObject.FindProperty("camSensitivity");
            zoomSpeedProperty = serializedObject.FindProperty("zoomSpeed");
            zoomMinProperty = serializedObject.FindProperty("zoomMin");
            zoomMaxProperty = serializedObject.FindProperty("zoomMax");
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
            EditorGUILayout.PropertyField(sprintSpeedProperty);
        
            //Camera Movement
            EditorGUILayout.PropertyField(camSensitivityProperty);
            EditorGUILayout.PropertyField(zoomSpeedProperty);
            EditorGUILayout.PropertyField(zoomMinProperty);
            EditorGUILayout.PropertyField(zoomMaxProperty);
            EditorGUILayout.PropertyField(camAngleMaximumProperty);
            EditorGUILayout.PropertyField(camAngleMinimumProperty);
            bool changed = serializedObject.ApplyModifiedProperties();
        
            if (changed) {
                Debug.Log("Value Modifier changed");
                instance.ValueUpdateApply();
                changed = false;
            }
            GUILayout.EndVertical();
        }
    }
#endif
}