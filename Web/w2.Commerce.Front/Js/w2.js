
//======================================================================================
// �N���b�N�W���b�N�U���΍�
//======================================================================================
function CheckClickJack(strBlankPageUrl, strErrorPageUrl)
{
	try
	{
		// �C�����C���t���[���H
		if (window.self.location != window.top.location)
		{
			// �h���C�����قȂ�΃u�����N��
			if (window.top.location.host != window.self.location.host)
			{
				location.href = strBlankPageUrl;
			}
		}
	}
	catch (ex)
	{
		// ���ۂɂ̓N���X�T�C�g�̃h���C���͎Q�Ƃł��Ȃ��̂ł����Ńu�����N��
		location.href = strBlankPageUrl;
	}
}

//======================================================================================
// ��d�����`�F�b�N�h�~
//======================================================================================
var exec_submit_flg = 0;
function exec_submit()
{
	if (exec_submit_flg == 0)
	{
		exec_submit_flg = 1;
		return true;
	}
	else
	{
		return false;
	}
}

// iOS bfcache�΍�
var use_bf_cache_flg = false;
var request_count = 0;
window.onpageshow = function(e) {
	if (e.persisted)
	{
		exec_submit_flg = 0;
		use_bf_cache_flg = true;
		request_count = 0;
	}
};

// IOS safari�ł͕�����̃u���E�U�o�b�N��pageshow�C�x���g���������Ȃ����߁A�������̍X�V�����m������
history.replaceState(null, document.getElementsByTagName('title')[0].innerHTML, null);
window.addEventListener('popstate', function (e) {
	request_count = 0;
});

//======================================================================================
// �摜�����ւ��̃��Z�b�g
//======================================================================================
var strOriginalImageNameMouseMoveChange = null;
function reset_picture(tag_name)
{
	var obj = document.getElementById(tag_name);
	if (obj != null)
	{
		// �I���W�i���̖��O�ɖ߂�
		if (strOriginalImageNameMouseMoveChange != null)
		{
			obj.src = strOriginalImageNameMouseMoveChange;
		}
	}
}

//======================================================================================
// �摜�����ւ�
//======================================================================================
function change_picture(tag_name, file)
{
	var obj = document.getElementById(tag_name);
	if (obj != null)
	{
		// �I���W�i���̖��O��ۑ�
		if (strOriginalImageNameMouseMoveChange == null)
		{
			strOriginalImageNameMouseMoveChange = obj.src;
		}
		obj.src = file;
	}
	return false;
}

//======================================================================================
// �E�C���h�E�|�b�v�A�b�v
//======================================================================================
function show_popup_window(strUrl, iWidth, iHeight, blDispScrollbar, blResizable, strWindowName)
{
	var strScrollbars = blDispScrollbar ? "yes" : "no";
	var strResizable = blResizable ? "yes" : "no";
	scWidthCenter = screen.availWidth / 2;
	scHeightCenter = screen.availHeight / 2;
	var strOption = "toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=" + strScrollbars + ",resizable=" + strResizable + ",width=" + iWidth + ",height=" + iHeight + ",left=" + (scWidthCenter - (iWidth / 2)) + ",top=" + (scHeightCenter - (iHeight / 2));
	var wObj = window.open(strUrl, strWindowName, strOption);
	if (wObj != null)
	{
		wObj.focus();
	}
}

// ���C�ɓ���o�^��J�ڗp�`�F�b�N
function add_favorite_check(isAlready) {
	var message = (isAlready) ? "���C�ɓ���ɓo�^�ς݂ł��B" : "���C�ɓ���ɓo�^���܂����B";
	$('#txt-tooltip').html(message);
	$('#addFavoriteTip').css("top", yPos + 20);
	$('#addFavoriteTip').css("left", xPos + 20);
	showTooltip();
}

//tootip��\�����鏈��
function showTooltip() {
	// �J�[�g��������������Ȃ�c�[���`�b�v�\���������s��
	$('p.toolTip').fadeIn(function () {
		autoFadeOut();
	});
	// �J�[�g�����I��cookie���N���A
	var delDate = new Date();
	delDate.setTime(0);

	var popcounter;
	function autoFadeOut() {
		popcounter = setTimeout(function () {
			$('p.toolTip').fadeOut();
		}, 2000);
	}

	$('p.toolTip').mouseenter(function () {
		clearTimeout(popcounter);
	});
	$('p.toolTip').mouseleave(function () {
		autoFadeOut();
	});
}

