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

/*------------------------------------------------
* user agent set
------------------------------------------------*/

var userAgent = window.navigator.userAgent.toLowerCase();
if (userAgent.indexOf('msie') != -1 || userAgent.indexOf('trident') != -1) {
  var browser = 'ie';
} else if (userAgent.indexOf('edge') != -1) {
  var browser = 'edge';
} else if (userAgent.indexOf('chrome') != -1) {
  var browser = 'chrome';
} else if (userAgent.indexOf('safari') != -1) {
  var browser = 'safari';
} else if (userAgent.indexOf('firefox') != -1) {
  var browser = 'firefox';
} else if (userAgent.indexOf('opera') != -1) {
  var browser = 'opera';
} else {
  var browser = 'other';
}

/*------------------------------------------------
* 共通処理
------------------------------------------------*/
$(function () {

  //DOM解析完了までコンテンツを隠す
  $('.content').css({
    'opacity': 1
  });

  //ウインドウの幅と高さを格納
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
    $(".main-contents-inner-support").append('<div class="btn_help"><a href="' + supportSiteUrl + '?s=' + encodeURI(title + '&al=on&rd=on') + '" data-popover-message="「' + title + '」について" target="_blank"><span class="icon-help"></span></a></div>');
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
    //状態をcookieに保存
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
    setCookie(cmn.cookie_sidemenu_key, cmn.cookie_sidemenu_value, {
      path: root_path,
      expires: cmn.cookie_sidemenu_expires
    });

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
      if ($('.sidemenu_slim').length) {
        var tgt = $(this).find('.sidemenu-child-list');
        tgt.addClass('popmenu-open');

        var html = tgt.html();
        var width_ = tgt.outerWidth();
        var height_ = tgt.outerHeight();
        var top = $(this).offset().top;
        cmn.wh = $(window).height();
        cmn.st = $(window).scrollTop();

        if ((cmn.st + cmn.wh - top - height_) < 0) {
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
        if ((cmn.sidemenu_slim_height - cmn.st) > cmn.wh) {
          sm.css({
            "position": "relative",
            "top": 0,
          });
        } else {
          sm.css({
            "position": "fixed",
            "top": (cmn.wh - $('.sidemenu').outerHeight()) + "px",
          });
        }
      } else {
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

/*------------------------------------------------
* ポップアップメッセージ
------------------------------------------------*/
var popover = {
  wait_time: 0,
  show_classname: 'show',
  ini: function () {
    // テキスト版
    $('[data-popover-message]').each(function (i) {
      var this_ = $(this);
      if (!this_.attr('data-popover-message-ini')) {
        this_.attr('data-popover-message-ini', true);
        var content = this_.data('popover-message');
        var position = this_.data('popover-message-position');
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
            if (position == 'top') {
              //top
              $('.popover_' + i).css({
                "left": (left + (width * .5) - (pop_w * .5)) + 'px',
                "top": (top - pop_h) + "px"
              });
              $('.popover_' + i).addClass('top');
            } else if ((ww - left) / ww > 0.5) {
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
            $('.popover_' + i).addClass(popover.show_classname);
          }, popover.wait_time);
        }, function () {
          clearTimeout(popover_timer);
          $('.popover_' + i).removeClass(popover.show_classname);
          $('.popover_' + i).remove();
        });
      }
    });

    // HTML版
    $('[data-popover-html]').each(function (i) {
      var this_ = $(this);
      if (!this_.attr('data-popover-html-ini')) {
        this_.attr('data-popover-html-ini', true);
        var content = this_.next('.popover-html-content').html();
        var position = $(this).data('popover-position');
        var popover_timer;
        var trigger_hover_flg = false;
        var trigger_hover_timer;
        this_.hover(function (e) {
          var width = this_.outerWidth();
          var height = this_.outerHeight();
          var left = this_.offset().left;
          var top = this_.offset().top;
          var ow = this_.width();
          var margin = 10;
          var ww = $(window).width();
          var st = $(window).scrollTop();
          var position_order = '';

          $('.popover-html').remove();

          if (!$('.popover-html_' + i).length) {
            $('body').append('<div class="popover-html popover-html_' + i + '">' + content + '</div>');
            var pop_html = $('.popover-html_' + i);
            var pop_w = pop_html.outerWidth();
            var pop_h = pop_html.outerHeight();

            // 表示位置判定
            if (position) {
              // 指定あり
              position_order = position;
            } else {
              // 指定なし（自動）
              // 上、右、左、下の優先順位で判定
              if (top - st > pop_h) {
                position_order = 'top';
              } else if (ww - left - ow > pop_w) {
                position_order = 'right';
              } else if (left > pop_w) {
                position_order = 'left';
              } else {
                position_order = 'bottom';
              }
            }

            if (position_order == 'right') {
              //右
              pop_html.css({
                "left": (left + width + margin) + 'px',
                "top": (top + (height / 2)) + "px"
              });
              pop_html.addClass('right');
            }

            if (position_order == 'left') {
              //左に表示
              pop_html.css({
                "left": (left - pop_w - margin) + 'px',
                "top": (top + (height / 2)) + "px"
              });
              pop_html.addClass('left');
            }

            if (position_order == 'top') {
              //上に表示
              pop_html.css({
                "left": left + 'px',
                "top": (top - pop_h + 8) + "px"
              });
              pop_html.addClass('top');
            }
            if (position_order == 'bottom') {
              //下に表示
              pop_html.css({
                "left": left + 'px',
                "top": (top + pop_h - 10) + "px"
              });
              pop_html.addClass('bottom');
            }

            pop_html.hover(function () {
              pop_html.addClass('is_hover');
            }, function () {
              pop_html.removeClass('is_hover');
              pop_close();
            });
          }
          popover_timer = setTimeout(function () {
            $('.popover-html_' + i).addClass(popover.show_classname);
          }, popover.wait_time);

          trigger_hover_flg = true;
          clearTimeout(trigger_hover_timer);

          line_clamp_popover.set();

        }, function () {
          pop_close();
        });
      }

      var pop_close = function () {
        trigger_hover_flg = false;
        clearTimeout(trigger_hover_timer);
        trigger_hover_timer = setTimeout(function () {
          if (!trigger_hover_flg && !$('.popover-html_' + i).hasClass('is_hover')) {
            clearTimeout(popover_timer);
            var pop_html = $('.popover-html_' + i);
            pop_html.removeClass(popover.show_classname);
            pop_html.remove();
          }
        }, 500);
      };
    });

  },
  reloadContent: function (index) {
    var _this = this;

    $('[data-popover-html]').attr('data-popover-html-ini', true);

    // HTML版
    $('[data-popover-html]').each(function (i) {
      if (index === i) {
        $(this).removeAttr('data-popover-html-ini');
        _this.ini();
        return false;
      }
    });
  }
}
$(function () {
  popover.ini();
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
  $('.list_box_bg').each(function () {
    //reset
    $(this).find('.div_header_left, .div_data_left, .div_header_right, .div_data_right').css({
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
    $(this).find('.div_header_left').parent().css({
      "width": w + "px"
    });
    $(this).find('.div_data_right').css({
      "left": w + "px"
    });

    $(this).find(".div_header_right").width($(this).find(".div_data_right").outerWidth());
    $(this).find(".div_data_right").width($(this).find('.div_header_right').outerWidth());
    $(this).find(".div_data_left").height($(this).find('.div_data_right').outerHeight());
  });
};

//検索結果 table幅調整
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
* 共通モーダル
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

    //背景を固定する
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
      //モーダルが開いていたら
      //一旦閉じるアニメーション実行
      jQuery(modal.selector_modal).removeClass(modal.open_class_name);
      time_delay = 500;

      //既に開いているモーダルがあったらtrueに
      modal.flg_before_modal = true;

    } else {

      //既に開いているモーダルがなかったらfalseに
      modal.flg_before_modal = false;

      //モーダルを開いた状態でモーダルを開く場合に、直前のモーダルのセレクターを格納しておく
      modal.tgt_selector_last = [selector, classname, openedfunc];

    }

    setTimeout(function () {

      //カスタムクラスをセット
      jQuery(modal.selector_modal).removeClass(modal.tgt_classname);
      jQuery(modal.selector_modal).addClass(classname);

      modal.tgt_classname = classname;

      //直前のセレクタと違う場合
      var flg_tgt_selector_diff = false; //同じ場合はfalse 、違う場合はtrue
      if (modal.tgt_selector != selector) {
        flg_tgt_selector_diff = true;
      }

      //既に格納されていたDOM位置を元に戻す
      modal.return_content();

      //新しいセレクターの格納
      modal.tgt_selector = selector;

      //元のDOM位置を保持するための仮DOM
      jQuery(modal.tgt_selector).before('<div class="js-modal-content-original-position"></div>');

      //DOMをモーダルに追加
      jQuery(modal.selector_modal + ' .modal-inner').append(jQuery(modal.tgt_selector));
      jQuery(modal.selector_modal).show();

      //直前のセレクタと違う場合
      if (flg_tgt_selector_diff) {

        //callback function 実行
        if (openedfunc) {
          modal.callback_opened_function = openedfunc;
          eval(openedfunc);
        }

      } else {

        //直前のモーダルと違うcallback functionが指定されたら実行
        if (modal.callback_opened_function != openedfunc) {
          //callback function 実行
          if (openedfunc) {
            modal.callback_opened_function = openedfunc;
            eval(openedfunc);
          }
        }

      }

      //モーダルのwidth、heightをセット
      var modal_height = jQuery(modal.selector_modal + ' .modal-inner').height();

      if (jQuery(window).height() < modal_height) {
        jQuery(modal.selector_modal + ' .modal-content-wrapper').css({
          'max-height': modal_height + 'px'
        });
      }

      //モーダル内スクロール位置リセット
      jQuery(modal.selector_modal).scrollTop(0);

      //modal show
      jQuery(modal.selector_modal).addClass(modal.open_class_name);

      modal.flg_open = true;

      //モーダル内カスタムスクロールバー表示
      // new SimpleBar(jQuery(modal.selector_modal + ' .modal-content')[0]);

      tab.ini();

    }, time_delay);

  },
  close: function () {

    //背景固定を開放する
    jQuery('body').css({
      'overflow': '',
      'max-width': '',
      'margin-right': ''
    });

    jQuery(modal.selector_modal).removeClass(modal.open_class_name);

    jQuery(modal.selector_modal).hide();

    setTimeout(function () {
      //既に格納されていたDOM位置を元に戻す
      modal.return_content();
      //modal.tgt_selector = "";
      modal.flg_open = false;
    }, 500);

    //高さ指定解除
    jQuery(modal.selector_modal + ' .modal-content-wrapper').css({
      'height': ''
    });

    //モーダルから開かれたモーダルの場合は直前に開いていたモーダルを再度開く
    setTimeout(function () {
      if (modal.flg_before_modal) {
        modal.open(modal.tgt_selector_last[0], modal.tgt_selector_last[1], modal.tgt_selector_last[2]);
        modal.flg_before_modal = false;
      }

    }, 500);

  },
  return_content: function () {
    //DOM位置を元に戻す
    jQuery('.js-modal-content-original-position').before(jQuery(modal.tgt_selector));
    jQuery('.js-modal-content-original-position').remove();
  }
}
jQuery(function () {
  modal.ini();
});

/*------------------------------------------------
* 汎用 ドロップダウンメニュー
------------------------------------------------*/
var dropdown_toggle = {
  ini: function () {
    $('.dropdown-toggle').each(function () {
      var btn = $(this);
      var clone_tgt = null;
      btn.unbind('focus').bind('focus', function () {
        var tgt = null;
        var margin = 3;
        if (btn.data('dropdown-target')) {
          tgt = $(btn.data('dropdown-target'));
        } else {
          tgt = btn.next();
        }
        clone_tgt = tgt.clone(true);
        $('body').append(clone_tgt);
        var btn_position_left = btn.offset().left;
        if (btn_position_left > $(window).width() - clone_tgt.width()) {
          btn_position_left = $(window).width() - clone_tgt.width() - 20;
        }
        clone_tgt.css({
          position: 'absolute',
          top: btn.outerHeight() + margin + btn.offset().top,
          left: 0 + btn_position_left
        })
        clone_tgt.show();
        btn.addClass('is-dropdown-open');
        //btn.toggleClass('is-dropdown-open');
        //tgt.toggle();
      });
      btn.unbind('focusout').bind('focusout', function () {
        var tgt = null;
        var margin = 3;
        setTimeout(function () {
          btn.removeClass('is-dropdown-open');
          clone_tgt.remove();
        }, 300);
      });
    });
  }
};
$(function () {
  dropdown_toggle.ini();
});

/*------------------------------------------------
* カスタムスクロールバー
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

/*------------------------------------------------
* 汎用 トグル
------------------------------------------------*/
var toggle = {
  toggle_classname: 'is-toggle-open',
  ini: function (isUseUpdatePanel) {

    //ボタン形式
    $(".btn-toggle").each(function () {
      var toggle_trigger = $(this);
      var toggle_content_selector = toggle_trigger.data("toggle-content-selector");
      var toggle_trigger_label = toggle_trigger.data("toggle-trigger-label").split(',');
      var toggle_content = null;
      if (toggle_content_selector) {
        toggle_content = $(toggle_content_selector);
      } else {
        toggle_content = toggle_trigger.next();
      }
      toggle_trigger.find(".toggle-state-icon.icon-arrow-down").remove();
      toggle_trigger.find(".toggle-trigger-label").remove();
      if (isUseUpdatePanel) {
        toggle_trigger.append('<span class="toggle-state-icon icon-arrow-down" style="transition: none;"></span>');
      }
      else {
        toggle_trigger.append('<span class="toggle-state-icon icon-arrow-down"></span>');
      }
      toggle_trigger.append('<span class="toggle-trigger-label">' + toggle_trigger_label[0] + '</span>');

      toggle_trigger.unbind("click").bind("click", function () {
        if (toggle_content.is(':visible')) {
          toggle_content.slideUp();
          toggle_trigger.removeClass(toggle.toggle_classname);
          toggle_trigger.find('.toggle-trigger-label').text(toggle_trigger_label[0]);
          $('[data-toggle-content-selector="' + toggle_content_selector + '"]').removeClass(toggle.toggle_classname);
        } else {
          toggle_content.slideDown();
          toggle_trigger.addClass(toggle.toggle_classname);
          toggle_trigger.find('.toggle-trigger-label').text(toggle_trigger_label[1]);
          $('[data-toggle-content-selector="' + toggle_content_selector + '"]').addClass(toggle.toggle_classname);
        }
        height_fit.ini();
      });
      if (toggle_content.is(':visible')) {
        toggle_trigger.addClass(toggle.toggle_classname);
        $('[data-toggle-content-selector="' + toggle_content_selector + '"]').addClass(toggle.toggle_classname);
      }
    });

    //フォーム要素形式 選択されたら開く チェックボックス用
    $(".js-toggle-form").each(function () {
      var toggle_trigger = $(this);
      var toggle_content_selector = toggle_trigger.data("toggle-content-selector");
      var toggle_content = $(toggle_content_selector);
      var toggle_checked_hide = (toggle_trigger.data("toggle-checked-hide") == 1) ? true : false;

      var set2 = function () {
        if (toggle_trigger.prop('checked') == true) {
          if (toggle_checked_hide) {
            toggle_content.slideUp();
          } else {
            toggle_content.slideDown();
          }
        } else {
          if (toggle_checked_hide) {
            toggle_content.slideDown();
          } else {
            toggle_content.slideUp();
          }
        }
      }

      toggle_trigger.unbind("change").bind("change", function () {
        set2();
      });

      set2();

    });

    //フォーム要素形式 選択されたら開く チェックボックス用
    $(".js-toggle-form-radio").each(function () {
      var toggle_trigger = $(this);
      var toggle_target_name = toggle_trigger.attr('name');

      var set = function (time) {
        var animation_time = 500;
        if (time == 0) {
          animation_time = time;
        }
        $('[name="' + toggle_target_name + '"]').each(function () {
          var toggle_trigger = $(this);
          var toggle_content_selector = toggle_trigger.data("toggle-content-selector");
          var toggle_content = $(toggle_content_selector);
          if (toggle_trigger.prop('checked')) {
            toggle_content.slideDown(animation_time);
          } else {
            toggle_content.slideUp(animation_time);
          }
        });
      }

      $('[name="' + toggle_target_name + '"]').each(function () {
        $(this).unbind("change").bind("change", function () {
          set(0);
        });
      });

      set(0);

    });

    //フォーム要素形式 選択されたら開く プルダウン用
    $(".js-toggle-form-select").each(function () {
      var toggle_trigger = $(this);

      var toggle_set = function (time) {
        var selected_value = toggle_trigger.val();
        var animation_time = 300;
        var open_content_selector = '';
        if (time == 0) {
          animation_time = time;
        }
        // 開く
        toggle_trigger.find('option').each(function () {
          if ($(this).val() == selected_value) {
            var toggle_content_selector = $(this).data('toggle-content-selector');
            open_content_selector = toggle_content_selector;
            $(toggle_content_selector).slideDown(animation_time);
          }
        });
        // 閉じる
        toggle_trigger.find('option').each(function () {
          if ($(this).val() != selected_value) {
            var toggle_content_selector = $(this).data('toggle-content-selector');
            if (open_content_selector != toggle_content_selector) {
              $(toggle_content_selector).slideUp(animation_time);
            }
          }
        });
      }

      toggle_trigger.unbind("change").bind("change", function () {
        toggle_set();
      });

      toggle_set(0);
    });
  }
}
$(function () {
  toggle.ini();
});

/*------------------------------------------------
* 各エリアの高さ制御
------------------------------------------------*/
var height_fit = {
  ini: function () {
    //一覧、編集画面
    var wh = $(window).height();
    if (wh < 600) {
      wh = 600;
    }
    $('.main-content-frame .list-content, .main-content-frame .main-content-detail').each(function () {
      var margin = 35;
      var h = wh - $(this).offset().top - margin;
      var min_height = 300;
      $(this).height(h);
    });
    $('.main-contents .main-content-detail-body').each(function () {
      var margin = 35;
      var h = wh - $(this).offset().top - margin;
      $(this).outerHeight(h);
    });

    //レイアウト編集エリア
    $('.layout-edit').each(function () {
      var h = $(this).find('.layout-edit-page').outerHeight();
      $(this).find('.layout-edit-parts').css({
        'max-height': h + 'px'
      });
    });
  }
}
$(function () {
  height_fit.ini();

  setInterval(function () {
    height_fit.ini();
  }, 1000);

});

/*------------------------------------------------
* 汎用タブ
------------------------------------------------*/
var tab = {
  ini: function () {
    $('.tabs').each(function () {
      var tabs = $(this);
      var tab_contents_list = [];
      tabs.find('.tabs-tab a').each(function () {
        tab_contents_list.push($(this).data('tab-content-selector'));
        $(this).unbind('click').bind('click', function () {
          var selector = $(this).data('tab-content-selector');

          $(tabs).find('.tabs-tab a').removeClass('is-active');
          $(this).addClass('is-active');

          $(tab_contents_list).each(function (i) {
            if (tab_contents_list[i] == selector) {
              $(tab_contents_list[i]).show();
            } else {
              $(tab_contents_list[i]).hide();
            }
          });
        });

        height_fit.ini();

      });

      //ini active
      tabs.find('.tabs-tab a').each(function () {
        if ($(this).hasClass('is-active')) {
          var selector = $(this).data('tab-content-selector');
          $(tab_contents_list).each(function (i) {
            if (tab_contents_list[i] == selector) {
              $(tab_contents_list[i]).show();
            } else {
              $(tab_contents_list[i]).hide();
            }
          });
        }
      });
    });
  }
}
$(function () {
  tab.ini();
});

/*------------------------------------------------
* チェックボックス全選択 ※FixedMidashi と連携あり
------------------------------------------------*/
var check_list_all = {
  'btn': '.js-check-all-btn',
  'input': '.js-check-all-input',
  'ini': function () {
    $(check_list_all.btn).each(function (i) {
      // FixedMidashi で要素がコピーされるのでユニークなクラスで紐付けできるようにする
      $(this).attr('data-check-all-input-index', i);
      var input = $(this).closest('.table-wrapper').find('input' + check_list_all.input);
      input.addClass('js-check-all-input-index-' + i);
    });
  },
  'event_set': function () {
    $(check_list_all.btn).each(function (i) {
      if ($(this).prop("tagName") == 'INPUT' && $(this).attr('type') == 'checkbox') {
        // チェックボックスの場合
        $(this).unbind('change').bind('change', function () {
          var check_all_input_index = $(this).data('check-all-input-index');
          if ($(this).prop('checked')) {
            $('.js-check-all-input-index-' + check_all_input_index).each(function () {
              $(this).prop('checked', true);
            });
          } else {
            $('.js-check-all-input-index-' + check_all_input_index).each(function () {
              $(this).prop('checked', false);
            });
          }
          // 一度だけchangeイベント発火
          $('.js-check-all-input-index-' + check_all_input_index + ':eq(0)').change();
          return false;
        });

      } else {
        // ボタンの場合
        $(this).unbind('click').bind('click', function () {
          var check_all_input_index = $(this).data('check-all-input-index');
          var flg_check = false;
          $('.js-check-all-input-index-' + check_all_input_index).each(function () {
            if ($(this).prop('checked')) {
              flg_check = true;
            }
          });
          $('.js-check-all-input-index-' + check_all_input_index).each(function () {
            if (flg_check) {
              $(this).prop('checked', false);
            } else {
              $(this).prop('checked', true);
            }
          });
          // 一度だけchangeイベント発火
          $('.js-check-all-input-index-' + check_all_input_index + ':eq(0)').change();
          return false;
        });
      }

    });
  }
}
$(function () {
  check_list_all.ini();
});

/*------------------------------------------------
* テーブル 行クリック時にチェックを入れる
------------------------------------------------*/
var table_row_check = {
  selector: '.js-list-table-wrapper',
  ini: function () {

    $(table_row_check.selector).each(function (i) {
      $(this).attr('data-list-table-index', i);
    });

    $(table_row_check.selector).find('tbody tr').each(function (i) {
      var input = $(this).find('input.js-check-all-input');
      var index = i;

      input.click(function (e) {
        // イベントバブリングキャンセル inputクリックでチェック状態変更できるように
        e.stopPropagation();
      });

      $(this).attr('data-tr-index', index);

      $(this).hover(function () {
        $(table_row_check.selector).find('[data-tr-index="' + index + '"]').addClass('is-hover');
      }, function () {
        $(table_row_check.selector).find('[data-tr-index="' + index + '"]').removeClass('is-hover');
      })

      $(this).click(function () {
        if ($(this).find('input.js-check-all-input').prop('checked')) {
          // チェックを外す
          input.prop('checked', false);
          // FixedMidashiで複製されたtableにチェック状態を同期する
          var table_index = $(this).closest(table_row_check.selector).data('list-table-index');
          $('[data-list-table-index="' + table_index + '"] [data-tr-index="' + index + '"] input.js-check-all-input').prop('checked', false);
        } else {
          // チェックを入れる
          input.prop('checked', true);
          // FixedMidashiで複製されたtableにチェック状態を同期する
          var table_index = $(this).closest(table_row_check.selector).data('list-table-index');
          $('[data-list-table-index="' + table_index + '"] [data-tr-index="' + index + '"] input.js-check-all-input').prop('checked', true);
        }
        input.change();
      });

    });
  }
}
$(function () {
  table_row_check.ini();
});

/*------------------------------------------------
* テーブル スクロール 見出し固定処理
------------------------------------------------*/
$(window).on('load', function () {
  FixedMidashi.create();
  check_list_all.event_set();
});

/*------------------------------------------------
* loading animation
------------------------------------------------*/
var loading_animation = {
  'start': function (msg) {
    if (jQuery('.loading-animation').length == 0) {
      var html = '<div class="loading-animation"><span class="loading-animation-circle"></span></div>';
      jQuery('body').append(html);
    }
    setTimeout(function () {
      jQuery('body').addClass('is-loading');
    }, 200);
  },
  'end': function () {
    jQuery('body').addClass('is-loaded').removeClass('is-loading');
    setTimeout(function () {
      jQuery('body').removeClass('is-loaded');
    }, 1000);
  }
}

/*------------------------------------------------
* もっと見るトグル toggle-more
------------------------------------------------*/
var toggle_more = {
  'ini': function () {
    $('.toggle-more').each(function (i) {
      var block = $(this);
      block.find('.toggle-more-btn').unbind('click').bind('click', function () {
        var cont = block.find('.toggle-more-content');
        if (cont.is(':visible')) {
          block.removeClass('toggle-more-is-show');
          cont.hide();
        } else {
          block.addClass('toggle-more-is-show');
          cont.show();
        }
        return false;
      });
    });
  }
}
$(function () {
  toggle_more.ini();
});

/*------------------------------------------------
* 日付選択カレンダー
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
        closeText: "閉じる",
        prevText: "&#x3C;前",
        nextText: "次&#x3E;",
        currentText: "今日",
        monthNames: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
        monthNamesShort: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
        dayNames: ["日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"],
        dayNamesShort: ["日", "月", "火", "水", "木", "金", "土"],
        dayNamesMin: ["日", "月", "火", "水", "木", "金", "土"],
        weekHeader: "週",
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: "年",
        firstDay: 0, // 週の初めは月曜
        showButtonPanel: true, // "今日"ボタン, "閉じる"ボタンを表示する
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
* ファイル選択
------------------------------------------------*/
var dd_file_select = {
  selected_classname: 'is-selected',
  no_selected_message: '選択されていません',
  ini: function () {

    $('.js-dd-file-select').each(function (i) {
      var _this = $(this);
      var input = _this.find('.dd-file-select-input');

      if (browser == 'ie') {
        // IEはドラッグ＆ドロップに標準で対応していないため個別処理
        $('body').unbind('dragover drop').bind('dragover drop', function (e) {
          e.preventDefault();
        });
        _this.bind('drop', function (e) {
          e.stopPropagation();
          e.preventDefault();

          if (e.originalEvent.dataTransfer.files.length != 0) {
            _this.removeClass('is-dragover');
            _this.find('.dd-file-select-filename').html(e.originalEvent.dataTransfer.files[0].name);
            _this.addClass(dd_file_select.selected_classname);
          } else {
            _this.find('.dd-file-select-filename').html(dd_file_select.no_selected_message);
            _this.removeClass(dd_file_select.selected_classname);
          }
        });
      }

      _this.bind('dragover', function (e) {
        _this.addClass('is-dragover');
      });

      _this.bind('dragleave', function (e) {
        _this.removeClass('is-dragover');
      });

      _this.find('.dd-file-select-btn').click(function () {
        _this.find('.dd-file-select-input').click();
      });

      _this.find('.dd-file-select-file-cancel').click(function () {
        _this.find('.dd-file-select-filename').html(dd_file_select.no_selected_message);
        _this.removeClass(dd_file_select.selected_classname);
        input.val('');
      });

      input.change(function (event) {
        if (event.target.files.length != 0) {
          _this.removeClass('is-dragover');
          _this.find('.dd-file-select-filename').html(event.target.files[0].name);
          _this.addClass(dd_file_select.selected_classname);
        } else {
          _this.find('.dd-file-select-filename').html(dd_file_select.no_selected_message);
          _this.removeClass(dd_file_select.selected_classname);
        }
      });
    });
  }
}

$(function () {
  dd_file_select.ini();
});

/*------------------------------------------------
* 文章量が多い場合に三点リーダーで省略し、オンマウスでツールチップ表示
------------------------------------------------*/
var line_clamp_popover = {
  ini: function () {
    line_clamp_popover.set();
    $(window).resize(function () {
      line_clamp_popover.set();
    });
  },
  set: function () {
    $('.js-line-clamp-popover').each(function (i) {
      if ($(this).attr('data-line-clamp-popover-ini') != '1') {
        var font_size = Number($(this).css('font-size').replace('px', ''));
        var height = $(this).height();
        if (font_size * 1.9 < height) {
          // 2行以上
          $(this).attr('title', $(this).text());
          // $(this).attr('data-popover-html','');
          // $(this).attr('data-popover-position','top');
          // $(this).after('<div class="popover-html-content">'+$(this).text()+'</div>');
          $(this).addClass('line-clamp');
          //IEのみ
          if (browser == 'ie') {
            $(this).css({
              'white-space': 'nowrap'
            });
          }
          $(this).attr('data-line-clamp-popover-ini', '1');
        }
      }
    });
    popover.ini();
  }
}

$(function () {
  line_clamp_popover.ini();
});

/*------------------------------------------------
* 上部アクションエリアの固定表示
------------------------------------------------*/
var mainContents = {
  fixedHeader: {
    selectorHeader: '.main-contents-fixed-header',
    isFixedClassname: 'is-main-contents-fixed-header-fixed',
    ini: function () {

      var fixedHeader = $(mainContents.fixedHeader.selectorHeader);

      // スクロール時の吸着
      var fixedHeaderPos = fixedHeader.offset().top;
      $(window).bind('scroll', function () {
        var st = $(window).scrollTop();
        var headerHeight = $('#header').outerHeight();
        var fixedHeaderHeight = fixedHeader.outerHeight();
        if (st + headerHeight > fixedHeaderPos) {
          $('body').addClass(mainContents.fixedHeader.isFixedClassname).css({
            'padding-top': fixedHeaderHeight,
          });
        } else {
          $('body').removeClass(mainContents.fixedHeader.isFixedClassname).css({
            'padding-top': '',
            'padding-bottom': ''
          });
        }
      });
    },
  }
}
$(function () {
  $(mainContents.fixedHeader.selectorHeader)[0] && mainContents.fixedHeader.ini();
});

/*------------------------------------------------
* すべての設定項目を表示ボタン
------------------------------------------------*/
var blockSectionToggle = {
  selectorSection: '.block-section',
  selectorBlock: '.block-section-body',
  selectorAllToggleTrigger: '.js-form-element-all-toggle-trigger',
  selectorToggleBtn: '.js-block-section-toggle-btn-a',
  selectorToggleBtnClose: '.js-block-section-toggle-btn-a-close',
  selectorAllShowBtn: '.js-block-section-all-show-btn-a',
  selectorHideElement: '.is-form-element-toggle',
  classnameShow: 'is-form-element-toggle-show',
  iniFlg: 'data-block-toggle-ini',
  btnHtml: '<div class="block-section-toggle-btn"><a href="javascript:void(0);" class="block-section-toggle-btn-a js-block-section-toggle-btn-a" data-popover-message="すべての設定項目を表示する" data-popover-message-position="top"><span class="icon-arrow-down"></span></a><a href="javascript:void(0);" class="block-section-toggle-btn-a block-section-toggle-btn-a-close js-block-section-toggle-btn-a-close" data-popover-message="表示する設定項目を減らす" data-popover-message-position="top"><span class="icon-arrow-down"></span></a></div>',
  ini: function () {
    _this = this;

    // すべてのトグル
    $(_this.selectorAllToggleTrigger).off('change').on('change', function () {
      if ($(this).prop("checked")) {
        // すべて表示
        _this.allShow();
      } else {
        // 閉じる
        _this.allHide();
      }
    });
    $(_this.selectorAllShowBtn).off('click').on('click', function () {
      // すべて表示
      _this.allShow();
    });

    // 個別ブロックのトグルボタン設置
    $(_this.selectorBlock).each(function () {
      if ($(this).find(_this.selectorHideElement).length > 0) {
        if ($(this).find('.block-section-toggle-btn').length == 0) {
          $(this).append(_this.btnHtml);
        }
      } else {
        $(this).find('.block-section-toggle-btn').css('border', '2px solid red');
        $(this).find('.block-section-toggle-btn').remove();
      }
    });
    popover.ini();

    // 初期表示状態
    // セクションブロック内の非表示化
    $(_this.selectorBlock).each(function () {
      if (!$(this).hasClass(_this.classnameShow)) {
        $(this).find(_this.selectorHideElement).each(function () {
          $(this).css({ 'display': 'none' });
        });
      }
    });
    // セクションブロック自体の非表示化
    $(_this.selectorSection + _this.selectorHideElement).each(function () {
      $(this).css({ 'display': 'none' });
    });

    // トグルボタンのイベントセット 開く
    $(_this.selectorToggleBtn).each(function () {
      var toggleBtn = $(this);
      toggleBtn.off('click').on('click', function () {
        // スクロール位置保持
        var st = $(window).scrollTop();
        // 開く
        toggleBtn.closest(_this.selectorBlock).addClass(_this.classnameShow);
        toggleBtn.closest(_this.selectorBlock).find(_this.selectorHideElement).each(function (i) {
          var toggleObj = $(this);
          toggleObj.css({ 'display': '' });
          checkDisplayLimitedFixedPurchaseKbn();
          $(window).scrollTop(st);
          toggleBtn.hide();
          toggleBtn.closest(_this.selectorBlock).find(_this.selectorToggleBtnClose).show();
        });
      });
    });

    // トグルボタンのイベントセット 閉じる
    $(_this.selectorToggleBtnClose).each(function () {
      var toggleBtn = $(this);
      toggleBtn.hide();
      toggleBtn.off('click').on('click', function () {
        // 閉じる
        toggleBtn.closest(_this.selectorBlock).removeClass(_this.classnameShow);
        toggleBtn.closest(_this.selectorBlock).find(_this.selectorHideElement).each(function (i) {
          var toggleObj = $(this);
          toggleObj.css({ 'display': 'none' });
        });
        toggleBtn.hide();
        toggleBtn.closest(_this.selectorBlock).find(_this.selectorToggleBtn).show();
      });
    });

    // 初期状態の反映
    $(blockSectionToggle.selectorAllToggleTrigger).change();

  },
  allShow: function () {
    _this = this;
    // すべて表示
    $(_this.selectorHideElement).css({ 'display': '' });
    $(_this.selectorToggleBtn).css({ 'display': 'none' });

    if (!$(_this.selectorAllToggleTrigger).prop("checked")) {
      $(_this.selectorAllToggleTrigger).prop("checked", true);
    }

    $(_this.selectorAllShowBtn).hide();
    $(_this.selectorToggleBtn).hide();
    $(_this.selectorToggleBtnClose).show();
    checkDisplayLimitedFixedPurchaseKbn();

  },
  allHide: function () {
    _this = this;
    // すべて閉じる
    $(_this.selectorHideElement).css({ 'display': 'none' }).removeClass(_this.classnameShow);
    $(_this.selectorToggleBtn).css({ 'display': '' });

    if ($(_this.selectorAllToggleTrigger).prop("checked")) {
      $(_this.selectorAllToggleTrigger).prop("checked", false);
    }

    $(_this.selectorAllShowBtn).show();
    $(_this.selectorToggleBtn).show();
    $(_this.selectorToggleBtnClose).hide();

  }
}
$(function () {
  blockSectionToggle.ini();
});

/*------------------------------------------------
* スライド型チェックボックス
------------------------------------------------*/
var slide_check = {
  ini: function () {
    $('.slide-checkbox-wrap input[type="checkbox"]').each(function () {
      slide_check.label_set(this);
      $(this).unbind('click').bind('click', function () {
        slide_check.label_set(this);
      });
    });
  },
  label_set: function (obj) {
    var onLabel = $(obj).attr('data-on-label');
    var offLabel = $(obj).attr('data-off-label');
    if ($(obj).prop('checked') == true) {
      $(obj).parent().find('label .slide-checkbox-label').text(onLabel);
    } else {
      $(obj).parent().find('label .slide-checkbox-label').text(offLabel);
    }
  }
}
$(function () {
  slide_check.ini();
});

/*------------------------------------------------
* 期間選択
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

      // クリアボタン
      startClear.on('click', function () {
        startWrapper.find('input').val('');
        startWrapper.find(_this.errorMessageContainer).hide();
      });
      endClear.on('click', function () {
        endWrapper.find('input').val('');
        endWrapper.find(_this.errorMessageContainer).hide();
      });
    });
  }
}
$(function () {
  selectPeriod.ini();
});

/**
 * 氏名（姓・名）の自動振り仮名変換を実行する（ひらがな用）
 * @param firstName 名
 * @param firstNameKana 名（ひらがな）
 * @param lastName 姓
 * @param lastNameKana 姓（ひらがな）
*/
function execAutoKanaHiragana(firstName, firstNameKana, lastName, lastNameKana) {
  $.fn.autoKana(firstName, firstNameKana);
  $.fn.autoKana(lastName, lastNameKana);
}

/**
 * 氏名（姓・名）の自動振り仮名変換を実行する（かたかな用）
 * @param firstName 名
 * @param firstNameKana 名（かたかな）
 * @param lastName 姓
 * @param lastNameKana 姓（かたかな）
*/
function execAutoKanaKatakana(firstName, firstNameKana, lastName, lastNameKana) {
  $.fn.autoKana(firstName, firstNameKana, { katakana: true });
  $.fn.autoKana(lastName, lastNameKana, { katakana: true });
}

/**
 * 氏名（姓・名）の自動振り仮名変換を実行する
 * @param firstName 名
 * @param firstNameKana 名（かな）
 * @param lastName 姓
 * @param lastNameKana 姓（かな）
 * @param kanaType ひらがなorかたかな
*/
function execAutoKana(firstName, firstNameKana, lastName, lastNameKana, kanaType) {
  // 振り仮名種別がひらがなの場合、ひらがな変換を行う。カタカナの場合、カタカナ変換を行う
  if (kanaType == 'FULLWIDTH_HIRAGANA') {
    execAutoKanaHiragana(firstName, firstNameKana, lastName, lastNameKana);
  } else if (kanaType == 'FULLWIDTH_KATAKANA') {
    execAutoKanaKatakana(firstName, firstNameKana, lastName, lastNameKana);
  }
}

function unlockScreen( resResult, isConnection) {
    if (resResult === false) {
      if (isConnection === true) {
        alert('カードの認証に失敗しました。カード情報をご確認ください。');
      } else {
        alert('決済端末に接続できません。再度お試しください。');
      }
    }
    else {
      if (isConnection === true) {
        alert('カードの認証に成功しました。');
      } else {
        alert('決済端末に接続できません。再度お試しください。');
      }
     }

  let screenLock = document.getElementById("screenLock");
  screenLock.parentNode.removeChild(screenLock);
  $('#payTgModal').hide();
}
function lockScreen() {
  let lock_screen = document.createElement('div');
  lock_screen.id = "screenLock";

  lock_screen.style.height = '100%';
  lock_screen.style.left = '0px';
  lock_screen.style.position = 'fixed';
  lock_screen.style.top = '0px';
  lock_screen.style.width = '100%';
  lock_screen.style.zIndex = '9999';
  lock_screen.style.opacity = '0';

  let objBody = document.getElementsByTagName("body").item(0);
  objBody.appendChild(lock_screen);
  $('#payTgModal').show();
}

/*------------------------------------------------
* リスト入力
------------------------------------------------*/
var customlistInput = {
  wrapperSelector: '.custom-list-input',
  hiddenInputSelector: '.custom-list-input-hidden-input',
  ini: function () {
    var _this = this;
    $(_this.wrapperSelector).each(function () {
      var wrapper = $(this);
      var hiddenInput = wrapper.find(_this.hiddenInputSelector);
      var itemHtml = function (val) {
        return '<div class="custom-list-input-list-item"><input type="text" name="" class="custom-list-input-list-item-input" value="' + val + '" placeholder="値を入力してください"><div class="custom-list-input-list-item-input-btns"><a href="javascript:void(0);" class="custom-list-input-list-item-input-submit">決定</a><a href="javascript:void(0);" class="custom-list-input-list-item-input-up"><span class="icon-arrow-up2"></span></a><a href="javascript:void(0);" class="custom-list-input-list-item-input-down"><span class="icon-arrow-down2"></span></a><a href="javascript:void(0);" class="custom-list-input-list-item-input-delete"><span class="icon-close"></span></a></div></div>';
      }
      var listHtml = '<div class="custom-list-input-list">';
      hiddenInput.find('option').each(function () {
        listHtml += itemHtml($(this).val());
      });
      listHtml += itemHtml('');
      listHtml += '</div>';
      wrapper.append(listHtml);

      // リスト数格納
      var listLength = function () {
        return wrapper.find('.custom-list-input-list-item').length;
      }

      // 送信用データアップデート
      var hiddenDataUpdate = function () {
        hiddenInput.find('option').remove();
        wrapper.find('.custom-list-input-list-item-input').each(function () {
          if ($(this).val() != '') {
            hiddenInput.append('<option value="' + $(this).val() + '">' + $(this).val() + '</option>');
          }
        });
      }
      hiddenDataUpdate();

      // 各種イベントセット
      var eventSet = function () {
        wrapper.find('.custom-list-input-list-item').each(function (i) {
          // エンターキープレス
          $(this).find('.custom-list-input-list-item-input').off('keypress').on('keypress', function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
              _submit(i);
            }
          });
          $(this).find('.custom-list-input-list-item-input').off('blur').on('blur', function (event) {
            _submit(i);
          });

          $(this).find('.custom-list-input-list-item-input-submit').off('click').on('click', function () {
            _submit(i);
          })
          $(this).find('.custom-list-input-list-item-input-up').off('click').on('click', function () {
            _up(i);
          })
          $(this).find('.custom-list-input-list-item-input-down').off('click').on('click', function () {
            _down(i);
          })
          $(this).find('.custom-list-input-list-item-input-delete').off('click').on('click', function () {
            _delete(i);
          })
        });
      }
      eventSet();

      var _submit = function (index) {

        // 入力値が空の場合は処理しない
        if (wrapper.find('.custom-list-input-list-item-input:eq(' + index + ')').val() != '') {
          var nextIndex = index + 1;
          // 最後のinputであれば一行追加
          if (listLength() == nextIndex) {
            wrapper.find('.custom-list-input-list').append(itemHtml(''));
          }
          eventSet();

          var nextInput = wrapper.find('.custom-list-input-list-item-input:eq(' + nextIndex + ')');
          nextInput.focus();
          nextInput[0].setSelectionRange(nextInput.val().length, nextInput.val().length);

        }
        hiddenDataUpdate();

      }
      var _up = function (index) {
        if (index != 0) {
          var st = $(window).scrollTop();
          wrapper.find('.custom-list-input-list-item:eq(' + (index - 1) + ')').insertAfter(wrapper.find('.custom-list-input-list-item:eq(' + index + ')'));
          $('body,html').animate({
            scrollTop: st
          }, 0);
          eventSet();
          hiddenDataUpdate();
        }
      }
      var _down = function (index) {
        if (index != listLength()) {
          var st = $(window).scrollTop();
          wrapper.find('.custom-list-input-list-item:eq(' + index + ')').insertAfter(wrapper.find('.custom-list-input-list-item:eq(' + (index + 1) + ')'));
          $('body,html').animate({
            scrollTop: st
          }, 0);
          eventSet();
          hiddenDataUpdate();
        }
      }
      var _delete = function (index) {
        if (listLength() > 1) {
          wrapper.find('.custom-list-input-list-item:eq(' + index + ')').remove();
        }
        eventSet();
        hiddenDataUpdate();
      }
    });
  }
}

