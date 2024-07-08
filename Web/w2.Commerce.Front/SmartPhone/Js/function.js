function initializeFunctionJs(){

	// ヘッダーグローバルナビ
	$('.header-global-nav').on('click','a',function() {
		if ( !$(this).hasClass('active') ) {
			$('.header-toggle').hide();
			$("." + $(this).attr("id")).slideToggle();
			$('.header-global-nav a').removeClass('active');
			$(this).addClass("active");
		} else { 
			$('.header-toggle').hide();
			$('.header-global-nav a').removeClass('active');
		}
	});
	$('.header-toggle .close').click(function() {
		$('.header-toggle').hide();
		$('.header-global-nav a').removeClass('active');
		$('html, body').animate({scrollTop:0}, 'fast');
		return false;
	});
	$('.minicart .delete-product').click(function () {
		var prm = Sys.WebForms.PageRequestManager.getInstance();

		// 商品削除処理が終了後実行
		prm.add_endRequest(function () {
			$('.toggle-global-menu').show();
			$('li #toggle-global-menu').addClass("active");
		});
	});

	// 商品一覧：高さを揃える
	$(".product-list-3 li, .product-list-2 li").heightLine();

	// 商品一覧：ソート
	$(".sort-nav a").on("click", function() {
		if ( !$(this).hasClass('active') ) {
			$(".sort-toggle > div").hide();
			$("." + $(this).attr("id")).slideToggle();
			$(".sort-nav a").removeClass('active');
			$(this).addClass('active');
		} else { 
			$('.sort-toggle > div').hide();
			$('.sort-nav a').removeClass('active');
		}
	});

	$('.page-top a').click(function() {
		$('html, body').animate({scrollTop:0}, 'fast');
		return false;
	});
}