var xPos, yPos;
// �}�E�X�C�x���g�̏�����
function init() {
	if (window.Event) {
		document.captureEvents(Event.MOUSEMOVE);
	}
	document.onmousemove = getXY;
}

// �}�E�X��XY���W���擾 
function getXY(e) {
	xPos = (window.Event) ? e.clientX : event.clientX;
	yPos = (window.Event) ? e.clientY : event.clientY;
}

// onload�C�x���g��ǉ�����B 
function addOnload(func) {
	try {
		window.addEventListener("load", func, false);
	} catch (e) {
		// IE�p 
		window.attachEvent("onload", func);
	}
}


//======================================================================================
// �y�[�W���[�h���ʏ���
//======================================================================================
function pageLoad_common() {
	// ���d�񓯊����N�G�X�g�L�����Z��
	cancel_multiple_async_request();
}

//======================================================================================
// ���d�񓯊����N�G�X�g�L�����Z��
//======================================================================================
function cancel_multiple_async_request() {
	// ���݁A���s���̔񓯊��ʐM�����݂���Ώ����L�����Z��
	if (Sys.WebForms) {
		var mng = Sys.WebForms.PageRequestManager.getInstance();
		mng.add_initializeRequest(
			function (sender, args) {
				if (mng.get_isInAsyncPostBack() && ((use_bf_cache_flg == false) || request_count != 0)) {
					args.set_cancel(true);
				} else {
					request_count += 1;
				}
			}
		);

		// �񓯊��|�X�g�o�b�N�̊�����̏���
		mng.add_endRequest(
			function (sender, args) {
				request_count -= 1;
			}
		);
	}
}

//======================================================================================
// �����i���E���j�̎����U�艼���ϊ������s����i�Ђ炪�ȗp�j
//======================================================================================
function execAutoKanaHiragana(firstName, firstNameKana, lastName, lastNameKana) {
	$.fn.autoKana(firstName, firstNameKana);
	$.fn.autoKana(lastName, lastNameKana);
}

//======================================================================================
// �����i���E���j�̎����U�艼���ϊ������s����i�J�^�J�i�p�j
//======================================================================================
function execAutoKanaKatakana(firstName, firstNameKana, lastName, lastNameKana) {
	$.fn.autoKana(firstName, firstNameKana, { katakana: true });
	$.fn.autoKana(lastName, lastNameKana, { katakana: true });
}

//======================================================================================
// �ӂ肪�ȁi���E���j�̎����J�i�����ȕϊ������s����i�Ђ炪�ȗp�j
//======================================================================================
function execAutoChangeKanaHiragana(firstNameKana, lastNameKana) {
	$.fn.autoChangeKana(firstNameKana);
	$.fn.autoChangeKana(lastNameKana);
}

//======================================================================================
// �ӂ肪�ȁi���E���j�̎������ȁ��J�i�ϊ������s����i�J�^�J�i�p�j
//======================================================================================
function execAutoChangeKanaKatakana(firstNameKana, lastNameKana) {
	$.fn.autoChangeKana(firstNameKana, { katakana: true });
	$.fn.autoChangeKana(lastNameKana, { katakana: true });
}

//======================================================================================
// �X�֔ԍ������`�F�b�N�i�X�֔ԍ����������s���邩�̔���j
//======================================================================================
function checkZipCodeLength(zip1, zip2) {
	return ((zip1.val().length + zip2.val().length) == 7);
}

//======================================================================================
// �X�֔ԍ������`�F�b�N�y�їX�֔ԍ��������s��
//======================================================================================
function checkZipCodeLengthAndExecPostback(zip1, zip2, eventTarget, postUrl, errorMessageId) {
	if (checkZipCodeLength(zip1, zip2)) {
		getAddrJsonAsync(zip1.val(), zip2.val(), eventTarget, postUrl, errorMessageId);
	}
}

//======================================================================================
// �X�֔ԍ������`�F�b�N�y�їX�֔ԍ��������s���iSP�p�j
//======================================================================================
function checkZipCodeLengthAndExecPostbackForSp(zip1, zip2, eventTarget, postUrl, errorMessageId, errorMessageNecessary, errorMessageLength) {
	var zipcodeLength = zip1.val().length + zip2.val().length;
	if (zipcodeLength == 7) {
		getAddrJsonAsync(zip1.val(), zip2.val(), eventTarget, postUrl, errorMessageId);
	} else if (zipcodeLength == 0) {
		$(errorMessageId).html(errorMessageNecessary + errorMessageLength);
	} else {
		$(errorMessageId).html(errorMessageLength);
	}
}

