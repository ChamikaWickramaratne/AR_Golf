using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    public static PathVisualizer Instance { get; private set; }

    [Header("Line Settings")]  
    [SerializeField] private LineRenderer line;
    [Range(2,50)] public int samplesPerSegment = 10;

    [Header("Side Colliders")]
    [Tooltip("Prefab: thin cube with BoxCollider, no MeshRenderer")]
    [SerializeField] private GameObject sideColliderPrefab;
    [Tooltip("Thickness of each side collider (meters)")]
    [SerializeField] private float colliderThickness = 0.02f;
    [Tooltip("Height of each side collider (meters)")]
    [SerializeField] private float colliderHeight = 0.1f;

    [Header("Floor & Cap Colliders")]
    [Tooltip("Prefab: thin cube with BoxCollider, no MeshRenderer")]
    [SerializeField] private GameObject floorColliderPrefab;
    [Tooltip("Height of the floor collider (meters)")]
    [SerializeField] private float floorThickness = 0.02f;
    [Tooltip("Depth of end cap collider along path direction (meters)")]
    [SerializeField] private float capThickness = 0.02f;
    [Tooltip("Height of end cap collider (meters)")]
    [SerializeField] private float capHeight = 0.1f;

    [Tooltip("Prefab for the hole; should include a trigger collider")]
    [SerializeField] private GameObject holePrefab;
    [Tooltip("Vertical offset down from the end point for the hole (meters)")]
    [SerializeField] private float holeDepthOffset = 0.05f;

    [Tooltip("How far forward along the last segment to place the hole")]   
    [SerializeField] private float holeForwardOffset = 0.1f;
    [Tooltip("How far to the right (relative to path direction) to shift the hole (meters)")]   
    [SerializeField] private float holeSideOffset = -0.1f;

    private MeshCollider _meshCollider;
    private Mesh _bakedMesh;
    private List<GameObject> _sideColliders = new List<GameObject>();
    private List<GameObject> _floorColliders = new List<GameObject>();
    private float? groundY = null;
    private GameObject _spawnedHole;

    void Awake()
    {
        transform.SetParent(null, true);
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (line == null) line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.enabled = false;
        line.positionCount = 0;

        _meshCollider = GetComponent<MeshCollider>() ?? gameObject.AddComponent<MeshCollider>();
        _bakedMesh = new Mesh();
    }

    public void SetControlPoints(List<Vector3> pts)
    {
        if (_spawnedHole != null)
        {
            Destroy(_spawnedHole);
            _spawnedHole = null;
        }
        if (pts == null || pts.Count < 2)
        {
            line.enabled = false;
            line.positionCount = 0;
            _meshCollider.sharedMesh = null;
            ClearColliders();
            return;
        }

        var smooth = CatmullRomSpline(pts, samplesPerSegment);
        line.enabled = true;
        line.positionCount = smooth.Count;
        line.SetPositions(smooth.ToArray());

        _bakedMesh.Clear();
        line.BakeMesh(_bakedMesh, false);
        _meshCollider.sharedMesh = _bakedMesh;

        SpawnSideColliders(smooth);
        SpawnFloorAndCapColliders(smooth);
    }

    private void ClearColliders()
    {
        foreach (var go in _sideColliders) Destroy(go);
        _sideColliders.Clear();
        foreach (var go in _floorColliders) Destroy(go);
        _floorColliders.Clear();
    }

    private void SpawnSideColliders(List<Vector3> path)
    {
        foreach (var go in _sideColliders) Destroy(go);
        _sideColliders.Clear();
        if (sideColliderPrefab == null || path.Count < 2) return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 p0 = path[i]; Vector3 p1 = path[i+1];
            float len = Vector3.Distance(p0, p1);
            Vector3 mid = (p0 + p1) * 0.5f;
            Vector3 forward = (p1 - p0).normalized;
            Vector3 normal = Vector3.Cross(Vector3.up, forward).normalized;

            Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);
            Vector3 scale = new Vector3(colliderThickness, colliderHeight, len);

            var left = Instantiate(sideColliderPrefab, mid + normal * (line.startWidth * 0.5f), rot, transform);
            left.transform.localScale = scale;
            _sideColliders.Add(left);

            var right = Instantiate(sideColliderPrefab, mid - normal * (line.startWidth * 0.5f), rot, transform);
            right.transform.localScale = scale;
            _sideColliders.Add(right);
        }
    }

    private void SpawnFloorAndCapColliders(List<Vector3> path)
    {
        foreach (var go in _floorColliders) Destroy(go);
        _floorColliders.Clear();
        if (floorColliderPrefab == null || path.Count < 2) return;

        // Floor colliders under each segment
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 p0 = path[i]; Vector3 p1 = path[i+1];
            float len = Vector3.Distance(p0, p1);
            Vector3 mid = (p0 + p1) * 0.5f;
            Vector3 forward = (p1 - p0).normalized;

            Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);
            Vector3 floorScale = new Vector3(line.startWidth, floorThickness, len);
            var floor = Instantiate(floorColliderPrefab, mid - Vector3.up * (floorThickness * 0.5f), rot, transform);
            floor.transform.localScale = floorScale;
            _floorColliders.Add(floor);
        }

        CreateEndCap(path[0], path[1]);

        if (holePrefab != null)
        {
            Vector3 end      = path[^1];
            Vector3 before   = path[^2];
            Vector3 forward  = (end - before).normalized;
            Vector3 right   = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 spawnPos = end 
                            + forward * holeForwardOffset  
                            - Vector3.up   * holeDepthOffset
                            + right * holeSideOffset; 
            Quaternion holeRot = Quaternion.LookRotation(forward, Vector3.up);
            _spawnedHole = Instantiate(holePrefab, spawnPos, holeRot, transform);
        }
    }

    private void CreateEndCap(Vector3 position, Vector3 neighbor)
    {
        Vector3 dir = (neighbor - position).normalized;
        // Align cap forward with path direction
        Quaternion capRot = Quaternion.LookRotation(dir, Vector3.up);
        Vector3 capScale = new Vector3(line.startWidth, capHeight, capThickness);
        var cap = Instantiate(floorColliderPrefab, position, capRot, transform);
        cap.transform.localScale = capScale;
        _floorColliders.Add(cap);
    }

    private List<Vector3> CatmullRomSpline(List<Vector3> pts, int samples)
    {
        var result = new List<Vector3>();
        for (int i = 0; i < pts.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? pts[i] : pts[i - 1];
            Vector3 p1 = pts[i];
            Vector3 p2 = pts[i + 1];
            Vector3 p3 = i + 2 < pts.Count ? pts[i + 2] : pts[i + 1];
            for (int j = 0; j < samples; j++)
            {
                float t = j / (float)samples;
                result.Add(0.5f * (
                    2f * p1 +
                    (p2 - p0) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
                ));
            }
        }
        result.Add(pts[^1]);
        return result;
    }

    public void ClearAll()
    {
        if (_spawnedHole != null)
        {
            Destroy(_spawnedHole);
            _spawnedHole = null;
        }
        _floorColliders.Clear();
        _sideColliders.Clear();
        line.enabled = false;
        line.positionCount = 0;
        _meshCollider.sharedMesh = null;
    }

    void LateUpdate()
    {
        // Force the visualizer’s rotation back to identity every frame
        transform.rotation = Quaternion.identity;
    }
}
