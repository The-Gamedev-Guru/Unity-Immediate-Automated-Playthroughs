// Author of this file: 

using System;
using System.Collections;
using System.Collections.Generic;
using Popcorn.GameObjects.Persons;
using UnityEngine;

public static class C
{
    public static Player Player
    {
        get  { return Player.Instance;}
    }

    public static string GodMode
    {
        get  { return string.Format($"Godmode: {Player.Instance.godMode = !Player.Instance.godMode}");}
    }

    public static float TimeScale
    {
        get { return Time.timeScale; }
        set { Time.timeScale = value; }
    }

    public static float Gravity
    {
        get { return Player.Instance.rb.gravityScale; }
        set { Player.Instance.rb.gravityScale = value; }
    }

    public static bool Win
    {
        get  { Player.Instance.Win(); return true;}
    }

    public static Coroutine Execute(IEnumerator function)
    {
        return Player.Instance.StartCoroutine(function);
    }

    public static Coroutine Move(float direction, float dashTime)
    {
        return Player.Instance.StartCoroutine(Move_Internal(direction, dashTime));
    }

    public static Coroutine Jump(float direction, float force = 0)
    {
        if (Mathf.Approximately(force, 0f)) force = Player.Instance.jumpForce;
        return Player.Instance.StartCoroutine(Jump_Internal(direction, force));
    }

    public static IEnumerator Move_Internal(float direction, float dashTime)
    {
        Debug.Log($"[C] Dashing in direction {direction} for {dashTime} seconds");
        var startTime = Time.time;
        yield return new WaitUntil(() =>
        {
            Player.Instance.ExecuteMove(direction);
            return Time.time - startTime > dashTime;
        });
        Debug.Log($"[C] Dash finished");
    }

    private static IEnumerator Jump_Internal(float direction, float force)
    {
        var groundLayer = LayerMask.GetMask("Ground");
        var player = Player.Instance;

        Debug.Log($"[C] Jumping with force {force}");
        Player.Instance.ExecuteJump(force);
        var time0 = Time.time;
        yield return new WaitUntil(() =>
        {
            player.ExecuteMove(direction);

            float raycastDistance = 0.8f;
            var hit = Physics2D.Raycast(player.rb.position, Vector2.down, raycastDistance, groundLayer);
            var onPlatform = (hit.collider != null && hit.collider.CompareTag("Platform"));
            return (Time.time - time0 > 0.2f) && onPlatform;
        });
        Debug.Log($"[C] Jump finished");
    }

}