/*------------------------------------------------
* リスト入力(価格)
------------------------------------------------*/
var customlistInputPrice = {
  wrapperSelector: '.custom-list-input-price',
  hiddenInputSelector: '.custom-list-input-hidden-input',
  ini: function () {
    var _this = this;
    $(_this.wrapperSelector).each(function () {
      var wrapper = $(this);
      var hiddenInput = wrapper.find(_this.hiddenInputSelector);
      var itemHtml = function (val) {
        var first = "";
        var second = "";
        if (val.includes("{{")) {
          var first = val.substring(0, val.indexOf("{{"));
          var second = val.substring(val.indexOf("{{") + 2, val.indexOf("}}"));
          var second = second.replace(",", "");
        } else {
          first = val;
        }
        return '<div class="parent-price"><div class="child-price" style="width:8.5em; padding: 0 10px 0 0;">設定値 &nbsp;&nbsp;&nbsp;</div><div class="smallChild" style="width:400px"> <input placeholder="" value="' + first + '" size="26.5" type="text" class="firstControl">&nbsp;&nbsp;&nbsp;価格<input placeholder="" maxlength="7" value="' + second + '" type="text" class="secondControl"></div><div class="btnBackground"><div class="custom-list-input-list-item-input-btns"><a href="javascript:void(0);" class="custom-list-input-list-item-input-submit">決定</a><a href="javascript:void(0);" class="custom-list-input-list-item-input-up"><span class="icon-arrow-up2"></span></a><a href="javascript:void(0);" class="custom-list-input-list-item-input-down"><span class="icon-arrow-down2"></span></a><a href="javascript:void(0);" class="custom-list-input-list-item-input-delete"><span class="icon-close"></span></a></div></div></div>';
      }
      var listHtml = '<div class="custom-list-input-list">';
      hiddenInput.find('option').each(function () {
        listHtml += itemHtml($(this).val());
      });
      listHtml += itemHtml('');
      listHtml += '</div>';
      wrapper.append(listHtml);
      // リスト数格納
      var listLength = function () {
        return wrapper.find('.parent-price').length;
      }
      // 送信用データアップデート
      var hiddenDataUpdate = function () {
        hiddenInput.find('option').remove();
        var i = 0;
        var valuefirst = [];
        var valueSecond = [];
        wrapper.find('.firstControl').each(function () {
          if ($(this).val() != '') {
            valuefirst[i] = $(this).val();
            i++;
          }
        });
        i = 0;
        wrapper.find('.secondControl').each(function () {
          if ($(this).val() != '') {
            valueSecond[i] = $(this).val();
            i++;
          }
        });
        for (let i = 0; i < valuefirst.length; i++) {
          hiddenInput.append('<option value="' + valuefirst[i] + '{{' + valueSecond[i] + '}}' + '">' + valuefirst[i] + '{{' + valueSecond[i] + '}}' + '</option>');
        }
      }
      hiddenDataUpdate();

      // 各種イベントセット
      var eventSet = function () {
        wrapper.find('.parent-price').each(function (i) {
          // エンターキープレス
          $(this).find('.secondControl').off('keypress').on('keypress', function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
              _submit(i);
            }
          });
          $(this).find('.firstControl').off('blur').on('blur', function (event) {
            _submit(i);
          });
          $(this).find('.secondControl').off('blur').on('blur', function (event) {
            _submit(i);
          });
          $(this).find('.custom-list-input-list-item-input-submit').off('click').on('click', function () {
            _submit(i);
          })
          $(this).find('.custom-list-input-list-item-input-up').off('click').on('click', function () {
            _up(i);
          })
          $(this).find('.custom-list-input-list-item-input-down').off('click').on('click', function () {
            _down(i);
          })
          $(this).find('.custom-list-input-list-item-input-delete').off('click').on('click', function () {
            _delete(i);
          })
        });
      }
      eventSet();

      var _submit = function (index) {

        // 入力値が空の場合は処理しない
        if (wrapper.find('.firstControl:eq(' + index + ')').val() != '' && wrapper.find('.secondControl:eq(' + index + ')').val() != '') {
          var nextIndex = index + 1;
          // 最後のinputであれば一行追加
          if (listLength() == nextIndex) {
            wrapper.find('.custom-list-input-list').append(itemHtml(''));
          }

          var nextInput = wrapper.find('.firstControl:eq(' + nextIndex + ')');
          nextInput.focus();
          nextInput[0].setSelectionRange(nextInput.val().length, nextInput.val().length);
        }
        eventSet();
        hiddenDataUpdate();
      }
      var _up = function (index) {
        if (index != 0) {
          var st = $(window).scrollTop();
          wrapper.find('.parent-price:eq(' + (index - 1) + ')').insertAfter(wrapper.find('.parent-price:eq(' + index + ')'));
          $('body,html').animate({
            scrollTop: st
          }, 0);
          eventSet();
          hiddenDataUpdate();
        }
      }
      var _down = function (index) {
        if (index != listLength()) {
          var st = $(window).scrollTop();
          wrapper.find('.parent-price:eq(' + index + ')').insertAfter(wrapper.find('.parent-price:eq(' + (index + 1) + ')'));
          $('body,html').animate({
            scrollTop: st
          }, 0);
          eventSet();
          hiddenDataUpdate();
        }
      }
      var _delete = function (index) {
        if (listLength() > 1) {
          wrapper.find('.parent-price:eq(' + index + ')').remove();
        }
        eventSet();
        hiddenDataUpdate();
      }
    });
  }
}

