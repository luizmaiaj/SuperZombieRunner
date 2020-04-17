﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantVelocity : MonoBehaviour
{
    public Vector2 velocity = Vector2.zero;

    private Rigidbody2D body2d;

    void Awake()
    {
        body2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        body2d.velocity = velocity;
    }
}
