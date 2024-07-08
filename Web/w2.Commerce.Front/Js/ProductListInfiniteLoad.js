const ProductListInfiniteLoad = (function () {
  // プライベート変数
  let loadingAnimationObserver;
  let productElementsObserver;
  let useInfiniteLoadProductList;
  let hfPageNumberClientId;
  let hfDisplayPageNumberMaxClientId;
  let lbInfiniteLoadingUpperNextButtonClientId;
  let lbInfiniteLoadingLowerNextButtonClientId;
  let oldPageNumber;
  let pagenationThreshold;

  // 初期化関数
  function init(config) {
    useInfiniteLoadProductList = config.useInfiniteLoadProductList === "True";
    hfPageNumberClientId = config.hfPageNumberClientId || $("[id$='hfPageNumber']").attr("id");
    hfDisplayPageNumberMaxClientId = config.hfDisplayPageNumberMaxClientId || $("[id$='hfDisplayPageNumberMax']").attr("id");
    lbInfiniteLoadingUpperNextButtonClientId = config.lbInfiniteLoadingUpperNextButtonClientId || $("[id$='lbInfiniteLoadingUpperNextButton']").attr("id");
    lbInfiniteLoadingLowerNextButtonClientId = config.lbInfiniteLoadingLowerNextButtonClientId || $("[id$='lbInfiniteLoadingLowerNextButton']").attr("id");
    pagenationThreshold = config.pagenationThreshold || 0.5;

    if (useInfiniteLoadProductList === false) return;

    const url = new URL(window.location.href);
    const pageNumber = Number(url.searchParams.get("pno"));

    oldPageNumber = pageNumber;

    $(window).scrollTop(0);

    loadingAnimationObserver = new IntersectionObserver(detectionLoadingAnimation, {});
    productElementsObserver = new IntersectionObserver(detectionProductElement, { threshold: pagenationThreshold });

    // 監視対象の要素をセット
    setObserverProductElements();
    setObserverLoadingAnimationElements();

    // PageRequestManagerのインスタンスを作成
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
  }

  // UpdatePanel更新後処理
  function endRequestHandler(sender, args) {
    // ページ番号を取得して変更があれば更新
    const pageNumber = $("#" + hfPageNumberClientId).val();
    // SPの場合表示形式保持
    if (pagenationThreshold == 0.1) {
      if (sessionStorage.displayType == "list") {
        $("#btn-layout-list").click();
      }
      else {
        $("#btn-layout-list-2").click();
      }
    }

    // デザイン系の処理
    let $variation;
    $('.windowpanel').heightLine().biggerlink({ otherstriggermaster: false }).hover(function () {
      $('.windowpanel .variationview_wrap').hide();

      $variation = $(this).find('.variationview_wrap');
      if ($variation) $variation.show();
    }, function () {
      if ($variation) $variation.hide();
    });
    $('.variationview_bg').heightLine().biggerlink({ otherstriggermaster: false });
    $(loadingAnimationElement).find("img").css("opacity", "0");

    if (pageNumber != oldPageNumber) {

      updatePageNumber(pageNumber);

      // loadingAnimationElementのIDに応じてスクロール位置を設定
      if ($(loadingAnimationElement).length > 0) {
        let top = $(".infiniteLoadProducts");
        switch ($(loadingAnimationElement).attr("id")) {
          case "loadingAnimationUpper":
            if (($(".infiniteLoadProducts").eq(pageNumber).offset() == null) || (typeof $(".infiniteLoadProducts").eq(pageNumber).offset().top === 'undefined')) {
              return;
            }
            top = top.eq(pageNumber).offset().top - 50;
            $(window).scrollTop(top);
            break;

          case "loadingAnimationLower":
            const maxPageNumber = $("#" + hfDisplayPageNumberMaxClientId).val();
            if (maxPageNumber === "") break;
            let viewportHeight = window.innerHeight;
            let elementBottom = top.eq(maxPageNumber - 1).offset().top + top.eq(maxPageNumber - 1).height();
            let newScrollPosition = elementBottom - viewportHeight;
            $(window).scrollTop(newScrollPosition + 150);
            break;
        }
      }
    }

    // 監視対象の要素をセット
    setObserverLoadingAnimationElements();
    setObserverProductElements();
  }

  // 監視対象に商品情報要素をセット
  function setObserverProductElements() {
    productElementsObserver.disconnect();
    const productElementAll = $(".infiniteLoadProducts");
    const targetProductElements = productElementAll.filter(function (_, element) {
      return $(element).find("[id*='dInfiniteLoadProduct']").length > 0;
    });
    $.each(targetProductElements, function (_, productElement) {
      productElementsObserver.observe(productElement);
    });
  }

  // 監視対象にローディングアニメーション要素をセット
  function setObserverLoadingAnimationElements() {
    loadingAnimationObserver.disconnect();
    const isNeedLoadingUpperPruducts = $(".infiniteLoadProducts").eq(0).find("[id*='dInfiniteLoadProduct']").length === 0;
    const isNeedLoadingLowerPruducts = $(".infiniteLoadProducts").eq(-1).find("[id*='dInfiniteLoadProduct']").length === 0;
    if ((isNeedLoadingUpperPruducts === false) && (isNeedLoadingLowerPruducts === false)) return;

    const loadingAnimations = [
      isNeedLoadingLowerPruducts
        ? document.getElementById("loadingAnimationLower")
        : null,
      isNeedLoadingUpperPruducts
        ? document.getElementById("loadingAnimationUpper")
        : null,
    ];
    loadingAnimations.forEach(function (animationElement) {
      if (animationElement == null) return;
      loadingAnimationObserver.observe(animationElement);
    });
  }

  // 商品情報要素を検知
  function detectionProductElement(entries) {
    entries.forEach(function (entry) {
      if (entry.isIntersecting === false) return;
      const productElementAll = $(".infiniteLoadProducts");
      const pageNumber = Array.from(productElementAll).indexOf(entry.target) + 1;
      updatePageNumber(pageNumber);
    });
  }

  let loadingAnimationElement;
  // ローディングアニメーションを検知
  function detectionLoadingAnimation(entries) {
    entries.forEach(function (entry) {
      const targetId = $(entry.target).attr("id");
      if (entry.isIntersecting === false) return;
      loadingAnimationElement = $(entry.target);
      $(loadingAnimationElement).find("img").css("opacity", "1");

      if (targetId === "loadingAnimationUpper") {
        $("#" + lbInfiniteLoadingUpperNextButtonClientId)[0].click();
      }
      if (targetId === "loadingAnimationLower") {
        $("#" + lbInfiniteLoadingLowerNextButtonClientId)[0].click();
      }
    });
  }

  // ページ番号を更新
  function updatePageNumber(pageNumber) {
    const url = new URL(window.location.href);
    oldPageNumber = pageNumber;
    url.searchParams.set("pno", pageNumber);
    history.pushState({}, "", url);
  }

  return {
    init: init
  };
})();
