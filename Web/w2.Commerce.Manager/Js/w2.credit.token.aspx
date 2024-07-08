<%--
=========================================================================================================
  Module      : クレジットトークン取得スクリプト出力画面(w2credittoken.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" ContentType="application/javascript"%>
<%@ OutputCache Duration="300" VaryByParam="*" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.YamatoKwc.Helper" %>

<%--
	これらのスクリプトはフロント・管理共通とする
--%>

<%-- カード与信失敗してもポストバックするかのフラグ（「次へ」ボタンはしない、それ以外はする） --%>
var doPostbackEvenIfCardAuthFailed = true;
<%-- トークン取得処理中件数（取得完了までをカウント） --%>
var tokenProcessingCountUntilGetTokenCount = 0;
<%-- トークン取得処理中件数（エラー表示完了までをカウント） --%>
var tokenProcessingUntilFinishDisplayErrorProcessCount = 0;
<%-- トークン取得エラー件数 --%>
var paymentErrorCount = 0;
<%-- トークンセットHidden Field --%>
var toHfID = "";
<%-- エラー発生時にフォーカスするコントロールID --%>
var errorFocusIDTmp = "";
<%-- 保持したい値トークンセットのために保持したい値（ヤマトKWCで利用） --%>
var paymentInfoForToken = "";
<%--ZEUSクライアントIP --%>
var zeusClientIp = "<%= Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP %>";
<%-- クレジットトークン取得失敗時にポストバックの代わりに実行するスクリプト --%>
var onErrorScript = "";

<%----------------------------------------------------
	ポストバックをオーバーロードて共通処理に渡す
------------------------------------------------------%>
var __doPostBackOriginal = __doPostBack;
__doPostBack = function (eventTarget, eventArgument) {
	<%-- 共通処理実行 --%>
	postbackOverloadEvent(function() {
		__doPostBackOriginal.call(this, eventTarget, eventArgument);
	});
}
<%----------------------------------------------------
	ポストバックをオーバーロードて共通処理に渡す（UpdatePanel内の場合）
------------------------------------------------------%>
var WebForm_DoPostBackWithOptionsOriginal = WebForm_DoPostBackWithOptions;
WebForm_DoPostBackWithOptions = function (eventOption) {
	<%-- 共通処理実行 --%>
	postbackOverloadEvent(function() {
		WebForm_DoPostBackWithOptionsOriginal.call(this, eventOption);
	});
}

<%----------------------------------------------------
	ポストバックをオーバーロードして実行する共通処理
	ポストバック前にトークン取得＆マスキングを行う（UpdatePanel内の場合）
------------------------------------------------------%>
var postbackOverloadEvent = function (postbackEvent) {

	<%-- 各カウント初期化 --%>
	tokenProcessingCountUntilGetTokenCount = 0;
	tokenProcessingUntilFinishDisplayErrorProcessCount = 0;

	errorFocusIDTmp = "";
	paymentErrorCount = 0;


	<%-- トークン取得（非同期） --%>
	eval(getTokenAndSetToFormJs);

	<%-- トークン取得し終わるまで待ち、マスキングやポストバック処理を行う --%>
	waitForFinishDisplayErrorProcessGetToken(function () {
		<%-- ここに来たときはエラー表示していないといけない --%>

		<%-- 必要であればポストバック --%>
		if ((paymentErrorCount == 0) || doPostbackEvenIfCardAuthFailed) {

			<%-- マスキング処理（トークン取得できているもののみ） --%>
			eval(maskFormsForTokenJs);

			<%-- ポストバック実行 --%>
			postbackEvent.call(this);
		} else {
			<%-- 次へボタンでセットされたフラグを戻す --%>
			doPostbackEvenIfCardAuthFailed = true;
			if (onErrorScript) eval(onErrorScript);
		}
	}, 100);
}

<%----------------------------------------------------
	トークンを取得してフォームにセットするイベント
------------------------------------------------------%>
function GetTokenAndSetToForm(sameAs1tokenHiddenIDs, paymentParamsSame1) {

	<%-- GMO初期化処理 --%>
	<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Gmo) { %>
		Multipayment.init("<%:Constants.PAYMENT_SETTING_GMO_SHOP_ID %>");
	<%} %>

	<%--
		カード情報取得（連想配列の配列）・ループ
		cis
			ci.CardCompany ： カード会社
			ci.CardNo ： カード番号
			ci.ExpMM ： 有効期限 （MM形式）
			ci.ExpYY ： 有効期限 （yy形式）
			ci.SecurityCode ： セキュリティコード
			ci.AuthorName ： 名義
			ci.TokenHiddenID ： Token格納先のHiddenのID（ここにTokenを入れるようにすること）
			ci.TokenHiddenIDSame1 ： Token格納先のHiddenのID（ここにTokenを入れるようにすること）※カート１と同じ決済のときに利用
			ci.errorMessageID ： エラーメッセージID
			ci.errorMessage ： エラーメッセージ
			ci.PaymentParams ： トークン作成向け決済情報（決済代行会社により異なる）
	--%>
	var cis = GetCardInfo();
	$.each(cis, function (i, ci) {

		<%-- すでにトークンがある場合はスキップ --%>
		if ($("#" + ci.TokenHiddenID)[0]
			&& $("#" + ci.TokenHiddenID).val()) return true;

		<%-- カード入力フォームなしはスキップ --%>
		if (!ci.TokenHiddenID) return true;

		<%-- 複数カートがカート１と同じ決済を利用する場合を考え、最初にそれぞれ取得 --%>
		if ((i == 0) && (sameAs1tokenHiddenIDs.indexOf(ci.TokenHiddenIDSame1) == 0)) {
			<%-- カート毎にある、（カート1と決済が同じ場合の）トークンHiddenに対して１つずつトークン取得（複数カートの場合は先に実行されたものが終わるまで待つ）  --%>
			$.each(sameAs1tokenHiddenIDs, function (f, tokenHiddenID) {
				waitForFinishGetToken(function () {
					CallgetToken(ci, tokenHiddenID, paymentParamsSame1[f], true);
				}, 100);
			});
		}
		
		<%-- 該当カートのトークンを取得する（複数カートの場合は先に実行されたものが終わるまで待つ） --%>
		waitForFinishGetToken(function () {
			CallgetToken(ci, ci.TokenHiddenID, ci.PaymentParams, false);
		}, 100);
	});
	return false;
}

