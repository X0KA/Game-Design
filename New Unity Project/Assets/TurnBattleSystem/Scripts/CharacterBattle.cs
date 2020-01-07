/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class CharacterBattle : MonoBehaviour {

    //private Character_Base characterBase;
    private Stats CharStats;
    private BattleHandler BH;

    private State state;
    private Vector3 slideTargetPosition;
    private Action onSlideComplete;
    private Action onWaitComplete;
    public bool isPlayerTeam;
    public bool Dead;
    private GameObject selectionCircleGameObject;
    private GameObject m_GameObject;
    private HealthSystem healthSystem;
    private World_Bar healthBar;
    private Animator animator;
    public Stats.CharacterClass charclass;
    private bool timeToAttack;
    private bool going;
    private Animation currentanimation;
    public int ID;
    public int Health;
    public int Level;
    public int Experience = 0;
    public int Speed = 0;
    private int counter;

    private enum State {
        Idle,
        Sliding,
        Dead,
        Waiting,
        Busy,
    }

    private void Awake() {
        //characterBase = GetComponent<Character_Base>();
        selectionCircleGameObject = transform.Find("SelectionCircle").gameObject;
        BH = GameObject.Find("BattleHandler").GetComponent<BattleHandler>();
        HideSelectionCircle();
        state = State.Idle;
        
    }

    private void Start() {
    }

    public void Setup(bool isPlayerTeam,GameObject Own) {
        this.isPlayerTeam = isPlayerTeam;
        m_GameObject = Instantiate(Own,transform.position,Quaternion.identity,transform);
        CharStats = m_GameObject.GetComponent<Stats>();
        Level = CharStats.Level;
        Health = CharStats.Health;
        Speed = CharStats.Speed;
        charclass = CharStats.charclass;
    
        if (isPlayerTeam) {

        } else {
            Vector3 transform = m_GameObject.transform.localScale;
            transform.x *= -1;
            m_GameObject.transform.localScale = transform;
        }

        healthSystem = new HealthSystem(CharStats.Health);
        healthBar = new World_Bar(transform, new Vector3(0, 18), new Vector3(12, 1.7f), Color.grey, Color.red, 1f, 100, new World_Bar.Outline { color = Color.black, size = .6f });
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        animator = m_GameObject.GetComponentInChildren<Animator>();
        going = false;
        //PlayAnimIdle();
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
        healthBar.SetSize(healthSystem.GetHealthPercent());
    }

    private void PlayAnimIdle() {
        if (isPlayerTeam) {
            animator.Play("ReadyMelee1H");
        } else {
            animator.Play("ReadyMelee1H");
        }
    }

    private void Update() {

        if(state == State.Dead)
        {
            animator.SetBool("DieBack", true);
            BH.PopCharacterFromList(this, 0.9f);
            return;
        }

        switch (state)
        {

        case State.Idle:
            break;
        case State.Busy:
            break;
        case State.Waiting:

            if (timeToAttack)
            {
                animator.SetTrigger("Slash"); // Play animation randomly
                timeToAttack = false;
            }
            counter++;
            if (counter >= 40)
            {
                onWaitComplete();
            }
            break;

        case State.Sliding:
            float slideSpeed = 10f;
            transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;

            float reachedDistance = 1f;
            if (Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance) {
                onSlideComplete();
            }
            break;
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void Damage(CharacterBattle attacker, int damageAmount) {
        healthSystem.Damage(damageAmount);
        Vector3 dirFromAttacker = (GetPosition() - attacker.GetPosition()).normalized;

        DamagePopup.Create(GetPosition(), damageAmount, false);
        Blood_Handler.SpawnBlood(GetPosition(), dirFromAttacker);
        
        CodeMonkey.Utils.UtilsClass.ShakeCamera(1f, .1f);

        if (healthSystem.IsDead()) {
            Dead = true;
            // Died
            state = State.Dead;
            //animator.Play("DieBack");
        }
    }

    public bool IsDead() {
        return healthSystem.IsDead();
    }

    public void Attack(CharacterBattle targetCharacterBattle, Action onAttackComplete) {

        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        // Slide to Target
        going = true;
        timeToAttack = true;
        SlideToPosition(slideTargetPosition, () => {
            // Arrived at Target, attack him
            counter = 0;
            state = State.Busy;
            Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;

            waitPosition(() => {

                int damageAmount = CharStats.Damage;
                if (!CharStats.Blinded)
                    targetCharacterBattle.Damage(this, damageAmount);
                else
                {
                    if (--CharStats.BlindDuration < 1)
                        CharStats.Blinded = false;
                }

                SlideToPosition(startingPosition, () =>
                {
                    // Slide back completed, back to idle
                    state = State.Idle;
                    onAttackComplete();
                    //animator.Play("IdleMelee");
                });
            });

            

          
        });
    }

    public void PurifyingStrike(CharacterBattle[] targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle[1].GetPosition() + (GetPosition() - targetCharacterBattle[1].GetPosition()).normalized *10f;
        Vector3 startingPosition = GetPosition();

        // Slide to Target
        SlideToPosition(slideTargetPosition, () =>
        {
            // Arrived at Target, attack him
            state = State.Busy;
         
            animator.Play("SlashMelee1H");
            //int damageAmount = UnityEngine.Random.Range(20, 50);
            int damageAmount = CharStats.Damage /3;
            for (int i = 0; i < targetCharacterBattle.Length;++i) {
                targetCharacterBattle[i].Damage(this, damageAmount);
            }

            SlideToPosition(startingPosition, () =>
            {
                // Slide back completed, back to idle
                state = State.Idle;
                //characterBase.PlayAnimIdle(attackDir);
                onAttackComplete();
            });
        });
    }

    public void Delirium(Action onAttackComplete)
    {
        CharStats.Damage += 10;
        CharStats.Speed += 50;
        onAttackComplete();
    }

    public void HollyWater(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        animator.Play("Victory");

        waitPosition(() => {

            DamagePopup.Create(targetCharacterBattle.GetPosition(), CharStats.HealingPoints, false, true);
            targetCharacterBattle.healthSystem.Heal(CharStats.HealingPoints);
            onAttackComplete();

            state = State.Idle;
     
        });

    
    }

    public void BlindingDart(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        int blindDuration = 0;

        if (CharStats.Level < 9)
            blindDuration = 2;
        else if (CharStats.Level < 10)
            blindDuration = 3;
        else
            blindDuration = 4;

        targetCharacterBattle.CharStats.Blinded = true;
        targetCharacterBattle.CharStats.BlindDuration = blindDuration;

        int damageAmount = CharStats.Damage / 6;
        targetCharacterBattle.Damage(this, damageAmount);

        onAttackComplete();
    }

    public void PreciSeshot(CharacterBattle target, Action onattackcomplete)
    {

    }

    //public void Blessing(CharacterBattle target, Action onAttackComplete)
    //{

    //}

    //public void Slice(CharacterBattle target, Action onAttackComplete)
    //{

    //}


    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete) {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;

        if (going==true)
        {
            animator.Play("ChargeAttack2H");
            going = false;
        }
   
    }
    private void waitPosition(Action onWaitComplete)
    {
        this.onWaitComplete = onWaitComplete;
        state = State.Waiting;
        //animator.Play("SlashMelee1H");
    }


    public void HideSelectionCircle() {
        selectionCircleGameObject.SetActive(false);
    }

    public void ShowSelectionCircle() { 
        selectionCircleGameObject.SetActive(true);
    }


    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }


}
