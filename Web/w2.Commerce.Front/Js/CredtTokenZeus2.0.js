
var zeusTokenClass = function () {

	this.cgiUrl = "https://linkpt.cardservice.co.jp/cgi-bin/token/token.cgi";

	this.zeusTokenItem = {
		"zeus_token_action_type_quick_label": "登録済みのカードを使う",
		"zeus_token_card_cvv_for_registerd_card_label": "セキュリティコード",
		"zeus_token_action_type_new_label": "新しいカードを使う",
		"zeus_token_card_number_label": "カード番号",
		"zeus_token_card_expires_label": "カード有効期限",
		"zeus_token_card_expires_month_suffix": "月",
		"zeus_token_card_expires_year_suffix": "年",
		"zeus_token_card_expires_note": "例）12月　2012年",
		"zeus_token_card_cvv_for_new_card_label": "セキュリティコード",
		"zeus_token_card_name_label": "カード名義",
		"zeus_token_error_messages": {
			"88888888": "メンテナンス中です。",
			"90100100": "通信に失敗しました。",
			"99999999": "その他のシステムエラーが発生しました。",

			"02030105": "METHOD が 'POST' 以外",
			"02030106": "CONTENT-TYPE が 'text/xml' もしくは 'application/xml' 以外",
			"02030107": "CONTENT-LENGTH が存在しないか、0 が指定されている",
			"02030108": "CONTENT-LENGTH が 8192 byte より大きい",
			"02030207": "XML データが未送信",
			"02030208": "XML データが 8192 byte より大きい",
			"02030209": "XML データに構文エラーがある",
			"02080114": "XML の action が空",
			"02080115": "無効な action が指定されている",
			"02130114": "XML に authentication clientip が存在しない",
			"02130117": "clientip のフォーマットが不正",
			"02130110": "不正な clientip が指定された",
			"02130118": "不正な clientip が指定された",
			"02130514": "「カード番号」を入力してください。",
			"02130517": "「カード番号」を正しく入力してください。",
			"02130619": "「カード番号」を正しく入力してください。",
			"02130620": "「カード番号」を正しく入力してください。",
			"02130621": "「カード番号」を正しく入力してください。",
			"02130640": "「カード番号」を正しく入力してください。",
			"02130714": "「有効期限(年)」を入力してください。",
			"02130717": "「有効期限(年)」を正しく入力してください。",
			"02130725": "「有効期限(年)」を正しく入力してください。",
			"02130814": "「有効期限(月)」を入力してください。",
			"02130817": "「有効期限(月)」を正しく入力してください。",
			"02130825": "「有効期限(月)」を正しく入力してください。",
			"02130922": "「有効期限」を正しく入力してください。",
			"02131014": "CVVが不正です。",
			"02131017": "「セキュリティコード」を正しく入力してください。",
			"02131117": "「カード名義」を正しく入力してください。",
			"02131123": "「カード名義」を正しく入力してください。",
			"02131124": "「カード名義」を正しく入力してください。",
		},
	};

	if (typeof zeusTokenCustomItem == "object" && zeusTokenCustomItem) {
		for (var zeus_token_item_tmp_name in this.zeusTokenItem) {
			if (typeof zeusTokenCustomItem[zeus_token_item_tmp_name] == "string") {
				this.zeusTokenItem[zeus_token_item_tmp_name] = zeusTokenCustomItem[zeus_token_item_tmp_name];
			}
		}
	}
};

