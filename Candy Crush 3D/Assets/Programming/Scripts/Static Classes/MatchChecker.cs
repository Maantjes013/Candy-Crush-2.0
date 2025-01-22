using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MatchChecker
{
    public static List<Candy> candyList = new();
    public static List<Candy> matchedCandy = new();

    private static readonly List<Candy> horizontalMatches = new();
    private static readonly List<Candy> verticalMatches = new();

    /// <summary>
    /// Check if either of the swapped candy is part of a connected row and remove duplicate items from match list.
    /// </summary>
    /// <param name="selectedCandy"></param>
    /// <param name="candyToSwap"></param>
    /// <returns>List of all the matching candy</returns>
    public static List<Candy> CheckForMatchingCandy(Candy selectedCandy, Candy candyToSwap)
    {
        if (selectedCandy == null || candyToSwap == null) return null;

        SpecialCandyType newSpecialCandyType;
        if (IsPartOfMatchingRow(selectedCandy))
        {
            newSpecialCandyType = CheckForCandyUpgrade(selectedCandy);

            if (!newSpecialCandyType.Equals(SpecialCandyType.None))
                selectedCandy.ChangeSpecialCandyType(newSpecialCandyType);
        }

        if (IsPartOfMatchingRow(candyToSwap))
        {
            newSpecialCandyType = CheckForCandyUpgrade(candyToSwap);

            if (!newSpecialCandyType.Equals(SpecialCandyType.None))
                candyToSwap.ChangeSpecialCandyType(newSpecialCandyType);
        }

        CheckForDestroyedSpecialCandy();

        matchedCandy = matchedCandy.Distinct().ToList();
        return matchedCandy;
    }

    public static void CheckForDestroyedSpecialCandy()
    {
        List<Candy> specialCandyList = matchedCandy.FindAll(x => !x.specialCandyType.Equals(SpecialCandyType.None));
        if (specialCandyList.Count > 0)
        {
            specialCandyList.ForEach(candy =>
            {
                matchedCandy.AddRange(candy.specialCandyManager.ActivateSpecialEffect(candy));
            });
        }
    }

    /// <summary>
    /// Checks if given candy is in a matching row of at least 3 candy with the same CandyType
    /// </summary>
    /// <param name="originCandy"></param>
    /// <param name="newCoordinates"></param>
    /// <returns></returns>
    private static bool IsPartOfMatchingRow(Candy originCandy)
    {
        horizontalMatches.Clear();
        verticalMatches.Clear();

        bool horizontalMatch = CheckDirection(originCandy, Vector2.right, horizontalMatches) + CheckDirection(originCandy, Vector2.left, horizontalMatches) >= 2;

        if (horizontalMatch)
        {
            matchedCandy.AddRange(horizontalMatches);
        }

        bool verticalMatch = CheckDirection(originCandy, Vector2.up, verticalMatches) + CheckDirection(originCandy, Vector2.down, verticalMatches) >= 2;

        if (verticalMatch)
            matchedCandy.AddRange(verticalMatches);

        return horizontalMatch || verticalMatch;
    }

    /// <summary>
    /// Checks all candy in a given direction from the current direction. Stops when a different CandyType is found.
    /// </summary>
    /// <param name="originCandy"></param>
    /// <param name="newCoordinates"></param>
    /// <param name="direction"></param>
    /// <param name="typeToMatch"></param>
    /// <returns>Amount of candy with same CandyType in given direction</returns>
    private static int CheckDirection(Candy originCandy, Vector2 direction, List<Candy> matches)
    {
        int count = 0;
        Vector2 checkLocation = originCandy.CurrentCoordinates;

        while (true)
        {
            checkLocation += direction;

            Candy matchingCandy = GetCandyAtLocation(checkLocation);
            if (matchingCandy != null && matchingCandy.CandyType == originCandy.CandyType)
            {
                matches.Add(matchingCandy);
                count++;
            }

            else break;
        }

        return count;
    }

    private static Candy GetCandyAtLocation(Vector2 location)
    {
        foreach (Candy candy in candyList)
        {
            if (candy.CurrentCoordinates == location)
            {
                return candy;
            }
        }

        return null;
    }

    private static SpecialCandyType CheckForCandyUpgrade(Candy selectedCandy)
    {
        int horizontalCount = horizontalMatches.Count;
        int verticalCount = verticalMatches.Count;

        if (horizontalCount >= 4 || verticalCount >= 4)
            return SpecialCandyType.Bomb;

        if (horizontalCount == 3 && verticalCount == 3)
            return SpecialCandyType.VerticalAndHorizontal;

        if (horizontalCount == 3)
            return SpecialCandyType.Horizontal;

        if (verticalCount == 3)
            return SpecialCandyType.Vertical;

        matchedCandy.Add(selectedCandy);
        return SpecialCandyType.None;
    }
}