//======================================================================================
// �����i���E���j�̎����U�艼���ϊ������s����
//======================================================================================
function execAutoKana(firstName, firstNameKana, lastName, lastNameKana, kanaType) {
	// �U�艼����ʂ��Ђ炪�Ȃ̏ꍇ�A�Ђ炪�ȕϊ����s���B�J�^�J�i�̏ꍇ�A�J�^�J�i�ϊ����s��
	if (kanaType == 'FULLWIDTH_HIRAGANA') {
		execAutoKanaHiragana(firstName, firstNameKana, lastName, lastNameKana);
	} else if (kanaType == 'FULLWIDTH_KATAKANA') {
		execAutoKanaKatakana(firstName, firstNameKana, lastName, lastNameKana);
	}
}

//======================================================================================
// �ӂ肪�ȁi���E���j�̎������ȁ����J�i�ϊ������s����
//======================================================================================
function execAutoChangeKana(firstNameKana, lastNameKana, kanaType) {
	// �U�艼����ʂ��Ђ炪�Ȃ̏ꍇ�A�Ђ炪�ȕϊ����s���B�J�^�J�i�̏ꍇ�A�J�^�J�i�ϊ����s��
	if (kanaType == 'FULLWIDTH_HIRAGANA') {
		execAutoChangeKanaHiragana(firstNameKana, lastNameKana);
	} else if (kanaType == 'FULLWIDTH_KATAKANA') {
		execAutoChangeKanaKatakana(firstNameKana, lastNameKana);
	}
}

//======================================================================================
// �����Z���������ʃ��C���[��\������
//======================================================================================
function showPopupAndLayer(fadeIntime) {
	$('#search-result-layer').fadeIn(fadeIntime);
}

//======================================================================================
// �����Z���������ʃ��C���[�����
//======================================================================================
function closePopupAndLayer() {
	// �Z�������������ʂ��폜����
	$(".search-result-layer-addrs").empty();

	$('#search-result-layer').hide();
}

//======================================================================================
// �����Z�����擾����
//======================================================================================
function getAddrJsonAsync(zipcode1, zipcode2, eventTarget, postUrl, errorMessageId) {
	// �񓯊��ʐM�ŕ����Z����������Json���擾����
	$.ajax({
		type: "POST",
		url: postUrl,
		contentType: "application/json; charset=utf-8",
		dataType: 'json',
		data: "{ zipcode1: '" + zipcode1 + "', zipcode2: '" + zipcode2 + "' }",
		success: function (result) {
			// �����Z���������ʂ����C���[�Ƀo�C���h����
			var jsonAddrSearchResult = JSON.parse(result.d);
			var addrSearchResultCount = jsonAddrSearchResult.length
			$(".search-result-layer-addrs").empty();
			$.each(jsonAddrSearchResult, function (i, addr) {
				$('.search-result-layer-addrs')
					.append('<li><span class="addr">' + addr.Prefecture +
							'</span>&nbsp;<span class="city">' + addr.City +
							'</span>&nbsp;<span class="town">' + addr.Town +
							'</span></li>');
			});

			// �Z���̑Ώی��������݂��Ȃ��܂���1���̏ꍇ�͏Z�������C�x���g�����s�A2���ȏ�̏ꍇ�͌������ʂ����C���[�Ƀo�C���h����
			if ((addrSearchResultCount == 0) || (addrSearchResultCount == 1)) {
				__doPostBack(eventTarget, "");
			} else if (addrSearchResultCount > 1) {
				showPopupAndLayer(200);
				$('.search-result-count').text("[" + addrSearchResultCount + "��]");
				$('.search-result-layer-addrs li').attr('class', 'search-result-layer-addr');
				$(".search-result-layer-addr").css("padding", "10px 20px");
				$(errorMessageId).html("");
			}
		},
		error: function () {
		}
	});
}

//======================================================================================
// �L�[�C�x���g���̗L���ȃL�[�R�[�h�ł��邩�̔���
//======================================================================================
function isValidKeyCodeForKeyEvent(keyCode) {
	// �L�[�R�[�h����
	// [9�FTab], [35�FEnd], [36�FHome], [37�F��], [38�F��], [39�F��], [40�F��]
	var invalidKeyCodes = [9, 35, 36, 37, 38, 39, 40];
	return ($.inArray(keyCode, invalidKeyCodes) == -1);
}

