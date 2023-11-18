using UnityEngine;
using UniRx;

// ゲームマネージャ
public class GameManager : MonoBehaviour {
    // オブジェクト変数
    [SerializeField] private GameObject OB_LocalBallPrefab = null;          // ローカルボールプレファブ
    [SerializeField] private GameObject OB_UpScoreStatusAnimPrefab = null;  // ピンボール上のステータスプレファブ
    [SerializeField] private GameObject OB_DownScoreStatusAnimPrefab = null;// ピンボール下のステータスプレファブ
    [SerializeField] private GameObject OB_BomAnimPrefab = null;            // 爆発プレファブ
    [SerializeField] private GameObject OB_WinPanel = null;                 // 勝利パネル
    [SerializeField] private GameObject OB_LosePanel = null;                // 敗北パネル
    [SerializeField] private GameObject OB_LeftRoomPanel = null;            // 退室パネル

    // スクリプト変数
    [SerializeField] private SC_GameSound      SC_GameSound = null;         // ゲームサウンド
    [SerializeField] private SC_GameSE         SC_GameSE = null;            // ゲーム効果音
    [SerializeField] private SC_NetworkManager SC_NetworkManager = null;    // ネットワークマネージャ
    [SerializeField] private SC_BlockManager   SC_BlockManager = null;      // ブロックマネージャ

    // コンポーネント変数
    [SerializeField] private Transform CP_LocalBalls = null;                // ローカルボール親オブジェクト
    [SerializeField] private Transform CP_Animations = null;                // アニメーション親オブジェクト

    // 定数
    private const int   BATTLEBGM = 1;               // バトルBGM
    private const float PINBALLBORDER = -100.0f;     // ピンボール１とピンボール２のボーダー
    private const int   NETBALLCREATE = 1;           // ネットワークボールを生成
    private const int   NETBALLMULTICFRATE = 2;      // ネットワークボールを2倍にする
    private const int   LOCALBALLMAX = 10;           // ローカルボールの最大生成数
    private Vector2     LOCALBALLPOS = new Vector2(-375f, -230f); // ローカルボールの生成場所

    /// <summary> 
    /// ゲーム開始
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void Start() {
        // 初期化
        Initialize();
    }

    /// <summary> 
    /// 初期化処理
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void Initialize() {
        // スクリプト初期化
        SC_GameSound.Select(BATTLEBGM);     // BGMを再生
        SC_NetworkManager.Initialize();     // ネットワークマネージャの初期化
        SC_NetworkManager.PlayerCreate();   // ネットワークプレイヤー生成
        SC_BlockManager.Create();           // ネットワークブロック生成

        // ゲーム勝敗画面と退室画面を非表示
        OB_WinPanel.SetActive(false);
        OB_LosePanel.SetActive(false);
        OB_LeftRoomPanel.SetActive(false);

        // ローカルボール生成
        LocalBallCreate();
    }

    /// <summary> 
    /// ローカルボール生成
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void LocalBallCreate() {
        // ローカルボール生成オブジェクトを宣言
        GameObject   w_LocalBall;
        SC_LocalBall w_CompLocalBall;

        // ローカルボールの生成
        w_LocalBall = Instantiate(OB_LocalBallPrefab, CP_LocalBalls);
        w_LocalBall.GetComponent<RectTransform>().anchoredPosition = LOCALBALLPOS;
        w_CompLocalBall = w_LocalBall.GetComponent<SC_LocalBall>();
        w_CompLocalBall.Initialize();

        // スコアブロックに当たった時の処理
        w_CompLocalBall.GB_ScoreBlockCollision.Subscribe (
            w_ScoreBlock => ScoreBlockEvent(w_ScoreBlock)
        );
    }

    /// <summary> 
    /// スコアブロック衝突後処理
    /// </summary> 
    /// <param name="pm_ScoreBlock">スコアブロック衝突ステータス</param> 
    /// <returns>なし</returns>
    private void ScoreBlockEvent(SC_LocalBall.ScoreBlockInform pm_ScoreBlock) {
        // ボールの状態更新
        BallStatusUpdate(pm_ScoreBlock);

        // ブロック衝突時のアニメーションを表示
        ScoreAnimation(pm_ScoreBlock);
    }

