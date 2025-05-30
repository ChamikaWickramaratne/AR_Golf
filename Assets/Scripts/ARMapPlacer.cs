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
    public GameObject club;
    private GameObject map;
    public float ballInsideOffset = 0.2f;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        //only place 1 map
        if (_mapPlaced) return;

        var screen = Touchscreen.current;
        if (screen == null) return;

        var touch = screen.primaryTouch;
        if (touch == null || !touch.press.wasPressedThisFrame) return;

        int id = touch.touchId.ReadValue();
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(id))
            return;

        //get touch position
        Vector2 touchPos = touch.position.ReadValue();
        //only place when club is not active
        if (!club.activeSelf &&
            _raycastManager.Raycast(touchPos, _hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = _hits[0].pose;
            var rotated = hitPose.rotation * Quaternion.Euler(0f, 0f, 180f);
            //instantiate map
            map = Instantiate(mapPrefab, hitPose.position, hitPose.rotation);
            _mapPlaced = true;
            //Get positiion the spawn ball
            Vector3 insidePos = map.transform.position
                                + map.transform.forward * ballInsideOffset
                                + Vector3.up * 0.05f;
            //initiate game
            GolfGameManager.Instance.SetupHole(insidePos);
        }
    }

    public void ClearAll()
    //clear everything
    {
        if(map){
            Destroy(map);
        }
    }
}
