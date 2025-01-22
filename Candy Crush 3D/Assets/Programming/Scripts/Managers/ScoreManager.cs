using TMPro;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private int startingScore;

    private int currentScore;

    private void Awake()
    {
        scoreText.SetText(startingScore.ToString());
        currentScore = startingScore;
    }
    public void UpdateScore(int destroyedCandy)
    {
        currentScore += destroyedCandy;
        scoreText.SetText(currentScore.ToString());
    }
}
