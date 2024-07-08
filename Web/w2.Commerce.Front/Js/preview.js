/*------------------------------------------------
* 複数選択プルダウン
------------------------------------------------*/
var multiselect = {
	ini: function () {
		var linkText = {
			checkAll: { text: '全選択', title: '全選択' },
			uncheckAll: { text: '全選択解除', title: '全選択解除' }
		};
		$(".multiselect").each(function () {
			var tgt = $(this);
			tgt.multiselect({
				buttonWidth: 'auto',
				menuWidth: 'auto',
				linkInfo: linkText,
				noneSelectedText: '選択してください',
				selectedList: 4,
				selectedText: "#個選択中"
			});
		});
	}
};
/*------------------------------------------------
* 日付選択カレンダー
------------------------------------------------*/
var input_datepicker = {
	ini: function (mode) {
		$('.input-datepicker').each(function () {
			$('.input-datepicker').datepicker({
				dateFormat: "yy/mm/dd",
				closeText: "閉じる",
				prevText: "&#x3C;前",
				nextText: "次&#x3E;",
				currentText: "今日",
				monthNames: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
				monthNamesShort: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
				dayNames: ["日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"],
				dayNamesShort: ["日", "月", "火", "水", "木", "金", "土"],
				dayNamesMin: ["日", "月", "火", "水", "木", "金", "土"],
				weekHeader: "週",
				isRTL: false,
				showMonthAfterYear: true,
				yearSuffix: "年",
				firstDay: 0, // 週の初めは月曜
				showButtonPanel: true // "今日"ボタン, "閉じる"ボタンを表示する
			});
		});

		$('.input-timepicker').each(function () {
			var input = $(this);

			input.off('focus').on('focus', function () {
				var l = input.offset().left;
				if (mode == "PC") {
					$('body').append('<div class="timepicker" style="position:fixed;bottom:0px;left:' + l + 'px;"><ul><li>00:00</li><li>01:00</li><li>02:00</li><li>03:00</li><li>04:00</li><li>05:00</li></ul><ul><li>06:00</li><li>07:00</li><li>08:00</li><li>09:00</li><li>10:00</li><li>11:00</li></ul><ul><li>12:00</li><li>13:00</li><li>14:00</li><li>15:00</li><li>16:00</li><li>17:00</li></ul><ul><li>18:00</li><li>19:00</li><li>20:00</li><li>21:00</li><li>22:00</li><li>23:00</li></ul><ul><li>23:59</li></ul></div>');
				}else{
					$('body').append('<div class="timepicker" style="width: 110px;position:fixed;bottom:0px;left:' + l + 'px;"><ul><li>00:00</li><li>01:00</li><li>02:00</li><li>03:00</li><li>04:00</li><li>05:00</li><li>06:00</li><li>07:00</li><li>08:00</li><li>09:00</li><li>10:00</li><li>11:00</li></ul><ul><li>12:00</li><li>13:00</li><li>14:00</li><li>15:00</li><li>16:00</li><li>17:00</li><li>18:00</li><li>19:00</li><li>20:00</li><li>21:00</li><li>22:00</li><li>23:00</li><li>23:59</li></ul></div>');
				}
				$('.timepicker li').off('click').on('click', function () {
					input.val($(this).text());
					$('.timepicker').remove();
				});
			});
			input.off('blur').on('blur', function () {
				setTimeout(function () {
					$('.timepicker').remove();
				}, 300);
			});
		});

		$('.access-authority-setting-period').each(function () {
			var obj = $(this);

			//クリアボタン
			obj.find('.access-authority-setting-period-start-clear').each(function () {
				$(this).off('click').on('click', function () {
					obj.find('.access-authority-setting-period-start-input input').val('');
				});
			});
			obj.find('.access-authority-setting-period-end-clear').each(function () {
				$(this).off('click').on('click', function () {
					obj.find('.access-authority-setting-period-end-input input').val('');
				});
			});
		});

	}
}
$(function () {
	if ($('#previewFooterPC').get(0)) {
		$('#Foot').css('padding-bottom', '150px');
		multiselect.ini();
		input_datepicker.ini("PC");
	}
	if ($('#previewFooterSP').get(0)) {
		$('#Foot').css('padding-bottom', '150px');
		multiselect.ini();
		input_datepicker.ini("SP");
	}
});

// ページ内アクションの無効化
function InvalidateAction() {
    // プレビュー時の選択バー内アクションは除外
    $('a').not('.preview-element-valid').each(function () {
        $(this).attr("href", "");
    });
    $('input').not(':hidden, .preview-element-valid').each(function () {
        $(this).prop("disabled", "true");
    });

    $('select').not('.preview-element-valid').each(function () {
        $(this).prop("disabled", "true");
    });
}