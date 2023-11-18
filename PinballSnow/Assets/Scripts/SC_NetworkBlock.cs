using Photon.Pun;
using UnityEngine;

// �l�b�g���[�N�u���b�N
public class SC_NetworkBlock : MonoBehaviourPunCallbacks {
    // �萔
    private const int PLAYERONE = 8;
    private const int PLAYERTWO = 9;

    /// <summary> 
    /// �l�b�g���[�N�u���b�N�̏�����
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void Initialize() {
        // ���g�̏ꍇ����
        if (photonView.IsMine) {
            // �u���b�N�̃I�v�V��������
            photonView.RPC(nameof(InitializeRPC), RpcTarget.All);
        }
    }

    /// <summary> 
    /// �l�b�g���[�N�u���b�N�̏���������
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    [PunRPC]
    private void InitializeRPC() {
        // �e�I�u�W�F�N�g[NetworkBlocks]�z���ɐݒu
        Transform w_Parent = GameObject.Find("NetworkBlocks").transform;
        transform.SetParent(w_Parent);
    }

    /// <summary> 
    /// �l�b�g���[�N�u���b�N�̏Փˏ���
    /// </summary> 
    /// <param name="pm_Collision">�Փ˃I�u�W�F�N�g</param> 
    /// <returns>�Ȃ�</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // �u���b�N�Փ˓���
        photonView.RPC(nameof(BlockCollisionRPC), RpcTarget.All, pm_Collision.gameObject.name);
    }

    /// <summary> 
    /// �l�b�g���[�N�u���b�N�Փˎ��̓���
    /// </summary> 
    /// <param name="pm_Collision">�Փ˃I�u�W�F�N�g��</param> 
    /// <returns>�Ȃ�</returns>
    [PunRPC]
    private void BlockCollisionRPC(string pm_Collision) {
        // �X�v���C�g�ϐ�
        SpriteRenderer w_SpriteRenderer;
        w_SpriteRenderer = GetComponent<SpriteRenderer>();

        // �Փ˂����l�b�g���[�N�{�[���̐F�ɍ��킹��
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
