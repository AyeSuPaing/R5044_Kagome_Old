/*
=========================================================================================================
  Module      : �g���b�J�[�X�N���v�g(tracker.js)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
//---------------------------------------------
// ���O�o�͗p�p�����^�L�[
//---------------------------------------------
// �A�J�E���gID�L�[��
var KEY_ACCOUNT_ID = "__account_id";

// �A�N�Z�X���[�UID�L�[���i�N�b�L�[KEY�Ƃ��Ă����p�j
var KEY_ACCESS_USER_ID = "__access_user_id";

// �Z�b�V����ID�L�[���i�N�b�L�[KEY�Ƃ��Ă����p�j
var KEY_SESSION_ID = "__session_id";

// �A�N�Z�XID�L�[��
var KEY_ACCESS_ID = "__acc_id";

// ���[�UID�L�[���i�N�b�L�[KEY�Ƃ��Ă����p�j
var KEY_USER_ID = "__real_user_id";

// ���񃍃O�C���`�F�b�N�t���O
var KEY_FIRST_LOGIN_FLG = "__first_login_flg";

// ���t�@���[
var KEY_REFERRER = "__referrer";

// �A�N�V�����敪
var KEY_ACTION_KBN = "__action_kbn";

// �A�N�V�����p�����[�^
var KEY_ACTION_PARAM = "__action_param";

// �A�N�Z�X�Ԋu�i���P�ʁj
var KEY_ACS_INTERVAL = "__acs_interval";

// �ŏI�A�N�Z�X�����i�N�b�L�[�ۑ��̂݁j
var KEY_LAST_ACS_DATE = "__last_acs_date";

// URL�F�h���C��
var KEY_URL_DOMAIN = "__url_domain";

// URL�F�y�[�W
var KEY_URL_PAGE = "__url_page";

// URL�F�p�����^
var KEY_URL_PARAM = "__url_param";

// �����G���W��
var KEY_SEARCH_ENGINE = "__srch_engn";

// �������[�h
var KEY_SEARCH_DOMAIN = "__srch_word";

//---------------------------------------------
// �A�N�V�����敪�ꗗ
//---------------------------------------------
// ���O�C���A�N�V����
var KBN_ACTION_LOGIN = "__login"

// �މ�A�N�V����
var KBN_ACTION_LEAVE = "__leave"

//---------------------------------------------
// �p�����[�^�敪�ꗗ
//---------------------------------------------
var KBN_ACTION_LOGIN_USERID = "s1";
var KBN_ACTION_LEAVE_USERID = "s1";


var KBN_ACTION_S_HEAD = "__s_";
var KBN_ACTION_P_HEAD = "__p_";

//---------------------------------------------
// �����G���W����`
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
// �O���[�o���ϐ�
//---------------------------------------------
// �h���C���n�b�V��
var strDomainHash = get_domain_hash();

///======================================================================================
// ���O���擾�i���h���C���w�肠��j
///======================================================================================
function getlog() {
	return getlog_for_action(w2accesslog_account_id, "", "", w2accesslog_target_domain);
}

///======================================================================================
// ���O���擾�i���O�C���p�E���h���C���w��Ȃ��j
//	arguments[0] = ���[�UID
///======================================================================================
function getlog_for_login() {
	return getlog_for_action(w2accesslog_account_id, KBN_ACTION_LOGIN, KBN_ACTION_LOGIN_USERID + "=" + arguments[0], w2accesslog_target_domain);
}

///======================================================================================
// ���O���擾�i�މ�p�E���h���C���w��Ȃ��j
//	arguments[0] = ���[�UID
///======================================================================================
function getlog_for_leave() {
	return getlog_for_action(w2accesslog_account_id, KBN_ACTION_LEAVE, KBN_ACTION_LEAVE_USERID + "=" + arguments[0], w2accesslog_target_domain);
}

///======================================================================================
// ���O���擾�i���ʁE���̑��A�N�V�����p�j
//	strAccountId	= ID
//	strAction		= �A�N�V����
//	strActionParam	= ���̑��̃p�����^
//  strAllowDomains = ���h���C���i�J���}��؂�ANULL�̏ꍇ�͂��ׂċ��j
///======================================================================================
function getlog_for_action(strAccountId, strAction, strActionParams, strAllowDomains) {
	var strReferrer;			// ���t�@���[
	var strAccessUserId = "";	// �A�N�Z�X���[�UID
	var strSessionId = "";	// �Z�b�V����ID
	var strAccessId = "";		// �A�N�Z�XID
	var strUserId = "";			// ���[�UID

	//-------------------------------------------------
	// ���h���C���w���́i�ΏۊO�̏ꍇ�����𔲂���j
	//-------------------------------------------------
	if (strAllowDomains != null) {
		// ��x�ϐ��ɂ���Ă����Ȃ��Ɛ������������Ă���Ȃ�
		var iFindIndex = ("," + strAllowDomains + ",").indexOf("," + document.domain + ",")
		if (iFindIndex == -1) {
			return;
		}
	}

	//-------------------------------------------------
	// �p�����^�����E�i�[
	//-------------------------------------------------
	var TmpParams = strActionParams.replace(",,", "@@@escape_comma@@@").split(",");
	var Params = new Array();
	for (var iLoop = 0; iLoop < TmpParams.length; iLoop++) {
		Params[TmpParams[iLoop].substring(0, TmpParams[iLoop].indexOf("="))] = TmpParams[iLoop].substring(TmpParams[iLoop].indexOf("=") + 1, TmpParams[iLoop].length);
	}

	//-------------------------------------------------
	// �A�N�Z�X���[�UID�擾�ior �����j
	//-------------------------------------------------
	var blGetNewAccessUserId = false;

	// �A�N�Z�XID�擾
	strAccessUserId = get_access_user_id(document.cookie);

	// ����Ă���Ȃ���ΐ���
	if (strAccessUserId == "") {
		strAccessUserId = create_cookie_id();
		blGetNewAccessUserId = true;
	}

	//-------------------------------------------------
	// �Z�b�V����ID�擾�ior �����j
	//-------------------------------------------------
	var blGetNewSessionId = false;

	strSessionId = get_access_session_id(document.cookie);

	// ����Ă���Ȃ���ΐ���
	if (strSessionId == "") {
		strSessionId = create_cookie_id();
		blGetNewSessionId = true;
	}

	//-------------------------------------------------
	// �A�N�Z�XID�����i�摜���L���b�V���Ɏc��̂�h�����߁j
	//-------------------------------------------------
	strAccessId = create_cookie_id();

	//-------------------------------------------------
	// ���[�UID�擾
	//-------------------------------------------------
	var blGetNewUserId = false;

	// �N�b�L�[���擾
	strUserId = get_user_id(document.cookie);

	// ���O�C���A�N�V�����ł���΃p�����^���擾�i���ʗp�n�b�V���t���j
	if (strAction == KBN_ACTION_LOGIN) {
		if ((strUserId == "") || (strUserId != Params[KBN_ACTION_LOGIN_USERID])) {
			strUserId = strDomainHash + "." + Params[KBN_ACTION_LOGIN_USERID];
			blGetNewUserId = true;
		}
	}

	//-------------------------------------------------
	// �A�N�Z�X�p�x�擾
	//-------------------------------------------------
	var iNowMsec = (new Date()).getTime();
	var iLastAcsMsc = get_acs_msec(document.cookie);
	var iAcsIntervalDay = -1;	// ����K���-1
	if (iLastAcsMsc != "") {
		iAcsIntervalDay = parseInt((iNowMsec - iLastAcsMsc) / 1000 / 60 / 60 / 24);
	}

	//-------------------------------------------------
	// �N�b�L�[�ۑ�
	//-------------------------------------------------
	var strExpires = " expires=Sun, 18 Jan 2038 00:00:00 GMT;";
	var strPath = " path=" + w2accesslog_cookie_root + ";";

	// ����̏ꍇ�̓A�N�Z�X���[�UID�ۑ�
	if (blGetNewAccessUserId == true) {
		document.cookie = KEY_ACCESS_USER_ID + "=" + strAccessUserId + ";" + strExpires + strPath;
	}

	// ����̏ꍇ�̓Z�b�V�����UID�ۑ�
	if (blGetNewSessionId == true) {
		document.cookie = KEY_SESSION_ID + "=" + strSessionId + ";" + strPath
	}

	// �X�V���������ꍇ�̓��[�UID�ۑ�
	if (blGetNewUserId == true) {
		document.cookie = KEY_USER_ID + "=" + strUserId + ";" + strExpires + strPath;
	}

	// �A�N�Z�X���ۑ��i�~���b�`���j
	document.cookie = KEY_LAST_ACS_DATE + "=" + strDomainHash + "." + iNowMsec + ";" + strExpires + strPath;

	//-------------------------------------------------
	// ���t�@���[�擾
	//-------------------------------------------------
	strReferrer = get_referrer();

	//-------------------------------------------------
	// �f�[�^����
	//-------------------------------------------------
	if (strAction == null) strAction = "";

	//-------------------------------------------------
	// �p�����^�쐬
	//-------------------------------------------------
	var strImageParams = "";
	strImageParams += KEY_ACCESS_ID + "=" + encode_url(strAccessId);
	strImageParams += "&" + KEY_ACCOUNT_ID + "=" + encode_url(strAccountId);
	strImageParams += "&" + KEY_ACCESS_USER_ID + "=" + encode_url(strAccessUserId);
	strImageParams += "&" + KEY_SESSION_ID + "=" + encode_url(strSessionId);
	strImageParams += "&" + KEY_USER_ID + "=" + encode_url(strUserId.replace(strDomainHash + ".", ""));	// ���ʗp�n�b�V���폜
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
	// HTTP/HTTPS����
	//-------------------------------------------------
	if (location.protocol == "https:") {
		w2accesslog_getlog_path = w2accesslog_getlog_path.replace("http://", "https://");
	}

	//-------------------------------------------------
	// �摜�쐬�����O�擾
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
// �A�N�Z�X���[�UID���N�b�L�[����擾
///======================================================================================
function get_access_user_id(strCookie) {
	return get_cookie_value(strCookie, KEY_ACCESS_USER_ID);
}
///======================================================================================
// �Z�b�V����ID���N�b�L�[����擾
///======================================================================================
function get_access_session_id(strCookie) {
	return get_cookie_value(strCookie, KEY_SESSION_ID);
}
///======================================================================================
// ���[�UID���N�b�L�[����擾
///======================================================================================
function get_user_id(strCookie) {
	return get_cookie_value(strCookie, KEY_USER_ID);
}
///======================================================================================
// �ŏI�A�N�Z�X��(�~���b)���N�b�L�[����擾
///======================================================================================
function get_acs_msec(strCookie) {
	return get_cookie_value(strCookie, KEY_LAST_ACS_DATE).replace(strDomainHash + ".", "");
}

///======================================================================================
// �l���N�b�L�[����擾
///======================================================================================
function get_cookie_value(strCookie, strCookieKey) {
	var strResult = "";

	var iCookieIndex = strCookie.indexOf(strCookieKey + "=" + strDomainHash);
	if (iCookieIndex != -1) {
		// �N�b�L�[�̒l���擾
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
// ���[�U���ʃN�b�L�[ID���s		�N�b�L�[ID �F �h���C���n�b�V�� + �b�� + �����_���l
///======================================================================================
function create_cookie_id() {
	// ���t�̕b�l�쐬
	var objDate = new Date();
	var iDateSecond = Math.round(objDate.getTime() / 1000);

	// �����_���l�쐬
	var iRandom = Math.round(Math.random() * 2147483647);

	// �N�b�L�[�l�쐬
	var strResut = strDomainHash + "." + iDateSecond + "." + iRandom;

	return strResut;
}


///======================================================================================
// ���t�@���[��Ԃ�
///======================================================================================
function get_referrer() {
	var strResult = "";

	try {
		if (document.referrer != parent.frames.location) {
			// ���t�@���[�擾
			strResult = document.referrer;
		}
		else {
			// �t���[����HTML�̏ꍇ�́A�e�t���[���̃��t�@���[���擾
			strResult = top.document.referrer;
		}
	}
	catch (e) {
		// �e�t���[�����ʃh���C���������ꍇ�͗�O���������Ă����ɗ���B
		// ���̏ꍇ���t�@���͎擾�ł��Ȃ��̂Ŏd���Ȃ��e�t���[�����̂��̂��擾����
		strResult = document.referrer;
	}

	// OUTLOOK-POPUP�⑼�T�C�g����̃|�b�v�A�b�v�̏ꍇ�A
	// window.opener�����ڑ���ł����u�A�N�Z�X�����ۂ���܂����v�Ƃ����G���[�ɂȂ�B
	// ������IE�Z�L�����e�B�̂Ȃ肷�܂��h�~�@�\�Łu�����ȍ\���G���[�v��ʂ�
	// �J�ڂ��Ă��܂����߁A�����ł�window.opener���g�p�������t�@���[�擾
	// �͂�����߂�������Ȃ��B
	// �������A�u���E�U���ɔ��肵�Atry�`catch����g����΃G���[�����O����\�B
	// ���̂Ƃ��AIE4���̏ꍇ��try�`catch���X�N���v�g���ɑ��݂��邾���ŃG���[�ƂȂ邽��
	// eval������ɑg�ݍ��킹�Ĕ��菈�������s����K�v������B
	/*
	if ((strResult.length == 0) && (window.opener != null))
	{
		// ���t�@���[�����opener������ꍇ�Aopener�̃��P�[�V�������Z�b�g
		strResult = window.opener.location;
	}
	*/

	if (strResult.length != 0) {
		// ���t�@���[�́uhttp://�v�uhttps://�v���폜
		strResult = strResult.replace("http://", "");
		strResult = strResult.replace("https://", "");
	}

	return strResult;
}

