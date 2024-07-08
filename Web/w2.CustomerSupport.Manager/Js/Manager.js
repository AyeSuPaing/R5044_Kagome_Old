/*
=========================================================================================================
  Module      : マネージャ側javascript(Manager.js)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/

//=========================================================================================
//
// 左メニュー画像用スクリプト
// 
//=========================================================================================
var pic1 = new Array();
var pic2 = new Array();

//------------------------------------------------------
// 画像読み込み
//    引数img_name, in_img, out_img
//------------------------------------------------------
function preload()
{
	var arg = preload.arguments;
	pic1[arg[0]] = new Image(); pic1[arg[0]].src = arg[1];
	pic2[arg[0]] = new Image(); pic2[arg[0]].src = arg[2];
}

//------------------------------------------------------
// マウスインイベント
//------------------------------------------------------
function mouse_in(img_name)
{
	find_object(img_name).src = pic1[img_name].src;
}

//------------------------------------------------------
// マウスアウトイベント
//------------------------------------------------------
function mouse_out(img_name)
{
	find_object(img_name).src = pic2[img_name].src;
}

//------------------------------------------------------
// オブジェクト検索
//------------------------------------------------------
function find_object(n, d)
{
	var p, i, x;

	if (!d)
	{
		d = document;
	}

	if ((p = n.indexOf("?")) > 0 && parent.frames.length)
	{
		d = parent.frames[n.substring(p + 1)].document;
		n = n.substring(0, p);
	}
	if (!(x = d[n]) && d.all)
	{
		x = d.all[n];
	}

	for (i = 0; (!x && i < d.forms.length); i++)
	{
		x = d.forms[i][n];
	}

	for (i = 0; (!x && d.layers && i < d.layers.length); i++)
	{
		x = find_object(n, d.layers[i].document);
	}
	if (!x && d.getElementById)
	{
		x = d.getElementById(n);
	}

	return x;
}

//=========================================================================================
//
// リストセレクタ用スクリプト
// 
//=========================================================================================
var class_bgcolover = 'list_item_bg_over'; // マウスオーバー時の色
var class_bgcolout1 = 'list_item_bg1'; // マウスアウトしたときの色１
var class_bgcolout2 = 'list_item_bg2'; // マウスアウトしたときの色２
var class_bgcolout3 = 'list_item_bg3'; // マウスアウトしたときの色３
var class_bgcolouts = new Array(class_bgcolout1, class_bgcolout2, class_bgcolout3);
var class_bgcolclck = 'list_item_bg_click'; // マウスクリックしたときの色

var xx_mousedown = 0; 					// マウスダウンしたときのX座標
var yy_mousedown = 0; 					// マウスダウンしたときのY座標

var clickControl = ""; // アクティブなコントロール
var selectionText; // 選択されている文字

//------------------------------------------------------
// マウスオーバーイベント
//------------------------------------------------------
function listselect_mover(obj)
{
	if (obj != null)
	{
		obj.className = class_bgcolover;
	}
}

//------------------------------------------------------
// マウスアウトイベント
//------------------------------------------------------
function listselect_mout(obj, lineIdx)
{
    if (obj != null)
    {
        if (lineIdx == 0) obj.className = class_bgcolout1;
        else if (lineIdx == 1) obj.className = class_bgcolout2;
        else if (lineIdx == 2) obj.className = class_bgcolout3;
    }
}
// マウスアウトイベント２（既存向け）
function listselect_mout1(obj)
{
    listselect_mout(obj, 0);
}
// マウスアウトイベント２（既存向け）
function listselect_mout2(obj)
{
    listselect_mout(obj, 1);
}
// マウスアウトイベント３（既存向け）
function listselect_mout3(obj)
{
    listselect_mout(obj, 2);
}

//------------------------------------------------------
// マウスダウンイベント（座標取得）
//------------------------------------------------------
function listselect_mdown()
{
	if (navigator.appName.charAt(0) == "M")
	{
		// IE
		funcMouseMove(event);
	}
	//if (navigator.appName.charAt(0)=="N"){
	// Mozilla用
	//}

	xx_mousedown = mouseX;
	yy_mousedown = mouseY;
}

//------------------------------------------------------
// マウスダウンイベント（座標とフォーカス取得）
//------------------------------------------------------
function listselect_mdown1() {
  clickControl = document.activeElement;
  selectionText = document.getSelection().toString();
  if (navigator.appName.charAt(0) === "M") {
    // IE
    funcMouseMove(event);
  }

  xx_mousedown = mouseX;
  yy_mousedown = mouseY;
}

//------------------------------------------------------
// マウスクリックイベント（座標判定して画面遷移）
//------------------------------------------------------
function listselect_mclick(obj, target_url, noClickCheck)
{
	if (navigator.appName.charAt(0) == "M")
	{
	    funcMouseMove(event);
	}

	// 20マウスクリックしてから離すまでが指定ドット以下の場合、画面遷移
	if (noClickCheck || ((Math.abs(xx_mousedown - mouseX) < 25) && (Math.abs(yy_mousedown - mouseY) < 25)))
	{
		obj.className = class_bgcolclck;
		if (target_url != null) location.href = target_url;

		return true;
	}
	return false;
}

//------------------------------------------------------
// マウス座標取得用
//------------------------------------------------------
// マウス座標保持用変数
var mouseX = 0;
var mouseY = 0;

if (navigator.appName.charAt(0) == "N")
{
	// NN,Firefox用（マウス座標取得用イベントリスナ）
	document.addEventListener('mousemove', funcMouseMove, false);
}

// マウス座標取得用関数
function funcMouseMove(event)
{
    if (event != null)
    {
        mouseX = event.clientX;
        mouseY = event.clientY;
    }
}

//------------------------------------------------------
// マウスクリックイベント（テーブルの内容を挿入）
//------------------------------------------------------
function listselect_mclick_Insert(obj, url) {
  if (navigator.appName.charAt(0) === "M") {
    funcMouseMove(event);
  }
  // 20マウスクリックしてから離すまでが指定ドット以下の場合、画面遷移
  if ((Math.abs(xx_mousedown - mouseX) < 25) && (Math.abs(yy_mousedown - mouseY) < 25)) {
    obj.className = class_bgcolclck;
    if (clickControl.id === url) {
      var head = clickControl.value.substr(0, clickControl.selectionStart);
      var foot = clickControl.value.substr(clickControl.selectionEnd);
      var replacementTag = obj.cells[1].innerText;
      if (replacementTag.slice(0, 1) === "<") {
        replacementTag = replacementTag + selectionText + replacementTag.slice(0, 1) + "/" + replacementTag.slice(1);
      }
      var endPsition = clickControl.selectionStart + replacementTag.length;

      //置換タグを挿入しフォーカスを合わせる
      clickControl.blur();
      clickControl.focus();
      clickControl.value = head + replacementTag + foot;
      clickControl.setSelectionRange(endPsition, endPsition);
    }
  }
}

//------------------------------------------------------
// ポップアップウィンドウ用
//------------------------------------------------------
// 別ウィンドウ表示
function open_window(link_file, window_name, window_type) {
	var new_win = window.open(link_file, window_name, window_type);
	try
	{
		new_win.focus();
	}
	catch (Exception) 
	{
		// なにもしない //
	}
}

// 別ウィンドウ表示(ウィンドウ取得)
function open_and_get_window(link_file, window_name, window_type)
{
	var new_win = window.open(link_file, window_name, window_type);
	try
	{
		new_win.focus();
	}
	catch (Exception)
	{
		// なにもしない //
	}

	return new_win;
}

// サポートサイトウィンドウ表示
function open_supportsite(url) {
    window.open(url, "_support", "width=800px, height=640px,status=yes, toolbar=yes, location=yes, menubar=yes, scrollbars=yes, resizable=yes");
}

// 非同期二重押しチェック
function setDoubleSubmitCheck()
{
    var mng = Sys.WebForms.PageRequestManager.getInstance();
    mng.add_initializeRequest(
		function (sender, args)
		{
		    // 現在、実行中の非同期通信が存在したら後続の処理をキャンセル
		    if (mng.get_isInAsyncPostBack()) args.set_cancel(true);
		}
	);
}

/*
 * テキストボックスの文字数カウント
 * textbox_id：テキストボックスのID
 * max：最大文字数
 * max_count_display：最大文字数を表示するかどうか。bool
 * text_count：「文字数」「文字カウント」等任意のテキスト表示用
 */
function set_text_count(textbox_id, max, max_count_display, text_count)
{
	// 現在のカウント数の表示
	var nowCountHtml = '<p  style="font-weight: bold;"><span>' + text_count + '：</span><span class="count_' + textbox_id + '">' + 0 + '</span>' + (max_count_display ? (' / ' + max) : '') + '</p>';
	var count_area = $(nowCountHtml).insertAfter($('#' + textbox_id));
	$('#' + textbox_id).bind('keydown keyup keypress change', function ()
		{
			var thisValueLength = $(this).val().length;
			$('.count_' + textbox_id).html(thisValueLength);

			// 最大数チェック
			if (thisValueLength > max)
			{
				count_area.css('color', '#ff0000');
			}
			else {
				count_area.css('color', '#505050');
			}
		});

	// 初期設定
	$('#' + textbox_id).trigger("keyup");
}
