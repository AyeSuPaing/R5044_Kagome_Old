/*------------------------------------------------
* フォーム一体型LP
------------------------------------------------*/
var formlp = {
  page_obj: null,
  blocks: null,
  dragging_element_id: null,
  selected_section: null,
  device_mode: 'pc',
  droparea_html: '<div class="formlp-front-section-droparea"><div class="formlp-front-section-droparea-inner"></div></div>',
  blocklist: null,
  blockeditor: null,
  simplebar_col_page_elem: null,
  simplebar_col_page_scroll: 0,
  simplebar_col_side_elem: null,
  simplebar_col_side_scroll: 0,
  rep_pc_site_root: '',
  font_size_list: [
    '10px', '11px', '12px', '14px', '16px', '18px', '20px', '24px', '28px', '32px', '36px', '40px', '50px', '62px', '74px', '86px'
  ],
  font_weight_list: [
    {
      label: 'ノーマル',
      value: '100'
    },
    {
      label: 'ボールド',
      value: '700'
    }
  ],
  font_list: [
    {
      name: "ゴシック体",
      ref: "",
      classname: "f-sanserif"
    },
    {
      name: "明朝体",
      ref: "",
      classname: "f-serif"
    },
    {
      name: "Noto Sans JP",
      ref: "https://fonts.googleapis.com/css?family=Noto+Sans+JP:400,700&display=swap&subset=japanese",
      classname: "f-notosansjp"
    },
    {
      name: "Noto Serif JP",
      ref: "https://fonts.googleapis.com/css?family=Noto+Serif+JP:400,700&display=swap&subset=japanese",
      classname: "f-notoserifjp"
    },
    {
      name: "さわらび明朝",
      ref: "https://fonts.googleapis.com/css?family=Sawarabi+Mincho",
      font_family: "Sawarabi Mincho",
      classname: "f-sawarabimincho"
    }
  ],
	ini: function () {

    if ($('.formlp-design-editor-body-sp').length) {
      formlp.device_mode = 'sp';
    }

    formlp.blocklist = $('.formlp-design-editor-sidecolumn-blocklist');
    formlp.blockeditor = $('.formlp-design-editor-sidecolumn-blockeditor');
    //ブロック一覧読み込み
    $.ajax({
			url: '../Js/formlp/formlp-blocks.json',
      type: "GET",
      dataType: "json",
      timespan: 1000
		}).done(function (json, textStatus, jqXHR) {
      // console.log("成功", json, textStatus, jqXHR);
			$(json.block_groups).each(function () {
        var html = '<div class="formlp-design-editor-sidecolumn-blocklist-group" data-blocklist-gruop-id="' + this.id + '"><div class="formlp-design-editor-sidecolumn-blocklist-group-title">' + this.title + '</div><div class="formlp-design-editor-sidecolumn-blocklist-group-content"></div></div></div>';
        $('.formlp-design-editor-sidecolumn-blocklist-content').append(html);
      });

      formlp.blocks = json.blocks;

			$(formlp.blocks).each(function () {
        var html = '<div class="formlp-design-editor-sidecolumn-blocklist-item" data-block-id="' + this.id + '"><div class="formlp-design-editor-sidecolumn-blocklist-item-img"><img src="' + this.thum + '" alt=""></div><div class="formlp-design-editor-sidecolumn-blocklist-item-name">' + this.title + '</div></div>';
        $('[data-blocklist-gruop-id="' + this.group + '"] .formlp-design-editor-sidecolumn-blocklist-group-content').append(html);
      });

		}).fail(function (jqXHR, textStatus, errorThrown) {

      // console.log("失敗", textStatus, jqXHR);

		}).always(function () {

      // console.log('終了');
      formlp.show();

    });

  },
	show: function () {

    formlp.event_set();
    formlp_image_file_select.ini();
    
		setTimeout(function () {
      $('.formlp-design-editor-body').css('opacity', '1');
			setTimeout(function () {
        // 各カラムのスクロールバー
        formlp.simplebar_col_page_elem = new SimpleBar($('.formlp-design-editor-page')[0]);
        formlp.simplebar_col_side_elem = new SimpleBar($('.formlp-design-editor-sidecolumn')[0]);
        loading_animation.end();
			}, 2000);
		}, 1000);

  },
	event_set: function () {

    //ページHTMLの加工
    formlp.page_obj = $('.formlp-design-editor-page-body');
    formlp.page_obj.find('.formlp-front-section-droparea,.formlp-design-editor-block-edit').remove();

    //各ブロックの編集ツール系要素を挿入
		formlp.page_obj.find('.formlp-front-section').not('.formlp-front-section--sfree').append(
			'<div class="formlp-design-editor-block-edit"><div class="formlp-design-editor-block-edit-inner"><div class="formlp-design-editor-block-edit-btns1"><a href="javascript:void(0)" class="formlp-design-editor-block-edit-move-up btn btn-main btn-size-m" title="上へ移動"><span class="icon-arrow-down icon-arrow-down-top"></span></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-move-down btn btn-main btn-size-m" title="下へ移動"><span class="icon-arrow-down"></span></a></div>'
			+ '<div class="formlp-design-editor-block-edit-btns2"><a href="javascript:void(0)" class="formlp-design-editor-block-html-copy btn btn-main btn-size-m" title="フリーHTMLとして複製"><span class="icon icon-preview"></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-copy btn btn-main btn-size-m" title="複製する"><span class="icon icon-copy"></span></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-delete btn btn-main btn-size-m" title="削除する"><span class="icon icon-trash"></span></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-edit btn btn-main btn-size-m" title="編集する"><span class="icon icon-pencil"></span></a></div></div></div>');

		formlp.page_obj.find('.formlp-front-section.formlp-front-section--sfree').append(
			'<div class="formlp-design-editor-block-edit"><div class="formlp-design-editor-block-edit-inner"><div class="formlp-design-editor-block-edit-btns1"><a href="javascript:void(0)" class="formlp-design-editor-block-edit-move-up btn btn-main btn-size-m" title="上へ移動"><span class="icon-arrow-down icon-arrow-down-top"></span></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-move-down btn btn-main btn-size-m" title="下へ移動"><span class="icon-arrow-down"></span></a></div>'
			+ '<div class="formlp-design-editor-block-edit-btns2"><a href="javascript:void(0)" class="formlp-design-editor-block-edit-copy btn btn-main btn-size-m" title="複製する"><span class="icon icon-copy"></span></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-delete btn btn-main btn-size-m" title="削除する"><span class="icon icon-trash"></span></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-edit btn btn-main btn-size-m" title="編集する"><span class="icon icon-pencil"></span></a></div></div></div>');

    //フォームブロックに並べ替え系要素を挿入
    formlp.page_obj.find('.formlp-front-section-form').append('<div class="formlp-design-editor-block-edit"><div class="formlp-design-editor-block-edit-inner"><div class="formlp-design-editor-block-edit-btns1"><a href="javascript:void(0)" class="formlp-design-editor-block-edit-move-up btn btn-main btn-size-m" title="上へ移動"><span class="icon-arrow-down icon-arrow-down-top"></span></a><a href="javascript:void(0)" class="formlp-design-editor-block-edit-move-down btn btn-main btn-size-m" title="下へ移動"><span class="icon-arrow-down"></span></a></div></div></div>');

    //ブロックの並び替え
    formlp.page_obj.sortable({
      handle: ".formlp-design-editor-block-edit",
      axis: "y",
      placeholder: "ui-state-highlight",
      distance: 15,
			start: function (event, ui) {
        formlp.page_obj.addClass('is-sort');
        ui.helper.css('top', ui.originalPosition.top);
				formlp.page_obj.sortable("refreshPositions");
      },
			stop: function () {
        formlp.event_set();
        formlp.page_obj.removeClass('is-sort');
      }
    });

    //up
		formlp.page_obj.find('.formlp-design-editor-block-edit-move-up').on('click', function () {
      var this_ = $(this).closest('.formlp-front-section,.formlp-front-section-form');
      var prev_ = this_.prev();

      this_.css({
        'transition': '0.5s',
        'transform': 'translateY(' + '-' + (prev_.outerHeight()) + 'px)'
      });

      prev_.css({
        'transition': '0.5s',
        'transform': 'translateY(' + (this_.outerHeight()) + 'px)'
      });

			setTimeout(function () {
        prev_.before(this_);
        prev_.css({
          'transition': '',
          'transform': ''
        });
        this_.css({
          'transition': '',
          'transform': ''
        });
      }, 500);

    });

    //down
		formlp.page_obj.find('.formlp-design-editor-block-edit-move-down').on('click', function () {
      var this_ = $(this).closest('.formlp-front-section,.formlp-front-section-form');
      var next_ = this_.next();

      this_.css({
        'transition': '0.5s',
        'transform': 'translateY(' + (next_.outerHeight()) + 'px)'
      });

      next_.css({
        'transition': '0.5s',
        'transform': 'translateY(-' + (this_.outerHeight()) + 'px)'
      });

			setTimeout(function () {
        this_.before(next_);
        next_.css({
          'transition': '',
          'transform': ''
        });
        this_.css({
          'transition': '',
          'transform': ''
        });
      }, 500);

    });

    //copy
		formlp.page_obj.find('.formlp-design-editor-block-edit-copy').on('click', function () {
      var tgt = $(this).closest('.formlp-front-section');
      var tgt_height = tgt.outerHeight();
      var clone_obj = $(tgt).clone(true).css({
        'opacity': '0',
        'height': '0px'
      });
      clone_obj.insertAfter(tgt);
      clone_obj.animate({
        opacity: 1,
        height: tgt_height
			}, 500, function () {
        $(this).attr('style', '');
        formlp.block_editor.close();
      });
		});

		// FreeHTMLCopy
		formlp.page_obj.find('.formlp-design-editor-block-html-copy').on('click',
			function () {
				var tgt = $(this).closest('.formlp-front-section');
				var tgt_height = tgt.outerHeight();
				$(formlp.blocks).each(function () {
					if (this.id === 'bfree') {
						var tgtHtml = tgt.html();
						var blockHtml = this.src;
						var block = jQuery.parseHTML(blockHtml);
						var t = $(block).find("[data-edit-prop-placeholder]");
						$.each(t, function (j, ele) {
							var ph = ($(ele).attr('data-edit-prop-placeholder'));
							if (ph === "HTML") {
							  $(ele).html(tgtHtml);
							  $(ele).next('input').val(tgtHtml);
							}
    });

						// HTMLを指定した後、HTML以外のeditpropをremove
						t = $(block).find("[data-edit-prop-placeholder]");
						$.each(t, function (j, ele) {
							var ph = ($(ele).attr('data-edit-prop-placeholder'));
							if (ph !== "HTML") {
								$(ele).removeAttr("data-edit-prop-placeholder");
								$(ele).removeAttr("data-edit-prop");
							}
						});

						$(block).css({
							'opacity': '0',
							'height': '0px'
						});
						tgt.after(block);
						$(block).animate({
							opacity: 1,
							height: tgt_height
						},
							500,
							function () {
								$(this).attr('style', '');
								formlp.block_editor.close();
								formlp.event_set();
							}
						);
					}
				});
			});

    //delete
		formlp.page_obj.find('.formlp-design-editor-block-edit-delete').on('click', function () {
      if (window.confirm('削除してもよろしいですか？この操作は取り消すことができません。')) {
        var tgt = $(this).closest('.formlp-front-section');
        tgt.animate({
          'opacity': 0,
          'height': 0
				}, 500, function () {
          tgt.remove();
        });

        //ブロックエディターを閉じる
        formlp.block_editor.close();
      }
    });

    //edit
		formlp.page_obj.find('.formlp-design-editor-block-edit-edit').on('click', function () {
      formlp.block_editor.open($(this).closest('.formlp-front-section'));
    });
    //ブロックをダブルクリックで編集モードに移行
		$('.formlp-front-section .formlp-design-editor-block-edit').each(function () {
			$(this).on('dblclick', function () {
        formlp.block_editor.open($(this).closest('.formlp-front-section'));
      });
    });


    //ブロックをドラッグ＆ドロップで配置
    $(".formlp-design-editor-sidecolumn-blocklist-item").draggable({
      connectToSortable: ".formlp-front-section-droparea",
      helper: "clone",
      appendTo: ".formlp-design-editor-body",
			start: function (event, ui) {
        
        //ドラッグ中はクラス追加
        $('.formlp-design-editor-page-body').addClass('is-block-dragging');

        //bodyのスクロールを無効に
        $('body').css({
          'position': 'fixed'
        });

        formlp.dragging_element_id = $(ui.helper).data('block-id');

        //ドロップエリア用HTMLを挿入
        formlp.page_obj.find('.formlp-front-section-droparea').remove();
        formlp.page_obj.find('.formlp-front-section,.formlp-front-section-form').after(formlp.droparea_html);
        formlp.page_obj.prepend(formlp.droparea_html);

        $(".formlp-front-section-droparea-inner").droppable({
          accept: ".formlp-design-editor-sidecolumn-blocklist-item",
          classes: {
            "ui-droppable-active": "ui-state-active",
            "ui-droppable-hover": "ui-state-hover"
          },
					drop: function (event, ui) {

            //ブロックを配置
            var drop_obj = $(this);
						$(formlp.blocks).each(function () {
              if (this.id == formlp.dragging_element_id) {
								if (this.list != null) {
                  //選択型ブロック
                  var droparea = drop_obj.closest('.formlp-front-section-droparea');
                  droparea.after('<div class="formlp-front-section-insert-html" style="display:none;"></div>');
                  formlp.block_select.tgt_list = this.list;
                  modal.open('#modal-block-select', 'modal-size-l', 'formlp.block_select.open("' + this.id + '")');
                } else {
                  //通常ブロック
                  var droparea = drop_obj.closest('.formlp-front-section-droparea');
                  var insert_html = this.src.replace(/@@ rep_pc_site_root @@/g, formlp.rep_pc_site_root);
                  droparea.after('<div class="formlp-front-section-insert-html" style="display:none;">' + insert_html + '</div>' + formlp.droparea_html);
									setTimeout(function () {
                    var insert_obj = $('.formlp-front-section-insert-html');
										insert_obj.slideDown(500, function () {
                      insert_obj.find('> div').unwrap();
                      formlp.event_set();
                    });
                  }, 200);
                }
              }
            });
          }
        });

      },
			stop: function () {

        //ドラッグ終了後はクラス削除
        $('.formlp-design-editor-page-body').removeClass('is-block-dragging');

        //bodyのスクロールを有効に
        $('body').css({
          'position': ''
        });

      }
    });

  },
  block_select: {
		tgt_list: null,
		selected_block: null,
		open: function (id) {
      //modalにリストをセット
      var html = '';
      $(formlp.block_select.tgt_list).each(function (i) {
      var thum_src = '';
      if(formlp.device_mode == 'pc'){
        //PC
        thum_src = this.thum;
      } else {
        //SP
        thum_src = this.thum_sp;
      }
        html += 
				  '<div class="modal-inner-block-select-item">' +
				  '  <div class="modal-inner-block-select-item-thum"><img src="' + thum_src + '" alt=""></div>' +
				  '  <div class="modal-inner-block-select-item-title">' + this.title + '</div>' +
          '</div>';
      });
      
      $(modal.tgt_selector).find('.modal-inner-block-select').html(html);
      
      var selected_class_name = 'is-select';

			$(modal.tgt_selector).find('.modal-inner-block-select-item').each(function (i) {
				if (i == 0) {
          $(this).addClass(selected_class_name);
        }
				$(this).on('click', function () {
          $(modal.tgt_selector).find('.modal-inner-block-select-item').removeClass(selected_class_name);
          $(this).addClass(selected_class_name);
          formlp.block_select.selected_block = formlp.block_select.tgt_list[i];
        });
      });

      //1つ目のブロックを選択状態に
      formlp.block_select.selected_block = formlp.block_select.tgt_list[0];

    },
		set: function () {
      
      var insert_obj = $('.formlp-front-section-insert-html');
      insert_obj.html(formlp.block_select.selected_block.src.replace(/@@ rep_pc_site_root @@/g, formlp.rep_pc_site_root));
			insert_obj.slideDown(500, function () {
        insert_obj.find('> div').unwrap();
        formlp.event_set();
      });
      
      modal.close();

    }
  },
  block_editor: {
		open: function (section) {
      formlp.blocklist.hide();
      formlp.blockeditor.show();

      //spectrum 要素削除
      $('.sp-container').remove();

      formlp.selected_section = section;

      //選択中のセクションにクラス付与
      $('.formlp-front-section-editing').removeClass('formlp-front-section-editing');
      formlp.selected_section.addClass('formlp-front-section-editing');

      formlp.blockeditor.find('.formlp-design-editor-sidecolumn-blockeditor-content').html('');

      //アニメーション完了後に表示
			setTimeout(function () {

        //編集フォームの生成
        var html = "";

        //グループ化するための印つけ
        //既に付与されているものを一旦削除
        section.find('[data-edit-prop]').removeData('edit-group-last-flg');

        var group_id_arr = [];
				section.find('[data-edit-group]').each(function () {
          group_id_arr.push($(this).data('edit-group'));
        });

        var group_maxcount = 10;
				if (section.find('[data-edit-group]:eq(0)').data('edit-group-max') != undefined) {
          group_maxcount = section.find('[data-edit-group]:eq(0)').data('edit-group-max');
        }
        
        //重複削除
				group_id_arr = group_id_arr.filter(function (x, i, self) {
          return self.indexOf(x) === i;
        });
				$(group_id_arr).each(function () {
          var id = String(this);
					section.find('*[data-edit-group="' + id + '"]').each(function (i) {

             $(this).find('*[data-edit-prop]:first').data('edit-group-index-first', true);
             $(this).find('*[data-edit-prop]:last').data('edit-group-index-last', true);

            //最後に追加ボタン設置
            if (i == section.find('*[data-edit-group="' + id + '"]').length - 1) {
              $(this).find('*[data-edit-prop]:last').data('edit-group-last-flg', true).data('edit-group-id', id);
            }

          });
        });

        //グループの何個目かを管理する変数
        var group_index = 0;
				section.find('*[data-edit-prop]').each(function (i) {
          var element = $(this);

          //グループ化
          if (element.data('edit-group-index-first')) {
            group_index++;
            var placeholder = element.closest('*[data-edit-group]').data('edit-group-placeholder') + ' ' + group_index;
            var element_group = element.closest('*[data-edit-group]').data('edit-group');
            html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-group"><div class="formlp-design-editor-sidecolumn-blockeditor-content-element-group-title">' + placeholder + '</div>';

            var tgt_group = $(formlp.selected_section).find('*[data-edit-group="' + element_group + '"]');
            //残り1個の場合は削除ボタンは非表示にする
						if (tgt_group.length > 1) {
              html += '<a class="formlp-design-editor-sidecolumn-blockeditor-content-element-group-btn-delete btn btn-txt btn-size-s" data-target-name="' + i + '" data-target-group="' + element_group + '" data-target-group-index="' + group_index + '">削除</a>';
            }
          }

          var prop = element.data('edit-prop').split(',');
          var placeholder = element.data('edit-prop-placeholder');
          html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element">';
          if (placeholder) {
            html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-title">' + placeholder + '</div>';
          }


					$(prop).each(function (n) {

            //テキスト内容・フォント関連の設定
            if (prop[n] == 'text') {
              html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-block">';

              var prop_name = 'text';
              var value = element.html();
              var title = 'テキスト内容';
              html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '">' + title + '</label></div><div class="form-element-group-content"><textarea name="prop-' + i + '-' + n + '"  id="form-input-prop-' + i + '-' + n + '" placeholder="' + title + '" data-element-class="' + prop_name + '" maxlength="300">' + value + '</textarea></div></div>';

              //フォント
              var prop_name = 'font';
              var value = element.css('font-family');
              
              var html_select_font_options = '';

              var attr_checked = '';
							$(formlp.font_list).each(function () {
								if (element.hasClass(this.classname)) {
                  attr_checked = 'selected="selected"';
                } else {
                  attr_checked = '';
                }
								html_select_font_options += '<option value="' + this.classname + '" ' + attr_checked + '>' + this.name + '</option>';
              });
              
              var title = 'フォント';
							html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '-' + prop_name + '">' + title + '</label></div><div class="form-element-group-content"><select name="prop-' + i + '-' + n + '-' + prop_name + '" id="form-input-prop-' + i + '-' + n + '-' + prop_name + '" data-element-class="' + prop_name + '">' + html_select_font_options + '</select></div></div>';

              //文字ウエイト
              var prop_name = 'font-weight';
              var value = element.css(prop_name);
              var title = 'ウエイト';
              var html_select_font_weight_options = '';

              var attr_checked = '';
							$(formlp.font_weight_list).each(function () {
								if (element.css('font-weight') == this.value) {
                  attr_checked = 'selected="selected"';
                } else {
                  attr_checked = '';
                }
								html_select_font_weight_options += '<option value="' + this.value + '" ' + attr_checked + '>' + this.label + '</option>';
              });

							html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '-' + prop_name + '">' + title + '</label></div><div class="form-element-group-content"><select name="prop-' + i + '-' + n + '-' + prop_name + '" id="form-input-prop-' + i + '-' + n + '-' + prop_name + '" data-element-class="' + prop_name + '">' + html_select_font_weight_options + '</select></div></div>';

              //文字色
              var prop_name = 'color';
              var value = element.css(prop_name);
              var title = '文字色';
              html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '-' + prop_name + '">' + title + '</label></div><div class="form-element-group-content"><input name="prop-' + i + '-' + n + '-' + prop_name + '" value="' + value + '" type="text" id="form-input-prop-' + i + '-' + n + '-' + prop_name + '" placeholder="' + title + '" data-element-class="' + prop_name + '"></div></div>';

              //文字サイズ
              var prop_name = 'font-size';
              var value = element.css(prop_name);
              var title = 'サイズ';
              html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '-' + prop_name + '">' + title + '</label></div><div class="form-element-group-content"><input name="prop-' + i + '-' + n + '-' + prop_name + '" value="' + value + '" type="text" id="form-input-prop-' + i + '-' + n + '-' + prop_name + '" placeholder="' + title + '" data-element-class="' + prop_name + '"></div></div>';

              html += '</div>';

            }

            //href
            if (prop[n].match(/href/)) {
              html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-block">';

              var attr_checkbox_checked = '';
              if (element.attr('target') == '_blank') {
                attr_checkbox_checked = 'checked';
              }
              var title = 'リンク';
              html += '<div class="form-element-group form-element-group-vertical ' + prop[n] + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '">' + title + '</label></div><div class="form-element-group-content"><input type="text" name="prop-' + i + '-' + n + '" id="form-input-prop-' + i + '-' + n + '" placeholder="' + title + '" data-element-class="' + prop[n] + '" maxlength="300"></div></div>';

              //target blank
              html += '<div class="form-element-group form-element-group-vertical target"><div class="form-element-group-content">';
              html += '<input type="checkbox" name="prop-' + i + '-' + n + '-target" value="_blank" id="form-input-prop-' + i + '-' + n + '-target" data-element-class="target" ' + attr_checkbox_checked + '><label for="form-input-prop-' + i + '-' + n + '-target">別窓で開く</label>';
              html += '</div></div>';

              html += '</div>';

            }

            //画像
            if (prop[n] == 'src') {
              html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-block">';

              var value = element.attr(prop[n]);
              var title = '画像';
              html += '<div class="form-element-group form-element-group-vertical ' + prop[n] + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '">' + title + '</label></div><div class="form-element-group-content"><div class="formlp-design-editor-image-select"><div class="formlp-design-editor-image-select-selected-image-box"><img src="' + value + '" alt="" class="formlp-design-editor-image-select-selected-image" data-target-name="prop-' + i + '-' + n + '"><div class="formlp-design-editor-image-select-selected-image-message"></div></div><input name="prop-' + i + '-' + n + '" value="' + value + '" type="hidden" data-element-class="' + prop[n] + '"><div class="formlp-design-editor-image-select-btns"><a href="javascript:void(0);" class="formlp-design-editor-image-select-btn-change btn btn-txt btn-size-s" data-target-name="prop-' + i + '-' + n + '">変更</a><a href="javascript:void(0);" class="formlp-design-editor-image-select-btn-delete btn btn-txt btn-size-s" data-target-name="prop-' + i + '-' + n + '">削除</a></div></div></div></div>';

              html += '</div>';
            }

            //背景
            if (prop[n] == 'background') {
              html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-block">';

              //背景色
              var prop_name = 'background-color';
              var value = element.css(prop_name);
              var title = '背景色';
              html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '-' + prop_name + '">' + title + '</label></div><div class="form-element-group-content"><input name="prop-' + i + '-' + n + '-' + prop_name + '" value="' + value + '" type="text" id="form-input-prop-' + i + '-' + n + '-' + prop_name + '" placeholder="' + title + '" data-element-class="' + prop_name + '"></div></div>';

              //背景画像
              var prop_name = 'background-image';
              var value = '';
              if (element.css(prop_name).match(/url/)) {
                value = element.css(prop_name).split('"')[1];
              }
              var title = '背景画像';
              html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '-' + prop_name + '">' + title + '</label></div><div class="form-element-group-content"><div class="formlp-design-editor-image-select"><div class="formlp-design-editor-image-select-selected-image-box"><img src="' + value + '" alt="" class="formlp-design-editor-image-select-selected-image" data-target-name="prop-' + i + '-' + n + '-' + prop_name + '"><div class="formlp-design-editor-image-select-selected-image-message"></div></div><input name="prop-' + i + '-' + n + '-' + prop_name + '" value="' + value + '" type="hidden" data-element-class="' + prop_name + '"><div class="formlp-design-editor-image-select-btns"><a href="javascript:void(0);" class="formlp-design-editor-image-select-btn-change btn btn-txt btn-size-s" data-target-name="prop-' + i + '-' + n + '-' + prop_name + '">変更</a><a href="javascript:void(0);" class="formlp-design-editor-image-select-btn-delete btn btn-txt btn-size-s" data-target-name="prop-' + i + '-' + n + '-' + prop_name + '">削除</a></div></div></div></div>';

              html += '</div>';
            }

            //枠線
            if (prop[n] == 'border') {
              html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-block">';

              //枠線の色
              var prop_name = 'border-color';
              var value = element.css(prop_name);
              var title = '枠色';
              html += '<div class="form-element-group form-element-group-vertical ' + prop_name + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '">' + title + '</label></div><div class="form-element-group-content"><input name="prop-' + i + '-' + n + '-' + prop_name + '" value="' + value + '" type="text" id="form-input-prop-' + i + '-' + n + '-' + prop_name + '" placeholder="' + title + '" data-element-class="' + prop_name + '"></div></div>';

              html += '</div>';
            }


            //フリーHTML
            if (prop[n] == 'html-src') {
              html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-block">';
              var value = $(element).next('input').val() ? $(element).next('input').val() : $(element).html();
              var textAreaContent = escapeHtml(value);
              var title = 'HTMLソース';
              var remarks = '<br />※ HTML構文が不正の場合に実際の表示内容が崩れる可能性があります。<br />フリーHTMLを変更した際は更新前に「プレビュー」での確認をお願いします。';
              html += '<div class="form-element-group form-element-group-vertical ' + prop[n] + '"><div class="form-element-group-title"><label for="form-input-prop-' + i + '-' + n + '">' + title + '</label></div><div class="form-element-group-content"><div class="codeeditor" id="editor" style="display: none;">' + textAreaContent + '</div><textarea name="prop-' + i + '-' + n + '"  id="form-input-prop-' + i + '-' + n + '" placeholder="' + title + '" data-element-class="' + prop[n] + '" maxlength="1000">' + textAreaContent + '</textarea></div><div class="form-element-group-remarks">' + remarks + '</div></div>';

              html += '</div>';
            }

          });

          html += '</div>';

          //グループ閉じ
          if (element.data('edit-group-index-last')) {
            html += "</div>";
          }

          //最後のグループの後に「追加」ボタン設置
          if (element.data('edit-group-last-flg') && group_maxcount > group_index) {
            var element_group_id = element.data('edit-group-id');
            html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-element-group-addbtn"><a href="javascript:void(0);" class="formlp-design-editor-sidecolumn-blockeditor-content-element-group-addbtn-btn btn btn-txt btn-size-s" data-target-group-id="' + element_group_id + '"><span class="icon icon-plus"></span>追加する</a></div>';
            group_index = 0;
          }

        });

        html += '<div class="formlp-design-editor-sidecolumn-blockeditor-content-btn"><a href="javascript:void(0);" class="formlp-design-editor-sidecolumn-blockeditor-content-btn-close btn btn-txt btn-size-s">閉じる</a></div><div class="formlp-design-editor-sidecolumn-blockeditor-content-btn2"><a href="javascript:void(0);" class="formlp-design-editor-sidecolumn-blockeditor-content-btn-close btn btn-txt btn-size-m">閉じる</a></div>';

        formlp.blockeditor.find('.formlp-design-editor-sidecolumn-blockeditor-content').append(html);

        section.find('*[data-edit-prop]').each(function (i) {
          var element = $(this);
          var prop = element.data('edit-prop').split(',');
          $(prop).each(function (n) {
            if (prop[n].match(/href/)) {
              $('#form-input-prop-' + i + '-' + n).val(element.attr('href'));
            }
          });
        });

        //スクロール位置
        $(formlp.simplebar_col_side_elem).scrollTop(formlp.simplebar_col_side_scroll);
        formlp.simplebar_col_side_scroll = 0;

        //スライド式チェックボックス
        slide_check.ini();

        //フォント選択プルダウンの初期選択設定
				formlp.blockeditor.find('.form-element-group.font select,.form-element-group.font-weight select').each(function () {
					$(this).on('change', function () {
            formlp.block_editor.prop_reflect();
          });
        });

        //カラーピッカーUI
        //パレットカラー一覧取得
        var color_palette_arr = [];
				$('*[data-edit-prop]').each(function () {
          color_palette_arr.push($(this).css('color'));
          color_palette_arr.push($(this).css('background-color'));
          color_palette_arr.push($(this).css('border-color'));
        });
        //重複を削除
				color_palette_arr = color_palette_arr.filter(function (x, i, self) {
          return self.indexOf(x) === i;
        });
				formlp.blockeditor.find('.form-element-group.color input[type="text"],.form-element-group.background-color input[type="text"],.form-element-group.border-color input[type="text"]').each(function () {
          var input_obj = $(this);
          input_obj.spectrum({
            showInput: true,
            showPalette: true,
            showButtons: false,
            showInitial: true,
            showAlpha: true,
            palette: [
              color_palette_arr
            ],
            chooseText: "選択",
            cancelText: "キャンセル",
						move: function (color) {
              input_obj.val(color.toRgbString());
              formlp.block_editor.prop_reflect();
            }
          });
        });

        //フォントサイズUI
				formlp.blockeditor.find('.form-element-group.font-size input[type="text"]').each(function () {
          var input_obj = $(this);
          var content = input_obj.closest('.form-element-group-content');
					input_obj.on('focus', function () {

            var list_html = '';
						$(formlp.font_size_list).each(function () {
              list_html += '<div class="input-custom-options-option" data-value="' + this + '">' + this + '</div>';
            });

            var input_height = input_obj.outerHeight();
            var input_width = input_obj.outerWidth();

            content.css('position', 'relative').append('<div class="input-custom-options" style="position:absolute;top:' + input_height + 'px;left:0px;width:' + input_width + 'px;">' + list_html + '</div>');

						content.find('.input-custom-options-option').each(function () {
							$(this).off('click').on('click', function () {
                var val = $(this).data('value');
                content.find('input[type="text"]').val(val);
                formlp.block_editor.prop_reflect();
              });
            });

          });
					input_obj.on('blur', function () {
						setTimeout(function () {
              content.find('.input-custom-options').hide();
            }, 200);
          });
        });

        //画像ファイル指定
        //モーダルトリガー
				formlp.blockeditor.find('.formlp-design-editor-image-select-btn-change').each(function () {
          var target_name = $(this).data('target-name');
					$(this).on('click', function () {
            //変更ボタンを押したら、モーダル開く
            formlp.image_edit_target_name = target_name;
						modal.open('#modal-image-select', 'modal-size-l modal-image-select', function () {
              formlp_image_file_select.ini()
            });
          });
        });

				formlp.blockeditor.find('.formlp-design-editor-image-select-btn-delete').each(function () {
          var target_name = $(this).data('target-name');
					$(this).on('click', function () {
            //削除ボタンを押したら、画像情報を削除
            formlp.blockeditor.find('img[data-target-name="' + target_name + '"]').attr('src', '');
            formlp.blockeditor.find('input[name="' + target_name + '"]').val('');
            formlp.block_editor.prop_reflect();
          });
        });

        //HTMLソースエディター設置
		formlp.blockeditor.find('.codeeditor').each(function () {
      var element = $(this)[0];
      var editor = ace.edit(element, {
				mode: "ace/mode/html",
				selectionStyle: "text",
				setTheme: "ace/theme/clouds",
				useWorker: false
			});
			var textarea = $(this).next('textarea').hide();
      editor.getSession().setValue(html_beautify(textarea.val(), { contentUnformatted: '', unformatted: '' }));
			editor.getSession().on('change', function () {
				textarea.val(editor.getSession().getValue());
				formlp.block_editor.prop_reflect();
			});
			$(this).show();
		});

		//変更内容をリアルタイムに反映
		formlp.blockeditor.find('input,textarea').each(function () {
			$(this).on('change', function () {
				formlp.block_editor.prop_reflect();
			});
		});

		// 画面リサイズ時にエディターを再設定
		var $resizeTimer;
		$(".formlp-design-editor-sidecolumn").on("resize", function () {
			if ($("#editor").length) {
				if ($resizeTimer) return;
				clearTimeout($resizeTimer);
				$resizeTimer = setTimeout(function () {
					var editor = ace.edit("editor");
					editor.resize();
					$resizeTimer = null;
				}, 500);
			}
		});

        //Enterでのsubmitを禁止
				$(document).ready(function () {
					formlp.blockeditor.find('input,textarea[readonly]').not($('input[type="button"],input[type="submit"]')).keypress(function (e) {
            if (!e) var e = window.event;
            if (e.keyCode == 13)
              return false;
          });
        });

        //グループ内のブロックを追加
				$('.formlp-design-editor-sidecolumn-blockeditor-content-element-group-addbtn-btn').on('click', function () {
          formlp.simplebar_col_side_scroll = $(formlp.simplebar_col_side_elem).scrollTop();
          var target_group_id = $(this).data('target-group-id');
					$(formlp.selected_section).find('*[data-edit-group="' + target_group_id + '"]:last').each(function () {
            $(this).clone(true).insertAfter(this);
            formlp.block_editor.open($(formlp.selected_section));
          });
        });

        //グループ削除
				$('.formlp-design-editor-sidecolumn-blockeditor-content-element-group-btn-delete').on('click', function () {
          if (window.confirm('削除してもよろしいですか？この操作は取り消すことができません。')) {
            var group = $(this).data('target-group');
            var group_index = $(this).data('target-group-index') - 1;
            var tgt_group = $(formlp.selected_section).find('*[data-edit-group="' + group + '"]');
            tgt_group.eq(group_index).remove();
            formlp.block_editor.open($(formlp.selected_section));
          }
        });

        //閉じるボタン
				$('.formlp-design-editor-sidecolumn-blockeditor-content-btn-close').on('click', function () {
          formlp.block_editor.close();
        });

        //スクロール位置をリセット
        $('.formlp-design-editor-sidecolumn').scrollTop(0);

        //テキストエリア高さ自動調整
				formlp.blockeditor.find('.text textarea').each(function () {
          var target = $(this);
          var line_height = parseInt(target.css('lineHeight'));
					var height_set = function () {
            var raw_target = target.get(0);
            while (raw_target.scrollHeight > raw_target.offsetHeight) {
              line_height++;
              target.css("height", line_height);
            }
          };
					target.on('input', function () {
            height_set();
          });
          height_set();
        });

      }, 500);
    },
		close: function (section) {
      formlp.blocklist.show();
      formlp.blockeditor.hide();
      formlp.blockeditor.find('.formlp-design-editor-sidecolumn-blockeditor-content').html('');
      $('.formlp-front-section-editing').removeClass('formlp-front-section-editing');

      //通知
			// notification.show('保存されました。', 'info', 'fadeout');
    },
		prop_reflect: function () {
			formlp.blockeditor.find('.formlp-design-editor-sidecolumn-blockeditor-content-element').each(function (i) {
        //各属性を反映
				$(this).find('input,textarea,select').each(function () {
          var element = $(formlp.selected_section).find('*[data-edit-prop]:eq(' + i + ')');
          var element_class = $(this).data('element-class');
          var value = $(this).val();
          var flg_checked = $(this).prop("checked");
          
          if (element_class == 'text') {
            element.html(value);
          }

          if (element_class == 'href') {
            element.attr('href', value);
          }

          if (element_class == 'target') {
						if (flg_checked) {
              element.attr('target', value);
            } else {
              element.attr('target', '');
            }
          }

          if (element_class == 'font') {
						$(formlp.font_list).each(function () {
              element.removeClass(this.classname);
            });
            element.addClass(value);
          }

          if (element_class == 'color' || element_class == 'font-weight' || element_class == 'font-size' || element_class == 'background-color' || element_class == 'border-color') {
            element.css(element_class, value);
          }

          if (element_class == 'background-image') {
            if (value) {
              element.css(element_class, 'url("' + value + '")');
            } else {
              element.css(element_class, 'none');
            }
          }

          if (element_class == 'src') {
            element.attr(element_class, value);
          }

          if (element_class == 'html-src') {
            // JavaScriptタグの削除
            var tempHtmlContent = delete_javascript_tag(value);
            // 不正なHTMLタグ要素を削除
            $('#free_html_temp').html(tempHtmlContent);
            var html = $('#free_html_temp').html();
            $('#free_html_temp').html("");
            element.html(html);

            // 入力内容はそのまま、不正なHTMLタグ要素を削除せず保持
            element.next('input').val(value);
          }


        });

      });

      //外部ウェブフォントインポート
      formlp.block_editor.webfontset();

    },
		webfontset: function () {
      //外部ウェブフォントインポート
      $(formlp.font_list).each(function(){
        if(formlp.page_obj.find('.'+this.classname).length){
          //クラスあり
          if(this.ref != ''){
            if(formlp.page_obj.find('.formlp-front-webfont').find('link[href="'+this.ref+'"]').length == 0){
              formlp.page_obj.find('.formlp-front-webfont').append('<link href="'+this.ref+'" rel="stylesheet">');
            }
          }
        } else {
          //クラスなし
          if(this.ref != ''){
            if(formlp.page_obj.find('.formlp-front-webfont').find('link[href="'+this.ref+'"]').length != 0){
              formlp.page_obj.find('.formlp-front-webfont link[href="'+this.ref+'"]').remove();
            }
            }
          }
      });
    }
  }
};

