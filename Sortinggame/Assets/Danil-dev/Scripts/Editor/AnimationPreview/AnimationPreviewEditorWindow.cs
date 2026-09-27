using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace D_Dev.AnimatorView.Editor
{
    public class AnimationPreviewEditorWindow : EditorWindow
    {
        #region Fields

        private GameObject _targetObject;
        private AnimationClip _previewClip;
        private float _normalizedTime = 0f;
        private bool _isPreviewActive = false;
        private bool _autoPlay = false;
        private float _playbackSpeed = 1f;
        
        private PlayableGraph _previewGraph;
        private AnimationClipPlayable _clipPlayable;
        private AnimationPlayableOutput _playableOutput;
        private Animator _animator;
        
        private Vector2 _scrollPosition;

        #endregion

        #region Menu

        [MenuItem("Tools/D_Dev/Utility/Animation Preview")]
        public static void ShowWindow()
        {
            GetWindow<AnimationPreviewEditorWindow>("Animation Preview");
        }

        #endregion

        #region EditorWindow

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            CleanupPreview();
        }

        private void OnDestroy()
        {
            CleanupPreview();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Animation Preview", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            DrawTargetSelection();
            EditorGUILayout.Space(10);
            
            DrawClipSelection();
            EditorGUILayout.Space(10);
            
            if (_previewClip != null && _targetObject != null)
            {
                DrawPlaybackControls();
                EditorGUILayout.Space(10);
                DrawTimeSlider();
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Drawing

        private void DrawTargetSelection()
        {
            EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
            
            GameObject newTarget = EditorGUILayout.ObjectField(
                "Character", 
                _targetObject, 
                typeof(GameObject), 
                true
            ) as GameObject;

            if (newTarget != _targetObject)
            {
                CleanupPreview();
                _targetObject = newTarget;
                
                if (_targetObject != null)
                {
                    SetupTarget();
                }
            }

            if (_targetObject == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a GameObject with Animator component", 
                    MessageType.Info
                );
                return;
            }

            if (_animator == null)
            {
                EditorGUILayout.HelpBox(
                    "Selected object does not have an Animator component!", 
                    MessageType.Error
                );
            }
            else
            {
                EditorGUILayout.LabelField($"Animator: {_animator.name}", EditorStyles.miniLabel);
            }
        }

        private void DrawClipSelection()
        {
            EditorGUILayout.LabelField("Animation Clip", EditorStyles.boldLabel);
            
            AnimationClip newClip = EditorGUILayout.ObjectField(
                "Clip", 
                _previewClip, 
                typeof(AnimationClip), 
                false
            ) as AnimationClip;

            if (newClip != _previewClip)
            {
                _previewClip = newClip;
                _normalizedTime = 0f;
                
                if (_isPreviewActive)
                {
                    RecreatePlayable();
                }
            }

            if (_previewClip != null)
            {
                EditorGUILayout.LabelField($"Length: {_previewClip.length:F2} seconds", EditorStyles.miniLabel);
                EditorGUILayout.LabelField($"Frames: {_previewClip.length * _previewClip.frameRate:F0}", EditorStyles.miniLabel);
            }
        }

        private void DrawPlaybackControls()
        {
            EditorGUILayout.LabelField("Playback Controls", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            GUI.backgroundColor = _isPreviewActive ? Color.green : Color.gray;
            if (GUILayout.Button(_isPreviewActive ? "Stop Preview" : "Start Preview", GUILayout.Height(30)))
            {
                TogglePreview();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            _autoPlay = GUILayout.Toggle(_autoPlay, "Auto Play", GUILayout.Width(80));
            
            if (_autoPlay)
            {
                EditorGUILayout.LabelField("Speed:", GUILayout.Width(45));
                _playbackSpeed = EditorGUILayout.Slider(_playbackSpeed, -2f, 2f);
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (_autoPlay && _isPreviewActive)
            {
                EditorGUILayout.HelpBox(
                    "Animation is playing automatically. Disable Auto Play to scrub manually.", 
                    MessageType.Info
                );
            }
        }

        private void DrawTimeSlider()
        {
            EditorGUILayout.LabelField("Time Control (Normalized 0-1)", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("0", GUILayout.Width(20));
            float newTime = EditorGUILayout.Slider(_normalizedTime, 0f, 1f);
            EditorGUILayout.LabelField("1", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            
            if (EditorGUI.EndChangeCheck())
            {
                _normalizedTime = newTime;
                _autoPlay = false;
                
                if (_isPreviewActive)
                {
                    UpdatePlayableTime();
                }
            }

            float currentTime = _normalizedTime * (_previewClip?.length ?? 0);
            EditorGUILayout.LabelField(
                $"Time: {currentTime:F3}s / {_previewClip?.length:F3}s (Frame: {(int)(currentTime * (_previewClip?.frameRate ?? 1))})", 
                EditorStyles.centeredGreyMiniLabel
            );
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("|<"))
            {
                _normalizedTime = 0f;
                _autoPlay = false;
                if (_isPreviewActive) UpdatePlayableTime();
            }
            
            if (GUILayout.Button("<"))
            {
                _normalizedTime = Mathf.Max(0f, _normalizedTime - 0.01f);
                _autoPlay = false;
                if (_isPreviewActive) UpdatePlayableTime();
            }
            
            if (GUILayout.Button(">"))
            {
                _normalizedTime = Mathf.Min(1f, _normalizedTime + 0.01f);
                _autoPlay = false;
                if (_isPreviewActive) UpdatePlayableTime();
            }
            
            if (GUILayout.Button(">|"))
            {
                _normalizedTime = 1f;
                _autoPlay = false;
                if (_isPreviewActive) UpdatePlayableTime();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Preview Management

        private void SetupTarget()
        {
            if (_targetObject == null) return;
            
            _animator = _targetObject.GetComponent<Animator>();
        }

        private void TogglePreview()
        {
            if (_isPreviewActive)
            {
                CleanupPreview();
            }
            else
            {
                StartPreview();
            }
        }

        private void StartPreview()
        {
            if (_targetObject == null || _animator == null || _previewClip == null)
            {
                EditorUtility.DisplayDialog(
                    "Animation Preview", 
                    "Please select a target GameObject with Animator and an Animation Clip.", 
                    "OK"
                );
                return;
            }

            CreatePreviewGraph();
            _isPreviewActive = true;
        }

        private void CreatePreviewGraph()
        {
            CleanupPreview();

            string graphName = $"PreviewGraph_{_targetObject.name}_{_previewClip.name}";
            _previewGraph = PlayableGraph.Create(graphName);
            _previewGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            _clipPlayable = AnimationClipPlayable.Create(_previewGraph, _previewClip);
            _clipPlayable.SetApplyFootIK(false);
            _clipPlayable.SetSpeed(0);

            _playableOutput = AnimationPlayableOutput.Create(_previewGraph, "Animation", _animator);
            _playableOutput.SetSourcePlayable(_clipPlayable);

            UpdatePlayableTime();
        }

        private void RecreatePlayable()
        {
            CleanupPreview();
            CreatePreviewGraph();
        }

        private void CleanupPreview()
        {
            _isPreviewActive = false;

            if (_clipPlayable.IsValid())
            {
                _clipPlayable.Destroy();
            }

            if (_previewGraph.IsValid())
            {
                _previewGraph.Destroy();
            }

            _clipPlayable = new AnimationClipPlayable();
        }

        private void UpdatePlayableTime()
        {
            if (!_previewGraph.IsValid() || !_clipPlayable.IsValid()) return;

            float time = _normalizedTime * _previewClip.length;
            _clipPlayable.SetTime(time);
            _previewGraph.Evaluate(0);
            
            SceneView.RepaintAll();
        }

        private void OnEditorUpdate()
        {
            if (!_isPreviewActive || !_autoPlay || !_clipPlayable.IsValid()) return;

            float deltaTime = Time.deltaTime * _playbackSpeed;
            float newTime = _normalizedTime + (deltaTime / _previewClip.length);

            if (_previewClip.isLooping)
            {
                _normalizedTime = newTime % 1f;
                if (_normalizedTime < 0) _normalizedTime += 1f;
            }
            else
            {
                _normalizedTime = Mathf.Clamp01(newTime);
                if (_normalizedTime >= 1f || _normalizedTime <= 0f)
                {
                    _autoPlay = false;
                }
            }

            UpdatePlayableTime();
            Repaint();
        }

        #endregion
    }
}
