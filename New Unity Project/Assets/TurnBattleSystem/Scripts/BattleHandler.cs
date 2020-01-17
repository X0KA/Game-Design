/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleHandler : MonoBehaviour {

    public enum SceneStatus
    {
         MainMenu,
         Options,
         Battle
    }


    private static BattleHandler instance;

    public static BattleHandler GetInstance() {
        return instance;
    }

    public Transform[] PlayerPositions;
    public Transform[] EnemyPositions;
    [SerializeField] private Transform pfCharacterBattle;
    //public Texture2D playerSpritesheet;
    //public Texture2D enemySpritesheet;
    public GameObject[] Player;
    public GameObject[] Enemy;

    public GameObject monkSkillsMenu;
    public GameObject priestSkillsMenu;
    public GameObject warriorSkillsMenu;
    public SceneStatus currentScene = SceneStatus.MainMenu;

    public CharacterBattle currentTarget;

    private int IndexCreatorPlayers=0;
    private int IndexCreatorEnemys=0;

    private List<CharacterBattle> playerCharacterBattle = new List <CharacterBattle>();
    private List<CharacterBattle> enemyCharacterBattle = new List<CharacterBattle>();
    private CharacterBattle activeCharacterBattle;
    private List<CharacterBattle> QueueOfCharacterBattles = new List<CharacterBattle>();
    private State state;
    public GameObject UIinBattle;
    public GameObject UI_Menu;
    //Used to click on characters
    private Ray ray;
    public Camera camera;
    public bool playerVsBoot;
    public bool Battlefinished;
    private LoadingInfo info;

    private enum State {
        WaitingForPlayer,
        Busy,
    }

    private void Awake() {
        instance = this;
    }


    public void StartScene()
    {
        playerVsBoot = info.PlayerVsBot;
        currentScene = SceneStatus.Battle;

        for (int i = 0; i < 3; i++)
        {
            playerCharacterBattle.Add(SpawnCharacter(true, Player[IndexCreatorPlayers]));
            enemyCharacterBattle.Add(SpawnCharacter(false, Enemy[(int)info.EnemyList[i]]));
            QueueOfCharacterBattles.Add(playerCharacterBattle[i]);
            QueueOfCharacterBattles.Add(enemyCharacterBattle[i]);
        }

        //QueueOfCharacterBattles.Sort(SortBySpeed);
        //playerCharacterBattle = SpawnCharacter(true, Player);
        //enemyCharacterBattle = SpawnCharacter(false,Enemy);

        SetActiveCharacterBattle(QueueOfCharacterBattles[0]);
        currentTarget = enemyCharacterBattle[Random.Range(0, enemyCharacterBattle.Count)];
        currentTarget.EnableCursor();

        if (activeCharacterBattle.isPlayerTeam)
        {
            state = State.WaitingForPlayer;
        }
        else
            state = State.Busy;
        UI_Menu.SetActive(false);
        UIinBattle.SetActive(true);

    }
    public void CleanUp()
    {
        playerCharacterBattle.Clear();
        enemyCharacterBattle.Clear();

        foreach (CharacterBattle npc in QueueOfCharacterBattles)
        {
            PopCharacterFromList(npc,0.0f);
            int i = 0;
        }
        //QueueOfCharacterBattles.Clear();
    }

    private void Start() {

        info = GameObject.Find("GameManager").GetComponent<LoadingInfo>();
    }

    private void Update() {

        if (TestBattleOver())
        {
            return;
        }
        if (currentScene == SceneStatus.Battle)
        {
            if (playerVsBoot)
            {
                ClickOnObjective();

                if (state == State.WaitingForPlayer)
                {

                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        currentTarget = enemyCharacterBattle[Random.Range(0, enemyCharacterBattle.Count)];
                        state = State.Busy;
                        ToAttack(activeCharacterBattle.charclass, currentTarget);
                    }

                }
            }
            else
            {
                if (state == State.WaitingForPlayer)
                {
                    currentTarget = enemyCharacterBattle[Random.Range(0, enemyCharacterBattle.Count)];
                    state = State.Busy;
                    ToAttack(activeCharacterBattle.charclass, currentTarget);
                }
            }
        }
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam,GameObject toSpawn) {
        Vector3 position;
        if (isPlayerTeam) {
            position = PlayerPositions[IndexCreatorPlayers].position;
            IndexCreatorPlayers++;
            //position = new Vector3(-50, 0);
        } else {
            position = EnemyPositions[IndexCreatorEnemys].position;
            IndexCreatorEnemys++;
        }

        Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattle.ID = Random.Range(0,100000000);
        characterBattle.Setup(isPlayerTeam,toSpawn);

        return characterBattle;
    }

    private void SetActiveCharacterBattle(CharacterBattle characterBattle) {
        if (activeCharacterBattle != null) {
            activeCharacterBattle.HideSelectionCircle();
        }

        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowSelectionCircle();
    }

    private void ChooseNextActiveCharacter() {

        if (TestBattleOver()) {
            return;
        }
        UpdateBufs();
        QueueOfCharacterBattles.RemoveAt(0);
        QueueOfCharacterBattles.Add(activeCharacterBattle);
        //activeCharacterBattle = QueueOfCharacterBattles[0];
        SetActiveCharacterBattle(QueueOfCharacterBattles[0]);

        if (!activeCharacterBattle.isPlayerTeam)
        {
            state = State.Busy;
            ToAttack(activeCharacterBattle.charclass, playerCharacterBattle[Random.Range(0, playerCharacterBattle.Count)]);

        }
        else
        {
            state = State.WaitingForPlayer;
            currentTarget = enemyCharacterBattle[Random.Range(0, enemyCharacterBattle.Count)];
            switch (activeCharacterBattle.charclass)
            {
                case Stats.CharacterClass.Monk:
                    monkSkillsMenu.SetActive(true);
                    warriorSkillsMenu.SetActive(false);
                    priestSkillsMenu.SetActive(false);
                    break;
                case Stats.CharacterClass.Warrior:
                    monkSkillsMenu.SetActive(false);
                    warriorSkillsMenu.SetActive(true);
                    priestSkillsMenu.SetActive(false);
                    break;
                case Stats.CharacterClass.Sacerdotist:
                    monkSkillsMenu.SetActive(false);
                    warriorSkillsMenu.SetActive(false);
                    priestSkillsMenu.SetActive(true);
                    break;
            }
        }

    }

    private bool TestBattleOver() {

        //TODO CHECK FOR ALL CHARACTERS

        //if()
        if (currentScene == SceneStatus.Battle)
        {
            if (playerCharacterBattle.Count == 0)
            {

                BattleOverWindow.Show_Static("Enemy Wins!");

                return true;
            }
            if (enemyCharacterBattle.Count == 0)
            {

                BattleOverWindow.Show_Static("Player Wins!");
                return true;

            }
        }
        return false;
    }

    public void Return()
    {
        CleanUp();
        currentScene = SceneStatus.MainMenu;
    }
    static int SortBySpeed(CharacterBattle p1, CharacterBattle p2)
    {
        return p2.Speed.CompareTo(p1.Speed);
    }

    
    public void PopCharacterFromList(CharacterBattle toPop,float delay)
    {
        QueueOfCharacterBattles.Remove(toPop);

        if (toPop.isPlayerTeam)
        {
            //CharacterBattle handler = toPop;
            playerCharacterBattle.Remove(toPop);
            Destroy(toPop.gameObject,delay);
            return;

        }
        else
        {
            enemyCharacterBattle.Remove(toPop);
            Destroy(toPop.gameObject,delay);
            return;
        }

    }
    
    public void ToAttack(Stats.CharacterClass charclass,CharacterBattle target)
    {
        int indexSkill = RandomizeAttack();

        switch (charclass)
        {
            case Stats.CharacterClass.None:
                break;
            case Stats.CharacterClass.Monk:
                MonkSkills(indexSkill,target);
                break;
            case Stats.CharacterClass.Warrior:
                WarriorSkills(indexSkill, target);
                break;
            case Stats.CharacterClass.Sacerdotist:
                SacerdotistSkills(indexSkill, target);
                break;
            case Stats.CharacterClass.Enemy:
                //only to Test
                activeCharacterBattle.Attack(target, () => {
                    ChooseNextActiveCharacter();
                });
                break;
            default:
                break;
        }
    }

    public CharacterBattle getMinimumHealth()
    {

        if (activeCharacterBattle.isPlayerTeam)
        {
            CharacterBattle temp = new CharacterBattle();
            foreach (CharacterBattle pt in playerCharacterBattle)
            {
                if (temp.Health < pt.Health)
                {
                    temp = pt;
                }
            }
            return temp;
        }
        else
        {
            CharacterBattle temp = new CharacterBattle();
            foreach (CharacterBattle et in enemyCharacterBattle)
            {
                if (temp.Health < et.Health)
                {
                    temp = et;
                }
            }
            return temp;
        }

    }
    
    public int RandomizeAttack()
    {

        return Random.Range(0, activeCharacterBattle.CharStats.AbleToSkill());
      
    }

    public void MonkSkills(int skill, CharacterBattle target)
    {
        switch (skill)
        {
            case 0:
                activeCharacterBattle.Attack(target, () => {
                    ChooseNextActiveCharacter();
                });
                break;
            case 1:
                activeCharacterBattle.PurifyingStrike(enemyCharacterBattle, () => {
                    ChooseNextActiveCharacter();
                });
                break;
            case 2:
                activeCharacterBattle.LethalThreat(target, () => {
                    ChooseNextActiveCharacter();
                });
                break;
            case 3:
                activeCharacterBattle.SheerWill(() => {
                    ChooseNextActiveCharacter();
                });
                break;
            default:
                break;
        }
       
    }

    public void SacerdotistSkills(int skill, CharacterBattle target)
    {
        switch (skill)
        {
            case 0:
                activeCharacterBattle.Attack(target, () => {
                    ChooseNextActiveCharacter();
                });
                break;
            case 1:
                activeCharacterBattle.HollyWater(getMinimumHealth(), () =>
                {
                    ChooseNextActiveCharacter();
                });
                break;
            case 2:
                activeCharacterBattle.PreciseShot(target, () =>
                {
                    ChooseNextActiveCharacter();
                });
                break;
            case 3:
                activeCharacterBattle.Smoke(enemyCharacterBattle,() =>
                {
                    ChooseNextActiveCharacter();
                });
                break;
            default:
                break;
        }

    }

    public void WarriorSkills(int skill, CharacterBattle target)
    {
        switch (skill)
        {
            case 0:
                activeCharacterBattle.Attack(currentTarget, () => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 1:
                activeCharacterBattle.Camouflage(() => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 2:
                activeCharacterBattle.Slice(currentTarget, () => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            default:
                break;
        }
    }

    public void MonkSkills(int skill)
    {
        if (state == State.Busy)
            return;

        
        switch (skill)
        {
            case 0:
                if (currentTarget.charclass == Stats.CharacterClass.Sacerdotist || currentTarget.charclass == Stats.CharacterClass.Warrior)
                    return;
                activeCharacterBattle.Attack(currentTarget, () => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 1:
                activeCharacterBattle.PurifyingStrike(enemyCharacterBattle, () => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 2:
                if (currentTarget.charclass == Stats.CharacterClass.Sacerdotist || currentTarget.charclass == Stats.CharacterClass.Warrior)
                    return;
                activeCharacterBattle.LethalThreat(currentTarget, () => {
                    ChooseNextActiveCharacter();
                    state = State.Busy;
                });
                break;
            case 3:
                activeCharacterBattle.SheerWill(() => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            default:
                break;
        }

    }

    public void SacerdotistSkills(int skill)
    {

        if (state == State.Busy)
            return;

        //state = State.Busy;
        switch (skill)
        {
            case 0:

                if (currentTarget.charclass == Stats.CharacterClass.Monk || currentTarget.charclass == Stats.CharacterClass.Warrior)
                    return;

                activeCharacterBattle.Attack(currentTarget, () => {
                    ChooseNextActiveCharacter();
                });

                state = State.Busy;
                break;
            case 1:
                if (!(currentTarget.charclass == Stats.CharacterClass.Monk) && !(currentTarget.charclass == Stats.CharacterClass.Warrior))
                    return;

                activeCharacterBattle.HollyWater(getMinimumHealth(), () =>
                {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 2:
                if (currentTarget.charclass == Stats.CharacterClass.Monk || currentTarget.charclass == Stats.CharacterClass.Warrior)
                    return;
                activeCharacterBattle.PreciseShot(currentTarget, () =>
                {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 3:
                if (currentTarget.charclass == Stats.CharacterClass.Monk || currentTarget.charclass == Stats.CharacterClass.Warrior)
                    return;
                activeCharacterBattle.Smoke(enemyCharacterBattle, () =>
                {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            default:
                break;
        }

    }

    public void WarriorSkills(int skill)
    {
        if (state == State.Busy)
            return;

        //state = State.Busy;
        switch (skill)
        {
            case 0:
                activeCharacterBattle.Attack(currentTarget, () => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 1:
                activeCharacterBattle.Camouflage(() => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            case 2:
                activeCharacterBattle.Slice(currentTarget, () => {
                    ChooseNextActiveCharacter();
                });
                state = State.Busy;
                break;
            default:
                break;
        }
    }


    private void UpdateBufs()
    {
        foreach(CharacterBattle ch in QueueOfCharacterBattles)
        {
            if (ch.BuffTurns > 0)
            {
                ch.BuffTurns--;
                if (ch.BuffTurns == 0)
                {
                    ch.DeactiveBuff = true;
                }
            }
            
            if(ch.DeBuffTurns>0)
            {
                ch.DeBuffTurns--;
                if (ch.DeBuffTurns == 0)
                {
                    ch.DeactiveDeBuff = true;
                }
            }
        }
    }

    private void ClickOnObjective()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit_;
            if (Physics.Raycast(ray, out hit_, 200.0f))
            {
                currentTarget.DisableCursor();
                Debug.DrawRay(ray.GetPoint(0.0f), ray.direction * 70.0f, Color.blue);
                currentTarget = hit_.collider.gameObject.GetComponent<Stats>().parent.GetComponent<CharacterBattle>();
                currentTarget.EnableCursor();
            }
            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                Debug.DrawRay(ray.GetPoint(0.0f), ray.direction * 70.0f, Color.red);
            }
        }
    }
}
