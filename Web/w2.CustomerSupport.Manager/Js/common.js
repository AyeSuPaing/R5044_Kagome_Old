var cmn = {
	"cookie_sidemenu_expires": 7,
	"cookie_sidemenu_key": "w2cs_sidemenu",
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
	//�X�N���[����200�ɒB������scroll�t�^
	var header_toggle = function () {
		if ($(window).scrollTop() > 1) {
			$("body").addClass('scroll');
		} else {
			$("body").removeClass('scroll');
		}
	};

	header_toggle();

	var title = $('.top_title').text().trim();

	//�����j���[�J�����g�\��
	$('.sidemenu-child-list .sidemenu-list a').each(function () {
		if ($(this).text() == title) {
			$(this).closest('.sidemenu-list').addClass('current');
		}
	});

	// �y�[�W�g�b�v�֖߂�{�^���ǉ�
	$(".main-contents-inner").append('<div class="btn_pagetop"><a href="javascript:void(0)"></a></div>');
	$(".btn_pagetop").click(function () {
		$('body,html').animate({
			scrollTop: 0
		}, 500);
	});




	var toggle_spped = 300;
	var toggle_open_class = "open";


	// btn sidemenu toggle

	var classname_sidemenu_all_open = "sidemenu_all_open";
	var sidemenu_restore = function () {
		var cookie_sidemenu_data = getCookie(cmn.cookie_sidemenu_key);
		var allmenuIsOpen = true;
		var isAnimationNone = false;
		if (cookie_sidemenu_data) {
			$(cookie_sidemenu_data.split('')).each(function (i) {

				if (i == 0) {
					if ($(this)[0][0] == "1") {
						$('body').addClass("animation_none").addClass(cmn.toggle_slim_class);
						setTimeout(function () {
							$('body').removeClass("animation_none");
						}, 300);
						isAnimationNone = true;
					}
				} else {
					if ($(this)[0][0] == "0") {
						$('.sidemenu > .sidemenu-list:eq(' + (i - 1) + ')').removeClass(toggle_open_class).find('.sidemenu-child-list').slideUp(0);
						allmenuIsOpen = false;
					} else {
						$('.sidemenu > .sidemenu-list:eq(' + (i - 1) + ')').addClass(toggle_open_class).find('.sidemenu-child-list').slideDown(0);
					}
				}
			});
		}

		if (isAnimationNone || allmenuIsOpen) {
			$('body').addClass(classname_sidemenu_all_open);
		} else {
			$('body').removeClass(classname_sidemenu_all_open);
		}
	};
	sidemenu_restore();

	var sidemenu_save_state = function () {
		//��Ԃ�cookie�ɕۑ�
		cmn.cookie_sidemenu_value = "";
		if ($('body').hasClass(cmn.toggle_slim_class) == false) {
			cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "0";
		} else {
			cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "1";
		}
		$('.sidemenu .simplebar-content > .sidemenu-list').each(function (i) {
			if ($(this).hasClass(toggle_open_class) == false) {
				cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "0";
			} else {
				cmn.cookie_sidemenu_value = cmn.cookie_sidemenu_value + "1";
			}
		});
		setCookie(cmn.cookie_sidemenu_key, cmn.cookie_sidemenu_value, { path: root_path, expires: cmn.cookie_sidemenu_expires });

	};

	$('.sidemenu > .sidemenu-list').each(function () {
		var this_ = $(this);
		var tgt = this_.find('.sidemenu-child-list');
		$(this).find('> .sidemenu-list-label > a:eq(0)').click(function () {
			if (!$('.sidemenu_slim').length) {
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

	//�J�e�S���[�A�R�[�f�B�I���J��
	$('.sidemenu-list-grandson-lv1').each(function (i) {
		if (!$(this).prev().hasClass('sidemenu-list-grandson-lv1')) {
			$(this).prev().append('<span class="btn-toggle-grandson btn-toggle-grandson-lv1"></span>');
		}
	});

	$('.sidemenu-list-grandson-lv2').each(function (i) {
		if (!$(this).prev().hasClass('sidemenu-list-grandson-lv2')) {
			$(this).prev().append('<span class="btn-toggle-grandson btn-toggle-grandson-lv2"></span>');
		}
	});

	$('.sidemenu-list-grandson-lv3').each(function (i) {
		if (!$(this).prev().hasClass('sidemenu-list-grandson-lv3')) {
			$(this).prev().append('<span class="btn-toggle-grandson btn-toggle-grandson-lv3"></span>');
		}
	});
	var grandson_open_speed = 300;
	$('.btn-toggle-grandson-lv1').each(function () {
		$(this).click(function () {
			$(this).toggleClass('grandson-open');
			$(this).closest('.sidemenu-list').nextAll().each(function () {
				if ($(this).hasClass('sidemenu-list-grandson-lv1')) {
					$(this).slideToggle(grandson_open_speed);
				} else {
					return false;
				}
			});
		});
	});

	$('.btn-toggle-grandson-lv2').each(function () {
		$(this).click(function () {
			$(this).toggleClass('grandson-open');
			$(this).closest('.sidemenu-list').nextAll().each(function () {
				if ($(this).hasClass('sidemenu-list-grandson-lv2')) {
					$(this).slideToggle(grandson_open_speed);
				} else {
					return false;
				}
			});
		});
	});

	$('.btn-toggle-grandson-lv3').each(function () {
		$(this).click(function () {
			$(this).toggleClass('grandson-open');
			$(this).closest('.sidemenu-list').nextAll().each(function () {
				if ($(this).hasClass('sidemenu-list-grandson-lv3')) {
					$(this).slideToggle(grandson_open_speed);
				} else {
					return false;
				}
			});
		});
	});


	// btn sidemenu all toggle
	var sidemenu_toggle_status_check = function () {
		var status = true; // all open
		$('.sidemenu .simplebar-content > .sidemenu-list').each(function (i) {
			var this_ = $(this);
			//this_.addClass("sidemenu-list-"+i);
			if (this_.hasClass(toggle_open_class) == false) {
				status = false;
			}
		});
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

		//��������table���Z�b�g
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

		//��������table���Z�b�g
		setTimeout(function () {
			data_table_width_fit();
		}, 600);

	});

	// sidemenu slim popmenu
	$('.sidemenu > .sidemenu-list').each(function (i) {
		var timer;
		$(this).hover(function (e) {
			if ($('.sidemenu_slim').length) {
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
					console.log("�B�ꂠ��");
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
		if ($(".sidemenu_slim").length) {
			cmn.wh = $(window).height();
			cmn.sidemenu_slim_height = $('.sidemenu').outerHeight() + $(".tabs-packages").outerHeight() + $("#header").outerHeight();
			if (cmn.wh < cmn.sidemenu_slim_height) {
				//console.log("sidemenu�̂��ׂĂ�window�Ɏ��܂��Ă��Ȃ�");
				if ((cmn.sidemenu_slim_height - cmn.st) > cmn.wh) {
					//console.log("�X�N���[���ʂ�sidemenu�̍����𒴂��Ă��Ȃ�");
					sm.css({
						"position": "relative",
						"top": 0,
					});
				} else {
					//console.log("�X�N���[���ʂ�sidemenu�̍����𒴂���");
					sm.css({
						"position": "fixed",
						"top": (cmn.wh - $('.sidemenu').outerHeight()) + "px",
					});
				}
			} else {
				//console.log("sidemenu�̂��ׂĂ�window�Ɏ��܂��Ă���");
				sm.css({
					"position": "fixed",
					"top": "",
					"bottom": ""
				});
			}
		}
	};
	var scroll_functions = function () {
		header_toggle();
		sm_scroll_function();
	};

	//scroll�C�x���g���גጸ
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
	set_popover();
});

//popover
var set_popover = function () {
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

			if (!$('.popover_' + i).length) {
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
			},
				popover_wait_time);
		},
			function () {
				clearTimeout(popover_timer);
				$('.popover_' + i).removeClass(popover_class);
				$('.popover_' + i).remove();
			});
	});
}

var set_disabled_class = function () {
	//disabled�t�H�[���v�f��label��class�t�^
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

//�������� table������
$(window).bind('load', function () {
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

/*------------------------------------------------
* ���ʃ��[�_��
------------------------------------------------*/
var modal = {
	tgt_selector: '',
	tgt_selector_last: [],
	tgt_classname: '',
	open_class_name: 'is-open',
	flg_open: false,
	flg_before_modal: false,
	selector_modal: '.js-modal',
	tgt_btn: null,
	st: null,
	timer: null,
	callback_opened_function: null,
	ini: function (isUseUpdatePanel) {

		if ((isUseUpdatePanel === undefined) || (isUseUpdatePanel === false)) {
			jQuery('body').append(
				'<div class="modal js-modal" style="display: none;">' +
				'  <div class="modal-bg js-modal-close-btn"></div>' +
				'  <div class="modal-content-wrapper">' +
				'    <div class="js-modal-close-btn modal-header-close-btn"><span class="icon icon-close"></span></div>' +
				'    <div class="modal-content">' +
				'      <div class="modal-inner"></div>' +
				'    </div>' +
				'  </div>' +
				'</div>');
		}

		//btn open trigger
		jQuery('.btn-modal-open').each(function () {
			jQuery(this).unbind('click').bind('click', function () {
				modal.open(jQuery(this).data('modal-selector'), jQuery(this).data('modal-classname'), jQuery(this).data('modal-callback-opened'));
			});
		});

		//btn close trigger
		jQuery('.js-modal-close-btn').each(function () {
			jQuery(this).unbind('click').bind('click', function () {
				modal.close();
			});
		});
	},
	open: function (selector, classname, openedfunc) {

		//�w�i���Œ肷��
		var body_width = jQuery('body').width();
		jQuery('body').css({
			'overflow': 'hidden',
			'max-width': body_width,
			'margin-right': window.innerWidth - document.body.clientWidth
		});
		jQuery(window).resize(function () {
			jQuery('body').css({
				'max-width': ''
			});
		});

		//modal.tgt_btn = jQuery(this);
		var time_delay = 0;
		if (modal.flg_open) {
			//���[�_�����J���Ă�����
			//��U����A�j���[�V�������s
			jQuery(modal.selector_modal).removeClass(modal.open_class_name);
			time_delay = 500;

			//���ɊJ���Ă��郂�[�_������������true��
			modal.flg_before_modal = true;

		} else {

			//���ɊJ���Ă��郂�[�_�����Ȃ�������false��
			modal.flg_before_modal = false;

			//���[�_�����J������ԂŃ��[�_�����J���ꍇ�ɁA���O�̃��[�_���̃Z���N�^�[���i�[���Ă���
			modal.tgt_selector_last = [selector, classname, openedfunc];

		}


		setTimeout(function () {

			//�J�X�^���N���X���Z�b�g
			jQuery(modal.selector_modal).removeClass(modal.tgt_classname);
			jQuery(modal.selector_modal).addClass(classname);

			modal.tgt_classname = classname;

			//���O�̃Z���N�^�ƈႤ�ꍇ
			var flg_tgt_selector_diff = false; //�����ꍇ��false �A�Ⴄ�ꍇ��true
			if (modal.tgt_selector != selector) {
				flg_tgt_selector_diff = true;
			}

			//���Ɋi�[����Ă���DOM�ʒu�����ɖ߂�
			modal.return_content();

			//�V�����Z���N�^�[�̊i�[
			modal.tgt_selector = selector;

			//����DOM�ʒu��ێ����邽�߂̉�DOM
			jQuery(modal.tgt_selector).before('<div class="js-modal-content-original-position"></div>');

			//DOM�����[�_���ɒǉ�
			jQuery(modal.selector_modal + ' .modal-inner').append(jQuery(modal.tgt_selector));
			jQuery(modal.selector_modal).show();

			//���O�̃Z���N�^�ƈႤ�ꍇ
			if (flg_tgt_selector_diff) {

				//callback function ���s
				if (openedfunc) {
					modal.callback_opened_function = openedfunc;
					eval(openedfunc);
				}

			} else {

				//���O�̃��[�_���ƈႤcallback function���w�肳�ꂽ����s
				if (modal.callback_opened_function != openedfunc) {
					//callback function ���s
					if (openedfunc) {
						modal.callback_opened_function = openedfunc;
						eval(openedfunc);
					}
				}

			}

			//���[�_����width�Aheight���Z�b�g
			var modal_height = jQuery(modal.selector_modal + ' .modal-inner').height();

			if (jQuery(window).height() < modal_height) {
				jQuery(modal.selector_modal + ' .modal-content-wrapper').css({
					'max-height': modal_height + 'px'
				});
			}

			//���[�_�����X�N���[���ʒu���Z�b�g
			jQuery(modal.selector_modal).scrollTop(0);

			//modal show
			jQuery(modal.selector_modal).addClass(modal.open_class_name);

			modal.flg_open = true;

			//���[�_�����J�X�^���X�N���[���o�[�\��
			// new SimpleBar(jQuery(modal.selector_modal + ' .modal-content')[0]);

		}, time_delay);


	},
	close: function () {

		//�w�i�Œ���J������
		jQuery('body').css({
			'overflow': '',
			'max-width': '',
			'margin-right': ''
		});

		jQuery(modal.selector_modal).removeClass(modal.open_class_name);

		jQuery(modal.selector_modal).hide();

		setTimeout(function () {
			//���Ɋi�[����Ă���DOM�ʒu�����ɖ߂�
			modal.return_content();
			//modal.tgt_selector = "";
			modal.flg_open = false;
		}, 500);

		//�����w�����
		jQuery(modal.selector_modal + ' .modal-content-wrapper').css({
			'height': ''
		});

		//���[�_������J���ꂽ���[�_���̏ꍇ�͒��O�ɊJ���Ă������[�_�����ēx�J��
		setTimeout(function () {
			if (modal.flg_before_modal) {
				modal.open(modal.tgt_selector_last[0], modal.tgt_selector_last[1], modal.tgt_selector_last[2]);
				modal.flg_before_modal = false;
			}

		}, 500);
	},
	return_content: function () {
		//DOM�ʒu�����ɖ߂�
		jQuery('.js-modal-content-original-position').before(jQuery(modal.tgt_selector));
		jQuery('.js-modal-content-original-position').remove();
	}
}

/*------------------------------------------------
* �J�X�^���X�N���[���o�[
------------------------------------------------*/
var custom_scroll = {
	ini: function () {
		if ($('.custom-scroll-area').length != 0) {
			$('.custom-scroll-area').each(function () {
				var simplebar = new SimpleBar($(this)[0]);
				var simplebar_elem = simplebar.getScrollElement();
				$(simplebar_elem).off('scroll').on('scroll', function () {
					var st = $(this).scrollTop();
					if (st > 0) {
						$(simplebar_elem).closest('.custom-scroll-area').addClass('simplebar-is-scroll');
					} else {
						$(simplebar_elem).closest('.custom-scroll-area').removeClass('simplebar-is-scroll');
					}
				});

			});
		}

		if ($('.sidemenu').length != 0) {
			var simplebar_sidemenu = new SimpleBar($('.sidemenu')[0]);
		}

		if ($('.main-content-detail-body').length != 0) {
			var simplebar_main_content_detail_body = new SimpleBar($('.main-content-detail-body')[0]);
			var simplebar_main_content_detail_body_elem = simplebar_main_content_detail_body.getScrollElement();
			$(simplebar_main_content_detail_body_elem).off('scroll').on('scroll', function () {
				var st = $(this).scrollTop();
				if (st > 0) {
					$(simplebar_main_content_detail_body_elem).closest('.main-content-detail-body').addClass('simplebar-is-scroll');
				} else {
					$(simplebar_main_content_detail_body_elem).closest('.main-content-detail-body').removeClass('simplebar-is-scroll');
				}
			});
		}

		if ($('.layout-edit-basic-layout-select').length != 0) {
			$('.layout-edit-basic-layout-select').each(function () {
				new SimpleBar($(this)[0]);
			});
		}

		// if ($('.layout-edit-parts-list').length != 0) {
		//   $('.layout-edit-parts-list').each(function(){
		//     new SimpleBar($(this)[0]);
		//   });
		// }

	}
}
$(function () {
	custom_scroll.ini();
});

$(function () {
	modal.ini();
});

/*------------------------------------------------
* ���t�I���J�����_�[
------------------------------------------------*/
var input_datepicker = {
	inputDatepickerSelector: '.input-datepicker',
	inputTimepickerSelector: '.input-timepicker',
	errorMessageContainerSelector: '.error-message-datepicker-container',
	ini: function () {
		_this = this;
		var errorMessageContainers = $(_this.errorMessageContainerSelector);
		$(_this.inputDatepickerSelector).each(function () {
			$(_this.inputDatepickerSelector).datepicker({
				dateFormat: "yy/mm/dd",
				closeText: "����",
				prevText: "&#x3C;�O",
				nextText: "��&#x3E;",
				currentText: "����",
				monthNames: ["1��", "2��", "3��", "4��", "5��", "6��", "7��", "8��", "9��", "10��", "11��", "12��"],
				monthNamesShort: ["1��", "2��", "3��", "4��", "5��", "6��", "7��", "8��", "9��", "10��", "11��", "12��"],
				dayNames: ["���j��", "���j��", "�Ηj��", "���j��", "�ؗj��", "���j��", "�y�j��"],
				dayNamesShort: ["��", "��", "��", "��", "��", "��", "�y"],
				dayNamesMin: ["��", "��", "��", "��", "��", "��", "�y"],
				weekHeader: "�T",
				isRTL: false,
				showMonthAfterYear: true,
				yearSuffix: "�N",
				firstDay: 0, // �T�̏��߂͌��j
				showButtonPanel: true, // "����"�{�^��, "����"�{�^����\������
				beforeShow: function (input, inst) {
					errorMessageContainers.hide();
					var calendar = inst.dpDiv;
					setTimeout(function () {
						calendar.position({
							my: 'left bottom',
							at: 'left top',
							of: input
						});
					}, 1);
				}
			});
		});

		$(_this.inputTimepickerSelector).each(function () {
			var input = $(this);
			var dataValue = input.data('input-hidden-selector');
			var isStartTime = ((dataValue !== undefined) && (dataValue.lastIndexOf('start-time') !== -1));

			input.unbind('focus').bind('focus', function () {
				errorMessageContainers.hide();
				var h = input.outerHeight();
				var t = input.offset().top + h + 1;
				var l = input.offset().left;

				if (isStartTime) {
					$('body').append('<div class="timepicker" style="position:absolute;top:' + t + 'px;left:' + l + 'px;"><ul><li>00:00</li><li>01:00</li><li>02:00</li><li>03:00</li><li>04:00</li><li>05:00</li></ul><ul><li>06:00</li><li>07:00</li><li>08:00</li><li>09:00</li><li>10:00</li><li>11:00</li></ul><ul><li>12:00</li><li>13:00</li><li>14:00</li><li>15:00</li><li>16:00</li><li>17:00</li></ul><ul><li>18:00</li><li>19:00</li><li>20:00</li><li>21:00</li><li>22:00</li><li>23:00</li></ul></div>');
				} else {
					$('body').append('<div class="timepicker" style="position:absolute;top:' + t + 'px;left:' + l + 'px;"><ul><li>00:00</li><li>01:00</li><li>02:00</li><li>03:00</li><li>04:00</li><li>05:00</li></ul><ul><li>06:00</li><li>07:00</li><li>08:00</li><li>09:00</li><li>10:00</li><li>11:00</li></ul><ul><li>12:00</li><li>13:00</li><li>14:00</li><li>15:00</li><li>16:00</li><li>17:00</li></ul><ul><li>18:00</li><li>19:00</li><li>20:00</li><li>21:00</li><li>22:00</li><li>23:00</li></ul><ul><li>23:59</li></ul></div>');
				}

				$('.timepicker li').unbind('click').bind('click', function () {
					var text = ($(this).text() == '23:59') ? $(this).text() + ':59' : $(this).text() + ':00';
					input.val(text);
					$('.timepicker').remove();

					if ($(input).hasClass("update-time-input-search")) {
						$.ajax({
							type: "POST",
							url: "List",
							data: $('#search_form').serialize(),
						}).done(function (viewHTML) {
							$("#list-content").html(viewHTML);
						});
					}
				});
			});
			input.unbind('blur').bind('blur', function () {
				setTimeout(function () {
					$('.timepicker').remove();
				}, 300);
			});
		});
	}
}
$(function () {
	input_datepicker.ini();
});

/*------------------------------------------------
* ���ԑI��
------------------------------------------------*/
var selectPeriod = {
	wrapperSelector: '.select-period',
	errorMessageContainer: '.error-message-datepicker-container',
	startWrapperSelector: '.select-period-start',
	endWrapperSelector: '.select-period-end',
	startClearSelector: '.select-period-start-clear',
	endClearSelector: '.select-period-end-clear',
	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);
			var startWrapper = wrapper.find(_this.startWrapperSelector);
			var endWrapper = wrapper.find(_this.endWrapperSelector);
			var startClear = wrapper.find(_this.startClearSelector);
			var endClear = wrapper.find(_this.endClearSelector);

			// �N���A�{�^��
			$(".select-period-start-clear").click(function () {
				startWrapper.find('input').val('');
				startWrapper.find(_this.errorMessageContainer).hide();
			});
			$(".select-period-end-clear").click(function () {
				endWrapper.find('input').val('');
				endWrapper.find(_this.errorMessageContainer).hide();
			});
		});
	}
}
$(function () {
	selectPeriod.ini();
});

/*------------------------------------------------
* ���ԑI�����[�_��
------------------------------------------------*/
var selectPeriodModal = {
	wrapperSelector: '.js-select-period-modal',
	submitBtnSelector: '.select-period-modal-submit',
	errorMessageContainerSelector: '.error-message-datepicker-container',
	startDateInputSelector: '.select-period-start-input-date input',
	startTimeInputSelector: '.select-period-start-input-time input',
	endDateInputSelector: '.select-period-end-input-date input',
	endTimeInputSelector: '.select-period-end-input-time input',
	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);
			var submitBtn = wrapper.find(_this.submitBtnSelector);
			var startDateInput = wrapper.find(_this.startDateInputSelector);
			var startTimeInput = wrapper.find(_this.startTimeInputSelector);
			var endDateInput = wrapper.find(_this.endDateInputSelector);
			var endTimeInput = wrapper.find(_this.endTimeInputSelector);
			var errorMessageContainers = wrapper.find(_this.errorMessageContainerSelector);

			// ���Ƀf�[�^���Z�b�g����Ă���ꍇ�̏���
			var valStartDate = $(startDateInput.data('input-hidden-selector')).val();
			var valStartTime = $(startTimeInput.data('input-hidden-selector')).val();
			var valEndDate = $(endDateInput.data('input-hidden-selector')).val();
			var valEndTime = $(endTimeInput.data('input-hidden-selector')).val();

			startDateInput.val(valStartDate);
			startTimeInput.val(valStartTime);
			endDateInput.val(valEndDate);
			endTimeInput.val(valEndTime);

			if ((valStartDate + valStartTime + valEndDate + valEndTime) != '') {
				$(submitBtn.data('value-text-selector')).text(startDateInput.val() + ' ' + startTimeInput.val() + ' �` ' + endDateInput.val() + ' ' + endTimeInput.val() + '');
			}

			$(".select-period-modal-submit").click(function () {
				// console.log('submit');
				// �e���͏���hidden�ɔ��f
				// console.log(startDateInput.data('input-hidden-selector'));
				var format = new DateFormat('yyyy/MM/dd HH:mm:ss');

				var startDateTimeInput = startDateInput.val() + ' ' + startTimeInput.val();
				var startDateTime = format.parse(startDateTimeInput);
				var endDateTimeInput = endDateInput.val() + ' ' + endTimeInput.val();
				var endDateTime = format.parse(endDateTimeInput);

				var hasError = false;
				if ((startDateTimeInput.trim() !== '') && (startDateTime == null)) {
					$(errorMessageContainers[0]).show();
					return;
				}

				if ((endDateTimeInput.trim() !== '') && (endDateTime == null)) {
					$(errorMessageContainers[1]).show();
					return;
				}

				if ((startDateTime == null) && (endDateTime == null)) {
					errorMessageContainers.show();
					return;
				}

				$(startDateInput.data('input-hidden-selector')).val(startDateInput.val());
				$(startTimeInput.data('input-hidden-selector')).val(startTimeInput.val());
				$(endDateInput.data('input-hidden-selector')).val(endDateInput.val());
				$(endTimeInput.data('input-hidden-selector')).val(endTimeInput.val());
				$($(this).data('value-text-selector')).text(startDateInput.val() + ' ' + startTimeInput.val() + ' �` ' + endDateInput.val() + ' ' + endTimeInput.val() + '');
				modal.close();
			});
		});
	}
}
$(function () {
	selectPeriodModal.ini();
});

function ddlSelect() {
	if (Sys.WebForms == null) return;
	$('.select2-select').select2({
		placeholder: '�I�����Ă�������',
		allowClear: true,
	});
	// HACK:�B���v�f��width�����������邱�ƂŃf�U�C�������h�~���Ă��܂��B
	$('select.select2-offscreen').width('0px');
	$.extend($.fn.select2.defaults, {
		formatNoMatches: function () { return "�Y���f�[�^������܂���"; },
		formatSearching: function () { return "�������c"; }
	});
}
