using System.Collections.Generic;
using System.Security;
using UnityEngine;

public static class MatchChecker
{

    public static List<Candy> candyList = new();
    public static List<Candy> matchedCandy = new();

    /// <summary>
    /// Check if either of the swapped candy is part of a connected row.
    /// </summary>
    /// <param name="selectedCandy"></param>
    /// <param name="candyToSwap"></param>
    /// <returns>List of all the matching candy</returns>
    public static List<Candy> CheckForMatchingCandy(Candy selectedCandy, Candy candyToSwap)
    {
        if (selectedCandy == null || candyToSwap == null) return null;

        if (IsPartOfMatchingRow(selectedCandy, candyToSwap.currentCoordinates))
        {
            matchedCandy.Add(selectedCandy);
        }

        if (IsPartOfMatchingRow(candyToSwap, selectedCandy.currentCoordinates))
        {
            matchedCandy.Add(candyToSwap);
        }

        matchedCandy.ForEach(candy => Debug.Log(candy.name + " + "));
        return matchedCandy;
    }

    /// <summary>
    /// Checks if given candy is in a matching row of at least 3 candy with the same CandyType
    /// </summary>
    /// <param name="originCandy"></param>
    /// <param name="newCoordinates"></param>
    /// <returns></returns>
    private static bool IsPartOfMatchingRow(Candy originCandy, Vector2 newCoordinates)
    {
        List<Candy> horizontalMatches = new();
        List<Candy> verticalMatches = new();

        bool horizontalMatch = CheckDirection(originCandy, newCoordinates, Vector2.right, horizontalMatches) + CheckDirection(originCandy, newCoordinates, Vector2.left, horizontalMatches) >= 2;

        if (horizontalMatch)
            matchedCandy.AddRange(horizontalMatches);

        bool verticalMatch = CheckDirection(originCandy, newCoordinates, Vector2.up, verticalMatches) + CheckDirection(originCandy, newCoordinates, Vector2.down, verticalMatches) >= 2;

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
    private static int CheckDirection(Candy originCandy, Vector2 newCoordinates, Vector2 direction, List<Candy> matches)
    {
        int count = 0;
        Vector2 checkLocation = newCoordinates;

        if (originCandy.currentCoordinates - newCoordinates == direction)
        {
            return count;
        }

        while (true)
        {
            checkLocation += direction;

            Candy matchingCandy = GetCandyAtLocation(checkLocation);
            if (matchingCandy != null && matchingCandy.candyType == originCandy.candyType)
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
            if (candy.currentCoordinates == location)
            {
                return candy;
            }
        }

        return null;
    }
}
