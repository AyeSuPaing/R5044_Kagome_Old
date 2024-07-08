/*
=========================================================================================================
  Module      : 左メニューアコーディオン(menuaccordion.js)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
$(function () {
    var slideUpSpeed = 'fast'; // 'slow', 'fast', 任意ミリ秒
    var slideDownSpeed = 0;

    var avtiveMenu = 'avtive_menu';
    var openMenu = 'open_menu';
    var lMenu = 'span > img#lmenu';

    var selectorAccordion = ".manager_menu dt";
    var cookieMenuIndex = 'cookie_marketing_menu'; // ECとMPで別々のキー
    var menuIndex = getCookie(cookieMenuIndex);

    // 閉じてるメニューをCookie保持している場合は復元
    if ((menuIndex != null) && (menuIndex != "")) {
        var arrayMenuIndex = menuIndex.split(',');
        $(arrayMenuIndex).each(function (i, val) {
            $(selectorAccordion).each(function (menuIndex) {
                if ((val != "") && (menuIndex == val) && ($(this).hasClass(avtiveMenu) == false)) {
                    $(this).removeClass(openMenu).next().slideUp(0);
                    $(lMenu, this).attr("src", $(lMenu, this).attr("src").replace('1_03.gif', '1_01.gif'));
                }
            });
        });
    }

    $(selectorAccordion).click(function () {
        // 大分類メニューをマウスクリックしたら小分類メニューを開閉
        if (($(this).hasClass(openMenu) == false) && ($(this).hasClass(avtiveMenu) == false)) {
            $(this).addClass(openMenu).next().slideDown(slideDownSpeed);
            $(lMenu, this).attr("src", $(lMenu, this).attr("src").replace('1_01.gif', '1_03.gif'));

        }
        else if ($(this).hasClass(openMenu)) {
            $(this).removeClass(openMenu).next().slideUp(slideUpSpeed);
            $(lMenu, this).attr("src", $(lMenu, this).attr("src").replace('1_03.gif', '1_01.gif'));
        }

        // 閉じてるメニューをCookie保存
        var closeMenuIndex = '';
        $(selectorAccordion).each(function (index) {
            if (($(this).hasClass(openMenu) == false) && ($(this).hasClass(avtiveMenu) == false)) {
                closeMenuIndex += index + ',';
            }
        });
        setCookie(cookieMenuIndex, closeMenuIndex, { expires: 7, path: '/' });
    });
});
