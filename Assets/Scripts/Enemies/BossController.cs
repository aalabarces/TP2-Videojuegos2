using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Enemy
{
    override protected void Update()
    {
        base.Update();
        HandleBossBehavior();
    }

    protected override void Move()
    {
        // steering
        FleeFromPlayer();
        AvoidObstacles();
        base.Move();
    }

    private void AvoidObstacles()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, 2.0f, LayerMask.GetMask("Obstacles"));
        if (hit.collider != null)
        {
            Vector2 obstacle = (hit.collider.transform.position - transform.position).normalized;
            Vector2 desiredVelocity = Vector2.Perpendicular(obstacle) * maxSpeed;
            Vector2 steering = desiredVelocity - rb.velocity;
            rb.velocity += steering * Time.fixedDeltaTime;
        }
    }

    override public void FleeFromPlayer()
    {
        Vector2 position = transform.position;
        Vector2 playerPos = GameManager.Instance.player.transform.position;
        if (Vector2.Distance(position, playerPos) > 5.0f)
        {
            return; // Only flee if within a certain distance
        }
        Vector2 desiredVelocity = (position - playerPos).normalized * maxSpeed;
        Vector2 steering = desiredVelocity - rb.velocity;
        rb.velocity += steering * Time.fixedDeltaTime;
    }

    private void HandleBossBehavior()
    {
        // Additional boss-specific behavior can be implemented here
    }
}
