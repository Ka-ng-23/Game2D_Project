using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;                  // refenrence to camera in game
    public Transform followTarget;      // refenrence to the transform (object "player") in game

    // Staring positions for the parallax game object
    Vector2 startingPosition;

    // Start the Z value of the parallax game object ("depth" between backgrounds and players)
    float startingZ;

    // Distance that the camera has moved from the starting position of the parallax object
    Vector2 camMoveSinceStart => (Vector2) cam.transform.position - startingPosition;
    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;

    // If object is in front of target, use near clip plane. If behind target, use farClipplane
    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));
    
    // the further the object from player, the faster ParallaxEffect object will move. Drag it's Z value closer to the target to make it move slower
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;
   
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;  
        startingZ = transform.position.z;

    }

    // Update is called once per frame
    void Update()
    {
        // when the target moves, move the parallax object the same distance times a multiplier
        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;
        
        // the x,y position changes based on 
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
