using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour {
    public enum MovementType {
        MoveY,
        MoveX
    }

    public MovementType type;
    public float speed;
    private float startingSpeed;

    public bool isActive = false;
    private Vector3 spawnPosition;
    private float spawnRangeY = 3f;
    private Animator anim;

	public void Spawn() {
        InitializeVariables();
        DetermineMovementType();
        SetRandomSpawnPosition();
        SetAnimation();
        InitializePosition();
    }

    private void InitializeVariables() {
        if (anim == null) anim = GetComponent<Animator>();
        if (startingSpeed == 0) startingSpeed = speed;
        isActive = true;
    }

    private void DetermineMovementType() {
        type = Random.Range(0, 2) == 0 ? MovementType.MoveX : MovementType.MoveY;
    }

    private void SetAnimation() {
        if (type == MovementType.MoveX) anim.SetTrigger("IsBat");
        if (type == MovementType.MoveY) anim.SetTrigger("IsBomb");
    }

    private void InitializePosition() {
        if (spawnPosition.x > 0 && type == MovementType.MoveX) speed = -startingSpeed;
        else speed = startingSpeed;

        transform.position = spawnPosition;

        if (type == MovementType.MoveX) {
            ChooseInitialXPosition();
        }
    }

    private void SetRandomSpawnPosition() {
        float playerY = PlayerSystem.instance.transform.position.y;
        float randomY = type == MovementType.MoveX ? Random.Range(playerY, playerY + spawnRangeY) : playerY + 10;
        spawnPosition = new Vector3(ChooseRandomXPosition(), randomY, transform.position.z);
    }

    private float ChooseRandomXPosition() {
        return Random.Range(-6f, 6f); 
    }

    private void ChooseInitialXPosition() {
        float initialX = Random.Range(0, 2) == 0 ? -10f : 10f;
        GetComponent<SpriteRenderer>().flipX = initialX > 0;
        transform.position = new Vector3(initialX, transform.position.y, transform.position.z);
    }

    private void Update() {
        if (!isActive) {
            return;
        }

        if (type == MovementType.MoveX) {
            MoveX();
        } else if (type == MovementType.MoveY) {
            MoveY();
        }
    }

    private void MoveX() {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if ((transform.position.x > 9f && speed > 0) || (transform.position.x < -9f && speed < 0)) {
            isActive = false;
        }
    }

    private void MoveY() {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < PlayerSystem.instance.transform.position.y - 9f) {
            isActive = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            PlayerSystem.instance.HitEnemy(); 
        }
    }
}
