using UnityEngine;
using UnityMeshSimplifier;

public class AutoGenerateLODs : MonoBehaviour
{
    [SerializeField, Tooltip("The simplification options.")]
    private SimplificationOptions simplificationOptions = SimplificationOptions.Default;
    [SerializeField, Tooltip("If renderers should be automatically collected, otherwise they must be manually applied for each level.")]
    private bool autoCollectRenderers = true;
    [SerializeField, Tooltip("The LOD levels.")]
    private LODLevel[] levels = null;

    private void Start()
    {
        GenerateLODs();
    }

    private void Reset()
    {
        simplificationOptions = SimplificationOptions.Default;
        autoCollectRenderers = true;
        levels = new LODLevel[]
        {
            new LODLevel(0.5f, 1f)
            {
                CombineMeshes = false,
                CombineSubMeshes = false,
                SkinQuality = SkinQuality.Auto,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ReceiveShadows = true,
                SkinnedMotionVectors = true,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes,
            },
            new LODLevel(0.17f, 0.65f)
            {
                CombineMeshes = true,
                CombineSubMeshes = false,
                SkinQuality = SkinQuality.Auto,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ReceiveShadows = true,
                SkinnedMotionVectors = true,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Simple
            },
            new LODLevel(0.02f, 0.4225f)
            {
                CombineMeshes = true,
                CombineSubMeshes = true,
                SkinQuality = SkinQuality.Bone2,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                ReceiveShadows = false,
                SkinnedMotionVectors = false,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off
            }
        };
    }

    private void GenerateLODs()
    {
        LODGenerator.GenerateLODs(gameObject, levels, autoCollectRenderers, simplificationOptions);
    }
}