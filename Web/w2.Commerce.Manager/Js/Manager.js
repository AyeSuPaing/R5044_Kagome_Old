/*
=========================================================================================================
	Module      : マネージャ側javascript(Manager.js)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
	Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

//=========================================================================================
//
// 左メニュー画像用スクリプト
// 
//=========================================================================================
var pic1 = new Array();
var pic2 = new Array();

////------------------------------------------------------
//// 画像読み込み
////    引数img_name, in_img, out_img
////------------------------------------------------------
//function preload()
//{
//	var arg = preload.arguments;
//	pic1[arg[0]] = new Image(); pic1[arg[0]].src = arg[1];
//	pic2[arg[0]] = new Image(); pic2[arg[0]].src = arg[2];
//}

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
// マウスアウトイベント１
//------------------------------------------------------
function listselect_mout1(obj)
{
	if (obj != null)
	{
		obj.className = class_bgcolout1;
	}
}

//------------------------------------------------------
// マウスアウトイベント２
//------------------------------------------------------
function listselect_mout2(obj)
{
	if (obj != null)
	{
		obj.className = class_bgcolout2;
	}
}

//------------------------------------------------------
// マウスアウトイベント３
//------------------------------------------------------
function listselect_mout3(obj) {
	if (obj != null) {
		obj.className = class_bgcolout3;
	}
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
	//if (navigator.appName.charAt(0)=="N"){
	// Mozilla用
	//}

	xx_mousedown = mouseX;
	yy_mousedown = mouseY;
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
// マウスクリックイベント（テーブルの内容を挿入）
//------------------------------------------------------
function listselect_mclick_Insert(obj, subject, body, subjectMobile , bodyMobile, bodyHtml)
{
	if (navigator.appName.charAt(0) === "M") {
		funcMouseMove(event);
	}
	// 20マウスクリックしてから離すまでが指定ドット以下の場合、画面遷移
	if ((Math.abs(xx_mousedown - mouseX) < 25) && (Math.abs(yy_mousedown - mouseY) < 25)) {
		obj.className = class_bgcolclck;
		if (clickControl.id === body
			|| clickControl.id === subject
			|| clickControl.id === subjectMobile
			|| clickControl.id === bodyMobile
			|| clickControl.id === bodyHtml) {
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
			clickControl.setSelectionRange(endPsition,endPsition);
		}
	}
}

//------------------------------------------------------
// マウスクリックイベント（使えないメールテンプレートタグを表示)
//------------------------------------------------------
function MoveCheck(body, subject, subjectMobile, bodyMobile, tag, bodyHtml) {
	tag = document.getElementById(tag).value;
	var messeage = tagValidate(subject, tag) + tagValidate(body, tag);

	if ($('#' + bodyHtml).length > 0)
	{
		messeage += tagValidate(bodyHtml, tag);
	}

	if (bodyMobile !== "" && subjectMobile !== "") {
		messeage = messeage + tagValidate(subjectMobile, tag) + tagValidate(bodyMobile, tag);
	}
	if (messeage !== "") {
		return confirm("有効ではないタグが存在しますがよろしいですか？" + messeage);
	}
	return true;
}

//------------------------------------------------------
// マウスクリックイベント（使えないメールテンプレートタグを表示)
//------------------------------------------------------
function tagValidate(control, tag) {
	var tagArray = tag.split(",");
	var pattern = /<[/]?@@[^@<>]+@@>|@@[^@<>]+@@/g;
	var messeage = "";
	var reprecementTag = document.getElementById(control).value.replace(/\r\n/g, "").match(pattern);
	if (reprecementTag !== null)
	{
		for (var i = 0; i < reprecementTag.length; i++) {
			if (tagArray.indexOf(reprecementTag[i]) < 0) {
				console.log(reprecementTag[i]);
				if (reprecementTag[i].indexOf(":") !== -1) {
					 continue;
				}

				//囲みタグの処理
				if (reprecementTag[i].slice(1, 2) === "/") {
					var tee = reprecementTag[i].replace("/", "");
					if (tagArray.indexOf(tee) < 0) {
						messeage += "\n" + reprecementTag[i];
					}
					continue;
				}
				messeage += "\n" + reprecementTag[i];
			}
		}
	}
	return messeage;
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

// set scrollLeft two table
function scrollLeftTwoTable(tableTarget, tableReference) {
	$("." + tableReference).scroll(function () {
		$("." + tableTarget).scrollLeft($("." + tableReference).scrollLeft());
	});
}

// set scrollTop two table
function scrollTopTwoTable(tableTarget, tableReference) {
	$("." + tableReference).scroll(function () {
		$("." + tableTarget).scrollTop($("." + tableReference).scrollTop());
	});
}

// set same width two table
function setWidthTwoTable(tableTarget, tableReference) {
	// ヘッダサイズ調整
	var ColNums = $("." + tableTarget).find("tr:first td").length;
	for (var i = 1; i < ColNums + 1; i++) {
		var row = $("." + tableReference + " td:nth-child(" + i + ")").width();
		$("." + tableReference + " td:nth-child(" + i + ")").width(row);
		$("." + tableTarget + " td:nth-child(" + i + ")").width(row);
	}
}

// set same height two table
function setHeightTwoTable(tableTarget, tableReference) {
	var RowNums = $("." + tableTarget).find("tr").length;
	for (var i = 1; i < RowNums + 1; i++) {
		var height = $("." + tableReference + " tr:nth-child(" + i + ")").outerHeight() + 1;
		$("." + tableReference + " tr:nth-child(" + i + ")").height(height);
		$("." + tableTarget + " tr:nth-child(" + i + ")").height(height);
	}
}

// set hover two table
function hoverTwoTable(tableTarget, tableReference) {
	$("table." + tableTarget + " tr").each(function (k, v) {
		var self = this;
		var rows = $('tr', $('table.' + tableReference));
		(function (index) { // index: 0 = second row; 1= 3rd row... of table#names
			$(self).hover(
								function () {
									rows.eq(index).addClass('list_item_bg_over');
								}, function () {
									rows.eq(index).removeClass('list_item_bg_over');
								}
						);
		})(k); // pass index
	});
}

// 項目メモ設定のPopup表示
function displayMemoPopup() {
	
	$('.tip_memo').each(function () {
		$(this).qtip({
			content: {
				text: '<iframe style="width:255px; height:215px;padding:0;margin:0" frameborder="0" src="' + $(this).attr('href-data') + '" />',
				title: $(this).attr('title'),
				button: 'Close'
			},
			show: {
				solo: true, // Only show one tooltip at a time
				event: 'click',
			},
			hide: {
				event: 'click',
			},
			position: {
				viewport: $(window),
				adjust: {
					scroll: false
				}
			},
			style: {
				classes: 'qtip-bootstrap',
			},
			events: {
				render: function(event, api) {
					$(this).draggable({ cursor: 'move' }); // Tooltipを移動できるためjqueryUIを利用する
				}
			}
		});
	});
}

function updateTooltip() {
	// Tooltipを閉じる時、dummyボタンのクリックTriggerでUpdatePanelの更新を実行する
	__doPostBack("<%= btnTooltipInfo.ClientID %>", "");
}

/*
 * テキストボックスの文字数カウント
 * textbox_id：テキストボックスのID
 * max：最大文字数
 * max_count_display：最大文字数を表示するかどうか。bool
 * text_count：「文字数」「文字カウント」等任意のテキスト表示用
 */
