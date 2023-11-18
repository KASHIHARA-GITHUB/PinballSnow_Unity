using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// �Q�[���^�C�g���}�l�[�W��
public class SC_GameTitleManager : MonoBehaviourPunCallbacks {
    // �X�N���v�g�ϐ�
    [SerializeField] private SC_Room SC_Room = null;            // ���[��
    [SerializeField] private SC_GameSound SC_GameSound = null;  // �Q�[���T�E���h

    // �R���|�[�l���g�ϐ�
    [SerializeField] private Text CP_PlayerNameText = null;     // �v���C���[���e�L�X�g
    [SerializeField] private Text CP_WarningText = null;        // �x���e�L�X�g

    // �萔
    private const int TITLEBGM = 0;  // BGM
    private const int NAMELENGTHMIN = 0;  // �v���C���[��������ŏ�
    private const int NAMELENGTHMAX = 11; // �v���C���[��������ő�
    private const string CONNECTMSG = "�����T���Ă��܂�";   // �ڑ����b�Z�[�W
    private const string WARNINGMSG = "1�����ȏ�10�����ȉ��œ��͂��Ă�������"; // �x�����b�Z�[�W

    /// <summary> 
    /// �Q�[���^�C�g���J�n����
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void Start() {
        SC_GameSound.Select(TITLEBGM);
    }

    /// <summary> 
    /// �e�L�X�g����v���C���[���擾
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public bool PlayerNameSet() {
        // �v���C���[���擾
        string w_PlayerName = CP_PlayerNameText.text;

        //���O�̕�������1�����ȏ�10�����ȓ��ł���Α����T��
        if (w_PlayerName.Length > NAMELENGTHMIN && 
            w_PlayerName.Length < NAMELENGTHMAX) {
            // �ڑ����b�Z�[�W��\��
            CP_WarningText.text = CONNECTMSG;
            SC_Room.Connecting(w_PlayerName);

            return true;
        }

        //�x�����b�Z�[�W��\��
        CP_WarningText.text = WARNINGMSG;

        return false;
    }
}