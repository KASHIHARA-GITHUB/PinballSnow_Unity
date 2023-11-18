using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

// ネットワークマネージャ
public class SC_NetworkManager : MonoBehaviourPunCallbacks {
    // スクリプト変数
    [SerializeField] private GameManager SC_GameManager = null;
    [SerializeField] private SC_GameSE   SC_GameSE = null;
    private SC_NetworkPlayer SC_CompNetworkPlayer = null;

    // コンポーネント変数
    [SerializeField] private Text CP_BallHoldText = null;

    // グローバル変数
    private Vector2 GB_NetPlayerPos = new Vector2 (0, 0);      // ネットワークオブジェクトの位置
    private int  GB_NetBallShotCount = 0;                      // ネットワークボールの発射数
    private bool GB_BallShotAllowFlag = true;                  // ネットワークボール発射許可フラグ
    private bool GB_BallShotStopFlag = false;                  // ネットワークボール発射中断フラグ
    private bool GB_BattleJudgeShowFlag = false;               // 勝敗画面表示フラグ
    private const int BALLMAX = 1000;                       // ネットワークボールの発射最大数
    private const float BALLSHOTDURING = 0.1f;                 // ネットワークボールの発射間隔

    /// <summary> 
    /// ネットワークマネージャー初期化処理
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Initialize() {
        // ネットワークボール保持数を初期化
        CP_BallHoldText.text = "0";

        // ネットワークプレイヤー通知初期化
        SC_NetObjectInform.SetEventInform();
    }

    /// <summary> 
    /// ネットワークプレイヤー生成
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void PlayerCreate() {
        // ネットワークプレイヤーの生成
        GameObject w_Player = 
            PhotonNetwork.Instantiate("NetworkPlayer", GB_NetPlayerPos, Quaternion.identity);
        SC_CompNetworkPlayer = w_Player.GetComponent<SC_NetworkPlayer>();
        SC_CompNetworkPlayer.Initialize();

        // ネットワークプレイヤー衝突通知受信
        SC_NetObjectInform.GB_JudgeStatus.Subscribe(
            w_JudgeStatus => CustomResult(w_JudgeStatus)
        );
    }

    /// <summary> 
    /// カスタムプロパティから衝突結果を取得
    /// </summary> 
    /// <param name="pm_JudgeStatus">プレイヤー位置、勝敗状態</param> 
    /// <returns>なし</returns>
    private void CustomResult(SC_NetObjectInform.BattleJudge pm_JudgeStatus) {
        //ボールショット
        GB_BallShotStopFlag = true;
        GB_BattleJudgeShowFlag = true;
        SC_GameManager.BattleJudge(pm_JudgeStatus);
    }

    /// <summary> 
    /// ネットワークボールを発射
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void BallShot() {
        // ボールを発射直後は再発射不可にする
        if(GB_BallShotAllowFlag) {
            // ボールをショット時にボール保持加算を防止
            GB_BallShotAllowFlag = false;

            //ボールショット
            StartCoroutine(BallShotCor(GB_NetBallShotCount));
        }
    }

    /// <summary> 
    /// ネットワークボール生成
    /// </summary> 
    /// <param name="pm_BallShotCount">ネットワークボール発射数</param> 
    /// <returns>なし</returns>
    private IEnumerator BallShotCor(int pm_BallShotCount) {
        //ネットワークボールのオブジェクト宣言
        GameObject     w_Ball;
        SC_NetworkBall w_CompBall;

        // ボールを投げるときの発声効果音
        int w_Rnd = Random.Range(1, 10);
        if (w_Rnd < 4) { SC_GameSE.Attack1(); }
        else if (w_Rnd < 7) { SC_GameSE.Attack2(); }
        else {  SC_GameSE.Attack3(); }

        // ボールを発射
        for (int i = 1; i <= pm_BallShotCount; i++) {
            // ボールを投げる音
            SC_GameSE.ThrowBall();

            // ネットワークボールを生成
            Vector3 w_PlayerPosition = SC_CompNetworkPlayer.GetPosition();
            w_Ball = PhotonNetwork.Instantiate("NetworkBall", w_PlayerPosition, Quaternion.identity);
            w_CompBall = w_Ball.GetComponent<SC_NetworkBall>();
            w_CompBall.Initialize(w_PlayerPosition);

            // ネットワークボール保持テキストを更新
            CP_BallHoldText.text = $"{pm_BallShotCount - i}";

            // 0.1秒待機
            yield return new WaitForSeconds(BALLSHOTDURING);

            // ボール発射中断
            if (GB_BallShotStopFlag) { break; }
        }

        // ボール発射可能状態に更新
        GB_NetBallShotCount  = 0;
        GB_BallShotAllowFlag = true;
    }

