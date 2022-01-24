using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectOnPlane_NewInput : MonoBehaviour
{
    [SerializeField]
    [Tooltip("A GameObject to place when a raycast from a user touch hits a plane.")]
    GameObject placedprefab;
    [SerializeField]
    [Tooltip("A Spawned GameObject will reposition whereever the user hits on the trackable plane .")]
    bool repositionObject = false;
    [SerializeField]
    [Tooltip("Disables tracking plane after you place the 3D Model.")]
    bool disableARPlane = true;
    GameObject spawnedObject;
    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    ARPlaneManager aRPlaneManeger;

    // Start is called before the first frame update
    void Start()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManeger = GetComponent<ARPlaneManager>();
    }

    // This function is called when you touch the screen (via new Input System) 
    public void OnPlaceObject(InputValue value)
    {
        Vector2 touchPosition = value.Get<Vector2>();
        if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Get postion of the point where the ray hits the tracked plane
            Pose hitPose = hits[0].pose;
            if (spawnedObject == null)
            {
                //Instantiating the 3D Model
                spawnedObject = Instantiate(placedprefab, hitPose.position, hitPose.rotation);
            }
            else if (repositionObject)
            {
                spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }

        }
        //If you want to disable tracking plane after you place the model
        if (spawnedObject != null && disableARPlane == true)
        {
            foreach (var plane in aRPlaneManeger.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}
