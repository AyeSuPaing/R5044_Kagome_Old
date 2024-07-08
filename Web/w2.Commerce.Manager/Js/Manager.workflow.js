/*------------------------------------------------
* ���[�N�t���[���s��� ���[�N�t���[�I������
------------------------------------------------*/
var workflow = {
	'ini': function () {

	},
	// �X�e�b�v�\���؂�ւ�
	step: {
		current: 1,
		switch: function (step) {
			var selected_classname = 'is-selected';
			var current_classname = 'is-current';
			var loading_classname = 'is-loading';
			$('.order-workflow-list-block').each(function (i) {
				var index = i + 1;
				if (index < step) {
					$('.order-workflow-list-block' + index).addClass(selected_classname);
					$('.order-workflow-list-block' + index).removeClass(current_classname);
				}
				if (index == step) {
					$('.order-workflow-list-block' + index).removeClass(selected_classname);
					$('.order-workflow-list-block' + index).addClass(current_classname);
					$('.order-workflow-list-block' + index).addClass(loading_classname);
				}
				if (index > step) {
					$('.order-workflow-list-block' + index).removeClass(selected_classname);
					$('.order-workflow-list-block' + index).removeClass(current_classname);
				}
			});

			// ���݂̃X�e�b�v���i�[
			workflow.step.current = step;

			// �ʃX�e�b�v���� 1
			if (workflow.step.current == 1) {
				// �ꗗ�\���`����߂�
				workflow_view.switch($(workflow_view.radio + ':checked').val());
				$(workflow.select.wrapper).removeClass(workflow.select.select_classname);
			}

			// �ʃX�e�b�v���� 2
			if (workflow.step.current == 2) {
				// �ꗗ�\���`����list�ɕύX
				workflow_view.switch('list-list');
			}

			// �ʃX�e�b�v���� 3
			if (workflow.step.current == 3) {
				// ���o���X�N���[��
				var st = $('.order-workflow-list-block' + step).offset().top - 100;
				$('html, body').animate({ scrollTop: st }, 500);
			}

		},
		show: function (step) {
			// loading�\���I��
			var loading_classname = 'is-loading';
			var loaded_classname = 'is-loaded';
			$('.order-workflow-list-block' + step).removeClass(loading_classname);
			$('.order-workflow-list-block' + step).addClass(loaded_classname);

		}
	},
	// ���[�N�t���[�I���֘A
	select: {
		'wrapper': '.order-workflow-list-block1 .order-workflow-list-block-inner',
		'item': '.order-workflow-list-block-list-item',
		'btn': '.order-workflow-list-block-list-item-btn',
		'btn_clear': '.order-workflow-list-block-list-item-btn-selected',
		'select_classname': 'is-selected',
		ini: function () {

			// ���Z�b�g����
			workflow.list_table.ini_flg = false;
			// �e�[�u�����o���Œ菈������
			FixedMidashi.remove();

			$(workflow.select.wrapper).each(function () {
				var wrapper = $(this);
				wrapper.find(workflow.select.item).click(function () {
					// WF�I��
					if (workflow.step.current == 1) {
						var item = $(this);
						if (!item.hasClass(workflow.select.select_classname)) {
							wrapper.addClass(workflow.select.select_classname);
							wrapper.find(workflow.select.item).removeClass(workflow.select.select_classname);
							item.addClass(workflow.select.select_classname);
						}
						workflow.step.switch(2);
					};
					return false;
				});
				wrapper.find(workflow.select.btn_clear).click(function () {
					// WF�I������
					if (workflow.step.current > 1) {
						wrapper.find(workflow.select.item).each(function () {
							if ($(this).hasClass(workflow.select.select_classname)) {
								var item = $(this);
								item.removeClass(workflow.select.select_classname);
							}
						});

						workflow.step.switch(1);
					}
					return false;
				});
			});
		}
	},
	// ���[�N�t���[���s�֘A
	list_table: {
		// �ꗗ�`���e�[�u���\������
		html_data_wrapper: null,
		html_data_search: null,
		html_table_wrapper: null,
		html_table_bottom: null,
		html_submit_num: null,
		table_height: null,
		ini_flg: false,
		hitcount: 0,
		select_mode_all: false,
		table_type: null,
		ini: function () {
			// workflow.list_table.ini_flg = false;
			workflow.list_table.table_type = null;
			// �^�C�v����
			if ($('.order-workflow-list-block-data-table').length) {
				// ���X�g�`���̏ꍇ
				workflow.list_table.table_type = 'list';
			} else if ('.order-workflow-list-block-data-cassette-wrapper') {
				// �J�Z�b�g�`���̏ꍇ
				workflow.list_table.table_type = 'cassette';
			}

			workflow.list_table.html_data_wrapper = $('.order-workflow-list-block-data');
			workflow.list_table.html_data_search = $('.order-workflow-list-block-data-search');
			workflow.list_table.html_table_wrapper = $('.order-workflow-list-block-data-table-wrapper');
			workflow.list_table.html_table_bottom = $('.order-workflow-list-block-data-bottom');
			workflow.list_table.html_submit_num = $('.order-workflow-list-block-data-submit-num-value-select');
			workflow.list_table.html_submit_num_all = $('.order-workflow-list-block-data-submit-num-value-all');
			workflow.list_table.message.obj = $('.order-workflow-list-block-data-message');
			workflow.list_table.html_submit_select_btn = $('.order-workflow-list-block-data-submit-btn-select');
			workflow.list_table.html_submit_select_date = $('.order-workflow-list-block-data-bottom-row-select .order-workflow-list-block-data-update-date input');
			workflow.list_table.html_submit_select_modal_num = $('#modal-order-workflow-submit-confirm-select .modal-order-workflow-submit-confirm-content-count-value-num');
			workflow.list_table.html_submit_select_modal_date = $('#modal-order-workflow-submit-confirm-select .modal-order-workflow-submit-confirm-content-date-value');
			workflow.list_table.html_submit_select_modal_num_cassete = $('#modal-order-workflow-submit-confirm-select .modal-order-workflow-submit-confirm-content-count-value');
			workflow.list_table.html_submit_all_btn = $('.order-workflow-list-block-data-submit-btn-all');
			workflow.list_table.html_submit_all_date = $('.order-workflow-list-block-data-bottom-row-all .order-workflow-list-block-data-update-date input');
			workflow.list_table.html_submit_all_modal_num = $('#modal-order-workflow-submit-confirm-all .modal-order-workflow-submit-confirm-content-count-value-num');
			workflow.list_table.html_submit_all_modal_date = $('#modal-order-workflow-submit-confirm-all .modal-order-workflow-submit-confirm-content-date-value');

			// �Y���������擾
			workflow.list_table.hitcount = workflow.list_table.html_data_wrapper.find('input[name="hitcount"]').val();

			// ���X�g�`���̏ꍇ
			if (workflow.list_table.table_type == 'list') {
				// �X�N���[���⃊�T�C�Y�ɍ��킹�āA���s�{�^���G���A�̈ʒu�𐧌�
				$(window).scroll(function () {
					if ((workflow.step.current == 2) && (workflow.list_table.table_type == 'list')) {
						workflow.list_table.set();
					}
				});
				$(window).resize(function () {
					if ((workflow.step.current == 2) && (workflow.list_table.table_type == 'list')) {
						workflow.list_table.set();
					}
				});

				// �����j���[�̃R���p�N�g�\���ɑΉ�
				var wrapper_width = 0;
				setInterval(function () {
					if ((wrapper_width != workflow.list_table.html_data_wrapper.width())
					  && (workflow.list_table.table_type == 'list')) {
						wrapper_width = workflow.list_table.html_data_wrapper.width();
						workflow.list_table.set();
					}
				}, 1000)

				workflow.list_table.set();

				// ���X�g�̑I����Ԃɍ��킹�ă��b�Z�[�W�\��
				workflow.list_table.html_table_wrapper.find('table').each(function () {
					$(this).find('.js-check-all-input').each(function (i) {
						$(this).attr('data-row-index', i);
						$(this).get(0).onchange = function () {
							list_table_check();
						};
					});
				});

				var list_table_check = function () {
					workflow.list_table.message.show();
				};

			};

			// �J�Z�b�g�`���̏ꍇ
			if (workflow.list_table.table_type == 'cassette') {
				$('.order-workflow-list-block-data-cassette-list').each(function () {
					var list = $(this);
					var no_process_val = $(this).find('input.js-select-no-process').val();
					var inputs = $(this).find('input[type="radio"]');
					inputs.each(function () {
						$(this).change(function () {
							var no_process_flg = false;
							var has_prosess = 0;
							list.find('input:checked').each(function () {
								if ($(this).val() != no_process_val) {
									no_process_flg = true;
									has_prosess++;
								}
							});
							// ���ׂāu���������Ȃ��v��I������Ă���ꍇ�́A�{�^�����A�N�e�B�u�ɂ��Ȃ�
							if (no_process_flg) {
								// �u���������Ȃ��v�ȊO����ȏ�I������Ă���
								workflow.list_table.html_submit_select_btn.prop('disabled', false);
							} else {
								// ���ׂāu���������Ȃ��v���I������Ă���
								workflow.list_table.html_submit_select_btn.prop('disabled', true);
							}
							// �u���������Ȃ��v�ȊO��I�����Ă��鐔���J�E���g���ăZ�b�g
							workflow.list_table.html_submit_num.html(has_prosess);
						});
					});
					// ���X�g�`���p��css���K�p����Ȃ��悤�ɂ���
					workflow.list_table.message.obj.css({
						'position': '',
						'width': '',
						'left': '',
						'bottom': '',
						'margin-bottom': ''
					});
					workflow.list_table.html_table_bottom.css({
						'position': '',
						'width': '',
						'left': '',
						'bottom': '',
						'margin-bottom': ''
					});
				});

			}

			// �I�����s�{�^������
			workflow.list_table.html_submit_select_btn.click(function () {
				// ���[�_���̓��e�ύX
				if (workflow.list_table.table_type == 'list') {
					// �Ώی����A�X�V�������Z�b�g
					var checked_count = 0;
					workflow.list_table.html_table_wrapper.find('input.js-check-all-input:checked').each(function () {
						checked_count++;
					});
					workflow.list_table.html_submit_select_modal_num.html(checked_count);
					workflow.list_table.html_submit_select_modal_date.html(workflow.list_table.html_submit_select_date.val());
				}
				if (workflow.list_table.table_type == 'cassette') {
					// �Ώی����Ə������e�A�X�V�������Z�b�g
					var no_process_label = $('label[for="' + $('.order-workflow-list-block-data-cassette-list input.js-select-no-process').attr('id') + '"]').html();
					var select_list = [];
					$('.order-workflow-list-block-data-cassette-list-item-process:eq(0) label').each(function (i) {
						select_list[i] = {
							'label': $(this).html(),
							'count': 0
						}
					});
					$('.order-workflow-list-block-data-cassette-list').each(function () {
						$(this).find('input:checked').each(function () {
							var label = $('label[for="' + $(this).attr('id') + '"]').html();
							$(select_list).each(function () {
								if (this.label == label) {
									this.count++;
								}
							});
						});
					});
					var html = '';
					$(select_list).each(function () {
						if (this.label != no_process_label && this.count != 0) {
							html += '<p class="modal-order-workflow-submit-confirm-content-count-value-list"><span data-popover-message="' + this.label + '" class="modal-order-workflow-submit-confirm-content-count-value-list-label line-clamp">' + this.label + '</span><span class="modal-order-workflow-submit-confirm-content-count-value-list-num">' + this.count + '</span><span class="modal-order-workflow-submit-confirm-content-count-value-list-unit">��</span></p>';
						}
					});
					workflow.list_table.html_submit_select_modal_num_cassete.html(html);
					workflow.list_table.html_submit_select_modal_date.html(workflow.list_table.html_submit_select_date.val());
					line_clamp_popover.ini();
				}

				modal.open('#modal-order-workflow-submit-confirm-select', 'modal-size-m', 'line_clamp_popover.set()');
			});

			// �S�����s�{�^������
			workflow.list_table.html_submit_all_btn.click(function () {
				// ���[�_���̓��e�ύX
				// �X�e�[�^�X�X�V�����f
				workflow.list_table.html_submit_all_modal_date.html(workflow.list_table.html_submit_all_date.val());

				modal.open('#modal-order-workflow-submit-confirm-all', 'modal-size-m', 'line_clamp_popover.set()');
			});

		},
		set: function () {
			var st = $(window).scrollTop();
			var wh = $(window).height();
			var table_height = $('.order-workflow-list-block-data-table').height() + 20;
      var max_table_height = table_height > wh * 0.3 ? wh * 0.3 : table_height;
			var table_wrapper_height = 0;
			var message_obj_height = 0;
      if (workflow.list_table.message.obj.is(':visible')) {
				message_obj_height = workflow.list_table.message.obj.outerHeight();
			}
			var validate_message = $('.order-workflow-list-block-validate-message');
			if (validate_message.is(':visible')) {
				message_obj_height += validate_message.outerHeight();
			}
			table_wrapper_height = st + wh - workflow.list_table.html_table_bottom.outerHeight() - workflow.list_table.html_table_wrapper.offset().top - message_obj_height;

			// Adjust table height according to window width
			if (wh >= 1024) {
				table_wrapper_height += 240;
			} else if ((wh < 1024) && (wh >= 800)) {
				table_wrapper_height += 300;
			} else {
				table_wrapper_height += 340;
			}

			if (table_wrapper_height >= max_table_height) {
				// �X�N���[���I��
				// ���܂ŃX�N���[�����ꂽ
				table_wrapper_height = max_table_height;
				workflow.list_table.message.obj.css({
					'position': '',
					'width': '',
					'left': '',
					'bottom': '',
					'margin-bottom': ''
				});
				workflow.list_table.html_table_bottom.css({
					'position': '',
					'width': '',
					'left': '',
					'bottom': '',
					'margin-bottom': ''
				});
			} else {
				// �X�N���[������
				workflow.list_table.message.obj.css({
					'position': 'fixed',
					'width': workflow.list_table.html_data_wrapper.outerWidth(),
					'left': workflow.list_table.html_data_wrapper.offset().left,
					'bottom': workflow.list_table.html_table_bottom.height(),
					'margin-bottom': '0'
				});
				if ((720 <= wh) && (200 < table_wrapper_height)) {
					workflow.list_table.html_table_bottom.css({
						'position': 'fixed',
						'width': workflow.list_table.html_data_wrapper.outerWidth(),
						'left': workflow.list_table.html_data_wrapper.offset().left,
						'bottom': '0',
						'margin-bottom': '0'
					});
				}
			}

			FixedMidashi.resize();
		},
		message: {
			obj: null,
			timer: null,
			show: function () {
				var checked_count = 0;
				workflow.list_table.html_table_wrapper.find('input.js-check-all-input:checked').each(function () {
					checked_count++;
				});
				workflow.list_table.message.obj.html(selectedTargetDispText.replace('@ 1 @', checked_count));
				workflow.list_table.html_submit_num.html(checked_count);
				workflow.list_table.message.obj.show();
				workflow.list_table.html_submit_num.html(checked_count);
				workflow.list_table.select_mode_all = false;
				if (checked_count == 0) {
					workflow.list_table.message.obj.hide();
					workflow.list_table.html_submit_num.html(checked_count);
					workflow.list_table.select_mode_all = false;
					workflow.list_table.html_submit_select_btn.prop('disabled', true);
				} else {
					workflow.list_table.html_submit_select_btn.prop('disabled', false);
				}
				workflow.list_table.set();
			}
		}
	}
};
$(function () {
	workflow.select.ini();
});


