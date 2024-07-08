var cmn = {
	"cookie_sidemenu_expires": 7,
	"cookie_sidemenu_key": "w2c_sidemenu",
	"cookie_sidemenu_value": "",
	"toggle_slim_spped": 300,
	"toggle_slim_class": "sidemenu_slim",
	"sidemenu_slim_height": 0,
	"st": 0,
	"wh": 0,
	"ww": 0
}

$(function () {

	$('.content').css({
		'opacity': 1
	})

	$(window).resize(function () {
		cmn.wh = window.innerHeight;
		cmn.ww = window.innerWidth;
	});

	var header = $('#header');
	//スクロールが200に達したらscroll付与
	var header_toggle = function () {
		if ($(window).scrollTop() > 1) {
			$("body").addClass('scroll');
		} else {
			$("body").removeClass('scroll');
		}
	};

	header_toggle();

	// ヘルプアイコン追加
	var title = $('.page-title').text();
	if (title != "") {
		$(".main-contents-inner-support").append('<div class="btn_help"><a href="' + 'http://v5.supportsite.w2solution.com/?s=' + encodeURI(title + '&al=on&rd=on') + '" data-popover-message="「' + title + '」について" target="_blank"><span class="icon-help"></span></a></div>');
	}

	//左メニューカレント表示
	$('.sidemenu-child-list .sidemenu-list a').each(function () {
		if ($(this).text() == title) {
			$(this).closest('.sidemenu-list').addClass('current');
		}
	});

	// ページトップへ戻るボタン追加
	$(".main-contents-inner").append('<div class="btn_pagetop"><a href="javascript:void(0)"></a></div>');
	$(".btn_pagetop").click(function () {
		$('body,html').animate({
			scrollTop: 0
		}, 500);
	});




	var toggle_spped = 300;
	var toggle_open_class = "open";


	// btn sidemenu toggle

	var sidemenu_restore = function () {
		var cookie_sidemenu_data = $.cookie(cmn.cookie_sidemenu_key);
		if (cookie_sidemenu_data) {
			$(cookie_sidemenu_data.split('')).each(function (i) {

				if (i == 0) {
					if ($(this)[0][0] == "1") {
						$('body').addClass("animation_none").addClass(cmn.toggle_slim_class);
						setTimeout(function () {
							$('body').removeClass("animation_none");
						}, 300);
					}
				} else {
					if ($(this)[0][0] == "0") {
						$('.sidemenu > .sidemenu-list:eq(' + (i - 1) + ')').removeClass(toggle_open_class).find('.sidemenu-child-list').slideUp(0);
					} else {
						$('.sidemenu > .sidemenu-list:eq(' + (i - 1) + ')').addClass(toggle_open_class).find('.sidemenu-child-list').slideDown(0);
					}
				}
			});
		}
	};
	sidemenu_restore();

	var sidemenu_save_state = function () {
		//状態をcookieに保存
		cmn.cookie_sidemenu_value = "";
		if ($('body').hasClass(cmn.toggle_slim_class) == false) {
			cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "0";
		} else {
			cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "1";
		}
		$('.sidemenu > .sidemenu-list').each(function (i) {
			if ($(this).hasClass(toggle_open_class) == false) {
				cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "0";
			} else {
				cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "1";
			}
		});
		$.cookie(cmn.cookie_sidemenu_key, cmn.cookie_sidemenu_value, { path: root_path, expires: cmn.cookie_sidemenu_expires });

	};

	$('.sidemenu > .sidemenu-list').each(function () {
		var this_ = $(this);
		var tgt = this_.find('.sidemenu-child-list');
		$(this).find('> .sidemenu-list-label > a:eq(0)').click(function () {
			if (!$('.sidemenu_slim').size()) {
				if (this_.hasClass(toggle_open_class)) {
					//close
					this_.removeClass(toggle_open_class);
					tgt.slideUp(toggle_spped);
					sidemenu_toggle_status_check();
				} else {
					//open
					this_.addClass(toggle_open_class);
					tgt.slideDown(toggle_spped);
					sidemenu_toggle_status_check();
				}
			}
		});
	});

	// btn sidemenu all toggle
	var classname_sidemenu_all_open = "sidemenu_all_open";
	var sidemenu_toggle_status_check = function () {
		var status = true; // all open
		$('.sidemenu > .sidemenu-list').each(function (i) {
			var this_ = $(this);
			//this_.addClass("sidemenu-list-"+i);
			if (this_.hasClass(toggle_open_class) == false) {
				status = false;
			}
		});
		if (status) {
			$('body').addClass(classname_sidemenu_all_open);
		} else {
			$('body').removeClass(classname_sidemenu_all_open);
		}

		sidemenu_save_state();
	};

	$('.btn-alltoggle-open').click(function () {
		$('.sidemenu-child-list').each(function () {
			$(this).slideDown(toggle_spped);
			$(this).closest('.sidemenu-list').addClass(toggle_open_class);
		});
		$('body').addClass(classname_sidemenu_all_open);
		sidemenu_save_state();
	});
	$('.btn-alltoggle-close').click(function () {
		$('.sidemenu-child-list').each(function () {
			$(this).slideUp(toggle_spped);
			$(this).closest('.sidemenu-list').removeClass(toggle_open_class);
		});
		$('body').removeClass(classname_sidemenu_all_open);
		sidemenu_save_state();
	});


	//btn sidemenu slim toggle_sppedbtn-slimtoggle-close

	$('.btn-slimtoggle-on').click(function () {
		$('body').addClass(cmn.toggle_slim_class);
		sidemenu_save_state();

		//検索結果table幅セット
		setTimeout(function () {
			data_table_width_fit();
		}, 600);

	});

	$('.btn-slimtoggle-off').click(function () {
		$('body').removeClass(cmn.toggle_slim_class);
		sidemenu_save_state();
		$('.sidemenu').css({
			"position": "",
			"top": ""
		});

		//検索結果table幅セット
		setTimeout(function () {
			data_table_width_fit();
		}, 600);

	});

	// sidemenu slim popmenu
	$('.sidemenu > .sidemenu-list').each(function (i) {
		var timer;
		$(this).hover(function (e) {
			if ($('.sidemenu_slim').size()) {
				var tgt = $(this).find('.sidemenu-child-list');
				tgt.addClass('popmenu-open');

				var html = tgt.html();
				var width_ = tgt.outerWidth();
				var height_ = tgt.outerHeight();
				var top = $(this).offset().top;
				cmn.wh = $(window).height();
				cmn.st = $(window).scrollTop();
				console.log(cmn.st, cmn.wh, top, height_);
				console.log(cmn.st + cmn.wh - top - height_);

				if ((cmn.st + cmn.wh - top - height_) < 0) {
					console.log("隠れあり");
					tgt.css({
						"top": (top - cmn.st) - (top + height_ - cmn.st - cmn.wh) + "px"
					});
				} else {
					tgt.css({
						"top": (top - cmn.st) + "px"
					});
				}
				clearTimeout(timer);
			}
		}, function () {
			var tgt = $(this).find('.sidemenu-child-list');
			tgt.removeClass('popmenu-open');
			tgt.css({
				"top": ""
			});
		});

	});

	var sm = $(".sidemenu");

	var sm_scroll_function = function () {
		cmn.st = $(this).scrollTop();
		if ($(".sidemenu_slim").size()) {
			cmn.wh = $(window).height();
			cmn.sidemenu_slim_height = $('.sidemenu').outerHeight() + $(".tabs-packages").outerHeight() + $("#header").outerHeight();
			if (cmn.wh < cmn.sidemenu_slim_height) {
				//console.log("sidemenuのすべてがwindowに収まっていない");
				if ((cmn.sidemenu_slim_height - cmn.st) > cmn.wh) {
					//console.log("スクロール量がsidemenuの高さを超えていない");
					sm.css({
						"position": "relative",
						"top": 0,
					});
				} else {
					//console.log("スクロール量がsidemenuの高さを超えた");
					sm.css({
						"position": "fixed",
						"top": (cmn.wh - $('.sidemenu').outerHeight()) + "px",
					});
				}
			} else {
				//console.log("sidemenuのすべてがwindowに収まっている");
				sm.css({
					"position": "fixed",
					"top": "",
					"bottom": ""
				});
			}
		}
	};


	//popover
	var popover_wait_time = 0;
	var popover_class = 'show';
	$('[data-popover-message]').each(function (i) {
		var this_ = $(this);
		var content = this_.data('popover-message');
		var popover_timer;
		this_.hover(function (e) {
			var width = this_.outerWidth();
			var height = this_.outerHeight();
			var left = this_.offset().left;
			var top = this_.offset().top;
			var margin = 10;
			var ww = $(window).width();

			if (!$('.popover_' + i).size()) {
				$('body').append('<div class="popover popover_' + i + '">' + content + '</div>');
				var pop_w = $('.popover_' + i).outerWidth();
				var pop_h = $('.popover_' + i).outerHeight();
				if ((ww - left) / ww > 0.5) {
					//right
					$('.popover_' + i).css({
						"left": (left + width + margin) + 'px',
						"top": (top + (height / 2)) + "px"
					});
					$('.popover_' + i).addClass('right');
				} else {
					//left
					$('.popover_' + i).css({
						"left": (left - pop_w - margin) + 'px',
						"top": (top + (height / 2)) + "px"
					});
					$('.popover_' + i).addClass('left');
				}
			}
			popover_timer = setTimeout(function () {
				$('.popover_' + i).addClass(popover_class);
			}, popover_wait_time);
		}, function () {
			clearTimeout(popover_timer);
			$('.popover_' + i).removeClass(popover_class);
			$('.popover_' + i).remove();
		});
	});

	var scroll_functions = function () {
		header_toggle();
		sm_scroll_function();
	};

	//scrollイベント負荷低減
	var w_scroll = false;
	var event_interval_time = 50;
	$(window).scroll(function () {
		if (!w_scroll) {
			w_scroll = true;
			setTimeout(function () {
				w_scroll = false;
				scroll_functions();
			}, event_interval_time);
		}
	});

	set_disabled_class();
});