//======================================================================================
// ���W�y�[�W�R���e���c���ёւ�
//======================================================================================

function sortFeaturePageContents(sort) {
  for (var startPos = 0; startPos < sort.length; startPos++) {
    var startElement = $('#' + sort[startPos]);
    for (var afterElementPos = startPos + 1; afterElementPos < sort.length; afterElementPos++) {
      var afterElement = $('#' + sort[afterElementPos]);

      // ���я����t�ɂȂ��Ă���̂Ō�������
      if(sort[afterElementPos] == 'feature-group-items'){
        if ($(startElement).index() > $(afterElement).parent().index()) {
          swapItem("#" + sort[startPos], "#" + sort[afterElementPos]);
        }
      } else if (sort[startPos] == 'feature-group-items'){
        if ($(startElement).parent().index() > $(afterElement).index()) {
          swapItem("#" + sort[startPos], "#" + sort[afterElementPos]);
        }
      } else{
        if ($(startElement).index() > $(afterElement).index()) {
          swapItem("#" + sort[startPos], "#" + sort[afterElementPos]);
        }
      }
    }
  }
};

//======================================================================================
// �v�f����ւ�
//======================================================================================
function swapItem(from, to) {
	var $from = $(from);
	var $to = $(to);

	$from.replaceWith('<div id="REPLACE_TEMP_DIV"></div>');
	$to.replaceWith($from);
	$("#REPLACE_TEMP_DIV").replaceWith($to);
}

function LoadingShow() {
    $("#loadingBackground").show();
    $("#loadingIcon").fadeIn();
}

function LoadingMessageShow() {
    $("#loadingBackground").show();

    $("#loadingIcon").css('left','40%');
    $("#loadingIcon").css('width','300px');
    $("#loadingIcon").css('height','100px');
    $("#loadingIcon").css('border-radius','5px');
    $("#loadingIcon").fadeIn();

    $("#loadingMessage").fadeIn();
}

function LoadingMessageSpShow() {
    $("#loadingBackground").show();

    $("#loadingIcon").css('left','30%');
    $("#loadingIcon").css('top','30%');
    $("#loadingIcon").css('width','180px');
    $("#loadingIcon").css('height','130px');
    $("#loadingIcon").css('line-height','20px');
    $("#loadingIcon").fadeIn();

    $("#loadingMessage").fadeIn();
}

function LoadingHide() {
    $("#loadingBackground").fadeOut();
    $("#loadingIcon").fadeOut();
    $("#loadingMessage").fadeOut();
}

// �Z�b�V�����ێ����\�b�h
var MaintainSession = (function () {
    var path = '';
    var mouseX = 0;
    var mouseY = 0;

	// ���C�����\�b�h
	function _execute(pathroot) {
		path = pathroot + 'MaintainSession.ashx';
		setInterval(function () {
			document.addEventListener('touchstart', _moveListener);
			document.addEventListener('mousemove', _moveListener);
		}, 18 * 60 * 1000);
	};

	// �ړ����m�C�x���g���X�i�[
	function _moveListener(e) {
		if (mouseX !== e.clientX || mouseY !== e.clientY) {
			_fetch();
			document.removeEventListener('mousemove', _moveListener);
			document.removeEventListener('touchstart', _moveListener);
		}
		e = window.event || e;
		mouseX = e.clientX;
		mouseY = e.clientY;
	}

	// �Z�b�V�����ێ��p���N�G�X�g���\�b�h
	function _fetch() {
		try {
			var http = new XMLHttpRequest();
			http.open('GET', path);
			http.send(null);
		} catch (e) {
			// �ʐM�G���[�͖���
		}
	}

	return {
		execute: _execute
	}
}());

