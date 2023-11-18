using UnityEngine;
using UniRx;

// アニメーションスクリプト
public class SC_Animation: MonoBehaviour {
    public Subject<int> GB_BomInform = new Subject<int>();    // 爆発アニメーションを終了後に返す
    private const string BOMPRINTEVENT = "Bom";         // PrintEventの結果文字列
    private const int BOMANIMSTATUS = default;          // 通知変数として使う(空変数)

    /// <summary> 
    /// アニメーションの位置を設定
    /// </summary> 
    /// <param name="pm_Position">アニメーション位置</param> 
    /// <returns>なし</returns>
    public void PositionSet(Vector2 pm_Position) {
        GetComponent<RectTransform>().anchoredPosition = pm_Position;
    }

    /// <summary> 
    /// アニメーション終了後処理
    /// </summary> 
    /// <param name="pm_PrintEvent">AnimationClip文字</param> 
    /// <returns>なし</returns>
    public void PrintEvent(string pm_PrintEvent) {
        // 爆発アニメーション終了後
        if (pm_PrintEvent == BOMPRINTEVENT) { GB_BomInform.OnNext(BOMANIMSTATUS); }
        
        // アニメーションを削除
        Destroy(gameObject);
    }
}
