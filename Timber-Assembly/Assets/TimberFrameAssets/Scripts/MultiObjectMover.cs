using System;
using UnityEngine;

public class MultiObjectSmoothMover : MonoBehaviour
{
    [Serializable]
    public struct MoveTarget
    {
        public Transform targetTransform;
        public Vector3 positionA; // Assembled localPosition
        public Vector3 positionB; // Exploded localPosition

        // Cached LineRenderer references from XRInspectableObject.ExplodeLineRenderers
        public LineRenderer[] lineRenderers;

        // Precomputed local-space terminal vector for each LineRenderer at full explosion (t = 1)
        public Vector3[] fullExplosionLineEndPoints;
    }

    [SerializeField] private float duration = 0.5f;

    private MoveTarget[] _targets = Array.Empty<MoveTarget>();
    private float _progress = 0f;
    private float _targetProgress = 0f;
    private Action _onTransitionComplete;

    public float Duration
    {
        get => duration;
        set => duration = Mathf.Max(0.001f, value);
    }

    public bool IsAtStateB => Mathf.Approximately(_progress, 1f);
    public bool IsAtStateA => Mathf.Approximately(_progress, 0f);
    public bool IsMoving => !Mathf.Approximately(_progress, _targetProgress);

    private void Awake()
    {
        enabled = false;
    }

    /// <summary>
    /// Configures the mover targets and precalculates each LineRenderer's local trail vector.
    /// </summary>
    public void ConfigureTargets(MoveTarget[] configuredTargets, bool resetProgress = true)
    {
        _targets = configuredTargets ?? Array.Empty<MoveTarget>();

        for (int i = 0; i < _targets.Length; i++)
        {
            LineRenderer[] lines = _targets[i].lineRenderers;
            if (lines == null) continue;

            for (int j = 0; j < lines.Length; j++)
            {
                if (lines[j] == null) continue;
                lines[j].useWorldSpace = false;
                lines[j].positionCount = 2;
                lines[j].SetPosition(0, Vector3.zero);
                lines[j].SetPosition(1, Vector3.zero);
                lines[j].enabled = false;
            }
        }

        if (resetProgress)
        {
            _progress = 0f;
            _targetProgress = 0f;
            enabled = false;
        }
    }

    /// <summary>
    /// Smoothly transitions all configured objects and their LineRenderer trails toward State B (true) or State A (false).
    /// </summary>
    public void TransitionToStateB(bool moveToB, Action onComplete = null)
    {
        _targetProgress = moveToB ? 1f : 0f;
        _onTransitionComplete = onComplete;

        if (moveToB)
        {
            SetLineRenderersEnabled(true);
        }

        enabled = true;
    }

    public void ToggleState(Action onComplete = null)
    {
        TransitionToStateB(_targetProgress < 0.5f, onComplete);
    }

    private void Update()
    {
        float step = (duration > 0f) ? (Time.deltaTime / duration) : 1f;
        _progress = Mathf.MoveTowards(_progress, _targetProgress, step);

        float smoothedT = Mathf.SmoothStep(0f, 1f, _progress);

        for (int i = 0; i < _targets.Length; i++)
        {
            ref MoveTarget target = ref _targets[i];
            if (target.targetTransform == null) continue;

            // 1. Interpolate part local position
            target.targetTransform.localPosition = Vector3.LerpUnclamped(
                target.positionA,
                target.positionB,
                smoothedT
            );

            // 2. Grow or shrink each attached LineRenderer trail proportionally
            if (target.lineRenderers != null && target.fullExplosionLineEndPoints != null)
            {
                for (int j = 0; j < target.lineRenderers.Length; j++)
                {
                    LineRenderer lr = target.lineRenderers[j];
                    if (lr != null)
                    {
                        lr.SetPosition(1, target.fullExplosionLineEndPoints[j] * smoothedT);
                    }
                }
            }
        }

        // Disable Update loop when transition completes
        if (Mathf.Approximately(_progress, _targetProgress))
        {
            if (Mathf.Approximately(_progress, 0f))
            {
                SetLineRenderersEnabled(false);
            }

            enabled = false;
            Action callback = _onTransitionComplete;
            _onTransitionComplete = null;
            callback?.Invoke();
        }
    }

    private void SetLineRenderersEnabled(bool state)
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            LineRenderer[] lines = _targets[i].lineRenderers;
            if (lines == null) continue;

            for (int j = 0; j < lines.Length; j++)
            {
                if (lines[j] != null)
                {
                    lines[j].enabled = state;
                }
            }
        }
    }
}
