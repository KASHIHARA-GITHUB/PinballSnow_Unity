using UniRx;
using UnityEngine;
using UnityEngine.UI;

// ローカルボールスクリプト
public class SC_LocalBall : MonoBehaviour {
    // オブジェクト変数
    [SerializeField] private GameObject OB_Arrow = null;          // ローカルボールの矢印            

    // グローバル変数
    public Subject<ScoreBlockInform> GB_ScoreBlockCollision =
                new Subject<ScoreBlockInform>();     // ブロックが衝突時にゲームマネージャに返す

    // スコアブロック(GameManagerに通知)
    public struct ScoreBlockInform {
        public int Status;
        public Vector2 Position;
    }

    private int   GB_CollisionCount = 0;       // 衝突回数
    private Vector2 GB_InMousePos;             // マウスを押した時の位置取得
    private Vector2 GB_OutMousePos;            // マウスを離した時の位置取得

    /*************** 定数 **************/
    private const int PINBALLLAYER = 12;                          // ピンボールレイヤー
    private const int BALLSHOTLAYER = 11;                         // ボールショットレイヤー
    private const int NOTHINGPARA = -1;                           // 空のパラメータ
    private const int BALLSHOTAREAINDEX = 1;                      // ボールショットエリアインデックス
    private const int COLORMAX = 255;                             // カラー最大値
    private const float COLLISIONMAX = 10.0f;                     // 衝突最大数(衝突を一定数以上超えると発射エリアに戻る)
    private const float ARROWSIZEX = 30.0f;                       // 矢印のサイズX
    private const float ARROWMAXSIZEY = 120.0f;                   // 矢印のMAXサイズY
    private const float ARROWMINSIZEY = 70.0f;                    // 矢印のMINサイズY
    private const float SHOTRAITO = 10.0f;                        // 発射倍率
    private const float SHOTX = 0.0f;                             // 発射X
    private Vector2 BALLSHOTAREA = new Vector2(-377.5f, -227.5f); // ボールショットエリアに位置移動
    private Vector2 PINBALLUPAREA = new Vector3(-278f, 240f);     // パチンコエリア２に位置移動

    // スコアブロック(衝突変数)
    private struct ScoreBlockCollision {
        public string BlockName;
        public int    ToArea;
        public int    Status;

        public ScoreBlockCollision(string p1, int p2, int p3) {
            this.BlockName = p1;
            this.ToArea = p2;
            this.Status = p3;
        }
    }

    // スコアブロック衝突後の状態
    private ScoreBlockCollision[] SCOREBLOCKCOL = {
        new ScoreBlockCollision("Block1-1", 1, 1),
        new ScoreBlockCollision("Block1-2", 2, 1),
        new ScoreBlockCollision("Block1-3", 2, 1),
        new ScoreBlockCollision("Block1-4", 2, 1),
        new ScoreBlockCollision("Block1-5", 1, 1),
        new ScoreBlockCollision("Block2-1", 2, 1),
        new ScoreBlockCollision("Block2-2", 2, 3),
        new ScoreBlockCollision("Block2-3", 2, 1),
        new ScoreBlockCollision("Block2-4", 2, 2),
        new ScoreBlockCollision("Block2-5", 2, 4),
    };
    /*************** 定数 **************/

    /// <summary> 
    /// 初期化
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Initialize() {
        //矢印を非表示
        OB_Arrow.SetActive(false);
    }

    /// <summary> 
    /// ローカルボール衝突処理
    /// </summary> 
    /// <param name="pm_Collision">衝突オブジェクト</param> 
    /// <returns>なし</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // ボールの位置とボール状態変数初期化
        int w_ToArea = NOTHINGPARA;
        int w_BallStatus = NOTHINGPARA;

        // 衝突したスコアブロックの位置を取得
        Vector2 w_ColPosX = pm_Collision.gameObject.transform.localPosition;

        // スコアブロックに当たった場合、ボールの状態を変更
        for (int i = 0; i < SCOREBLOCKCOL.Length; i++) {
            if (SCOREBLOCKCOL[i].BlockName == pm_Collision.gameObject.name) {
                w_ToArea = SCOREBLOCKCOL[i].ToArea;
                w_BallStatus = SCOREBLOCKCOL[i].Status;
                break;
            }
        }

        // スコアブロック以外の衝突はリターンする
        if ( w_ToArea == NOTHINGPARA) { return; }

        // ボールが一定数以上衝突した場合処理する
        w_ToArea = BallDamage(w_ToArea);

        // ボールの宛先移動
        if (w_ToArea == BALLSHOTAREAINDEX) { ToBallShotArea(); }  // ボール発射エリアに移動
        else { ToPinballUpArea(); }                              // パチンコ上段エリアに移動