<%----------------------------------------------------
	カード情報取得
------------------------------------------------------%>
function GetCardInfo() {
	var cis = []
	$("[name=hidCinfo]").each(function (i, elem) {
		var hval = '(' + ($(elem).val()) + ')';
		var ci = eval(hval);
		cis.push(ci);
	});
	return cis;
}

<%----------------------------------------------------
	トークン取得実行処理
------------------------------------------------------%>
// get Token
function CallgetToken(ci, tokenHiddenID, paymentParams, isSameAs1Process) {

	<%-- 該当カートのTokenHiddenIDSame1であればスキップ --%>
	if (ci.TokenHiddenIDSame1 == tokenHiddenID) return;

	<%-- ※この値は途中でretunする場合は-1すること --%>
	tokenProcessingCountUntilGetTokenCount++;
	tokenProcessingUntilFinishDisplayErrorProcessCount++;

	<%-- 入力チェック --%>
	<%-- セキュリティコードがない場合はエラーとする（セキュリティ上必須にしたい） --%>
	if ((!ci.CardNo)
		|| (ci.CardNo.match(/[^0-9]+/))
		|| (!ci.ExpMM)
		|| (ci.ExpMM.match(/[^0-9]+/))
		|| (!ci.ExpYY)
		|| (ci.ExpYY.match(/[^0-9]+/))
		|| (!ci.AuthorName)
		|| (ci.AuthorName.match(/[^a-zA-Z ]+/))
		<%if (OrderCommon.CreditSecurityCodeEnable) {%> || (!ci.SecurityCode)<%} %>
		<%if (OrderCommon.CreditSecurityCodeEnable) {%> || (ci.SecurityCode.match(/[^0-9]+/))<%} %>) {
			dispTokenError(ci);
			paymentErrorCount++;
			tokenProcessingCountUntilGetTokenCount--;
			tokenProcessingUntilFinishDisplayErrorProcessCount--;
			return;
	}

	<%-- まずはエラー非表示に --%>
	if (!isSameAs1Process) $("#" + ci.errorMessageID).css('display', 'none');

	<%-- トークン取得API実行 --%>
	toHfID = tokenHiddenID;
	$("#" + toHfID).val("");

	<%-- ▼▼GMOトークン取得▼▼ --%>
	<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Gmo) { %>
	Multipayment.getToken({
		cardno: ci.CardNo,
		expire: '20' + ci.ExpYY + ci.ExpMM,
		securitycode: ci.SecurityCode,
		holdername: ci.AuthorName
	}, getTokenCallbackGmo);
	<%} %>
	<%-- ▲▲GMOトークン取得▲▲ --%>
	<%-- ▼▼SBPSトークン取得▼▼ --%>
	<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.SBPS) { %>
	com_sbps_system.generateToken({
		merchantId : '<%= Constants.PAYMENT_SETTING_SBPS_MERCHANT_ID %>',
		serviceId : '<%= Constants.PAYMENT_SETTING_SBPS_SERVICE_ID %>',
		ccNumber : ci.CardNo,
		ccExpiration : '20' + ci.ExpYY + ci.ExpMM,
		securityCode : ci.SecurityCode
	}, getTokenCallbackSbps);
	<%} %>
	<%-- ▲▲SBPSトークン取得▲▲ --%>
	<%-- ▼▼ZEUSトークン取得▼▼ --%>
	<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Zeus) { %>
	var zeusToken = new zeusTokenClass();
	zeusToken.ipcode = zeusClientIp;
	zeusToken.getToken({
		cardNumber: ci.CardNo,
		cardExpiresYear: '20' + ci.ExpYY,
		cardExpiresMonth: ci.ExpMM,
		cardCvv: ci.SecurityCode,
		cardName: ci.AuthorName
	}, getTokenCallbackZeus);
	<%} %>
	<%-- ▲▲ZEUSトークン取得▲▲ --%>
	<%-- ▼▼ヤマトKWCークン取得▼▼ --%>
	<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.YamatoKwc) { %>
	<%-- 保持しておきたい値をセット --%>
	paymentInfoForToken = paymentParams[0] + " " + paymentParams[1];

	var createTokenInfo = {
		traderCode: '<%= Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE %>',
		authDiv: "<%= Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_SECURITYCODE ? "2" : "0" %>",
		optServDiv: "01",	// オプションサービス受注
		memberId: paymentParams[0],
		authKey: paymentParams[1],
		checkSum: paymentParams[2],
		cardNo: ci.CardNo,
		cardOwner:  ci.AuthorName,
		cardExp: ci.ExpMM + ci.ExpYY,
		securityCode: ci.SecurityCode
	};

	// ｗｅｂコレクトが提供するJavaScript関数を実行し、トークンを発行する。
	WebcollectTokenLib.createToken(createTokenInfo, callbackSuccess, callbackFailure);
	<%} %>
	<%-- ▲▲ヤマトKWCトークン取得▲▲ --%>

	<%-- トークン取得し終わるまで待って次を実行しないようにする --%>
	waitForFinishGetToken(function () {
		<%-- 強制ポストバックさせない場合はエラー表示させる --%>
		if ((!$("#" + toHfID).val()) && (!isSameAs1Process) && !doPostbackEvenIfCardAuthFailed) {
			dispTokenError(ci);
		}
		tokenProcessingUntilFinishDisplayErrorProcessCount--;
	}, 50);
}

