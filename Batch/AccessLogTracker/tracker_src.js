/*
=========================================================================================================
  Module      : トラッカースクリプト(tracker.js)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
//---------------------------------------------
// ログ出力用パラメタキー
//---------------------------------------------
// アカウントIDキー名
var KEY_ACCOUNT_ID = "__account_id";

// アクセスユーザIDキー名（クッキーKEYとしても利用）
var KEY_ACCESS_USER_ID = "__access_user_id";

// セッションIDキー名（クッキーKEYとしても利用）
var KEY_SESSION_ID = "__session_id";

// アクセスIDキー名
var KEY_ACCESS_ID = "__acc_id";

// ユーザIDキー名（クッキーKEYとしても利用）
var KEY_USER_ID = "__real_user_id";

// 初回ログインチェックフラグ
var KEY_FIRST_LOGIN_FLG = "__first_login_flg";

// リファラー
var KEY_REFERRER = "__referrer";

// アクション区分
var KEY_ACTION_KBN = "__action_kbn";

// アクションパラメータ
var KEY_ACTION_PARAM = "__action_param";

// アクセス間隔（日単位）
var KEY_ACS_INTERVAL = "__acs_interval";

// 最終アクセス日時（クッキー保存のみ）
var KEY_LAST_ACS_DATE = "__last_acs_date";

// URL：ドメイン
var KEY_URL_DOMAIN = "__url_domain";

// URL：ページ
var KEY_URL_PAGE = "__url_page";

// URL：パラメタ
var KEY_URL_PARAM = "__url_param";

// 検索エンジン
var KEY_SEARCH_ENGINE = "__srch_engn";

// 検索ワード
var KEY_SEARCH_DOMAIN = "__srch_word";

//---------------------------------------------
// アクション区分一覧
//---------------------------------------------
// ログインアクション
var KBN_ACTION_LOGIN = "__login"

// 退会アクション
var KBN_ACTION_LEAVE = "__leave"

//---------------------------------------------
// パラメータ区分一覧
//---------------------------------------------
var KBN_ACTION_LOGIN_USERID = "s1";
var KBN_ACTION_LEAVE_USERID = "s1";


var KBN_ACTION_S_HEAD = "__s_";
var KBN_ACTION_P_HEAD = "__p_";

//---------------------------------------------
// 検索エンジン定義
//---------------------------------------------
var alSrchEngineName = new Array();
var alSrchReqKey = new Array();
(function () {
	var iEngineCnt = 0;
	alSrchEngineName[iEngineCnt] = "google"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "yahoo"; alSrchReqKey[iEngineCnt++] = "p";
	alSrchEngineName[iEngineCnt] = "msn"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "goo"; alSrchReqKey[iEngineCnt++] = "MT";
	alSrchEngineName[iEngineCnt] = "livedoor"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "nifty"; alSrchReqKey[iEngineCnt++] = "Text";
	alSrchEngineName[iEngineCnt] = "infoseek"; alSrchReqKey[iEngineCnt++] = "qt";
	alSrchEngineName[iEngineCnt] = "excite"; alSrchReqKey[iEngineCnt++] = "qkw";
	alSrchEngineName[iEngineCnt] = "excite"; alSrchReqKey[iEngineCnt++] = "search";
	alSrchEngineName[iEngineCnt] = "lycos"; alSrchReqKey[iEngineCnt++] = "query";
	alSrchEngineName[iEngineCnt] = "ask"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "rssnavi"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "allabout"; alSrchReqKey[iEngineCnt++] = "qs";
	alSrchEngineName[iEngineCnt] = "aol"; alSrchReqKey[iEngineCnt++] = "query";
	alSrchEngineName[iEngineCnt] = "altavista"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "netscape"; alSrchReqKey[iEngineCnt++] = "query";
	alSrchEngineName[iEngineCnt] = "earthlink"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "cnn"; alSrchReqKey[iEngineCnt++] = "query";
	alSrchEngineName[iEngineCnt] = "looksmart"; alSrchReqKey[iEngineCnt++] = "key";
	alSrchEngineName[iEngineCnt] = "about"; alSrchReqKey[iEngineCnt++] = "terms";
	alSrchEngineName[iEngineCnt] = "mamma"; alSrchReqKey[iEngineCnt++] = "query";
	alSrchEngineName[iEngineCnt] = "alltheweb"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "gigablast"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "voila"; alSrchReqKey[iEngineCnt++] = "kw";
	alSrchEngineName[iEngineCnt] = "virgilio"; alSrchReqKey[iEngineCnt++] = "qs";
	alSrchEngineName[iEngineCnt] = "teoma"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "fresheye"; alSrchReqKey[iEngineCnt++] = "kw";
	alSrchEngineName[iEngineCnt] = "search"; alSrchReqKey[iEngineCnt++] = "q";
	alSrchEngineName[iEngineCnt] = "bing"; alSrchReqKey[iEngineCnt++] = "q";
})();

//---------------------------------------------
// グローバル変数
//---------------------------------------------
// ドメインハッシュ
var strDomainHash = get_domain_hash();

///======================================================================================
// ログを取得（許可ドメイン指定あり）
///======================================================================================
function getlog() {
	return getlog_for_action(w2accesslog_account_id, "", "", w2accesslog_target_domain);
}

///======================================================================================
// ログを取得（ログイン用・許可ドメイン指定なし）
//	arguments[0] = ユーザID
///======================================================================================
function getlog_for_login() {
	return getlog_for_action(w2accesslog_account_id, KBN_ACTION_LOGIN, KBN_ACTION_LOGIN_USERID + "=" + arguments[0], w2accesslog_target_domain);
}

///======================================================================================
// ログを取得（退会用・許可ドメイン指定なし）
//	arguments[0] = ユーザID
///======================================================================================
function getlog_for_leave() {
	return getlog_for_action(w2accesslog_account_id, KBN_ACTION_LEAVE, KBN_ACTION_LEAVE_USERID + "=" + arguments[0], w2accesslog_target_domain);
}

///======================================================================================
// ログを取得（共通・その他アクション用）
//	strAccountId	= ID
//	strAction		= アクション
//	strActionParam	= その他のパラメタ
//  strAllowDomains = 許可ドメイン（カンマ区切り、NULLの場合はすべて許可）
///======================================================================================
function getlog_for_action(strAccountId, strAction, strActionParams, strAllowDomains) {
	var strReferrer;			// リファラー
	var strAccessUserId = "";	// アクセスユーザID
	var strSessionId = "";	// セッションID
	var strAccessId = "";		// アクセスID
	var strUserId = "";			// ユーザID

	//-------------------------------------------------
	// 許可ドメイン指定解析（対象外の場合処理を抜ける）
	//-------------------------------------------------
	if (strAllowDomains != null) {
		// 一度変数にいれておかないと正しく処理してくれない
		var iFindIndex = ("," + strAllowDomains + ",").indexOf("," + document.domain + ",")
		if (iFindIndex == -1) {
			return;
		}
	}

	//-------------------------------------------------
	// パラメタ分割・格納
	//-------------------------------------------------
	var TmpParams = strActionParams.replace(",,", "@@@escape_comma@@@").split(",");
	var Params = new Array();
	for (var iLoop = 0; iLoop < TmpParams.length; iLoop++) {
		Params[TmpParams[iLoop].substring(0, TmpParams[iLoop].indexOf("="))] = TmpParams[iLoop].substring(TmpParams[iLoop].indexOf("=") + 1, TmpParams[iLoop].length);
	}

	//-------------------------------------------------
	// アクセスユーザID取得（or 生成）
	//-------------------------------------------------
	var blGetNewAccessUserId = false;

	// アクセスID取得
	strAccessUserId = get_access_user_id(document.cookie);

	// 取ってこれなければ生成
	if (strAccessUserId == "") {
		strAccessUserId = create_cookie_id();
		blGetNewAccessUserId = true;
	}

	//-------------------------------------------------
	// セッションID取得（or 生成）
	//-------------------------------------------------
	var blGetNewSessionId = false;

	strSessionId = get_access_session_id(document.cookie);

	// 取ってこれなければ生成
	if (strSessionId == "") {
		strSessionId = create_cookie_id();
		blGetNewSessionId = true;
	}

	//-------------------------------------------------
	// アクセスID生成（画像がキャッシュに残るのを防ぐため）
	//-------------------------------------------------
	strAccessId = create_cookie_id();

	//-------------------------------------------------
	// ユーザID取得
	//-------------------------------------------------
	var blGetNewUserId = false;

	// クッキーより取得
	strUserId = get_user_id(document.cookie);

	// ログインアクションであればパラメタより取得（識別用ハッシュ付加）
	if (strAction == KBN_ACTION_LOGIN) {
		if ((strUserId == "") || (strUserId != Params[KBN_ACTION_LOGIN_USERID])) {
			strUserId = strDomainHash + "." + Params[KBN_ACTION_LOGIN_USERID];
			blGetNewUserId = true;
		}
	}

	//-------------------------------------------------
	// アクセス頻度取得
	//-------------------------------------------------
	var iNowMsec = (new Date()).getTime();
	var iLastAcsMsc = get_acs_msec(document.cookie);
	var iAcsIntervalDay = -1;	// 初回訪問は-1
	if (iLastAcsMsc != "") {
		iAcsIntervalDay = parseInt((iNowMsec - iLastAcsMsc) / 1000 / 60 / 60 / 24);
	}

	//-------------------------------------------------
	// クッキー保存
	//-------------------------------------------------
	var strExpires = " expires=Sun, 18 Jan 2038 00:00:00 GMT;";
	var strPath = " path=" + w2accesslog_cookie_root + ";";

	// 初回の場合はアクセスユーザID保存
	if (blGetNewAccessUserId == true) {
		document.cookie = KEY_ACCESS_USER_ID + "=" + strAccessUserId + ";" + strExpires + strPath;
	}

	// 初回の場合はセッションザID保存
	if (blGetNewSessionId == true) {
		document.cookie = KEY_SESSION_ID + "=" + strSessionId + ";" + strPath
	}

	// 更新があった場合はユーザID保存
	if (blGetNewUserId == true) {
		document.cookie = KEY_USER_ID + "=" + strUserId + ";" + strExpires + strPath;
	}

	// アクセス日保存（ミリ秒形式）
	document.cookie = KEY_LAST_ACS_DATE + "=" + strDomainHash + "." + iNowMsec + ";" + strExpires + strPath;

	//-------------------------------------------------
	// リファラー取得
	//-------------------------------------------------
	strReferrer = get_referrer();

	//-------------------------------------------------
	// データ調整
	//-------------------------------------------------
	if (strAction == null) strAction = "";

	//-------------------------------------------------
	// パラメタ作成
	//-------------------------------------------------
	var strImageParams = "";
	strImageParams += KEY_ACCESS_ID + "=" + encode_url(strAccessId);
	strImageParams += "&" + KEY_ACCOUNT_ID + "=" + encode_url(strAccountId);
	strImageParams += "&" + KEY_ACCESS_USER_ID + "=" + encode_url(strAccessUserId);
	strImageParams += "&" + KEY_SESSION_ID + "=" + encode_url(strSessionId);
	strImageParams += "&" + KEY_USER_ID + "=" + encode_url(strUserId.replace(strDomainHash + ".", ""));	// 識別用ハッシュ削除
	strImageParams += "&" + KEY_REFERRER + "=" + encode_url(strReferrer);
	strImageParams += "&" + KEY_ACTION_KBN + "=" + encode_url(strAction);

	var ParamChar = KBN_ACTION_S_HEAD;
	for (var iLoop = 1; iLoop <= 2; iLoop++) {
		for (var jLoop = 1; jLoop <= 20; jLoop++) {
			var strValue = Params[ParamChar.replace(/_/g, "") + jLoop];
			if (strValue != null) {
				strImageParams += "&" + ParamChar + jLoop + "=" + encode_url(strValue.replace("@@@escape_comma@@@", ","));
			}
		}
		ParamChar = KBN_ACTION_P_HEAD;
	}
	strImageParams += "&" + KEY_FIRST_LOGIN_FLG + "=" + ((blGetNewUserId == true) ? "1" : "0");
	strImageParams += "&" + KEY_ACS_INTERVAL + "=" + iAcsIntervalDay;
	strImageParams += "&" + KEY_URL_DOMAIN + "=" + encode_url(document.domain);
	strImageParams += "&" + KEY_URL_PAGE + "=" + encode_url(location.pathname);
	strImageParams += "&" + KEY_URL_PARAM + "=" + encode_url((location.search.length != 0) ? location.search.substring(1, location.search.length) : "");
	strImageParams += "&" + get_search_engine_info(document.referrer)

	//-------------------------------------------------
	// HTTP/HTTPS判定
	//-------------------------------------------------
	if (location.protocol == "https:") {
		w2accesslog_getlog_path = w2accesslog_getlog_path.replace("http://", "https://");
	}

	//-------------------------------------------------
	// 画像作成＆ログ取得
	//-------------------------------------------------
	var objImage = new Image(1, 1);
	objImage.src = w2accesslog_getlog_path + "?" + strImageParams;
	objImage.onload = function () { _uVoid(); }
}

//=========================================================================
//
//=========================================================================
function _uVoid() { return; }

///======================================================================================
// アクセスユーザIDをクッキーから取得
///======================================================================================
function get_access_user_id(strCookie) {
	return get_cookie_value(strCookie, KEY_ACCESS_USER_ID);
}
///======================================================================================
// セッションIDをクッキーから取得
///======================================================================================
function get_access_session_id(strCookie) {
	return get_cookie_value(strCookie, KEY_SESSION_ID);
}
///======================================================================================
// ユーザIDをクッキーから取得
///======================================================================================
function get_user_id(strCookie) {
	return get_cookie_value(strCookie, KEY_USER_ID);
}
///======================================================================================
// 最終アクセス日(ミリ秒)をクッキーから取得
///======================================================================================
function get_acs_msec(strCookie) {
	return get_cookie_value(strCookie, KEY_LAST_ACS_DATE).replace(strDomainHash + ".", "");
}

///======================================================================================
// 値をクッキーから取得
///======================================================================================
function get_cookie_value(strCookie, strCookieKey) {
	var strResult = "";

	var iCookieIndex = strCookie.indexOf(strCookieKey + "=" + strDomainHash);
	if (iCookieIndex != -1) {
		// クッキーの値を取得
		var iLastIndex = strCookie.indexOf(";", iCookieIndex);
		if (iLastIndex == -1) {
			iLastIndex = strCookie.length;
		}

		var strTargetCookieString = strCookie.substring(iCookieIndex, iLastIndex);
		strResult = strTargetCookieString.substring(strTargetCookieString.indexOf("=") + 1, strTargetCookieString.length);
	}

	return strResult;
}

///======================================================================================
// ユーザ識別クッキーID発行		クッキーID ： ドメインハッシュ + 秒数 + ランダム値
///======================================================================================
function create_cookie_id() {
	// 日付の秒値作成
	var objDate = new Date();
	var iDateSecond = Math.round(objDate.getTime() / 1000);

	// ランダム値作成
	var iRandom = Math.round(Math.random() * 2147483647);

	// クッキー値作成
	var strResut = strDomainHash + "." + iDateSecond + "." + iRandom;

	return strResut;
}


///======================================================================================
// リファラーを返す
///======================================================================================
function get_referrer() {
	var strResult = "";

	try {
		if (document.referrer != parent.frames.location) {
			// リファラー取得
			strResult = document.referrer;
		}
		else {
			// フレーム内HTMLの場合は、親フレームのリファラーを取得
			strResult = top.document.referrer;
		}
	}
	catch (e) {
		// 親フレームが別ドメインだった場合は例外が発生してここに来る。
		// その場合リファラは取得できないので仕方なく親フレームそのものを取得する
		strResult = document.referrer;
	}

	// OUTLOOK-POPUPや他サイトからのポップアップの場合、
	// window.openerが直接操作できず「アクセスが拒否されました」というエラーになる。
	// しかもIEセキュリティのなりすまし防止機能で「無効な構文エラー」画面へ
	// 遷移してしまうため、ここではwindow.openerを使用したリファラー取得
	// はあきらめざるをえない。
	// ただし、ブラウザ毎に判定し、try～catchを駆使すればエラーを事前判定可能。
	// このとき、IE4等の場合はtry～catchがスクリプト内に存在するだけでエラーとなるため
	// evalをさらに組み合わせて判定処理を実行する必要がある。
	/*
	if ((strResult.length == 0) && (window.opener != null))
	{
		// リファラーが空でopenerがある場合、openerのロケーションをセット
		strResult = window.opener.location;
	}
	*/

	if (strResult.length != 0) {
		// リファラーの「http://」「https://」を削除
		strResult = strResult.replace("http://", "");
		strResult = strResult.replace("https://", "");
	}

	return strResult;
}

