function getCookie(name, options) {
  var cookies = document.cookie.split('; ');
  for (var i = 0; i < cookies.length; i++) {
    var cookie = cookies[i].trim();
    var cookieParts = cookie.split('=');
    var cookieName = decodeURIComponent(cookieParts[0]);

    if (cookieName === name) {
      var cookieValue = decodeURIComponent(cookieParts[1]);

      if (options) {
        setCookie(name, cookieValue, options);
      }
      return cookieValue;
    }
  }
  return null;
}

function setCookie(name, value, options) {
  var cookieString = encodeURIComponent(name) + '=' + encodeURIComponent(value);

  if (options) {
    if (options.path) {
      cookieString += '; path=' + options.path;
    }
    if (options.expires) {
      var expirationDate = new Date();
      expirationDate.setDate(expirationDate.getDate() + options.expires);
      cookieString += '; expires=' + expirationDate.toUTCString();
    }
  }

  document.cookie = cookieString;
}
