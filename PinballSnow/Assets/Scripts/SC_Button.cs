using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// �{�^���X�N���v�g
public class SC_Button : MonoBehaviourPunCallbacks
{
    //�X�N���v�g�ϐ�
    [SerializeField] private SC_GameTitleManager SC_GameTitleManager;   // �Q�[���^�C�g���}�l�[�W��
    [SerializeField] private SC_GameSE SC_GameSE;                       // �Q�[�����ʉ�

    /// <summary> 
    /// �{�^���N���b�N����
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void OnClick() {
        // �{�^�����m�F
        Debug.Log(transform.name);

        // �e�{�^���̏���
        switch (transform.name) {
            case "GameStartButton":
                // �v���C���[���`�F�b�N
                PlayerNameCheck();
                break;
            case "MainButton":
                // �l�b�g���[�N�ؒf
                PhotonNetwork.Disconnect();
                break;
            default:
                break;
        }
    }

    /// <summary> 
    /// �v���C���[���`�F�b�N
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void PlayerNameCheck() {
        // �v���C���[���`�F�b�N
        if (SC_GameTitleManager.PlayerNameSet()) {
            SC_GameSE.GameStart();  // �Q�[���X�^�[�g���ʉ�
            GetComponent<Button>().interactable = false; // �{�^����񊈐�
        } else {
            SC_GameSE.Warning();    // �x�����b�Z�[�W���ʉ�
        }
    }

    /// <summary> 
    /// �^�C�g���V�[���֑J��(�}�X�^�[�T�[�o�ؒf��̃R�[���o�b�N)
    /// </summary> 
    /// <param name="pm_Disconnect">�}�X�^�[�T�[�o�ؒf����</param> 
    /// <returns>�Ȃ�</returns>
    public override void OnDisconnected(DisconnectCause pm_Disconnect) {
        // �^�C�g���V�[���֑J��
        SceneManager.LoadScene("TitleScene");
    }
}
