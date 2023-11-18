using Photon.Pun;
using UnityEngine;

// ネットワークボールスクリプト
public class SC_NetworkBall : MonoBehaviourPunCallbacks, IPunObservable {
    // ボールオプション変数
    private struct Ball {
        public string Name;
        public Color Color;
        public float Y_Direct;
        public int Layer;

        public Ball(string p1, Color p2, float p3, int p4) {
            this.Name     = p1;
            this.Color    = p2;
            this.Y_Direct = p3;
            this.Layer    = p4;
        }
    }

    // ネットワークボールの設定値
    private Ball[] Balls = {
            new Ball ( "NetworkBall1" ,new Color(255f, 100f, 0f), -1f, 6),
            new Ball ( "NetworkBall2" ,new Color(255f, 0f, 100f), 1f, 7),
    };

    // ネットワークボールの体力
    private int GB_BallHp = 3;

    /// <summary> 
    /// ネットワークボールのRigidBody2Dを同期
    /// </summary> 
    /// <param name="pm_Stream">送受信メッセージ</param> 
    /// <param name="pm_Info"  >通信情報</param>
    /// <returns>なし</returns>
    void IPunObservable.OnPhotonSerializeView(PhotonStream pm_Stream, PhotonMessageInfo pm_Info) {
        PhotonNetwork.SendRate = 100; // 1秒間にメッセージ送信を行う回数
        PhotonNetwork.SerializationRate = 100; // 1秒間にオブジェクト同期を行う回数

        if (pm_Stream.IsWriting) {
            //他のプレイヤーに値を送信
            pm_Stream.SendNext(transform.GetComponent<Rigidbody2D>().velocity);
        } else {
            //他のプレイヤーから送信された値を受信
            transform.GetComponent<Rigidbody2D>().velocity = (Vector2)pm_Stream.ReceiveNext();
        }
    }

    /// <summary> 
    /// ネットワークボールの初期化
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Initialize(Vector2 pm_PlayerPosition) {
        if (photonView.IsMine) {
            // プレイヤー番号を取得
            int w_PlayerNumber = photonView.OwnerActorNr - 1;

            // ネットワークボールを設定同期
            photonView.RPC(nameof(InitializeRPC), RpcTarget.All, w_PlayerNumber, pm_PlayerPosition);
        }
    }

    /// <summary> 
    /// ネットワークボールの初期化同期
    /// </summary> 
    /// <param name="pm_PlayerNumber">プレイヤー番号</param> 
    /// <param name="pm_PlayerPosition">プレイヤー場所</param> 
    /// <returns>なし</returns>
    [PunRPC]
    private void InitializeRPC(int pm_PlayerNumber, Vector2 pm_PlayerPosition) {
        // コンポーネント変数宣言
        Transform      w_Parent;
        SpriteRenderer w_Sprite;

        // 親オブジェクト[NetworkBalls]配下に設置
        w_Parent = GameObject.Find("NetworkBalls").transform;
        transform.SetParent(w_Parent);

        // ネットワークボールに設定
        w_Sprite = GetComponent<SpriteRenderer>();
        w_Sprite.color     = Balls[pm_PlayerNumber].Color;
        gameObject.name    = Balls[pm_PlayerNumber].Name;
        gameObject.layer   = Balls[pm_PlayerNumber].Layer;
        transform.localPosition = pm_PlayerPosition;
        Vector2 w_BallVector = new Vector2(Random.Range(-0.7f, 0.7f), Balls[pm_PlayerNumber].Y_Direct);
        transform.GetComponent<Rigidbody2D>().velocity = w_BallVector.normalized * 10;
    }

    /// <summary> 
    /// ネットワークボール衝突処理
    /// </summary> 
    /// <param name="pm_Collision">衝突オブジェクト</param> 
    /// <returns>なし</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // ネットワークブロックの衝突時だけ行う
        if (pm_Collision.gameObject.name == "NetworkBlock(Clone)") {
            //ボール削除を同期
            photonView.RPC(nameof(DestroyRPC), RpcTarget.All);
            return;
        }

        // ネットワークボールの体力減少
        GB_BallHp--;
        if (GB_BallHp < 1) {
            //ボール削除を同期
            photonView.RPC(nameof(DestroyRPC), RpcTarget.All);
        }
    }

    /// <summary> 
    /// ネットワークボールの削除同期
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    [PunRPC]
    private void DestroyRPC() {
        Destroy(gameObject);
    }
}
