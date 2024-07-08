$(function () {

    // イージング名
    var EN = "easeInOutElastic";

    // メインビジュアルの動くスピード
    var SP = 550;

    // メインビジュアルの入れ替わる間隔
    var IT = 4000;

    // メインビジュアルを囲むDIV
    var photoSetMain = $("#photoset-main");

    // メイン画像の数
    var numMain = $("p", photoSetMain).length;

    // メイン画像を二倍にした数
    var numMainAll = numMain * 2;

    // メインビジュアルの横幅（一枚目から取得）
    var imageWidth =  $("p:first", photoSetMain).outerWidth();

    // サムネイル群
    var thums = $("#mainvisual-thumb ul li");

    // メイン画像を二倍にする。
    $(photoSetMain.html()).appendTo(photoSetMain);

    // 現在のメイン画像の位置
    var now = 0;

    // はじめのテキスト以外全て非表示
    $("#mainvisual-txt li:gt(0)").hide();

    // メイン画像の動き
    function moveMainVisual() {

        // 動いていたら止める
        photoSetMain.stop();
        // 今が最後の一枚だったら。
        if (now == numMainAll - 1) {

            // 一巡目の最後の一枚のナンバー取得
            var returnPoint =  numMain - 1;
            // 一巡目の同じ画像の位置を取得
            var returnLeft  = (imageWidth * returnPoint) * -1;
            // 画像を一巡目の同じ画像にすりかえる。
            photoSetMain.css({
                "left" : returnLeft + "px"
            });
            // 現在の位置に一巡目の最後の画像の位置を設定。
            now = returnPoint;
        }
        // 次の画像の位置
        now++;
        // 次の画像の位置を取得
        var moveLeft = (imageWidth * now) * -1;
        // 次の画像までアニメーションで動かす。
        photoSetMain.animate({
            left : moveLeft
        }, SP);
        // サムネイルの枠の位置を変更。
        var thumIndex = now;
        // 現在の画像が2順目に入っていたら。
        if (now >= numMain) {
            // サムネイルのインデックスを割り出す。
            thumIndex = now - numMain;
        }

        // 現在のサムネイル画像にフォーカスを当てる。
        setPosition(fcs, thumIndex);
        changeMainText(thumIndex);

    }

    function changeMainText(mi) {
        // テキストの変更
        var mvt = $("#mainvisual-txt");
        // テキストの繰り返しで一致するテキストを表示する。
        $("p", mvt).each(function(i, v) {
            if (i == mi) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    }

    // インターバルの設定。
    // メイン画像を一定の間隔で動かす
    var intervalName = setInterval(moveMainVisual, IT);

    // ボーダーのPX
    var borderPx = 3;

    // メイン画像のホバー、インターバルを調整
    photoSetMain.hover(

        function() {
            clearInterval(intervalName);
        },

        function() {
            intervalName = setInterval(moveMainVisual, IT);
        }
    );

    // サムネイルのオンマウスでカーソルをポインターにする。
    thums.css("cursor","pointer");

    // フォーカスエレメントのID
    var focusElementId = "thumbnailfocus";

    // フォーカスエレメントをbody直下に生成
    var fcs = $('<a href="javascript:void(0);return false;" id="'
        + focusElementId
        + '" style="cursor:pointer;border:'
        + borderPx
        + 'px solid #000; position:absolute;left:0;top:0;z-index:101;width:30px;height:30px;text-decoration:none;">'
        + '</a>').appendTo("body");


    // フォーカスエレメントのデフォルト位置を設定
    setPosition(fcs, 0);

    $(window).resize(function() {
        var pos = now;
        if (now >= numMain) {
            pos = now - numMain;
        }
        setPosition(fcs, pos);
    });

    // サムネイルフォーカスの位置とサイズを設定。
    function setPosition(focusElement, thumIndex, animation) {

        setLinkForFcs(thumIndex);

        var thum = thums.get(thumIndex);
        // フォーカスの移動をストップ
        focusElement.stop();
        // サムネイル画像の情報を取得（横幅、縦幅、位置）
        var width  = $("img", thum).width()  - (borderPx * 2);
        var height = $("img", thum).height() - (borderPx * 2);
        var left   = $(thum).offset().left;
        var top    = $(thum).offset().top;

        // 位置情報等をフォーカスに設定する。
        var setObj = {
            "width"    : width  + "px"
            , "height" : height + "px"
            , "left"   : left   + "px"
            , "top"    : top    + "px"
        };

        // 引数のアニメーションフラグがたっている場合はアニメーションでフォーカスを移動
        if (animation) {
            focusElement.animate(setObj);
        } else {
            focusElement.css(setObj);
        }
    }

    // サムネイルにカーソルを乗せたり乗せなかったりした時。
    thums.hover(
        function() {

            // サムネイルに乗ってる間インターバルを止める。
            clearInterval(intervalName);

            // サムネイルフォーカスの移動
            setPosition(fcs, thums.index(this));

            // メインヴィジュアルをサムネイルと対になる画像へと移動
            moveMainVisualCaseHov(thums.index(this));

        },

        function() {
            // カーソルがサムネイルから外れたとき、インターバル再開
            intervalName = setInterval(moveMainVisual, IT);
        }
    );

    // サムネイルがホバーされた時に呼び出される。
    // メイン画像の移動。
    function moveMainVisualCaseHov(hovIndex) {

        // メイン画像が動いていたら止める。
        photoSetMain.stop();
        // ホバーされたサムネイルの位置を取得
        var movePosition = hovIndex;
        if (now >= numMain) {
            // 2ループ目に入っているとき。
            movePosition = hovIndex + numMain;
        }
        // 現在のメイン画像の位置に移動するメイン画像の位置を設定
        now = movePosition;
        // 移動するメイン画像の位置を取得
        var moveLeft = (imageWidth * movePosition) * -1;
        // メイン画像を移動
        photoSetMain.animate({
            left : moveLeft
        }, SP);
        // メインヴィジュアルテキストの変更
        changeMainText(hovIndex);

        $(this).attr("href", $("p a", photoSetMain).get(hovIndex));
    }

    // フォーカスオブジェクトのホバー
    fcs.hover(
        function() {
            clearInterval(intervalName);
        },
        function() {
            intervalName = setInterval(moveMainVisual, IT);
        }
    );

    // メインヴィジュアルテキストの変更
    changeMainText(0);

    // フォーカスオブジェクトにリンクを設定
    function setLinkForFcs(mIndex) {
        
        fcs.attr("href", $("p a", photoSetMain).get(mIndex));
        fcs.attr("target", $($("p a", photoSetMain).get(mIndex)).attr("target"));
    }
});