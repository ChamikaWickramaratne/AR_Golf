using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARRaycastManager))]
public class ARMapPlacer : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab;
    private ARRaycastManager _raycastManager;
    private bool _mapPlaced = false;
    static List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    [Tooltip("Drag your club GameObject here (the child of AR Camera)")]
    public GameObject club;
    private GameObject map;
    [Tooltip("How far inside the map to spawn the ball")]
    public float ballInsideOffset = 0.2f;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (_mapPlaced) return;

        var screen = Touchscreen.current;
        if (screen == null) return;

        var touch = screen.primaryTouch;
        if (touch == null || !touch.press.wasPressedThisFrame) return;

        // Only block if that *finger* is over a UI element
        int id = touch.touchId.ReadValue();
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(id))
            return;

        Vector2 touchPos = touch.position.ReadValue();
        if (!club.activeSelf &&
            _raycastManager.Raycast(touchPos, _hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = _hits[0].pose;
            var rotated = hitPose.rotation * Quaternion.Euler(0f, 0f, 180f);
            map = Instantiate(mapPrefab, hitPose.position, hitPose.rotation);
            _mapPlaced = true;

            Vector3 insidePos = map.transform.position
                                + map.transform.forward * ballInsideOffset
                                + Vector3.up * 0.05f;
            GolfGameManager.Instance.SetupHole(insidePos);
        }
    }

    public void ClearAll()
    {
        if(map){
            Destroy(map);
        }
    }
}