// 入力されたカード情報から、トークンを取得する
zeusTokenClass.prototype.getToken = function (cardInfo, callback_function) {

	var request = new XMLHttpRequest();
	request.onreadystatechange = function () {
		switch (this.readyState) {
			case 4:
				if (this.status == 0) {
					// 通信失敗
					var data = {
						'result': 0,
						'error_code': '90100100',
					};
					if (typeof callback_function == 'function') {
						// 指定されたコールバック関数を実行する
						callback_function(data);
					}
					return;
				}
				if ((200 <= this.status && this.status < 300) || (this.status == 304)) {

					var xml = this.responseXML.documentElement;
					var status = '';
					if (xml.getElementsByTagName('status').length != 0) {
						status = xml.getElementsByTagName('status')[0].textContent;
					}
					var error_code = '';
					if (xml.getElementsByTagName('code').length != 0) {
						error_code = xml.getElementsByTagName('code')[0].textContent;
					}

					switch (status) {
						case 'success':
							var token_key = '';
							if (xml.getElementsByTagName('token_key').length != 0) {
								token_key = xml.getElementsByTagName('token_key')[0].textContent;
							}
							var masked_card_number = '';
							if (xml.getElementsByTagName('masked_card_number').length != 0) {
								masked_card_number = xml.getElementsByTagName('masked_card_number')[0].textContent;
							}
							var masked_cvv = '';
							if (xml.getElementsByTagName('masked_cvv').length != 0) {
								masked_cvv = xml.getElementsByTagName('masked_cvv')[0].textContent;
							}
							var card_expires_month = '';
							if (xml.getElementsByTagName('card_expires_month').length != 0) {
								card_expires_month = xml.getElementsByTagName('card_expires_month')[0].textContent;
							}
							var card_expires_year = '';
							if (xml.getElementsByTagName('card_expires_year').length != 0) {
								card_expires_year = xml.getElementsByTagName('card_expires_year')[0].textContent;
							}
							var card_name = '';
							if (xml.getElementsByTagName('card_name').length != 0) {
								card_name = xml.getElementsByTagName('card_name')[0].textContent;
							}
							var data = {
								'result': 1,
								'token_key': token_key,
								'masked_card_number': masked_card_number,
								'masked_cvv': masked_cvv,
								'card_expires_month': card_expires_month,
								'card_expires_year': card_expires_year,
								'card_name': card_name,
							};
							break;
						case 'invalid':
						case 'failure':
							var data = {
								'result': 0,
								'error_code': error_code,
							};
							break;
						case 'maintenance':
							var data = {
								'result': 0,
								'error_code': '88888888',
							};
							break;
						default:
							var data = {
								'result': 0,
								'error_code': '99999999',
							};
							break;
					}
					if (typeof callback_function == 'function') {
						// 指定されたコールバック関数を実行する
						callback_function(data);
					}
				}
		}
	}

	var action_type = ''; // 空欄で送信された場合の入力チェックは、サーバサイドで行われ、エラーコード101020で返却される。
	action_type = 'newcard';

	var zeus_token_card_number = cardInfo.cardNumber;
	var zeus_token_card_expires_year = cardInfo.cardExpiresYear;
	var zeus_token_card_expires_month = cardInfo.cardExpiresMonth;
	var zeus_token_card_cvv = cardInfo.cardCvv;
	var zeus_token_card_name = cardInfo.cardName;

	var data = '<?xml version="1.0" encoding="utf-8"?>'
		+ '<request service="token" action="' + action_type + '">'
		+ '<authentication>'
		+ '<clientip>' + this.ipcode + '</clientip>'
		+ '</authentication>'
		+ '<card>'
		+ ((zeus_token_card_cvv) ? '<cvv>' + zeus_token_card_cvv + '</cvv>' : '')
		+ '<number>' + zeus_token_card_number + '</number>'
		+ '<expires>'
		+ '<year>' + zeus_token_card_expires_year + '</year>'
		+ '<month>' + zeus_token_card_expires_month + '</month>'
		+ '</expires>'
		+ '<name>' + zeus_token_card_name + '</name>'
		+ '</card>'
		+ '</request>';
	request.open("POST", this.cgiUrl, false);
	request.setRequestHeader("Content-Type", "text/xml");
	try {
		request.send(data);
	} catch (e) {
		// 通信失敗
		var return_data = {
			'result': 0,
			'error_code': '90100100',
		};
		if (typeof callback_function == 'function') {
			// 指定されたコールバック関数を実行する
			callback_function(return_data);
		 
		}
		return;
	}
};

/////////////////////////////////////////////////////////////
//以降3ds-web-wrapperと共通
/**
 * iframeのID指定用変数
 */
var iframeId;
/**
 * EMV3DSecure認証コンテナ
 */
var threedsContainer;

/**
 * 加盟店webサーバのurl
 */
var merchantUrl;

/**
 * リスクベース認証結果の一時保存
 */
var securePaData;

/**
 * securePaのmd
 */
var xid_sha512;

/**
 * タイムアウト解除用の変数
 */
let timeout;

//iframeからのイベント受信を行う。CROS対策方式
window.addEventListener("message", receiveMessage, false);
function receiveMessage(event) {	
	//環境ごとに変化
	if (event.origin !== "https://linkpt.cardservice.co.jp"){
		return;
	}

	const jsonObj = JSON.parse(event.data);
	if(jsonObj.event == '3DSMethodSkipped' || jsonObj.event == '3DSMethodFinished'){
		clearTimeout(timeout);
		_doAuth(xid_sha512, merchantUrl);
	} else if(jsonObj.event === 'AuthResultReady'){ //チャレンジフローの終了時にiframeから受信するメッセージ
		securePaData.transStatus = jsonObj.transStatus;
		_onAuthResult(securePaData);//PaResをフロントから送信する。
	}
}

