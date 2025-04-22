using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject ballPrefab;

    public int homeScore = 0;
    public int awayScore = 0;

    public GameObject homeLogoObject;
    public GameObject awayLogoObject;
    public GameObject homeNameObject;
    public GameObject awayNameObject;
    public GameObject homeScoreObject;
    public GameObject awayScoreObject;
    public GameObject countdownObject;    // Countdown display (e.g. "3", "2", "1", "GO")
    public GameObject clockObject;        // Clock UI in mm:ss format
    public GameObject winnerUIObject;     // Display when the game ends
    public GameObject winnerText;         // Text element inside winnerUIObject

    public Vector3 player1Position = new Vector3(0, 1.25f, 11);
    public Vector3 player2Position = new Vector3(0, 1.25f, -11);
    public Vector3 ballSpawnPosition = new Vector3(0, 1, 0);
    public Material[] playerSkins = new Material[2];

    private static int chosenSkinIndex = -1;
    private bool ballSpawned = false;

    public int minutes = 90;
    public int seconds = 0;
    private bool gameRunning = false;

    private void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        // Only master client spawns ball once both players are in
        if (!ballSpawned && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine(BeginMatchCountdown());
            ballSpawned = true;
        }

        // If game is running, tick the timer
        if (gameRunning)
        {
            TickGameClock();
        }
    }

    private void SpawnPlayer()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        Vector3 spawnPosition = playerCount == 1 ? player1Position : player2Position;

        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        // Assign team skins
        if (playerObj.TryGetComponent(out Renderer renderer))
        {
            int skinIndex;

            if (playerCount == 1)
            {
                chosenSkinIndex = Random.Range(0, 2);  // First player picks randomly
                skinIndex = chosenSkinIndex;
            }
            else
            {
                skinIndex = 1 - chosenSkinIndex;       // Second gets the other
            }

            if (playerSkins.Length >= 2 && playerSkins[skinIndex] != null)
            {
                renderer.material = playerSkins[skinIndex];
            }
        }
    }

    private IEnumerator BeginMatchCountdown()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;

        Text countdownText = countdownObject.GetComponent<Text>();
        countdownObject.SetActive(true);

        string[] countdown = { "3", "2", "1", "GO!" };

        for (int i = 0; i < countdown.Length; i++)
        {
            countdownText.text = countdown[i];
            yield return new WaitForSeconds(1f);
        }

        countdownObject.SetActive(false);
        PhotonNetwork.Instantiate(ballPrefab.name, ballSpawnPosition, Quaternion.identity);
        gameRunning = true;
    }

    private float fastClockTimer = 0f;
    private void TickGameClock()
    {
        // Simulate 60 game seconds per real second
        fastClockTimer += Time.deltaTime * 60f;

        if (fastClockTimer >= 1f)
        {
            fastClockTimer = 0f;
            seconds--;

            if (seconds < 0)
            {
                seconds = 59;
                minutes--;
            }

            UpdateClockDisplay();

            if (minutes < 0)
            {
                EndGame();
            }
        }
    }


    private void UpdateClockDisplay()
    {
        string timeStr = string.Format("{0:00}:{1:00}", Mathf.Max(0, minutes), Mathf.Max(0, seconds));
        clockObject.GetComponent<Text>().text = timeStr;
    }

    public void UpdateScore(bool isHomeGoal)
    {
        if (isHomeGoal)
            awayScore++;
        else
            homeScore++;

        homeScoreObject.GetComponent<Text>().text = homeScore.ToString();
        awayScoreObject.GetComponent<Text>().text = awayScore.ToString();
    }

    private void EndGame()
    {
        gameRunning = false;
        winnerUIObject.SetActive(true);

        string winner = homeScore > awayScore ? "Home Wins!" :
                        awayScore > homeScore ? "Away Wins!" :
                        "It's a Draw!";

        winnerText.GetComponent<Text>().text = winner;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: {newPlayer.NickName}");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