// Amazon(CV2)�X�N���v�g
function showAmazonPayCv2Button(divId, sellerId, isSandbox, payload, signature, publickKeyId) {
	if ($(divId).length) {
		amazon.Pay.renderButton(divId, {
			merchantId: sellerId,
			ledgerCurrency: 'JPY',
			sandbox: isSandbox,
			checkoutLanguage: 'ja_JP',
			productType: 'PayAndShip',
			placement: 'Cart',
			buttonColor: 'Gold',
			createCheckoutSessionConfig: {
				payloadJSON: payload,
				signature: signature,
				publicKeyId: publickKeyId
			}
		});
	}
}
// Amazon���O�C��(CV2)�X�N���v�g
function showAmazonSignInCv2Button(divId, sellerId, isSandbox, payload, signature, publickKeyId) {
	if ($(divId).length) {
		amazon.Pay.renderButton(divId, {
			merchantId: sellerId,
			ledgerCurrency: 'JPY',
			sandbox: isSandbox,
			checkoutLanguage: 'ja_JP',
			productType: 'SignIn',
			placement: 'Cart',
			buttonColor: 'Gold',
			signInConfig: {
				payloadJSON: payload,
				signature: signature,
				publicKeyId: publickKeyId
			}
		});
	}
}
// Amazon(CV2)�X�N���v�g
function showAmazonCv2Button(divId, sellerId, isSandbox, payload, signature, publickKeyId, type) {
	switch (type) {
		case 'PayAndShip':
			showAmazonPayCv2Button(divId, sellerId, isSandbox, payload, signature, publickKeyId);
			break;
		case 'SignIn':
			showAmazonSignInCv2Button(divId, sellerId, isSandbox, payload, signature, publickKeyId);
			break;
		default:
			return;
	}
}

// ���[���h���C�����擾
function getMailDomains(source)
{
	var datas = [];
	$.ajax({
		type: "GET",
		url: source,
		dataType: "xml",
		global: false,
		async: false,
		success: function (xmlContent)
		{
			$(xmlContent).find('value').each(function ()
			{
				datas.push($(this).text());
			});
		}
	});
	return datas;
}

// �w�肵���v�f�����S�ɗ���悤�ɃX�N���[��
function scrollToElementCenter(elementId) {
	var element = document.getElementById(elementId);
	element.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

// Check zip code short input length
function checkZipCodeShortInputLength(zip) {
	var result = ((zip.length >= 7) && (zip.length <= 8));
	return result;
}

// Check zip code short input length and execution postback
function checkZipCodeShortInputLengthAndExecPostback(
	zip,
	eventTarget,
	postUrl,
	errorMessageId) {
	var zipCode = zip.val().replace("-", "");
	if (checkZipCodeShortInputLength(zipCode)) {
		getAddrJsonAsync(
			zipCode.substr(0, 3),
			zipCode.substr(3, zipCode.length - 3),
			eventTarget,
			postUrl,
			errorMessageId);
	}
}

// Check zip code short input length and execution postback for smart phone
function checkZipCodeShortInputLengthAndExecPostbackForSp(
	zip,
	eventTarget,
	postUrl,
	errorMessageId,
	errorMessageNecessary,
	errorMessageLength) {
	var zipCode = zip.val().replace("-", "");
	var zipcodeLength = zipCode.length;

	if ((zipcodeLength == 7)
		|| (zipcodeLength == 8)) {
		getAddrJsonAsync(
			zipCode.substr(0, 3),
			zipCode.substr(3, zipCode.length - 3),
			eventTarget,
			postUrl,
			errorMessageId);
	} else if (zipcodeLength == 0) {
		$(errorMessageId).html(errorMessageNecessary + errorMessageLength);
	} else {
		$(errorMessageId).html(errorMessageLength);
	}
}

// Set zip code
function textboxChangeSearchZipCode(
	textboxId,
	textboxId1,
	textboxId2,
	uniqueId,
	uniqueId2,
	url,
	errorMessageId) {
	if (textboxId2 == "") {
		$('#' + textboxId).keyup(function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			checkZipCodeShortInputLengthAndExecPostback(
				$('#' + textboxId),
				uniqueId,
				url,
				errorMessageId)
		});
	}
	else {
		$('#' + textboxId2).keyup(function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			checkZipCodeLengthAndExecPostback(
				$('#' + textboxId1),
				$('#' + textboxId2),
				uniqueId2,
				url,
				errorMessageId)
		});
	}
}

// Click search zip code
function clickSearchZipCode(
	textboxId,
	textboxId1,
	textboxId2,
	linkbuttonClientId,
	linkbuttonUniqueId,
	url,
	errorMessageId) {
	$('#' + linkbuttonClientId).on('click', function () {
		if (textboxId2 == "") {
			checkZipCodeShortInputLengthAndExecPostback(
				$('#' + textboxId),
				linkbuttonUniqueId,
				url,
				errorMessageId)
		}
		else {
			checkZipCodeLengthAndExecPostback(
				$('#' + textboxId1),
				$('#' + textboxId2),
				linkbuttonUniqueId,
				url,
				errorMessageId)
		}
	});
}