/*------------------------------------------------
* ���[�N�t���[���s��� �ꗗ��������
------------------------------------------------*/
var adjust_window_size = function () {
	var st = $(window).scrollTop();
	$('.order-workflow-list-block-inner').each(function () {
		var winHeight = $(window).height();
		var boxPos = $(this).offset().top;
		var boxHeight = 0;
		boxHeight = winHeight - boxPos - 20 + st;
		$(this).height(boxHeight);
	});

	if (browser == 'ie') {
		var width = (window.innerWidth > 0) ? window.innerWidth : screen.width;
		$('.order-workflow-list-block-fixed').css('max-width', width - 30);
	}
}
$(function () {
	adjust_window_size();
});
$(window).bind('load', function () {
	adjust_window_size();
});
$(window).resize(function () {
	adjust_window_size();
});


/*------------------------------------------------
* ���[�N�t���[���s��� �\���`���ؑ�
------------------------------------------------*/
var workflow_view = {
	'workflow_list': '.order-workflow-list-block-list',
	'radio': '[name="select-view"]',
	'ini': function () {
		$(workflow_view.radio).click(function () {
			workflow_view.switch($(this).val());
		});
	},
	'switch': function (mode) {
		if (mode == 'detail') {
			$(workflow_view.workflow_list).addClass('list-detail');
		} else {
			$(workflow_view.workflow_list).removeClass('list-detail');
		}
	}
};
$(function () {
	workflow_view.ini();
});


/*------------------------------------------------
* �_�~�[����
------------------------------------------------*/
$(function () {
	// ���ʉ�ʂ�
	$('.js-execute-upload').click(function () {
		workflow.step.switch(3);
		modal.close();
		return false;
	});

	// ���ʉ�� ���̃��[�N�t���[����
	$('.order-workflow-list-block-next-workflow-btn').click(function () {
		workflow.step.switch(2);
		return false;
	});

	// ���ʉ�� �����ď���������
	$('.order-workflow-list-block-continue-btn').click(function () {
		workflow.step.switch(2);
		return false;
	});
});

