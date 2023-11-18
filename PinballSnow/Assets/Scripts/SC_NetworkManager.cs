using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

// �l�b�g���[�N�}�l�[�W��
public class SC_NetworkManager : MonoBehaviourPunCallbacks {
    // �X�N���v�g�ϐ�
    [SerializeField] private GameManager SC_GameManager = null;
    [SerializeField] private SC_GameSE   SC_GameSE = null;
    private SC_NetworkPlayer SC_CompNetworkPlayer = null;

    // �R���|�[�l���g�ϐ�
    [SerializeField] private Text CP_BallHoldText = null;

    // �O���[�o���ϐ�
    private Vector2 GB_NetPlayerPos = new Vector2 (0, 0);      // �l�b�g���[�N�I�u�W�F�N�g�̈ʒu
    private int  GB_NetBallShotCount = 0;                      // �l�b�g���[�N�{�[���̔��ː�
    private bool GB_BallShotAllowFlag = true;                  // �l�b�g���[�N�{�[�����ˋ��t���O
    private bool GB_BallShotStopFlag = false;                  // �l�b�g���[�N�{�[�����˒��f�t���O
    private bool GB_BattleJudgeShowFlag = false;               // ���s��ʕ\���t���O
    private const int BALLMAX = 1000;                       // �l�b�g���[�N�{�[���̔��ˍő吔
    private const float BALLSHOTDURING = 0.1f;                 // �l�b�g���[�N�{�[���̔��ˊԊu

    /// <summary> 
    /// �l�b�g���[�N�}�l�[�W���[����������
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void Initialize() {
        // �l�b�g���[�N�{�[���ێ�����������
        CP_BallHoldText.text = "0";

        // �l�b�g���[�N�v���C���[�ʒm������
        SC_NetObjectInform.SetEventInform();
    }

    /// <summary> 
    /// �l�b�g���[�N�v���C���[����
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void PlayerCreate() {
        // �l�b�g���[�N�v���C���[�̐���
        GameObject w_Player = 
            PhotonNetwork.Instantiate("NetworkPlayer", GB_NetPlayerPos, Quaternion.identity);
        SC_CompNetworkPlayer = w_Player.GetComponent<SC_NetworkPlayer>();
        SC_CompNetworkPlayer.Initialize();

        // �l�b�g���[�N�v���C���[�Փ˒ʒm��M
        SC_NetObjectInform.GB_JudgeStatus.Subscribe(
            w_JudgeStatus => CustomResult(w_JudgeStatus)
        );
    }

    /// <summary> 
    /// �J�X�^���v���p�e�B����Փˌ��ʂ��擾
    /// </summary> 
    /// <param name="pm_JudgeStatus">�v���C���[�ʒu�A���s���</param> 
    /// <returns>�Ȃ�</returns>
    private void CustomResult(SC_NetObjectInform.BattleJudge pm_JudgeStatus) {
        //�{�[���V���b�g
        GB_BallShotStopFlag = true;
        GB_BattleJudgeShowFlag = true;
        SC_GameManager.BattleJudge(pm_JudgeStatus);
    }

