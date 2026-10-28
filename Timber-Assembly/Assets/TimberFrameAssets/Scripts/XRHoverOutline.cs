using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class XRHoverOutline : MonoBehaviour
{
    private class RendererOutlineState
    {
        public MeshRenderer OverlayRenderer;
        public MaterialPropertyBlock PropertyBlock;
        public readonly HashSet<XRHoverOutline> HoverRequesters = new HashSet<XRHoverOutline>();
        public readonly HashSet<XRHoverOutline> SelectedRequesters = new HashSet<XRHoverOutline>();
    }

    private static readonly Dictionary<MeshRenderer, RendererOutlineState> RendererStates =
        new Dictionary<MeshRenderer, RendererOutlineState>();

    private static readonly int OutlineColorProp = Shader.PropertyToID("_OutlineColor");

    [Header("Outline Setup")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private MeshRenderer[] targetRenderers;

    [Header("State Colors")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1.0f, 0.65f, 0.0f, 1.0f); // Default Orange/Gold

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            if (transform.parent != null)
            {
                targetRenderers = transform.parent.GetComponentsInChildren<MeshRenderer>();
            }

            if (targetRenderers == null || targetRenderers.Length == 0)
            {
                targetRenderers = GetComponentsInParent<MeshRenderer>();
            }
        }

        RegisterRenderers();
    }

    private void RegisterRenderers()
    {
        if (targetRenderers == null || outlineMaterial == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            MeshRenderer target = targetRenderers[i];
            if (target == null || target.gameObject.name == "__OutlineOverlay") continue;

            if (!RendererStates.TryGetValue(target, out RendererOutlineState state))
            {
                MeshFilter targetFilter = target.GetComponent<MeshFilter>();
                if (targetFilter == null || targetFilter.sharedMesh == null) continue;

                GameObject overlayObj = new GameObject("__OutlineOverlay");
                overlayObj.layer = target.gameObject.layer;
                overlayObj.transform.SetParent(target.transform, false);

                MeshFilter overlayFilter = overlayObj.AddComponent<MeshFilter>();
                overlayFilter.sharedMesh = targetFilter.sharedMesh;

                MeshRenderer overlayRenderer = overlayObj.AddComponent<MeshRenderer>();
                overlayRenderer.shadowCastingMode = ShadowCastingMode.Off;
                overlayRenderer.receiveShadows = false;
                overlayRenderer.lightProbeUsage = LightProbeUsage.Off;
                overlayRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

                int subMeshCount = targetFilter.sharedMesh.subMeshCount;
                Material[] overlayMats = new Material[subMeshCount];
                for (int s = 0; s < subMeshCount; s++)
                {
                    overlayMats[s] = outlineMaterial;
                }
                overlayRenderer.sharedMaterials = overlayMats;
                overlayRenderer.enabled = false;

                state = new RendererOutlineState
                {
                    OverlayRenderer = overlayRenderer,
                    PropertyBlock = new MaterialPropertyBlock()
                };

                RendererStates[target] = state;
            }
        }
    }

    private void OnEnable()
    {
        interactable.firstHoverEntered.AddListener(OnFirstHoverEntered);
        interactable.lastHoverExited.AddListener(OnLastHoverExited);
    }

    private void OnDisable()
    {
        interactable.firstHoverEntered.RemoveListener(OnFirstHoverEntered);
        interactable.lastHoverExited.RemoveListener(OnLastHoverExited);
        SetHoverOutline(false);
        SetSelectedOutline(false);
    }

    private void OnDestroy()
    {
        if (targetRenderers == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            MeshRenderer renderer = targetRenderers[i];
            if (renderer != null && RendererStates.ContainsKey(renderer))
            {
                RendererStates.Remove(renderer);
            }
        }
    }

    private void OnFirstHoverEntered(HoverEnterEventArgs args) => SetHoverOutline(true);
    private void OnLastHoverExited(HoverExitEventArgs args) => SetHoverOutline(false);

    public void SetHoverOutline(bool active)
    {
        if (targetRenderers == null || outlineMaterial == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            MeshRenderer renderer = targetRenderers[i];
            if (renderer == null || renderer.gameObject.name == "__OutlineOverlay") continue;

            if (RendererStates.TryGetValue(renderer, out RendererOutlineState state))
            {
                if (active) state.HoverRequesters.Add(this);
                else state.HoverRequesters.Remove(this);

                EvaluateRendererState(state);
            }
        }
    }

    public void SetSelectedOutline(bool active)
    {
        if (targetRenderers == null || outlineMaterial == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            MeshRenderer renderer = targetRenderers[i];
            if (renderer == null || renderer.gameObject.name == "__OutlineOverlay") continue;

            if (RendererStates.TryGetValue(renderer, out RendererOutlineState state))
            {
                if (active) state.SelectedRequesters.Add(this);
                else state.SelectedRequesters.Remove(this);

                EvaluateRendererState(state);
            }
        }
    }

    private void EvaluateRendererState(RendererOutlineState state)
    {
        if (state.OverlayRenderer == null) return;

        bool isSelected = state.SelectedRequesters.Count > 0;
        bool isHovered = state.HoverRequesters.Count > 0;

        if (!isSelected && !isHovered)
        {
            state.OverlayRenderer.enabled = false;
            return;
        }

        // Selection state takes visual precedence over hover state
        Color targetColor = isSelected ? selectedColor : hoverColor;

        state.OverlayRenderer.GetPropertyBlock(state.PropertyBlock);
        state.PropertyBlock.SetColor(OutlineColorProp, targetColor);
        state.OverlayRenderer.SetPropertyBlock(state.PropertyBlock);

        state.OverlayRenderer.enabled = true;
    }
}