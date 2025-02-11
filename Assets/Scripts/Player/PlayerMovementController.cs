using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class PlayerMovementController : MonoBehaviour
{

    private PlayerStates.State state;

    private Transform Model;
    private Transform FirePoint;

    private Transform closeCrosshair;
    private Vector3 closeCrosshairDefault = new Vector3(0, 0, 30); //location for the crosshair to return to
    private Transform farCrosshair;

    Camera cam;
  
    // Start is called before the first frame update
    void Start()
    {

        //So that I don't move off screen while testing
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        cam = Camera.main;

        //Gets the playermodel mesh as a seperate Transform
        Model = this.gameObject.transform.GetChild(0);
        FirePoint = this.gameObject.transform.GetChild(1);
        closeCrosshair = this.gameObject.transform.GetChild(2);
        farCrosshair = this.gameObject.transform.GetChild(3);

        state = new PlayerStates.Idle(gameObject);

    }
  
    void Update()
    {

        //Controls using WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        /* Code for knife flying left or right, LeanTween is awesome for this */
   

        if (Input.GetKey("e")){
            LeanTween.rotateZ(Model.gameObject, -90, 0.2f);

        }

        if (Input.GetKeyUp("e"))
        {
            LeanTween.rotateZ(Model.gameObject, 0, 0.2f);
        }

        if (Input.GetKey("q"))
        {
            LeanTween.rotateZ(Model.gameObject, 90, 0.2f); 
        }

        if (Input.GetKeyUp("q"))
        {
            LeanTween.rotateZ(Model.gameObject, 0, 0.2f);
        }

        /* Do a barrel roll! */
        if (Input.GetKeyDown("z"))
        {
            LeanTween.rotateAroundLocal(Model.gameObject, Vector3.forward, 360f, 0.4f);

        }

        Move(horizontal, vertical, 20);
        //Functions to make ship look cooler when moving
        HorizontalLean(Model, horizontal, 60, 0.2f);
        //Option to turn on yaw pitching, looks mid
        //yawLean(transform, horizontal, 15, 0.5f);
        VerticalLean(Model, -vertical, 40, 0.2f);


        HandleNewState(state.OnUpdate(), state);
    }

    private void LateUpdate()
    {
        Clamp(closeCrosshair.transform);
        Clamp(transform);
    }


    void Move(float x, float y, float s)
    {

        closeCrosshair.transform.localPosition += new Vector3(x, -y, 0) * s * Time.deltaTime;
        transform.localPosition += new Vector3(closeCrosshair.transform.localPosition.x, closeCrosshair.transform.localPosition.y, 0) * 2 * Time.deltaTime;

        if (x == 0f && y == 0f)
        {
            closeCrosshair.localPosition = Vector3.Lerp(closeCrosshair.localPosition, closeCrosshairDefault, 2 * Time.deltaTime);
            //I commented this line out because I didn't know what it was doing and it fixed the dolly track bug.
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 1f);
        }

        var lookPos = closeCrosshair.transform.position - Model.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);

        //Model.rotation = Quaternion.Slerp(Model.transform.rotation, rotation, Time.deltaTime * 2.75f);

        //transform.LookAt(closeCrosshair.transform);

        //Model.LookAt(closeCrosshair.transform);

        FirePoint.LookAt(closeCrosshair.transform);

        //Model.rotation = FirePoint.rotation;

        //This draws the far crosshair
        RaycastHit hit;

        if (Physics.Raycast(FirePoint.transform.position, FirePoint.transform.forward, out hit))
        {
            if (hit.collider)
            {
                farCrosshair.transform.position = hit.point;
            }

        }

    }


    // Clamp function taken from https://answers.unity.com/questions/799656/how-to-keep-an-object-within-the-camera-view.html
    void Clamp(Transform target) 
    {

        Vector3 pos = Camera.main.WorldToViewportPoint(target.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        target.position = Camera.main.ViewportToWorldPoint(pos);

    }


    //Leaning function code adapted from code seen in this video https://www.youtube.com/watch?v=JVbr7osMYTo
    void HorizontalLean(Transform target, float axis, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, -axis * leanLimit, lerpTime));
    }

    void VerticalLean(Transform target, float axis, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngles = target.localEulerAngles;
        target.localEulerAngles = new Vector3(Mathf.LerpAngle(targetEulerAngles.x, -axis * leanLimit, lerpTime), targetEulerAngles.y, targetEulerAngles.z);
    }

    void yawLean(Transform target, float axis, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngels.x, Mathf.LerpAngle(targetEulerAngels.y, axis * leanLimit, lerpTime), targetEulerAngels.z);
    }

    void HandleNewState(PlayerStates.State newState, PlayerStates.State oldState)
    {
        if (newState != oldState)
        {
            state = newState;
            state.OnEnter();
        }
    }

}
