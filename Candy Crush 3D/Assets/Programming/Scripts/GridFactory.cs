using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridFactory : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    [SerializeField] private float distanceBetweenCandy;

    [Header("Candy Prefab To Spawn")]
    [SerializeField]
    private Candy candyPrefab;

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                SpawnCandy(new Vector2(x, y));
            }
        }
    }

    private void SpawnCandy(Vector2 spawnCoordinates)
    {
        Candy instantiatedCandy = Instantiate(candyPrefab, spawnCoordinates * distanceBetweenCandy, Quaternion.identity, transform);
        instantiatedCandy.currentCoordinates = spawnCoordinates;
        instantiatedCandy.candyType = GetValidRandomCandyType(instantiatedCandy);
        instantiatedCandy.OnCandyTypeChanged();
        instantiatedCandy.name = "Candy " + spawnCoordinates.ToString();

        MatchChecker.candyList.Add(instantiatedCandy);
    }

    // Ensures no row/column of 3 candies with the same type is formed
    private CandyType GetValidRandomCandyType(Candy candy)
    {
        CandyType randomType;
        randomType = (CandyType)Random.Range(0, System.Enum.GetValues(typeof(CandyType)).Length);
        return randomType;
    }

    public void CheckCandySwap(Candy selectedCandy, Vector2 swapDirection)
    {
        Candy candyToSwap = MatchChecker.candyList.SingleOrDefault(x => x.currentCoordinates.Equals(selectedCandy.currentCoordinates + swapDirection));
        if (MatchChecker.CheckForMatchingCandy(selectedCandy, candyToSwap) != null && MatchChecker.matchedCandy.Count >= 3)
        {
            //Destroy all candy
            SetNewPositions(selectedCandy, candyToSwap);

            MatchChecker.matchedCandy.ForEach(x =>
            {
                MatchChecker.candyList.Remove(x);
                Destroy(x.gameObject);
            });

            MatchChecker.matchedCandy.Clear();
            return;
        }

        Debug.Log("Cannot Swap!");
    }

    private void SetNewPositions(Candy selectedCandy, Candy candyToSwap)
    {
        Vector2 tempPosition = selectedCandy.transform.position;
        Vector2 tempCoordinates = selectedCandy.currentCoordinates;

        selectedCandy.transform.SetPositionAndRotation(candyToSwap.transform.position, Quaternion.identity);
        candyToSwap.transform.SetPositionAndRotation(tempPosition, Quaternion.identity);

        selectedCandy.currentCoordinates = candyToSwap.currentCoordinates;
        candyToSwap.currentCoordinates = tempCoordinates;
    }
}
