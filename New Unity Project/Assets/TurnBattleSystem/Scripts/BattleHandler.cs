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

public class BattleHandler : MonoBehaviour {

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

    private int IndexCreatorPlayers=0;
    private int IndexCreatorEnemys=0;

    private List<CharacterBattle> playerCharacterBattle = new List <CharacterBattle>();
    private List<CharacterBattle> enemyCharacterBattle = new List<CharacterBattle>();
    private CharacterBattle activeCharacterBattle;
    private List<CharacterBattle> QueueOfCharacterBattles = new List<CharacterBattle>();
    private State state;

    private enum State {
        WaitingForPlayer,
        Busy,
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {

        for(int i = 0; i < 3; i++)
        {
            playerCharacterBattle.Add(SpawnCharacter(true, Player[IndexCreatorPlayers]));
            enemyCharacterBattle.Add(SpawnCharacter(false, Enemy[IndexCreatorEnemys]));
            QueueOfCharacterBattles.Add(playerCharacterBattle[i]);
            QueueOfCharacterBattles.Add(enemyCharacterBattle[i]);
        }

        QueueOfCharacterBattles.Sort(SortBySpeed);
        //playerCharacterBattle = SpawnCharacter(true, Player);
        //enemyCharacterBattle = SpawnCharacter(false,Enemy);

        SetActiveCharacterBattle(QueueOfCharacterBattles[0]);

        if (activeCharacterBattle.isPlayerTeam)
        {
            state = State.WaitingForPlayer;
        }
        else
            state = State.Busy;
    }

    private void Update() {
        if (TestBattleOver())
        {
            return;
        }

        if (state == State.WaitingForPlayer) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = State.Busy;
                activeCharacterBattle.BlindingDart(enemyCharacterBattle[1], () => {
                    ChooseNextActiveCharacter();
                });
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                state = State.Busy;
                ToAttack(activeCharacterBattle.charclass, enemyCharacterBattle[Random.Range(0, enemyCharacterBattle.Count)]);

                //activeCharacterBattle.Attack(enemyCharacterBattle[Random.Range(0,enemyCharacterBattle.Count)],() => {
                //    ChooseNextActiveCharacter();
                //});
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

        QueueOfCharacterBattles.RemoveAt(0);
        QueueOfCharacterBattles.Add(activeCharacterBattle);
        //activeCharacterBattle = QueueOfCharacterBattles[0];
        SetActiveCharacterBattle(QueueOfCharacterBattles[0]);

        if (!activeCharacterBattle.isPlayerTeam)
        {
            state = State.Busy;
            ToAttack(activeCharacterBattle.charclass,playerCharacterBattle[Random.Range(0, playerCharacterBattle.Count)]);

        }
        else
            state = State.WaitingForPlayer;

        //if (activeCharacterBattle == playerCharacterBattle) {
        //    SetActiveCharacterBattle(enemyCharacterBattle);
        //    state = State.Busy;
            
        //    enemyCharacterBattle.Attack(playerCharacterBattle, () => {
        //        ChooseNextActiveCharacter();
        //    });
        //} else {
        //    SetActiveCharacterBattle(playerCharacterBattle);
        //    state = State.WaitingForPlayer;
        //}
    }

    private bool TestBattleOver() {

        //TODO CHECK FOR ALL CHARACTERS

        //if()

        if (playerCharacterBattle.Count==0) {
            // Player dead, enemy wins
            //CodeMonkey.CMDebug.TextPopupMouse("Enemy Wins!");
            BattleOverWindow.Show_Static("Enemy Wins!");
            return true;
        }
        if (enemyCharacterBattle.Count==0) {
            // Enemy dead, player wins
            //CodeMonkey.CMDebug.TextPopupMouse("Player Wins!");
            BattleOverWindow.Show_Static("Player Wins!");
            return true;
            
        }
        
        return false;
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
    
    private void ToAttack(Stats.CharacterClass charclass,CharacterBattle target)
    {
        switch (charclass)
        {
            case Stats.CharacterClass.None:
                break;
            case Stats.CharacterClass.Monk:
                activeCharacterBattle.Attack(target, () => {
                    ChooseNextActiveCharacter();
                });
                break;
            case Stats.CharacterClass.Warrior:
                break;
            case Stats.CharacterClass.Mage:
                break;
            case Stats.CharacterClass.Sacerdotist:
                int i = Random.Range(0, 2);
                //int i = 1;
                if (i == 0)
                {
                    activeCharacterBattle.Attack(target, () => {
                        ChooseNextActiveCharacter();
                    });
                }
                else if (i == 1)
                {
                    if (activeCharacterBattle.isPlayerTeam)
                    {
                        activeCharacterBattle.HollyWater(playerCharacterBattle[Random.Range(0, playerCharacterBattle.Count)], () =>
                        {
                            ChooseNextActiveCharacter();
                        });
                    }
                    else
                    {
                        activeCharacterBattle.HollyWater(enemyCharacterBattle[Random.Range(0, enemyCharacterBattle.Count)], () =>
                        {
                            ChooseNextActiveCharacter();
                        });
                    }
                }
                break;
            default:
                break;
        }
    }

}
