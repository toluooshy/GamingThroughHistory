using Photon.Pun;
using UnityEngine;

public class RoomSync : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}