    /// <summary> 
    /// ネットワークボール保持
    /// </summary> 
    /// <param name="pm_BallHoldCount">ネットワークボール保持数</param> 
    /// <returns>なし</returns>
    public void BallHold(int pm_BallHoldStatus) {
        // ネットワークボール発射時は無効
        if (GB_BallShotAllowFlag == true) {
            // ネットワークボール増殖を更新 
            if (pm_BallHoldStatus == 1) {
                // ネットワークボールを加算
                GB_NetBallShotCount = GB_NetBallShotCount + 1;
            } else {
                // ネットワークボールを2倍にする
                GB_NetBallShotCount = GB_NetBallShotCount * 2;
            }

            // ボールがMAXより超えない
            if (GB_NetBallShotCount > BALLMAX) {
                GB_NetBallShotCount = BALLMAX;
            }

            // 現在のボール保持数をテキストに反映
            CP_BallHoldText.text = $"{GB_NetBallShotCount}";
        }
    }

    /// <summary> 
    /// 退室処理(コールバック関数)
    /// </summary> 
    /// <param name="pm_OtherPlayer">他プレイヤー</param> 
    /// <returns>なし</returns>
    public override void OnPlayerLeftRoom(Player pm_OtherPlayer) {
        // ログの確認
        Debug.Log($"{pm_OtherPlayer.NickName}が退出しました");

        //勝敗決定後は退出パネルは表示しない
        if (!GB_BattleJudgeShowFlag) {
            SC_GameManager.LocalBallDelete();
            SC_GameManager.LeftRoom();
        }
    }
}

// ネットワークオブジェクト通知スクリプト
public static class SC_NetObjectInform {
    // グローバル変数
    public struct BattleJudge {
        public Vector2 LosePos;
        public bool WinFlag;
    }

    public static Subject<BattleJudge> GB_JudgeStatus = null;

    /// <summary> 
    /// ネットワークオブジェクト通知初期化
    /// </summary> 
    /// <param name="pm_Player">プレイヤー情報</param> 
    /// <returns>なし</returns>
    public static void SetEventInform() {
        // 勝敗をネットワークマネージャに返す
        GB_JudgeStatus = new Subject<BattleJudge>();
    }

    /// <summary> 
    /// ネットワークプレイヤーから勝敗判定を取得
    /// </summary> 
    /// <param name="pm_Player">プレイヤー情報</param> 
    /// <param name="pm_WinFlag">プレイヤー勝敗(true:勝ち,false:負け)</param>
    /// <param name="pm_PlayerPos">プレイヤー位置</param>
    /// <returns>なし</returns>
    public static void BattleJudgeInform(this Player pm_Player, bool pm_WinFlag, Vector2 pm_PlayerPos) {
        // 勝敗結果変数
        BattleJudge w_BattleJudgeStatus;
        w_BattleJudgeStatus.WinFlag = pm_WinFlag;
        w_BattleJudgeStatus.LosePos = pm_PlayerPos;

        // NetworkManagerに通知
        GB_JudgeStatus.OnNext(w_BattleJudgeStatus);
        GB_JudgeStatus.OnCompleted();
    }
}
