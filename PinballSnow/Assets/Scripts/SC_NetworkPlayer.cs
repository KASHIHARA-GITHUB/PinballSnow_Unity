using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// ネットワークプレイヤー
public class SC_NetworkPlayer : MonoBehaviourPunCallbacks{
    // コンポーネント変数
    [SerializeField] private Text CP_PlayerNameText;        // プレイヤー名

    // グローバル変数
    private const bool WIN = true;       // 勝利
    private const bool LOSE = false;     // 敗北

    // プレイヤーのオプション変数
    private struct Player {
        public string Name;
        public Color Color;
        public Vector2 Position;
        public int Layer;

        public Player(string p1, Color p2, Vector2 p3, int p4) {
            this.Name    = p1;
            this.Color   = p2;
            this.Position = p3;
            this.Layer   = p4;
        }
    }

    private Player[] Players = {
            new Player ("NetworkPlayer1", new Color(255f, 100f, 0f), new Vector2(135.0f, 225.0f), 8 ),
            new Player ("NetworkPlayer2", new Color(255f, 0f, 100f), new Vector2(135.0f, -195.0f), 9 )
    };

    /// <summary> 
    /// プレイヤーを操作
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Update() {
        // 自身のネットワークプレイヤー
        if (photonView.IsMine) {
            // プレイヤーを移動
            MovePlayer();
        }
    }

    /// <summary> 
    /// プレイヤーを移動
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void MovePlayer() {
        //変数宣言
        float w_CurrentX = transform.localPosition.x;

        //移動最大距離範囲
        if (Input.GetKey(KeyCode.X)) {
            w_CurrentX = Mathf.Min(w_CurrentX + 2, 350);
        } else if (Input.GetKey(KeyCode.Z)) {
            w_CurrentX = Mathf.Max(w_CurrentX - 2, -50);
        }

        //バーの現在位置
        Vector3 w_Pos = new Vector3(w_CurrentX, transform.localPosition.y, 0);
        transform.localPosition = w_Pos;
    }

    /// <summary> 
    /// ネットワークプレイヤー初期化
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Initialize() {
        if (photonView.IsMine) {
            // プレイヤー番号
            int w_PlayerNum = photonView.OwnerActorNr - 1;

            // プレイヤー同期
            photonView.RPC(nameof(InitializeRPC), RpcTarget.All, w_PlayerNum);
        }
    }

    /// <summary> 
    /// ネットワークプレイヤー初期化同期
    /// </summary> 
    /// <param name="pm_PlayerNumber">プレイヤー番号</param> 
    /// <returns>なし</returns>
    [PunRPC]
    private void InitializeRPC(int pm_PlayerNumber) {
        // 変数宣言
        Transform w_Parent;
        SpriteRenderer w_Sprite;
        RectTransform w_Rect;

        // 自身のプレイヤー名を表示
        CP_PlayerNameText.text = $"{photonView.Owner.NickName}";

        // 親オブジェクト[NetworkPlayers]配下にプレイヤーを設置
        w_Parent = GameObject.Find("NetworkPlayers").transform;
        transform.SetParent(w_Parent);

        // コンポーネント取得
        w_Sprite = GetComponent<SpriteRenderer>();
        w_Rect   = GetComponent<RectTransform>();
        
        // 自身のプレイヤーに設定
        gameObject.name  = Players[pm_PlayerNumber].Name;
        gameObject.layer = Players[pm_PlayerNumber].Layer;
        w_Sprite.color   = Players[pm_PlayerNumber].Color;
        w_Rect.anchoredPosition = Players[pm_PlayerNumber].Position;
    }

    /// <summary> 
    /// プレイヤー位置を取得
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>プレイヤー位置取得</returns>
    public Vector3 GetPosition()
    {
        // 自身プレイヤーの位置
        return transform.localPosition;
    }

    /// <summary> 
    /// ネットワークプレイヤー衝突処理
    /// </summary> 
    /// <param name="pm_Collision">衝突オブジェクト</param> 
    /// <returns>なし</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // ネットワークプレイヤー以外の衝突はリターンする
        if (!(gameObject.name == "NetworkPlayer1" || gameObject.name == "NetworkPlayer2"))
            return;

        // ネットワークボールと自身プレイヤーの衝突位置を取得
        Vector2 w_PlayerPos = transform.localPosition;

        if (photonView.IsMine) {
            // 勝敗判定
            PhotonNetwork.LocalPlayer.BattleJudgeInform(LOSE, w_PlayerPos);             // 敗北を[SC_NetObjectInform]に通知
            photonView.RPC(nameof(JudgeRPC), RpcTarget.OthersBuffered, WIN, w_PlayerPos); // 勝利を自身以外に同期     
        } else {
            // 勝敗判定
            PhotonNetwork.LocalPlayer.BattleJudgeInform(WIN, w_PlayerPos);               // 勝利を[SC_NetObjectInform]に通知
            photonView.RPC(nameof(JudgeRPC), RpcTarget.OthersBuffered, LOSE, w_PlayerPos); // 敗北を自身以外に同期
        }
        photonView.RPC(nameof(DestroyRPC), RpcTarget.All); // 自プレイヤーの削除を全員に同期
    }

    /// <summary> 
    /// 勝敗を他プレイヤーに表示
    /// </summary> 
    /// <param name="pm_WinFlag"  >勝利フラグ(true:勝ち false:負け)</param> 
    /// <param name="pm_PlayerPos">プレイヤー位置</param> 
    /// <returns>なし</returns>
    [PunRPC]
    private void JudgeRPC(bool pm_WinFlag, Vector2 pm_PlayerPos) {
        // 勝利を[SC_NetObjectInform]に通知
        PhotonNetwork.LocalPlayer.BattleJudgeInform(pm_WinFlag, pm_PlayerPos);
    }

    /// <summary> 
    /// ネットワークプレイヤー削除同期
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    [PunRPC]
    private void DestroyRPC() {
        // プレイヤーを削除
        Destroy(gameObject);
    }
}