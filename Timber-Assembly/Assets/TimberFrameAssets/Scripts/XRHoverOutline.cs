using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Omit .Interactables if using XRI 2.x

[RequireComponent(typeof(XRBaseInteractable))]
public class XRHoverOutline : MonoBehaviour
{
    private class RendererOutlineState
    {
        public Material[] BaseMaterials; // Cache of object's base materials
        public Material[] HoverMaterials; // Cache of object's base materials + [outlineMaterial]
        public readonly HashSet<XRHoverOutline> ActiveRequesters = new HashSet<XRHoverOutline>();
    }

    // Tracks active "outline requesters" per MeshRenderer across all XRHoverOutline instances
    // Used to ensure two colliders can highlight the same mesh without flickering or disappearance
    // when moving between them.
    private static readonly Dictionary<MeshRenderer, RendererOutlineState> RendererStates =
        new Dictionary<MeshRenderer, RendererOutlineState>();

    [SerializeField] private Material outlineMaterial;
    [Tooltip("Assign sibling MeshRenderers manually, or leave empty to auto-detect all MeshRenderers under the same parent.")]
    [SerializeField] private MeshRenderer[] targetRenderers;

    private XRBaseInteractable interactable;

    // Initialize
    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        // If not given any targetRenderers, automatically set targetRenderers
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            // If this object has a parent, get all MeshRenderers descended from it.
            // Otherwise, get all MeshRenderers descended from this object
            targetRenderers = transform.parent != null
                ? transform.parent.GetComponentsInChildren<MeshRenderer>()
                : GetComponentsInChildren<MeshRenderer>();
        }

        RegisterRenderers();
    }

    /// <summary>
    /// Initializes and caches the base and outline material arrays for each target renderer,
    /// storing them in a tracking dictionary so they can be easily toggled for selection or hover effects.
    /// </summary>
    private void RegisterRenderers()
    {
        // Guard Clauses: targetRenderers set (manually or automatically) && outlineMaterial set
        if (targetRenderers == null || outlineMaterial == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            MeshRenderer renderer = targetRenderers[i];
            if (renderer == null) continue; // Guard Clause: Skip missing or destroyed renderers

            // If this renderer hasn't been registered yet, cache its materials
            if (!RendererStates.TryGetValue(renderer, out RendererOutlineState state))
            {
                Material[] baseMats = renderer.sharedMaterials;
                Material[] hoverMats = new Material[baseMats.Length + 1];
                baseMats.CopyTo(hoverMats, 0);
                hoverMats[hoverMats.Length - 1] = outlineMaterial;

                state = new RendererOutlineState
                {
                    BaseMaterials = baseMats,
                    HoverMaterials = hoverMats
                };

                RendererStates[renderer] = state;
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

        ReleaseOutline();
    }

    private void OnDestroy()
    {
        // Clean up static dictionary entries if the target renderers are destroyed with the scene/prefab
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

    private void OnFirstHoverEntered(HoverEnterEventArgs args)
    {
        RequestOutline();
    }

    private void OnLastHoverExited(HoverExitEventArgs args)
    {
        ReleaseOutline();
    }

    public void RequestOutline()
    {
        if (targetRenderers == null || outlineMaterial == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            MeshRenderer renderer = targetRenderers[i];
            if (renderer == null) continue;

            if (!RendererStates.TryGetValue(renderer, out RendererOutlineState state))
            {
                RegisterRenderers();
                state = RendererStates[renderer];
            }

            bool wasEmpty = state.ActiveRequesters.Count == 0;
            state.ActiveRequesters.Add(this);

            if (wasEmpty && state.ActiveRequesters.Count > 0)
            {
                renderer.sharedMaterials = state.HoverMaterials;
            }
        }
    }

    public void ReleaseOutline()
    {
        if (targetRenderers == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            MeshRenderer renderer = targetRenderers[i];
            if (renderer == null) continue;

            if (RendererStates.TryGetValue(renderer, out RendererOutlineState state))
            {
                if (state.ActiveRequesters.Remove(this) && state.ActiveRequesters.Count == 0)
                {
                    renderer.sharedMaterials = state.BaseMaterials;
                }
            }
        }
    }
}