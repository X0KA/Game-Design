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
        Mage
    }
    public string name = "DefaultName";
    public CharacterClass charclass = CharacterClass.None;
    public int Health = 0;
    public int Damage = 0;
    public int Level = 1;
    public int Speed = 0;
    public int MaxHealth = 0;
    public bool Blinded = false;
    public int BlindDuration = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
