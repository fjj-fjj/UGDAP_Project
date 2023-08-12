using Skill;
using UnityEngine;

enum JumpDirection {
    None,
    Left,
    Right,
    Up
}

public class PlayerController : MonoBehaviour {
    public float JumpSpeed;
    public float MoveForce;
    public float MaxSpeed;

    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private JumpDirection lastJumpDirection = JumpDirection.None;
    private CharacterSkillManager skillManager;
    private SkillData dashSkillData;

    private void Start() {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
        this.collider = this.GetComponent<Collider2D>();
        this.skillManager = this.GetComponent<CharacterSkillManager>();
    }

    private void Update() {
        if (this.dashSkillData != null && this.dashSkillData.isCasting) {
            return;
        }

        if (Input.GetButtonDown("Dash")) {
            this.dashSkillData = this.skillManager.PrepareSkill(1);
            if (this.dashSkillData != null) {
                this.skillManager.GenerateSkill(this.dashSkillData);
            } else {
                Debug.LogWarning("Dash skill is still cooling");
            }
            return;
        }

        float velocity = Input.GetAxis("Horizontal");
        if (Mathf.Approximately(velocity, 0) == false) {
            //this.rigidbody.velocity = new Vector2(velocity * Speed, this.rigidbody.velocity.y);
            this.rigidbody.AddForce(velocity * MoveForce * Vector2.right);
        }

        JumpDirection direction = this.getAllowJumpDirection();
        if (direction != JumpDirection.None && Input.GetButtonDown("Jump")) {
            if (direction == JumpDirection.Up || direction != this.lastJumpDirection) { 
                this.rigidbody.velocity = new Vector2(this.rigidbody.velocity.x, JumpSpeed);
                this.lastJumpDirection = direction;
            }
        }

        this.rigidbody.velocity = new Vector2(
            Mathf.Clamp(this.rigidbody.velocity.x, -MaxSpeed, MaxSpeed),
            this.rigidbody.velocity.y
        );
    }

    private JumpDirection getAllowJumpDirection() {
        int downCount = this.collider.Cast(Vector2.down, new ContactFilter2D {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        if (downCount > 0)
            return JumpDirection.Up;

        int leftCount = this.collider.Cast(Vector2.left, new ContactFilter2D {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        if (leftCount > 0)
            return JumpDirection.Right;

        int rightCount = this.collider.Cast(Vector2.right, new ContactFilter2D {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        if (rightCount > 0)
            return JumpDirection.Left;

        return JumpDirection.None;
    }
}