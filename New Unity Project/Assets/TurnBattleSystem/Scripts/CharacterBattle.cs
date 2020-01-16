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
using Assets.HeroEditor.Common.CharacterScripts;

public class CharacterBattle : MonoBehaviour {

    //private Character_Base characterBase;
    public Stats CharStats;
    private BattleHandler BH;

    ParticleSystem[] particles;
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
    private GameObject InShield;
    public bool blinded;

    public bool DeactiveBuff;
    public bool DeactiveDeBuff;
    public int BuffTurns;
    public int DeBuffTurns;

    private int counter;
    public GameObject toDestroy;
    public GameObject deBuffToDestroy;
    public LayerManager LM;

    private bool invulnerable;
    private bool Hide;
    private bool bleed;
    private int bleedcounter;

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
    
    public void Setup(bool isPlayerTeam,GameObject Own) {

        this.isPlayerTeam = isPlayerTeam;

        m_GameObject = Instantiate(Own,transform.position,Quaternion.identity,transform);
        CharStats = m_GameObject.GetComponent<Stats>();
        Level = CharStats.Level;
        Health = CharStats.Health;
        Speed = CharStats.Speed;
        charclass = CharStats.charclass;
        particles = CharStats.particles;
        LM = m_GameObject.GetComponent<LayerManager>();

        CharStats.parent = this.gameObject;

        if (isPlayerTeam)
        {
            InShield = m_GameObject.transform.Find("SheerWillShield").gameObject;
        } else {
            Vector3 transform = m_GameObject.transform.localScale;
            transform.x *= -1;
            m_GameObject.transform.localScale = transform;
        }

        Vector3 namePos = transform.position;
   
        invulnerable = false;
        healthSystem = new HealthSystem(CharStats.Health);
        if (CharStats.name != "Amadea")
        {
            healthBar = new World_Bar(transform, new Vector3(0, 24), new Vector3(12, 1.7f), Color.grey, Color.red, 1f, 100, new World_Bar.Outline { color = Color.black, size = .6f });
            namePos.x -= 8;
            namePos.y += 25;
        }
        else
        {
            healthBar = new World_Bar(transform, new Vector3(0, 29), new Vector3(12, 1.7f), Color.grey, Color.red, 1f, 100, new World_Bar.Outline { color = Color.black, size = .6f });
            namePos.x -= 8;
            namePos.y += 30;
        }

        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        animator = m_GameObject.GetComponentInChildren<Animator>();
        going = false;

        TextScript.Create(namePos, CharStats.name, 35, false, gameObject);


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

        if (DeactiveBuff)
        {
            invulnerable = false;
            if (Hide == true)
            {
                LM.RestartDefaultColors();
                Hide = false;
            }
            InShield.SetActive(false);
            DeactiveBuff = false;
        }
        if (DeactiveDeBuff)
        {
            blinded = false;
            DeactiveDeBuff = false;
            Destroy(deBuffToDestroy); 
        }
        if (bleed)
        {
            healthSystem.Damage(2);
            bleedcounter--;
            if (bleedcounter <= 0)
            {
                bleed = false;
            }
        }

        if (toDestroy != null)
        {
            Destroy(toDestroy, 2.0f);
        }

        if (state == State.Dead)
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

        animator.Play("Hit");
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
                if (!blinded)  
                    if(targetCharacterBattle.invulnerable==false)
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

                });
            });
        
        });
    }

    public void PurifyingStrike(List<CharacterBattle> targets, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = GetPosition() + new Vector3(5,0);
        Vector3 startingPosition = GetPosition();

        string LoginScreen = CharStats.name + " uses Purifying Strike to Group of enemys";

        TextScript.Create(new Vector3(-50, -40), LoginScreen, 45, true);

        going = true;
        timeToAttack = true;
        SlideToPosition(slideTargetPosition, () => {
            foreach(ParticleSystem ps in particles)
            {
                if (ps.name == "PurifingStrike")
                {
                    
                    toDestroy = Instantiate(ps).gameObject;
                }
            }
            // Arrived at Target, attack him
            counter = 0;
            state = State.Busy;
            
            waitPosition(() => {
                
                foreach(CharacterBattle tg in targets)
                {
                    tg.Damage(this, CharStats.Damage);
                }

                SlideToPosition(startingPosition, () =>
                {
                    // Slide back completed, back to idle
                    state = State.Idle;
                    onAttackComplete();
                });
            });

        });

        // Slide to Target

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

        string LoginScreen = CharStats.name + " uses Holly water on " + targetCharacterBattle.CharStats.name;

        TextScript.Create(new Vector3(-50, -40), LoginScreen, 45, true);

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

    public void PreciseShot(CharacterBattle target, Action onAttackComplete)
    {
        animator.Play("ReadyBowShot");

        string LoginScreen = CharStats.name + " uses Precise Shot on " + target.CharStats.name;

        TextScript.Create(new Vector3(-50, -40), LoginScreen, 45, true);

        Vector3 slideTargetPosition = GetPosition() + new Vector3(5, 0);
        Vector3 startingPosition = GetPosition();
        float angle = Mathf.Atan2((target.GetPosition().y-GetPosition().y) , (target.GetPosition().x - GetPosition().x));
        angle = angle * Mathf.Rad2Deg;
        going = true;
        timeToAttack = true;

        foreach (ParticleSystem ps in particles)
        {
            if (ps.name == "PreciseShoot")
            {
                Vector3 Rot = ps.transform.rotation.eulerAngles;
                Rot.z += -angle;
                toDestroy = Instantiate(ps, m_GameObject.transform.position, Quaternion.Euler(Rot)).gameObject;
            }
        }

        SlideToPosition(slideTargetPosition, () => {

            // Arrived at Target, attack him
            counter = 0;
            state = State.Busy;

            waitPosition(() => {

                int damageAmount = CharStats.Damage*2;
                if (!CharStats.Blinded)
                    if (target.invulnerable == false)
                        target.Damage(this, damageAmount);

                SlideToPosition(startingPosition, () =>
                {
                    // Slide back completed, back to idle
                    state = State.Idle;
                    onAttackComplete();
                });
            });

        });
    }

    public void Slice(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {

        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        // Slide to Target
        going = true;
        timeToAttack = true;
        SlideToPosition(slideTargetPosition, () => {
            // Arrived at Target, attack him
            counter = 0;
            state = State.Busy;
            targetCharacterBattle.bleed = true;
            targetCharacterBattle.bleedcounter = 3;

            foreach (ParticleSystem ps in particles)
            {
                if (ps.name == "Slice")
                {
                    toDestroy = Instantiate(ps).gameObject;
                }
            }

            waitPosition(() => {

                int damageAmount = CharStats.Damage;
                if (!blinded)
                    if (targetCharacterBattle.invulnerable == false)
                        targetCharacterBattle.Damage(this, damageAmount*2);

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

                });
            });

        });
    }

    public void LethalThreat(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = (targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 10f)+new Vector3(30,0);
        Vector3 startingPosition = GetPosition();

        string LoginScreen = CharStats.name + " uses Lethal Threat on " + targetCharacterBattle.CharStats.name;

        TextScript.Create(new Vector3(-50, -40), LoginScreen, 45, true);


        foreach (ParticleSystem ps in particles)
        {
            if (ps.name == "Dash")
            {
                toDestroy = Instantiate(ps,this.transform).gameObject;
            }
        }       
        // Slide to Target
        going = true;
        SlideToPosition(slideTargetPosition, () => {
            // Arrived at Target, attack him
            state = State.Busy;
            targetCharacterBattle.Damage(this, CharStats.Damage);
            SlideToPosition(startingPosition, () =>
            {
                // Slide back completed, back to idle
                state = State.Idle;
                onAttackComplete();
            });
        }, "JabMelee2H");
    }

    public void SheerWill(Action onAttackComplete)
    {
        animator.Play("Victory");

        string LoginScreen = CharStats.name + " uses Sheer Will to protect himself";

        TextScript.Create(new Vector3(-50, -40), LoginScreen, 45, true);


        waitPosition(() => {
            invulnerable = true;
            InShield.SetActive(true);
            onAttackComplete();
            BuffTurns = 5;
            state = State.Idle;
        });
    }

    public void Camouflage(Action onAttackComplete)
    {
        animator.Play("Victory");

        string LoginScreen = CharStats.name + " uses Camouflage to hide himself";

        TextScript.Create(new Vector3(-50, -40), LoginScreen, 45, true);

        waitPosition(() => {
            invulnerable = true;
            Hide = true;
            LM.SetColor(new Color(1f, 1f, 1f, 0.3f));
            onAttackComplete();
            BuffTurns = 5;
            state = State.Idle;
        });
    }

    public void Smoke(List<CharacterBattle> targets,Action onAttackComplete)
    {
        animator.Play("Victory");
        GameObject BindParticle = null;
        state = State.Busy;
        foreach (ParticleSystem ps in particles)
        {
            if (ps.name == "Smoke")
            {
                toDestroy = Instantiate(ps).gameObject;
            }
            if (ps.name == "Blind")
            {
                BindParticle = ps.gameObject;
            }
        }

        string LoginScreen = CharStats.name + " uses Smoke on Enemies";

        TextScript.Create(new Vector3(-50, -40), LoginScreen, 45, true);

        waitPosition(() => {

            foreach (CharacterBattle tg in targets)
            {
                tg.blinded = true;
                tg.DeBuffTurns = 5;
                tg.deBuffToDestroy = Instantiate(BindParticle, tg.transform);
            }

            onAttackComplete();
            state = State.Idle;
        });
    }

    //public void Blessing(CharacterBattle target, Action onAttackComplete)
    //{

    //}

    //public void Slice(CharacterBattle target, Action onAttackComplete)
    //{

    //}


    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete , string Animation = "ChargeAttack2H") {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;

        if (going==true)
        {
            animator.Play(Animation);
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