// Javascriptタグの削除
function delete_javascript_tag(content) {
    var result = content
        .replace(/<(script)[\s\S]*?<\/\1(\s|\S|)>/ig, '')
        .replace(/<(script)[\s\S]*?>/ig, '');
  return result;
}

// HTML内容のエスケープ
function escapeHtml(str) {
  str = str.replace(/&/g, '&amp;');
  str = str.replace(/>/g, '&gt;');
  str = str.replace(/</g, '&lt;');
  str = str.replace(/"/g, '&quot;');
  str = str.replace(/'/g, '&#x27;');
  str = str.replace(/`/g, '&#x60;');
  return str;
}

/*------------------------------------------------
* 画像ファイル選択UI（フォーム一体型LP用）
------------------------------------------------*/
var formlp_image_file_select = {
	ini: function () {
    var max_file_size = 1048576; // 最大ファイルサイズ(byte)

		$('.formlp-image-upload-content').each(function () {
      if ($(this).find('.formlp-image-upload-content-block-image img').length == 0) {
        $(this).find('.formlp-image-upload-content-block-control').hide();
      }
      var block = $(this);
      var drag_area = block.find('.formlp-image-upload-content-drag');
      var indexNum = $('.formlp-image-upload-content').index(this);

      var switch_active = {
				on: function () {
					if (!drag_area.hasClass('active')) {
            drag_area.css({
							'height': drag_area.outerHeight() + 'px',
							'padding': 0,
							'line-height': drag_area.outerHeight() + 'px'
            }).addClass('active');
          }
        },
				off: function () {
          drag_area.css({
						'height': '',
						'padding': '',
						'line-height': ''
          }).removeClass('active');
        }
      }

      if (browser == 'firefox') {
				$('body').on('dragover drop', function (event) {
          dragover(event);
        });

				drag_area.on('dragover', function (event) {
          switch_active.on();
        });

				drag_area.on('dragleave', function (event) {
          switch_active.off();
        });

				drag_area.on('drop', function (event) {
          switch_active.off();
          selectImg(event, 'drop');
        });

				block.find('.formlp-image-upload-content-drag .upload_img').on('change', function (event) {
          switch_active.off();
          selectImg(event, block, indexNum);
        });
      } else {
				$('body').on('dragover drop', function () {
          dragover(event);
        });

				drag_area.on('dragover', function () {
          switch_active.on();
        });

				drag_area.on('dragleave', function () {
          switch_active.off();
        });

				drag_area.on('drop', function () {
          switch_active.off();
          selectImg(event, 'drop');
        });

				block.find('.formlp-image-upload-content-drag .upload_img').on('change', function () {
          switch_active.off();
          selectImg(event, 'file_select');
        });

        $(".formlp-design-editor-sidecolumn").resizable({
          handles: "w"
        });
      }

        });

    function dragover(_e) {
      var e = _e;
      if (_e.originalEvent) {
        e = _e.originalEvent;
      }
      e.preventDefault();
    }

    function selectImg(_e, action_type) {
      var e = _e;
      if (_e.originalEvent) {
        e = _e.originalEvent;
      }
      e.preventDefault();

      if (action_type == 'drop') {
        files = e.dataTransfer.files;
      } else if (action_type == 'file_select') {
        files = e.target.files;
      }

      for (var i = 0; i < files.length; i++) {

        if (!files[i] || (files[i].type.indexOf('image/jpeg') < 0 && files[i].type.indexOf('image/gif') < 0 && files[i].type.indexOf('image/png') < 0)) {
          notification.show('ファイル形式はJPEG / GIF / PNGとなります。', 'warning', 'fadeout');
          continue;
        } else if (files[i].size > max_file_size) {
          notification.show('ファイルサイズは' + max_file_size / 1024 + 'KBまでです。', 'warning', 'fadeout');
          continue;
        }

        var fileReader = new FileReader();
				fileReader.onload = function (event) {

          modal.close();

          //アップされた画像を表示に反映
          var loadedImageUri = event.target.result;

          //エディターのサムネイルに反映
          $('img[data-target-name="' + formlp.image_edit_target_name + '"]').attr('src', loadedImageUri);

          //エディターのinputに反映
          $('input[name="' + formlp.image_edit_target_name + '"]').val(loadedImageUri);
          formlp.block_editor.prop_reflect();
        };
        fileReader.readAsDataURL(files[i]);
      }
    }

  }
}



$(function () {
  
  loading_animation.start();
  
  formlp.ini();

});
