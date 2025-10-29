using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    enum PuzzleType { Enemy, Pattern }

    [SerializeField] PuzzleType puzzleType;

    [SerializeField] GameObject platformToSpawn, obstacleToDelete;

    [SerializeField] GameObject[] enemiesToDefeat;
    [SerializeField] GameObject[] patternTiles;
    [SerializeField] int[] correctPatternIndices;

    bool puzzleCompleted = false;

    void Awake()
    {
        // For pattern puzzles, populate tiles from this GameObject's children
        // Only populate if the inspector array is empty to avoid overwriting manual assignments.
        if (puzzleType == PuzzleType.Pattern)
        {
            if (patternTiles == null || patternTiles.Length == 0)
            {
                int childCount = transform.childCount;
                if (childCount == 0)
                {
                    Debug.LogWarning($"{name}: Pattern puzzle has no child tiles.");
                    patternTiles = new GameObject[0];
                }
                else
                {
                    patternTiles = new GameObject[childCount];
                    for (int i = 0; i < childCount; i++)
                    {
                        patternTiles[i] = transform.GetChild(i).gameObject;
                    }
                }
            }
        }

        // For enemy puzzles, populate enemiesToDefeat from this GameObject's children
        // Only populate if the inspector array is empty to avoid overwriting manual assignments.
        if (puzzleType == PuzzleType.Enemy)
        {
            if (enemiesToDefeat == null || enemiesToDefeat.Length == 0)
            {
                int childCount = transform.childCount;
                if (childCount == 0)
                {
                    Debug.LogWarning($"{name}: Enemy puzzle has no child enemies.");
                    enemiesToDefeat = new GameObject[0];
                }
                else
                {
                    enemiesToDefeat = new GameObject[childCount];
                    for (int i = 0; i < childCount; i++)
                    {
                        enemiesToDefeat[i] = transform.GetChild(i).gameObject;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (puzzleCompleted) return;

        if (enemiesToDefeat.Length > 0)
        {
            bool allDefeated = true;
            foreach (var enemy in enemiesToDefeat)
            {
                if (enemy != null)
                {
                    allDefeated = false;
                    break;
                }
            }
            if (allDefeated)
            {
                CompletePuzzle();
            }
        }
        
        if (PatternTilesMatchDestroyedState())
        {
           CompletePuzzle();
        }
    }

    bool PatternTilesMatchDestroyedState()
    {
        // Basic validation
        if (patternTiles == null || patternTiles.Length == 0) return false;
        if (correctPatternIndices == null || correctPatternIndices.Length == 0) return false;

        // Build a set of correct indices for fast lookup
        var correctSet = new HashSet<int>(correctPatternIndices);

        // Ensure all correct indices are valid and destroyed (null)
        foreach (var idx in correctSet)
        {
            if (idx < 0 || idx >= patternTiles.Length)
            {
                // invalid configuration: index out of range
                return false;
            }
            if (patternTiles[idx] != null)
            {
                // a correct tile is still present -> not completed yet
                return false;
            }
        }

        // Ensure that none of the other (non-correct) tiles have been destroyed
        for (int i = 0; i < patternTiles.Length; i++)
        {
            if (correctSet.Contains(i)) continue;

            // non-correct tiles must still exist (not destroyed)
            if (patternTiles[i] == null)
            {
                // a wrong tile was destroyed -> pattern invalid
                return false;
            }
        }

        // All correct tiles destroyed and all other tiles intact
        return true;
    }

    void CompletePuzzle()
    {
        puzzleCompleted = true;
        
        if (platformToSpawn != null)
        {
            platformToSpawn.SetActive(true);
        }
        if (obstacleToDelete != null)
        {
            Destroy(obstacleToDelete);
        }
    }
}