/*------------------------------------------------
* カスタムカテゴリ選択
------------------------------------------------*/
var customCategorySelector = {
  wrapperSelector: '.custom-category-selector',
  hiddenInputSelector: '.custom-category-selector-hidden',
  suggestListSelector: 'suggest-list',
  ini: function (option) {
    var _this = this;
    if (typeof option === 'undefined') option = '';
    var optionWrappers = $(_this.wrapperSelector + option);
    optionWrappers.each(function () {
      var wrapper = $(this);
      wrapper.find('.custom-category-selector-input').remove();
      var hiddenInput = wrapper.find(_this.hiddenInputSelector);
      var suggestDataList = eval(wrapper.data(_this.suggestListSelector));
      if (suggestDataList === undefined) suggestDataList = [];

      // 初期HTML生成
      var selectedItemHtml = '';
      var selectedItem = 0;
      hiddenInput.each(function () {
        var val = $(this).val();
        if (val != '') {
          suggestDataList.forEach(function (element) {
            if (element.id == val) {
              selectedItemHtml +=
                '<div class="custom-category-selector-input-selected-item" data-id="' + element.id + '">\
									<span class="custom-category-selector-input-selected-item-label"><span class="custom-category-selector-input-selected-item-label-name">'+ element.label + '</span><span class="custom-category-selector-input-selected-item-label-id">（' + element.id + '）</span></span>\
									<span class="custom-category-selector-input-selected-item-delete"><span class="icon-close"></span></span>\
								</div>';
              selectedItem++;
              return false;
            }
          });
        }
      });

      var suggestListHtml = '';
      suggestDataList.forEach(function (element) {
        suggestListHtml +=
          '<div class="custom-category-selector-input-suggest-list-item" style="margin-left: ' + element.indent + 'px;" data-id="' + element.id + '" data-match-value="' + element.id + element.label + '" title="' + element.label + '(' + element.id + ')">' + element.label + '<span class="custom-category-selector-input-suggest-list-item-id">（' + element.id + '）</span></div>';
      });

      var html =
        '<div class="custom-category-selector-input">\
					<div class="custom-category-selector-input-selected-items">'+ selectedItemHtml + '</div>\
					<div class="custom-category-selector-input-suggest">\
						<input type="text" class="custom-category-selector-input-suggest-search" placeholder="選択する">\
						<div class="custom-category-selector-input-suggest-list">'+ suggestListHtml + '</div>\
					</div>\
				</div>\
				<div class="product-error-message-container" data-id="' + wrapper.data('field-name') + '"></div>';
      wrapper.append(html);

      var searchInput = wrapper.find('.custom-category-selector-input-suggest-search');
      var suggestList = wrapper.find('.custom-category-selector-input-suggest-list');
      var focusTimer = null;

      // 各種イベントセット
      var eventSet = function () {
        // サジェスト動作
        searchInput.off('focus').on('focus', function () {
          suggestList.show();
          clearTimeout(focusTimer);
        });
        searchInput.off('blur').on('blur', function () {
          focusTimer = setTimeout(function () {
            suggestList.hide();
          }, 1000);
        });
        searchInput.off('keyup change').on('keyup change', function () {
          if (searchInput.val() != '') {
            var pattern = new RegExp(escapeRegExp(searchInput.val()), 'gi');
            // マッチするものを表示する
            suggestList.find('.custom-category-selector-input-suggest-list-item').each(function () {
              if ($(this).data('match-value').search(pattern) != -1) {
                $(this).show();
              } else {
                $(this).hide();
              }
            });
          } else {
            suggestList.find('.custom-category-selector-input-suggest-list-item').show();
          }
          suggestList.show();
        });

        // 確定処理
        searchInput.off('keypress').on('keypress', function (event) {
          var keycode = (event.keyCode ? event.keyCode : event.which);
          if (keycode == '13') {
            if (suggestList.find('.custom-category-selector-input-suggest-list-item:visible').length == 1) {
              var targetId = suggestList.find('.custom-category-selector-input-suggest-list-item:visible').data('id');
              suggestList.find('.custom-category-selector-input-suggest-list-item').show();
              _submit(targetId);
            }
          }
        });

        // サジェストリストクリック
        wrapper.find('.custom-category-selector-input-suggest-list-item').each(function (i) {
          $(this).off('click').on('click', function () {
            _submit($(this).data('id'));
          });
        })

        // 削除処理
        wrapper.find('.custom-category-selector-input-selected-item-delete').each(function () {
          $(this).off('click').on('click', function () {
            $(this).closest('.custom-category-selector-input-selected-item').remove();
            hiddenDataSet();
          });
        })
      }
      eventSet();

      var _submit = function (targetId) {
        suggestDataList.forEach(function (element) {
          if (element.id == targetId) {
            var html =
              '<div class="custom-category-selector-input-selected-item" data-id="' + element.id + '">\
								<span class="custom-category-selector-input-selected-item-label"><span class="custom-category-selector-input-selected-item-label-name">'+ element.label + '</span><span class="custom-category-selector-input-selected-item-label-id">（' + element.id + '）</span></span>\
								<span class="custom-category-selector-input-selected-item-delete"><span class="icon-close"></span></span>\
							</div>';
            wrapper.find('.custom-category-selector-input-selected-items').append(html);
            searchInput.val('');
            eventSet();
            suggestList.hide();
            return false;
          }
        });
        hiddenDataSet();
      }

      var hiddenDataSet = function () {
        $(hiddenInput).val('');
        wrapper.find('.custom-category-selector-input-selected-item').each(function (i) {
          $(hiddenInput[i]).val($(this).data('id'));
        });
        // 最大値まで登録されているかチェック
        if ($(hiddenInput).last().val() != '') {
          wrapper.find('.custom-category-selector-input-suggest').hide();
        } else {
          wrapper.find('.custom-category-selector-input-suggest').show();
        }
      }
      hiddenDataSet();
    });
  }
}

