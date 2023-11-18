using UnityEngine;

// ゲーム効果音
public class SC_GameSE : MonoBehaviour {
    [SerializeField] private AudioSource GameSound = null;   // 効果音
    [SerializeField] private AudioClip CP_GameStart = null;  // ゲームスタート
    [SerializeField] private AudioClip CP_Warning = null;  // 警告音
    [SerializeField] private AudioClip CP_GetStatus = null;  // スコアブロック衝突音
    [SerializeField] private AudioClip CP_Attack1 = null;    // 雪玉投げる声1
    [SerializeField] private AudioClip CP_Attack2 = null;    // 雪玉投げる声2
    [SerializeField] private AudioClip CP_Attack3 = null;    // 雪玉投げる声3
    [SerializeField] private AudioClip CP_ThrowBall = null;  // 雪玉投げる効果音
    [SerializeField] private AudioClip CP_Win = null;        // プレイヤー勝利
    [SerializeField] private AudioClip CP_Lose = null;       // プレイヤー敗北

    /// <summary> 
    /// ゲームスタート
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void GameStart() {
        GameSound.PlayOneShot(CP_GameStart);
    }

    /// <summary> 
    /// 警告音
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Warning() {
        GameSound.PlayOneShot(CP_Warning);
    }

    /// <summary> 
    /// スコアブロック衝突音
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void GetStatus() {
        GameSound.PlayOneShot(CP_GetStatus);
    }

    /// <summary> 
    /// プレイヤー攻撃１
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Attack1() {
        GameSound.PlayOneShot(CP_Attack1);
    }

    /// <summary> 
    /// プレイヤー攻撃２
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Attack2() {
        GameSound.PlayOneShot(CP_Attack2);
    }

    /// <summary> 
    /// プレイヤー攻撃３
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Attack3() {
        GameSound.PlayOneShot(CP_Attack3);
    }

    /// <summary> 
    /// 雪玉投げる音
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void ThrowBall() {
        GameSound.PlayOneShot(CP_ThrowBall);
    }

    /// <summary> 
    /// 勝利音
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Win() {
        GameSound.PlayOneShot(CP_Win);
    }

    /// <summary> 
    /// 敗北音
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Lose() {
        GameSound.PlayOneShot(CP_Lose);
    }
}
