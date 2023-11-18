using UnityEngine;
using UniRx;

// �Q�[���}�l�[�W��
public class GameManager : MonoBehaviour {
    // �I�u�W�F�N�g�ϐ�
    [SerializeField] private GameObject OB_LocalBallPrefab = null;          // ���[�J���{�[���v���t�@�u
    [SerializeField] private GameObject OB_UpScoreStatusAnimPrefab = null;  // �s���{�[����̃X�e�[�^�X�v���t�@�u
    [SerializeField] private GameObject OB_DownScoreStatusAnimPrefab = null;// �s���{�[�����̃X�e�[�^�X�v���t�@�u
    [SerializeField] private GameObject OB_BomAnimPrefab = null;            // �����v���t�@�u
    [SerializeField] private GameObject OB_WinPanel = null;                 // �����p�l��
    [SerializeField] private GameObject OB_LosePanel = null;                // �s�k�p�l��
    [SerializeField] private GameObject OB_LeftRoomPanel = null;            // �ގ��p�l��

    // �X�N���v�g�ϐ�
    [SerializeField] private SC_GameSound      SC_GameSound = null;         // �Q�[���T�E���h
    [SerializeField] private SC_GameSE         SC_GameSE = null;            // �Q�[�����ʉ�
    [SerializeField] private SC_NetworkManager SC_NetworkManager = null;    // �l�b�g���[�N�}�l�[�W��
    [SerializeField] private SC_BlockManager   SC_BlockManager = null;      // �u���b�N�}�l�[�W��

    // �R���|�[�l���g�ϐ�
    [SerializeField] private Transform CP_LocalBalls = null;                // ���[�J���{�[���e�I�u�W�F�N�g
    [SerializeField] private Transform CP_Animations = null;                // �A�j���[�V�����e�I�u�W�F�N�g

    // �萔
    private const int   BATTLEBGM = 1;               // �o�g��BGM
    private const float PINBALLBORDER = -100.0f;     // �s���{�[���P�ƃs���{�[���Q�̃{�[�_�[
    private const int   NETBALLCREATE = 1;           // �l�b�g���[�N�{�[���𐶐�
    private const int   NETBALLMULTICFRATE = 2;      // �l�b�g���[�N�{�[����2�{�ɂ���
    private const int   LOCALBALLMAX = 10;           // ���[�J���{�[���̍ő吶����
    private Vector2     LOCALBALLPOS = new Vector2(-375f, -230f); // ���[�J���{�[���̐����ꏊ

    /// <summary> 
    /// �Q�[���J�n
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void Start() {
        // ������
        Initialize();
    }

    /// <summary> 
    /// ����������
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void Initialize() {
        // �X�N���v�g������
        SC_GameSound.Select(BATTLEBGM);     // BGM���Đ�
        SC_NetworkManager.Initialize();     // �l�b�g���[�N�}�l�[�W���̏�����
        SC_NetworkManager.PlayerCreate();   // �l�b�g���[�N�v���C���[����
        SC_BlockManager.Create();           // �l�b�g���[�N�u���b�N����

        // �Q�[�����s��ʂƑގ���ʂ��\��
        OB_WinPanel.SetActive(false);
        OB_LosePanel.SetActive(false);
        OB_LeftRoomPanel.SetActive(false);

        // ���[�J���{�[������
        LocalBallCreate();
    }

    /// <summary> 
    /// ���[�J���{�[������
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void LocalBallCreate() {
        // ���[�J���{�[�������I�u�W�F�N�g��錾
        GameObject   w_LocalBall;
        SC_LocalBall w_CompLocalBall;

        // ���[�J���{�[���̐���
        w_LocalBall = Instantiate(OB_LocalBallPrefab, CP_LocalBalls);
        w_LocalBall.GetComponent<RectTransform>().anchoredPosition = LOCALBALLPOS;
        w_CompLocalBall = w_LocalBall.GetComponent<SC_LocalBall>();
        w_CompLocalBall.Initialize();

        // �X�R�A�u���b�N�ɓ����������̏���
        w_CompLocalBall.GB_ScoreBlockCollision.Subscribe (
            w_ScoreBlock => ScoreBlockEvent(w_ScoreBlock)
        );
    }

    /// <summary> 
    /// �X�R�A�u���b�N�Փˌ㏈��
    /// </summary> 
    /// <param name="pm_ScoreBlock">�X�R�A�u���b�N�Փ˃X�e�[�^�X</param> 
    /// <returns>�Ȃ�</returns>
    private void ScoreBlockEvent(SC_LocalBall.ScoreBlockInform pm_ScoreBlock) {
        // �{�[���̏�ԍX�V
        BallStatusUpdate(pm_ScoreBlock);

        // �u���b�N�Փˎ��̃A�j���[�V������\��
        ScoreAnimation(pm_ScoreBlock);
    }

