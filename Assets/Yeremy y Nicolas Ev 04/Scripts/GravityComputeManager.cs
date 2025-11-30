using UnityEngine;
using System.Collections.Generic;

public class GravityComputeManager : MonoBehaviour
{
    public static GravityComputeManager Instance;

    public ComputeShader gravityComputeShader;
    public float globalGravity = 9.8f;

    private ComputeBuffer _bodiesBuffer;
    private ComputeBuffer _planetsBuffer;

    private List<PhysicsBodyData> _bodiesData = new List<PhysicsBodyData>();
    private List<PlanetData> _planetsData = new List<PlanetData>();

    private List<GameRigidbody2D> _cachedBodies = new List<GameRigidbody2D>();

    // ------------------------
    // STRUCTS PARA GPU
    // ------------------------
    private struct PhysicsBodyData
    {
        public Vector2 position;
        public Vector2 velocity;
        public float mass;
        public int useGravity;
    }

    private struct PlanetData
    {
        public Vector2 position;
        public float mass;
    }

    // ------------------------
    // UNITY
    // ------------------------
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CacheInitialBodies();
        InitializeBuffers();
    }

    void FixedUpdate()
    {
        UpdateBodyDataFromScene();
        DispatchComputeShader();
        ApplyResultsToScene();
    }

    private void OnDestroy()
    {
        _bodiesBuffer?.Release();
        _planetsBuffer?.Release();
    }

    // ------------------------
    // CACHE INICIAL
    // ------------------------
    void CacheInitialBodies()
    {
        GameRigidbody2D[] found = FindObjectsByType<GameRigidbody2D>(FindObjectsSortMode.None);

        foreach (var rb in found)
            _cachedBodies.Add(rb);

        UpdatePlanetData();
    }

    // ------------------------
    // PARA AGREGAR CUERPOS EN RUNTIME (EJ: BALAS)
    // ------------------------
    public void RegisterBody(GameRigidbody2D body)
    {
        if (!_cachedBodies.Contains(body))
            _cachedBodies.Add(body);
    }

    // ------------------------
    // BUFFERS GPU
    // ------------------------
    void InitializeBuffers()
    {
        if (_bodiesData.Count > 0)
        {
            _bodiesBuffer = new ComputeBuffer(_bodiesData.Count, sizeof(float) * 5 + sizeof(int));
            _bodiesBuffer.SetData(_bodiesData);
        }

        if (_planetsData.Count > 0)
        {
            _planetsBuffer = new ComputeBuffer(_planetsData.Count, sizeof(float) * 3);
            _planetsBuffer.SetData(_planetsData);
        }
    }

    // ------------------------
    // CPU → GPU SYNC
    // ------------------------
    void UpdateBodyDataFromScene()
    {
        _bodiesData.Clear();

        foreach (GameRigidbody2D body in _cachedBodies)
        {
            if (body != null)
            {
                PhysicsBodyData data = new PhysicsBodyData
                {
                    position = body.position,
                    velocity = body.velocity,
                    mass = body.mass,
                    useGravity = body.useGravity ? 1 : 0
                };

                _bodiesData.Add(data);
            }
        }
    }

    void UpdatePlanetData()
    {
        _planetsData.Clear();

        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

        foreach (GameObject planet in planets)
        {
            GameRigidbody2D rb = planet.GetComponent<GameRigidbody2D>();

            if (rb != null)
            {
                PlanetData data = new PlanetData
                {
                    position = rb.position,
                    mass = rb.mass
                };

                _planetsData.Add(data);
            }
        }
    }

    // ------------------------
    // GPU COMPUTE
    // ------------------------
    void DispatchComputeShader()
    {
        if (_bodiesData.Count == 0 || _planetsData.Count == 0)
            return;

        // Recrear buffer si cambió tamaño
        if (_bodiesBuffer == null || _bodiesBuffer.count != _bodiesData.Count)
        {
            _bodiesBuffer?.Release();
            _bodiesBuffer = new ComputeBuffer(_bodiesData.Count, sizeof(float) * 5 + sizeof(int));
        }

        _bodiesBuffer.SetData(_bodiesData);

        if (_planetsBuffer == null || _planetsBuffer.count != _planetsData.Count)
        {
            _planetsBuffer?.Release();
            _planetsBuffer = new ComputeBuffer(_planetsData.Count, sizeof(float) * 3);
        }

        _planetsBuffer.SetData(_planetsData);

        int kernel = gravityComputeShader.FindKernel("CSMain");

        gravityComputeShader.SetBuffer(kernel, "_Bodies", _bodiesBuffer);
        gravityComputeShader.SetBuffer(kernel, "_Planets", _planetsBuffer);

        gravityComputeShader.SetFloat("_GlobalGravity", globalGravity);
        gravityComputeShader.SetInt("_BodyCount", _bodiesData.Count);
        gravityComputeShader.SetInt("_PlanetCount", _planetsData.Count);
        gravityComputeShader.SetFloat("_DeltaTime", Time.fixedDeltaTime);

        int threadGroups = Mathf.CeilToInt(_bodiesData.Count / 64f);
        gravityComputeShader.Dispatch(kernel, threadGroups, 1, 1);
    }

    // ------------------------
    // GPU → CPU SYNC
    // ------------------------
    void ApplyResultsToScene()
    {
        if (_bodiesData.Count == 0 || _bodiesBuffer == null)
            return;

        PhysicsBodyData[] results = new PhysicsBodyData[_bodiesData.Count];
        _bodiesBuffer.GetData(results);

        for (int i = 0; i < _cachedBodies.Count && i < results.Length; i++)
        {
            if (_cachedBodies[i] != null)
            {
                _cachedBodies[i].velocity = results[i].velocity;

                // 🔥 CORRECCIÓN FINAL → uso directo del método
                _cachedBodies[i].ApplyPosition(results[i].position);
            }
        }
    }
}
