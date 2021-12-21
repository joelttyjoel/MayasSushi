using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderStuff;

[CreateAssetMenu(fileName = "LevelInformation", menuName = "LevelInformation", order = 1)]
public class LevelInformation : ScriptableObject
{
    [Header("0 = ris_avocado_avocado")]
    [Header("1 = ris_räka_räka")]
    [Header("2 = ris_gurka_gurka")]
    [Header("3 = ris_lax_lax")]
    [Header("4 = ris_ägg_ägg")]
    [Header("5 = ris_avocado_avocado_raka")]
    [Header("6 = ris_lax_lax_raka")]
    [Header("7 = ris_ägg_ägg_lax")]
    [Header("8 = ris_gurka_gurka_gurka")]
    [Header("9 = ris_raka_avodaco_ägg")]

    [Header("Make sure to update both this one and boardmanager one")]
    public List<PieceToCreate> piecesToCreate;

    public int scoreMultiplierxLevel;
    public string GameSceneName;
    public List<Level> levels;

    [ContextMenu("CalculateScores")]
    private void CalculateScores()
    {
        foreach (Level a in levels)
        {
            int levelScore = 0;
            foreach (Customer b in a.customers)
            {
                foreach (int c in b.itemsInOrderIndexInBoardManager)
                {
                    levelScore += (piecesToCreate[c].pieceLevel0to3 + 1) * scoreMultiplierxLevel;
                }
            }

            a.score = levelScore;
        }
    }
}

[System.Serializable]
public class Level
{
    public int score;
    public List<Customer> customers;
    public List<int> boardPercentages;
}

[System.Serializable]
public struct PieceToCreate
{
    public int pieceLevel0to3;
    public Sprite sprite;
    public List<GameObject> ingredients;

    public PieceToCreate(int inLevel, Sprite InSprite, List<GameObject> InIngredients)
    {
        pieceLevel0to3 = inLevel;
        sprite = InSprite;
        ingredients = InIngredients;
    }
}