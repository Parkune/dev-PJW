using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;       // 포톤 엔진의 기본 네임스페이스
using Photon.Realtime;  // 포톤 코어엔진의 네임스페이스
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 게임 버전 설정
    private readonly string gameVersion = "v1.0";
    // 플레이어의 닉네임    
    private string userId = "AvatarName";

    public TMP_InputField userIdText;
    public TMP_Dropdown userSelectChanel;


    // 룸 목록을 저장하기 위한 딕셔너리 자료형
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

    // 룸 프리팹
    public GameObject roomPrefab;

    // 룸이 생성될 게임오브젝트의 Transform
    public Transform scrollContent;

    //UI SETACTIVE 컨트롤을 위해서 함수 등록
    public GameObject AlbumPanel;
    public GameObject LobbyList;

    //선택한 앨범을 판별할 int 함수
    [SerializeField]
    int AlbumNum = 0;


    TypedLobby BTS = new TypedLobby("BTS", LobbyType.Default);
    TypedLobby AESPA = new TypedLobby("Aespa", LobbyType.Default);


    private void Awake()
    {
        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        // 플레이어의 닉네임 지정
        PhotonNetwork.NickName = userId;

        // 방장이 호출한(로드한) 씬을 자동으로 로딩하는 기능
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected == false)
        {
            // 포톤 서버에 접속
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
    // 포톤서버에 접속했을 때 호출되는 콜백함수

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server !!!");
        // 룸 목록을 수신하기 위해 로비로 접속
    }

    void ShowPanel(GameObject CurPanel)
    {
        // 초기 버튼에서 앨범 선택 시 화면 
        AlbumPanel.SetActive(false);
        // 앨범 선택 시 출력되는 수록 목록 리스트
       // AlbumMusicList.SetActive(false);
        // 앨범 선택 후 나올 로비리스트 UI
        LobbyList.SetActive(false);
        //초기 커스터마이징/앨범 선택 등 할 수 있는 UI
      //  LobbyButton.SetActive(false);

        CurPanel.SetActive(true);
    }



    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        print("로비 리스트 받아왔습니다.");
        
        
    }


    //앨범 선택 시 부를 이벤트들
    public void GoBTS()
    {
        PhotonNetwork.JoinLobby(BTS);
        print("BTS 접속했다");
        ShowPanel(LobbyList);
        AlbumNum = 1;
    }
    public void GoAESPA()
    {
        PhotonNetwork.JoinLobby(AESPA);
        print("Aespa 접속했다");
        ShowPanel(LobbyList);
        AlbumNum = 2;
    }

    // 로비에 입장(접속)했을 때 호출되는 콜백
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 입장했음 !!!");

    }

    // 룸목록을 수신하는 콜백함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach (RoomInfo room in roomList)
        {
            //Debug.Log($"룸 이름={room.Name}, 접속자수={room.PlayerCount}/{room.MaxPlayers}");
            // 룸 삭제 여부를 확인
            if (room.RemovedFromList == true)
            {
                // 딕셔너리에서 삭제, Room 프리팹도 삭제
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                // 처음 생성된 룸인 경우
                // 딕셔너리에 데이터 추가, Room 프리팹을 Content 게임오브젝트 하위 생성
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    //룸 프리팹 생성
                    GameObject _room = Instantiate(roomPrefab, scrollContent);

                    // 딕셔너리에 저장
                    roomDict.Add(room.Name, _room);

                    // 룸 정보를 저장
                    _room.GetComponent<RoomData>().RoomInfo = room;
                }
                else // 룸 정보를 갱신
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

 /*   // 룸의 속성을 설정
    RoomOptions ro = new RoomOptions();
    ro.IsOpen = true;
    ro.IsVisible = true;
    ro.MaxPlayers = 2;

    // 룸 생성
    PhotonNetwork.CreateRoom("MyRoom", ro);*/
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료");
    }

    // 방에 입장한 후 호출되는 콜백함수
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        // 네트워크 탱크를 생성
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
         // 룸 속성을 설정
         RoomOptions ro = new RoomOptions();
         ro.IsOpen = ro.IsVisible = true;
         ro.MaxPlayers = 2;



         if (string.IsNullOrEmpty(roomNameText.text))
         {
             roomNameText.text = $"ROOM_{Random.Range(0, 1000):000}";
         }

         PhotonNetwork.NickName = userIdText.text;
         // 룸 생성
         PhotonNetwork.CreateRoom(roomNameText.text, ro);
     }*/

    #endregion

}