    /// <summary> 
    /// ボール状態更新
    /// </summary> 
    /// <param name="pm_ScoreBlock">スコアブロック衝突ステータス</param> 
    /// <returns>なし</returns>
    private void BallStatusUpdate(SC_LocalBall.ScoreBlockInform pm_ScoreBlock) {
        // スコアブロックの衝突音を鳴らす
        SC_GameSE.GetStatus();

        // 衝突したブロックによって状態を更新
        switch (pm_ScoreBlock.Status) {
            case 1:
                // ネットワークボールの発射を加算
                SC_NetworkManager.BallHold(NETBALLCREATE);
                break;
            case 2:
                // ネットワークボールの発射を２倍
                SC_NetworkManager.BallHold(NETBALLMULTICFRATE);
                break;
            case 3:
                // ローカルボール生成(5つまで)
                if (CP_LocalBalls.transform.childCount < LOCALBALLMAX) { LocalBallCreate(); }
                break;
            case 4:
                // ネットワークボールを発射
                SC_NetworkManager.BallShot();
                break;
            default:
                break;
        }
    }

    /// <summary> 
    /// スコアブロックのアニメーション表示
    /// </summary> 
    /// <param name="pm_ScoreBlock">スコアブロック衝突ステータス</param> 
    /// <returns>なし</returns>
    private void ScoreAnimation(SC_LocalBall.ScoreBlockInform pm_ScoreBlock) {
        // スコアブロックに当たった時のアニメーションを宣言
        GameObject   w_Animation;
        SC_Animation w_CompAnim;

        // スコアアニメーションを生成
        if (pm_ScoreBlock.Position.y > PINBALLBORDER) {   
            // ピンボールエリア上段にスコアアニメーションを生成
            w_Animation = Instantiate(OB_UpScoreStatusAnimPrefab, CP_Animations);
        } else {
            // ピンボールエリア下段にスコアアニメーションを生成
            w_Animation = Instantiate(OB_DownScoreStatusAnimPrefab, CP_Animations);
        }
        
        // スコアアニメーションに位置をセット
        w_CompAnim = w_Animation.GetComponent<SC_Animation>();
        w_CompAnim.PositionSet(pm_ScoreBlock.Position);
    }

    /// <summary> 
    /// 勝敗判定処理
    /// </summary> 
    /// <param name="pm_BattleJudge">勝敗判定ステータス</param> 
    /// <returns>なし</returns>
    public void BattleJudge(SC_NetObjectInform.BattleJudge pm_BattleJudge) {
        // 爆発オブジェクトを宣言
        GameObject w_Bom;
        SC_Animation w_CompBomAnim;

        // 敗北のほうに爆発アニメーションを生成
        w_Bom = Instantiate(OB_BomAnimPrefab, CP_Animations);
        w_CompBomAnim = w_Bom.GetComponent<SC_Animation>();
        w_CompBomAnim.PositionSet(pm_BattleJudge.LosePos);

        // 爆発アニメーション後に勝敗画面を表示
        w_CompBomAnim.GB_BomInform.Subscribe (
            w_Null => BattleJudgePanel(pm_BattleJudge.WinFlag)
        );

        // 勝敗効果音
        if (pm_BattleJudge.WinFlag) { SC_GameSE.Win(); } 
        else { SC_GameSE.Lose(); }
        
        // ローカルボール削除
        LocalBallDelete();
    }

    /// <summary> 
    /// 勝敗パネル表示
    /// </summary> 
    /// <param name="pm_WinFlag">勝利フラグ(true:勝ち false:負け)</param> 
    /// <returns>なし</returns>
    public void BattleJudgePanel(bool pm_WinFlag) {
        // 勝敗判定画面を表示
        OB_WinPanel.SetActive(pm_WinFlag);
        OB_LosePanel.SetActive(!pm_WinFlag);
    }

    /// <summary> 
    /// ローカルボールを削除
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void LocalBallDelete() {
        // ゲーム終了後はローカルボールを削除
        foreach (Transform w_ChildTransform in CP_LocalBalls.transform) {
            Destroy(w_ChildTransform.gameObject);
        }
    }

    /// <summary> 
    /// 退室パネルを表示
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void LeftRoom() {
        // 退室パネルを表示
        OB_LeftRoomPanel.SetActive(true);
    }
}
