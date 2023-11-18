using Photon.Pun;
using UnityEngine;

// ネットワークブロック
public class SC_NetworkBlock : MonoBehaviourPunCallbacks {
    // 定数
    private const int PLAYERONE = 8;
    private const int PLAYERTWO = 9;

    /// <summary> 
    /// ネットワークブロックの初期化
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Initialize() {
        // 自身の場合処理
        if (photonView.IsMine) {
            // ブロックのオプション同期
            photonView.RPC(nameof(InitializeRPC), RpcTarget.All);
        }
    }

    /// <summary> 
    /// ネットワークブロックの初期化同期
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    [PunRPC]
    private void InitializeRPC() {
        // 親オブジェクト[NetworkBlocks]配下に設置
        Transform w_Parent = GameObject.Find("NetworkBlocks").transform;
        transform.SetParent(w_Parent);
    }

    /// <summary> 
    /// ネットワークブロックの衝突処理
    /// </summary> 
    /// <param name="pm_Collision">衝突オブジェクト</param> 
    /// <returns>なし</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // ブロック衝突同期
        photonView.RPC(nameof(BlockCollisionRPC), RpcTarget.All, pm_Collision.gameObject.name);
    }

    /// <summary> 
    /// ネットワークブロック衝突時の同期
    /// </summary> 
    /// <param name="pm_Collision">衝突オブジェクト名</param> 
    /// <returns>なし</returns>
    [PunRPC]
    private void BlockCollisionRPC(string pm_Collision) {
        // スプライト変数
        SpriteRenderer w_SpriteRenderer;
        w_SpriteRenderer = GetComponent<SpriteRenderer>();

        // 衝突したネットワークボールの色に合わせる
        switch (pm_Collision) {
            case "NetworkBall1":
                w_SpriteRenderer.color = new Color(255f, 100f, 0f);
                gameObject.layer = PLAYERONE;
                break;
            case "NetworkBall2":
                w_SpriteRenderer.color = new Color(255f, 0f, 100f);
                gameObject.layer = PLAYERTWO;
                break;
            default:
                break;
        }
    }
}
