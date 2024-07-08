//======================================================================================
// 氏名（姓・名）の自動振り仮名変換を実行する
//======================================================================================
function execAutoKana(firstName, firstNameKana, lastName, lastNameKana, kanaType) {
	// 振り仮名種別がひらがなの場合、ひらがな変換を行う。カタカナの場合、カタカナ変換を行う
	if (kanaType == 'FULLWIDTH_HIRAGANA') {
		execAutoKanaHiragana(firstName, firstNameKana, lastName, lastNameKana);
	} else if (kanaType == 'FULLWIDTH_KATAKANA') {
		execAutoKanaKatakana(firstName, firstNameKana, lastName, lastNameKana);
	}
}

//======================================================================================
// ふりがな（姓・名）の自動かな←→カナ変換を実行する
//======================================================================================
function execAutoChangeKana(firstNameKana, lastNameKana, kanaType) {
	// 振り仮名種別がひらがなの場合、ひらがな変換を行う。カタカナの場合、カタカナ変換を行う
	if (kanaType == 'FULLWIDTH_HIRAGANA') {
		execAutoChangeKanaHiragana(firstNameKana, lastNameKana);
	} else if (kanaType == 'FULLWIDTH_KATAKANA') {
		execAutoChangeKanaKatakana(firstNameKana, lastNameKana);
	}
}

//======================================================================================
// 氏名（姓・名）の自動振り仮名変換を実行する（ひらがな用）
//======================================================================================
function execAutoKanaHiragana(firstName, firstNameKana, lastName, lastNameKana) {
	$.fn.autoKana(firstName, firstNameKana);
	$.fn.autoKana(lastName, lastNameKana);
}

//======================================================================================
// 氏名（姓・名）の自動振り仮名変換を実行する（カタカナ用）
//======================================================================================
function execAutoKanaKatakana(firstName, firstNameKana, lastName, lastNameKana) {
	$.fn.autoKana(firstName, firstNameKana, { katakana: true });
	$.fn.autoKana(lastName, lastNameKana, { katakana: true });
}

//======================================================================================
// ふりがな（姓・名）の自動カナ→かな変換を実行する（ひらがな用）
//======================================================================================
function execAutoChangeKanaHiragana(firstNameKana, lastNameKana) {
	$.fn.autoChangeKana(firstNameKana);
	$.fn.autoChangeKana(lastNameKana);
}

//======================================================================================
// ふりがな（姓・名）の自動かな→カナ変換を実行する（カタカナ用）
//======================================================================================
function execAutoChangeKanaKatakana(firstNameKana, lastNameKana) {
	$.fn.autoChangeKana(firstNameKana, { katakana: true });
	$.fn.autoChangeKana(lastNameKana, { katakana: true });
}