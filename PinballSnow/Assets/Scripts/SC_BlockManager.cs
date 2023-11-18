using Photon.Pun;
using UnityEngine;

// �u���b�N�}�l�[�W���[
public class SC_BlockManager : MonoBehaviourPunCallbacks {

    // �l�b�g���[�N�u���b�N
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

    // �l�b�g���[�N�u���b�N�l�Z�b�g
    private NetworkBlock[] NETWORKBLOCKPOS = {
        new NetworkBlock(-2, 12, -5, 5),
        new NetworkBlock(-1, 12, -5, 5),
        new NetworkBlock(-2, 12, -3, 5)
    };

    /// <summary> 
    /// �l�b�g���[�N�u���b�N�𐶐�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void Create() {
        // MasterClient���u���b�N��ݒu����
        if (PhotonNetwork.IsMasterClient) {
            BlockSet();
        }
    }

    /// <summary> 
    /// �l�b�g���[�N�u���b�N�𐶐�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void BlockSet() {
        // �ϐ��錾
        float w_PosX, w_PosY;   // �u���b�N�̈ʒuXY
        Vector2 w_Position;     // �u���b�N�̈ʒu 
        GameObject w_NetworkBlock;          // �l�b�g���[�N�u���b�N�I�u�W�F�N�g
        SC_NetworkBlock w_CompNetworkBlock; // �l�b�g���[�N�u���b�N�I�u�W�F�N�g�ݒ�
        NetworkBlock NetworkBlockPos;       // �l�b�g���[�N�u���b�N�̍ő�ŏ��ʒuXY

        // �����_���ϐ����Z�b�g
        int w_BlockRandom = Random.Range(0, 1);

        // �u���b�N�̔z�u�ʒu�������_���ɐݒ�
        if (w_BlockRandom > 0.7f) { NetworkBlockPos = NETWORKBLOCKPOS[0]; }
        else if (w_BlockRandom > 0.4f) { NetworkBlockPos = NETWORKBLOCKPOS[1]; }
        else { NetworkBlockPos = NETWORKBLOCKPOS[2]; }

        //�u���b�N�𐶐�
        for (int x = NetworkBlockPos.MinX; x < NetworkBlockPos.MaxX; x++) {
            for (int y = NetworkBlockPos.MinY; y < NetworkBlockPos.MaxY; y++) {
                // �u���b�N�̈ʒu���v�Z
                w_PosX = x * 1.0f;
                w_PosY = y * 1.0f;
                w_Position = new Vector2(w_PosX, w_PosY);

                //�l�b�g���[�N�u���b�N����
                w_NetworkBlock = PhotonNetwork.Instantiate("NetworkBlock", w_Position, Quaternion.identity);
                w_CompNetworkBlock = w_NetworkBlock.GetComponent<SC_NetworkBlock>();
                w_CompNetworkBlock.Initialize();
            }
        }
    }
}
