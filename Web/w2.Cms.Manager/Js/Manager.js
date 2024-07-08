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
 * max_count_dispay：最大文字数を表示するかどうか。bool
 */
function set_text_count(textbox_id, max, max_count_dispay) {
    // 現在のカウント数の表示
    var nowCountHtml = '<p  style="font-weight: bold;"><span>カウント数：</span><span class="count_' + textbox_id + '">' + 0 + '</span></p>';
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

    // 最大カウント数の表示
    if (max_count_dispay) {
        var maxCountHtml = '<p  style="font-weight: bold; color:#ff0000; "><span>最大カウント数：' + max + '</span></p>';
        $(maxCountHtml).insertAfter($('#' + textbox_id));
    }
}

/**
 * 差分表示エリア構築
 * param {} type 端末タイプ
 */
function diff_main_content_model(type) {
    this.type = type;
    this.diffModels = [];
    /**
     * 差分確認対象エリアの追加
     * param {} key 項目名
     * param {} beforeCodeContentId 差分確認元エリアID
     * param {} afterCodeContentId 差分確認元エリアID
     */
    this.addModel = function (key, beforeContentId, afterContentId) {
        this.diffModels.push(new diff_content_model(key, beforeContentId, afterContentId));
    }
    /**
     * 差分内容の表示を生成
     * param {} selecter 生成先セレクタ 
     * returns {}　差分に有無 有り→true 無し→false
     */
    this.diffHtml = function (selecter, doesEdit) {
        if (typeof doesEdit === 'undefined') doesEdit = true;

        var content_diff_area = $('#' + selecter);
        content_diff_area.html(null);
        var diffCheck = false;
        var diffHtml = "";
        if (doesEdit === false) {
            diffHtml = '<h2 class="h2">' + this.type + "(変更しません)" + '</h2>';
            content_diff_area.append(diffHtml);
            return false;
        }
        this.diffModels.forEach(function (model) {
            var diff = model.diff();
            if (diff !== "") {
                diffHtml += '<h4 class="h4">' + model.Key + '</h4>';
                diffHtml +=
                    '<div class="custom-scroll-area" style="height: 170px;border: 1px solid #999;border-radius : 5px">' +
                    diff +
                    '</div>';
                diffCheck = true;
            }
        });
        diffHtml = '<h2 class="h2">' + this.type + ((diffCheck) ? "" : "(差分はありません)") + '</h2>' + diffHtml;
        content_diff_area.append(diffHtml);

        return diffCheck;
    }
}

/**
 * 項目別差分確認
 * param {} key 項目名
 * param {} beforeCodeContentId 差分確認元エリアID
 * param {} afterCodeContentId 差分確認元エリアID
 * returns {} 差分結果モデル
 */
function diff_content_model(key, beforeCodeContentId, afterCodeContentId) {
    this.Key = key;
    this.BeforeSelecter = $("#" + beforeCodeContentId);
    this.AfterSelecter = $("#" + afterCodeContentId);
    this.Dmp = new diff_match_patch();
    /**
     * 差分確認
     * returns {} 行差分結果
     */
    this.diff = function () {
        return diff_lineMode(this.BeforeSelecter.val(), this.AfterSelecter.val());
    };
}

/**
 * 行単位の差分確認
 * param {} text1 差分テキスト
 * param {} text2 差分テキスト
 */
function diff_lineMode(text1, text2) {
    // 比較時に片方が完全に空だと差分が取れないためスペースを入れる
    text1 = (text1 === "") ? " " : text1;
    text2 = (text2 === "") ? " " : text2;
    var dmp = new diff_match_patch();
    var a = dmp.diff_linesToChars_(text1, text2);
    var lineText1 = a.chars1;
    var lineText2 = a.chars2;
    var lineArray = a.lineArray;
    var diffs = dmp.diff_main(lineText1, lineText2, false);
    dmp.diff_charsToLines_(diffs, lineArray);

    if (diffs.length === 1) return "";
    return dmp.diff_prettyHtml(diffs);
}

/**
 * ACEコードエディタ設定
 * param {} editorId 対象エディタID
 * param {} editorHideId レスポンス用 隠しテキストエリアID
 * param {} resizeBtnId エディタの拡大縮小ボタンID
 */