// Escape RegExp
function escapeRegExp(string) {
  // $& means the whole matched string
  return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}
/*------------------------------------------------
* 数値入力
------------------------------------------------*/
var countInput = {
  wrapperSelector: '.count-input',
  btnUpSelector: '.count-input-btn-up',
  btnDownSelector: '.count-input-btn-down',
  inputSelector: '.count-input-input',
  ini: function () {
    var _this = this;
    $(_this.wrapperSelector).each(function () {
      var wrapper = $(this);
      var input = wrapper.find(_this.inputSelector);
      var btnUp = wrapper.find(_this.btnUpSelector);
      var btnDown = wrapper.find(_this.btnDownSelector);
      btnUp.on('click', function () {
        input.val(Number(input.val()) + 1);
        input.change(); // changeイベントを意図的に起こす
      });
      btnDown.on('click', function () {
        input.val(Number(input.val()) - 1);
        input.change(); // changeイベントを意図的に起こす
      });
    });
  }
}
$(function () {
  countInput.ini();
});

/*------------------------------------------------
* notification
------------------------------------------------*/
var notification = {
  ini: function () {
    $('body').append('<div class="notification"></div>');
  },

  show: function (message, classname, type) {
    var id = Math.random().toString(36).slice(-8);
    $('.notification').append('<div class="notification-message notification-message-' + classname + '" id="notification-id-' + id + '">' + message + '<span class="notification-message-close"><span class="icon-close"></span></div></div>');
    setTimeout(function () {
      $('#notification-id-' + id).addClass('show');
    }, 200);
    if (type == 'fadeout') {
      setTimeout(function () {
        notification.hide(id);
      }, 5000);
    }

    $('#notification-id-' + id + ' .notification-message-close').off('click').on('click', function () {
      notification.hide(id);
    });
  },

  hide: function (id) {
    $('#notification-id-' + id + '').remove();
  }
}
$(function () {
  notification.ini();
});