<%-- ▼▼GMOトークンコールバック▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Gmo) { %>
<%----------------------------------------------------
	GMOトークン取得後処理（コールバック）
------------------------------------------------------%>
function getTokenCallbackGmo(result) {
	var resc = result.resultCode;
	if (resc == "000") {
		<%-- トークンと有効期限をセット --%>
		$("#" + toHfID).val(result.tokenObject.token + " " + result.tokenObject.toBeExpiredAt);
	} else {
		paymentErrorCount++;
	}
	tokenProcessingCountUntilGetTokenCount--;
}
<%} %>
<%-- ▲▲GMOトークンコールバック▲▲ --%>
<%-- ▼▼SBPSトークンコールバック▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.SBPS) { %>
<%----------------------------------------------------
	SBPSトークン取得後処理（コールバック）
------------------------------------------------------%>
var getTokenCallbackSbps = function(response) {
	if (response.result == "OK") {
		<%-- トークンとトークンキーをセット --%>
		$("#" + toHfID).val(response.tokenResponse.token + " " + response.tokenResponse.tokenKey);
	} else {
		paymentErrorCount++;
	}
	tokenProcessingCountUntilGetTokenCount--;
}
<%} %>
<%-- ▲▲SBPSトークンコールバック▲▲ --%>
<%-- ▼▼ZEUSトークンコールバック▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.Zeus) { %>
<%----------------------------------------------------
	ZEUSトークン取得後処理（コールバック）
------------------------------------------------------%>
var getTokenCallbackZeus = function(response) {
	if (response['result']) {
		<%-- トークンセット --%>
		$("#" + toHfID).val(response['token_key']);
	} else {
		paymentErrorCount++;
	}
	tokenProcessingCountUntilGetTokenCount--;
}
<%} %>
<%-- ▲▲ZEUSトークンコールバック▲▲ --%>
<%-- ▼▼ヤマトKWCークンコールバック▼▼ --%>
<%if (Constants.PAYMENT_CARD_KBN == w2.App.Common.Constants.PaymentCard.YamatoKwc) { %>
<%----------------------------------------------------
	ヤマトKWCトークン取得後処理（コールバック）
------------------------------------------------------%>
<%-- 「正常」 --%>
var callbackSuccess = function(response) {
	$("#" + toHfID).val(paymentInfoForToken + " " + response.token);
	<%-- トークンセット --%>
	tokenProcessingCountUntilGetTokenCount--;
	paymentInfoForToken = "";
}
<%-- 「異常」 --%>
var callbackFailure = function(response) {
	paymentErrorCount++;

	for (var i = 0; i < response.errorInfo.length; i++) {
		alert( response.errorInfo[i].errorCode + " : " +  response.errorInfo[i].errorMsg);
	}
	tokenProcessingCountUntilGetTokenCount--;
	paymentInfoForToken = "";
};
<%} %>
<%-- ▲▲ヤマトKWCークンコールバック▲▲ --%>