    /// <summary> 
    /// �l�b�g���[�N�{�[���𔭎�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void BallShot() {
        // �{�[���𔭎˒���͍Ĕ��˕s�ɂ���
        if(GB_BallShotAllowFlag) {
            // �{�[�����V���b�g���Ƀ{�[���ێ����Z��h�~
            GB_BallShotAllowFlag = false;

            //�{�[���V���b�g
            StartCoroutine(BallShotCor(GB_NetBallShotCount));
        }
    }

    /// <summary> 
    /// �l�b�g���[�N�{�[������
    /// </summary> 
    /// <param name="pm_BallShotCount">�l�b�g���[�N�{�[�����ː�</param> 
    /// <returns>�Ȃ�</returns>
    private IEnumerator BallShotCor(int pm_BallShotCount) {
        //�l�b�g���[�N�{�[���̃I�u�W�F�N�g�錾
        GameObject     w_Ball;
        SC_NetworkBall w_CompBall;

        // �{�[���𓊂���Ƃ��̔������ʉ�
        int w_Rnd = Random.Range(1, 10);
        if (w_Rnd < 4) { SC_GameSE.Attack1(); }
        else if (w_Rnd < 7) { SC_GameSE.Attack2(); }
        else {  SC_GameSE.Attack3(); }

        // �{�[���𔭎�
        for (int i = 1; i <= pm_BallShotCount; i++) {
            // �{�[���𓊂��鉹
            SC_GameSE.ThrowBall();

            // �l�b�g���[�N�{�[���𐶐�
            Vector3 w_PlayerPosition = SC_CompNetworkPlayer.GetPosition();
            w_Ball = PhotonNetwork.Instantiate("NetworkBall", w_PlayerPosition, Quaternion.identity);
            w_CompBall = w_Ball.GetComponent<SC_NetworkBall>();
            w_CompBall.Initialize(w_PlayerPosition);

            // �l�b�g���[�N�{�[���ێ��e�L�X�g���X�V
            CP_BallHoldText.text = $"{pm_BallShotCount - i}";

            // 0.1�b�ҋ@
            yield return new WaitForSeconds(BALLSHOTDURING);

            // �{�[�����˒��f
            if (GB_BallShotStopFlag) { break; }
        }

        // �{�[�����ˉ\��ԂɍX�V
        GB_NetBallShotCount  = 0;
        GB_BallShotAllowFlag = true;
    }

    /// <summary> 
    /// �l�b�g���[�N�{�[���ێ�
    /// </summary> 
    /// <param name="pm_BallHoldCount">�l�b�g���[�N�{�[���ێ���</param> 
    /// <returns>�Ȃ�</returns>
    public void BallHold(int pm_BallHoldStatus) {
        // �l�b�g���[�N�{�[�����ˎ��͖���
        if (GB_BallShotAllowFlag == true) {
            // �l�b�g���[�N�{�[�����B���X�V 
            if (pm_BallHoldStatus == 1) {
                // �l�b�g���[�N�{�[�������Z
                GB_NetBallShotCount = GB_NetBallShotCount + 1;
            } else {
                // �l�b�g���[�N�{�[����2�{�ɂ���
                GB_NetBallShotCount = GB_NetBallShotCount * 2;
            }

            // �{�[����MAX��蒴���Ȃ�
            if (GB_NetBallShotCount > BALLMAX) {
                GB_NetBallShotCount = BALLMAX;
            }

            // ���݂̃{�[���ێ������e�L�X�g�ɔ��f
            CP_BallHoldText.text = $"{GB_NetBallShotCount}";
        }
    }

    /// <summary> 
    /// �ގ�����(�R�[���o�b�N�֐�)
    /// </summary> 
    /// <param name="pm_OtherPlayer">���v���C���[</param> 
    /// <returns>�Ȃ�</returns>
    public override void OnPlayerLeftRoom(Player pm_OtherPlayer) {
        // ���O�̊m�F
        Debug.Log($"{pm_OtherPlayer.NickName}���ޏo���܂���");

        //���s�����͑ޏo�p�l���͕\�����Ȃ�
        if (!GB_BattleJudgeShowFlag) {
            SC_GameManager.LocalBallDelete();
            SC_GameManager.LeftRoom();
        }
    }
}

// �l�b�g���[�N�I�u�W�F�N�g�ʒm�X�N���v�g
public static class SC_NetObjectInform {
    // �O���[�o���ϐ�
    public struct BattleJudge {
        public Vector2 LosePos;
        public bool WinFlag;
    }

    public static Subject<BattleJudge> GB_JudgeStatus = null;

    /// <summary> 
    /// �l�b�g���[�N�I�u�W�F�N�g�ʒm������
    /// </summary> 
    /// <param name="pm_Player">�v���C���[���</param> 
    /// <returns>�Ȃ�</returns>
    public static void SetEventInform() {
        // ���s���l�b�g���[�N�}�l�[�W���ɕԂ�
        GB_JudgeStatus = new Subject<BattleJudge>();
    }

    /// <summary> 
    /// �l�b�g���[�N�v���C���[���珟�s������擾
    /// </summary> 
    /// <param name="pm_Player">�v���C���[���</param> 
    /// <param name="pm_WinFlag">�v���C���[���s(true:����,false:����)</param>
    /// <param name="pm_PlayerPos">�v���C���[�ʒu</param>
    /// <returns>�Ȃ�</returns>
    public static void BattleJudgeInform(this Player pm_Player, bool pm_WinFlag, Vector2 pm_PlayerPos) {
        // ���s���ʕϐ�
        BattleJudge w_BattleJudgeStatus;
        w_BattleJudgeStatus.WinFlag = pm_WinFlag;
        w_BattleJudgeStatus.LosePos = pm_PlayerPos;

        // NetworkManager�ɒʒm
        GB_JudgeStatus.OnNext(w_BattleJudgeStatus);
        GB_JudgeStatus.OnCompleted();
    }
}