// Textbox change search zip code
function textboxChangeSearchZipCodeInRepeater(
	textboxId,
	textboxId1,
	textboxId2,
	uniqueId1,
	uniqueId2,
	linkbuttonId,
	url,
	errorMessageId,
	type) {
  if (textboxId1 == "") {
		$('#' + textboxId).off('keyup.zip');
		$('#' + textboxId).on('keyup.zip', function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			$('#' + linkbuttonId).click();
		});
	}
  else {
		$('#' + textboxId2).off('keyup.zip');
		$('#' + textboxId2).on('keyup.zip', function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			checkZipCodeLengthAndExecPostback(
				$('#' + textboxId1),
				$('#' + textboxId2),
				uniqueId2,
				url,
				errorMessageId)
		});
	}
	multiAddrsearchTriggerType = type;
}

// Click search zip code
function clickSearchZipCodeInRepeater(
	textboxId,
	textboxId1,
	textboxId2,
	linkbuttonClientId,
	linkbuttonUniqueId,
	url,
	errorMessageId,
	type) {
	$('#' + linkbuttonClientId).off('click.zip');
	$('#' + linkbuttonClientId).on('click.zip', function () {
		if (textboxId1 == "") {
			checkZipCodeShortInputLengthAndExecPostback(
				$('#' + textboxId),
				linkbuttonUniqueId,
				url,
				errorMessageId)
		}
		else {
			checkZipCodeLengthAndExecPostback(
				$('#' + textboxId1),
				$('#' + textboxId2),
				linkbuttonUniqueId,
				url,
				errorMessageId)
		}

		var topValue = $("#" + linkbuttonClientId).offset().top + 24;
		var leftValue = $("#" + textboxId).offset().left;

		if (type == "shipping") {
		  topValue -= ($("#dvUserBox").length) ? $("#dvUserBox").offset().top : 0;
		  leftValue -= ($("#dvUserBox").length) ? $("#dvUserBox").offset().left : 0;
		}

		if (($("#search-result-layer").css != undefined)
			&& ($("#search-result-layer").position() != undefined)) {
		  $("#search-result-layer").css("top", topValue);
		  $("#search-result-layer").css("left", leftValue);
		}
		multiAddrsearchTriggerType = type;
	});
}

// Textbox change search zip code for smart phone
function textboxChangeSearchZipCodeForSp(
	textboxId,
	textboxId1,
	textboxId2,
	uniqueId,
	uniqueId2,
	url,
	errorMessageId) {
	if (textboxId2 == "") {
		$('#' + textboxId).keyup(function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			checkZipCodeShortInputLengthAndExecPostback(
				$('#' + textboxId),
				uniqueId,
				url,
				errorMessageId)
		});
	}
	else {
		$('#' + textboxId2).keyup(function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			checkZipCodeLengthAndExecPostback(
				$('#' + textboxId1),
				$('#' + textboxId2),
				uniqueId2,
				url,
				errorMessageId)
		});
	}
}

// Click search zip code for smart phone
function clickSearchZipCodeForSp(
	textboxId,
	textboxId1,
	textboxId2,
	linkbuttonClientId,
	linkbuttonUniqueId,
	url,
	errorMessageId,
	errorMessageNecessary,
	errorMessageLength) {
	$('#' + linkbuttonClientId).on('click', function () {
		if (textboxId2 == "") {
			checkZipCodeShortInputLengthAndExecPostbackForSp(
				$('#' + textboxId),
				linkbuttonUniqueId,
				url,
				errorMessageId,
				errorMessageNecessary,
				errorMessageLength)
		}
		else {
			checkZipCodeLengthAndExecPostbackForSp(
				$('#' + textboxId1),
				$('#' + textboxId2),
				linkbuttonUniqueId,
				url,
				errorMessageId,
				errorMessageNecessary,
				errorMessageLength)
		}
	});
}

// Textbox change search zip code in repeater for smart phone
function textboxChangeSearchZipCodeInRepeaterForSp(
	textboxId,
	textboxId1,
	textboxId2,
	uniqueId1,
	uniqueId2,
	linkbuttonId,
	url,
	errorMessageId,
	type) {
	if (textboxId2 == "") {
		$('#' + textboxId).off('keyup.zip');
		$('#' + textboxId).on('keyup.zip', function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			$('#' + linkbuttonId).click();
		});
	}
	else {
		$('#' + textboxId2).off('keyup.zip');
		$('#' + textboxId2).on('keyup.zip', function (e) {
			if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

			checkZipCodeLengthAndExecPostback(
				$('#' + textboxId1),
				$('#' + textboxId2),
				uniqueId2,
				url,
				errorMessageId)
		});
	}
}

