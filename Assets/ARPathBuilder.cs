using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPathBuilder : MonoBehaviour
{
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject golfBallPrefab;
    [SerializeField] private float insideOffset = 0.1f;
    [SerializeField] private float verticalOffset = 0.05f;
    private ARRaycastManager _raycastManager;
    private List<Vector3> _points = new List<Vector3>();
    private GameObject _spawnedBall; 
    [Tooltip("Drag your club GameObject here (the child of AR Camera)")]
    public GameObject club;
    private float? groundY = null;
    private List<GameObject> _waypointInstances = new List<GameObject>();

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        var screen = Touchscreen.current;
        if (screen == null) 
            return;  

        var touch = screen.primaryTouch;
        if (touch == null) 
            return;
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if(!club.activeSelf){
            if (touch.press.wasPressedThisFrame)
            {
                Vector2 screenPos = touch.position.ReadValue();

                var hits = new List<ARRaycastHit>();
                if (_raycastManager.Raycast(screenPos, hits, TrackableType.All))
                {
                    var pose = hits[0].pose;
                    if (groundY == null)
                        groundY = pose.position.y;
                    pose.position.y = groundY.Value;
                    _points.Add(pose.position);
                    var wp = Instantiate(waypointPrefab, pose.position, Quaternion.identity);
                    _waypointInstances.Add(wp);
                    PathVisualizer.Instance.SetControlPoints(_points);
                    if (_points.Count == 2 && golfBallPrefab != null && _spawnedBall == null)
                    {
                        Vector3 start   = _points[0];
                        Vector3 next    = _points[1];
                        Vector3 forward = (next - start).normalized;

                        // Move the spawn position forward along the path, plus a small vertical lift
                        Vector3 spawnPos = start 
                                            + forward * insideOffset 
                                            + Vector3.up  * verticalOffset;

                        // Hand off to the game manager
                        GolfGameManager.Instance.SetupHole(spawnPos);
                    }
                }
                else
                {
                    Debug.Log("[Direct] Raycast returned no hits");
                }
            }
        }
    }

    public void ClearAll()
    {
        _points.Clear();
        foreach (var wp in _waypointInstances)
            Destroy(wp);
        _waypointInstances.Clear();
        PathVisualizer.Instance.ClearAll();
    }
}
