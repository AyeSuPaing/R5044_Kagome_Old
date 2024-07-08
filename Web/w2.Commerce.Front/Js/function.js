function initializeFunctionJs() {
	// ヘッダーグローバル
	$('#HeadGlobalNavi ul > li.onMenu').hover(
		function(){
		$(".HeadGNaviList:not(:animated)",this).slideDown("fast");
		$(this).addClass('active');
	},
	function(){
		$(".HeadGNaviList",this).slideUp("fast");
		$(this).removeClass('active');
	});

	//ヘッダーメニューマウスオーバー
	$('#HeadRight .hoverMenu').hover(
	function(){
		$(this).children('.menu').stop().slideDown("fast");
	},
	function(){
		$(this).children('.menu').stop().slideUp("fast");
	});

	// 新着ニュース
	$('#dvTopNews ul').jScrollPane({ mouseWheelSpeed: 50 });

	// サイドメニュー
	// UpdatePanel利用時にイベントの多重登録を防ぐ
	var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
	if (isAsyncPostback === false) {
		$(".categoryList .toggle").click(function() {
			$(this).next().slideToggle();
			$(this).toggleClass("active");
		});
	}

	// 商品一覧（ウィンドウショッピング）
  $(document).ready(function () {
    var $variation;
    $('.glbPlist').heightLine().biggerlink({ otherstriggermaster: false }).hover(function () {
      $('.glbPlist .variationview_wrap').hide();
			$variation = $(this).find('.variationview_wrap');
			if ($variation) $variation.show();
		}, function() {
			if ($variation) $variation.hide();
    });

    $('.variationview_bg').heightLine().biggerlink({ otherstriggermaster: false });
	});

	// 商品一覧（一覧）
	$('.plPhoto').hover(function() {
		$variation = $(this).find('.variationview_wrap');
		if ( $variation ) $variation.show();
	}, function() {
		if ( $variation ) $variation.hide();
	});;

	// お気に入りリスト
	$('.favoriteProductImage').hover(function() {
		$variation = $(this).find('.variationview_wrap');
		if ( $variation ) $variation.show();
	}, function() {
		if ( $variation ) $variation.hide();
	});;

	// 購入履歴一覧
  $('.orderBtr').biggerlink();

	// ページトップ
	$('.page-top a').click(function() {
		$('html, body').animate({scrollTop:0}, 'fast');
		return false;
	});
}