        // ボールの状態を更新
        StatusSet(w_BallStatus, w_ColPosX);
    }

    /// <summary> 
    /// ローカルボール体力減少
    /// </summary> 
    /// <param name="pm_ToArea">宛先位置</param> 
    /// <returns>なし</returns>
    private int BallDamage(int pm_ToArea) {
        // スコアブロックとボール衝突回数
        GB_CollisionCount++;

        // 衝突するたびに赤色に増していく
        float w_Color = COLORMAX - (COLORMAX * (GB_CollisionCount / COLLISIONMAX));
        gameObject.GetComponent<Image>().color = new Color32(COLORMAX, (byte)w_Color, (byte)w_Color, COLORMAX);

        // スコアブロックが指定回数以上衝突した場合ボール発射エリアに移動
        if (GB_CollisionCount > COLLISIONMAX) {
            GB_CollisionCount = 0;
            pm_ToArea = BALLSHOTAREAINDEX;
        }

        return pm_ToArea;
    }

    /// <summary> 
    /// ボール状態更新
    /// </summary> 
    /// <param name="pm_BallStatusNumber">ボールの状態番号</param> 
    /// <param name="pm_BlockColPos"　   >ブロックの衝突位置</param> 
    /// <returns>なし</returns>
    private void StatusSet(int pm_BallStatusNumber, Vector2 pm_BlockColPos) {
        // 衝突ステータス
        ScoreBlockInform w_BlockCollisionEvent;
        w_BlockCollisionEvent.Status = pm_BallStatusNumber;
        w_BlockCollisionEvent.Position = pm_BlockColPos;

        // GmaeManagerに結果を返す
        GB_ScoreBlockCollision.OnNext(w_BlockCollisionEvent);
    }

    /// <summary> 
    /// パチンコエリア上段に移動
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void ToPinballUpArea() {
        // ピンボールエリア上段に移動
        RectTransform w_LocalBallPos = GetComponent<RectTransform>();
        w_LocalBallPos.anchoredPosition = PINBALLUPAREA;
    }

    /// <summary> 
    /// ボールショットエリアに移動
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void ToBallShotArea() {
        // ローカルボールのコンポーネント変数
        RectTransform w_LocalBallPos = GetComponent<RectTransform>();
        Rigidbody2D w_LocalBallRigid = GetComponent<Rigidbody2D>();

        // ボールショットエリアのレイヤーに変更
        gameObject.layer = BALLSHOTLAYER;

        // ボールショットエリアにボールを移動
        w_LocalBallPos.anchoredPosition = BALLSHOTAREA;

        // RigidBody2Dを初期化
        w_LocalBallRigid.velocity = Vector2.zero;
        w_LocalBallRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        w_LocalBallRigid.constraints = RigidbodyConstraints2D.None;
    }

    /// <summary> 
    /// マウスIN処理(コールバック関数)
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void OnMouseDown() {
        // パチンコエリアに移動したらマウスで触れない
        if (gameObject.layer != PINBALLLAYER) {
            //マウスを押した位置を取得
            GB_InMousePos = Input.mousePosition;
        }
    }

    /// <summary> 
    /// マウスUP処理(コールバック関数)
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void OnMouseUp() {
        // パチンコエリアに移動したらマウスで触れない
        if (gameObject.layer != PINBALLLAYER) {
            // ローカルボールを発射
            BallShot();
        }
    }

    /// <summary> 
    /// マウスDRAG処理(コールバック関数)
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void OnMouseDrag() {
        // パチンコエリアに移動したらマウスで触れない
        if (gameObject.layer != PINBALLLAYER) {
            // マウスのドラッグ位置を取得 & 離した位置を取得
            GB_OutMousePos = Input.mousePosition;
            BallArrow();
        }
    }

    /// <summary> 
    /// ローカルボールショット
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void BallShot() {

        // 矢印を非表示
        OB_Arrow.SetActive(false);

        // ボール弾きのボールの弾きを計算
        float w_BallLengthY = (GB_InMousePos.y - GB_OutMousePos.y) * SHOTRAITO;

        // ボールの発射力を設定
        if (w_BallLengthY > ARROWMAXSIZEY * SHOTRAITO) {
            w_BallLengthY = ARROWMAXSIZEY * SHOTRAITO;
        } else if (w_BallLengthY < ARROWMINSIZEY * SHOTRAITO) {
            return;
        }

        // レイヤーを変更
        gameObject.layer = PINBALLLAYER;

        // ボールを発射
        transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(SHOTX, w_BallLengthY));
    }

    /// <summary> 
    /// ローカルボールの矢印表示
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void BallArrow() {
        // 矢印の長さを計算
        float w_ArrowLengthY = (GB_InMousePos.y - GB_OutMousePos.y);

        // 矢印を表示
        if (w_ArrowLengthY >= ARROWMINSIZEY) {
            OB_Arrow.SetActive(true);
        } else {
            OB_Arrow.SetActive(false);
        }

        // 矢印に長さを反映
        if (w_ArrowLengthY > ARROWMAXSIZEY) {
            OB_Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(ARROWSIZEX, ARROWMAXSIZEY);
        } else if (w_ArrowLengthY >= ARROWMINSIZEY) {
            OB_Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(ARROWSIZEX, w_ArrowLengthY);
        }
    }
}
