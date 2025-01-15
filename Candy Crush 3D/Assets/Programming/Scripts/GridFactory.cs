using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridFactory : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    [SerializeField] private float distanceBetweenCandy;

    [Header("Settings")]
    [SerializeField] private Camera gridCamera;
    [SerializeField] private float animationTime;

    [Header("Candy Prefab To Spawn")]
    [SerializeField]
    private Candy candyPrefab;

    private bool isAnimationPlaying = false;

    private List<Vector2> spawnLocationList = new();

    private void Start()
    {
        PositionCamera();
        CreateGrid();
    }

    /// <summary>
    /// Sets position of camera in the middle of grid based on grid size and distance between candy. Also places camera at a proper position to view full grid no matter the size.
    /// </summary>
    private void PositionCamera()
    {
        int biggestVector = Mathf.Max(xSize, ySize);
        gridCamera.transform.SetPositionAndRotation(new Vector3((xSize - 1) * distanceBetweenCandy / 2, (ySize - 1) * distanceBetweenCandy / 2, -biggestVector - 1), Quaternion.identity);
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

    /// <summary>
    /// Spawn candy in correct position and assign corresponding values
    /// </summary>
    /// <param name="spawnCoordinates"></param>
    private void SpawnCandy(Vector2 spawnCoordinates)
    {
        Candy instantiatedCandy = Instantiate(candyPrefab, spawnCoordinates * distanceBetweenCandy, Quaternion.identity, transform);
        instantiatedCandy.CurrentCoordinates = spawnCoordinates;
        instantiatedCandy.CandyType = GetValidRandomCandyType(instantiatedCandy);

        MatchChecker.candyList.Add(instantiatedCandy);
    }

    /// <summary>
    /// Remove all CandyType that would form a match and pick a random one out of the remaining ones
    /// </summary>
    /// <param name="candy"></param>
    /// <returns></returns>
    private CandyType GetValidRandomCandyType(Candy candy)
    {
        Vector2 candyCoords = candy.CurrentCoordinates;
        CandyType[] allTypes = (CandyType[])System.Enum.GetValues(typeof(CandyType));
        List<CandyType> validTypes = new List<CandyType>(allTypes);

        validTypes.RemoveAll(type => CausesMatch(candyCoords, type));

        return validTypes[Random.Range(0, validTypes.Count)];
    }

    /// <summary>
    /// Checks if the random Candy Type causes a match of 3 candy to be formed. If so, other Candy Type should be created.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool CausesMatch(Vector2 coordinates, CandyType type)
    {
        int x = (int)coordinates.x;
        int y = (int)coordinates.y;

        // Check horizontal match
        if (MatchesType(x - 1, y, type) && MatchesType(x - 2, y, type)) return true;
        if (MatchesType(x - 1, y, type) && MatchesType(x + 1, y, type)) return true;
        if (MatchesType(x + 1, y, type) && MatchesType(x + 2, y, type)) return true;

        // Check vertical match
        if (MatchesType(x, y - 1, type) && MatchesType(x, y - 2, type)) return true;
        if (MatchesType(x, y - 1, type) && MatchesType(x, y + 1, type)) return true;
        if (MatchesType(x, y + 1, type) && MatchesType(x, y + 2, type)) return true;

        return false;
    }

    private bool MatchesType(int x, int y, CandyType type)
    {
        if (x < 0 || y < 0 || x >= xSize || y >= ySize) return false;

        Candy candy = MatchChecker.candyList.Find(c => c.CurrentCoordinates == new Vector2(x, y));
        return candy != null && candy.CandyType == type;
    }

    public void CheckCandySwap(Candy selectedCandy, Vector2 swapDirection)
    {
        if (isAnimationPlaying) 
            return;

        Candy candyToSwap = MatchChecker.candyList.SingleOrDefault(x => x.CurrentCoordinates.Equals(selectedCandy.CurrentCoordinates + swapDirection));

        if (candyToSwap == null)
            return;

        StartCoroutine(SwapCandy(selectedCandy, candyToSwap));
    }

    /// <summary>
    /// Swap Candy place. If match, destroy all matching candy. If not, swap candy back to original spot.
    /// </summary>
    /// <param name="selectedCandy"></param>
    /// <param name="candyToSwap"></param>
    /// <returns></returns>
    private IEnumerator SwapCandy(Candy selectedCandy, Candy candyToSwap)
    {
        isAnimationPlaying = true;

        yield return StartCoroutine(AnimateCandySwap(selectedCandy, candyToSwap));

        if (MatchChecker.CheckForMatchingCandy(selectedCandy, candyToSwap) != null && MatchChecker.matchedCandy.Count >= 3)
            DestroyCandy();
        else
            yield return StartCoroutine(AnimateCandySwap(selectedCandy, candyToSwap));

        isAnimationPlaying = false;
    }

    /// <summary>
    /// Destroy current candy and call method to spawn new ones in same position
    /// </summary>
    private void DestroyCandy()
    {
        spawnLocationList.Clear();

        MatchChecker.matchedCandy.ForEach(x =>
        {
            spawnLocationList.Add(x.CurrentCoordinates);
            MatchChecker.candyList.Remove(x);
            Destroy(x.gameObject);
        });

        spawnLocationList.ForEach(x =>
        {
            SpawnCandy(x);
        });

        MatchChecker.matchedCandy.Clear();
    }

    private IEnumerator AnimateCandySwap(Candy selectedCandy, Candy candyToSwap)
    {
        float elapsedTime = 0f;

        Vector2 startPositionA = selectedCandy.transform.position;
        Vector2 startPositionB = candyToSwap.transform.position;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationTime);

            selectedCandy.transform.position = Vector2.Lerp(startPositionA, startPositionB, t);
            candyToSwap.transform.position = Vector2.Lerp(startPositionB, startPositionA, t);

            yield return null;
        }

        selectedCandy.transform.SetPositionAndRotation(startPositionB, Quaternion.identity);
        candyToSwap.transform.SetPositionAndRotation(startPositionA, Quaternion.identity);

        Vector2 tempCoordinates = selectedCandy.CurrentCoordinates;
        selectedCandy.CurrentCoordinates = candyToSwap.CurrentCoordinates;
        candyToSwap.CurrentCoordinates = tempCoordinates;

        yield return new WaitForSeconds(animationTime);
    }
}
