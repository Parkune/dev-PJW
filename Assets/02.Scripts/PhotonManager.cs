using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;       // ���� ������ �⺻ ���ӽ����̽�
using Photon.Realtime;  // ���� �ھ���� ���ӽ����̽�
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // ���� ���� ����
    private readonly string gameVersion = "v1.0";
    // �÷��̾��� �г���    
    private string userId = "AvatarName";

    public TMP_InputField userIdText;
    public TMP_Dropdown userSelectChanel;


    // �� ����� �����ϱ� ���� ��ųʸ� �ڷ���
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

    // �� ������
    public GameObject roomPrefab;

    // ���� ������ ���ӿ�����Ʈ�� Transform
    public Transform scrollContent;

    //UI SETACTIVE ��Ʈ���� ���ؼ� �Լ� ���
    public GameObject AlbumPanel;
    public GameObject LobbyList;

    //������ �ٹ��� �Ǻ��� int �Լ�
    [SerializeField]
    int AlbumNum = 0;


    TypedLobby BTS = new TypedLobby("BTS", LobbyType.Default);
    TypedLobby AESPA = new TypedLobby("Aespa", LobbyType.Default);


    private void Awake()
    {
        // ���� ���� ����
        PhotonNetwork.GameVersion = gameVersion;
        // �÷��̾��� �г��� ����
        PhotonNetwork.NickName = userId;

        // ������ ȣ����(�ε���) ���� �ڵ����� �ε��ϴ� ���
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected == false)
        {
            // ���� ������ ����
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 5):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
        ShowPanel(AlbumPanel);
    }

    #region PHOTON_CALLBACK
    // ���漭���� �������� �� ȣ��Ǵ� �ݹ��Լ�

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server !!!");
        // �� ����� �����ϱ� ���� �κ�� ����
    }

    void ShowPanel(GameObject CurPanel)
    {
        // �ʱ� ��ư���� �ٹ� ���� �� ȭ�� 
        AlbumPanel.SetActive(false);
        // �ٹ� ���� �� ��µǴ� ���� ��� ����Ʈ
       // AlbumMusicList.SetActive(false);
        // �ٹ� ���� �� ���� �κ񸮽�Ʈ UI
        LobbyList.SetActive(false);
        //�ʱ� Ŀ���͸���¡/�ٹ� ���� �� �� �� �ִ� UI
      //  LobbyButton.SetActive(false);

        CurPanel.SetActive(true);
    }



    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        print("�κ� ����Ʈ �޾ƿԽ��ϴ�.");
        
        
    }


    //�ٹ� ���� �� �θ� �̺�Ʈ��
    public void GoBTS()
    {
        PhotonNetwork.JoinLobby(BTS);
        print("BTS �����ߴ�");
        ShowPanel(LobbyList);
        AlbumNum = 1;
    }
    public void GoAESPA()
    {
        PhotonNetwork.JoinLobby(AESPA);
        print("Aespa �����ߴ�");
        ShowPanel(LobbyList);
        AlbumNum = 2;
    }

    // �κ� ����(����)���� �� ȣ��Ǵ� �ݹ�
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� �������� !!!");

    }

    // ������ �����ϴ� �ݹ��Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (RoomInfo room in roomList)
        {
            //Debug.Log($"�� �̸�={room.Name}, �����ڼ�={room.PlayerCount}/{room.MaxPlayers}");
            // �� ���� ���θ� Ȯ��
            if (room.RemovedFromList == true)
            {
                // ��ųʸ����� ����, Room �����յ� ����
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                // ó�� ������ ���� ���
                // ��ųʸ��� ������ �߰�, Room �������� Content ���ӿ�����Ʈ ���� ����
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    //�� ������ ����
                    GameObject _room = Instantiate(roomPrefab, scrollContent);

                    // ��ųʸ��� ����
                    roomDict.Add(room.Name, _room);

                    // �� ������ ����
                    _room.GetComponent<RoomData>().RoomInfo = room;
                }
                else // �� ������ ����
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
    Debug.Log($"Join failed {returnCode}={message}");

 /*   // ���� �Ӽ��� ����
    RoomOptions ro = new RoomOptions();
    ro.IsOpen = true;
    ro.IsVisible = true;
    ro.MaxPlayers = 2;

    // �� ����
    PhotonNetwork.CreateRoom("MyRoom", ro);*/
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� �Ϸ�");
    }

    // �濡 ������ �� ȣ��Ǵ� �ݹ��Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� �Ϸ�");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        // ��Ʈ��ũ ��ũ�� ����
        // PhotonNetwork.Instantiate("Tank", new Vector3(0, 2.0f, 0), Quaternion.identity, 0);

        if (PhotonNetwork.IsMasterClient)
        {
            if (AlbumNum == 1 )
            {
                PhotonNetwork.LoadLevel("BTS");
                AlbumNum = 0;
            }
            else if (AlbumNum == 2)
            { 
                PhotonNetwork.LoadLevel("Aespa");
                AlbumNum = 0;
            }
        }
    }

    #endregion

    #region USER_DEFINE_CALLBACK

    public void OnChangeUserId()
    {
        PlayerPrefs.SetString("USER_ID", userIdText.text);
    }

    public void OnLoginClick()
    {
        if (string.IsNullOrEmpty(userIdText.text))
        {
            userId = $"USER_{Random.Range(0, 20):00}";
            userIdText.text = userId;
        }

        PlayerPrefs.SetString("USER_ID", userIdText.text);

        RoomOptions ro = new RoomOptions();
        ro.IsOpen = ro.IsVisible = true;
        ro.MaxPlayers = 2;


        string text = userSelectChanel.options[userSelectChanel.value].text; ;
        PhotonNetwork.NickName = userIdText.text;
       // PhotonNetwork.CreateRoom(text, ro);
       if (AlbumNum == 1)
        { 

        PhotonNetwork.JoinOrCreateRoom(text + "BTS", ro, BTS);

        } else if(AlbumNum == 2)
        {

        PhotonNetwork.JoinOrCreateRoom(text + "AESPA", ro, AESPA);

        }
        //  PhotonNetwork.JoinOrCreateRoom(PhotonNetwork.NickName, ro);

        //roomNameText.text = $"ROOM_{ }";

    }




    /* public void OnMakeRoomClick()
     {
         // �� �Ӽ��� ����
         RoomOptions ro = new RoomOptions();
         ro.IsOpen = ro.IsVisible = true;
         ro.MaxPlayers = 2;



         if (string.IsNullOrEmpty(roomNameText.text))
         {
             roomNameText.text = $"ROOM_{Random.Range(0, 1000):000}";
         }

         PhotonNetwork.NickName = userIdText.text;
         // �� ����
         PhotonNetwork.CreateRoom(roomNameText.text, ro);
     }*/

    #endregion

}