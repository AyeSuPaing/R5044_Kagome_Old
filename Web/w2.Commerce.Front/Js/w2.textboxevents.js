// �t�H�[�J�X�A�E�g�E�G���^�[�L�[�_�E���|�X�g�o�b�N�C�x���g�p���s���ԁE������
var lastBlurOnEnterTime;

// �t�H�[�J�X�A�E�g�E�G���^�[�L�[�_�E���|�X�g�o�b�N�C�x���g�p���s���ԏ�����
function InitializeLastBlurOnEnterTime()
{
    lastBlurOnEnterTime = "ok";
}
// �t�H�[�J�X�A�E�g�E�G���^�[�L�[�_�E���|�X�g�o�b�N�C�x���g�p���s���ԏ�����
function ResetLastBlurOnEnterTime()
{
    lastBlurOnEnterTime = null;
}
// �t�H�[�J�X�A�E�g�E�G���^�[�L�[�_�E���C�x���g���s�\����
function CheckBlurOnEnterEnabled()
{
    return (lastBlurOnEnterTime == "ok");
}