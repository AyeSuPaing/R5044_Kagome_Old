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