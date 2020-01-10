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
    public int MaxHealth = 0;
    public int HealingPoints = 0;
    public bool Blinded = false;
    public int BlindDuration = 0;


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


    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
