using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 部屋スクリプト
public class SC_Room : MonoBehaviourPunCallbacks {
    // コンポーネント変数
    [SerializeField] private Text CP_WarningText = null;     // ワーニングテキスト

    // グローバル変数
    private bool GB_InRoom;         // 部屋に参加したら相手を探す処理を行う
    private bool GB_IsMatchingFlag; // 参加後はUpdate関数を処理しないようにする

    // 定数
    private const int PLAYERINROOMMAX = 2;    // 最大プレイヤー人数
    private const int PLAYERCONNETCTMAX = 20; // 最大接続人数

    /// <summary> 
    /// マスターサーバ接続
    /// </summary> 
    /// <param name="pm_PlayerName">プレイヤー名</param> 
    /// <returns>なし</returns>
    public void Connecting(string pm_PlayerName) {
        //プレイヤーの名前を登録
        PhotonNetwork.NickName = pm_PlayerName;

        //マスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary> 
    /// マスターサーバ接続完了後処理(コールバック関数)
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public override void OnConnectedToMaster() {
        // ゲームサーバーへランダムに接続する
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary> 
    /// ゲームサーバ接続完了後処理(コールバック関数)
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public override void OnJoinedRoom() {
        GB_InRoom = true;
    }

    /// <summary> 
    /// ルーム生成処理※ルームが見つからない場合(コールバック関数)
    /// </summary> 
    /// <param name="returnCode">コールバックコード</param>
    /// <param name="message"   >メッセージ</param> 
    /// <returns>なし</returns>
    public override void OnJoinRandomFailed(short returnCode, string message) {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = PLAYERINROOMMAX }, TypedLobby.Default);
    }

    /// <summary> 
    /// ルーム参加待ち
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void Update() {
        // マッチが成功したらUpdate関数を終了する
        if (GB_IsMatchingFlag) {
            return;
        }

        //部屋に参加したら下記の処理を行う
        if (GB_InRoom) {
            // 同時接続20人を超えた
            if(PhotonNetwork.CountOfPlayers > PLAYERCONNETCTMAX) {
                CP_WarningText.text = "接続人数が越えました";
                return;
            }

            // 2人接続出来たらシーン移動
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount) {
                Debug.Log("CountOfRooms " + PhotonNetwork.CountOfRooms);
                Debug.Log("CountOfPlayersOnMaster " + PhotonNetwork.CountOfPlayersOnMaster);
                Debug.Log("CountOfPlayersInRooms " + PhotonNetwork.CountOfPlayersInRooms);
                Debug.Log("CountOfPlayers " + PhotonNetwork.CountOfPlayers);

                // マッチングフラグをtrueにする
                GB_IsMatchingFlag = true;

                // 入室を不可にする
                PhotonNetwork.CurrentRoom.IsOpen = false;

                // ゲーム画面へ遷移
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}