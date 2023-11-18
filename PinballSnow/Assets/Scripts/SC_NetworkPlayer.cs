using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// �l�b�g���[�N�v���C���[
public class SC_NetworkPlayer : MonoBehaviourPunCallbacks{
    // �R���|�[�l���g�ϐ�
    [SerializeField] private Text CP_PlayerNameText;        // �v���C���[��

    // �O���[�o���ϐ�
    private const bool WIN = true;       // ����
    private const bool LOSE = false;     // �s�k

    // �v���C���[�̃I�v�V�����ϐ�
    private struct Player {
        public string Name;
        public Color Color;
        public Vector2 Position;
        public int Layer;

        public Player(string p1, Color p2, Vector2 p3, int p4) {
            this.Name    = p1;
            this.Color   = p2;
            this.Position = p3;
            this.Layer   = p4;
        }
    }

    private Player[] Players = {
            new Player ("NetworkPlayer1", new Color(255f, 100f, 0f), new Vector2(135.0f, 225.0f), 8 ),
            new Player ("NetworkPlayer2", new Color(255f, 0f, 100f), new Vector2(135.0f, -195.0f), 9 )
    };

    /// <summary> 
    /// �v���C���[�𑀍�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void Update() {
        // ���g�̃l�b�g���[�N�v���C���[
        if (photonView.IsMine) {
            // �v���C���[���ړ�
            MovePlayer();
        }
    }

    /// <summary> 
    /// �v���C���[���ړ�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void MovePlayer() {
        //�ϐ��錾
        float w_CurrentX = transform.localPosition.x;

        //�ړ��ő勗���͈�
        if (Input.GetKey(KeyCode.X)) {
            w_CurrentX = Mathf.Min(w_CurrentX + 2, 350);
        } else if (Input.GetKey(KeyCode.Z)) {
            w_CurrentX = Mathf.Max(w_CurrentX - 2, -50);
        }

        //�o�[�̌��݈ʒu
        Vector3 w_Pos = new Vector3(w_CurrentX, transform.localPosition.y, 0);
        transform.localPosition = w_Pos;
    }

    /// <summary> 
    /// �l�b�g���[�N�v���C���[������
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void Initialize() {
        if (photonView.IsMine) {
            // �v���C���[�ԍ�
            int w_PlayerNum = photonView.OwnerActorNr - 1;

            // �v���C���[����
            photonView.RPC(nameof(InitializeRPC), RpcTarget.All, w_PlayerNum);
        }
    }

    /// <summary> 
    /// �l�b�g���[�N�v���C���[����������
    /// </summary> 
    /// <param name="pm_PlayerNumber">�v���C���[�ԍ�</param> 
    /// <returns>�Ȃ�</returns>
    [PunRPC]
    private void InitializeRPC(int pm_PlayerNumber) {
        // �ϐ��錾
        Transform w_Parent;
        SpriteRenderer w_Sprite;
        RectTransform w_Rect;

        // ���g�̃v���C���[����\��
        CP_PlayerNameText.text = $"{photonView.Owner.NickName}";

        // �e�I�u�W�F�N�g[NetworkPlayers]�z���Ƀv���C���[��ݒu
        w_Parent = GameObject.Find("NetworkPlayers").transform;
        transform.SetParent(w_Parent);

        // �R���|�[�l���g�擾
        w_Sprite = GetComponent<SpriteRenderer>();
        w_Rect   = GetComponent<RectTransform>();
        
        // ���g�̃v���C���[�ɐݒ�
        gameObject.name  = Players[pm_PlayerNumber].Name;
        gameObject.layer = Players[pm_PlayerNumber].Layer;
        w_Sprite.color   = Players[pm_PlayerNumber].Color;
        w_Rect.anchoredPosition = Players[pm_PlayerNumber].Position;
    }

    /// <summary> 
    /// �v���C���[�ʒu���擾
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�v���C���[�ʒu�擾</returns>
    public Vector3 GetPosition()
    {
        // ���g�v���C���[�̈ʒu
        return transform.localPosition;
    }

    /// <summary> 
    /// �l�b�g���[�N�v���C���[�Փˏ���
    /// </summary> 
    /// <param name="pm_Collision">�Փ˃I�u�W�F�N�g</param> 
    /// <returns>�Ȃ�</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // �l�b�g���[�N�v���C���[�ȊO�̏Փ˂̓��^�[������
        if (!(gameObject.name == "NetworkPlayer1" || gameObject.name == "NetworkPlayer2"))
            return;

        // �l�b�g���[�N�{�[���Ǝ��g�v���C���[�̏Փˈʒu���擾
        Vector2 w_PlayerPos = transform.localPosition;

        if (photonView.IsMine) {
            // ���s����
            PhotonNetwork.LocalPlayer.BattleJudgeInform(LOSE, w_PlayerPos);             // �s�k��[SC_NetObjectInform]�ɒʒm
            photonView.RPC(nameof(JudgeRPC), RpcTarget.OthersBuffered, WIN, w_PlayerPos); // ���������g�ȊO�ɓ���     
        } else {
            // ���s����
            PhotonNetwork.LocalPlayer.BattleJudgeInform(WIN, w_PlayerPos);               // ������[SC_NetObjectInform]�ɒʒm
            photonView.RPC(nameof(JudgeRPC), RpcTarget.OthersBuffered, LOSE, w_PlayerPos); // �s�k�����g�ȊO�ɓ���
        }
        photonView.RPC(nameof(DestroyRPC), RpcTarget.All); // ���v���C���[�̍폜��S���ɓ���
    }

    /// <summary> 
    /// ���s�𑼃v���C���[�ɕ\��
    /// </summary> 
    /// <param name="pm_WinFlag"  >�����t���O(true:���� false:����)</param> 
    /// <param name="pm_PlayerPos">�v���C���[�ʒu</param> 
    /// <returns>�Ȃ�</returns>
    [PunRPC]
    private void JudgeRPC(bool pm_WinFlag, Vector2 pm_PlayerPos) {
        // ������[SC_NetObjectInform]�ɒʒm
        PhotonNetwork.LocalPlayer.BattleJudgeInform(pm_WinFlag, pm_PlayerPos);
    }

    /// <summary> 
    /// �l�b�g���[�N�v���C���[�폜����
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    [PunRPC]
    private void DestroyRPC() {
        // �v���C���[���폜
        Destroy(gameObject);
    }
}