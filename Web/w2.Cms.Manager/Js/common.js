var cmn = {
  "cookie_sidemenu_expires": 7,
  "cookie_sidemenu_key": "w2cms_sidemenu",
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
$(function() {

  //DOM解析完了までコンテンツを隠す
  $('.content').css({
    'opacity': 1
  });

  //ウインドウの幅と高さを格納
  $(window).resize(function() {
    cmn.wh = window.innerHeight;
    cmn.ww = window.innerWidth;
  });

  //スクロールが200に達したらscroll付与
  var header_toggle = function() {
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
  $('.sidemenu-child-list .sidemenu-list a').each(function() {
    if ($(this).text() == title) {
      $(this).closest('.sidemenu-list').addClass('current');
    }
  });

  // ページトップへ戻るボタン追加
  $(".main-contents-inner").append('<div class="btn_pagetop"><a href="javascript:void(0)"></a></div>');
  $(".btn_pagetop").click(function() {
    $('body,html').animate({
      scrollTop: 0
    }, 500);
  });

  var toggle_spped = 300;
  var toggle_open_class = "open";


  // btn sidemenu toggle

  var sidemenu_restore = function () {
    var cookie_sidemenu_data = getCookie(cmn.cookie_sidemenu_key);
    if (cookie_sidemenu_data) {
      $(cookie_sidemenu_data.split('')).each(function(i) {

        if (i == 0) {
          if ($(this)[0][0] == "1") {
            $('body').addClass("animation_none").addClass(cmn.toggle_slim_class);
            setTimeout(function() {
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

  var sidemenu_save_state = function() {
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

  $('.sidemenu > .sidemenu-list').each(function() {
    var this_ = $(this);
    var tgt = this_.find('.sidemenu-child-list');
    $(this).find('> .sidemenu-list-label > a:eq(0)').click(function() {
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
  var classname_sidemenu_all_open = "sidemenu_all_open";
  var sidemenu_toggle_status_check = function() {
    var status = true; // all open
    $('.sidemenu .simplebar-content > .sidemenu-list').each(function (i) {
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

  $('.btn-alltoggle-open').click(function() {
    $('.sidemenu-child-list').each(function() {
      $(this).slideDown(toggle_spped);
      $(this).closest('.sidemenu-list').addClass(toggle_open_class);
    });
    $('body').addClass(classname_sidemenu_all_open);
    sidemenu_save_state();
  });
  $('.btn-alltoggle-close').click(function() {
    $('.sidemenu-child-list').each(function() {
      $(this).slideUp(toggle_spped);
      $(this).closest('.sidemenu-list').removeClass(toggle_open_class);
    });
    $('body').removeClass(classname_sidemenu_all_open);
    sidemenu_save_state();
  });


  //btn sidemenu slim toggle_sppedbtn-slimtoggle-close

  $('.btn-slimtoggle-on').click(function() {
    $('body').addClass(cmn.toggle_slim_class);
    sidemenu_save_state();
  });

  $('.btn-slimtoggle-off').click(function() {
    $('body').removeClass(cmn.toggle_slim_class);
    sidemenu_save_state();
    $('.sidemenu').css({
      "position": "",
      "top": ""
    });

  });

  // sidemenu slim popmenu
  $('.sidemenu > .sidemenu-list').each(function(i) {
    var timer;
    $(this).hover(function(e) {
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
    }, function() {
      var tgt = $(this).find('.sidemenu-child-list');
      tgt.removeClass('popmenu-open');
      tgt.css({
        "top": ""
      });
    });

  });

  var sm = $(".sidemenu");

  var sm_scroll_function = function() {
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


  var scroll_functions = function() {
    header_toggle();
    sm_scroll_function();
  };

  //scrollイベント負荷低減
  var w_scroll = false;
  var event_interval_time = 50;
  $(window).scroll(function() {
    if (!w_scroll) {
      w_scroll = true;
      setTimeout(function() {
        w_scroll = false;
        scroll_functions();
      }, event_interval_time);
    }
  });

});


/*------------------------------------------------
* ポップアップメッセージ
------------------------------------------------*/
var popover = {
  wait_time : 0,
  show_classname : 'show',
  ini : function(){
    $('[data-popover-message]').each(function(i) {
      var this_ = $(this);
      var content = this_.data('popover-message');
      var popover_timer;
      this_.hover(function(e) {
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
        popover_timer = setTimeout(function() {
          $('.popover_' + i).addClass(popover.show_classname);
        }, popover.wait_time);
      }, function() {
        clearTimeout(popover_timer);
        $('.popover_' + i).removeClass(popover.show_classname);
        $('.popover_' + i).remove();
      });
    });
  }
}
$(function() {
  popover.ini();
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
  ini: function() {
    //
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

    //btn open trigger
    jQuery('.btn-modal-open').each(function() {
      jQuery(this).off('click').on('click', function() {
        modal.open(jQuery(this).data('modal-selector'), jQuery(this).data('modal-classname'), jQuery(this).data('modal-callback-opened'));
      });
    });

    //btn close trigger
    jQuery('.js-modal-close-btn').each(function() {
      jQuery(this).off('click').on('click', function() {
        modal.close();
      });
    });
  },
  open: function(selector, classname, openedfunc) {

    //背景を固定する
    jQuery('body').css({
      'overflow': 'hidden'
    });

    //modal.tgt_btn = jQuery(this);
    var time_delay = 0;
    if (modal.flg_open) {
      //モーダルが開いていたら
      //一旦閉じるアニメーション実行
      modal.close();
      time_delay = 500;

      //既に開いているモーダルがあったらtrueに
      modal.flg_before_modal = true;

    } else {

      //既に開いているモーダルがなかったらfalseに
      modal.flg_before_modal = false;

      //モーダルを開いた状態でモーダルを開く場合に、直前のモーダルのセレクターを格納しておく
      modal.tgt_selector_last = [selector, classname, openedfunc];

    }


    setTimeout(function() {

      //カスタムクラスをセット
      jQuery(modal.selector_modal).removeClass(modal.tgt_classname);
      jQuery(modal.selector_modal).addClass(classname);

      modal.tgt_classname = classname;

      //直前のセレクタと違う場合
      var flg_tgt_selector_diff = false; //同じ場合はfalse 、違う場合はtrue
      if (modal.tgt_selector != selector) {
        flg_tgt_selector_diff = true;
      }

      //新しいセレクターの格納
      modal.tgt_selector = selector;

      //既に格納されていたDOM位置を元に戻す
      modal.return_content();

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
      new SimpleBar(jQuery(modal.selector_modal + ' .modal-content')[0]);

      tab.ini();
      access_authority_setting.ini();

    }, time_delay);


  },
  close: function() {

    //背景固定を開放する
    jQuery('body').css({
      'overflow': ''
    });

    jQuery(modal.selector_modal).removeClass(modal.open_class_name);

    jQuery(modal.selector_modal).hide();

    setTimeout(function() {
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
    if (modal.flg_before_modal) {
      setTimeout(function () {
        modal.open(modal.tgt_selector_last[0], modal.tgt_selector_last[1], modal.tgt_selector_last[2]);
        modal.flg_before_modal = false;
      }, 500)
    };
  },
  return_content: function() {
    //DOM位置を元に戻す
    jQuery('.js-modal-content-original-position').before(jQuery(modal.tgt_selector));
    jQuery('.js-modal-content-original-position').remove();
    //モーダル子要素に残っているDOMの削除
    jQuery(modal.selector_modal + " .modal-inner " + modal.tgt_selector).children().remove();
  }
}
jQuery(function() {
  modal.ini();
});

var news_modal = {
  "btn_selector": ".btn-news-modal-open",
  "modal_selector": "modal-selector",
  "ini": function () {
    jQuery(news_modal.btn_selector).each(function () {
      jQuery(this).click(function () {
        var selector = jQuery(this).data(news_modal.modal_selector);
        news_modal.open(selector);
      });
    });
  },
  "open": function (selector) {
    var content_ = jQuery(selector).html();
    jQuery("body").append('<div class="news-modal"><div class="news-modal-bg" onclick="news_modal.close();"></div><div class="news-modal-btn-close" onclick="news_modal.close();"></div><div class="news-modal-content-wrapper">' + content_ + '</div></div>');
    setTimeout(function () {
      jQuery(".news-modal").addClass('show');
    }, 1);
  },
  "close": function () {
    jQuery(".news-modal").removeClass("show");
    setTimeout(function () {
      jQuery(".news-modal").remove();
    }, 300);
  }
};
jQuery(function() {
  news_modal.ini();
});

/*------------------------------------------------
* 汎用 ドロップダウンメニュー
------------------------------------------------*/
var dropdown_toggle = {
  ini : function(){
    $('.dropdown-toggle').each(function() {
      var btn = $(this);
      var clone_tgt = null;
      btn.off('focus').on('focus', function() {
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
      btn.off('focusout').on('focusout', function() {
        var tgt = null;
        var margin = 3;
        setTimeout(function() {
          btn.removeClass('is-dropdown-open');
          clone_tgt.remove();
        }, 300);
      });
      btn.off('click').on('click', function (event) {
        event.stopPropagation();
      });
    });
  }
};
$(function() {
  dropdown_toggle.ini();
});


/*------------------------------------------------
* 汎用 トグル
------------------------------------------------*/
var toggle = {
  toggle_classname : 'is-toggle-open',
  ini : function(){

    //ボタン形式
    $(".btn-toggle").each(function() {
      var toggle_trigger = $(this);
      var toggle_content_selector = toggle_trigger.data("toggle-content-selector");
      var toggle_content = null;
      if (toggle_content_selector) {
        toggle_content = $(toggle_content_selector);
      } else {
        toggle_content = toggle_trigger.next();
      }
      toggle_trigger.find(".toggle-state-icon.icon-arrow-down").remove();
      toggle_trigger.append('<span class="toggle-state-icon icon-arrow-down"></span>');
      toggle_trigger.off("click").on("click", function() {
        if (toggle_content.is(':visible')) {
          toggle_content.slideUp();
          toggle_trigger.removeClass(toggle.toggle_classname);
          $('[data-toggle-content-selector="' + toggle_content_selector + '"]').removeClass(toggle.toggle_classname);
        } else {
          toggle_content.slideDown();
          toggle_trigger.addClass(toggle.toggle_classname);
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
    $(".js-toggle-form").each(function() {
      var toggle_trigger = $(this);
      var toggle_content_selector = toggle_trigger.data("toggle-content-selector");
      var toggle_content = $(toggle_content_selector);

      var set2 = function(){
        if (toggle_trigger.prop('checked') == true) {
          toggle_content.slideDown();
        } else {
          toggle_content.slideUp();
        }
      }

      toggle_trigger.off("change").on("change", function() {
        set2();
      });

      set2();

    });

    //フォーム要素形式 選択されたら開く チェックボックス用
    $(".js-toggle-form-radio").each(function() {
      var toggle_trigger = $(this);
      var toggle_target_name = toggle_trigger.attr('name');

      var set = function(time){
        var animation_time = 500;
        if(time == 0){
          animation_time = time;
        }
        $('[name="' + toggle_target_name + '"]').each(function() {
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

      $('[name="' + toggle_target_name + '"]').each(function() {
        $(this).off("change").on("change", function() {
          set();
        });
      });

      set(0);

    });
  }
}
$(function() {
  toggle.ini();
});


/*------------------------------------------------
* 一覧 グループ トグル
------------------------------------------------*/
var list_group_toggle = {
  toggle_classname : 'is-toggle-open',
  ini: function () {
    $(".list-group-toggle-btn").each(function() {
      var toggle_trigger = $(this);
      var toggle_content = toggle_trigger.closest('.list-group').find('.list-group-toggle-content');
      toggle_trigger.find(".toggle-state-icon.icon-arrow-down").remove();
      toggle_trigger.append('<span class="toggle-state-icon icon-arrow-down"></span>');
      toggle_trigger.parent().off("click").on("click", function () {
        if (toggle_content.is(':visible')) {
          toggle_content.slideUp();
          toggle_trigger.removeClass(list_group_toggle.toggle_classname);
        } else {
          toggle_content.slideDown();
          toggle_trigger.addClass(list_group_toggle.toggle_classname);
        }
      });
      if (toggle_content.is(':visible')) {
        toggle_trigger.addClass(list_group_toggle.toggle_classname);
      }
    });
  }
}
$(function() {
  list_group_toggle.ini();
});

/*------------------------------------------------
* カスタムスクロールバー
------------------------------------------------*/
var custom_scroll = {
  ini: function () {
    if ($('.custom-scroll-area').length != 0) {
      $('.custom-scroll-area').each(function() {
        var simplebar = new SimpleBar($(this)[0]);
        var simplebar_elem = simplebar.getScrollElement();
        $(simplebar_elem).off('scroll').on('scroll', function() {
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
      $(simplebar_main_content_detail_body_elem).off('scroll').on('scroll', function() {
        var st = $(this).scrollTop();
        if (st > 0) {
          $(simplebar_main_content_detail_body_elem).closest('.main-content-detail-body').addClass('simplebar-is-scroll');
        } else {
          $(simplebar_main_content_detail_body_elem).closest('.main-content-detail-body').removeClass('simplebar-is-scroll');
        }
      });
    }
    
    if ($('.layout-edit-basic-layout-select').length != 0) {
      $('.layout-edit-basic-layout-select').each(function(){
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
$(function() {
  custom_scroll.ini();
});


/*------------------------------------------------
* 各エリアの高さ制御
------------------------------------------------*/
var height_fit = {
  ini : function(){
    //一覧、編集画面
    var wh = $(window).height();
    if(wh < 600){
      wh = 600;
    }
    $('.main-content-frame .list-content, .main-content-frame .main-content-detail').each(function() {
      var margin = 35;
      var h = wh - $(this).offset().top - margin;
      var min_height = 300;
      $(this).height(h);
    });
    $('.main-contents .main-content-detail-body').each(function() {
      var margin = 35;
      var h = wh - $(this).offset().top - margin;
      $(this).outerHeight(h);
    });

    //レイアウト編集エリア
    $('.layout-edit').each(function () {
      var isPc = $('#tab_pc > a').hasClass('is-active');
      if (isPc) {
      	$(this).find('.layout-edit-page').height($('.pc').outerHeight() + $('.layout-edit-basic-layout-select').outerHeight() + 35);
      } else {
      	$(this).find('.layout-edit-page').height($('.sp').outerHeight() + $('.layout-edit-basic-layout-select').outerHeight() + 35);
      }
      var h = $(this).find('.layout-edit-page').outerHeight();
      $(this).find('.layout-edit-parts').css({
        'max-height' : h + 'px'
      });
    });
  }
}
$(function() {
  height_fit.ini();

  $(window).off('resize load').on('resize load', function() {
    //height_fit.ini();
  });

  setInterval(function() {
    height_fit.ini();
  }, 1000);

});


/*------------------------------------------------
* 一覧
------------------------------------------------*/
var list = {
  ini: function () {
    //デモ用の処理
    $('.list-row a.list-col-title-a').each(function() {
      $(this).off('click').on('click', function() {
        list.open();
      });
    });
    $('.main-content-detail-btn-close').off('click').on('click', function() {
      if ($('body').hasClass('is-loading') == false) {

        // 確認ポップアップキャンセル判定
        var isCancel = $(this).attr("is_cancel");
        if (isCancel === "cancel") return;

        // 警告表示を削除
        $('.notification-message-warning').remove();
        // 選択を解除
        $(".selected-low").removeClass("selected-low");
        // プレースフォルダーを見えるように
        $(".placeholder-display-none").removeClass("placeholder-display-none");
        list.close();
      }
    });

    //一覧表示スタイル切り替え ini
    list.style.ini();

    //ウィンドウ幅1280px以下の時に表示される一覧メニューボタン ini
    list.menu.ini();
    
    //ページタイトル変更 ini
    list.title_edit.ini();

    //グループ名変更 ini
    list.group_name_edit.ini();

    //一覧並べ替え
    list.sortable();

  },
  open : function(){
    //一覧表示から編集画面表示へ
    $('.main-content-frame').addClass('is-open-detail');
    setTimeout(function() {
        $('.main-content-detail-inner').show();
        var event;
        if (typeof (Event) === 'function') {
          event = new Event('resize');
        } else {
          event = document.createEvent('Event');
          event.initEvent('resize', true, true);
        }
        window.dispatchEvent(event);
    }, 500);
  },
  close : function(){
    //詳細画面を閉じて一覧表示へ戻る
    $('.main-content-detail-inner').hide();
    $('.main-content-frame').addClass('animate-close-detail');
    setTimeout(function() {
      $('.main-content-frame').removeClass('is-open-detail');
      $('.main-content-frame').removeClass('animate-close-detail');
    }, 500);
  },
  
  /*------------------------------------------------
  * 一覧表示スタイル切り替え
  ------------------------------------------------*/
  style : {
    ini : function(){

      $('.list-style-switch').each(function() {
        var this_ = $(this);

        this_.find('input').off('change').on('change', function() {
          list.style.change(this_);
        });

        //ini
        list.style.change(this_);
      });

    },
    change : function(this_){
      var list_content = this_.closest('.list').find('.list-content');
      var list_head = this_.closest('.list').find('.list-head');
      var name = this_.find('input').attr('name');

      var style = this_.find('[name="' + name + '"]:checked').val();

      list_content.removeClass(function(index, className) {
        //is-list-style- で始まるclassを削除
        return (className.match(/\bis-list-style-\S+/g) || []).join(' ');
      })
      list_content.addClass('is-list-style-' + style);

      list_content.find('.list-col').css('display','');
      list_content.find('.list-col-inner-row').css('display', '');
      list_content.find('.hide-style-' + style).hide();
      list_head.find('.list-head-col').css('display', '');
      list_head.find('.hide-style-' + style).hide();
    }
  },

  /*------------------------------------------------
  * 一覧 タイトル変更
  ------------------------------------------------*/
  title_edit : {
    is_edit_classname: 'is-title-edit',
    ini: function() {
      $('.list-title-edit-trigger').each(function() {
        $(this).off('click').on('click', function() {
          var id = $(this).data('target-list');
          $('[data-list-id="' + id + '"]').addClass(list.title_edit.is_edit_classname);
          return false;
        });
      });
      $('.list-title-edit-cancel').each(function() {
        $(this).off('click').on('click', function() {
          $(this).closest('.list-row').removeClass(list.title_edit.is_edit_classname);
          return false;
        });
      });
      $('.list-title-edit-submit').each(function() {
        $(this).off('click').on('click', function() {
          $(this).closest('.list-row').removeClass(list.title_edit.is_edit_classname);
          return false;
        });
      });
    }
  },

  /*------------------------------------------------
  * グループ名変更
  ------------------------------------------------*/
  group_name_edit : {
    is_edit_classname: 'is-group-name-edit',
    ini: function() {
      $('.list-group-name-edit-trigger').each(function() {
        $(this).off('click').on('click', function() {
          var id = $(this).data('target-list-group');
          $('[data-list-group-id="' + id + '"]').addClass(list.group_name_edit.is_edit_classname);
          return false;
        });
      });
      $('.list-group-edit-cancel').each(function() {
        $(this).off('click').on('click', function() {
          $(this).closest('.list-group').removeClass(list.group_name_edit.is_edit_classname);
          return false;
        });
      });
      $('.list-group-edit-submit').each(function() {
        $(this).off('click').on('click', function() {

          $(this).closest('.list-group').removeClass(list.group_name_edit.is_edit_classname);
          return false;
        });
      });
    }
  },

  /*------------------------------------------------
  * 一覧 並べ替え
  ------------------------------------------------*/
  sortable : function(){
    $(".list-group-sortable").sortable({
      //revert: true,
      handle: ".icon-drag",
      axis: "y",
      placeholder: "ui-state-highlight"
    });

    $(".list-sortable").sortable({
      //revert: true,
      handle: ".icon-drag",
      axis: "y",
      connectWith: ".list-sortable",
      placeholder: "ui-state-highlight"
    });

    $(".icon-drag").off("click").on("click", function (event) {
      event.stopPropagation();
    });
  },

  /*------------------------------------------------
  * ウィンドウ幅1280px以下の時に表示される一覧メニューボタン
  ------------------------------------------------*/
  menu : {
    ini : function(){
      $('.list-menu-btn').each(function() {
        $(this).off('click').on('click', function() {
          var toggle_class_name = "is-show-float-list";
          var main_frame = $('.main-content-frame');
          if (!main_frame.hasClass(toggle_class_name)) {
            //開く
            $(this).find('.icon').removeClass('icon-menu').addClass('icon-close');
            main_frame.addClass(toggle_class_name);
          } else {
            //閉じる
            $(this).find('.icon').removeClass('icon-close').addClass('icon-menu');
            main_frame.removeClass(toggle_class_name);
          }
        });
      });
    }
  }
}
$(function() {
  list.ini();
});



/*------------------------------------------------
* ページURL変更
------------------------------------------------*/
var page_url_edit = {
  is_edit_classname: 'is-page-url-edit',
  ini: function() {
    $('.page-url').each(function() {
      var obj = $(this);
      obj.find('.page-url-edit-trigger').each(function() {
        $(this).off('click').on('click', function() {
          $(this).closest('.page-url').addClass(page_url_edit.is_edit_classname);
          return false;
        });
      });
      obj.find('.page-url-edit-cancel').each(function() {
        $(this).off('click').on('click', function() {
          $(this).closest('.page-url').removeClass(page_url_edit.is_edit_classname);
          return false;
        });
      });
      obj.find('.page-url-edit-submit').each(function() {
        $(this).off('click').on('click', function() {
          $(this).closest('.page-url').removeClass(page_url_edit.is_edit_classname);

          var inputValue = obj.find('.page-url-value-form-input').val();
          //表示に反映
          obj.find('.page-url-value-filename').html(inputValue);

          //hiddenに格納
          obj.find('.page-url-value-form-input-hidden').val(inputValue);
          return false;
        });
      });
    });
  }
}
$(function() {
  page_url_edit.ini();
});


/*------------------------------------------------
* 汎用タブ
------------------------------------------------*/
var tab = {
  ini: function() {
    $('.tabs').each(function() {
      var tabs = $(this);
      var tab_contents_list = [];
      tabs.find('.tabs-tab a').each(function() {
        tab_contents_list.push($(this).data('tab-content-selector'));
        $(this).off('click').on('click', function() {
          var selector = $(this).data('tab-content-selector');

          $(tabs).find('.tabs-tab a').removeClass('is-active');
          $(this).addClass('is-active');

          $(tab_contents_list).each(function(i) {
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
      tabs.find('.tabs-tab a').each(function() {
        if ($(this).hasClass('is-active')) {
          var selector = $(this).data('tab-content-selector');
          $(tab_contents_list).each(function(i) {
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
$(function() {
  tab.ini();
});


/*------------------------------------------------
* code editor
------------------------------------------------*/
var codeeditor = {
  ini : function(){
    $('.codeeditor').each(function() {
      var element = $(this)[0];
      ace.edit(element, {
        mode: "ace/mode/html",
        selectionStyle: "text",
        setTheme: "ace/theme/clouds"
      })
    });
  }
}
$(function() {
  codeeditor.ini();
});


/*------------------------------------------------
* custom select
------------------------------------------------*/
var custom_select = {
  ini : function(){
    $('.custom-select').each(function() {
      
      $(this).find('.custom-select-options, .custom-select-input').remove();
      
      var select = $(this).find('select');
      select.hide();

      //data set
      var options = [];
      select.find('option').each(function() {
        options.push({
          'value': $(this).attr('value'),
          'label': $(this).html(),
          'selected': $(this).prop('selected')
        })
      });

      //html set
      var options_html = '<span class="custom-select-options" style="display:none;"><span class="custom-select-options-search"><input type="text" value="" class="custom-select-options-search-input"><span class="icon icon-search"></span></span><span class="custom-select-options-list">';
      var selected_label = '';
      var selected_value = '';
      $(options).each(function() {
        if (this.label != "") {
          options_html += '<span data-value="' + this.value + '" class="custom-select-option line-clamp" title="' + this.label + '">' + this.label + '</span>';
        } else {
          options_html += '<span data-value="" class="custom-select-option">　</span>';
        }
        if (this.selected == true) {
          selected_label = this.label;
          selected_value = this.value;
        }
      });
      options_html += '</span></span>';


      $(this).append('<span class="custom-select-input"><input type="text" value="' + selected_label + '" readonly="readonly" class="custom-select-input-label"><input type="hidden" value="' + selected_value + '" class="custom-select-input-value"></span>' + options_html);

      //event set
      var custom_select_options_search_input = $(this).find('.custom-select-options-search-input');
      var custom_select_input = $(this).find('.custom-select-input');
      var custom_select_input_label = $(this).find('.custom-select-input-label');
      var custom_select_input_value = $(this).find('.custom-select-input-value');
      var custom_select_icon = $(this).find('.custom-select-icon');
      var custom_select_options = $(this).find('.custom-select-options');

      var last_index = custom_select_options.find('.custom-select-option').length - 1;
      var select_index = 0;
      var visible_selected_index = 0;
      var option_height = 40;

      //selectedの初期状態を反映
      $(options).each(function(i){
        if (this.selected == true) {
          select_index = i;
        }
      });
      custom_select_options.find('.custom-select-option:eq('+select_index+')').addClass('selected');

      //optionをクリックしたらそれを選択
      custom_select_options.find('.custom-select-option').each(function() {
        $(this).off('click').on('click', function() {
          custom_select_input_label.val($(this).html());
          custom_select_input_value.val($(this).html());
          custom_select_options.find('.custom-select-option').removeClass('selected');
          $(this).addClass('selected');
          clearInterval(timer);
          for (i = 0; i < $('.custom-select-option').length; i++) {
            if (custom_select_options.find('.custom-select-option:eq('+i+')').hasClass('selected')) {
              select_index = i - 1;
            }
          }
          select.val($(this).data('value'));
        });
      });

      //timer
      var timer;

      custom_select_input.off('click').on('click', function() {

        custom_select_options.css({
          'top': custom_select_input_label.outerHeight()
        });
        custom_select_options.show();
        visible_selected_index = select_index;
        custom_select_options_search_input.val('');
        custom_select_options_search_input.focus();

        //選択要素が下の方にある場合のスクロール位置調整
        var view_max_length = 6;
        var view_max_height = view_max_length * option_height;
        var scroll_top = custom_select_options.find('.custom-select-options-list').scrollTop();
        var selected_item_position = ( visible_selected_index + 1 ) * option_height;
        

        if( view_max_height + scroll_top < selected_item_position ){
          //下にはみ出ない位置までスクロール
          setTimeout(function(){
            custom_select_options.find('.custom-select-options-list').scrollTop(( visible_selected_index - view_max_length ) * option_height);
          },100);
        };

        //項目すべて表示
        custom_select_options.find('.custom-select-option').addClass('visible');

        //inputに入力された内容に応じてoptionの表示制御
        var inputed_search_keyword = '';
        timer = setInterval(function() {
          if(inputed_search_keyword != custom_select_options_search_input.val()){
            //キーワードが変化したら

            //見えているリストが変化したらselectedは一番上に移動
            visible_selected_index = 0;
            
            custom_select_options.find('.custom-select-option').each(function() {
              if ($(this).html().indexOf(custom_select_options_search_input.val())) {
                $(this).removeClass('visible');
              } else {
                $(this).addClass('visible');
              }
            });
            custom_select_options.find('.custom-select-option').removeClass('selected');
            custom_select_options.find('.custom-select-option.visible:eq('+visible_selected_index+')').addClass('selected');

            //直近で入力されたキーワードを格納
            inputed_search_keyword = custom_select_options_search_input.val();
          }
        }, 500);

        //マウスで選択
        custom_select_options.find('.custom-select-option').on('mouseenter', function() {
          custom_select_options.find('.custom-select-option').removeClass('selected');
          $(this).addClass('selected');
          select_index = $(this).index() - 1;
        });


        $('.custom-select').off('keyup').on('keyup', function(e) {

          if ((e.which && e.which === 38) || (e.keyCode && e.keyCode === 38)) {
            //上にvisible要素が存在しない場合はselectedを移動しない
            if(visible_selected_index > 0){
              custom_select_options.find('.custom-select-option').removeClass('selected');
              visible_selected_index --;
              custom_select_options.find('.custom-select-option.visible:eq(' + ( visible_selected_index ) + ')').addClass('selected');
            }

            //スクロール位置調整
            var view_max_length = 6;
            var view_max_height = view_max_length * option_height;
            var scroll_top = custom_select_options.find('.custom-select-options-list').scrollTop();
            var selected_item_position = ( visible_selected_index + 1 ) * option_height;

            if( ( visible_selected_index * option_height ) < scroll_top ) {
              //上にはみ出る位置まで到達した
              custom_select_options.find('.custom-select-options-list').scrollTop(visible_selected_index * option_height);
            };
          }
          if ((e.which && e.which === 40) || (e.keyCode && e.keyCode === 40)) {
            //表示されているもののうちの一つ下へselectを移動
            //下にvisible要素が存在しない場合はselectedを移動しない
            if(custom_select_options.find('.custom-select-option.visible:eq(' + ( visible_selected_index + 1 ) + ')').length > 0){
              custom_select_options.find('.custom-select-option').removeClass('selected');
              visible_selected_index ++;
              custom_select_options.find('.custom-select-option.visible:eq(' + ( visible_selected_index ) + ')').addClass('selected');
            }

            //スクロール位置調整
            var view_max_length = 6;
            var view_max_height = view_max_length * option_height;
            var scroll_top = custom_select_options.find('.custom-select-options-list').scrollTop();
            var selected_item_position = ( visible_selected_index + 1 ) * option_height;
            
            if( view_max_height + scroll_top < selected_item_position ){
              //下にはみ出る位置まで到達した
              custom_select_options.find('.custom-select-options-list').scrollTop(( visible_selected_index - view_max_length ) * option_height);
            };

          }
        });

        //Enterキーで決定処理
        $('.custom-select').off('keypress').on('keypress', function(e) {
          if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {

            //selectedクラスを持っている要素のindexを調べる
            select_index = custom_select_options.find('.custom-select-option.selected').index();

            setTimeout(function() {
              custom_select_input_label.val(custom_select_options.find('.custom-select-option').eq(select_index).text());
              custom_select_input_value.val(custom_select_options.find('.custom-select-option').eq(select_index).text());
              $(this).find('select option').each(function() {
                if ($(this).val() == custom_select_options.find('.custom-select-option').eq(select_index).data.value()) {
                  $(this).prop('selected', true);
                }
              });
              custom_select_options.hide();
              select.val(custom_select_options.find('.custom-select-option').eq(select_index).data('value'));
            }, 200);
            clearInterval(timer);
            return false;
          }
        });

      });

      custom_select_options_search_input.off('blur').on('blur', function() {
        setTimeout(function() {
          custom_select_options.hide();
        }, 200);
        clearInterval(timer);
        for (i = 0; i < $('.custom-select-option').length; i++) {
          if (custom_select_options.find('.custom-select-option:eq('+i+')').hasClass('selected')) {
            select_index = i - 1;
          }
        }
      });

    });
  }
}

$(function() {
  custom_select.ini();
});


/*------------------------------------------------
* タグ入力
------------------------------------------------*/
var custom_tag_input = {
  ini : function(){
    $('.custom-tags-input , .custom-category-input').each(function () {
      var autocomplete_source = $(this).data('autocomplete-source').split(',');
      var placeholder = $(this).data('placeholder').split(',');
      $(this).tagsInput({
        'placeholder' : placeholder,
        'autocomplete' : {
          source: autocomplete_source
        }
      });
    });
  }
}
$(function() {
  custom_tag_input.ini();
});

/*------------------------------------------------
* parts drag layout
------------------------------------------------*/
var parts = {
    ini : function(){
    //削除エリア追加
    $(".layout-edit").each(function(){
      $(this).append('<div class="droparea-area-delete" style="display: none;">削除する場合はこちらへドラッグしてください</div>');
    });
    $(".droparea-area").sortable({
      connectWith: ".droparea-area",
      placeholder: "ui-state-highlight",
      update: function(event, ui) {
        ui.item.css({
          'width': '',
          'height': ''
        });
        height_fit.ini();
      },
      start : function(){
        $('.droparea-area-delete').show();
      },
      stop : function(){
        $('.droparea-area-delete').hide();
        height_fit.ini();
      }
    });

    //パーツリストからエリアへドラッグ＆ドロップ
    $(".dd-parts-list .dd-parts-list-item").draggable({
      connectToSortable: ".droparea-area",
      helper: "clone"
    });

    //削除エリアにドロップされたら削除
    $(".droparea-area-delete").droppable({
      drop: function( event, ui ) {
        ui.draggable.remove();
        height_fit.ini();
      },
      out : function(event, ui){
        ui.draggable.removeClass('ui-state-delete');
      },
      over : function(event, ui){
        ui.draggable.addClass('ui-state-delete');
      }
    });
  }
}
$(function() {
  parts.ini();
});

/*------------------------------------------------
* notification
------------------------------------------------*/
var notification = {
  ini: function() {
    $('body').append('<div class="notification"></div>');
  },
  show: function(message, classname, type) {
    var id = Math.random().toString(36).slice(-8);
    $('.notification').append('<div class="notification-message notification-message-' + classname + '" id="notification-id-' + id + '">' + message + '<span class="notification-message-close"><span class="icon-close"></span></div></div>');
    setTimeout(function() {
      $('#notification-id-' + id).addClass('show');
    }, 200);
    if (type == 'fadeout') {
      setTimeout(function() {
        notification.hide(id);
      }, 5000);
    }
    $('#notification-id-' + id + ' .notification-message-close').off('click').on('click', function() {
      notification.hide(id);
    });
  },
  hide: function(id) {
    $('#notification-id-' + id + '').remove();
  }
}

$(function() {
  notification.ini();
});


/*------------------------------------------------
* ページ管理
------------------------------------------------*/
var page = {
  ini : function(){

    //サイトのテンプレート使用する・しない切り替え
    page.layout_template_selector();

    //編集画面 グループ追加
    page.group_select();

    //レイアウト編集：基本レイアウト選択
    page.basic_layout_select();
  },

  /*------------------------------------------------
  * ページ管理：サイトのテンプレート使用する・しない切り替え
  ------------------------------------------------*/
  layout_template_selector : function(){
    $('.page-layout-template-active-selector input[type="checkbox"]').each(function() {
      $(this).off('change').on('change', function() {
        if ($(this).prop("checked")) {
          $(this).closest('.main-content-detail-site').addClass('is-no-active');
        } else {
          $(this).closest('.main-content-detail-site').removeClass('is-no-active');
        }
      });
    });
  },

  /*------------------------------------------------
  * ページ管理：編集画面 グループ追加
  ------------------------------------------------*/
  group_select : function(){
    $('.group-select').each(function() {
      var group_select = $(this);
      group_select.find('.group-select-add-group-trigger').off('click').on('click', function() {
        group_select.find('.group-select-add-group-form').toggleClass('is-show');
        group_select.find('.group-select-add-group-form-input').val('');

        if(group_select.find('.group-select-add-group-form').hasClass('is-show')){
          group_select.find('.group-select-add-group-bg').show();
          group_select.find('.group-select-add-group-bg').off('click').on('click', function () {
            group_select.find('.group-select-add-group-form').toggleClass('is-show');
            group_select.find('.group-select-add-group-form-input').val('');
            group_select.find('.group-select-add-group-bg').hide();
      });
        }else{
          group_select.find('.group-select-add-group-bg').hide();
        }
      });
      $('.group-select-add-group-submit').off('click').on('click', function() {
        group_select.find('.group-select-add-group-form').toggleClass('is-show');
        group_select.find('.group-select-add-group-bg').hide();
      });
      
    });
  },

  /*------------------------------------------------
  * ページ管理：レイアウト編集：基本レイアウト選択
  ------------------------------------------------*/
  basic_layout_select : function() {
    var selected_classname = "is-selected";
    $(".layout-edit-page").each(function() {
      var layout_edit_page = $(this);
      var droparea = $(this).find('.layout-edit-page-droparea');
      var classname_list = [];
      layout_edit_page.find('.layout-edit-basic-layout-select-item').each(function() {
        classname_list.push($(this).data('layout-classname'));
        $(this).off('click').on('click', function() {
          droparea.removeClass(classname_list);
          droparea.addClass($(this).data('layout-classname'));

          if (layout_edit_page.find('.layout-edit-page-droparea .dd-parts-list-item:hidden').length) {
            //非表示となるパーツが1つ以上ある場合に確認ダイアログ
            if (window.confirm('レイアウトを変更すると、非表示になったエリアに配置されたパーツはクリアされます。よろしいですか？')) {
              // 「OK」時の処理終了

            } else {
              // 「キャンセル」時の処理開始

              layout_edit_page.find('.layout-edit-basic-layout-select-item.' + selected_classname).click();

              return false;
            }
          }
          // 選択レイアウトを更新
          layout_edit_page.find('.basic-layout-select-hidden-input-date').val($(this).data('layout-type'));

          layout_edit_page.find('.layout-edit-basic-layout-select-item').removeClass(selected_classname);
          $(this).addClass(selected_classname);

          layout_edit_page.find('.layout-edit-page-droparea .dd-parts-list-item').each(function() {

            if (!$(this).is(':visible')) {
              $(this).remove();
            }
          });


    });
      });
    });
  }


}
$(function() {
  page.ini();
});


/*------------------------------------------------
* 日付選択カレンダー
------------------------------------------------*/
var input_datepicker = {
  ini: function() {
    $('.input-datepicker').each(function() {
      $('.input-datepicker').datepicker({
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
        showButtonPanel: true // "今日"ボタン, "閉じる"ボタンを表示する
      });
    });

    $('.input-timepicker').each(function() {
      var input = $(this);

      input.off('focus').on('focus', function() {
        var h = input.height();
        var t = input.offset().top + h;
        var l = input.offset().left;
        $('body').append('<div class="timepicker" style="position:absolute;top:' + t + 'px;left:' + l + 'px;"><ul><li>00:00</li><li>01:00</li><li>02:00</li><li>03:00</li><li>04:00</li><li>05:00</li></ul><ul><li>06:00</li><li>07:00</li><li>08:00</li><li>09:00</li><li>10:00</li><li>11:00</li></ul><ul><li>12:00</li><li>13:00</li><li>14:00</li><li>15:00</li><li>16:00</li><li>17:00</li></ul><ul><li>18:00</li><li>19:00</li><li>20:00</li><li>21:00</li><li>22:00</li><li>23:00</li></ul><ul><li>23:59</li></ul></div>');
        $('.timepicker li').off('click').on('click', function() {
          input.val($(this).text());
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
      input.off('blur').on('blur', function() {
        setTimeout(function() {
          $('.timepicker').remove();
        }, 300);
      });
    });
  }
}
$(function() {
  input_datepicker.ini();
});


/*------------------------------------------------
* 公開範囲指定
------------------------------------------------*/
var access_authority_setting = {
  ini : function(){
    //期間指定
    access_authority_setting.period.ini();

    //ターゲットリスト指定
    access_authority_setting.targetlist.ini();

    //会員指定
    access_authority_setting.member.ini();
  },

  /*------------------------------------------------
  * 公開範囲指定：期間指定
  ------------------------------------------------*/
  period : {
    ini : function(){
      $('.access-authority-setting-period-info-value-wrap').each(function(){

        var target_obj = $(this);
        
        $(this).find('a').off('click').on('click',function(){
          var timer = setInterval(function(){
            
            if(modal.flg_open){

              //モーダル起動時の処理
              $('.access-authority-setting-period').each(function() {
                var obj = $(this);

                //クリアボタン
                obj.find('.access-authority-setting-period-start-clear').each(function() {
                  $(this).off('click').on('click', function() {
                    obj.find('.access-authority-setting-period-start-input input').val('');
                  });
                });
                obj.find('.access-authority-setting-period-end-clear').each(function() {
                  $(this).off('click').on('click', function() {
                    obj.find('.access-authority-setting-period-end-input input').val('');
                  });
                });

              });

              //初期値セット
              target_obj.each(function(){
                var tgt = $(this);
                var start_date = tgt.find('.access-authority-setting-period-info-hidden-input-start-date').val();
                var start_time = tgt.find('.access-authority-setting-period-info-hidden-input-start-time').val();
                var end_date = tgt.find('.access-authority-setting-period-info-hidden-input-end-date').val();
                var end_time = tgt.find('.access-authority-setting-period-info-hidden-input-end-time').val();
                if($(modal.selector_modal).find('#access-authority-setting-period-start-input-date-feature-area-banner').length > 0){
                  $(modal.selector_modal).find('#access-authority-setting-period-start-input-date-feature-area-banner').val(start_date);
                  $(modal.selector_modal).find('#access-authority-setting-period-start-input-time-feature-area-banner').val(start_time);
                  $(modal.selector_modal).find('#access-authority-setting-period-end-input-date-feature-area-banner').val(end_date);
                  $(modal.selector_modal).find('#access-authority-setting-period-end-input-time-feature-area-banner').val(end_time);
                  clearInterval(timer);
                }
              });

              //設定ボタン押下時の処理
              $('.access-authority-setting-period-submit').each(function(){
                $(this).off('click').on('click', function () {
                	access_authority_setting.period.set_value(target_obj, $(modal.selector_modal));

                  //モーダル閉じる
                  modal.close();
                });
              });

              clearInterval(timer);
            }
          },100);


        });
      });

    },
    set: function (tgt, tgtModel) {
        var start_date = tgtModel.find('.access-authority-setting-period-start-input-date input').val();
        var start_time = tgtModel.find('.access-authority-setting-period-start-input-time input').val();
        var end_date = tgtModel.find('.access-authority-setting-period-end-input-date input').val();
        var end_time = tgtModel.find('.access-authority-setting-period-end-input-time input').val();
        
      //hiddenにセット
      tgt.find('.access-authority-setting-period-info-hidden-input-start-date').val(start_date);
      tgt.find('.access-authority-setting-period-info-hidden-input-start-time').val(start_time);
      tgt.find('.access-authority-setting-period-info-hidden-input-end-date').val(end_date);
      tgt.find('.access-authority-setting-period-info-hidden-input-end-time').val(end_time);

      //表示側テキストにセット
      var show_text = "";
      if (start_date != "") {
        show_text += disptext_from.replace('@ 1 @', start_date + ' ' + start_time);
      }
      if (end_date != "") {
        show_text += disptext_to_publish.replace('@ 1 @', end_date + ' ' + end_time);
      } else {
        show_text += disptext_publish;
      }
      if (start_date == "" && end_date == "") {
        show_text = disptext_not_setting + disptext_publish_for_all_periods;
      }

      tgt.find('.access-authority-setting-period-info-value').text(show_text);
    },
    set_value: function (tgt, tgtModel) {
        var start_date = tgtModel.find('.access-authority-setting-period-start-input-date input').val();
        var start_time = tgtModel.find('.access-authority-setting-period-start-input-time input').val();
        var end_date = tgtModel.find('.access-authority-setting-period-end-input-date input').val();
        var end_time = tgtModel.find('.access-authority-setting-period-end-input-time input').val();

        if (start_date !== "" && start_time === "") {
            start_time = "00:00";
        }

        if (end_date !== "" && end_time === "") {
            end_time = "23:59";
        }

        if (((start_date === "") && (start_time !== "")) || check_date(start_date)) {
            notification.show(disptext_exact_start_date, 'warning', 'fadeout');
            return;
        }

        if (((end_date === "") && (end_time !== "")) || check_date(end_date)) {
            notification.show(disptext_exact_end_date, 'warning', 'fadeout');
            return;
        }

        //hiddenにセット
        tgt.find('.access-authority-setting-period-info-hidden-input-start-date').val(start_date);
        tgt.find('.access-authority-setting-period-info-hidden-input-start-time').val(start_time);
        tgt.find('.access-authority-setting-period-info-hidden-input-end-date').val(end_date);
        tgt.find('.access-authority-setting-period-info-hidden-input-end-time').val(end_time);

        //表示側テキストにセット
        var show_text = "";
        if (start_date != "") {
            show_text += disptext_from.replace('@ 1 @', start_date + ' ' + start_time);
        }
        if (end_date != "") {
            show_text += disptext_to_publish.replace('@ 1 @', end_date + ' ' + end_time)
        } else {
            show_text += disptext_publish;
        }
        if (start_date == "" && end_date == "") {
            show_text = disptext_not_setting + disptext_publish_for_all_periods;
        }

        tgt.find('.access-authority-setting-period-info-value').text(show_text);

        function check_date(date) {
            var match_date = date.match(/^(\d+)\/(\d+)\/(\d+)$/);
            if (match_date) {
                var match_date_year = parseInt(match_date[1]);
                var match_date_month = parseInt(match_date[2]) - 1;
                var match_date_day = parseInt(match_date[3]);
                match_date = new Date(match_date_year, match_date_month, match_date_day);
                return (((match_date_year == match_date.getFullYear())
                    && (match_date_month == match_date.getMonth())
                    && (match_date_day == match_date.getDate())) == false);
            }
        };
    }
},

  /*------------------------------------------------
  * 公開範囲指定：会員指定
  ------------------------------------------------*/
  member : {
    ini : function(){

      $('.access-authority-setting-member-info-value-wrap').each(function(){

        var target_obj = $(this);
        

        $(this).find('a').off('click').on('click',function(){
          var timer = setInterval(function(){
            
            if(modal.flg_open){

              //初期値セット
              target_obj.each(function(){
                var tgt = $(this);
                var type = tgt.find('.access-authority-setting-member-info-hidden-input-type').val();
                var rank = tgt.find('.access-authority-setting-member-info-hidden-input-rank').val();
                if(type != ''){
                  $(modal.selector_modal).find('input[name="access-authority-setting-member"][value="'+type+'"]').prop('checked', true);
                } else {
                  $(modal.selector_modal).find('input[name="access-authority-setting-member"][name="access-authority-setting-member"]').prop('checked', false);
                }
                if(rank != ''){
                  $(modal.selector_modal).find('input[name="access-authority-setting-member-rank"][value="'+rank+'"]').prop('checked', true);
                } else {
                  $(modal.selector_modal).find('input[name="access-authority-setting-member-rank"]').prop('selected', false);
                  $(modal.selector_modal).find('.access-authority-setting-member-rank-selector').hide();
                }
                toggle.ini();
              });

              //設定ボタン押下時の処理
              $('.access-authority-setting-member-submit').each(function(){
                $(this).off('click').on('click', function () {
                  access_authority_setting.member.set(target_obj,$(modal.selector_modal));

                  //モーダル閉じる
                  modal.close();
                });
              });

              clearInterval(timer);

            }
          },100);

        });
      });



    },
    set: function (tgt, tgtModel) {
      var tgt_modal_checked = tgtModel.find('input[name*="access-authority-setting-member"]:checked');
        var type = tgt_modal_checked.val();
      var type_label = tgtModel.find('label[for="' + tgt_modal_checked.attr('id') + '"]').text();

      var rank = tgtModel.find('[name="access-authority-setting-member-rank"]').val();
      var rank_label = tgtModel.find('[name="access-authority-setting-member-rank"] option[value="' + rank + '"]').text();

      //表示側テキストにセット
      var show_text = "";
      show_text = type_label;
      if (type_label == disptext_specific_member_rank) {
        show_text += '（' + rank_label + '）';
      } else {
          rank = '';
      }

      tgt.find('.access-authority-setting-member-info-value').text(show_text);

      //hiddenにセット
      tgt.find('.access-authority-setting-member-info-hidden-input-type').val(type);
      tgt.find('.access-authority-setting-member-info-hidden-input-rank').val(rank);
    }

  },

  /*------------------------------------------------
  * 公開範囲指定：ターゲットリスト
  ------------------------------------------------*/
  targetlist : {
    selected_list: null,
    all_list: null,
    ini: function(trigger_obj) {
      $('.access-authority-setting-targetlist-info-value-wrap').each(function(){

        var target_obj = $(this);
        
        $(this).find('a').off('click').on('click',function(){
          var timer = setInterval(function(){
            
            if(modal.flg_open){

              // モーダル開いたときに実行

              // 選択されたターゲットリストが格納される要素を定義
              access_authority_setting.targetlist.selected_list = $(modal.selector_modal).find('.access-authority-setting-targetlist-selected-list');
              access_authority_setting.targetlist.alllist = $(modal.selector_modal).find('.access-authority-setting-targetlist-list-content');

              //初期値セット
              //一旦リセット
              access_authority_setting.targetlist.selected_list.removeClass('is-selected');
              $(modal.selector_modal).find('.access-authority-setting-targetlist-selected-list-item').remove();

              //選択済みの要素をセット
              var selected_val = target_obj.find('.access-authority-setting-targetlist-info-hidden-input-ids').val();
              var selected_vals = null;
              if(selected_val){
                if(selected_val.indexOf(',')){
                  selected_vals = selected_val.split(',');
                } else {
                  selected_vals = selected_val;
                }
              }
              $(selected_vals).each(function(){
                var code = String(this);
                access_authority_setting.targetlist.alllist.find('a[onclick*="access_authority_setting.targetlist"]').each(function(){
                  var regexp = new RegExp(".*"+code+".,.*");
                  if($(this).attr('onclick').match(regexp)){
                    $(this).click();
                    return false;
                  }
                });

              });

              //設定ボタン押下時の処理
              $('.access-authority-setting-targetlist-submit').each(function(){
                var btn = $(this);
                btn.off('click').on('click', function () {
                    access_authority_setting.targetlist.set(target_obj, $(modal.tgt_selector));

                    //モーダル閉じる
                    modal.close();
                });
              });

              clearInterval(timer);
            }
          },100);


        });
      });

    },
    set: function (tgt, tgtModal) {
        var selected_ids = [];
        tgtModal.find('.access-authority-setting-targetlist-selected-list > .access-authority-setting-targetlist-selected-list-item').each(function () {
            selected_ids.push({
                id: $(this).find('.access-authority-setting-targetlist-selected-list-item-id').text(),
                title: $(this).find('.access-authority-setting-targetlist-selected-list-item-title').text()
            });
        });

        var type = tgtModal.find('[name*="access-authority-setting-targetlist-type"]:checked').val();
        tgt.find('.access-authority-setting-targetlist-info-hidden-input-type').val(type);

        //表示側テキストにセット
        var selected_title_text = '';
        if (selected_ids.length) {
            $(selected_ids).each(function (i) {
        selected_title_text += selected_ids[i].title + '(' + selected_ids[i].id + ')';
        if (selected_ids.length - 1 > i) {
          selected_title_text += '<br>';
      }
      });
      if ((type == "OR") && (selected_ids.length > 1)) {
        selected_title_text += disptext_meet_any.replace("@ 1 @", "<br>") + "(OR)";
      } else if ((type == "AND") && (selected_ids.length > 1)) {
        selected_title_text += disptext_meet_all.replace("@ 1 @", "<br>") + "(AND)";
      }
    } else {
        selected_title_text = disptext_not_setting;
    }
        tgt.find('.access-authority-setting-targetlist-info-value').html(null).append(selected_title_text);

        //hiddenにセット
        var selected_ids_value = '';
        $(selected_ids).each(function (i) {
            selected_ids_value += selected_ids[i].id;
            if (selected_ids.length - 1 > i) {
                selected_ids_value += ',';
            }
        });
        tgt.find('.access-authority-setting-targetlist-info-hidden-input-ids').val(selected_ids_value);
    },
    select: function(id, title) {
      access_authority_setting.targetlist.selected_list = $(modal.selector_modal).find('.access-authority-setting-targetlist-selected-list');
      access_authority_setting.targetlist.selected_list.addClass('is-selected');
      access_authority_setting.targetlist.selected_list.append('<div class="access-authority-setting-targetlist-selected-list-item" id="access-authority-setting-targetlist-selected-list-item-' + id + '" data-targetlist-id="' + id + '"><p class="access-authority-setting-targetlist-selected-list-item-id">' + id + '</p><p class="access-authority-setting-targetlist-selected-list-item-title line-clamp">' + title + '</p><div class="access-authority-setting-targetlist-selected-list-item-delete btn-close" onclick="var temp = $(this).closest(\'.modal-inner\'); access_authority_setting.targetlist.delete(\'' + id + '\'); target_list_select(temp);"><span class="icon icon-close"></span></div></div>');
    },
    delete: function(id) {
      $('#access-authority-setting-targetlist-selected-list-item-' + id).remove();

      if (access_authority_setting.targetlist.selected_list.find('.access-authority-setting-targetlist-selected-list-item').length == 0) {
        access_authority_setting.targetlist.selected_list.removeClass('is-selected');
      }

    }

  }
}
$(function() {
  access_authority_setting.ini();
});




/*------------------------------------------------
* 画像ファイル選択UI
------------------------------------------------*/
var image_file_select = {
  target_contents: '',
  ini : function(){
    var save_files_data = new Array($('.image-upload-content').length);
	var number_of_executions = 0;
    //指定可能数格納用配列
    var save_files_data_max_count = new Array($('.image-upload-content').length);

    var pc_header_banner = null;
    var sp_header_banner = null;

    if (typeof max_file_size !== 'undefined') {
      //ローカルテスト用
      var max_file_size = 1048576; // 最大ファイルサイズ(byte)
    }

    for (i = 0; i < $('.image-upload-content').length; i++) {
      save_files_data[i] = new Array();
    }

    $('.image-upload-content').each(function() {

      var block = $(this);
      var drag_area = block.find('.image-upload-content-drag');
      var index_num = $('.image-upload-content').index(this);

      var max_count = 1;
      if($(this).data('max-select-count') != undefined){
        max_count = $(this).data('max-select-count');
      }
      save_files_data_max_count[index_num] = max_count;

      var switch_active = {
        on : function(){
          if(!drag_area.hasClass('active')){
            drag_area.css({
              'height' : drag_area.outerHeight()+'px',
              'padding' : 0,
              'line-height' : drag_area.outerHeight()+'px'
            }).addClass('active');
          }
        },
        off : function(){
          drag_area.css({
            'height' : '',
            'padding' : '',
            'line-height' : ''
          }).removeClass('active');
        }
      }

      if (browser == 'firefox') {
        $('body').off('dragover drop').on('dragover drop', function(event) {
          dragover(event);
        });

        drag_area.off('dragover').on('dragover', function(event) {
          switch_active.on();
        });

        drag_area.off('dragleave').on('dragleave', function(event) {
          switch_active.off();
        });

        drag_area.off('drop').on('drop', function(event) {
          switch_active.off();
          selectImg(event, block, index_num);
        });

        block.find('.image-upload-content-drag .upload_img').off('change').on('change', function(event) {
          selectImg(event, block, index_num);
        });
      } else {
        $('body').off('dragover drop').on('dragover drop', function() {
          dragover(event);
        });

        drag_area.off('dragenter').on('dragenter', function () {}, false);

        drag_area.off('dragover').on('dragover', function() {
          switch_active.on();
        });

        drag_area.off('dragleave').on('dragleave', function() {
          switch_active.off();
        });

        drag_area.off('drop').on('drop', function() {
          switch_active.off();
          selectImg(event, block, index_num);
        });

        block.find('.image-upload-content-drag .upload_img').off('change').on('change', function() {
          selectImg(event, block, index_num);
        });
      }

      $('.upload_test').off('click').on('click', function() {
        uploadImg();
      });

      $('.upload_feature_image').off('click').on('click', function () {
        uploadFeatureImg();
      });

      if ( block.find('.image-upload-content-block-image img').length > 0 ) {
        block.find('.image-upload-content-block-image img').addClass('existing_img');
      }
      
    });

    //既にセットされている画像をsave_files_dataに格納
    $('.image-upload-content').each(function(i){
      $(this).find('.image-upload-content-block').each(function(n){
          var src = $(this).find('.image-upload-content-block-image img').attr('src');
          var splitSrc = src.split(',');
          if ((splitSrc.length > 1) && (splitSrc[1]).search(/(jpg|jpeg|gif|png|bmp|svg|tiff)$/) == -1) {
              src = convertBase64ImageToBlobObject(src);
          }
        save_files_data[i].push(src);
      });
    });

    //削除ボタンイベントセット
    setEventDeleteImg();

    // 特集ページ更新クリック
    $('.feature-page-update').off('click').on('click', function () {
      checkOtherOperatorFileOpening('featurePageId' + $('#Input_PageId').val()).done(function (result) {
        if (result !== "") {
          notification.show(result, 'warning', 'fixed');
        } else {
          loading_animation.start();
          delete_default_product_list();

          $('.image-upload-content.pc-header-banner').each(function () {
            if ($(this).find('.image-upload-content-block').length == 0) {
              $('input[name="input.PcContentInput.HeaderBanner.IsRemove"]').val("True");
            }
          });
          $('.image-upload-content.sp-header-banner').each(function () {
            if ($(this).find('.image-upload-content-block').length == 0) {
              $('input[name="input.SpContentInput.HeaderBanner.IsRemove"]').val("True");
            }
          });

          var result = $('input[id^="feature-page-element-"]');
          var form = $('#detail_form').get(0);
          var formData = new FormData(form);

          var arr = $(".feature-page-elements")
              .sortable("toArray", { attribute: 'data-list-id' });
          var spArr = $(".sp-contents")
              .sortable("toArray", { attribute: 'data-list-id' });
          for (var i = 0; i < arr.length; i++) {
            // 商品一覧を追加するはカウントしない
            if (arr[i] != "") formData.append('input.PcContentInput.sort', arr[i]);
          }
          for (var i = 0; i < spArr.length; i++) {
            if (spArr[i] != "") formData.append('input.SpContentInput.sort', spArr[i]);
          }
          // PC用の商品一覧を設定
          for (var i = 0; i < arr.length; i++) {
            if ((arr[i] != "") && (arr[i].indexOf("product-list-") != -1)) formData.append('input.PcContentInput.ProductList', arr[i]);
          }

          // PC用のトグル情報を設定
          for (var i = 0; i < result.length; i++) {
            formData.append('input.PcContentinput.ProductListDisp', result[i].checked);
          }

          // SP用の商品一覧を設定
          for (var i = 0; i < spArr.length; i++) {
            if ((spArr[i] != "") && (spArr[i].indexOf("product-list-") != -1)) formData.append('input.SpContentInput.ProductList', spArr[i]);
          }

          // SP用のトグル情報を設定
          for (var i = 0; i < result.length; i++) {
            formData.append('input.SpContentinput.ProductListDisp', result[i].checked);
          }

          for (var i = 0; i < save_files_data.length; i++) {
            for (var j = 0; j < save_files_data[i].length; j++) {
              if (save_files_data[i][j] === pc_header_banner) {
                formData.append('input.PcContentInput.HeaderBanner.UploadFile', pc_header_banner);
              }

              if (save_files_data[i][j] === sp_header_banner) {
                formData.append('input.SpContentInput.HeaderBanner.UploadFile', sp_header_banner);
              }
            }
          }

          set_pc_layout_form_data(formData);
          set_sp_layout_form_data(formData);

          $.ajax({
            type: "POST",
            url: "UpdateDetailPage",
            data: formData,
            processData: false,
            contentType: false,
          }).done(function (errorMessage) {
            if (errorMessage === "") {
              list_redraw();
              open_page($('#Input_PageId').val());
              notification.show('ページを更新しました。', 'info', 'fadeout');
            } else {
              setTimeout(function () {
                notification.show(errorMessage, 'warning', 'fixed');
                loading_animation.end();
              },
              500);
            }
          });
        }
      });
    });

    /*
      //フォームデータ取得テスト用
      $('h2:contains("画像選択（ファイル選択）")').after('<a href="javascript:void(0);" class="test">TEST</a>');
      $(document).on('click', '.test', function() {
        uploadImg();
      });
    */

    //Base64をBlobへ変換
    function convertBase64ImageToBlobObject(image) {
        var splitImage = image.split(',');
        var binary = atob(splitImage[1]);
        var buffer = new Uint8Array(binary.length);
        for (var i = 0; i < binary.length; i++) {
            buffer[i] = binary.charCodeAt(i);
        }
        var blob = new Blob([buffer.buffer], { type: splitImage[0].split(/:|;/)[1] });
        return blob;
    }

    function dragover(_e) {
      var e = _e;
      if (_e.originalEvent) {
        e = _e.originalEvent;
      }
      e.preventDefault();
    }

    function selectImg(_e, obj, num) {
      var e = _e;
      if (_e.originalEvent) {
        e = _e.originalEvent;
      }
      e.preventDefault();

      if(_e.type == 'change'){
        files = e.target.files;
      } else if(_e.type == 'drop'){
        files = e.dataTransfer.files;
      }

      for (var i = 0; i < files.length; i++) {

        save_files_data[num].push(files[i]);

        if ($(obj).hasClass('pc-header-banner')) {
          pc_header_banner = files[i];
        }
        if ($(obj).hasClass('sp-header-banner')) {
          sp_header_banner = files[i];
        }

        preview(i, files, obj, num);

        if ($('#controller').val() === 'CoordinatePage' || $('#controller').val() === 'LandingPage') {
            if (number_of_executions < 1) {
                uploadImage($('#controller').val());
                number_of_executions++;
            }
        }

        $('.upload_feature_image').prop('disabled', false);
      }
    }

    // 削除ボタンが同時に発火しないようにユニークなIDを付与
    var imgCnt = 0;

    function preview(i, files, obj, obj_index) {
      if (!files[i] || (files[i].type.indexOf('image/jpeg') < 0 && files[i].type.indexOf('image/gif') < 0 && files[i].type.indexOf('image/png') < 0 && files[i].type.indexOf('image/bmp') < 0 && files[i].type.indexOf('image/svg') < 0 && files[i].type.indexOf('image/tiff') < 0)) {
        notification.show(disptext_file_format_message.replace('@ 1 @', 'JPEG / GIF / PNG / BMP / SVG / TIFF'), 'warning', 'fadeout');
        cancelFeatureImg(files[i], obj, this);
        $('.upload_feature_image').prop('disabled', true);
        return;
      } else if (files[i].size > max_file_size) {
        notification.show(disptext_file_size_message.replace('@ 1 @', max_file_size / 1024), 'warning', 'fadeout');
        cancelFeatureImg(files[i], obj, this);
        $('.upload_feature_image').prop('disabled', true);
        return;
      }

      //最初から表示している画像を削除
      obj.find('.image-upload-content-block-wrapper > .image-upload-content-block > .image-upload-content-block-control > button').click();

      var fileReader = new FileReader();
      fileReader.onload = function (event) {
        var loadedImageUri = event.target.result;

        if ($(obj).hasClass('feature-image-form')) {
          obj.find('.image-upload-content-block > div').append(
            '<div style="margin-right:10px;width:150px"><img src="' +
            loadedImageUri +
            '" class="loaded_img" style="border: 3px solid #e2e2e2;"><div><dl><dt>' +
            files[i].name +
            '</dt></dl><button id="' +
            imgCnt +
            '" type="button" class="btn btn-main btn-size-s">' + disptext_delete_img + '</button></div>'
          );
          obj.find('.image-upload-content-block div div div #' + imgCnt).on('click', function () {
            cancelFeatureImg(files[i], obj, this);
            $('input[type=file]').val('');
          });
          imgCnt++;
          obj.find('.image-upload-content-block div').fadeIn();
        } else {

          obj.find('.image-upload-content-block-wrapper').append(
            '<div class="image-upload-content-block"><div class="image-upload-content-block-image"><img src="' +
            loadedImageUri +
            '" alt=""></div><div class="image-upload-content-block-control"><dl><dt>' +
            files[i].name +
            '</dt><dd></dd></dl><button id="' +
            i +
            '" type="button" class="btn btn-main btn-size-s">' + disptext_delete_img + '</button></div></div>');

          if (save_files_data[obj_index].length > save_files_data_max_count[obj_index]) {
            // max-count に到達している場合data配列の最初を削除
            cancelImg(obj_index, 0);
          } else if ($(obj).hasClass('pc-header-banner') || $(obj).hasClass('sp-header-banner')) {
            // 特集ページヘッダーバナーで最初から表示している画像を消す用
            obj.find(
              '.image-upload-content-block-wrapper > .image-upload-content-block > .image-upload-content-block-control > button')
              .click();
          }
        }

        // 削除ボタン
        setEventDeleteImg();

      };
      fileReader.readAsDataURL(files[i]);

    };

    //削除ボタンイベントセット
    function setEventDeleteImg() {
      $('.image-upload-content').each(function(i){
        $(this).find('.image-upload-content-block').each(function(n){
          $(this).find('.image-upload-content-block-control button').off('click').on('click',function(){
             cancelImg(i, n);
             $("#btnUploadImage").css("display", "none");
          });
        });
      });
    }

    function cancelImg(obj_index, img_index) {
      save_files_data[obj_index].splice(img_index, 1);
      $('.image-upload-content:eq(' + obj_index + ') .image-upload-content-block:eq(' + img_index + ')').remove();
      if (save_files_data[obj_index] >=0) {
        $('input[type=file]').val('');
      }
      setEventDeleteImg();
    }

    // 特集画像プレビューの削除ボタン
    function cancelFeatureImg(data, obj, child) {
      for (i = 0; i < save_files_data.length; i++) {
        for (j = 0; j < save_files_data[i].length; j++) {
          if (save_files_data[i][j] == data) {
            save_files_data[i].splice(j, 1);
          }
        }
        if (save_files_data[i].length === 0) {
            $('.upload_feature_image').prop("disabled", true);
      }
      }
      $(child).parent().parent().remove();
    }

    function uploadImg() {
      var formData = new FormData();

      for (i = 0; i < save_files_data; i++) {
        for (var j = 0; j < save_files_data[i].length; j++) {

          var file_data = save_files_data[i][j];
          var fileName = save_files_data[i][j].name;

          if (!file_data || (file_data.type.indexOf('image/jpeg') < 0 && file_data.type.indexOf('image/gif') < 0 && file_data.type.indexOf('image/png') < 0)) {
            continue;
          } else if (files[i][j].size > max_file_size) {
            continue;
          }
          formData.append("files", file_data);
          formData.append("file_name", fileName);
        }
        if (save_files_data[i].length == 0) {
            notification.show(disptext_file_not_select, 'warning', 'fadeout');
        } else {}
      }
    }

    // 特集画像アップロード
    function uploadFeatureImg() {
      // グループのドロップダウン取得
      var gid = $('[name=group]').val();

      for (var i = 0; i < save_files_data.length; i++) {
        syncUpload(save_files_data[i], 0, gid);
        save_files_data[i] = new Array();
      }

      $('.image-upload-content-block div').fadeOut();

      function syncUpload(data, j, gid) {
        if (j >= data.length) {
          if (j == 0) {
            notification.show(disptext_file_not_select, 'warning', 'fadeout');
          } else {
            notification.show(disptext_file_has_been_uploaded, 'info', 'fadeout');
          }
          $('.feature-image-form').find('.image-upload-content-block div div div button').click();
          return;
        }

        var formData = new FormData();
        var file_data = data[j];

          formData.append("files", file_data);
          formData.append("groupId", gid);

          $.ajax({
            url: "Upload",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
          }).done(function (viewHTML) {
            if (viewHTML.match(/.*\(\)$/)) {
              eval(viewHTML);
            } else {
              $("#list-content").html(viewHTML);

              j++;
              syncUpload(data, j, gid);
            }
          });
      }
    }

    $('.upload_staff_image').on('click', function () {
        uploadStaffImage();
    });

    $('[id^=ogp-setting-button-update]').off('click').on('click', function () {
		loading_animation.start();

		var sendData = new FormData();
		sendData.append("file", save_files_data[0][0]);

		$.ajax({
			url: "Upload",
			type: "POST",
			data: sendData,
			processData: false,
			contentType: false
		}).done(function(uploadUrl) {
			ogpSettingImageUploadDone(uploadUrl);
		});
	})

    function uploadImage(controller) {
        for (var i = 0; i < save_files_data.length; i++) {
            switch (controller) {
            case "CoordinatePage":
                coordinateUpload(save_files_data[i], 0);
                break;

            case "LandingPage":
                landingPageUpload(save_files_data[i], 0);
                break;
            }

            if (save_files_data[i].length == 0) {
                return;
            }
            $('.image-upload-content-block-image').fadeOut();
        }
    }
    function uploadStaffImage() {
        for (var i = 0; i < save_files_data.length; i++) {
            staffUpload(save_files_data[i]);

            if (save_files_data[i].length == 0) {
                notification.show(disptext_file_not_select, 'warning', 'fadeout');
                return;
            }
        }

        notification.show(disptext_file_has_been_uploaded, 'info', 'fadeout');

        $('.image-upload-content-block-image').fadeOut();
    }


    function staffUpload(data) {
        var formData = new FormData();
        var file_data = data[0];

        if (!file_data ||
        (file_data.type.indexOf('image/jpeg') < 0 &&
            file_data.type.indexOf('image/gif') < 0 &&
            file_data.type.indexOf('image/png') < 0 &&
            file_data.type.indexOf('image/bmp') < 0 &&
            file_data.type.indexOf('image/svg') < 0 &&
            file_data.type.indexOf('image/tiff') < 0)) {
            return;
        } else if (file_data.size > max_file_size) {
            return;
        }

        formData.append("files", file_data);
        formData.append("staffId", $('#staffId').val());

        $.ajax({
            url: "Upload",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
        }).done(function (message) {
            if (message != "") {
                upload_failed();
            } else {
                coordinateUpload(data);
                location.reload();
            }
        });
    }

    function coordinateUpload(data, j) {
        if (j >= data.length) {
            set_image_form();
            return;
    }
        var formData = new FormData();
        var file_data = data[j];

        if (!file_data ||
        (file_data.type.indexOf('image/jpeg') < 0 &&
            file_data.type.indexOf('image/gif') < 0 &&
            file_data.type.indexOf('image/png') < 0 &&
            file_data.type.indexOf('image/bmp') < 0 &&
            file_data.type.indexOf('image/svg') < 0 &&
            file_data.type.indexOf('image/tiff') < 0)) {
            return;
        } else if (file_data.size > max_file_size) {
            return;
        }

        formData.append("files", file_data);
        formData.append("coordinateId", $('#coordinateId').val());

        $.ajax({
            url: "Upload",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
        }).done(function (message) {
            if (message != "") {
                notification.show(message, 'warning', 'fadeout');;
                set_image_form();
            } else {
                j++;
                coordinateUpload(data, j);
            }
        });
    }

      function landingPageUpload(data, j) {
          if (j >= data.length) {
              set_image_form();
              return;
          }
          var formData = new FormData();
          var file_data = data[j];

          if (!file_data ||
          (file_data.type.indexOf('image/jpeg') < 0 &&
              file_data.type.indexOf('image/gif') < 0 &&
              file_data.type.indexOf('image/png') < 0 &&
              file_data.type.indexOf('image/bmp') < 0 &&
              file_data.type.indexOf('image/svg') < 0 &&
              file_data.type.indexOf('image/tiff') < 0)) {
              return;
          } else if (file_data.size > max_file_size) {
              return;
          }

          formData.append("files", file_data);
          formData.append("pageId", $('.pageId').val());

          $.ajax({
              url: "Upload",
              type: "POST",
              data: formData,
              processData: false,
              contentType: false,
          }).done(function (message) {
              if (message != "") {
                  notification.show(message, 'warning', 'fadeout');;
                  set_image_form();
              } else {
                  j++;
                  landingPageUpload(data, j);
              }
          });
      }

    // 特集エリア更新
    $('#featureAreaUpdateButton').off('click').on('click', function () {
        var formData = GetFeatureAreaFormData();
        main_content_detail_update(formData);
    });
    $('#featureAreaPreviewButton').off('click').on('click', function () {
        var formData = GetFeatureAreaFormData();

        var i = 0;
        $('.image-upload-content').each(function () {
          $(this).find('.image-upload-content-block').each(function () {
              var src = $(this).find('.image-upload-content-block-image img').attr('src');
              if (src.indexOf("data:image") !== 0) {
                  src = "";
              }
              formData.append("bannerInputs[" + i + "].BannerImageInput.PreviewBinary", src);
        });
        i ++;
      });
      
        previewFeatureArea(formData);
    });

    function GetFeatureAreaFormData() {
        var bannerImages = $('.image-upload-content-block-wrapper');
        for (var i = 0; i < bannerImages.length; i++) {
            var hiddenFileName = document.getElementsByName(bannerImages[i].id.split('.')[0] + ".FileName")[0];
            var hiddenDir = document.getElementsByName(bannerImages[i].id.split('.')[0] + ".FileDirPath")[0];
            if ((bannerImages[i].getElementsByTagName('dt')[0] != null)
                && (bannerImages[i].getElementsByTagName('dt')[0] != undefined)
                && (hiddenFileName != undefined)) {
                hiddenFileName.value = bannerImages[i].getElementsByTagName('dt')[0].innerText;
            } else if (hiddenFileName != undefined) {
                hiddenFileName.value = "";
            }
            if ((hiddenDir != undefined)
                && ((hiddenFileName.value != undefined)
                    && (hiddenFileName.value != null)
                    && (hiddenFileName.value != ""))) {
                hiddenDir.value = 'Contents/Feature/';
            } else if (hiddenDir != undefined) {
                hiddenDir.value = '';
            }
        }
        var form = $('#detail_form').get(0);
        var formData = new FormData(form);
        for (var j = 0; j < bannerImages.length; j++) {
            if (save_files_data[j].length > 0) {
                formData.append("bannerInputs[" + j + "].BannerImageInput.UploadFile", save_files_data[j][0]);
            }
        }
        return formData;
    }

    // 特集エリアタイプ更新
    $('#featureAreaTypeUpdateButton').off('click').on('click', function () {
        var iconImages = $('.image-upload-content-block-wrapper');
        for (var i = 0; i < iconImages.length; i++) {
            var hiddenFileName = document.getElementsByName(iconImages[i].id.split('.')[0] + ".FileName")[0];
            if ((iconImages[i].getElementsByTagName('dt')[0] != null)
                && (iconImages[i].getElementsByTagName('dt')[0] != undefined)) {
                hiddenFileName.value = iconImages[i].getElementsByTagName('dt')[0].innerText;
            } else {
                hiddenFileName.value = "";
            }
        }
        var form = $('#detail_form').get(0);
        var formData = new FormData(form);
        for (var j = 0; j < iconImages.length; j++) {
            if (save_files_data[j].length > 0) {
                formData.append("Input.AreaTypeImage.UploadFile", save_files_data[j][0]);
            }
        }
        main_content_detail_update(formData);
    });

  }
}

$(function() {
  image_file_select.ini();
});

/*------------------------------------------------
* ラジオボタンで、選択されている時にもう一度クリックされたらチェックを外す
------------------------------------------------*/
var remove_radio_check = {
  ini: function () {
    $('.search_table .can-removed-radio').each(function () {
      $(this).on('click', function () {
        if ($(this).hasClass('selected')) {
          $(this).prop('checked', false);
          $(this).removeClass('selected');
        }
        else {
          $(this).closest('td').find('.can-removed-radio').removeClass('selected');
          $(this).addClass('selected');
        }
      });
    });
  }
}
$(function () {
  remove_radio_check.ini();
});

/*------------------------------------------------
* スライド型チェックボックス
------------------------------------------------*/
var slide_check = {
  ini : function(){
    $('.slide-checkbox-wrap input[type="checkbox"]').each(function(){
      slide_check.label_set(this);
      $(this).off('change').on('change', function() {
        slide_check.label_set(this);
      });
    });
  },
  label_set : function(obj){
    var onLabel = $(obj).attr('data-on-label');
    var offLabel = $(obj).attr('data-off-label');
    if ($(obj).prop('checked') == true) {
      $(obj).parent().find('label .slide-checkbox-label').text(onLabel);
    } else {
      $(obj).parent().find('label .slide-checkbox-label').text(offLabel);
    }

  }
}
$(function() {
  slide_check.ini();
});



/*------------------------------------------------
* 特集ページ 
------------------------------------------------*/
var feature = {
  ini : function(){
    //要素をドラッグ&ドロップで並び替え
    feature.element_sort();

    //表示する商品の並べ替え
    feature.selected_item_sort();
  },
  /*------------------------------------------------
  * 特集ページ：要素をドラッグ&ドロップで並び替え
  ------------------------------------------------*/
  element_sort : function(){
    $(".feature-page-elements").each(function(){
      var elements = $(this);
      var element = elements.find('.feature-page-element');
      
      elements.sortable({
        //revert: true,
        handle: ".feature-page-element-head .icon-drag",
        axis: "y",
        placeholder: "ui-state-highlight",
        start: function (event, ui) {
          elements.addClass('is-sorting');
          elements.sortable( "refreshPositions" );
        },
        stop: function(event, ui) {
          elements.removeClass('is-sorting');
        }

      });

      //up
      element.find('.feature-page-element-sort-btn-up').off('click').on('click', function () {
        var this_ = $(this).closest('.feature-page-element');
        var prev_ = this_.prev();
        // 非表示の場合は飛ばす
        while (prev_.css('display') === "none") {
          prev_ = prev_.prev();
        }
        var target_ = $(this);

        this_.css({
          'transition' : '0.5s',
          'transform' : 'translateY('+'-' + ( prev_.height() + 20 ) + 'px)'
        });

        prev_.css({
          'transition' : '0.5s',
          'transform' : 'translateY('+ ( this_.height() + 20 ) + 'px)'
        });

        setTimeout(function(){
          prev_.before(this_);
          prev_.css({
            'transition' : '',
            'transform' : ''
          });
          this_.css({
            'transition' : '',
            'transform' : ''
          });
          if ($('#controller').val() === 'ScoringSale') {
            if (target_.attr("data-type") == 'ScoringSaleQuestionPage') {
              var currentIndex = this_.data().pageIndex;
              var nextIndex = currentIndex - 1;
              swap_question_page(currentIndex, nextIndex);
              feature.element_sort();
            } else if (target_.attr("data-type") == 'ProductRecommendCondition') {
              var currentIndex = this_.data().conditionIndex;
              var nextIndex = currentIndex - 1;
              swap_product_recommend(currentIndex, nextIndex);
              feature.element_sort();
            }
          }
        },500);

      });

      //down
      elements.find('.feature-page-element-sort-btn-down').off('click').on('click', function () {
        var this_ = $(this).closest('.feature-page-element');
        var next_ = this_.next();
        // 非表示の場合は飛ばす
        while (next_.css('display') === "none") {
          next_ = next_.next();
        }
        var target_ = $(this);

        this_.css({
          'transition' : '0.5s',
          'transform' : 'translateY('+ ( next_.height() + 20 ) + 'px)'
        });

        next_.css({
          'transition' : '0.5s',
          'transform' : 'translateY(-'+ ( this_.height() + 20 ) + 'px)'
        });

        setTimeout(function(){
          this_.before(next_);
          next_.css({
            'transition' : '',
            'transform' : ''
          });
          this_.css({
            'transition' : '',
            'transform' : ''
          });
          if ($('#controller').val() === 'ScoringSale') {
            if (target_.attr("data-type") == 'ScoringSaleQuestionPage') {
              var currentIndex = this_.data().pageIndex;
              var nextIndex = currentIndex + 1;
              swap_question_page(currentIndex, nextIndex);
              feature.element_sort();
            } else if (target_.attr("data-type") == 'ProductRecommendCondition') {
              var currentIndex = this_.data().conditionIndex;
              var nextIndex = currentIndex + 1;
              swap_product_recommend(currentIndex, nextIndex);
              feature.element_sort();
            }
          }
        },500);
        
      });
    });
  },
  /*------------------------------------------------
  * 特集ページ：表示する商品の並べ替え
  ------------------------------------------------*/
  selected_item_sort : function(){
    $(".feature-selected-item-list-body").each(function(){
      $(this).sortable({
        //revert: true,
        handle: ".icon-drag",
        axis: "y",
        placeholder: "ui-state-highlight"
      });
    });
  }
}
$(function() {
  feature.ini();
});

/*------------------------------------------------
* 特集エリア
------------------------------------------------*/
var feature_area = {
  ini : function(){
    //要素をドラッグ&ドロップで並び替え
    feature_area.element_sort();

    //削除ボタン
    $('.feature-area-banners-setting').on('click', '.feature-area-banner-content-btn-delete', function () {
      $(this).closest('.feature-area-banner').remove();
      $(this).closest('.feature-area-banner').addClass('deleted-banner');

      image_file_select.ini();
      feature_area.set_name_index();
      access_authority_setting.ini();
    });

    //追加ボタン
    $('.feature-area-banners-setting').each(function(){
      $(this).find('.feature-area-banners-add-btn a').off('click').on('click',function(){

        //ユニークな番号を生成
        var max_index_num = 0;
        $('.feature-area-banners-wrapper .feature-area-banner').each(function(){
          var index_num = Number($(this).attr('data-id').match(/\d+/)) + 1; //数字を抽出
          if(index_num > max_index_num){
            max_index_num = index_num;
          }
        });

        var html = $('.feature-area-banners-add-element-template').html();
        html = html.replace(/___index/g,(max_index_num));
        $('.feature-area-banners').append(html);
        
        feature_area.set_name_index();

        // 追加したDatepickerの紐づけをつけなおす
        $('#access-authority-setting-period-start-input-date' + max_index_num)
            .removeClass("hasDatepicker");
        $('#access-authority-setting-period-end-input-date' + max_index_num)
            .removeClass("hasDatepicker");
        input_datepicker.ini();

        // 各UIセット
        image_file_select.ini();
        feature_area.ini();
        access_authority_setting.ini();

      });
    });
  },
  /*------------------------------------------------
  * 特集エリア：要素の順番変更や追加・削除が起きたときにname属性を連番に変換
  ------------------------------------------------*/
  set_name_index: function () {
    // 再採番用のインデックス ループ変数のiには削除されたバナーもループに含むため採番には利用不可
    var index = 0;
    $('.feature-area-banners-wrapper').each(function () {
      $(this).find('.feature-area-banner').each(function (i) {

        if ($(this).hasClass('deleted-banner')) {
          return true;
        }

        // データIDをインデックスの値で再採番
        var data_id_ = $(this).attr('data-id');
        if (data_id_) {
          var replaced_data_id_ = data_id_.replace(/\d+/, index);
            $(this).attr('data-id', replaced_data_id_);
        }

        $(this).find('input,select,textarea,label,div').each(function () {
          var name_ = $(this).attr('name');
          if (name_) {
            var replaced_name_ = name_.replace(/\d+/, index);
            $(this).attr('name', replaced_name_);
          }
          var id_ = $(this).attr('id');
          if (id_) {
            var replaced_id_ = id_.replace(/\d+/, index);
            $(this).attr('id', replaced_id_);
          }
          var for_ = $(this).attr('for');
          if (for_) {
            var replaced_for_ = for_.replace(/\d+/, index);
            $(this).attr('for', replaced_for_);
          }

          //なぜなのかは原因不明だが、propでチェックを付け直す処理をしないと
          //バナー順番変更後、ポップアップ画面で選択肢が消えるバグが発生するため
          //checked付ける処理をこちらでし直す。
          var checked_ = $(this).attr('checked');
          if (checked_ && (name_.indexOf('access-authority-setting-member') >= 0)) {
            $(this).prop('checked', true);
          }
        });

        $(this).find('.image-upload-content-block-wrapper').each(function () {
            var name_ = $(this).attr('name');
            if (name_) {
              var replaced_name_ = name_.replace(/\d+/, index);
                $(this).attr('name', replaced_name_);
            }
            var id_ = $(this).attr('id');
            if (id_) {
              var replaced_id_ = id_.replace(/\d+/, index);
                $(this).attr('id', replaced_id_);
            }
        });

        $(this).find('.image-upload-content-drag-btns').each(function () {
          $(this).find('.btn.btn-main.btn-size-m').each(function () {
            var on_click_ = $(this).attr('onclick');
            if (on_click_) {
              var replaced_on_click_ = on_click_.replace(/\d+/, index);
              $(this).attr('onclick', replaced_on_click_);
            }
          });
        });

        $(this).find('.access-authority-setting-period-info-value-wrap a').each(function() {
            var on_click_ = $(this).attr('onclick');
            if (on_click_) {
              var replaced_on_click_ = on_click_.replace(/\d+/, index);
                $(this).attr('onclick', replaced_on_click_);
            }
        });

        $('input[name=ConditionPublishDateFromDate]').each(function () {
            $(this).removeClass("hasDatepicker");
        });
        $('input[name=ConditionPublishDateToDate]').each(function () {
            $(this).removeClass("hasDatepicker");
        });
        input_datepicker.ini();
        index++;
      });
    });
  },
  /*------------------------------------------------
  * 特集エリア：要素をドラッグ&ドロップで並び替え
  ------------------------------------------------*/
  element_sort : function(){
    $(".feature-area-banners").each(function(){
      var elements = $(this);
      var element = elements.find('.feature-area-banner');
      
      elements.sortable({
        //revert: true,
        handle: ".feature-area-banner-head .icon-drag",
        axis: "y",
        placeholder: "ui-state-highlight",
        start: function (event, ui) {
          elements.addClass('is-sorting');
          elements.sortable( "refreshPositions" );
        },
        sort: function (event, ui) {
          elements.sortable("refreshPositions");
        },
        stop: function(event, ui) {
          elements.removeClass('is-sorting');
          setTimeout(function () {
            image_file_select.ini();
          }, 500);
        }
      });

      //up
      element.find('.feature-area-banner-sort-btn-up').off('click').on('click',function(){
        var this_ = $(this).closest('.feature-area-banner');
        var prev_ = this_.prev();

        this_.css({
          'transition' : '0.5s',
          'transform' : 'translateY('+'-' + ( prev_.height() + 20 ) + 'px)'
        });

        prev_.css({
          'transition' : '0.5s',
          'transform' : 'translateY('+ ( this_.height() + 20 ) + 'px)'
        });

        setTimeout(function(){
          prev_.before(this_);
          prev_.css({
            'transition' : '',
            'transform' : ''
          });
          this_.css({
            'transition' : '',
            'transform' : ''
          });
          image_file_select.ini();
        }, 500);

      });

      //down
      elements.find('.feature-area-banner-sort-btn-down').off('click').on('click',function(){
        var this_ = $(this).closest('.feature-area-banner');
        var next_ = this_.next();

        this_.css({
          'transition' : '0.5s',
          'transform' : 'translateY('+ ( next_.height() + 20 ) + 'px)'
        });

        next_.css({
          'transition' : '0.5s',
          'transform' : 'translateY(-'+ ( this_.height() + 20 ) + 'px)'
        });

        setTimeout(function(){
          this_.before(next_);
          next_.css({
            'transition' : '',
            'transform' : ''
          });
          this_.css({
            'transition' : '',
            'transform' : ''
          });
          image_file_select.ini();
        }, 500);
        
      });
    });
  }
}
$(function() {
  feature_area.ini();
});


/*------------------------------------------------
* タグマニュアル
------------------------------------------------*/
var tag_manual = {
  ini : function(){
    $('.tag-manual').each(function(){
      var obj = $(this);
      var btn_open = obj.find('.tag-manual-toggle');
      var btn_close = obj.find('.tag-manual-head-arrow');
      btn_open.off('click').on('click',function(){
        btn_open.hide();
        obj.find('.tag-manual-toggle-content').slideDown(500,function(){
        obj.find('input').focus();
      });
      });
      btn_close.off('click').on('click',function(){
        obj.find('.tag-manual-toggle-content').slideUp(500);
        btn_open.show();
      });

      //検索・絞り込み
      obj.find('.tag-manual-content-navi-item a').each(function(){
        $(this).off('click').on('click',function(){
          var text = $(this).text();
          if(text == "すべて"){
            obj.find('.tag-manual-content-block').removeClass('is-hide');
          } else {
            obj.find('.tag-manual-content-block').removeClass('is-hide').each(function(){
              if($(this).data('page') != text){
                $(this).addClass('is-hide');
              }
            });
          }
          obj.find('.tag-manual-content-navi-item a').removeClass('is-selected');
          $(this).addClass('is-selected');
        });
      });
      obj.find('.tag-manual-content-block-search input').each(function(){
        var input = $(this);
        var focus_timer = null;
        var keyword = input.val();
        input.off('focus').on('focus',function(){

          focus_timer = setInterval(function(){
            if(keyword != input.val().toLowerCase()){
              var val = input.val().toLowerCase();
              obj.find('.tag-manual-content-block').removeClass('is-hide-keyword').each(function(){
                var text = $(this).text().replace(/\s+|\n+/g, "").toLowerCase();
                if( text.indexOf(val) == -1 ){
                  $(this).addClass('is-hide-keyword');
                }
              });
              keyword = val;
            }
          }, 500);

        });
        input.off('blur').on('blur',function(){
          clearInterval(focus_timer);
        });
      });
      
    });
  }
}
$(function() {
  tag_manual.ini();
});


/*------------------------------------------------
* ローディングアニメーション
------------------------------------------------*/
var loading_animation = {
  'start' : function(msg){
    if(jQuery('.loading-animation').length == 0){
      var html = '<div class="loading-animation"><span class="loading-animation-circle"></span></div>';
      jQuery('body').append(html);
    }
    setTimeout(function(){
      jQuery('body').addClass('is-loading');
      jQuery(".list-col-title-a").css("pointer-events", "none");
    },200);
  },
  'end' : function(){
      jQuery('body').addClass('is-loaded');
      setTimeout(function () {
          while (jQuery('body').hasClass('is-loading'))
          {
              jQuery('body').removeClass('is-loading');
          }
      }, 1000);
    setTimeout(function(){
      jQuery('body').removeClass('is-loaded');
      jQuery(".list-col-title-a").css("pointer-events", "auto");
    },1000);
  }
}

var block_loading_animation = {
  'start': function (jqueryElement) {
    if (jQuery(jqueryElement + ' .block-loading-animation').length == 0) {
      var html = '<div class="block-loading-animation"><span class="loading-animation-circle"></span></div>';
      jQuery(jqueryElement).append(html);
    }
    setTimeout(function () {
      jQuery(jqueryElement).addClass('is-loading');
    }, 200);
  },
  'end': function (jqueryElement) {
    jQuery(jqueryElement).addClass('is-loaded');
    setTimeout(function () {
      while (jQuery(jqueryElement).hasClass('is-loading')) {
        jQuery(jqueryElement).removeClass('is-loading');
      }
    }, 1000);
    setTimeout(function () {
      jQuery(jqueryElement).removeClass('is-loaded');
    }, 1000);
  }
}

// グループの全展開・全収納
function all_open_close_toggle_ini() {
  var openClass = 'is-list-open-toggle';
  var toggleBtnId = '#all_open_close_toggle_btn';
  $(toggleBtnId).off("click").on("click", function () {
    if ($(this).hasClass(openClass)) {
      // 収納処理
      $(this).removeClass(openClass)
        .removeClass('btn-sub')
        .addClass('btn-main');
      $('.list-group-toggle-btn.is-toggle-open').click();
    } else {
      // 展開処理
      $(this).addClass(openClass)
        .removeClass('btn-main')
        .addClass('btn-sub');
      $('.list-group-toggle-btn:not(.is-toggle-open)').click();
    }
  });
}

/*------------------------------------------------
* 複数選択プルダウン
------------------------------------------------*/
var multiselect = {
  ini : function(){
    var linkText = {
        checkAll: { text: disptext_select_all, title: disptext_select_all },
        uncheckAll: { text: disptext_deselect_all, title: disptext_deselect_all }
    };
    $(".multiselect").each(function(){
      var tgt = $(this);
      tgt.multiselect({
        buttonWidth: 'auto',
        menuWidth: 'auto',
        linkInfo: linkText,
        noneSelectedText: disptext_please_select,
        selectedList: 4,
        selectedText: disptext_selecting.replace('@ 1 @', '#')
      });
    });
  }
};
$(function() {
  multiselect.ini();
});

// サムネイル表示処理
function thumbnail_ini() {
  if (browser === 'ie') {
    $('.web-browser-capture-image').each(function() {
      $(this).show();
      $(this).on('error',
        function() {
          $(this).hide();
        });
    });
  } else {
    $('.web-browser-capture-image').on('load', function () {
      $(this).show();
    });
  }
}

// Disabled button
function disabled_button_by_max_number_limit(id, currentNumber, maxNumber) {
    var btnId = "#" + id;
    if (currentNumber >= maxNumber) {
        $(btnId).hide();
        return;
    }

    $(btnId).show();
}
