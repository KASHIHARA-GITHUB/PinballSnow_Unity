using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// �����X�N���v�g
public class SC_Room : MonoBehaviourPunCallbacks {
    // �R���|�[�l���g�ϐ�
    [SerializeField] private Text CP_WarningText = null;     // ���[�j���O�e�L�X�g

    // �O���[�o���ϐ�
    private bool GB_InRoom;         // �����ɎQ�������瑊���T���������s��
    private bool GB_IsMatchingFlag; // �Q�����Update�֐����������Ȃ��悤�ɂ���

    // �萔
    private const int PLAYERINROOMMAX = 2;    // �ő�v���C���[�l��
    private const int PLAYERCONNETCTMAX = 20; // �ő�ڑ��l��

    /// <summary> 
    /// �}�X�^�[�T�[�o�ڑ�
    /// </summary> 
    /// <param name="pm_PlayerName">�v���C���[��</param> 
    /// <returns>�Ȃ�</returns>
    public void Connecting(string pm_PlayerName) {
        //�v���C���[�̖��O��o�^
        PhotonNetwork.NickName = pm_PlayerName;

        //�}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary> 
    /// �}�X�^�[�T�[�o�ڑ������㏈��(�R�[���o�b�N�֐�)
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public override void OnConnectedToMaster() {
        // �Q�[���T�[�o�[�փ����_���ɐڑ�����
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary> 
    /// �Q�[���T�[�o�ڑ������㏈��(�R�[���o�b�N�֐�)
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public override void OnJoinedRoom() {
        GB_InRoom = true;
    }

    /// <summary> 
    /// ���[���������������[����������Ȃ��ꍇ(�R�[���o�b�N�֐�)
    /// </summary> 
    /// <param name="returnCode">�R�[���o�b�N�R�[�h</param>
    /// <param name="message"   >���b�Z�[�W</param> 
    /// <returns>�Ȃ�</returns>
    public override void OnJoinRandomFailed(short returnCode, string message) {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = PLAYERINROOMMAX }, TypedLobby.Default);
    }

    /// <summary> 
    /// ���[���Q���҂�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void Update() {
        // �}�b�`������������Update�֐����I������
        if (GB_IsMatchingFlag) {
            return;
        }

        //�����ɎQ�������牺�L�̏������s��
        if (GB_InRoom) {
            // �����ڑ�20�l�𒴂���
            if(PhotonNetwork.CountOfPlayers > PLAYERCONNETCTMAX) {
                CP_WarningText.text = "�ڑ��l�����z���܂���";
                return;
            }

            // 2�l�ڑ��o������V�[���ړ�
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount) {
                Debug.Log("CountOfRooms " + PhotonNetwork.CountOfRooms);
                Debug.Log("CountOfPlayersOnMaster " + PhotonNetwork.CountOfPlayersOnMaster);
                Debug.Log("CountOfPlayersInRooms " + PhotonNetwork.CountOfPlayersInRooms);
                Debug.Log("CountOfPlayers " + PhotonNetwork.CountOfPlayers);

                // �}�b�`���O�t���O��true�ɂ���
                GB_IsMatchingFlag = true;

                // ������s�ɂ���
                PhotonNetwork.CurrentRoom.IsOpen = false;

                // �Q�[����ʂ֑J��
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}