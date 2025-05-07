using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Collections;

public class GameSceneController : MonoBehaviourPunCallbacks
{
    private PhotonView view;
    public GameObject playerPrefab;
    public GameObject ballPrefab;

    public int homeScore = 0;
    public int awayScore = 0;

    public GameObject homeScoreObject;
    public GameObject awayScoreObject;
    public GameObject countdownObject;
    public GameObject clockObject;
    public GameObject winnerUIObject;
    public GameObject winnerText;

    public Vector3 player1Position = new Vector3(0, 1.25f, 11);
    public Vector3 player2Position = new Vector3(0, 1.25f, -11);

    public int homeSkin;
    public int awaySkin;
    public bool ballSpawned = false;

    public int minutes = 90;
    public int seconds = 0;
    private bool gameRunning = false;
    private bool isReturningToMenu = false;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable matchData = new ExitGames.Client.Photon.Hashtable
            {
                { "homeScore", homeScore },
                { "awayScore", awayScore },
                { "gameRunning", gameRunning },
                { "minutes", minutes },
                { "seconds", seconds },
                { "ballSpawned", ballSpawned },
                { "matchStarted", false },
                { "winnerDeclared", false },
                { "winnerMessage", ""}
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(matchData);
        }
    }

    private void Update()
    {
        if (!gameRunning && PhotonNetwork.IsMasterClient && !ballSpawned && AllPlayersReady())
        {
            StartCoroutine(BeginMatchCountdown());
            ballSpawned = true;
            UpdateMatchProperty("ballSpawned", true);
        }

        if (minutes <= 0 || minutes + seconds <= 0)
        {
            minutes = 0;
            seconds = 0;
            EndGame();
        }
    }

    private bool AllPlayersReady()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.InRoom;
    }

    private void SpawnPlayer()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Vector3 spawnPosition = playerCount == 1 ? player1Position : player2Position;

        if (PhotonNetwork.IsMasterClient && playerCount == 1)
        {
            homeSkin = Random.Range(0, 2);
            awaySkin = homeSkin == 0 ? 1 : 0;

            ExitGames.Client.Photon.Hashtable skinUpdate = new ExitGames.Client.Photon.Hashtable
            {
                { "homeSkin", homeSkin },
                { "awaySkin", awaySkin }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(skinUpdate);

            SpawnAndSetSkin(spawnPosition, homeSkin);
        }
        else
        {
            StartCoroutine(WaitForSkinPropertiesAndSpawn(spawnPosition));
        }
    }

    private IEnumerator WaitForSkinPropertiesAndSpawn(Vector3 spawnPosition)
    {
        int tries = 0;
        while (tries < 20)
        {
            ExitGames.Client.Photon.Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

            if (props.ContainsKey("homeSkin") && props.ContainsKey("awaySkin"))
            {
                homeSkin = (int)props["homeSkin"];
                awaySkin = (int)props["awaySkin"];

                int skinIndex = PhotonNetwork.IsMasterClient ? homeSkin : awaySkin;
                SpawnAndSetSkin(spawnPosition, skinIndex);
                yield break;
            }

            tries++;
            yield return new WaitForSeconds(0.1f);
        }

        Debug.LogError("Failed to receive skin data in time.");
    }

    private GameObject SpawnAndSetSkin(Vector3 spawnPosition, int skinIndex)
    {
        object[] instantiationData = new object[] { skinIndex };
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity, 0, instantiationData);

        player.tag = PhotonNetwork.IsMasterClient ? "HomePlayer" : "AwayPlayer";
        return player;
    }

    [PunRPC]
    private void ShowCountdown()
    {
        Debug.Log("ShowCountdown RPC received by: " + PhotonNetwork.NickName);
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        Text countdownText = countdownObject.GetComponent<Text>();
        countdownObject.SetActive(true);

        string[] countdown = { "3", "2", "1", "GO!" };

        foreach (string t in countdown)
        {
            countdownText.text = t;
            yield return new WaitForSeconds(1f);
        }

        countdownObject.SetActive(false);
    }

    private IEnumerator BeginMatchCountdown()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;

        yield return new WaitForSeconds(1f); // Let the other client load

        photonView.RPC("ShowCountdown", RpcTarget.All);

        yield return new WaitForSeconds(4f); // Countdown duration

        PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0, 1, -.25f), Quaternion.identity);

        gameRunning = true;
        UpdateMatchProperty("gameRunning", true);
        UpdateMatchProperty("matchStarted", true);

        StartCoroutine(TickClockRoutine()); // Start the clock here
    }

    private IEnumerator TickClockRoutine()
    {
        while (gameRunning)
        {
            yield return new WaitForSeconds(.005f);

            seconds--;
            if (seconds < 0)
            {
                seconds = 59;
                minutes--;
            }

            ExitGames.Client.Photon.Hashtable clockUpdate = new ExitGames.Client.Photon.Hashtable
            {
                { "minutes", minutes },
                { "seconds", seconds }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(clockUpdate);

            UpdateClockDisplay(); // Master updates own UI
        }
    }

    private void UpdateClockDisplay()
    {
        string timeStr = $"{Mathf.Max(0, minutes):00}:{Mathf.Max(0, seconds):00}";
        clockObject.GetComponent<Text>().text = timeStr;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("minutes"))
            minutes = (int)propertiesThatChanged["minutes"];
        if (propertiesThatChanged.ContainsKey("seconds"))
            seconds = (int)propertiesThatChanged["seconds"];

        UpdateClockDisplay();

        if (propertiesThatChanged.ContainsKey("homeScore"))
            homeScore = (int)propertiesThatChanged["homeScore"];
        if (propertiesThatChanged.ContainsKey("awayScore"))
            awayScore = (int)propertiesThatChanged["awayScore"];

        UpdateScoreDisplay();

        // Show winner UI only when game has ended and message is now present
        bool winnerDeclared = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("winnerDeclared") &&
                              (bool)PhotonNetwork.CurrentRoom.CustomProperties["winnerDeclared"];

        if (winnerDeclared && propertiesThatChanged.ContainsKey("winnerMessage"))
        {
            string message = (string)propertiesThatChanged["winnerMessage"];
            winnerUIObject.SetActive(true);
            winnerText.GetComponent<Text>().text = message;
        }
    }

    public void IncrementHomeScore()
    {
        homeScore++;
        UpdateMatchProperty("homeScore", homeScore);
    }

    public void IncrementAwayScore()
    {
        awayScore++;
        UpdateMatchProperty("awayScore", awayScore);
    }

    private void UpdateScoreDisplay()
    {
        homeScoreObject.GetComponent<Text>().text = homeScore.ToString();
        awayScoreObject.GetComponent<Text>().text = awayScore.ToString();
    }

    private void EndGame()
    {
        gameRunning = false;
        ExitGames.Client.Photon.Hashtable endProps = new ExitGames.Client.Photon.Hashtable
        {
            { "gameRunning", false },
            { "winnerDeclared", true }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(endProps);

        if (PhotonNetwork.IsMasterClient)
        {
            string winnerMessage = homeScore > awayScore ? "Real Madrid Wins!" : homeScore < awayScore ? "FC Barcelona Wins!" : "It's A Draw!";
            ExitGames.Client.Photon.Hashtable winnerProps = new ExitGames.Client.Photon.Hashtable
            {
                { "winnerMessage", winnerMessage }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(winnerProps);
        }
    }

    private void UpdateMatchProperty(string key, object value)
    {
        ExitGames.Client.Photon.Hashtable update = new ExitGames.Client.Photon.Hashtable { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(update);
    }

    public void ReturnToMenu()
    {
        isReturningToMenu = true;
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (isReturningToMenu)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