/**
 * 加盟店利用関数3.PaReqで返されたパラメータ
 * (EnrolReqの切っ掛けとなったリクエストへのレスポンス)
 * をそのまま取得する。
 * レスポンスで新しく画面を開く加盟店も、PaReq部分をこの関数に渡せばOK.
 * ajaxの加盟店はレスポンスをそのままここに渡す。
 * 2/16 paymentResultパラメータ不要化
 * @param params
 */
function setPareqParams(md,paReq,termUrl,threeDSMethod,iframeUrl){
	// 3DS認証可否の確認
	if (threeDSMethod == null) {
		//決済成功ページを表示
		doPost();
	} else {
		iframeId = String(Math.floor(100000 + Math.random() * 900000));
 
		setThreedsContainer();
 
		// 3DS認証利用OKの場合
		// termUrlを加盟店サーバURLとしてグローバル変数にセット
		merchantUrl = decodeTermurl(termUrl);
		// mdの値をグローバル変数にセット
		xid_sha512 = md;
		// ブラウザ情報収集用のiframeをコンテナに設置
		var decodeUrl = decodeURIComponent(iframeUrl);
		var appendNode = document.createElement('iframe');
		appendNode.setAttribute('id', '3ds_secureapi_' + iframeId);
		appendNode.setAttribute('width', '0');
		appendNode.setAttribute('height', '0');
		appendNode.setAttribute('style', 'visibility: hidden;');
		appendNode.setAttribute('src', decodeUrl);
		threedsContainer.appendChild(appendNode);

		// 20秒待ってもgpayからcallbackがない場合、次の認証処理を実施するためのタイマー（エラーになる想定）
		timeout = setTimeout(function(){
			_doAuth(md, merchantUrl);
		}, 20000 );
	}
}

/**
 * Get the authentication from Active Server
 * @param threeDSServerTransID
 * @param callbackFn
 */
function result(threeDSServerTransID, callbackFn) {
	getResult(threeDSServerTransID, callbackFn);
}

/**
 * Post authData to 3ds requestor with url
 * @param processName：呼出元
 * @param url
 * @param authData
 * @param onSuccess
 * @param onError
 * @param contentType
 */