function getMobileOperatingSystem() {
  var userAgent = navigator.userAgent || navigator.vendor || window.opera;

  // Windows Phone must come first because its UA also contains "Android"
  if (/windows phone/i.test(userAgent)) {
    return true;
  }

  // Android
  if (/android/i.test(userAgent)) {
    return true;
  }

  // iOS
  if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
    return true;
  }

  return false;
}

// Is Json object
function isJsonObject(str) {
  try {
    JSON.parse(str);
  } catch (e) {
    return false;
  }
  return true;
}

// Send ajax request with post form data
function sendAjaxRequestWithPostFormData(url, formData) {
  var request = $.ajax({
    type: "POST",
    statusCode: {
      401: function () {
        // Reload page when login session expired
        window.location.reload();
      }
    },
    url: url,
    data: formData,
    contentType: false,
    processData: false,
    error: pageReload
  });
  return request;
}

// Check session timeout for call ajax
function checkSessionTimeoutForCallAjax(xmlHttpRequest) {
  // Use only for requests and responses with a content type other than text/html
  if (xmlHttpRequest.getResponseHeader("Content-Type").indexOf('text/html') !== -1) {
    window.location.reload();
  }
}

// Send ajax request write log error messages
function sendAjaxRequestWriteLogErrorMessages(url, statusCode, responseText) {
  // Create error content
  var errorContent = '\r\n -> ';
  if ((statusCode >= 500) && isJsonObject(responseText)) {
    var exceptionObject = JSON.parse(responseText);
    errorContent += (exceptionObject.Message + '\r\n' + exceptionObject.StackTrace);
  } else if (statusCode == 401) {
    // Reload page when login session expired
    window.location.reload();
    return;
  } else {
    errorContent += responseText;
  }

  // Send action write log error
  $.ajax({
    type: "POST",
    url: url,
    data: JSON.stringify({
      errorContent: errorContent
    }),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
  });
}

// Iterator for array
var iterator = function (arr, n) {
	var current = 0;
	var length = arr.length;
	return function () {
		end = current + n;
		var part = arr.slice(current, end);
		current = (end < length) ? end : 0;
		return part;
	};
};
