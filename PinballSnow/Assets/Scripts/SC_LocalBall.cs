using UniRx;
using UnityEngine;
using UnityEngine.UI;

// ���[�J���{�[���X�N���v�g
public class SC_LocalBall : MonoBehaviour {
    // �I�u�W�F�N�g�ϐ�
    [SerializeField] private GameObject OB_Arrow = null;          // ���[�J���{�[���̖��            

    // �O���[�o���ϐ�
    public Subject<ScoreBlockInform> GB_ScoreBlockCollision =
                new Subject<ScoreBlockInform>();     // �u���b�N���Փˎ��ɃQ�[���}�l�[�W���ɕԂ�

    // �X�R�A�u���b�N(GameManager�ɒʒm)
    public struct ScoreBlockInform {
        public int Status;
        public Vector2 Position;
    }

    private int   GB_CollisionCount = 0;       // �Փˉ�
    private Vector2 GB_InMousePos;             // �}�E�X�����������̈ʒu�擾
    private Vector2 GB_OutMousePos;            // �}�E�X�𗣂������̈ʒu�擾

    /*************** �萔 **************/
    private const int PINBALLLAYER = 12;                          // �s���{�[�����C���[
    private const int BALLSHOTLAYER = 11;                         // �{�[���V���b�g���C���[
    private const int NOTHINGPARA = -1;                           // ��̃p�����[�^
    private const int BALLSHOTAREAINDEX = 1;                      // �{�[���V���b�g�G���A�C���f�b�N�X
    private const int COLORMAX = 255;                             // �J���[�ő�l
    private const float COLLISIONMAX = 10.0f;                     // �Փˍő吔(�Փ˂���萔�ȏ㒴����Ɣ��˃G���A�ɖ߂�)
    private const float ARROWSIZEX = 30.0f;                       // ���̃T�C�YX
    private const float ARROWMAXSIZEY = 120.0f;                   // ����MAX�T�C�YY
    private const float ARROWMINSIZEY = 70.0f;                    // ����MIN�T�C�YY
    private const float SHOTRAITO = 10.0f;                        // ���˔{��
    private const float SHOTX = 0.0f;                             // ����X
    private Vector2 BALLSHOTAREA = new Vector2(-377.5f, -227.5f); // �{�[���V���b�g�G���A�Ɉʒu�ړ�
    private Vector2 PINBALLUPAREA = new Vector3(-278f, 240f);     // �p�`���R�G���A�Q�Ɉʒu�ړ�

    // �X�R�A�u���b�N(�Փ˕ϐ�)
    private struct ScoreBlockCollision {
        public string BlockName;
        public int    ToArea;
        public int    Status;

        public ScoreBlockCollision(string p1, int p2, int p3) {
            this.BlockName = p1;
            this.ToArea = p2;
            this.Status = p3;
        }
    }

    // �X�R�A�u���b�N�Փˌ�̏��
    private ScoreBlockCollision[] SCOREBLOCKCOL = {
        new ScoreBlockCollision("Block1-1", 1, 1),
        new ScoreBlockCollision("Block1-2", 2, 1),
        new ScoreBlockCollision("Block1-3", 2, 1),
        new ScoreBlockCollision("Block1-4", 2, 1),
        new ScoreBlockCollision("Block1-5", 1, 1),
        new ScoreBlockCollision("Block2-1", 2, 1),
        new ScoreBlockCollision("Block2-2", 2, 3),
        new ScoreBlockCollision("Block2-3", 2, 1),
        new ScoreBlockCollision("Block2-4", 2, 2),
        new ScoreBlockCollision("Block2-5", 2, 4),
    };
    /*************** �萔 **************/

    /// <summary> 
    /// ������
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    public void Initialize() {
        //�����\��
        OB_Arrow.SetActive(false);
    }