// Click search zip code in repeater for smart phone
function clickSearchZipCodeInRepeaterForSp(
	textboxId,
	textboxId1,
	textboxId2,
	linkbuttonClientId,
	linkbuttonUniqueId,
	url,
	errorMessageId,
	triggerType,
	errorMessageNecessary,
	errorMessageLength) {
	$('#' + linkbuttonClientId).off('click.zip');
	$('#' + linkbuttonClientId).on('click.zip', function () {
		if (textboxId2 == "") {
			checkZipCodeShortInputLengthAndExecPostbackForSp(
				$('#' + textboxId),
				linkbuttonUniqueId,
				url,
				errorMessageId,
				errorMessageNecessary,
				errorMessageLength)
		}
		else {
			checkZipCodeLengthAndExecPostbackForSp(
				$('#' + textboxId1),
				$('#' + textboxId2),
				linkbuttonUniqueId,
				url,
				errorMessageId,
				errorMessageNecessary,
				errorMessageLength)
		}
		var topValue = $("#" + linkbuttonClientId).offset().top + 24;
		var leftValue = $("#" + textboxId).offset().left;

    if (($("#search-result-layer").css != undefined)
			&& ($("#search-result-layer").position() != undefined)) {
		  $("#search-result-layer").css("top", topValue);
		  $("#search-result-layer").css("left", leftValue);
		}
  });
}

// Scroll
function ScrollToTop() {
  $('html,body').animate({
    scrollTop: $("#orderDetailsInput").offset().top
  }, 0);
}

// Textbox change search global zip
function textboxChangeSearchGlobalZip(
	globalZipCodeId,
	eventTarget) {
	$('#' + globalZipCodeId).keyup(function (e) {
	if (isValidKeyCodeForKeyEvent(e.keyCode) == false) return;

	// Check length for global zipcode
	if (checkGlobalZipLength(globalZipCodeId) == false) return;
	__doPostBack(eventTarget, "");
	});
}

// Check global zipcode length
function checkGlobalZipLength(globalZipCodeId) {
	var globalZipCodeLength = $('#' + globalZipCodeId).val().length;
	var isValid = ((globalZipCodeLength >= 3)
		&& (globalZipCodeLength <= 20));
	return isValid;
}

//======================================================================================
// �h���b�v�_�E�����X�g�A���W�I�{�^�����X�g�l�̍Đݒ�(�u���E�U�o�b�N���ɏ����l�ɖ߂�̂�h�~�j
//======================================================================================

function UpdateDdlAndRbl(
	ddlOwnerBirthYearId,
	ddlOwnerBirthMonthId,
	ddlOwnerBirthDayId,
	rblOwnerSexId,
	ddlOwnerCountryId,
	selectedIndexYear,
	selectedIndexMonth,
	selectedIndexDay,
	selectedValueSex,
	selectedIndexCountry,
	ddlOwnerAddr1Id,
	selectedIndexOwnerAddr1) {

	const LOCAL_NAME = "input";

	var ddlOwnerBirthYear = document.getElementById(ddlOwnerBirthYearId);
	var ddlOwnerBirthMonth = document.getElementById(ddlOwnerBirthMonthId);
	var ddlOwnerBirthDay = document.getElementById(ddlOwnerBirthDayId);
	var rblOwnerSex = document.getElementById(rblOwnerSexId);
	var ddlOwnerCountry = document.getElementById(ddlOwnerCountryId);
	var ddlOwnerAddr1 = document.getElementById(ddlOwnerAddr1Id);

	if (ddlOwnerBirthYear != null) ddlOwnerBirthYear[selectedIndexYear].selected = true;
	if (ddlOwnerBirthMonth != null) ddlOwnerBirthMonth[selectedIndexMonth].selected = true;
	if (ddlOwnerBirthDay != null) ddlOwnerBirthDay[selectedIndexDay].selected = true;
	if (ddlOwnerCountry != null) ddlOwnerCountry[selectedIndexCountry].selected = true;
	if (ddlOwnerAddr1 != null) {
		ddlOwnerAddr1[selectedIndexOwnerAddr1].selected = true;
	}

	if (rblOwnerSex != null) {
		for (var elem of rblOwnerSex.childNodes) {
			if ((elem.localName == LOCAL_NAME) && (elem.value == selectedValueSex)) {
				elem.checked = true;
				break;
			}
		}
	}
}

