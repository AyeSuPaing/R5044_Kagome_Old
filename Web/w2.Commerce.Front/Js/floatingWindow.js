
//======================================================================================
// �J�[�g�������̃t���[�e�B���O�E�B���h�E����
//======================================================================================
var fwAddCart = "#addCartResultPopup";

// �t���[�e�B���O�E�B���h�E�̔�\������
function closeAddcartPopup()
{
    $(fwAddCart)
    .hide();
}

// �t���[�e�B���O�E�B���h�E�̕\������ (�J�ڐ�������Ŏ󂯎��)
function displayAddCartPopup(AddCartRedirectSetting)
{
    // ��ʑJ�ڂ��w�肷��ꍇ�ɂ̓t���[�e�B���O�E�B���h�E��\��
    if (AddCartRedirectSetting != "") return;

    // �\���ʒu�̐ݒ�i��ʒ����j
    var scrollTop = 0;
    if (navigator.userAgent.match(/AppleWebKit\/\d.+Safari\/\d.+/))
    {
        scrollTop = document.body.scrollTop;
    }
    else
    {
        scrollTop = document.documentElement.scrollTop;
    }

    var top = (document.documentElement.clientHeight - $(fwAddCart).height()) / 2 + scrollTop;
    var left = ($('html').width() - $(fwAddCart).width()) / 2;

    // �\������
    $(fwAddCart).css('top', top).css('left', left)
	.hide()
    .appendTo('body')
	.fadeIn() // �t�F�[�h�C��
	.delay(3000).fadeOut(); // 3�b��ɔ�\����
}
