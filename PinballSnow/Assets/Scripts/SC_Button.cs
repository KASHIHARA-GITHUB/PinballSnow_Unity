using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ボタンスクリプト
public class SC_Button : MonoBehaviourPunCallbacks
{
    //スクリプト変数
    [SerializeField] private SC_GameTitleManager SC_GameTitleManager;   // ゲームタイトルマネージャ
    [SerializeField] private SC_GameSE SC_GameSE;                       // ゲーム効果音

    /// <summary> 
    /// ボタンクリック処理
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void OnClick() {
        // ボタン名確認
        Debug.Log(transform.name);

        // 各ボタンの処理
        switch (transform.name) {
            case "GameStartButton":
                // プレイヤー名チェック
                PlayerNameCheck();
                break;
            case "MainButton":
                // ネットワーク切断
                PhotonNetwork.Disconnect();
                break;
            default:
                break;
        }
    }

    /// <summary> 
    /// プレイヤー名チェック
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void PlayerNameCheck() {
        // プレイヤー名チェック
        if (SC_GameTitleManager.PlayerNameSet()) {
            SC_GameSE.GameStart();  // ゲームスタート効果音
            GetComponent<Button>().interactable = false; // ボタンを非活性
        } else {
            SC_GameSE.Warning();    // 警告メッセージ効果音
        }
    }

    /// <summary> 
    /// タイトルシーンへ遷移(マスターサーバ切断後のコールバック)
    /// </summary> 
    /// <param name="pm_Disconnect">マスターサーバ切断原因</param> 
    /// <returns>なし</returns>
    public override void OnDisconnected(DisconnectCause pm_Disconnect) {
        // タイトルシーンへ遷移
        SceneManager.LoadScene("TitleScene");
    }
}
