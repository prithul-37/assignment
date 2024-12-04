using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Character : MonoBehaviour
{
    public CharacterAnimator anim;
    public float movespeed;
    public Light2D PlayerLight;

    public bool IsMoving {  get; private set; }

    //private void Awake()
    //{
    //    anim = GetComponent<CharacterAnimator>();
    //}
    public IEnumerator Move(Vector3 moveVec, Action OnMoveOver=null)
    {
        //Debug.Log(anim);
        anim.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f); ;
        anim.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f); ;

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if (!IsPathClear(targetPos)) {
            yield break;    
        }

        IsMoving=true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movespeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        anim.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        if(Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer |GameLayers.i.PlayerLayer) == true)
        {
            return false;
        }
        return true;
        
    }


    public bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            anim.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            anim.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
        {
            //Debug.LogError("Error in Look Towards: You can't ask the character to look diagonally");
            anim.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            anim.MoveY = 0;
        }
    }

    public CharacterAnimator Animator
    {
        get => anim;
    }

}
