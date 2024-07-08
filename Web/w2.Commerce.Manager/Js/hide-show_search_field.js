// 受注一覧などの結果表示時に検索フォームを非表示にしたい機能
// ユーザー管理画面検索時の表示／非表示
$(document).ready(function () {
	console.log("--- click ---");
	$("#user-hide-search-field-slide-toggle").click(function () {
		$(
			"#hide-field_SiteName,#hide-field_CompanyName,#hide-field_CountryIsoCode,#hide-field_FixedPurchaseMemberFlg,#hide-field_UserDateChanged,#hide-field_UserDateCreated,#hide-field_UserExtendSection,#hide-field_UserIntegrationFlg,#hide-field_UserManagementSection,#hide-field_UpUserMemo,#hide-field_Zip_Addr_UserEasyRegisterFlag,#hide-field_Tel_MailFlg_DelFlg")
			.slideToggle("fast");
		var self = $("#check-toggle-text-user");

		if (self.text() === "閉じる") {
			self.text("全ての検索項目を表示");
			// closeをSessionStorageにセット
			sessionStorage.userListSearchMenu = "close";
		} else {
			$("#check-toggle-text-user").text("閉じる");
			$(".search_btn_bg").attr("rowspan", 13);

			// openをSessionStorageにセット
			sessionStorage.userListSearchMenu = "open";
		}
		console.log(sessionStorage);
		$("#check-toggle-open").toggleClass('is-toggle-open');
	});

	// 読み込み時にopenなら「全て表示」ボタンをクリックしたことにする
	console.log("---ready---");
	console.log(sessionStorage.getItem("userListSearchMenu"));
	if (sessionStorage.getItem("userListSearchMenu") === "open") {
		$("#user-hide-search-field-slide-toggle").trigger("click");
	}


	// 「全て表示」のままページ遷移をすると、検索一覧のところに飛ぶ。
	var url = new URL(location.href);
	var params = url.searchParams;
	var pNumber = params.get("pno");
	var result;
	var resultPosition;

	if ((sessionStorage.getItem("userListSearchMenu") === "open") && (!pNumber == "")) {
		result = document.getElementById("userListSearchResult");
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	} else {
		$("html,body").scrollTop(0);
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	}
});

// 受注情報検索時の表示／非表示
$(document).ready(function () {
	$("#order-hide-search-field-slide-toggle").click(function () {
		console.log("--- click ---");
		$(
			"#hide-field_OwnerTelAddrZip,\r\n#hide-field_PaymentStatusOwnerKbn,.hide-field_OrderExtendStatus,\r\n#hide-field_OrderExtendUpdate,\r\n#hide-field_OrderExternalPayment,\r\n#hide-field_PaymentAuthDate,\r\n#hide-field_OrderShippingReservedStatus,\r\n#hide-field_ReturnsExchangeInfo,\r\n#hide-field_ReturnExchangeChecked,\r\n#hide-field_ReturnExchangeStatus,\r\n#hide-field_ReturnExchangeDate,\r\n#hide-field_MallEnabled,\r\n#hide-field_MallOptionEnabled,\r\n#hide-field_MemberRankEnabled,\r\n#hide-field_NoveltyEnabled,\r\n#hide-field_ContentsOptionEnabled,\r\n#hide-field_FixedPurchaseOptionEnabled,\r\n#hide-field_OrderCountSubscribeCount,\r\n#hide-field_OrderExtendOptionEnabled,\r\n#hide-field_OrderExtendOptionEnabled,\r\n#hide-field_ManagementMemo,\r\n#hide-field_ShippingMemo,\r\n#hide-field_PaymentMemo,\r\n#hide-field_RelationMemo,\r\n#hide-field_UserMemo,\r\n#hide-field_ProductOption,\r\n#hide-field_AdsCode,\r\n#hide-field_CompanyName,\r\n#hide-field_UserManagementLevel,\r\n#hide-field_ShippingSection,\r\n#hide-field_ShippingStatus,\r\n#hide-field_SearchShippingAddr,\r\n#hide-field_DisplayInvoice,\r\n#hide-field_UseLeadTime,\r\n#hide-field_ShippingDate,\r\n#hide-field_ShippingCountry,\r\n#hide-field_ShippingInfo,\r\n#hide-field_ReceiptOption,\r\n#hide-field_ProductBundleOption,\r\n#hide-field_CouponInfo,\r\n#hide-field_PaymentOrderPurchaseInfo,\r\n#hide-field_TwInvoiceOption,\r\n#hide-field_OrderMemo,\r\n#hide-field_StorePickupStatusOption")
			.slideToggle("fast");
		var self = $("#check-toggle-text-order");
		if (self.text() === "閉じる") {
			self.text("全ての検索項目を表示");
			// closeをSessionStorageにセット
			sessionStorage.orderListSearchMenu = "close";
		} else {
			$("#check-toggle-text-order").text("閉じる");
			$(".search_btn_bg").attr("rowspan", 45);

			// openをSessionStorageにセット
			sessionStorage.orderListSearchMenu = "open";
		}
		console.log(sessionStorage);
		$("#check-toggle-open").toggleClass('is-toggle-open');
	});

	// 読み込み時にopenなら「全て表示」ボタンをクリックしたことにする
	console.log("---ready---");
	console.log(sessionStorage.getItem("orderListSearchMenu"));
	if (sessionStorage.getItem("orderListSearchMenu") === "open") {
		$("#order-hide-search-field-slide-toggle").trigger("click");
	}

	// 「全て表示」のままページ遷移をすると、検索一覧のところに飛ぶ。
	var url = new URL(location.href);
	var params = url.searchParams;
	var pNumber = params.get("pno");
	var result;
	var resultPosition;

	if ((sessionStorage.getItem("orderListSearchMenu") === "open") && (!pNumber == "")) {
		result = document.getElementById("orderListSearchResult");
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	} else {
		$("html,body").scrollTop(0);
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	}
});