    /// <summary> 
    /// ���[�J���{�[���Փˏ���
    /// </summary> 
    /// <param name="pm_Collision">�Փ˃I�u�W�F�N�g</param> 
    /// <returns>�Ȃ�</returns>
    private void OnCollisionEnter2D(Collision2D pm_Collision) {
        // �{�[���̈ʒu�ƃ{�[����ԕϐ�������
        int w_ToArea = NOTHINGPARA;
        int w_BallStatus = NOTHINGPARA;

        // �Փ˂����X�R�A�u���b�N�̈ʒu���擾
        Vector2 w_ColPosX = pm_Collision.gameObject.transform.localPosition;

        // �X�R�A�u���b�N�ɓ��������ꍇ�A�{�[���̏�Ԃ�ύX
        for (int i = 0; i < SCOREBLOCKCOL.Length; i++) {
            if (SCOREBLOCKCOL[i].BlockName == pm_Collision.gameObject.name) {
                w_ToArea = SCOREBLOCKCOL[i].ToArea;
                w_BallStatus = SCOREBLOCKCOL[i].Status;
                break;
            }
        }

        // �X�R�A�u���b�N�ȊO�̏Փ˂̓��^�[������
        if ( w_ToArea == NOTHINGPARA) { return; }

        // �{�[������萔�ȏ�Փ˂����ꍇ��������
        w_ToArea = BallDamage(w_ToArea);

        // �{�[���̈���ړ�
        if (w_ToArea == BALLSHOTAREAINDEX) { ToBallShotArea(); }  // �{�[�����˃G���A�Ɉړ�
        else { ToPinballUpArea(); }                              // �p�`���R��i�G���A�Ɉړ�

        // �{�[���̏�Ԃ��X�V
        StatusSet(w_BallStatus, w_ColPosX);
    }

    /// <summary> 
    /// ���[�J���{�[���̗͌���
    /// </summary> 
    /// <param name="pm_ToArea">����ʒu</param> 
    /// <returns>�Ȃ�</returns>
    private int BallDamage(int pm_ToArea) {
        // �X�R�A�u���b�N�ƃ{�[���Փˉ�
        GB_CollisionCount++;

        // �Փ˂��邽�тɐԐF�ɑ����Ă���
        float w_Color = COLORMAX - (COLORMAX * (GB_CollisionCount / COLLISIONMAX));
        gameObject.GetComponent<Image>().color = new Color32(COLORMAX, (byte)w_Color, (byte)w_Color, COLORMAX);

        // �X�R�A�u���b�N���w��񐔈ȏ�Փ˂����ꍇ�{�[�����˃G���A�Ɉړ�
        if (GB_CollisionCount > COLLISIONMAX) {
            GB_CollisionCount = 0;
            pm_ToArea = BALLSHOTAREAINDEX;
        }

        return pm_ToArea;
    }

    /// <summary> 
    /// �{�[����ԍX�V
    /// </summary> 
    /// <param name="pm_BallStatusNumber">�{�[���̏�Ԕԍ�</param> 
    /// <param name="pm_BlockColPos"�@   >�u���b�N�̏Փˈʒu</param> 
    /// <returns>�Ȃ�</returns>
    private void StatusSet(int pm_BallStatusNumber, Vector2 pm_BlockColPos) {
        // �Փ˃X�e�[�^�X
        ScoreBlockInform w_BlockCollisionEvent;
        w_BlockCollisionEvent.Status = pm_BallStatusNumber;
        w_BlockCollisionEvent.Position = pm_BlockColPos;

        // GmaeManager�Ɍ��ʂ�Ԃ�
        GB_ScoreBlockCollision.OnNext(w_BlockCollisionEvent);
    }