    /// <summary> 
    /// �{�[����ԍX�V
    /// </summary> 
    /// <param name="pm_ScoreBlock">�X�R�A�u���b�N�Փ˃X�e�[�^�X</param> 
    /// <returns>�Ȃ�</returns>
    private void BallStatusUpdate(SC_LocalBall.ScoreBlockInform pm_ScoreBlock) {
        // �X�R�A�u���b�N�̏Փˉ���炷
        SC_GameSE.GetStatus();

        // �Փ˂����u���b�N�ɂ���ď�Ԃ��X�V
        switch (pm_ScoreBlock.Status) {
            case 1:
                // �l�b�g���[�N�{�[���̔��˂����Z
                SC_NetworkManager.BallHold(NETBALLCREATE);
                break;
            case 2:
                // �l�b�g���[�N�{�[���̔��˂��Q�{
                SC_NetworkManager.BallHold(NETBALLMULTICFRATE);
                break;
            case 3:
                // ���[�J���{�[������(5�܂�)
                if (CP_LocalBalls.transform.childCount < LOCALBALLMAX) { LocalBallCreate(); }
                break;
            case 4:
                // �l�b�g���[�N�{�[���𔭎�
                SC_NetworkManager.BallShot();
                break;
            default:
                break;
        }
    }

    /// <summary> 
    /// �X�R�A�u���b�N�̃A�j���[�V�����\��
    /// </summary> 
    /// <param name="pm_ScoreBlock">�X�R�A�u���b�N�Փ˃X�e�[�^�X</param> 
    /// <returns>�Ȃ�</returns>
    private void ScoreAnimation(SC_LocalBall.ScoreBlockInform pm_ScoreBlock) {
        // �X�R�A�u���b�N�ɓ����������̃A�j���[�V������錾
        GameObject   w_Animation;
        SC_Animation w_CompAnim;

        // �X�R�A�A�j���[�V�����𐶐�
        if (pm_ScoreBlock.Position.y > PINBALLBORDER) {   
            // �s���{�[���G���A��i�ɃX�R�A�A�j���[�V�����𐶐�
            w_Animation = Instantiate(OB_UpScoreStatusAnimPrefab, CP_Animations);
        } else {
            // �s���{�[���G���A���i�ɃX�R�A�A�j���[�V�����𐶐�
            w_Animation = Instantiate(OB_DownScoreStatusAnimPrefab, CP_Animations);
        }
        
        // �X�R�A�A�j���[�V�����Ɉʒu���Z�b�g
        w_CompAnim = w_Animation.GetComponent<SC_Animation>();
        w_CompAnim.PositionSet(pm_ScoreBlock.Position);
    }

    /// <summary> 
    /// ���s���菈��
    /// </summary> 
    /// <param name="pm_BattleJudge">���s����X�e�[�^�X</param> 
    /// <returns>�Ȃ�</returns>
    public void BattleJudge(SC_NetObjectInform.BattleJudge pm_BattleJudge) {
        // �����I�u�W�F�N�g��錾
        GameObject w_Bom;
        SC_Animation w_CompBomAnim;

        // �s�k�̂ق��ɔ����A�j���[�V�����𐶐�
        w_Bom = Instantiate(OB_BomAnimPrefab, CP_Animations);
        w_CompBomAnim = w_Bom.GetComponent<SC_Animation>();
        w_CompBomAnim.PositionSet(pm_BattleJudge.LosePos);

        // �����A�j���[�V������ɏ��s��ʂ�\��
        w_CompBomAnim.GB_BomInform.Subscribe (
            w_Null => BattleJudgePanel(pm_BattleJudge.WinFlag)
        );

        // ���s���ʉ�
        if (pm_BattleJudge.WinFlag) { SC_GameSE.Win(); } 
        else { SC_GameSE.Lose(); }
        
        // ���[�J���{�[���폜
        LocalBallDelete();
    }

    /// <summary> 
    /// ���s�p�l���\��
    /// </summary> 
    /// <param name="pm_WinFlag">�����t���O(true:���� false:����)</param> 
    /// <returns>�Ȃ�</returns>
    public void BattleJudgePanel(bool pm_WinFlag) {
        // ���s�����ʂ�\��
        OB_WinPanel.SetActive(pm_WinFlag);
        OB_LosePanel.SetActive(!pm_WinFlag);
    }

    /// <summary> 
    /// ���[�J���{�[�����폜
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void LocalBallDelete() {
        // �Q�[���I����̓��[�J���{�[�����폜
        foreach (Transform w_ChildTransform in CP_LocalBalls.transform) {
            Destroy(w_ChildTransform.gameObject);
        }
    }

    /// <summary> 
    /// �ގ��p�l����\��
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void LeftRoom() {
        // �ގ��p�l����\��
        OB_LeftRoomPanel.SetActive(true);
    }
}
