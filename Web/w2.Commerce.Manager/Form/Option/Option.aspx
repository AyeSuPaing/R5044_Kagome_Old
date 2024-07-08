<%--
=========================================================================================================
  Module      : オプション訴求ページ(Option.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="Option.aspx.cs" Inherits="Form_Option_Option" MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="w2.Common.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<!--▽ タイトル ▽-->
	<h1 class="page-title">拡張オプション</h1>
	<!--△ タイトル △-->
	<!--▽ 詳細 ▽-->
	<div class="menu-option">
		<%-- ピックアップオプション --%>
		<div class="option-slider">
			<div id="optionSlider" class="swiper option-slider__inner">
				<div class="option-slider__swiper-wrapper swiper-wrapper">
					<asp:Repeater ID="rSlider" runat="server" ItemType="OptionAppeal.OptionSlider">
						<ItemTemplate>
							<div class="option-slider__slide swiper-slide btn-open-modal" data-id="<%#: Item.OptionId %>">
								<a href="#" class="img__wrapper">
									<img id="Img1" runat="server" src="<%# Item.ImagePath %>" alt="option banner menu">
								</a>
							</div>
						</ItemTemplate>
					</asp:Repeater>
				</div>
				<!-- Add Pagination -->
				<div class="swiper-pagination"></div>
				<!-- Add Arrow -->
				<div class="swiper-button-prev"></div>
				<div class="swiper-button-next"></div>
			</div>
		</div>
		<%-- 人気オプション --%>
		<div class="option-popular">
			<h2 class="option-popular__ttl">人気オプション</h2>
			<div id="optionPopularSlider" class="swiper">
				<div class="swiper-wrapper option-popular__list">
					<asp:Repeater ID="rPopularOptionSlider" runat="server" ItemType="OptionAppeal.PopularOptionSlider">
						<ItemTemplate>
							<div class="option-popular__list-item swiper-slide btn-open-modal" data-id="<%#: Item.OptionId %>">
								<a href="#">
									<img id="Img2" runat="server" src="<%# Item.ImagePath %>" alt="popular option">
								</a>
							</div>
						</ItemTemplate>
					</asp:Repeater>
				</div>
				<!-- Add Arrow -->
				<div class="option-popular-arrow swiper-button-prev"></div>
				<div class="option-popular-arrow swiper-button-next"></div>
			</div>
		</div>

		<%-- 検索フォーム --%>
		<div class="option-narrow__input">
			<div class="option-narrow__select">
				<div class="option-narrow-display">&nbsp;</div>
				<ul id="displayOption" class="option-narrow-list">
					<li class="option-narrow-list__item selected" data-narrow="">
						<img class="narrow-icon" src="../../Images/w2Option/icon-all.svg" alt="all narrow">
						<p class="narrow-text-search">すべてのオプション分類</p>
					</li>
					<asp:Repeater ID="rNarrow" ItemType="OptionAppeal.OptionCategory" runat="server">
						<ItemTemplate>
							<li class="option-narrow-list__item" data-narrow="<%#: Item.CategoryId %>" runat="server">
								<img class='<%#: "narrow-icon-" + Item.CategoryParent.ToLower() %>' src="<%# Item.CategoryIcon %>" alt="user narrow">
								<p class="narrow-text-search"><%#: Item.CategoryName %></p>
							</li>
						</ItemTemplate>
					</asp:Repeater>
				</ul>
			</div>
			<div class="option-narrow__keyword keyword-search">
				<asp:TextBox class="searchOptionText" id="tbSearchCtategoryAndOption" runat="server" placeholder="キーワードを入力（例：ソーシャル連携　）" />
				<div class="search_btn_main">
					<a class="optionSearchButton">検索</a>
				</div>
			</div>
		</div>
		<div>
			<asp:Repeater ID="rOptionCategoryList" ItemType="OptionAppeal.OptionCategory" runat="server">
				<ItemTemplate>
					<div id="UserNarrow" itemid="<%#: Item.CategoryId %>" data-optioncount="<%#: Item.Options.Count %>" data-category="<%#: Item.CategoryId %>" class='<%#: "option-narrow__result option-" + Item.CategoryParent.ToLower() %>' runat="server" Visible="<%# Item.Options.Any()%>">
						<div class="option-narrow__result-ttl-wrap">
							<img class="option-narrow__result-ttl-icon" src="<%# Item.CategoryIcon %>">
							<h3 class="option-narrow__result-ttl"><%#: Item.CategoryName %></h3>
						</div>
						<ul class="option-narrow__result-list-wrap">
							<asp:Repeater ID="rOptionsByCategory" DataSource="<%# Item.Options %>" ItemType="OptionAppeal.OptionItem" runat="server">
								<ItemTemplate>
									<div id="Div1" class="options" runat="server" data-name="<%#: Item.OptionName %>" data-summary="<%#: Item.OptionSummary %>" data-detals="<%# Item.OptionDetals %>" data-categoryid="<%#: Item.CategoryId %>">
										<li class="option-narrow__result-list-item btn-open-modal" data-id="<%#: Item.OptionId %>" >
											<div class="option-narrow__result-list-item-wrap">
												<div class="option-narrow__result-list-item-inner">
													<div class="option-narrow__result-list-icon">
														<img src="<%# Item.OptionIconPath %>" alt="icon option result">
													</div>
													<div class="option-narrow__result-list-text">
														<h4 class="option-narrow__result-list-ttl"><%#: Item.OptionName %><span class="icon"></span></h4>
														<p class="option-narrow__result-list-detail"><%#: Item.OptionSummary %></p>
													</div>
												</div>
												<div class="option-narrow__result-list-price">
													<p><span class="price-type">初期：</span><%#: Item.OptionInitial %></p>
													<p><span class="price-type">月額：</span><%#: Item.OptionMonthly %></p>
												</div>
											</div>
										</li>
									</div>
								</ItemTemplate>
							</asp:Repeater>
						</ul>
					</div>
				</ItemTemplate>
			</asp:Repeater>
			<div id="notFound" class="not-found">
				<img class="icon-not-found" src="../../Images/w2Option/not-found.svg" alt="not found">
				<p class="option-narrow__result-not-found-text">該当するオプションが見つかりませんでした</p>
			</div>
		</div>
	</div>

	<div class="modal-content-hide">
		<!-- オプション訴求モーダル 入力 -->
		<div id="modal-menu-option-input">
			<div class="modal-option">
				<p class="modal-option-ttl">拡張オプションのご紹介</p>
				<div class="modal-option-inner">
					<div class="modal-option-name">
						<div class="modal-option-name-icon">
							<img id="option-icon-path" class="modal-option-name-icon" src="#" alt="option icon">
						</div>
						<p id="option-name" class="modal-option-name-label"></p>
					</div>
					<div class="modal-option-detail-price">
						<dl class="modal-option-detail-price-block">
							<dt class="modal-option-detail-price-name">初期：</dt>
							<dd class="modal-option-detail-price-value">
								<span id="option-initial" class="price-value-label"></span>
							</dd>
						</dl>
						<dl class="modal-option-detail-price-block">
							<dt class="modal-option-detail-price-name">月額：</dt>
							<dd class="modal-option-detail-price-value">
								<span id="option-monthly" class="price-value-label"></span>
							</dd>
						</dl>
						<dl class="modal-option-detail-price-block">
							<dt class="modal-option-detail-price-name">
								<span id="option-ani" class="price-value-label"></span>
							</dt>
						</dl>
					</div>
					<div class="modal-option-text">
						<p id="option-details" class="p"></p>
					</div>
					<div class="modal-option-detail">
						<p class="modal-option-detail-attention">この機能をご利用いただくにはお申込みが必要です。</p>
						<div class="modal-option-detail-btn">
							<div class="btn btn-contact btn-size-l">詳細をみる</div>
						</div>
					</div>
					<div class="modal-option-block">
						<div class="modal-option-block-header">お電話でのお問い合わせ</div>
						<div class="modal-option-block-content">
							<div class="modal-option-block-inquiry">
								<div class="modal-option-block-inquiry-tel">
									<div class="modal-option-icon-tel">
										<img src="../../Images/w2Option/icon-tel.svg" alt="icon-tel">
									</div>
									<span id="option-telnumber" class="modal-option-block-inquiry-tel-num"></span>
								</div>
								<p class="modal-option-block-inquiry-time">受付時間：<span id="option-inquiry-time" class="modal-option-block-inquiry-time"></span> ※土日祝は除く</p>
								<p class="modal-option-block-text">
									※ご契約状況の確認のために、ユーザーIDをお伺いいたします。<br>
									ご用意の上お電話いただくとスムーズにご案内可能です。
								</p>
							</div>
						</div>
					</div>
					<div class="modal-option-block modal-option-form">
						<div class="modal-option-block-header">
							<p>フォームからお問い合わせ</p>
							<p class="modal-option-block-text">フォームにご記入の上「送信」ボタンを押してください。</p>
						</div>
						<div class="modal-option-block-content">
							<table class="modal-option-block-table">
								<tr>
									<th>対象オプション</th>
									<td id="tbOptionName"></td>
								</tr>
								<tr>
									<th>氏名<span class="notice">*</span></th>
									<td>
										<asp:TextBox ID="tbUserName" runat="server" placeholder="田中 太郎" TextMode="SingleLine" />
									</td>
								</tr>
								<tr>
									<th>メールアドレス<span class="notice">*</span></th>
									<td>
										<asp:TextBox ID="tbUserEmail" runat="server" placeholder="yamada@sample.com" TextMode="SingleLine" />
									</td>
								</tr>
								<tr>
									<th>電話番号<span class="notice">*</span></th>
									<td>
										<asp:TextBox ID="tbUserTelNumber" runat="server" placeholder="000-0000-0000" TextMode="SingleLine" />
									</td>
								</tr>
								<tr>
									<th>お問い合わせ内容<span class="notice">*</span></th>
									<td>
										<ul class="modal-option-block-table-checkboxes">
											<li>
												<input id="inquiryMore" type="radio" name="inquiry_detail" value="inquiryMore" checked="checked">
												<label for="inquiryMore">詳しく聞きたい</label>
											</li>
											<li>
												<input id="inquiryNow" type="radio" name="inquiry_detail" value="inquiryNow">
												<label for="inquiryNow">今すぐ導入したい</label>
											</li>
											<li>
												<input id="inquiryOthers" type="radio" name="inquiry_detail" value="inquiryOthers">
												<label for="inquiryOthers">その他</label>
											</li>
										</ul>
										<asp:Textbox ID="tbInquiry" runat="server" textmode="MultiLine" placeholder="具体的な内容を入力してください" MaxLength="1000" />
									</td>
								</tr>
							</table>
							<div class="modal-error-box" style="display: none;">
								<div class="modal-error__list" id="divErrorMessageName" style="display: none;">
									<img class="modal-error__list-icon" src="../../Images/w2Option/icon-error.svg" alt="icon-error">
									<p>氏名を入力してください。</p>
								</div>
								<div class="modal-error__list" id="divErrorMessageMailAddress" style="display: none;">
									<img class="modal-error__list-icon" src="../../Images/w2Option/icon-error.svg" alt="icon-error">
									<p>メールアドレスを入力してください。</p>
								</div>
								<div class="modal-error__list" id="divErrorMessageInvalidMailAddress" style="display: none;">
									<img class="modal-error__list-icon" src="../../Images/w2Option/icon-error.svg" alt="icon-error">
									<p>正しいメールアドレスを入力してください。</p>
								</div>
								<div class="modal-error__list" id="divErrorMessageTelNumber" style="display: none;">
									<img class="modal-error__list-icon" src="../../Images/w2Option/icon-error.svg" alt="icon-error">
									<p>電話番号を入力してください。</p>
								</div>
								<div class="modal-error__list" id="divErrorMessageInvaidTelNumber" style="display: none;">
									<img class="modal-error__list-icon" src="../../Images/w2Option/icon-error.svg" alt="icon-error">
									<p>正しい電話番号を入力してください。</p>
								</div>
							</div>
							<div class="modal-option-block-btn">
								<div id="btnSendMail" class="btn btn-contact btn-size-l">送信</div>
							</div>
						</div>
					</div>
					<div class="modal-option-block modal-option-form-complete" style="display: none">
						<div class="modal-option-block-header">
							<p>フォームからお問い合わせ</p>
						</div>
						<div class="modal-option-block-content">
							<div class="modal-option-block-complete-icon-wrap">
								<img class="modal-option-block-complete-icon-image" src="../../Images/w2Option/icon-complete.svg" alt="complete">
							</div>
							<p class="modal-option-block-complete-message">
								お問い合わせありがとうございます。<br>
								後ほど担当者よりご連絡させていただきます。
							</p>
						</div>
					</div>
					<div class="modal-option-close-btn">
						<a href="javascript:void(0);" class="btn btn-contact-close btn-size-l js-modal-close-btn">
							<img class="icon-modal-close-btn" src="../../Images/w2Option/icon-modal-close-btn.svg" alt="">閉じる</a>
					</div>
				</div>
			</div>
		</div>

		<!-- オプション訴求モーダル 追加済み -->
		<div id="modal-menu-option-added">
			<div class="modal-option">
				<p class="modal-option-ttl">拡張オプションのご紹介</p>
				<div class="modal-option-inner modal-option-form-added">
					<div class="modal-option-block">
						<div class="modal-option-block-content">
							<div class="modal-option-block-added-icon-wrap">
								<img class="modal-option-block-added-icon-image" src="../../Images/w2Option/icon-complete.svg" alt="complete">
							</div>
							<p class="modal-option-block-added-message">
								既にご利用中オプションです
							</p>
						</div>
					</div>
					<div class="modal-option-name">
						<div class="modal-option-name-icon">
							<img id="option-icon-path-added" class="modal-option-name-icon" src="#" alt="option icon">
						</div>
						<p id="option-name-added" class="modal-option-name-label"></p>
					</div>
					<div class="modal-option-text">
						<p id="option-details-added" class="p"></p>
					</div>
					<div class="modal-option-detail">
						<div class="modal-option-detail-btn">
							<div class="btn btn-contact btn-size-l">詳細をみる</div>
						</div>
					</div>
					<div class="modal-option-close-btn">
						<a href="javascript:void(0);" class="btn btn-contact-close btn-size-l js-modal-close-btn">
							<img class="icon-modal-close-btn" src="../../Images/w2Option/icon-modal-close-btn.svg" alt="">閉じる</a>
					</div>
				</div>
			</div>
		</div>
	</div>

	<div id="footer" style="padding-bottom: 70px;"></div>
	<!--△ 詳細 △-->
	<link rel="stylesheet" href="../../Lib/swiper4.5.0/swiper.min.css">
	<script src="../../Lib/swiper4.5.0/swiper.min.js"></script>
	<script>
		// 拡張オプションスライダー
		var swiper1 = new Swiper("#optionSlider", {
			loop: true,
			slidesPerView: 'auto',//必須
			centeredSlides: true,//1枚目のスライド中央配置
			spaceBetween: 10,//スライド間の余白
			autoHeight: true,
			pagination: {
				el: ".swiper-pagination",
				clickable: true
			},
			navigation: {
				nextEl: ".swiper-button-next",
				prevEl: ".swiper-button-prev"
			}
		});

		// 人気オプションスライダー
		var optionPopularSlider;
		var optionPopularSliderOption;
		var optionPopularSliderItemEl = $('.option-popular__list-item').length;
		function optionPopularSliderFunc() {
			optionPopularSliderOption = {
				loop: true,
				slidesPerView: 4,//必須
				spaceBetween: 13,//スライド間の余白
				autoHeight: true,
				navigation: {
					nextEl: ".swiper-button-next",
					prevEl: ".swiper-button-prev"
				}
			};
			if (optionPopularSliderItemEl <= 4) {
				$('.option-popular-arrow').hide();
			}
			optionPopularSlider = new Swiper('#optionPopularSlider', optionPopularSliderOption);
		};
		optionPopularSliderFunc();
		// Narrow
		$('.option-narrow-display').html($('.option-narrow-list__item.selected').html());
		$('.option-narrow-display').click(function () {
			$(this).siblings('.option-narrow-list').slideToggle();
		});
		$('.option-narrow-list__item').click(function (item) {
			var selectedNarrow = $(this).data("narrow");
			if ($(this).closest('li').hasClass('selected')) {
				$(this).closest('li').removeClass('selected');
				$('.option-narrow-display').html($('.option-narrow-list__item:first-of-type').html());
				$('.option-narrow-list').slideUp();
			} else {
				$(this).closest('li').siblings('li').removeClass('selected');
				$(this).closest('li').addClass('selected');
				$('.option-narrow-display').html($(this).html());
				$('.option-narrow-list').slideUp();
			}
			$('.option-narrow__result').each(function () {
				var resultId = $(this).attr('itemid');
				if (resultId === selectedNarrow && !$(this).closest('div').hasClass('selected')) {
					$(this).closest('div').show();
				} else if (selectedNarrow === "") {
					$(this).closest('div').show();
				} else {
					$(this).closest('div').hide();
				}
			});

			var str = $('.searchOptionText')[0].value;
			var categoryVisible = {};

			$('.options').each(function () {
				var name = $(this).data('name');
				var summary = $(this).data('summary');
				var detals = $(this).data('detals');
				var categoryid = $(this).data('categoryid');

				if (name.indexOf(str) >= 0 || summary.indexOf(str) >= 0 || detals.indexOf(str) >= 0) {
					$(this).closest('div').show();
				} else if (str === "") {
					$(this).closest('div').show();
				} else {
					if (categoryVisible[categoryid] == null) categoryVisible[categoryid] = 0;
					categoryVisible[categoryid] += 1;
					$(this).closest('div').hide();
				}
			});

			var narow = $('.selected').data("narrow");

			$('.option-narrow__result').each(function () {
				var optionCount = $(this).data('optioncount');
				var resultId = $(this).data('category');
				var narowId = $(this).attr('itemid');

				if (categoryVisible[resultId] != null) {
					if (narow === "") {
						if (categoryVisible[resultId] === optionCount) {
							$(this).closest('div').hide();
						} else {
							$(this).closest('div').show();
						}
					} else {
						if (categoryVisible[resultId] === optionCount || narowId !== narow) {
							$(this).closest('div').hide();
						} else {
							$(this).closest('div').show();
						}
					}
				}
			});

			SearchResult();
		});

		// 検索機能
		$('.optionSearchButton').click(function (item) {
			SearchKeyword();
			SearchResult();
		});

		$('.searchOptionText').keypress(function (item) {
			if (item.keyCode === 13) {
				SearchKeyword();
			}
			SearchResult();
		});

		function SearchKeyword() {
			var str = $('.searchOptionText')[0].value;
			var categoryVisible = {};

			$('.options').each(function () {
				var name = $(this).data('name');
				var summary = $(this).data('summary');
				var detals = $(this).data('detals');
				var categoryid = $(this).data('categoryid');

				if (name.indexOf(str) >= 0 || summary.indexOf(str) >= 0 || detals.indexOf(str) >= 0) {
					$(this).closest('div').show();
				} else if (str === "") {
					$(this).closest('div').show();
				} else {
					if (categoryVisible[categoryid] == null) categoryVisible[categoryid] = 0;
					categoryVisible[categoryid] += 1;
					$(this).closest('div').hide();
				}
			});

			var narow = $('.selected').data("narrow");

			$('.option-narrow__result').each(function () {
				var optionCount = $(this).data('optioncount');
				var resultId = $(this).data('category');
				var narowId = $(this).attr('itemid');

				if (categoryVisible[resultId] != null) {
					if (narow === "") {
						if (categoryVisible[resultId] === optionCount) {
							$(this).closest('div').hide();
						} else {
							$(this).closest('div').show();
						}
					} else {
						if (categoryVisible[resultId] === optionCount || narowId !== narow) {
							$(this).closest('div').hide();
						} else {
							$(this).closest('div').show();
						}
					}
				} else {
					if (narow === "") {
						$(this).closest('div').show();
					} else {
						if (narowId !== narow) {
							$(this).closest('div').hide();
						} else {
							$(this).closest('div').show();
						}
					}
				}
			});
		}

		function SearchResult() {
			var result = true;
			$('.option-narrow__result').each(function() {
				var style = $(this).attr('style');
				if (style === null) return false;
				if (style !== "display: none;") {
					result = false;
				}
			});

			if (result) {
				$('#notFound').show();
			} else {
				$('#notFound').hide();
			}
		}

		// アコーディオン
		$('.option-narrow__result-ttl-wrap').click(function () {
			if ($(this).siblings('.option-narrow__result-list-wrap').not(':animated').length >= 1) {
				$(this).toggleClass('close');
				$(this).closest('.option-narrow__result').toggleClass('close');
				$(this).siblings('.option-narrow__result-list-wrap').slideToggle();
			}
		});

		// モーダルを開く
		$('.btn-open-modal').each(function () {
			$(this).on('click', function () {
				var optionId = $(this).data('id');
				var url = "<%: Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPTION_APPEAL %>" + "/SetOptionItem";
				var sendData = JSON.stringify({ optionId: optionId });
				$.ajax({
					type: "POST",
					url: url,
					contentType: "application/json; charset=utf-8",
					data: sendData,
					dataType: "json",
					success: function (response) {
						if ((response == null)
							|| (response.d == undefined)) return;

						var data = JSON.parse(response.d);
						if (data.length === 0) return;
						var option = data.Option;
						var inquiry = data.Inquiry;

						var type = option.OptionEnable;
						if (type === false) {
							modal.open('#modal-menu-option-input', 'modal-size-m');

							$("#option-icon-path").attr("src", option.OptionIconPath);
							$("#option-icon-path").attr("class",
								"modal-option-name-icon option-" + option.ParentCategoryId.toLowerCase());
							$("#option-name").html(option.OptionName +
								'<span id="category-name" class="option-category-name">' +
								option.CategoryName +
								'</span>');
							$("#option-initial").html(option.OptionInitial);
							$("#option-monthly").html(option.OptionMonthly);
							$("#option-ani").html(option.OptionAncillaryInformation);
							$("#option-details").html(option.OptionDetals);
							$(".modal-option-detail-btn").attr("data", option.OptionSupportSiteUrl);
							$("#tbOptionName").html(option.OptionName);
							$("#option-telnumber").html(inquiry.TelNumber);
							$("#option-inquiry-time").html(inquiry.ReceptionTime);
						} else {
							modal.open('#modal-menu-option-added', 'modal-size-m');

							$("#option-icon-path-added").attr("src", option.OptionIconPath);
							$("#option-icon-path-added").attr("class",
								"modal-option-name-icon option-" + option.ParentCategoryId.toLowerCase());
							$("#option-name-added").html(option.OptionName +
								'<span id="category-name-added" class="option-category-name">' +
								option.CategoryName +
								'</span>');
							$("#option-details-added").html(option.OptionDetals);
							$(".modal-option-detail-btn").attr("data", option.OptionSupportSiteUrl);
						}

						var userName = getCookie('<%: COOKIE_KEY_OPTIONAPPEAL_USER_NAME %>');
						var userEmail = getCookie('<%: COOKIE_KEY_OPTIONAPPEAL_USER_EMAIL %>');
						var userTelNumber = getCookie('<%: COOKIE_KEY_OPTIONAPPEAL_USER_TEL_NUMBER %>');

						$("#<%= tbUserName.ClientID %>").val((userName == null) ? "<%: this.LoginOperatorName %>" : userName);
						$("#<%= tbUserEmail.ClientID %>").val(userEmail);
						$("#<%= tbUserTelNumber.ClientID %>").val(userTelNumber);
						if ($(".modal-option-detail-btn").attr("data").length === 0) {
							$(".modal-option-detail-btn").hide();
						}
						else {
							$(".modal-option-detail-btn").show();
						}
					},
					complete: function () {
						$(".modal-option-form").show();
						$(".modal-option-form-complete").hide();
						$(".modal-error-box").hide();
						$("input:radio[name='inquiry_detail']").prop('checked', false);
						$("input:radio[name='inquiry_detail'][value='inquiryMore']").prop('checked', true);
						$("#<%= tbInquiry.ClientID %>").val("");
					}
				});
			});
		});

		//問い合わせフォーム入力チェック
		function CheckInput() {
			var hasName = $("#<%= tbUserName.ClientID %>").val();
			if (hasName.length === 0) {
				$("#divErrorMessageName").show();
			} else {
				$("#divErrorMessageName").hide();
			}

			var hasEmail = $("#<%= tbUserEmail.ClientID %>").val();
			var isEmail = false;
			if (hasEmail.length === 0) {
				$("#divErrorMessageMailAddress").show();
			} else {
				$("#divErrorMessageMailAddress").hide();

				var emailRegex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
				isEmail = emailRegex.test(hasEmail);

				if (!isEmail) {
					$("#divErrorMessageInvalidMailAddress").show();
				} else {
					$("#divErrorMessageInvalidMailAddress").hide();
				}
			}

			var hasTelNumber = $("#<%= tbUserTelNumber.ClientID %>").val();
			var isTelNumber = false;
			if (hasTelNumber.length === 0) {
				$("#divErrorMessageTelNumber").show();
			} else {
				$("#divErrorMessageTelNumber").hide();

				var telRegex1 = /\d{2,4}-\d{2,4}-\d{4}/;
				var telRegex2 = /^(0{1}\d{9,10})$/;
				isTelNumber = telRegex1.test(hasTelNumber) || telRegex2.test(hasTelNumber);

				if (!isTelNumber) {
					$("#divErrorMessageInvaidTelNumber").show();
				} else {
					$("#divErrorMessageInvaidTelNumber").hide();
				}
			}

			if ((hasName.length === 0) ||
				(hasEmail.length === 0) ||
				(hasTelNumber.length === 0) ||
				!isEmail ||
				!isTelNumber) {
				$(".modal-error-box").show();
				return false;
			} else {
				$(".modal-error-box").hide();
				return true;
			}
		}
		$("#btnSendMail").on("click", function () {
			if (!CheckInput()) return;
			url = "<%: Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPTION_APPEAL %>" + "/SendMail";
			var inquiryType = $("input[type=radio][name=inquiry_detail]:checked").val();
			sendData = JSON.stringify({
				name: $("#<%= tbUserName.ClientID %>").val(),
				optionName: $("#tbOptionName").html(),
				mailAddress: $("#<%= tbUserEmail.ClientID %>").val(),
				telNumber: $("#<%= tbUserTelNumber.ClientID %>").val(),
				inquiryTypeName: $('label[for="' + inquiryType + '"]').text(),
				content: $("#<%= tbInquiry.ClientID %>").val()
			});

			setCookie('<%: COOKIE_KEY_OPTIONAPPEAL_USER_NAME %>', $("#<%= tbUserName.ClientID %>").val());
			setCookie('<%: COOKIE_KEY_OPTIONAPPEAL_USER_EMAIL %>', $("#<%= tbUserEmail.ClientID %>").val());
			setCookie('<%: COOKIE_KEY_OPTIONAPPEAL_USER_TEL_NUMBER %>', $("#<%= tbUserTelNumber.ClientID %>").val());

			$.ajax({
				type: "POST",
				url: url,
				contentType: "application/json; charset=utf-8",
				data: sendData,
				dataType: "json",
				success: function (response) {
					$(".modal-option-form").hide();
					$(".modal-option-form-complete").show();
				}
			});
		});

		$(".modal-option-detail-btn").on("click", function () {
			window.open($(this).attr("data"));
		});

	</script>
</asp:Content>