    /// <summary> 
    /// �p�`���R�G���A��i�Ɉړ�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void ToPinballUpArea() {
        // �s���{�[���G���A��i�Ɉړ�
        RectTransform w_LocalBallPos = GetComponent<RectTransform>();
        w_LocalBallPos.anchoredPosition = PINBALLUPAREA;
    }

    /// <summary> 
    /// �{�[���V���b�g�G���A�Ɉړ�
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void ToBallShotArea() {
        // ���[�J���{�[���̃R���|�[�l���g�ϐ�
        RectTransform w_LocalBallPos = GetComponent<RectTransform>();
        Rigidbody2D w_LocalBallRigid = GetComponent<Rigidbody2D>();

        // �{�[���V���b�g�G���A�̃��C���[�ɕύX
        gameObject.layer = BALLSHOTLAYER;

        // �{�[���V���b�g�G���A�Ƀ{�[�����ړ�
        w_LocalBallPos.anchoredPosition = BALLSHOTAREA;

        // RigidBody2D��������
        w_LocalBallRigid.velocity = Vector2.zero;
        w_LocalBallRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        w_LocalBallRigid.constraints = RigidbodyConstraints2D.None;
    }

    /// <summary> 
    /// �}�E�XIN����(�R�[���o�b�N�֐�)
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void OnMouseDown() {
        // �p�`���R�G���A�Ɉړ�������}�E�X�ŐG��Ȃ�
        if (gameObject.layer != PINBALLLAYER) {
            //�}�E�X���������ʒu���擾
            GB_InMousePos = Input.mousePosition;
        }
    }

    /// <summary> 
    /// �}�E�XUP����(�R�[���o�b�N�֐�)
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void OnMouseUp() {
        // �p�`���R�G���A�Ɉړ�������}�E�X�ŐG��Ȃ�
        if (gameObject.layer != PINBALLLAYER) {
            // ���[�J���{�[���𔭎�
            BallShot();
        }
    }

    /// <summary> 
    /// �}�E�XDRAG����(�R�[���o�b�N�֐�)
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void OnMouseDrag() {
        // �p�`���R�G���A�Ɉړ�������}�E�X�ŐG��Ȃ�
        if (gameObject.layer != PINBALLLAYER) {
            // �}�E�X�̃h���b�O�ʒu���擾 & �������ʒu���擾
            GB_OutMousePos = Input.mousePosition;
            BallArrow();
        }
    }

    /// <summary> 
    /// ���[�J���{�[���V���b�g
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void BallShot() {

        // �����\��
        OB_Arrow.SetActive(false);

        // �{�[���e���̃{�[���̒e�����v�Z
        float w_BallLengthY = (GB_InMousePos.y - GB_OutMousePos.y) * SHOTRAITO;

        // �{�[���̔��˗͂�ݒ�
        if (w_BallLengthY > ARROWMAXSIZEY * SHOTRAITO) {
            w_BallLengthY = ARROWMAXSIZEY * SHOTRAITO;
        } else if (w_BallLengthY < ARROWMINSIZEY * SHOTRAITO) {
            return;
        }

        // ���C���[��ύX
        gameObject.layer = PINBALLLAYER;

        // �{�[���𔭎�
        transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(SHOTX, w_BallLengthY));
    }

    /// <summary> 
    /// ���[�J���{�[���̖��\��
    /// </summary> 
    /// <param name="message">�Ȃ�</param> 
    /// <returns>�Ȃ�</returns>
    private void BallArrow() {
        // ���̒������v�Z
        float w_ArrowLengthY = (GB_InMousePos.y - GB_OutMousePos.y);

        // ����\��
        if (w_ArrowLengthY >= ARROWMINSIZEY) {
            OB_Arrow.SetActive(true);
        } else {
            OB_Arrow.SetActive(false);
        }

        // ���ɒ����𔽉f
        if (w_ArrowLengthY > ARROWMAXSIZEY) {
            OB_Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(ARROWSIZEX, ARROWMAXSIZEY);
        } else if (w_ArrowLengthY >= ARROWMINSIZEY) {
            OB_Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(ARROWSIZEX, w_ArrowLengthY);
        }
    }
}