var set_disabled_class = function () {
	//disabledフォーム要素のlabelにclass付与
	$("label.disabled").each(function () {
		var tgt_label = $(this);
		var for_ = tgt_label.attr("for");
		$("#" + for_).each(function () {
			if (!$(this).is(':disabled')) {
				tgt_label.removeClass("disabled");
			}
		});
	});
	$("[disabled]").each(function () {
		var id = $(this).attr("id");
		$("label[for='" + id + "']").addClass("disabled");
	});
}

var data_table_width_fit = function () {
	console.log("data_table_width_fit");
	$('.list_box_bg').each(function () {
		//reset
		$(this).find('.div_header_left, .div_data_left, .div_header_right').css({
			"width": ""
		});

		var w = $(this).find('.div_data_left').width();
		if ($(this).find('.div_header_left').width() > w) {
			w = $(this).find('.div_header_left').width();
		}
		w += 10;
		$(this).find('.div_header_left , .div_data_left').css({
			"width": w + "px"
		});
		$(this).find('.div_data_right').css({
			"left": w + "px"
		});
		$(this).find(".div_header_right").width($(this).find('.div_data_right').width());
	});
};

//検索結果 table幅調整
$(window).load(function () {
	cmn.wh = window.innerHeight;
	cmn.ww = window.innerWidth;

	var now_ww = cmn.ww;
	setInterval(function () {
		if (now_ww != cmn.ww) {
			now_ww = cmn.ww;
			data_table_width_fit();
		}
	}, 1500);
	data_table_width_fit();

});

var modal = {
	"btn_selector": ".btn-modal-open",
	"modal_selector": "modal-selector",
	"ini": function () {
		$(modal.btn_selector).each(function () {
			$(this).click(function () {
				var selector = $(this).data(modal.modal_selector);
				modal.open(selector);
			});
		});
	},
	"open": function (selector) {
		var content_ = $(selector).html();
		$("body").append('<div class="modal"><div class="modal-bg" onclick="modal.close();"></div><div class="modal-btn-close" onclick="modal.close();"></div><div class="modal-content-wrapper">' + content_ + '</div></div>');
		setTimeout(function () {
			$(".modal").addClass('show');
		}, 1);
	},
	"close": function () {
		$(".modal").removeClass("show");
		setTimeout(function () {
			$(".modal").remove();
		}, 300);
	}
};
$(function () {
	modal.ini();
});
