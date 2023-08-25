using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {
    public enum MovementType {
        None,
        Rotate,
        MoveX,
        MoveY
    }

    public MovementType type;
    public float speed;
    public float range;

    private BoxCollider2D boxCollider;
    private Vector3 initialPosition;
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    private bool playerOnPlatform;

    private float timeSinceLastRotation = 0f;
    private bool rotated = false;

    private void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        initialPosition = transform.position;
        if (type == MovementType.Rotate) speed *= 10;
    }

    public void Spawn() {
        float typeRoll = Random.value;
        speed = Random.Range(speed - 1f, speed + 1f);
        range = Random.Range(1, 3);

        if (typeRoll < 0.2f) { 
            type = MovementType.MoveY;
            range = 1;
        } else {
            type = (MovementType)Random.Range(0, 3);  
        }

        if (type == MovementType.MoveX) {
            float xPosition = transform.position.x;

            if (xPosition + range > 6f) {
                range = Mathf.Max(6f - xPosition, 0.5f);
            } else if (xPosition - range < -6f) {
                range = Mathf.Max(xPosition + 6f, 0.5f);
            }

            if (range < 0.5f) {
                type = MovementType.None;
            }
        }
    }

    private void Update() {
        CheckColliderState();
        if(type == MovementType.MoveY || type == MovementType.MoveX) CheckPlayerOnPlatform();

        if (type == MovementType.MoveY) {
            MoveY();
        } else if (type == MovementType.MoveX) {
            MoveX();
        } else if (type == MovementType.Rotate) {
            Rotate();
        }
    }

    void CheckColliderState() {
        bool downArrowPressed = Input.GetKey(KeyCode.DownArrow);
        bool playerAbovePlatform = PlayerSystem.instance.transform.position.y >= transform.position.y + 0.5f;

        boxCollider.enabled = !downArrowPressed && playerAbovePlatform;
        if(PlayerSystem.instance.transform.position.y - 5.5f >= transform.position.y)gameObject.SetActive(false);
    }

    private void CheckPlayerOnPlatform() {
        if (playerOnPlatform && PlayerSystem.instance != null && !PlayerSystem.instance.isJumping && PlayerSystem.instance.Falling) {
            PlayerSystem.instance.transform.SetParent(transform);
        } else if (!playerOnPlatform && PlayerSystem.instance != null) {
            PlayerSystem.instance.transform.SetParent(PlayerSystem.instance.originalParent);
        }
    }

    private void MoveY() {
        float newY = initialPosition.y + Mathf.PingPong(Time.time * speed, range * 2) - range;
        float newZ = transform.position.z; 
        transform.position = new Vector3(transform.position.x, newY, newZ);
    }

    private void MoveX() {
        float newX = initialPosition.x + Mathf.PingPong(Time.time * speed, range * 2) - range;
        float newZ = transform.position.z;
        transform.position = new Vector3(newX, transform.position.y, newZ);
    }

    private void Rotate() {
        timeSinceLastRotation += Time.deltaTime;

        if (!rotated && timeSinceLastRotation >= 3f) {
            targetRotation += 90f;
            rotated = true;
        }

        if (currentRotation < targetRotation) {
            float rotationAmount = speed * Time.deltaTime;
            transform.rotation *= Quaternion.Euler(0, 0, rotationAmount);
            currentRotation += rotationAmount;

            if (currentRotation >= targetRotation) {
                // Snap to the nearest multiple of 90 degrees
                int numRotations = Mathf.RoundToInt(currentRotation / 90f);
                float snappedRotation = numRotations * 90f;
                transform.rotation = Quaternion.Euler(0, 0, snappedRotation);

                currentRotation = snappedRotation;
                timeSinceLastRotation = 0f;
                rotated = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerOnPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerOnPlatform = false;
        }
    }
}



