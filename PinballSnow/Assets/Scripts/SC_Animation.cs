using UnityEngine;
using UniRx;

// �A�j���[�V�����X�N���v�g
public class SC_Animation: MonoBehaviour {
    public Subject<int> GB_BomInform = new Subject<int>();    // �����A�j���[�V�������I����ɕԂ�
    private const string BOMPRINTEVENT = "Bom";         // PrintEvent�̌��ʕ�����
    private const int BOMANIMSTATUS = default;          // �ʒm�ϐ��Ƃ��Ďg��(��ϐ�)

    /// <summary> 
    /// �A�j���[�V�����̈ʒu��ݒ�
    /// </summary> 
    /// <param name="pm_Position">�A�j���[�V�����ʒu</param> 
    /// <returns>�Ȃ�</returns>
    public void PositionSet(Vector2 pm_Position) {
        GetComponent<RectTransform>().anchoredPosition = pm_Position;
    }

    /// <summary> 
    /// �A�j���[�V�����I���㏈��
    /// </summary> 
    /// <param name="pm_PrintEvent">AnimationClip����</param> 
    /// <returns>�Ȃ�</returns>
    public void PrintEvent(string pm_PrintEvent) {
        // �����A�j���[�V�����I����
        if (pm_PrintEvent == BOMPRINTEVENT) { GB_BomInform.OnNext(BOMANIMSTATUS); }
        
        // �A�j���[�V�������폜
        Destroy(gameObject);
    }
}
