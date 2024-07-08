$(function () {
    //ラジオボタンが入っているdivの数を数える
    var $radio_count = $("._radio").size();
    var $check_count = $("._check").size();
    $("._radio input[type='radio'],._check input[type='checkbox']").on('change', function () {
        var $checked_radio_count = $("._radio").find('input[type="radio"]:checked').size();
        var $checked_check_count = $("._check").find('input[type="checkbox"]:checked').size();
        if ($checked_radio_count == $radio_count && $checked_check_count >= $check_count) {
            $(".scoringsale_btn._go").removeClass('disable');
        } else {
            $(".scoringsale_btn._go").addClass('disable');
        }
    });

    var $checked_radio_count = $("._radio").find('input[type="radio"]:checked').size();
    var $checked_check_count = $("._check").find('input[type="checkbox"]:checked').size();
    if ($checked_radio_count == $radio_count && $checked_check_count >= $check_count) {
      $(".scoringsale_btn._go").removeClass('disable');
    } else {
      $(".scoringsale_btn._go").addClass('disable');
    }

    $('.scoringsale_product_img_main').slick({
        autoplay: true,
        autoplaySpeed: 5000,
        dots: false,
        // variableWidth: true,
        pauseOnFocus: false,
        pauseOnHover: false,
        pauseOnDotsHover: false,
        asNavFor: '.scoringsale_product_img_thumb',
    });

    $('.scoringsale_product_img_thumb').slick({
        autoplay: true,
        autoplaySpeed: 5000,
        dots: false,
        arrows: true,
        slidesToShow: 6,
        focusOnSelect: true,
        variableWidth: true,
        asNavFor: '.scoringsale_product_img_main',
        responsive: [{
            breakpoint: 769,  //ブレイクポイントを指定
            settings: {
                slidesToShow: 4,
            }
        }]
    });
});