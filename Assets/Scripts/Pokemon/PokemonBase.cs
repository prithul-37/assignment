using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create New Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite forntSprite;
    [SerializeField] Sprite backSprite;


    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;


    //base stats
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMoves> learnableMoves;

    public string Name
    {
        get { return name; }
    }
    public string Description { get { return description; } }
    public PokemonType PokemonType1 { get { return type1; } }
    public PokemonType PokemonType2 { get { return type2; } }
    public int MaxHP { get { return maxHP; } }
    public int Attack { get { return attack; } }
    public int Defence { get { return defence; } }
    public int SpAttack { get { return spAttack; } }
    public int SpDefence { get { return spDefence; } }
    public int Speed { get { return speed; } }
    public Sprite ForntSprite { get { return forntSprite; } }
    public Sprite BackSprite { get { return backSprite; } }
    public List<LearnableMoves> LearnableMoves { get { return learnableMoves; } }


}

[System.Serializable]
public class LearnableMoves
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base { get { return moveBase; } }
    public int Level { get { return level; } }
}
public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}


public class TypeChart
{

    static float[][] chart = new float[][]
    {
    // NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI
    new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },  // NOR
    new float[] { 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f },  // FIR
    new float[] { 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f },  // WAT
    new float[] { 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f },  // ELE
    new float[] { 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f },  // GRA
    new float[] { 1f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f },  // ICE
    new float[] { 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f },  // FIG
    new float[] { 1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f },  // POI
    };




    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];
    }
}