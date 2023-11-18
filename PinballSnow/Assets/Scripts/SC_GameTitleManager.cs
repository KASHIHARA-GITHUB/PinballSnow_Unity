using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// ゲームタイトルマネージャ
public class SC_GameTitleManager : MonoBehaviourPunCallbacks {
    // スクリプト変数
    [SerializeField] private SC_Room SC_Room = null;            // ルーム
    [SerializeField] private SC_GameSound SC_GameSound = null;  // ゲームサウンド

    // コンポーネント変数
    [SerializeField] private Text CP_PlayerNameText = null;     // プレイヤー名テキスト
    [SerializeField] private Text CP_WarningText = null;        // 警告テキスト

    // 定数
    private const int TITLEBGM = 0;  // BGM
    private const int NAMELENGTHMIN = 0;  // プレイヤー名文字列最小
    private const int NAMELENGTHMAX = 11; // プレイヤー名文字列最大
    private const string CONNECTMSG = "相手を探しています";   // 接続メッセージ
    private const string WARNINGMSG = "1文字以上10文字以下で入力してください"; // 警告メッセージ

    /// <summary> 
    /// ゲームタイトル開始処理
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void Start() {
        SC_GameSound.Select(TITLEBGM);
    }

    /// <summary> 
    /// テキストからプレイヤー名取得
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public bool PlayerNameSet() {
        // プレイヤー名取得
        string w_PlayerName = CP_PlayerNameText.text;

        //名前の文字数が1文字以上10文字以内であれば相手を探す
        if (w_PlayerName.Length > NAMELENGTHMIN && 
            w_PlayerName.Length < NAMELENGTHMAX) {
            // 接続メッセージを表示
            CP_WarningText.text = CONNECTMSG;
            SC_Room.Connecting(w_PlayerName);

            return true;
        }

        //警告メッセージを表示
        CP_WarningText.text = WARNINGMSG;

        return false;
    }
}