///======================================================================================
// �����h���C���E����������i���܂񂾃N�G��������j��Ԃ�
///======================================================================================
function get_search_engine_info(strReferrer) {
	var iIndex = 0;
	var strSearchDomain = "";
	var strSearchKeyword = "";

	if ((iIndex = strReferrer.indexOf("://")) > 0) {
		// �u://�v�ȍ~���擾
		var strRefTemp1 = strReferrer.substring(iIndex + 3, strReferrer.length);

		// �u/�v�܂ł̊Ԃ��擾
		if (strRefTemp1.indexOf("/") > -1) {
			strRefTemp1 = strRefTemp1.substring(0, strRefTemp1.indexOf("/"));
		}

		for (var iLoop = 0; iLoop < alSrchEngineName.length; iLoop++) {
			// �Y���G���W���H
			if (strRefTemp1.indexOf(alSrchEngineName[iLoop] + ".") > -1) {
				// �Y���G���W���̌��������N�G���p�����[�^���݁H
				if ((iIndex = strReferrer.indexOf("?" + alSrchReqKey[iLoop] + "=")) > -1 || (iIndex = strReferrer.indexOf("&" + alSrchReqKey[iLoop] + "=")) > -1) {
					// ���������擾
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

	// �����������Ԃ��i�h���C���̓G���R�[�h�A������͂��炩���߃G���R�[�h����Ă���̂ł��̂܂ܕԂ��j
	return KEY_SEARCH_ENGINE + "=" + encode_url(strSearchDomain) + "&" + KEY_SEARCH_DOMAIN + "=" + strSearchKeyword;
}

///======================================================================================
// �h���C���n�b�V����Ԃ��i�uwww.�v�͏����j
///======================================================================================
function get_domain_hash() {

	// �h���C���擾�i"www."�͏����j
	var strDomain = document.domain;
	if (strDomain.substring(0, 4) == "www.") {
		strDomain = strDomain.substring(4, strDomain.length);
	}

	return get_hash(strDomain);
}

//=========================================================================
// �n�b�V���l��Ԃ�
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
// URL�G���R�[�h
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
