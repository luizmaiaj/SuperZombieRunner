using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputState : MonoBehaviour
{
    public bool actionButton = false;
    public float absVelX = 0f;
    public float absVelY = 0f;
    public bool standing = false;
    public float standingThreshold = 1f;

    private Rigidbody2D body2d;

    private void Awake()
    {
        body2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        actionButton = Input.anyKeyDown;
    }

    private void FixedUpdate() // for all physics calculations, they should not be done in update
    {
        absVelX = System.Math.Abs(body2d.velocity.x);
        absVelY = System.Math.Abs(body2d.velocity.y);

        standing = absVelY <= standingThreshold;
    }
}
