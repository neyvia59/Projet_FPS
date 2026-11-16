
using System.Runtime.InteropServices;
using UnityEngine;

public class FogDensityController : MonoBehaviour
{
    private const int _maxLightSource = 1000000;

    [StructLayout(LayoutKind.Sequential)]
    private struct FogLightData
    {
        public Vector4 PositionRadius;
        public Vector4 Parameters;
    }

    [Header("Compute Shader")]
    public ComputeShader FogComputeShader;

    [Header("Fog Volume")]
    public BoxCollider FogVolume;

    [Header("Fog Settings")]
    [Range(8, 128)]
    public int Resolution = 32;

    [Range(0f, 1f)]
    public float BaseDensity = 1f;

    [Min(0f)]
    public float ReturnSpeed = 0.15f;

    private RenderTexture _densityTexture;
    private ComputeBuffer _lightBuffer;
    private FogLightData[] _lightData;

    private int _initializeKernel;
    private int _updateKernel;

    private int _currentLightCount;
    private int _lastReportedLightCount = -1;

    private void Awake()
    {
        GetComponentInChildren<GameObject>().SetActive(true);
        FogVolume = GetComponentInChildren<BoxCollider>();
    }

    private void Start()
    {
        InitializeDensityTexture();
        InitializeLightBuffer();
    }

    private void InitializeDensityTexture()
    {

        _initializeKernel =
            FogComputeShader.FindKernel("InitializeFog");

        _updateKernel =
            FogComputeShader.FindKernel("UpdateFog");

        _densityTexture = new RenderTexture(
            Resolution,
            Resolution,
            0,
            RenderTextureFormat.RFloat
        );

        _densityTexture.dimension =
            UnityEngine.Rendering.TextureDimension.Tex3D;

        _densityTexture.volumeDepth = Resolution;
        _densityTexture.enableRandomWrite = true;
        _densityTexture.filterMode = FilterMode.Bilinear;
        _densityTexture.wrapMode = TextureWrapMode.Clamp;

        _densityTexture.Create();

        Shader.SetGlobalTexture("_FogDensityTexture", _densityTexture);

        int groups = Mathf.CeilToInt(Resolution / 4f);

        FogComputeShader.SetInt("_Resolution", Resolution);
        FogComputeShader.SetFloat("_BaseDensity", BaseDensity);

        FogComputeShader.SetTexture(
            _initializeKernel,
            "_DensityTexture",
            _densityTexture
        );

        FogComputeShader.Dispatch(
            _initializeKernel,
            groups,
            groups,
            groups
        );

        FogComputeShader.SetTexture(
            _updateKernel,
            "_DensityTexture",
            _densityTexture
        );

    }

    private void InitializeLightBuffer()
    {
        if (FogComputeShader == null || _densityTexture == null)
            return;

        _lightData = new FogLightData[_maxLightSource];

        _lightBuffer = new ComputeBuffer(
            _maxLightSource,
            32,
            ComputeBufferType.Structured
        );

        FogComputeShader.SetBuffer(
            _updateKernel,
            "_Lights",
            _lightBuffer
        );

    }

    private void Update()
    {
        if (_densityTexture == null || _lightBuffer == null)
            return;

        UpdateLightBuffer();
        UpdateFogDensity();
    }

    private void UpdateLightBuffer()
    {
        int activeCount = 0;

        foreach (FogLightSource source in FogLightSource.ActiveSources)
        {
            if (source == null ||
                !source.isActive ||
                !source.isActiveAndEnabled)
                continue;

            if (activeCount >= _maxLightSource)
                break;

            Vector3 position = source.Position;

            _lightData[activeCount].PositionRadius = new Vector4(
                position.x,
                position.y,
                position.z,
                source.clearingRadius
            );

            _lightData[activeCount].Parameters = new Vector4(
                source.clearingStrength,
                0f,
                0f,
                0f
            );

            activeCount++;
        }

        _currentLightCount = activeCount;

        _lightBuffer.SetData(_lightData);

        if (_currentLightCount != _lastReportedLightCount)
        {

            _lastReportedLightCount = _currentLightCount;
        }
    }

    private void UpdateFogDensity()
    {
        FogComputeShader.SetInt("_Resolution", Resolution);
        FogComputeShader.SetInt("_LightCount", _currentLightCount);

        FogComputeShader.SetFloat("_BaseDensity", BaseDensity);
        FogComputeShader.SetFloat("_DeltaTime", Time.deltaTime);
        FogComputeShader.SetFloat("_ReturnSpeed", ReturnSpeed);

        FogComputeShader.SetVector(
            "_VolumeCenter",
            FogVolume.center
        );

        FogComputeShader.SetVector(
            "_VolumeSize",
            FogVolume.size
        );

        FogComputeShader.SetMatrix(
            "_LocalToWorld",
            FogVolume.transform.localToWorldMatrix
        );

        Shader.SetGlobalMatrix(
            "_FogWorldToLocal",
            FogVolume.transform.worldToLocalMatrix
        );

        Shader.SetGlobalVector(
            "_FogVolumeCenter",
            FogVolume.center
        );

        Shader.SetGlobalVector(
            "_FogVolumeSize",
            FogVolume.size
        );

        int groups = Mathf.CeilToInt(Resolution / 4f);

        FogComputeShader.Dispatch(
            _updateKernel,
            groups,
            groups,
            groups
        );
    }

    public Vector3 WorldToFogUV(Vector3 worldPosition)
    {
        if (FogVolume == null)
            return Vector3.zero;

        Vector3 localPosition =
            FogVolume.transform.InverseTransformPoint(worldPosition);

        Vector3 relativePosition =
            localPosition - FogVolume.center;

        Vector3 size = FogVolume.size;

        return new Vector3(
            relativePosition.x / size.x + 0.5f,
            relativePosition.y / size.y + 0.5f,
            relativePosition.z / size.z + 0.5f
        );
    }

    public bool IsInsideFogVolume(Vector3 worldPosition)
    {
        Vector3 uv = WorldToFogUV(worldPosition);

        return uv.x >= 0f && uv.x <= 1f &&
               uv.y >= 0f && uv.y <= 1f &&
               uv.z >= 0f && uv.z <= 1f;
    }

    private void OnDestroy()
    {
        if (_lightBuffer != null)
        {
            _lightBuffer.Release();
            _lightBuffer = null;
        }

        if (_densityTexture != null)
        {
            _densityTexture.Release();
            Destroy(_densityTexture);
            _densityTexture = null;
        }
    }
}