///======================================================================================
// 検索ドメイン・検索文字列（を含んだクエリ文字列）を返す
///======================================================================================
function get_search_engine_info(strReferrer) {
	var iIndex = 0;
	var strSearchDomain = "";
	var strSearchKeyword = "";

	if ((iIndex = strReferrer.indexOf("://")) > 0) {
		// 「://」以降を取得
		var strRefTemp1 = strReferrer.substring(iIndex + 3, strReferrer.length);

		// 「/」までの間を取得
		if (strRefTemp1.indexOf("/") > -1) {
			strRefTemp1 = strRefTemp1.substring(0, strRefTemp1.indexOf("/"));
		}

		for (var iLoop = 0; iLoop < alSrchEngineName.length; iLoop++) {
			// 該当エンジン？
			if (strRefTemp1.indexOf(alSrchEngineName[iLoop] + ".") > -1) {
				// 該当エンジンの検索文字クエリパラメータ存在？
				if ((iIndex = strReferrer.indexOf("?" + alSrchReqKey[iLoop] + "=")) > -1 || (iIndex = strReferrer.indexOf("&" + alSrchReqKey[iLoop] + "=")) > -1) {
					// 検索文字取得
					var strRefTemp2 = strReferrer.substring(iIndex + alSrchReqKey[iLoop].length + 2, strReferrer.length);
					if ((iIndex = strRefTemp2.indexOf("&")) > -1) {
						strRefTemp2 = strRefTemp2.substring(0, iIndex);
					}

					strSearchDomain = alSrchEngineName[iLoop];
					strSearchKeyword = strRefTemp2;
					break;
				}
			}
		}
	}

	// 検索文字列を返す（ドメインはエンコード、文字列はあらかじめエンコードされているのでそのまま返す）
	return KEY_SEARCH_ENGINE + "=" + encode_url(strSearchDomain) + "&" + KEY_SEARCH_DOMAIN + "=" + strSearchKeyword;
}

