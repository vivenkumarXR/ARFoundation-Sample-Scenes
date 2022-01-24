using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.AR;
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectOnPlane_Interaction : MonoBehaviour
{
    [SerializeField]
    [Tooltip("A GameObject to place when a raycast from a user touch hits a plane.")]
    GameObject placedprefab;
    [SerializeField]
    [Tooltip("Disables tracking plane after you place the 3D Model.")]
    bool disableARPlane = true;
    GameObject spawnedObject;
    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    ARPlaneManager aRPlaneManeger;
    ARSessionOrigin arSessionOrigin;
    GameObject aRGestureInteractable;

    // Start is called before the first frame update
    private void Awake()
    {
        // Simple "JUGAR" to go around the problem of simulataneous selection of
        // 3D model with instanciation
        aRGestureInteractable = gameObject.transform.GetChild(0).gameObject;
        aRGestureInteractable.GetComponent<ARGestureInteractor>().enabled = false;
    }
    void Start()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManeger = GetComponent<ARPlaneManager>();
        arSessionOrigin = GetComponent<ARSessionOrigin>();
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
                // Create anchor to track reference point and set it as the parent of placementObject.
                var anchor = new GameObject("PlacementAnchor").transform;
                anchor.position = hitPose.position;
                anchor.rotation = hitPose.rotation;
                spawnedObject.transform.parent = anchor;

                // Use Trackables object in scene to use as parent
                if (arSessionOrigin != null && arSessionOrigin.trackablesParent != null)
                {
                    anchor.parent = arSessionOrigin.trackablesParent;
                }
                // Simple "JUGAR" to go around the problem of simulataneous selection of
                // 3D model with instanciation
                StartCoroutine(EnableARGestureInteractot());
            }

        }
        //If you want to disable tracking plane on floor after you place the model
        if (spawnedObject != null && disableARPlane == true)
        {
            foreach (var plane in aRPlaneManeger.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator EnableARGestureInteractot()
    {
        Debug.Log("inside inem");
        yield return new WaitForSeconds(2f);
        aRGestureInteractable.GetComponent<ARGestureInteractor>().enabled = true;
        Debug.Log("outside inem");
    }
}
