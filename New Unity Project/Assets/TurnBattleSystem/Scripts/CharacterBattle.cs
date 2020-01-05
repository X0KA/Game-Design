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
    public bool isPlayerTeam;
    public bool Dead;
    private GameObject selectionCircleGameObject;
    private GameObject m_GameObject;
    private HealthSystem healthSystem;
    private World_Bar healthBar;
    private Animator animator;

    public int Health;
    public int Level;
    public int Experience = 0;
    public int Speed = 0;

    private enum State {
        Idle,
        Sliding,
        Dead,
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

        if (isPlayerTeam) {

        } else {
            Vector3 transform = m_GameObject.transform.localScale;
            transform.x *= -1;
            m_GameObject.transform.localScale = transform;
        }

        healthSystem = new HealthSystem(CharStats.Health);
        healthBar = new World_Bar(transform, new Vector3(0, 15), new Vector3(12, 1.7f), Color.grey, Color.red, 1f, 100, new World_Bar.Outline { color = Color.black, size = .6f });
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        animator = m_GameObject.GetComponentInChildren<Animator>();

        PlayAnimIdle();
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

        switch (state) {
        case State.Dead:
            
            animator.Play("DieBack");
            break;
        case State.Idle:
            break;
        case State.Busy:
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
        SlideToPosition(slideTargetPosition, () => {
            // Arrived at Target, attack him
            state = State.Busy;
            Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
            animator.Play("SlashMelee1H");
            //int damageAmount = UnityEngine.Random.Range(20, 50);
            int damageAmount = CharStats.Damage;
            targetCharacterBattle.Damage(this, damageAmount);

            SlideToPosition(startingPosition, () => {
                // Slide back completed, back to idle
                state = State.Idle;
                //characterBase.PlayAnimIdle(attackDir);
                onAttackComplete();
            });

            if (targetCharacterBattle.IsDead())
            {
                BH.PopCharacterFromList(targetCharacterBattle);
            }
         

            //characterBase.PlayAnimAttack(attackDir, () => {
            //    // Target hit
            //    int damageAmount = UnityEngine.Random.Range(20, 50);
            //    targetCharacterBattle.Damage(this, damageAmount);
            //    }, () => {
            //    // Attack completed, slide back
            //    SlideToPosition(startingPosition, () => {
            //        // Slide back completed, back to idle
            //        state = State.Idle;
            //        characterBase.PlayAnimIdle(attackDir);
            //        onAttackComplete();
            //    });
            //});
        });
    }

    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete) {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;
        if (slideTargetPosition.x > 0) {
            animator.Play("ChargeAttack2H");
            //characterBase.PlayAnimSlideRight();
        } else {
            animator.Play("ChargeAttack2H");

            //characterBase.PlayAnimSlideLeft();
        }
    }

    public void HideSelectionCircle() {
        selectionCircleGameObject.SetActive(false);
    }

    public void ShowSelectionCircle() { 
        selectionCircleGameObject.SetActive(true);
    }

}