function set_text_count(textbox_id, max, max_count_display, text_count) {
	// 現在のカウント数の表示
	var nowCountHtml = '<p  style="font-weight: bold;"><span>' + text_count + '：</span><span class="count_' + textbox_id + '">' + 0 + '</span>' + (max_count_display ? (' / ' + max) : '') + '</p>';
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

// open window with post data
function open_window_with_post(url, windowName, keys, values, windowType) {
	var form = document.createElement("form");
	form.target = windowName;
	form.method = "POST";
	form.action = url;
	form.style.display = "none";
	if (keys && values && (keys.length === values.length))
		for (var i = 0; i < keys.length; i++) {
			var input = document.createElement("input");
			input.type = "hidden";
			input.name = keys[i];
			input.value = decodeURIComponent(values[i]);
			form.appendChild(input);
		}

	document.body.appendChild(form);
	var new_win = window.open("", windowName, windowType);

	try {
		form.submit();
		new_win.focus();
	}
	catch (exception) {
		// なにもしない //
	}
	finally {
		document.body.removeChild(form);
	}
}

// Post data to download invoice
function postDataToDownloadInvoice(url, windowName, keys, values) {
	var form = document.createElement("form");
	form.target = windowName;
	form.method = "POST";
	form.action = url;
	form.style.display = "none";

	if (keys && values && (keys.length === values.length)) {
		for (var index = 0; index < keys.length; index++) {
			var input = document.createElement("input");
			input.type = "hidden";
			input.name = keys[index];
			input.value = decodeURIComponent(values[index]);
			form.appendChild(input);
		}
	}

	document.body.appendChild(form);

	try {
		form.submit();
	}
	catch (exception) {
	}
	finally {
		document.body.removeChild(form);
		// Prevent form resubmission when page is refreshed
		history.back();
	}
}

// Textbox change search global zip
function textboxChangeSearchGlobalZip(
	globalZipId,
	eventTarget) {
	$('#' + globalZipId).keyup(function (e) {
		if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

		// Check length for global zipcode
		if (checkGlobalZipLength(globalZipId) == false) return;
		__doPostBack(eventTarget, "");
	});
}

// Check global zipcode length
function checkGlobalZipLength(globalZipCode) {
	var globalZipCodeLength = $('#' + globalZipCode).val().length;
	var isValid = ((globalZipCodeLength >= 3)
		&& (globalZipCodeLength <= 20));
	return isValid;
}

// Determining whether the key code is valid at the time of a key event
function isValidKeyCodeForKeyEvent(keyCode) {
	// Key code description
	// [9：Tab], [35：End], [36：Home], [37：←], [38：↑], [39：→], [40：↓]
	var invalidKeyCodes = [9, 35, 36, 37, 38, 39, 40];
	return ($.inArray(keyCode, invalidKeyCodes) == -1);
}
