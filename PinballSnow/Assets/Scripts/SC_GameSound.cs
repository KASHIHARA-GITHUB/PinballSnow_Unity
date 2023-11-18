using UnityEngine;

// ゲームサウンド
public class SC_GameSound : MonoBehaviour {
    // コンポーネント変数
    [SerializeField] private AudioSource GameSound = null;  // サウンドソース
    [SerializeField] private AudioClip[] CP_BGM = null;     // BGM

    /// <summary> 
    /// ステージの曲選択
    /// </summary> 
    /// <param name="pm_SelectNumber">曲番号</param> 
    /// <returns>なし</returns>
    public void Select(int pm_SelectNumber) {
        GameSound.clip = CP_BGM[pm_SelectNumber];
        GameSound.Play();
    }
}
