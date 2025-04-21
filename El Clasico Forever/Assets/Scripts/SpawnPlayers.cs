using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minZ = 13;
    public float maxZ = -13;

    private void Start()
    {
        Vector3 randomPosition = new Vector3(0, 1, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }
}