// 定期台帳検索時の表示／非表示
$(document).ready(function () {
	$("#fp-hide-search-field-slide-toggle").click(function () {
		console.log("--- click ---");
		$(
			".hide-field_KakuExtendStatus,#hide-field_OrderShippedCountFrom,#hide-field_SubscriptionBox,#hide-field_OrderKbnPaymentStatus,#hide-field_FixedKbnProductId,#hide-field_ShippingInfos,#hide-field_fManagementMemo,#hide-field_fShippingMemo,#hide-field_UpdateDateExtendStatus,#hide-field_ReceiptFlg,#hide-field_OrderExtendName")
			.slideToggle("fast");
		var self = $("#check-toggle-text-fp");
		if (self.text() === "閉じる") {
			self.text("全ての検索項目を表示");
			// closeをSessionStorageにセット
			sessionStorage.fixedPurchaseSearchMenu = "close";
		} else {
			$("#check-toggle-text-fp").text("閉じる");
			$(".search_btn_bg").attr("rowspan", 14);

			// openをSessionStorageにセット
			sessionStorage.fixedPurchaseSearchMenu = "open";
		}
		console.log(sessionStorage);
		$("#check-toggle-open").toggleClass('is-toggle-open');
	});

	// 読み込み時にopenなら「全て表示」ボタンをクリックしたことにする
	console.log("---ready---");
	console.log(sessionStorage.getItem("fixedPurchaseSearchMenu"));
	if (sessionStorage.getItem("fixedPurchaseSearchMenu") === "open") {
		$("#fp-hide-search-field-slide-toggle").trigger("click");
	}

	// 「全て表示」のままページ遷移をすると、検索一覧のところに飛ぶ。
	var url = new URL(location.href);
	var params = url.searchParams;
	var pNumber = params.get("pno");
	var resultPosition;
	var result;

	if ((sessionStorage.getItem("fixedPurchaseSearchMenu") === "open") && (!pNumber == "")) {
		result = document.getElementById("fixedPurchaseListSearchResult");
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	} else {
		$("html,body").scrollTop(0);
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	}
});

// 商品情報画面検索時の表示／非表示
$(document).ready(function () {
	$("#product-hide-search-field-slide-toggle").click(function () {
		console.log("--- click ---");
		$(
			"#hide-field_SupplierCoop,#hide-field_CooperationIDs,#hide-field_ShippingSizeKbnType,#hide-field_DisplayKbnSell,#hide-field_Aicon,#hide-field_BuyableMemberRank,#hide-field_DisplayPrioLimitedPayment,#hide-field_ProductBundleItemType,#hide-field_Colour,#hide-field_ProductTaxCategory,#hide-field_pSubscriptionBox,#hide-field_ProductSellFromDate,#hide-field_ProductSellToDate,#hide-field_DisplayBuyableFixedPurchase")
			.slideToggle("fast");
		var self = $("#check-toggle-text-product");
		if (self.text() === "閉じる") {
			self.text("全ての検索項目を表示");
			// closeをSessionStorageにセット
			sessionStorage.productListSearchMenu = "close";
		} else {
			$("#check-toggle-text-product").text("閉じる");
			$(".search_btn_bg").attr("rowspan", 16);

			// openをSessionStorageにセット
			sessionStorage.productListSearchMenu = "open";
		}
		console.log(sessionStorage);
		$("#check-toggle-open").toggleClass('is-toggle-open');
	});

	// 読み込み時にopenなら「全て表示」ボタンをクリックしたことにする
	console.log("---ready---");
	console.log(sessionStorage.getItem("productListSearchMenu"));
	if (sessionStorage.getItem("productListSearchMenu") === "open") {
		$("#product-hide-search-field-slide-toggle").trigger("click");
	}

	// 「全て表示」のままページ遷移をすると、検索一覧のところに飛ぶ。
	var url = new URL(location.href);
	var params = url.searchParams;
	var pNumber = params.get("pno");
	var resultPosition;
	var result;

	if ((sessionStorage.getItem("productListSearchMenu") === "open") && (!pNumber == "")) {
		result = document.getElementById("productListSearchResult");
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	} else {
		$("html,body").scrollTop(0);
		resultPosition = result.getBoundingClientRect();
		scrollTo(0, resultPosition.top);
	}
});
