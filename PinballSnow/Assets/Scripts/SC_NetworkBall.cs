using Photon.Pun;
using UnityEngine;

// �l�b�g���[�N�{�[���X�N���v�g
public class SC_NetworkBall : MonoBehaviourPunCallbacks, IPunObservable {
    // �{�[���I�v�V�����ϐ�
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

    // �l�b�g���[�N�{�[���̐ݒ�l
    private Ball[] Balls = {
            new Ball ( "NetworkBall1" ,new Color(255f, 100f, 0f), -1f, 6),
            new Ball ( "NetworkBall2" ,new Color(255f, 0f, 100f), 1f, 7),
    };

    // �l�b�g���[�N�{�[���̗̑�
    private int GB_BallHp = 3;

    /// <summary> 
    /// �l�b�g���[�N�{�[����RigidBody2D�𓯊�
    /// </summary> 
    /// <param name="pm_Stream">����M���b�Z�[�W</param> 
    /// <param name="pm_Info"  >�ʐM���</param>
    /// <returns>�Ȃ�</returns>
    void IPunObservable.OnPhotonSerializeView(PhotonStream pm_Stream, PhotonMessageInfo pm_Info) {
        PhotonNetwork.SendRate = 100; // 1�b�ԂɃ��b�Z�[�W���M���s����
        PhotonNetwork.SerializationRate = 100; // 1�b�ԂɃI�u�W�F�N�g�������s����

        if (pm_Stream.IsWriting) {
            //���̃v���C���[�ɒl�𑗐M
            pm_Stream.SendNext(transform.GetComponent<Rigidbody2D>().velocity);
        } else {
            //���̃v���C���[���瑗�M���ꂽ�l����M
            transform.GetComponent<Rigidbody2D>().velocity = (Vector2)pm_Stream.ReceiveNext();
        }
    }

    /// <summary> 
    /// �l�b�g���[�N�{�[���̏�����
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void Initialize(Vector2 pm_PlayerPosition) {
        if (photonView.IsMine) {
            // �v���C���[�ԍ����擾
            int w_PlayerNumber = photonView.OwnerActorNr - 1;

            // �l�b�g���[�N�{�[����ݒ蓯��
            photonView.RPC(nameof(InitializeRPC), RpcTarget.All, w_PlayerNumber, pm_PlayerPosition);
        }
    }

    /// <summary> 
    /// �l�b�g���[�N�{�[���̏���������
    /// </summary> 
    /// <param name="pm_PlayerNumber">�v���C���[�ԍ�</param> 
    /// <param name="pm_PlayerPosition">�v���C���[�ꏊ</param> 
    /// <returns>�Ȃ�</returns>
    [PunRPC]
    private void InitializeRPC(int pm_PlayerNumber, Vector2 pm_PlayerPosition) {
        // �R���|�[�l���g�ϐ��錾
        Transform      w_Parent;
        SpriteRenderer w_Sprite;

        // �e�I�u�W�F�N�g[NetworkBalls]�z���ɐݒu
        w_Parent = GameObject.Find("NetworkBalls").transform;
        transform.SetParent(w_Parent);

        // �l�b�g���[�N�{�[���ɐݒ�
        w_Sprite = GetComponent<SpriteRenderer>();
        w_Sprite.color     = Balls[pm_PlayerNumber].Color;
        gameObject.name    = Balls[pm_PlayerNumber].Name;
        gameObject.layer   = Balls[pm_PlayerNumber].Layer;
        transform.localPosition = pm_PlayerPosition;
        Vector2 w_BallVector = new Vector2(Random.Range(-0.7f, 0.7f), Balls[pm_PlayerNumber].Y_Direct);
        transform.GetComponent<Rigidbody2D>().velocity = w_BallVector.normalized * 10;
    }

    /// <summary> 
    /// �l�b�g���[�N�{�[���Փˏ���
    /// </summary> 
    /// <param name="pm_Collision">�Փ˃I�u�W�F�N�g</param> 
    /// <returns>�Ȃ�</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // �l�b�g���[�N�u���b�N�̏Փˎ������s��
        if (pm_Collision.gameObject.name == "NetworkBlock(Clone)") {
            //�{�[���폜�𓯊�
            photonView.RPC(nameof(DestroyRPC), RpcTarget.All);
            return;
        }

        // �l�b�g���[�N�{�[���̗̑͌���
        GB_BallHp--;
        if (GB_BallHp < 1) {
            //�{�[���폜�𓯊�
            photonView.RPC(nameof(DestroyRPC), RpcTarget.All);
        }
    }

    /// <summary> 
    /// �l�b�g���[�N�{�[���̍폜����
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    [PunRPC]
    private void DestroyRPC() {
        Destroy(gameObject);
    }
}