<%----------------------------------------------------
	トークン取得処理が終わるまで待つ
------------------------------------------------------%>
function waitForFinishGetToken(func, ms) {
	setTimeout(function () {
		if (tokenProcessingCountUntilGetTokenCount == 0) {
			func.call();
		} else {
			waitForFinishGetToken(func, ms);
		}
	}, ms);
}
<%----------------------------------------------------
	エラー表示処理が終わるまで待つ
------------------------------------------------------%>
function waitForFinishDisplayErrorProcessGetToken(func, ms) {
	setTimeout(function () {
		if ((tokenProcessingCountUntilGetTokenCount == 0) && (tokenProcessingUntilFinishDisplayErrorProcessCount == 0)) {
			func.call();
		} else {
			waitForFinishDisplayErrorProcessGetToken(func, ms);
		}
	}, ms);
}

<%----------------------------------------------------
	トークン取得エラー表示
------------------------------------------------------%>
function dispTokenError(ci) {

	<%-- 次へ遷移ではないときはエラー表示しない --%>
	if (doPostbackEvenIfCardAuthFailed) return;

	<%-- エラー表示 --%>
	$("#" + ci.errorMessageID).css('display', 'block');
	$("#" + ci.errorMessageID).html(ci.errorMessage.replace(/\r?\n/g, "<br />"));

	<%-- 初回エラーのときだけフォーカスしたい --%>
	if (!errorFocusIDTmp)
	{
		errorFocusIDTmp = ci.errorFocusID;
		if (doPostbackEvenIfCardAuthFailed == false) $("#" + errorFocusIDTmp).focus();
	}
}

<%----------------------------------------------------
	トークン向けマスキング処理
------------------------------------------------------%>
function maskingInputForToken(cardIds, replace, nomaskingLastStringLength) {

	<%-- 全体のカード番号取得 --%>
	var cardNo = "";
	$.each(cardIds, function(i, id){
		if ($("#" + id) && $("#" + id).val()) {
			cardNo += $("#" + id).val();
		}
	});
	<%-- マスキング --%>
	var maskedNo = "";
	if (cardNo.length > nomaskingLastStringLength) {
		var firstHalf = cardNo.substring(0, cardNo.length - nomaskingLastStringLength);
		var lastHalf = cardNo.substring(cardNo.length - nomaskingLastStringLength);
		maskedNo = Array(firstHalf.length + 1).join(replace) + lastHalf;
	}
	<%-- 再セット --%>
	var index = 0;
	$.each(cardIds, function(i, id){
		if ($("#" + id) && $("#" + id).val()) {
			var length = $("#" + id).val().length;
			var value = maskedNo.substr(index, length);
			$("#" + id).val(value);
			index += length;
		}
	});
}