///======================================================================================
// ドメインハッシュを返す（「www.」は除く）
///======================================================================================
function get_domain_hash() {

	// ドメイン取得（"www."は除く）
	var strDomain = document.domain;
	if (strDomain.substring(0, 4) == "www.") {
		strDomain = strDomain.substring(4, strDomain.length);
	}

	return get_hash(strDomain);
}

//=========================================================================
// ハッシュ値を返す
//=========================================================================
function get_hash(strValue) {
	var iHash = 0;
	var iTemp = 0;

	for (var iLoop = strValue.length - 1; iLoop >= 0; iLoop--) {
		var ch = parseInt(strValue.charCodeAt(iLoop));

		iHash = ((iHash << 6) & 0xfffffff) + ch + (ch << 14);
		if ((iTemp = iHash & 0xfe00000) != 0) {
			iHash = (iHash ^ (iTemp >> 21));
		}
	}
	return iHash;
}

///======================================================================================
// URLエンコード
///======================================================================================
function encode_url(str) {
	var s0, i, s, u;
	s0 = "";				// encoded str
	for (i = 0; i < str.length; i++) {	// scan the source
		s = str.charAt(i);
		u = str.charCodeAt(i);			// get unicode of the char
		if (s == " ") { s0 += "+"; }		// SP should be converted to "+"
		else {
			if (u == 0x2a || u == 0x2d || u == 0x2e || u == 0x5f || ((u >= 0x30) && (u <= 0x39)) || ((u >= 0x41) && (u <= 0x5a)) || ((u >= 0x61) && (u <= 0x7a))) {	 // check for escape
				s0 = s0 + s;			// don't escape
			}
			else {						// escape
				if ((u >= 0x0) && (u <= 0x7f)) {	// single byte format
					s = "0" + u.toString(16);
					s0 += "%" + s.substr(s.length - 2);
				}
				else if (u > 0x1fffff) {	// quaternary byte format (extended)
					s0 += "%" + (oxf0 + ((u & 0x1c0000) >> 18)).toString(16);
					s0 += "%" + (0x80 + ((u & 0x3f000) >> 12)).toString(16);
					s0 += "%" + (0x80 + ((u & 0xfc0) >> 6)).toString(16);
					s0 += "%" + (0x80 + (u & 0x3f)).toString(16);
				}
				else if (u > 0x7ff) {	// triple byte format
					s0 += "%" + (0xe0 + ((u & 0xf000) >> 12)).toString(16);
					s0 += "%" + (0x80 + ((u & 0xfc0) >> 6)).toString(16);
					s0 += "%" + (0x80 + (u & 0x3f)).toString(16);
				}
				else {					// double byte format
					s0 += "%" + (0xc0 + ((u & 0x7c0) >> 6)).toString(16);
					s0 += "%" + (0x80 + (u & 0x3f)).toString(16);
				}
			}
		}
	}
	return s0;
}
