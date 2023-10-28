using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

namespace WTI.NetCode
{

    /// <summary>
    /// FOR NETCODE
    /// </summary>
    public class NetworkController : MonoBehaviour
    {
        public TMPro.TMP_Text ipText;
        public NetworkTransport transport;

        public string errorMessage;

        public static NetworkController Instance { get; private set; }

        public ulong GetClientId()
        {
            return NetworkManager.Singleton.LocalClientId;
        }

        public int GetClientCount()
        {
            return NetworkManager.Singleton.ConnectedClients.Count;
        }

        private IEnumerator Start()
        {
            if (PlayerPrefs.HasKey("ip"))
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = PlayerPrefs.GetString("ip");
            }
            else
            {
                SceneManager.LoadScene("ConfigScreen");
            }
            Debug.LogWarning("ip : " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address);
            yield return null;
            Debug.Log("START");
            Instance = this;
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
            NetworkManager.Singleton.ConnectionApprovalCallback += OnCheckApprovalConnection;
            NetworkManager.Singleton.OnTransportFailure += OnFailed;
            
            EventManager.onGetIsConnected += GetIsConnected;
            EventManager.onUseCameraSelected += OnWebCameraUsed;

            yield return null;
            
            //EventManager.onConnectToNetwork += Connect;
            EventManager.onLeaveRoom += ExitRoom;

            Connect();


            //if (GameManager.Instance.IsServer)
            //{
            //    //Connect();
            //    GameManager.Instance.HideControlTypeDropdown();
            //}
            //else
            //{ 
            //    GameManager.Instance.ShowControlTypeDropdown();
            //}
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnected;
                NetworkManager.Singleton.ConnectionApprovalCallback -= OnCheckApprovalConnection;
                NetworkManager.Singleton.OnTransportFailure -= OnFailed;
            }
            EventManager.onConnectToNetwork -= Connect;
            EventManager.onLeaveRoom -= ExitRoom;
            EventManager.onUseCameraSelected -= OnWebCameraUsed;
        }

        //private void Log(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
        //{
        //    Debug.Log(eventType + " " + clientId);
        //}

        private bool GetIsConnected()
        {
            return NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer;
        }

        private void OnFailed()
        {
            errorMessage = "Failed";
        }

        public void Connect()
        {
            Debug.Log("Connect");
            try
            {
                if (GameManager.Instance.IsServer)
                //if(true)
                {
                    if (NetworkManager.Singleton.StartServer())
                    {
                        errorMessage = "Server connected";
                        EventManager.onNetworkConnected?.Invoke();
                        Debug.LogWarning("server started"); 
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (NetworkManager.Singleton.StartClient())
                    {
                        errorMessage = "client connected";
                        GameManager.Instance.CheckControlType();
                        EventManager.onNetworkConnected?.Invoke();
                        Debug.LogWarning("client started");
                    }
                    else
                    {
                        errorMessage = "failed";
                    }
                }
            }
            catch
            {

            }
        }


        public void ExitRoom()
        {
            Debug.Log("ExitRoom");
            try
            {
                //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId); //this is for kick a client
                NetworkManager.Singleton.Shutdown(true);

                //PhotonNetwork.LeaveRoom(PhotonNetwork.IsMasterClient);
                if (GameManager.Instance.IsServer)
                {
                    GameManager.Instance.HideControlTypeDropdown();
                }
                else
                {
                    GameManager.Instance.ShowControlTypeDropdown();
                }
            }
            catch (Exception exc)
            {

            }
        }

        #region NetCode

        private void OnCheckApprovalConnection(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse resp)
        {
            resp.Approved = (NetworkManager.Singleton.ConnectedClientsIds.Count < GameManager.Instance.maxUser + 1);
            Debug.LogWarning("Check Approval");
        }

        public void OnServerStarted()
        {
            Debug.LogWarning("OnServerStarted");
            //GameManager.Instance.ShowExitRoomButton();
            //((FootballController)GameMatchController.Instance).ApplyRole();

            //((FootballController)GameMatchController.Instance).SendPlayerData();
        }

        public void OnConnected(ulong clientId)
        {
            Debug.LogWarning("OnConnected : " + clientId + "  " + NetworkManager.Singleton.ConnectedClients.Count);
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                //GameManager.Instance.ShowExitRoomButton();

                OnDevicePaired();
                //((FootballController)GameMatchController.Instance).CheckState();
            }
            else if (GameManager.Instance.IsServer)
            {
                GameMatchController.Instance.SendPlayerData();
                if (NetworkManager.Singleton.ConnectedClients.Count > 2 && !GameMatchController.Instance.isStarted)
                {
                    GameMatchController.Instance.StartMatch();
                }
            }
            //((FootballController)GameMatchController.Instance).ApplyRole();
        }

        public void OnDisconnected(ulong clientId)
        {
            Debug.Log("Disconnect : " + clientId);
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                //GameManager.Instance.ShowReconnectButton();

            }
            if (NetworkManager.Singleton.IsServer)
            {
                GameMatchController.Instance.OnDisconnected(clientId);
                EventManager.onOtherPlayerDisconnected?.Invoke(GameManager.Instance.GetClientId(), clientId);
            }

            Connect();
        }

        public void OnApplicationFocus(bool focus)
        {
            if (NetworkManager.Singleton.IsConnectedClient)
            {
                Connect();
            }
        }

        public void OnJoinedRoom()
        {
        }

        public void OnWebCameraUsed()
        {
            //ipText.text = "WebCam open";
            //GameManager.Instance.ShowCameraTexture();
        }

        #endregion

        private void OnDevicePaired()
        {
            GameManager.Instance.HideControlTypeDropdown();
        }

        //private void OnGUI()
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.fontSize = 50;
        //    style.normal.textColor = Color.red;

        //    GUI.Label(new Rect(40, 100, 300, 60), "IP : " + ((UnityTransport)transport).ConnectionData.Address.ToString() + ":" + ((UnityTransport)transport).ConnectionData.Port + " " + ((UnityTransport)transport).ConnectionData.ServerListenAddress.ToString(), style);
        //    GUI.Label(new Rect(40, 160, 300, 60), "msg : " + errorMessage, style);

        //}
    }
}
