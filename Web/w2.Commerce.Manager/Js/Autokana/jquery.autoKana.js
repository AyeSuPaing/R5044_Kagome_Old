// ※Mac時の独自の処理があるため上書きする際は注意してください!!

// Copyright (c) 2013 Keith Perhac @ DelfiNet (http://delfi-net.com)
//
// Based on the AutoRuby library created by:
// Copyright (c) 2005-2008 spinelz.org (http://script.spinelz.org/)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

(function ($) {
  $.fn.autoKana = function (element1, element2, passedOptions) {

    // Mac時の処理上書き注意
    var ua = navigator.userAgent.toLowerCase();
    var isMac = ((ua.indexOf("mac") > -1) && (ua.indexOf("os") > -1)) && !((ua.indexOf("iphone") > -1) || (ua.indexOf("ipad") > -1) || (ua.indexOf("windows") > -1));

    var options = $.extend(
			{
			  "katakana": false
			}, passedOptions);

    var kanaExtractionPattern = new RegExp("[^ 　ぁあ-んー]", "g");
    var kanaCompactingPattern = new RegExp("[ぁぃぅぇぉっゃゅょ]", "g");
    var elName,
			elKana,
			active = false,
			timer = null,
			flagConvert = true,
			input,
			romanChar,
			values,
			ignoreString,
			baseKana;

    elName = $(element1);
    elKana = $(element2);
    active = true;
    stateClear();

    if (isMac) {
      elName.keyup(eventKeyUp);
    } else {
      elName.blur(eventBlur);
      elName.focus(eventFocus);
      elName.keydown(eventKeyDown);
    }

    function checkConvert(newValues) {
      if (!flagConvert) {
        if (Math.abs(values.length - newValues.length) > 1) {
          var tmpValues = newValues.join("").replace(kanaCompactingPattern, "").split("");
          if (Math.abs(values.length - tmpValues.length) > 1) {
            stateConvert();
          }
        } else {
          if (values.length === input.length && values.join("") !== input) {
            if (input.match(kanaExtractionPattern)) {
              stateConvert();
            }
          }
        }
      }
    };

    function checkValue() {
      var newInput, newValues;
      newInput = elName.val();
      if (newInput === "") {
        stateClear();
        setKana();
      } else {
        newInput = removeString(newInput);
        if (input === newInput) {
          return;
        } else {
          input = newInput;
          if (!flagConvert) {
            newValues = newInput.replace(kanaExtractionPattern, "").split("");
            checkConvert(newValues);
            setKana(newValues);
          }
        }
      }
    };

    function resetInterval() {
      clearInterval(timer);
    };

    function eventBlur(event) {
      resetInterval();
    };
    function eventFocus(event) {
      stateInput();
      startInterval();
    };
    function eventKeyDown(event) {
      if (flagConvert) {
        stateInput();
      }
    };

    // Mac時の処理上書き注意
    function eventKeyUp(event) {// キーボード入力が行われた場合(キーが入力されたら毎度行う)

      var realTimeKeyCode = event.keyCode;// キーコードを所得
      if (65 <= realTimeKeyCode && realTimeKeyCode <= 90) {
        romanChar += String.fromCharCode(realTimeKeyCode)// ローマ字が入る(a-z)
      } else if (realTimeKeyCode === 8 && elName.val() === "") {// TextBoxの中身が空になった場合
        romanChar = "";
        elKana.val("");
      } else if (realTimeKeyCode === 13) {// Enterを押された場合
        romanChar = "";
      }
      if (romanChar) {
        var newValues = romanToHiragana(romanChar.toLowerCase());// ローマ字からひらがなへ変換している
        if (newValues.match(/^[ぁ-んー　]+$/)) {// ひらがなが入っているかチェックはいていたらsetKanaを行う
          setKanaMac(newValues);
          romanChar = "";
        }
      }
    };
    function isHiragana(chara) {
      return ((chara >= 12353 && chara <= 12435) || chara == 12445 || chara == 12446);
    };
    function removeString(newInput) {
      if (newInput.indexOf(ignoreString) !== -1) {
        return newInput.replace(ignoreString, "");
      } else {
        var i;
        var ignoreArray = ignoreString.split("");
        var inputArray = newInput.split("");
        for (i = 0; i < ignoreArray.length; i++) {
          if (ignoreArray[i] === inputArray[i]) {
            inputArray[i] = "";
          }
        }
        return inputArray.join("");
      }
    };
    function startInterval() {
      var self = this;
      timer = setInterval(checkValue, 30);
    };
    function setKana(newValues) {
      if (!flagConvert) {
        if (newValues) {
          values = newValues;
        }
        if (active) {
          var _val = toKatakana(baseKana + values.join(""));
          elKana.val(_val);
        }
      }
    };
    // Mac時の処理上書き注意
    function setKanaMac(newValues) {
      baseKana = elKana.val();
      var values = toKatakana(baseKana + newValues);
      elKana.val(values);
    };
    function stateClear() {
      baseKana = "";
      flagConvert = false;
      ignoreString = "";
      input = "";
      romanChar = "";
      values = [];
    };
    function stateInput() {
      baseKana = elKana.val();
      flagConvert = false;
      ignoreString = elName.val();
    };
    function stateConvert() {
      baseKana = baseKana + values.join("");
      flagConvert = true;
      values = [];
    };
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
    // Mac時の処理上書き注意
    var hiraganaBox = {
      "a": "あ", "i": "い", "u": "う", "e": "え", "o": "お",
      "ka": "か", "ki": "き", "ku": "く", "ke": "け", "ko": "こ",
      "sa": "さ", "si": "し", "su": "す", "se": "せ", "so": "そ",
      "ta": "た", "ti": "ち", "tu": "つ", "te": "て", "to": "と", "chi": "ち", "tsu": "つ",
      "na": "な", "ni": "に", "nu": "ぬ", "ne": "ね", "no": "の",
      "ha": "は", "hi": "ひ", "hu": "ふ", "he": "へ", "ho": "ほ", "fu": "ふ",
      "ma": "ま", "mi": "み", "mu": "む", "me": "め", "mo": "も",
      "ya": "や", "yi": "い", "yu": "ゆ", "ye": "いぇ", "yo": "よ",
      "ra": "ら", "ri": "り", "ru": "る", "re": "れ", "ro": "ろ",
      "wa": "わ", "wyi": "ゐ", "wu": "う", "wye": "ゑ", "wo": "を",
      "nn": "ん",
      "ga": "が", "gi": "ぎ", "gu": "ぐ", "ge": "げ", "go": "ご",
      "za": "ざ", "zi": "じ", "zu": "ず", "ze": "ぜ", "zo": "ぞ", "ji": "じ",
      "da": "だ", "di": "ぢ", "du": "づ", "de": "で", "do": "ど",
      "ba": "ば", "bi": "び", "bu": "ぶ", "be": "べ", "bo": "ぼ",
      "pa": "ぱ", "pi": "ぴ", "pu": "ぷ", "pe": "ぺ", "po": "ぽ",
      "kya": "きゃ", "kyu": "きゅ", "kyo": "きょ",
      "sya": "しゃ", "syu": "しゅ", "syo": "しょ",
      "tya": "ちゃ", "tyi": "ちぃ", "tyu": "ちゅ", "tye": "ちぇ", "tyo": "ちょ", "cha": "ちゃ", "chu": "ちゅ", "che": "ちぇ", "cho": "ちょ",
      "nya": "にゃ", "nyi": "にぃ", "nyu": "にゅ", "nye": "にぇ", "nyo": "にょ",
      "hya": "ひゃ", "hyi": "ひぃ", "hyu": "ひゅ", "hye": "ひぇ", "hyo": "ひょ",
      "mya": "みゃ", "myi": "みぃ", "myu": "みゅ", "mye": "みぇ", "myo": "みょ",
      "rya": "りゃ", "ryi": "りぃ", "ryu": "りゅ", "rye": "りぇ", "ryo": "りょ",
      "gya": "ぎゃ", "gyi": "ぎぃ", "gyu": "ぎゅ", "gye": "ぎぇ", "gyo": "ぎょ",
      "zya": "じゃ", "zyi": "じぃ", "zyu": "じゅ", "zye": "じぇ", "zyo": "じょ",
      "ja": "じゃ", "ju": "じゅ", "je": "じぇ", "jo": "じょ", "jya": "じゃ", "jyi": "じぃ", "jyu": "じゅ", "jye": "じぇ", "jyo": "じょ",
      "dya": "ぢゃ", "dyi": "ぢぃ", "dyu": "ぢゅ", "dye": "ぢぇ", "dyo": "ぢょ",
      "bya": "びゃ", "byi": "びぃ", "byu": "びゅ", "bye": "びぇ", "byo": "びょ",
      "pya": "ぴゃ", "pyi": "ぴぃ", "pyu": "ぴゅ", "pye": "ぴぇ", "pyo": "ぴょ",
      "fa": "ふぁ", "fi": "ふぃ", "fe": "ふぇ", "fo": "ふぉ",
      "fya": "ふゃ", "fyu": "ふゅ", "fyo": "ふょ",
      "xa": "ぁ", "xi": "ぃ", "xu": "ぅ", "xe": "ぇ", "xo": "ぉ", "la": "ぁ", "li": "ぃ", "lu": "ぅ", "le": "ぇ", "lo": "ぉ",
      "xya": "ゃ", "xyu": "ゅ", "xyo": "ょ",
      "xtu": "っ", "xtsu": "っ",
      "wi": "うぃ", "we": "うぇ",
      "va": "ヴぁ", "vi": "ヴぃ", "vu": "ヴ", "ve": "ヴぇ", "vo": "ヴぉ"
    };
    // Mac時の処理上書き注意
    function romanToHiragana(roman) {
      var i, iz, match, regex,
				hiragana = "", table = hiraganaBox;

      regex = new RegExp((function (table) {
        var key,
					s = "^(?:";

        for (key in table) if (table.hasOwnProperty(key)) {
          s += key + "|";
        }
        return s + "(?:n(?![aiueo]|y[aiueo]|$))|" + "([^aiueon])\\1)";
      })(table));
      for (i = 0, iz = roman.length; i < iz; ++i) {
        if (match = roman.slice(i).match(regex)) {
          if (match[0] === "n") {
            hiragana += "ん";
          } else if (/^([^n])\1$/.test(match[0])) {
            hiragana += "っ";
            --i;
          } else {
            hiragana += table[match[0]];
          }
          i += match[0].length - 1;
        } else {
          hiragana += roman[i];
        }
      }
      return hiragana;
    };
  };
})(jQuery);
