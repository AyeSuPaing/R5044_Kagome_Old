(function ($) {
	$.fn.autoChangeKana = function (element1, passedOptions) {

		// Mac時の処理上書き注意
		var ua = navigator.userAgent.toLowerCase();
		var isMac = ((ua.indexOf("mac") > -1) && (ua.indexOf("os") > -1)) && !((ua.indexOf("iphone") > -1) || (ua.indexOf("ipad") > -1) || (ua.indexOf("windows") > -1));

		var options = $.extend(
			{
				"katakana": false
			}, passedOptions);

		var elKana = $(element1);

		if (isMac) {
			// Macの入力補完が影響するため、IME確定時に変換処理を行う。
			elKana.on("compositionend", function () {
				changeKana();
			})
			// 貼り付け時に変換処理を行う(貼り付け直後だと内容が取得できない為、タイマーを設定)
			elKana.bind("paste", function (event) {
				setTimeout(function () {
					changeKana();
				}, 100, event);
			})
		} else {
			elKana.blur(changeKana);
		}

		function changeKana() {
			var result;
			var baseKana;
			baseKana = hankanaToZenkana(elKana.val());
			if (options.katakana) {
				result = toKatakana(baseKana);
			}
			else {
				result = toHiragana(baseKana);
			}
			elKana.val(result);
		}

		function toKatakana(src) {
			if (options.katakana) {
				var c, i, str;
				str = new String;
				for (i = 0; i < src.length; i++) {
					c = src.charCodeAt(i);
					if (isHiragana(c)) {
						str += String.fromCharCode(c + 96);
					} else {
						str += src.charAt(i);
					}
				}
				return str;
			} else {
				return src;
			}
		}

		function toHiragana(src) {
			if (!options.katakana) {
				var c, i, str;
				str = new String;
				for (i = 0; i < src.length; i++) {
					c = src.charCodeAt(i);
					if (isKatakana(c)) {
						str += String.fromCharCode(c - 96);
					} else {
						str += src.charAt(i);
					}
				}
				return str;
			} else {
				return src;
			}
		}

		function isHiragana(chara) {
			return ((chara >= 12353 && chara <= 12435) || chara == 12445 || chara == 12446);
		};

		function isKatakana(chara) {
			return ((chara >= 12449 && chara <= 12531) || chara == 12541 || chara == 12542);
		};

		function hankanaToZenkana(str) {
			var kanaMap = {
				"ｶﾞ": "ガ", "ｷﾞ": "ギ", "ｸﾞ": "グ", "ｹﾞ": "ゲ", "ｺﾞ": "ゴ",
				"ｻﾞ": "ザ", "ｼﾞ": "ジ", "ｽﾞ": "ズ", "ｾﾞ": "ゼ", "ｿﾞ": "ゾ",
				"ﾀﾞ": "ダ", "ﾁﾞ": "ヂ", "ﾂﾞ": "ヅ", "ﾃﾞ": "デ", "ﾄﾞ": "ド",
				"ﾊﾞ": "バ", "ﾋﾞ": "ビ", "ﾌﾞ": "ブ", "ﾍﾞ": "ベ", "ﾎﾞ": "ボ",
				"ﾊﾟ": "パ", "ﾋﾟ": "ピ", "ﾌﾟ": "プ", "ﾍﾟ": "ペ", "ﾎﾟ": "ポ",
				"ｳﾞ": "ヴ", "ﾜﾞ": "ヷ", "ｦﾞ": "ヺ",
				"ｱ": "ア", "ｲ": "イ", "ｳ": "ウ", "ｴ": "エ", "ｵ": "オ",
				"ｶ": "カ", "ｷ": "キ", "ｸ": "ク", "ｹ": "ケ", "ｺ": "コ",
				"ｻ": "サ", "ｼ": "シ", "ｽ": "ス", "ｾ": "セ", "ｿ": "ソ",
				"ﾀ": "タ", "ﾁ": "チ", "ﾂ": "ツ", "ﾃ": "テ", "ﾄ": "ト",
				"ﾅ": "ナ", "ﾆ": "ニ", "ﾇ": "ヌ", "ﾈ": "ネ", "ﾉ": "ノ",
				"ﾊ": "ハ", "ﾋ": "ヒ", "ﾌ": "フ", "ﾍ": "ヘ", "ﾎ": "ホ",
				"ﾏ": "マ", "ﾐ": "ミ", "ﾑ": "ム", "ﾒ": "メ", "ﾓ": "モ",
				"ﾔ": "ヤ", "ﾕ": "ユ", "ﾖ": "ヨ",
				"ﾗ": "ラ", "ﾘ": "リ", "ﾙ": "ル", "ﾚ": "レ", "ﾛ": "ロ",
				"ﾜ": "ワ", "ｦ": "ヲ", "ﾝ": "ン",
				"ｧ": "ァ", "ｨ": "ィ", "ｩ": "ゥ", "ｪ": "ェ", "ｫ": "ォ",
				"ｯ": "ッ", "ｬ": "ャ", "ｭ": "ュ", "ｮ": "ョ",
				"｡": "。", "､": "、", "ｰ": "ー", "｢": "「", "｣": "」", "･": "・"
			};

			var reg = new RegExp("(" + Object.keys(kanaMap).join("|") + ")", "g");
			return str
				.replace(reg, function (match) {
					return kanaMap[match];
				})
				.replace(/ﾞ/g, "゛")
				.replace(/ﾟ/g, "゜");
		};
	};
})(jQuery);