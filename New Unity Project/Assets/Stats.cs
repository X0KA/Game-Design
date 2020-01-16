using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public enum CharacterClass
    {
        None=-1,
        Monk,
        Warrior,
        Defender,
        Sacerdotist,
        Enemy
    }

    public ParticleSystem[] particles;
    public string name = "DefaultName";
    public CharacterClass charclass = CharacterClass.None;
    public int Health = 0;
    public int Damage = 0;
    public int Level = 1;
    public int Speed = 0;
    public int HealingPoints = 0;
    public bool Blinded = false;
    public int BlindDuration = 0;

    public GameObject parent;
    
    public int AbleToSkill()
    {
        int ret=1;

        switch (charclass)
        {
            case CharacterClass.None:
                break;
            case CharacterClass.Sacerdotist:
            case CharacterClass.Monk:
                if (Level >= 1) { ret++; }
                if (Level >= 4) { ret++; }
                if (Level >= 7) { ret++; }
                if (Level >= 10) { ret++; }
                break;
            case CharacterClass.Warrior:
                if (Level >= 1) { ret++; }
                if (Level >= 4) { ret++; }
                if (Level >= 7) { ret++; }
                break;
            case CharacterClass.Enemy:
                break;
            default:
                break;
        }

        return ret;
    }

    public int GetLevel()
    {
        return Level;
    }
    public void SetLevel( int lvl)
    {
        Level = lvl;
    }

    public void AdmonSetStatsbyLevel(int lvl)
    {
        Level = lvl;
        switch (lvl)
        {
            case 1:
                Health = 2;
                Damage = 1;
                break;
            case 2:
                Health = 6;
                Damage = 2;
                break;
            case 3:
                Health = 7;
                Damage = 3;
                break;
            case 4:
                Health = 8;
                Damage = 3;
                break;
            case 5:
                Health = 10;
                Damage = 4;
                break;
            case 6:
                Health = 12;
                Damage = 4;
                break;
            case 7:
                Health = 14;
                Damage = 5;
                break;
            case 8:
                Health = 16;
                Damage = 5;
                break;
            case 9:
                Health = 20;
                Damage = 5;
                break;
            case 10:
                Health = 25;
                Damage = 6;
                break;

            default:
                break;
        }


    }

    public void FlorenceSetStatsbyLevel(int lvl)
    {
        Level = lvl;
        switch (lvl)
        {
            case 1:
                Health = 3;
                Damage = 1;
                break;
            case 2:
                Health = 3;
                Damage = 2;
                break;
            case 3:
                Health = 4;
                Damage = 2;
                break;
            case 4:
                Health = 4;
                Damage = 3;
                break;
            case 5:
                Health = 5;
                Damage = 3;
                break;
            case 6:
                Health = 5;
                Damage = 3;
                break;
            case 7:
                Health = 7;
                Damage = 4;
                break;
            case 8:
                Health = 8;
                Damage = 4;
                break;
            case 9:
                Health = 10;
                Damage = 5;
                break;
            case 10:
                Health = 13;
                Damage = 5;
                break;
            default:
                break;
        }
    }

    public void ClaudioSetStatsbyLevel(int lvl)
    {
        Level = lvl;
        switch (lvl)
        {
            case 1:
                Health = 3;
                Damage = 1;
                break;
            case 2:
                Health = 3;
                Damage = 2;
                break;
            case 3:
                Health = 4;
                Damage = 3;
                break;
            case 4:
                Health = 6;
                Damage = 3;
                break;
            case 5:
                Health = 6;
                Damage = 4;
                break;
            case 6:
                Health = 9;
                Damage = 5;
                break;
            case 7:
                Health = 12;
                Damage = 5;
                break;
            case 8:
                Health = 14;
                Damage = 6;
                break;
            case 9:
                Health = 16;
                Damage = 8;
                break;
            case 10:
                Health = 20;
                Damage = 10;
                break;

            default:
                break;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