function set_ace_code_editor(editorId, editorHideId, resizeBtnId) {
    var editor = ace.edit(
        editorId,
        {
            mode: "ace/mode/csharp",
            selectionStyle: "text",
            setTheme: "ace/theme/clouds"
        });
    editor.getSession().on("change",
        function () {
            $("#" + editorHideId).val(editor.getSession().getValue());
        });
	editor.setOptions({
		fontSize: "13pt"
	});

    var openClass = 'toggle-open';
    var openText = "拡大";
    var closeText = "縮小";
    $('#' + resizeBtnId).text(openText);
    $('#' + resizeBtnId).off("click").on("click",
        function () {
            if ($(this).hasClass(openClass)) {
                // 縮小
                $(this).removeClass(openClass)
                    .removeClass('btn-sub')
                    .addClass('btn-main')
                    .text(openText);
                $('#' + editorId).css("height", 200 + "px");
                editor.resize();
            } else {
                // 拡大
                $(this).addClass(openClass)
                    .removeClass('btn-main')
                    .addClass('btn-sub')
                    .text(closeText);
                $('#' + editorId).css("height", 1700 + "px");
                editor.resize();
            }
        });
}

// 他のオペレータが開いているかを確認
function checkOtherOperatorFileOpening(name) {
    $('.notification-message-warning').remove();
    return $.ajax({
        url: root_path + 'PagePartsDesignSub/CheckOtherOperatorFileOpening',
        type: 'POST',
        data: { fileName: name },
        timeout: 5000
    });
}

// 開いているファイルの名前を通知
function sendOpeningFileName(name) {
    $.ajax({
        url: root_path + 'PagePartsDesignSub/NoticeOperatorOpeningFile',
        type: 'POST',
        data: { fileName: name },
        timeout: 5000
    }).done(function (result) {
        if (result !== "") {
            $('.notification-message-warning').remove();
            notification.show(result, 'warning', 'fixed');
        }
    });
}

// ページ表示エラーメッセージ
function open_page_failed() {
    list.close();
    notification.show('ページを開くことに失敗しました。', 'warning', 'fadeout');
    loading_animation.end();
    $(".main-content-detail").html(null);
}

// 画像モーダルを表示
function imageModal(img, size) {
    $('#image-modal').html('<img src="' + img + '" style="width: auto;">');

    if (size === 'sp') {
        $('#image-modal-m-btn').click();
    } else {
        $('#image-modal-l-btn').click();
    }
}

// 入力データ取得
function get_input_date() {
    var pageDate = $('#input_form').serializeArray();
    return pageDate;
}

// 入力確認
function validate(name, param) {
    loading_animation.start();
    var formData = get_input_date();
    if (param != undefined) formData.push(param);
    $.ajax({
        type: "POST",
        url: "Validate",
        data: formData
    }).done(function(errorMessage) {
        setTimeout(function() { loading_animation.end(); }, 200);
        // 警告表示を削除
        $('.notification-message-warning').remove();
        if (errorMessage !== "") {
            notification.show(errorMessage, 'warning', 'fixed');
        } else {
            $('#' + name).click();
        }
        return false;
    });
}

// 削除によるバリデート
function validateForDelete(name, param) {
    loading_animation.start();
    var formData = get_input_date();
    if (param != undefined) formData.push(param);
    $.ajax({
        type: "POST",
        url: "ValidateForDelete",
        data: formData
    }).done(function (errorMessage) {
        setTimeout(function () { loading_animation.end(); }, 200);
        // 警告表示を削除
        $('.notification-message-warning').remove();
        if (errorMessage !== "") {
            notification.show(errorMessage, 'warning', 'fixed');
        } else {
            $('#' + name).click();
        }
        console.log(errorMessage);
        return false;
    });
}

// エンターキーによるsubmit禁止
function prohibit_submission(exceptName) {
  $("input").keydown(function (e) {
    if (e.target.name !== exceptName) {
      if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
        return false;
      } else {
        return true;
      }
    }
  });
}

// エンターキーによるsubmit禁止（除外項目複数指定可能）
function prohibit_submission_with_exceptions(exceptNames) {
  $("input").keydown(function (e) {
    var targetName = e.target.name;

    if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
      if (exceptNames.indexOf(targetName) === -1) {
        return false;
      }
    }
    return true;
  });
}

// 該当しない検索エラーメッセージ
function search_error_no_result() {
  var notification = {
    ini: function () {
      $('body').append('<div class="notification"></div>');
    },
    show: function (message, classname, type) {
      var id = Math.random().toString(36).slice(-8);
      $('.notification').append('<div class="notification-message notification-message-' + classname + '" id="notification-id-' + id + '">' + message + '<span class="notification-message-close"><span class="icon-close"></span></div></div>');
      setTimeout(function () {
        $('#notification-id-' + id).addClass('show');
      }, 200);
      if (type == 'fadeout') {
        setTimeout(function () {
          notification.hide(id);
        }, 5000);
      }
      $('#notification-id-' + id + ' .notification-message-close').off('click').on('click', function () {
        notification.hide(id);
      });
    },
    hide: function (id) {
      $('#notification-id-' + id + '').remove();
    }
  }
}