// Set interval authentication message
var setIntervalId;
var timeCount = 0;
function setIntervalAuthenticationMessage(
	authenticationStatusId,
	authenticationMessageId,
	phoneNumber,
	authCodeDigits,
	expirationTime) {
	timeCount = expirationTime;
	clearInterval(setIntervalId);
	setIntervalId = setInterval(function () {
		timeCount--;
		var lbAuthenticationStatus = document.getElementById(authenticationStatusId);

		if (lbAuthenticationStatus == null) clearInterval(setIntervalId);

		var message = '�d�b�ԍ� '
			+ phoneNumber
			+ ' ���ɔF�؃R�[�h�𑗐M�������܂����B<br />'
			+ timeCount
			+ ' �b�ȓ���'
			+ authCodeDigits
			+ ' ���̔F�؃R�[�h����͂��Ă��������B';

		lbAuthenticationStatus.innerHTML = message;

		if (timeCount <= 0) {
			var lbAuthenticationMessage = document.getElementById(authenticationMessageId);
			lbAuthenticationMessage.style.color = "#FF0000";
			lbAuthenticationMessage.style.display = "block";
			lbAuthenticationMessage.innerHTML = '�F�؃R�[�h�̗L���������؂�Ă��܂��B�ēx�F�؃R�[�h���擾���Ă��������B'
			clearInterval(setIntervalId);
		}
	}, 1000);
}

// Check authentication code input
function checkAuthenticationCodeInput() {
	if ($('[id*=hfResetAuthenticationCode]') == undefined) return;
	if ($('[id*=tbAuthenticationCode]')[0].value === '') return;
	
	// Trigger event of button
	$('[id*=hfResetAuthenticationCode]')[0].value = '';
	$('[id*=lbCheckAuthenticationCode')[0].click();
};

// Reset authentication code
function resetAuthenticationCodeInput(customValidatorId) {
	if ($('[id*=hfResetAuthenticationCode]') == undefined) return;

	if (($('[id*=lbHasAuthentication]').length == 0)
		&& ($('[id*=lbAuthenticationStatus]')[0].innerHTML === '')) {
		return;
	}

	var customValidator = $('[id*=' + customValidatorId + ']');
	if (customValidator.length > 0) {
		if (customValidator[0].innerHTML !== '') return;
	}

	clearInterval(setIntervalId);

	// Trigger event of button
	$('[id*=hfResetAuthenticationCode]')[0].value = 'reset';
	$('[id*=lbCheckAuthenticationCode')[0].click();
}

// Check tel no
function checkTelNo(
	tbTel1Id,
	tbTel2Id,
	tbTel3Id,
	tbTelId,
	tbTelGlobalId) {
	var tbTel1Id = $('#' + tbTel1Id)[0];
	var tbTel2Id = $('#' + tbTel2Id)[0];
	var tbTel3Id = $('#' + tbTel3Id)[0];
	var telNo = $('#' + tbTelId)[0];
	var telNoGlobal = $('#' + tbTelGlobalId)[0];
	var telNoValue = '';

	// If there are 3 textboxes phone number
	if ((tbTel1Id !== undefined)
		&& (tbTel2Id !== undefined)
		&& (tbTel3Id !== undefined)) {
		telNoValue = tbTel1Id.value + tbTel2Id.value + tbTel3Id.value;
	}

	// If there are 1 textbox phone number
	if (telNo !== undefined) telNoValue = telNo.value;
	if (telNoGlobal !== undefined) telNoValue = telNoGlobal.value;

	var regex = /^\+?(^[0-9]{2,5}(-|�)[0-9]{1,4}(-|�)[0-9]{2,4}$)|^([0-9]{10,11}$)$/;
	if (telNoValue.match(regex)) return true;

	return false;
}

// Is number key
function isNumberKey(evt) {
	var charCode = evt.which
		? evt.which
		: evt.keyCode;
	if ((charCode > 31) && ((charCode < 48) || (charCode > 57))) return false;
	return true;
}

// �������{����}�̕�����u������
// ���p���@�F"{0}��{1}����".format("1","�}�C�i�X") �� "1���}�C�i�X����"
String.prototype.format = function () {
  var args = arguments;
  return this.replace(/{(\d+)}/g, function (match, number) {
    return typeof args[number] != "undefined" ? args[number] : match;
  });
}
