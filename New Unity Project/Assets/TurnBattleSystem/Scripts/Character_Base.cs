﻿using System;
using UnityEngine;
using V_AnimationSystem;
using CodeMonkey.Utils;

/*
 * Character Base Class
 * */
public class Character_Base : MonoBehaviour {

    #region BaseSetup
    private V_UnitSkeleton unitSkeleton;
    private V_UnitAnimation unitAnimation;
    private AnimatedWalker animatedWalker;
    private UnitAnimType attackUnitAnim;
    private Color materialTintColor;

    private void Awake() {
        Transform bodyTransform = transform.Find("Body");
        transform.Find("Body").GetComponent<MeshRenderer>().material = new Material(GetMaterial());
        unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint, (Mesh mesh) => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
        unitAnimation = new V_UnitAnimation(unitSkeleton);

        UnitAnimType idleUnitAnim = UnitAnimType.GetUnitAnimType("dSwordTwoHandedBack_Idle");
        UnitAnimType walkUnitAnim = UnitAnimType.GetUnitAnimType("dSwordTwoHandedBack_Walk");
        UnitAnimType hitUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Hit");
        attackUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_PunchQuickAttack");

        animatedWalker = new AnimatedWalker(unitAnimation, idleUnitAnim, walkUnitAnim, 1f, 1f);
    }

    private void Update() {
        unitSkeleton.Update(Time.deltaTime);

        if (materialTintColor.a > 0) {
            float tintFadeSpeed = 6f;
            materialTintColor.a -= tintFadeSpeed * Time.deltaTime;
            GetMaterial().SetColor("_Tint", materialTintColor);
        }
    }

    public V_UnitAnimation GetUnitAnimation() {
        return unitAnimation;
    }

    public AnimatedWalker GetAnimatedWalker() {
        return animatedWalker;
    }
    #endregion

    public Material GetMaterial() {
        return transform.Find("Body").GetComponent<MeshRenderer>().material;
    }

    public void SetColorTint(Color color) {
        materialTintColor = color;
    }

    public void PlayAnimMove(Vector3 moveDir) {
        animatedWalker.SetMoveVector(moveDir);
    }

    public void PlayAnimIdle() {
        animatedWalker.SetMoveVector(Vector3.zero);
    }

    public void PlayAnimIdle(Vector3 animDir) {
        animatedWalker.PlayIdleAnim(animDir);
    }

    public void PlayAnimSlideRight() {
        unitAnimation.PlayAnimForced(UnitAnim.GetUnitAnim("dBareHands_SlideRight"), 1f, null);
    }

    public void PlayAnimSlideLeft() {
        unitAnimation.PlayAnimForced(UnitAnim.GetUnitAnim("dBareHands_SlideLeft"), 1f, null);
    }

    public void PlayAnimLyingUp() {
        unitAnimation.PlayAnimForced(UnitAnim.GetUnitAnim("LyingUp"), 1f, null);
    }

    public void PlayAnimAttack(Vector3 attackDir, Action onHit, Action onComplete) {
        unitAnimation.PlayAnimForced(attackUnitAnim, attackDir, 1f, (UnitAnim unitAnim) => {
            if (onComplete != null) onComplete();
        }, (string trigger) => {
            if (onHit != null) onHit();
        }, null);
    }

    public void SetAnimsBareHands() {
        animatedWalker.SetAnimations(UnitAnimType.GetUnitAnimType("dBareHands_Idle"), UnitAnimType.GetUnitAnimType("dBareHands_Walk"), 1f, 1f);
        attackUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_PunchQuickAttack");
    }

    public void SetAnimsSwordTwoHandedBack() {
        animatedWalker.SetAnimations(UnitAnimType.GetUnitAnimType("dSwordTwoHandedBack_Idle"), UnitAnimType.GetUnitAnimType("dSwordTwoHandedBack_Walk"), 1f, 1f);
        attackUnitAnim = UnitAnimType.GetUnitAnimType("dSwordTwoHandedBack_Sword");
    }

    public void SetAnimsSwordShield() {
        animatedWalker.SetAnimations(UnitAnimType.GetUnitAnimType("dSwordShield_Idle"), UnitAnimType.GetUnitAnimType("dSwordShield_Walk"), 1f, 1f);
        attackUnitAnim = UnitAnimType.GetUnitAnimType("dSwordShield_Attack");
    }

    public Vector3 GetHandLPosition() {
        return unitSkeleton.GetBodyPartPosition("HandL");
    }

    public Vector3 GetHandRPosition() {
        return unitSkeleton.GetBodyPartPosition("HandR");
    }  
}
