using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float delayDuration;
    [SerializeField] private float respawnDelay;

    private SpringJoint2D currentBallSpringJoint;
    private Rigidbody2D currentBallRigidbody;
    private Camera mainCamera;
    private bool isDragging;

    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if(currentBallRigidbody == null)
        {
            return;
        }
        if (Touch.activeTouches.Count == 0)
        {
            if(isDragging)
            {
                LaunchBall();
            }
            
            isDragging = false;
            return;
        }
        isDragging = true;
        currentBallRigidbody.isKinematic = true;
        Vector2 touchPositions = new Vector2();
        foreach (Touch touch in Touch.activeTouches)
        {
            touchPositions += touch.screenPosition;
        }
        touchPositions /= Touch.activeTouches.Count;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPositions);
        currentBallRigidbody.position = worldPosition;
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();
        currentBallSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;
        Invoke(nameof(DetachBall), delayDuration);
        
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;
        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
