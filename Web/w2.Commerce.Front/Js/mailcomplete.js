(function ($) {
	// プラグイン定義
	$.fn.mailcomplete = function (options) {
		var defaults = {
			source: window.mailDomains
		};

		var $mailTextBox = $("#" + options.textBoxId);
		var $mailSuggestContainer = $("#" + options.mailDomainList);
		var $settings = $.extend(defaults, options);
		var mailDomainSuggests = [];
		var elements = this;
		var currentVal;
		var autocompleteFlag = false;

		elements.on("keyup", function () {
			currentVal = elements.val();
			$mailSuggestContainer.empty();
			if (currentVal.match(/@/)) {
				var atPosition = currentVal.indexOf('@');
				if (autocompleteFlag === false) {
					autocompleteFlag = true;
				}

				// @の前後に文字列が存在するか？
				else if ((/@[a-zA-Z0-9_-]+/.test(currentVal)) && (0 !== atPosition)) {
					var domains = getMailDomainSuggest(currentVal.indexOf('@') + 1);
					if (domains.length === 0) return;
					displayBorderLine(true);
					setAutoComplete(elements, domains);
				} else {
					displayBorderLine(false);
				}
			} else if (autocompleteFlag) {
				autocompleteFlag = false;
				displayBorderLine(false);
			}
		});

		// 候補となるメールドメインを取得
		function getMailDomainSuggest(position) {
			var results = [];

			for (var i = 0; i < $settings.source.length; i++) {
				var domain = currentVal.substr(position);
				if (domain === $settings.source[i].slice(0, currentVal.length - position)) {
					results.push($settings.source[i]);
				}
			}
			return results;
		}

		// メールドメインをリストへ挿入
		function setAutoComplete(target, domains) {
			var atIndex = currentVal.indexOf('@');
			var mailName = currentVal.slice(0, atIndex);

			// フォントサイズの調整
			var textBoxFontSize = parseInt(target.css('font-size'));
			$mailSuggestContainer.css('font-size', textBoxFontSize + "px");

			// リストサイズ調整
			var textBoxWidth = parseInt(target.width());
			$mailSuggestContainer.width(textBoxWidth);

			$mailSuggestContainer.css("padding", "0px");

			for (var i = 0; i < domains.length; i++) {
				mailDomainSuggests[i] = mailName + '@' + domains[i];
				$mailSuggestContainer.append("<li><span class='listtag'>" + mailDomainSuggests[i] + "</span></li>");
			}
		}

		$(document).on('click', function (event) {

			// リスト外をクリックした場合閉じる
			if ($(event.target).closest(".listtag").length === 0) {
				$mailSuggestContainer.empty();
				displayBorderLine(false);
			}
		});

		$mailSuggestContainer.on('mousedown', function (event) {
			var value = event.target.innerText;
			elements.val(value);
			$mailSuggestContainer.empty();
			displayBorderLine(false);
			$mailTextBox.trigger("change");
		});

		// ulタグの枠線の表示制御
		function displayBorderLine(borderFlag) {
			if (borderFlag) {
				$mailSuggestContainer.css('display', 'block');
			} else {
				$mailSuggestContainer.css('display', 'none');
			}
		}
	}
})(jQuery);