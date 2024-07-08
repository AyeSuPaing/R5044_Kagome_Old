/*
=========================================================================================================
Module      : マネージャ側javascript(Manager.js)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2006 All Rights Reserved.
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
var class_bgcolout1 = 'list_item_bg1'; // マウスアウトしたときの色１
var class_bgcolout2 = 'list_item_bg2'; // マウスアウトしたときの色２
// マウスアウトしたときの色３
var class_bgcolout3 = 'list_item_bg';
var class_bgcolover = 'list_item_bg_over'; // マウスオーバー時の色
var class_bgcolclck = 'list_item_bg_click'; // マウスクリックしたときの色

var xx_mousedown = 0; 					// マウスダウンしたときのX座標
var yy_mousedown = 0; 					// マウスダウンしたときのY座標

//------------------------------------------------------
// マウスオーバーイベント
//------------------------------------------------------
function listselect_mover(obj)
{
	obj.className = class_bgcolover;
}

//------------------------------------------------------
// マウスアウトイベント１
//------------------------------------------------------
function listselect_mout1(obj)
{
	obj.className = class_bgcolout1;
}

//------------------------------------------------------
// マウスアウトイベント２
//------------------------------------------------------
function listselect_mout2(obj)
{
	obj.className = class_bgcolout2;
}

// マウスアウトイベント３
function listselect_mout3(obj) {
	obj.className = class_bgcolout3;
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

// マウスダウン
function listselect_mdown2(obj) {
	obj.className = class_bgcolclck;
}

//------------------------------------------------------
// マウスクリックイベント（座標判定して画面遷移）
//------------------------------------------------------
function listselect_mclick(obj, target_url)
{
	if (navigator.appName.charAt(0) == "M")
	{
		funcMouseMove(event);
	}

	// 20マウスクリックしてから離すまでが指定ドット以下の場合、画面遷移
	if ((Math.abs(xx_mousedown - mouseX) < 25) && (Math.abs(yy_mousedown - mouseY) < 25))
	{
		obj.className = class_bgcolclck;
		location.href = target_url;
	}
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
	mouseX = event.clientX;
	mouseY = event.clientY;
}

//------------------------------------------------------
// ポップアップウィンドウ用
//------------------------------------------------------
// 別ウィンドウ表示
function open_window(link_file, window_name, window_type)
{
	var new_win = window.open(link_file, window_name, window_type);
	new_win.focus();
}

// サポートサイトウィンドウ表示
function open_supportsite(url) {
	window.open(url, "_support", "width=800px, height=640px,status=yes, toolbar=yes, location=yes, menubar=yes, scrollbars=yes, resizable=yes");
}

/*
 * テキストボックスの文字数カウント
 * textbox_id：テキストボックスのID
 * max：最大文字数
 * max_count_display：最大文字数を表示するかどうか。bool
 */
function set_text_count(textbox_id, max, max_count_display) {
	// 現在のカウント数の表示
	var nowCountHtml = '<p  style="font-weight: bold;"><span>カウント数：</span><span class="count_' + textbox_id + '">' + 0 + '</span>' + (max_count_display ? (' / ' + max) : '') + '</p>';
	var count_area = $(nowCountHtml).insertAfter($('#' + textbox_id));
	$('#' + textbox_id).bind('keydown keyup keypress change', function () {
		var thisValueLength = $(this).val().length;
		$('.count_' + textbox_id).html(thisValueLength);

		//最大数チェック
		if (thisValueLength > max) {
			count_area.css('color', '#ff0000');
		} else {
			count_area.css('color', '#505050');
		}
	});

	//初期設定
	$('#' + textbox_id).trigger("keyup");
}

/*
 * テキストボックスの文字数警告
 * textbox_id：テキストボックスのID
 * count_of_alert_min：警告を出す最小文字数
 * count_of_alert_max：警告を出す最大文字数
* alert_text:表示する警告文
 */
function set_text_count_alert(textbox_id, count_of_alert_min, count_of_alert_max, alert_text) {
	var nowCountHtml = '<p style="font-weight: bold;"><span class="count_' + textbox_id + '"></span></p>';
	var countArea = $(nowCountHtml).insertAfter($('#' + textbox_id));

	$('#' + textbox_id).bind('keydown keyup keypress change', function () {
		var thisValueLength = $(this).val().length;

		//最大数チェック
		if ((thisValueLength >= count_of_alert_max)
			|| (thisValueLength <= count_of_alert_min)) {
			$('.count_' + textbox_id).html(alert_text);
			countArea.css('color', '#ff0000');
		} else {
			$('.count_' + textbox_id).html('');
		}
	});

	//初期設定
	$('#' + textbox_id).trigger("keyup");
}

/*
 * uc:DateTimeInputの選択値をクリアする
 * id：親階層のタグid
 */
function datePeriodClear(id) {
	var dataSelect = $("#" + id).children("select");
	for (var i = 0; i < dataSelect.length; i++) {
		$("#" + dataSelect[i].id).val('');
	}
}
