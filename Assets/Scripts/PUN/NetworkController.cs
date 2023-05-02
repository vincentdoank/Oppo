using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace WTI.PUN
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        public static NetworkController Instance { get; private set; }

        public int GetClientId()
        {
            return PhotonNetwork.LocalPlayer.ActorNumber;   
        }

        private void Start()
        {
            Instance = this;
            Connect();

            EventManager.onGetIsConnected += GetIsConnected;
            EventManager.onConnectToNetwork += Connect;
            EventManager.onCreateRoom += CreateRoom;
            EventManager.onJoinRoom += JoinRoom;
            EventManager.onLeaveRoom += ExitRoom;
        }

        private void OnDestroy()
        {
            EventManager.onConnectToNetwork -= Connect;
            EventManager.onCreateRoom -= CreateRoom;
            EventManager.onJoinRoom -= JoinRoom;
            EventManager.onLeaveRoom -= ExitRoom;
        }

        private bool GetIsConnected()
        {
            return PhotonNetwork.IsConnected;
        }

        public void Connect()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public void CreateRoom()
        {
            int rand = Random.Range(1000, 10000);
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
            PhotonNetwork.CreateRoom(rand.ToString(), roomOptions);
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }


        public void ExitRoom()
        {
            PhotonNetwork.LeaveRoom(PhotonNetwork.IsMasterClient);
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer)
            {

            }
            else
            {

            }
        }

        #region Photon

        public override void OnConnected()
        {

            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {

            }
            else
            {

            }
        }

        public override void OnCreatedRoom()
        {

        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            if (!PhotonNetwork.IsConnected)
            {

            }
            else
            {

            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {

        }

        public override void OnJoinedLobby()
        {
            if (!PhotonNetwork.IsConnected)
            {

            }

            else
            {

            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (!PhotonNetwork.IsConnected)
            {

            }

            else
            {

            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("player joined : " + newPlayer.ActorNumber);

            if (PhotonNetwork.CountOfPlayers == 2)
            {
                OnDevicePaired();
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            if (otherPlayer.IsMasterClient)
            {
                ExitRoom();
            }
        }

        #endregion

        private void OnDevicePaired()
        {

        }
    }
}
