//IE 文字ズレ調整
var ie_fixed = {
  ini : function(){
    var userAgent = window.navigator.userAgent.toLowerCase();
    if (userAgent.indexOf('trident/7') !== -1) {
      console.log('ie_fixed.ini()');
      $('.f-serif').each(function() {
        var fs = Number($(this).css('font-size').replace('px', ''));
        var shift_position_px = fs * 0.1;
        $(this).css({
          'transform': 'translateY(' + shift_position_px + 'px)'
        });
      });
      $('.f-sawarabimincho').each(function() {
        var fs = Number($(this).css('font-size').replace('px', ''));
        var shift_position_px = fs * 0.1;
        $(this).css({
          'transform': 'translateY(' + shift_position_px + 'px)'
        });
      });
      
    }
  }
}
$(function() {
  ie_fixed.ini();
});
