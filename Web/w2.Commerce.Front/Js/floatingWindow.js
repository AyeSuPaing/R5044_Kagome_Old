
//======================================================================================
// カート投入時のフローティングウィンドウ処理
//======================================================================================
var fwAddCart = "#addCartResultPopup";

// フローティングウィンドウの非表示処理
function closeAddcartPopup()
{
    $(fwAddCart)
    .hide();
}

// フローティングウィンドウの表示処理 (遷移先を引数で受け取る)
function displayAddCartPopup(AddCartRedirectSetting)
{
    // 画面遷移を指定する場合にはフローティングウィンドウ非表示
    if (AddCartRedirectSetting != "") return;

    // 表示位置の設定（画面中央）
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

    // 表示処理
    $(fwAddCart).css('top', top).css('left', left)
	.hide()
    .appendTo('body')
	.fadeIn() // フェードイン
	.delay(3000).fadeOut(); // 3秒後に非表示へ
}