function _doPost(processName, url, authData, onSuccess, onError, contentType="application/json") {
	var request = new XMLHttpRequest();
	request.open(
		'POST',
		url,
		true
	);

	request.setRequestHeader('Content-Type', contentType);
	if(contentType === "application/json"){
		authData = JSON.stringify(authData);
	}
 
	request.onload = function(data) {
		//JQueryからXHRRequestへの変換時の互換性確保。JQueryのdate = XHRのdata.target.response
		let response = data.target.response;
		if (request.status >= 200 && request.status < 400) {
			if (contentType === "application/json") {
				window.location = data.target.responseURL
			}

			onSuccess(response);
		} else {
			onError({"message": "「" + request.status + "」" + processName + " 処理エラー"});
		}
	};

	request.onerror = function(data) {
		onError({"message": "「" + request.status + "」" + processName + " 処理エラー"});
	};

	request.send(authData);
}

	/**
	 * リスクベース認証を要求し、フリクションレス/チャレンジフローを開始する.
	 * @param md
	 * @param termUrl
	 * @private
	 */
	function _doAuth(md, termUrl) {
		//加盟店SecureAPI利用 PaReq送信

		content =
			'<?xml version="1.0" encoding="utf-8"?>'
			+ '<request service="secure_link_3d" action="securePa">'
			+ '<xid>' + md +'</xid>'
			+ '</request>';
		_doPost(
			'PaReq',
				'https://linkpt.cardservice.co.jp/cgi-bin/token/token.cgi',
			content,
			function (xml) {
				try {
					// 結果に応じてチャレンジ/フリクションレスフローを開始する。
					let domparser = new DOMParser();
					let doc = domparser.parseFromString(xml, "application/xml");
					var data = {};

					if (typeof doc.getElementsByTagName("status")[0] !== 'undefined') {
						data.status = doc.getElementsByTagName("status")[0].textContent;
					}
					else {
						data.status = 'maintenance';
					}

					if (typeof doc.getElementsByTagName("xid")[0] !== 'undefined') {
						data.xid = doc.getElementsByTagName("xid")[0].textContent;
					}

					if (typeof doc.getElementsByTagName("transStatus")[0] !== 'undefined') {
						data.transStatus = doc.getElementsByTagName("transStatus")[0].textContent;
					}
					else {
						data.transStatus = "N";
					}

					if(data.status == 'success' || data.status == 'outside') {
						securePaData = data;

						//フリクションレスのとき、challengeUrlを持たない。
						//challengeUrlが設定されている（チャレンジ）時だけ、challengeUrlを設定して渡す
						if (typeof doc.getElementsByTagName("challengeUrl")[0] !== 'undefined') {
							data.challengeUrl = doc.getElementsByTagName("challengeUrl")[0].textContent
						}
					} else {
						data.xid = md; //doc.getElementsByTagName("xid")[0].textContent;
						data.transStatus = "N";
					}
				} catch (error) {
					//XMLパースエラーなど発生時、失敗時の関数を呼ぶ
					_onError({"message": error.message});
				}
				_onDoAuthSuccess(data);
			},
			function (error) {
				_onError({"message": error.message});
			},
			"application/xml"
		);
	}

	/**
	 * callback function for _doAuth
	 * @param data
	 * @private
	 */
	// リスクベース認証完了後に動作
	function _onDoAuthSuccess(data) {
		if (data.transStatus === "C" || data.transStatus === "D") {
			// チャレンジフロー（C, D）
			data.challengeUrl
				? startChallenge(data.challengeUrl)
				: _onError({"message": "追加認証要求URLがありません。"});
		} else {
			/* iframe remove */
			//var iframe  = document.getElementById('3ds_secureapi_' + iframeId);
			//iframe.remove();
			//チャレンジ以外
			_onAuthResult(data);
		}
	}

	/**
	 * Setup iframe for challenge flow (Step 14(C))
	 * @param url is the challenge url returned from 3DS Server
	 */
	function startChallenge(url) {
		var challengeUrl = decodeURIComponent(url);
		var appendNode = document.createElement('iframe');
		appendNode.setAttribute('id', '3ds_challenge');
		appendNode.setAttribute('width', '100%');
		appendNode.setAttribute('height', '100%');
		appendNode.setAttribute('style', 'border:0');
		appendNode.setAttribute('src', challengeUrl);
		appendNode.setAttribute('onload', 'loadedChallenge()');

		setThreedsContainer();

		threedsContainer.appendChild(appendNode);
	}

	/**
	 * チャレンジ/フリクションレスフロー完了時に呼ばれるメソッド
	 **/
	function _onAuthResult(data) {
		sendPaRes(data);
	}

	/**
	 * PaResをPaReqで受け取ったTermUrlに向けて送信
	 */
	function sendPaRes(data) {
		var paResData = {};
		paResData.MD = data.xid;
		paResData.PaRes = data.transStatus;
		paResData.status = data.status;
		_doPost('PaRes', merchantUrl, paResData, _onPaResSuccess, _onError);
	}

	/**
	 * PaRes成功後のデフォルト関数。挙動なし。
	 * この宣言以降で同名関数の宣言によって挙動上書き可能。
	 */
	function _onPaResSuccess(data) {
	}

	/**
	 * この関数を上書きすることでエラー時の挙動を制御できます.
	 * @param error
	 * @private
	 */
	function _onError(error) {
	}

	function setThreedsContainer() {
		threedsContainer = document.querySelector("div[id='3dscontainer']");
		if(threedsContainer == null){
			var containerDiv = document.createElement("div");
			containerDiv.id = '3dscontainer';
			document.body.appendChild(containerDiv);
			threedsContainer = document.querySelector("div[id='3dscontainer']");
		}
	}

	function decodeTermurl(termUrl){
		var re = /^(https?:\/\/)/i;
		var returnUrl;
	
		if(re.test(termUrl)){
			returnUrl = termUrl;
		} else {
			returnUrl = decodeURIComponent(termUrl);
		}
		return returnUrl;
	}

	/**
	 * チャレンジ認証画面ロード完了時、加盟店側のメッセージを非表示とする関数
	 */
	function loadedChallenge() {
		var div_waiter;
		if ( div_waiter = document.querySelector("div[id='challenge_wait']") ) {
			div_waiter.style.display = 'none';
		}
	}