using Photon.Pun;
using UnityEngine;

// ブロックマネージャー
public class SC_BlockManager : MonoBehaviourPunCallbacks {

    // ネットワークブロック
    private struct NetworkBlock {
        public int MinX;
        public int MaxX;
        public int MinY;
        public int MaxY;

        public NetworkBlock(int p1, int p2, int p3, int p4) {
            this.MinX = p1;
            this.MaxX = p2;
            this.MinY = p3;
            this.MaxY = p4;
        }
    }

    // ネットワークブロック値セット
    private NetworkBlock[] NETWORKBLOCKPOS = {
        new NetworkBlock(-2, 12, -5, 5),
        new NetworkBlock(-1, 12, -5, 5),
        new NetworkBlock(-2, 12, -3, 5)
    };

    /// <summary> 
    /// ネットワークブロックを生成
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    public void Create() {
        // MasterClientがブロックを設置する
        if (PhotonNetwork.IsMasterClient) {
            BlockSet();
        }
    }

    /// <summary> 
    /// ネットワークブロックを生成
    /// </summary> 
    /// <param name="message">なし</param> 
    /// <returns>なし</returns>
    private void BlockSet() {
        // 変数宣言
        float w_PosX, w_PosY;   // ブロックの位置XY
        Vector2 w_Position;     // ブロックの位置 
        GameObject w_NetworkBlock;          // ネットワークブロックオブジェクト
        SC_NetworkBlock w_CompNetworkBlock; // ネットワークブロックオブジェクト設定
        NetworkBlock NetworkBlockPos;       // ネットワークブロックの最大最小位置XY

        // ランダム変数をセット
        int w_BlockRandom = Random.Range(0, 1);

        // ブロックの配置位置をランダムに設定
        if (w_BlockRandom > 0.7f) { NetworkBlockPos = NETWORKBLOCKPOS[0]; }
        else if (w_BlockRandom > 0.4f) { NetworkBlockPos = NETWORKBLOCKPOS[1]; }
        else { NetworkBlockPos = NETWORKBLOCKPOS[2]; }

        //ブロックを生成
        for (int x = NetworkBlockPos.MinX; x < NetworkBlockPos.MaxX; x++) {
            for (int y = NetworkBlockPos.MinY; y < NetworkBlockPos.MaxY; y++) {
                // ブロックの位置を計算
                w_PosX = x * 1.0f;
                w_PosY = y * 1.0f;
                w_Position = new Vector2(w_PosX, w_PosY);

                //ネットワークブロック生成
                w_NetworkBlock = PhotonNetwork.Instantiate("NetworkBlock", w_Position, Quaternion.identity);
                w_CompNetworkBlock = w_NetworkBlock.GetComponent<SC_NetworkBlock>();
                w_CompNetworkBlock.Initialize();
            }
        }
    }
}
