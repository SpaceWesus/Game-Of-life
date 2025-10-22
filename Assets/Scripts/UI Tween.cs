using System;
using System.Collections;
using UnityEngine;

//NOT MY CODE - FOUND HERE, WILL MAKE EDITS SOON (https://www.youtube.com/watch?v=AaudFyM3KV0&t)

public class UI_Tween : MonoBehaviour
    {
        [Header("Window Setup")]
        [SerializeField] private GameObject window;

        [SerializeField] private RectTransform windowRectTransform;
        [SerializeField] private CanvasGroup windowCanvasGroup;

        public enum AnimateToDirection
        {
            Top,
            Bottom,
            Left,
            Right
        }

        [Header("Animation Setup")]
        [SerializeField] private AnimateToDirection openDirection = AnimateToDirection.Top;
        [SerializeField] private AnimateToDirection closeDirection = AnimateToDirection.Bottom;
        [Space]
        [SerializeField] private Vector2 distanceToAnimate = new Vector2(100, 100);
        [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Range(0, 1f)] [SerializeField] private float animationDuration = 0.5f;

        private bool _isOpen;
        private Vector2 _initialPosition;
        private Vector2 _currentPosition;

        private Vector2 _upOffset;
        private Vector2 _downOffset;
        private Vector2 _leftOffset;
        private Vector2 _rightOffset;

        private Coroutine _animateWindowCoroutine;

        [Header("Helpers")]
        [SerializeField] private bool displayGizmos = true;

        public static event Action OnOpenWindow;
        public static event Action OnCloseWindow;

        private enum DisplayGizmosAtLocation
        {
            Open,
            Close,
            Both,
            Situational,
            None
        }

        [SerializeField] private DisplayGizmosAtLocation gizmoHandler;
        [SerializeField] private Color gizmoOpenColor = Color.green;
        [SerializeField] private Color gizmoCloseColor = Color.red;
        [SerializeField] private Color gizmoInitalLocationColor = Color.grey;
        private Vector2 _windowOpenPositionForGizmos;
        private Vector2 _windowClosePositionForGizmos;
        private Vector2 _initialPositionForGizmos;



        private void OnValidate()
        {
            if (window != null)
            {
                windowRectTransform = window.GetComponent<RectTransform>();
                windowCanvasGroup = window.GetComponent<CanvasGroup>();
            }

            distanceToAnimate.x = Mathf.Max(0, distanceToAnimate.x);
            distanceToAnimate.y = Mathf.Max(0, distanceToAnimate.y);


            _initialPosition = window.transform.position;

            RecalculateGizmoPositions();
        }

        #region AnimationFunctionality

        private void Start()
        {
            _initialPosition = window.transform.position;

            InitializeOffsetPositions();

            windowCanvasGroup.alpha = 0;
            windowCanvasGroup.interactable = false;
            windowCanvasGroup.blocksRaycasts = false;
            _isOpen = false;
    }

        private void InitializeOffsetPositions()
        {
            _upOffset = new Vector2(0, distanceToAnimate.y);
            _downOffset = new Vector2(0, -distanceToAnimate.y);

            _rightOffset = new Vector2(+distanceToAnimate.x, 0);
            _leftOffset = new Vector2(-distanceToAnimate.x, 0);
        }

        [ContextMenu("Toggle Open Close")]
        
        public void ToggleOpenClose()
        {
            if (_isOpen)
                CloseWindow();
            else
                OpenWindow();
        }

        [ContextMenu("Open Window")]
      
        public void OpenWindow()
        {
            if (_isOpen)
                return;

            _isOpen = true;
            OnOpenWindow?.Invoke();

            if (_animateWindowCoroutine != null)
                StopCoroutine(_animateWindowCoroutine);

            _animateWindowCoroutine = StartCoroutine(AnimateWindow(true));
        }

        [ContextMenu("Close Window")]
        
        public void CloseWindow()
        {
            if (!_isOpen)
                return;

            _isOpen = false;
            OnCloseWindow?.Invoke();

            if (_animateWindowCoroutine != null)
                StopCoroutine(_animateWindowCoroutine);

            _animateWindowCoroutine = StartCoroutine(AnimateWindow(false));
        }

        private Vector2 GetOffset(AnimateToDirection direction)
        {
            switch (direction)
            {
                case AnimateToDirection.Top:
                    return _upOffset;
                case AnimateToDirection.Bottom:
                    return _downOffset;
                case AnimateToDirection.Left:
                    return _leftOffset;
                case AnimateToDirection.Right:
                    return _rightOffset;
                default:
                    return Vector3.zero;
            }
        }

        private IEnumerator AnimateWindow(bool open)
        {
            if (open)
                window.gameObject.SetActive(true);

            _currentPosition = window.transform.position;

            float elapsedTime = 0;

            Vector2 targetPosition = _currentPosition;

            if (open)
                targetPosition = _currentPosition + GetOffset(openDirection);
            else
                targetPosition = _currentPosition + GetOffset(closeDirection);

            while (elapsedTime < animationDuration)
            {
                float evaluationAtTime = easingCurve.Evaluate(elapsedTime / animationDuration);

                window.transform.position = Vector2.Lerp(_currentPosition, targetPosition, evaluationAtTime);

                windowCanvasGroup.alpha = open
                    ? Mathf.Lerp(0f, 1f, evaluationAtTime)
                    : Mathf.Lerp(1f, 0f, evaluationAtTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            window.transform.position = targetPosition;

            windowCanvasGroup.alpha = open ? 1 : 0;
            windowCanvasGroup.interactable = open;
            windowCanvasGroup.blocksRaycasts = open;

            //if (!open)
            //{
              //  window.gameObject.SetActive(false);
                //window.transform.position = _initialPosition;
            //}
        }

        #endregion

        #region Visualisation
        
        private void Refresh()
        {
            OnValidate();
        }

        private void RecalculateGizmoPositions()
        {
            InitializeOffsetPositions();

            _initialPositionForGizmos = new Vector2(window.transform.position.x, window.transform.position.y) + windowRectTransform.rect.center;
            _windowOpenPositionForGizmos = _initialPositionForGizmos + GetOffset(openDirection);
            _windowClosePositionForGizmos = _windowOpenPositionForGizmos + GetOffset(closeDirection);
        }


        private void OnDrawGizmosSelected()
        {
            if (!displayGizmos)
                return;

            if (window == null)
                return;

            if (windowRectTransform == null)
                return;

            Gizmos.color = gizmoInitalLocationColor;
            Gizmos.DrawWireCube(_initialPositionForGizmos, windowRectTransform.sizeDelta);

            switch (gizmoHandler)
            {
                case DisplayGizmosAtLocation.Open:
                    DrawCube(_windowOpenPositionForGizmos, true);
                    break;

                case DisplayGizmosAtLocation.Close:
                    DrawCube(_windowClosePositionForGizmos, false);
                    break;

                case DisplayGizmosAtLocation.Both:
                    DrawCube(_windowClosePositionForGizmos, false);
                    DrawCube(_windowOpenPositionForGizmos, true);
                    break;

                case DisplayGizmosAtLocation.Situational:
                    if (_isOpen)
                        DrawCube(_windowClosePositionForGizmos, true);
                    else
                        DrawCube(_windowOpenPositionForGizmos, false);
                    break;

                default:
                case DisplayGizmosAtLocation.None:
                    break;
            }

            if (gizmoHandler != DisplayGizmosAtLocation.None)
                DrawIndicators();
        }

        private void DrawCube(Vector2 windowPosition, bool opens)
        {
            Gizmos.color = opens ? gizmoOpenColor : gizmoCloseColor;
            Gizmos.DrawWireCube(windowPosition, windowRectTransform.sizeDelta);
        }

        private void DrawIndicators()
        {
            Gizmos.color = gizmoOpenColor;
            Gizmos.DrawLine(_initialPositionForGizmos, _windowOpenPositionForGizmos);

            Gizmos.color = gizmoCloseColor;
            Gizmos.DrawLine(_windowOpenPositionForGizmos, _windowClosePositionForGizmos);
        }


        #endregion
    }