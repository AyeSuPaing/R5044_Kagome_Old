var KEY_ACCOUNT_ID="__account_id",KEY_ACCESS_USER_ID="__access_user_id",KEY_SESSION_ID="__session_id",KEY_ACCESS_ID="__acc_id",KEY_USER_ID="__real_user_id",KEY_FIRST_LOGIN_FLG="__first_login_flg",KEY_REFERRER="__referrer",KEY_ACTION_KBN="__action_kbn",KEY_ACTION_PARAM="__action_param",KEY_ACS_INTERVAL="__acs_interval",KEY_LAST_ACS_DATE="__last_acs_date",KEY_URL_DOMAIN="__url_domain",KEY_URL_PAGE="__url_page",KEY_URL_PARAM="__url_param",KEY_SEARCH_ENGINE="__srch_engn",KEY_SEARCH_DOMAIN="__srch_word",
KBN_ACTION_LOGIN="__login",KBN_ACTION_LEAVE="__leave",KBN_ACTION_LOGIN_USERID="s1",KBN_ACTION_LEAVE_USERID="s1",KBN_ACTION_S_HEAD="__s_",KBN_ACTION_P_HEAD="__p_",alSrchEngineName=[],alSrchReqKey=[];
(function(){var a=0;alSrchEngineName[a]="google";alSrchReqKey[a++]="q";alSrchEngineName[a]="yahoo";alSrchReqKey[a++]="p";alSrchEngineName[a]="msn";alSrchReqKey[a++]="q";alSrchEngineName[a]="goo";alSrchReqKey[a++]="MT";alSrchEngineName[a]="livedoor";alSrchReqKey[a++]="q";alSrchEngineName[a]="nifty";alSrchReqKey[a++]="Text";alSrchEngineName[a]="infoseek";alSrchReqKey[a++]="qt";alSrchEngineName[a]="excite";alSrchReqKey[a++]="qkw";alSrchEngineName[a]="excite";alSrchReqKey[a++]="search";alSrchEngineName[a]=
"lycos";alSrchReqKey[a++]="query";alSrchEngineName[a]="ask";alSrchReqKey[a++]="q";alSrchEngineName[a]="rssnavi";alSrchReqKey[a++]="q";alSrchEngineName[a]="allabout";alSrchReqKey[a++]="qs";alSrchEngineName[a]="aol";alSrchReqKey[a++]="query";alSrchEngineName[a]="altavista";alSrchReqKey[a++]="q";alSrchEngineName[a]="netscape";alSrchReqKey[a++]="query";alSrchEngineName[a]="earthlink";alSrchReqKey[a++]="q";alSrchEngineName[a]="cnn";alSrchReqKey[a++]="query";alSrchEngineName[a]="looksmart";alSrchReqKey[a++]=
"key";alSrchEngineName[a]="about";alSrchReqKey[a++]="terms";alSrchEngineName[a]="mamma";alSrchReqKey[a++]="query";alSrchEngineName[a]="alltheweb";alSrchReqKey[a++]="q";alSrchEngineName[a]="gigablast";alSrchReqKey[a++]="q";alSrchEngineName[a]="voila";alSrchReqKey[a++]="kw";alSrchEngineName[a]="virgilio";alSrchReqKey[a++]="qs";alSrchEngineName[a]="teoma";alSrchReqKey[a++]="q";alSrchEngineName[a]="fresheye";alSrchReqKey[a++]="kw";alSrchEngineName[a]="search";alSrchReqKey[a++]="q";alSrchEngineName[a]=
"bing";alSrchReqKey[a++]="q"})();var strDomainHash=get_domain_hash();function getlog(){return getlog_for_action(w2accesslog_account_id,"","",w2accesslog_target_domain)}function getlog_for_login(a){return getlog_for_action(w2accesslog_account_id,KBN_ACTION_LOGIN,KBN_ACTION_LOGIN_USERID+"="+a,w2accesslog_target_domain)}function getlog_for_leave(a){return getlog_for_action(w2accesslog_account_id,KBN_ACTION_LEAVE,KBN_ACTION_LEAVE_USERID+"="+a,w2accesslog_target_domain)}
function getlog_for_action(a,b,f,d){var c,g="",h="",e="",l="";if(null==d||-1!=(","+d+",").indexOf(","+document.domain+",")){d=f.replace(",,","@@@escape_comma@@@").split(",");f=[];for(g=0;g<d.length;g++)f[d[g].substring(0,d[g].indexOf("="))]=d[g].substring(d[g].indexOf("=")+1,d[g].length);c=!1;g=get_access_user_id(document.cookie);""==g&&(g=create_cookie_id(),c=!0);var m=!1,h=get_access_session_id(document.cookie);""==h&&(h=create_cookie_id(),m=!0);e=create_cookie_id();d=!1;l=get_user_id(document.cookie);
b!=KBN_ACTION_LOGIN||""!=l&&l==f[KBN_ACTION_LOGIN_USERID]||(l=strDomainHash+"."+f[KBN_ACTION_LOGIN_USERID],d=!0);var n=(new Date).getTime(),k=get_acs_msec(document.cookie),p=-1;""!=k&&(p=parseInt((n-k)/1E3/60/60/24));k=" path="+w2accesslog_cookie_root+";";!0==c&&(document.cookie=KEY_ACCESS_USER_ID+"="+g+"; expires=Sun, 18 Jan 2038 00:00:00 GMT;"+k);!0==m&&(document.cookie=KEY_SESSION_ID+"="+h+";"+k);!0==d&&(document.cookie=KEY_USER_ID+"="+l+"; expires=Sun, 18 Jan 2038 00:00:00 GMT;"+k);document.cookie=
KEY_LAST_ACS_DATE+"="+strDomainHash+"."+n+"; expires=Sun, 18 Jan 2038 00:00:00 GMT;"+k;c=get_referrer();null==b&&(b="");e=""+(KEY_ACCESS_ID+"="+encode_url(e));e+="&"+KEY_ACCOUNT_ID+"="+encode_url(a);e+="&"+KEY_ACCESS_USER_ID+"="+encode_url(g);e+="&"+KEY_SESSION_ID+"="+encode_url(h);e+="&"+KEY_USER_ID+"="+encode_url(l.replace(strDomainHash+".",""));e+="&"+KEY_REFERRER+"="+encode_url(c);e+="&"+KEY_ACTION_KBN+"="+encode_url(b);a=KBN_ACTION_S_HEAD;for(g=1;2>=g;g++){for(b=1;20>=b;b++)h=f[a.replace(/_/g,
"")+b],null!=h&&(e+="&"+a+b+"="+encode_url(h.replace("@@@escape_comma@@@",",")));a=KBN_ACTION_P_HEAD}e+="&"+KEY_FIRST_LOGIN_FLG+"="+(!0==d?"1":"0");e+="&"+KEY_ACS_INTERVAL+"="+p;e+="&"+KEY_URL_DOMAIN+"="+encode_url(document.domain);e+="&"+KEY_URL_PAGE+"="+encode_url(location.pathname);e+="&"+KEY_URL_PARAM+"="+encode_url(0!=location.search.length?location.search.substring(1,location.search.length):"");e+="&"+get_search_engine_info(document.referrer);"https:"==location.protocol&&(w2accesslog_getlog_path=
w2accesslog_getlog_path.replace("http://","https://"));f=new Image(1,1);f.src=w2accesslog_getlog_path+"?"+e;f.onload=function(){_uVoid()}}}function _uVoid(){}function get_access_user_id(a){return get_cookie_value(a,KEY_ACCESS_USER_ID)}function get_access_session_id(a){return get_cookie_value(a,KEY_SESSION_ID)}function get_user_id(a){return get_cookie_value(a,KEY_USER_ID)}function get_acs_msec(a){return get_cookie_value(a,KEY_LAST_ACS_DATE).replace(strDomainHash+".","")}
function get_cookie_value(a,b){var f="",d=a.indexOf(b+"="+strDomainHash);-1!=d&&(f=a.indexOf(";",d),-1==f&&(f=a.length),d=a.substring(d,f),f=d.substring(d.indexOf("=")+1,d.length));return f}function create_cookie_id(){var a=Math.round((new Date).getTime()/1E3),b=Math.round(2147483647*Math.random());return strDomainHash+"."+a+"."+b}
function get_referrer(){var a="";try{a=document.referrer!=parent.frames.location?document.referrer:top.document.referrer}catch(b){a=document.referrer}0!=a.length&&(a=a.replace("http://",""),a=a.replace("https://",""));return a}
function get_search_engine_info(a){var b=0,f="",d="";if(0<(b=a.indexOf("://"))){var c=a.substring(b+3,a.length);-1<c.indexOf("/")&&(c=c.substring(0,c.indexOf("/")));for(var g=0;g<alSrchEngineName.length;g++)if(-1<c.indexOf(alSrchEngineName[g]+".")&&(-1<(b=a.indexOf("?"+alSrchReqKey[g]+"="))||-1<(b=a.indexOf("&"+alSrchReqKey[g]+"=")))){a=a.substring(b+alSrchReqKey[g].length+2,a.length);-1<(b=a.indexOf("&"))&&(a=a.substring(0,b));f=alSrchEngineName[g];d=a;break}}return KEY_SEARCH_ENGINE+"="+encode_url(f)+
"&"+KEY_SEARCH_DOMAIN+"="+d}function get_domain_hash(){var a=document.domain;"www."==a.substring(0,4)&&(a=a.substring(4,a.length));return get_hash(a)}function get_hash(a){for(var b=0,f=0,d=a.length-1;0<=d;d--){var c=parseInt(a.charCodeAt(d)),b=(b<<6&268435455)+c+(c<<14);0!=(f=b&266338304)&&(b^=f>>21)}return b}
function encode_url(a){var b,f,d,c;b="";for(f=0;f<a.length;f++)d=a.charAt(f),c=a.charCodeAt(f)," "==d?b+="+":42==c||45==c||46==c||95==c||48<=c&&57>=c||65<=c&&90>=c||97<=c&&122>=c?b+=d:0<=c&&127>=c?(d="0"+c.toString(16),b+="%"+d.substr(d.length-2)):(2097151<c?(b+="%"+(oxf0+((c&1835008)>>18)).toString(16),b+="%"+(128+((c&258048)>>12)).toString(16),b+="%"+(128+((c&4032)>>6)).toString(16)):2047<c?(b+="%"+(224+((c&61440)>>12)).toString(16),b+="%"+(128+((c&4032)>>6)).toString(16)):b+="%"+(192+((c&1984)>>
6)).toString(16),b+="%"+(128+(c&63)).toString(16));return b};