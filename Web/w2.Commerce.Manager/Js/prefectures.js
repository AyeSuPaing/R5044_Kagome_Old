// 配送先 都道府県-グルーピング機能
$(function () {
  // 北海道
  $('[ID$="cbLocalArea1"]').click(function () {
    if ($('[ID$="cbLocalArea1"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_0"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_0"]').prop('checked', false);
    }
  });
  // 東北
  $('[ID$="cbLocalArea2"]').click(function () {
    if ($('[ID$="cbLocalArea2"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_1"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_2"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_3"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_4"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_5"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_6"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_1"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_2"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_3"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_4"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_5"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_6"]').prop('checked', false);
    }
  });
  // 関東
  $('[ID$="cbLocalArea3"]').click(function () {
    if ($('[ID$="cbLocalArea3"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_7"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_8"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_9"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_10"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_11"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_12"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_13"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_7"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_8"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_9"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_10"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_11"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_12"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_13"]').prop('checked', false);
    }
  });
  // 中部
  $('[ID$="cbLocalArea4"]').click(function () {
    if ($('[ID$="cbLocalArea4"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_14"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_15"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_16"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_17"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_18"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_19"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_20"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_21"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_22"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_14"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_15"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_16"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_17"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_18"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_19"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_20"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_21"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_22"]').prop('checked', false);
    }
  });
  // 近畿
  $('[ID$="cbLocalArea5"]').click(function () {
    if ($('[ID$="cbLocalArea5"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_23"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_24"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_25"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_26"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_27"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_28"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_29"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_23"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_24"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_25"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_26"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_27"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_28"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_29"]').prop('checked', false);
    }
  });
  // 中国
  $('[ID$="cbLocalArea6"]').click(function () {
    if ($('[ID$="cbLocalArea6"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_30"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_31"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_32"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_33"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_34"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_30"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_31"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_32"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_33"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_34"]').prop('checked', false);
    }
  });
  // 四国
  $('[ID$="cbLocalArea7"]').click(function () {
    if ($('[ID$="cbLocalArea7"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_35"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_36"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_37"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_38"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_35"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_36"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_37"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_38"]').prop('checked', false);
    }
  });
  // 九州・沖縄
  $('[ID$="cbLocalArea8"]').click(function () {
    if ($('[ID$="cbLocalArea8"]').prop('checked')) {
      $('[ID$="cblShippingPrefectures_39"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_40"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_41"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_42"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_43"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_44"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_45"]').prop('checked', true);
      $('[ID$="cblShippingPrefectures_46"]').prop('checked', true);
    } else {
      $('[ID$="cblShippingPrefectures_39"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_40"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_41"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_42"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_43"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_44"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_45"]').prop('checked', false);
      $('[ID$="cblShippingPrefectures_46"]').prop('checked', false);
    }
  });
});

// 地域別チェック自動デザイン
function setCheckStateForAllLocalAreaCheckBoxs() {
  // 北海道
  if ($('[ID$="cblShippingPrefectures_0"]').prop('checked')) {
    $('[ID$="cbLocalArea1"]').prop('checked', true);

  } else {
    $('[ID$="cbLocalArea1"]').prop('checked', false);
  }

  // 東北
  if ($('[ID$="cblShippingPrefectures_1"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_2"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_3"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_4"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_5"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_6"]').prop('checked')) {
    $('[ID$="cbLocalArea2"]').prop('checked', true);
  } else {
    $('[ID$="cbLocalArea2"]').prop('checked', false);
  };

  // 関東
  if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
    $('[ID$="cbLocalArea3"]').prop('checked', true);
  } else {
    $('[ID$="cbLocalArea3"]').prop('checked', false);
  }

  // 中部
  if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
    $('[ID$="cbLocalArea4"]').prop('checked', true);
  } else {
    $('[ID$="cbLocalArea4"]').prop('checked', false);
  }

  // 近畿
  if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
    $('[ID$="cbLocalArea5"]').prop('checked', true);
  } else {
    $('[ID$="cbLocalArea5"]').prop('checked', false);
  }

  // 中国
  if ($('[ID$="cblShippingPrefectures_30"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_31"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_32"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_33"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_34"]').prop('checked')) {
    $('[ID$="cbLocalArea6"]').prop('checked', true);
  } else {
    $('[ID$="cbLocalArea6"]').prop('checked', false);
  }

  // 四国
  if ($('[ID$="cblShippingPrefectures_35"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_36"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_37"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_38"]').prop('checked')) {
    $('[ID$="cbLocalArea7"]').prop('checked', true);
  } else {
    $('[ID$="cbLocalArea7"]').prop('checked', false);
  }

  // 九州・沖縄
  if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
    && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
    $('[ID$="cbLocalArea8"]').prop('checked', true);
  } else {
    $('[ID$="cbLocalArea8"]').prop('checked', false);
  }
};
$(function () {
  setCheckStateForAllLocalAreaCheckBoxs();
});

// 地域のチェックボックスのデザインを調整する
$(function () {
  // 地域を割り振る
  $('[ID="LocalArea8"]').insertBefore($('[ID$="cblShippingPrefectures_39"]'));
  $('[ID="LocalArea7"]').insertBefore($('[ID$="cblShippingPrefectures_35"]'));
  $('[ID="LocalArea6"]').insertBefore($('[ID$="cblShippingPrefectures_30"]'));
  $('[ID="LocalArea5"]').insertBefore($('[ID$="cblShippingPrefectures_23"]'));
  $('[ID="LocalArea4"]').insertBefore($('[ID$="cblShippingPrefectures_14"]'));
  $('[ID="LocalArea3"]').insertBefore($('[ID$="cblShippingPrefectures_7"]'));
  $('[ID="LocalArea2"]').insertBefore($('[ID$="cblShippingPrefectures_1"]'));
  $('[ID="LocalArea1"]').insertBefore($('[ID$="cblShippingPrefectures_0"]'));

  // 地域と都道府県の境界線デザインを入れる
  $('[ID$="cblShippingPrefectures_39"]').before("&nbsp;&nbsp;&nbsp;|");
  $('[ID$="cblShippingPrefectures_35"]').before("&emsp;&emsp;&emsp;|");
  $('[ID$="cblShippingPrefectures_30"]').before("&emsp;&emsp;&emsp;|");
  $('[ID$="cblShippingPrefectures_23"]').before("&emsp;&emsp;&emsp;|");
  $('[ID$="cblShippingPrefectures_14"]').before("&emsp;&emsp;&emsp;|");
  $('[ID$="cblShippingPrefectures_7"]').before("&emsp;&emsp;&emsp;|");
  $('[ID$="cblShippingPrefectures_1"]').before("&emsp;&emsp;&emsp;|");
  $('[ID$="cblShippingPrefectures_0"]').before("&emsp;&emsp;|");

  // 各都道府県を地域ごとに分割する
  $('[ID="LocalArea8"]').before("<br>");
  $('[ID="LocalArea7"]').before("<br>");
  $('[ID="LocalArea6"]').before("<br>");
  $('[ID="LocalArea5"]').before("<br>");
  $('[ID="LocalArea4"]').before("<br>");
  $('[ID="LocalArea3"]').before("<br>");
  $('[ID="LocalArea2"]').before("<br>");

  // 北海道
  $('[ID$="cblShippingPrefectures_0"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_0"]').prop('checked')) {
      $('[ID$="cbLocalArea1"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea1"]').prop('checked', false);
    }
  });

  // 東北
  $('[ID$="cblShippingPrefectures_1"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_1"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_2"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_3"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_4"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_5"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_6"]').prop('checked')) {
      $('[ID$="cbLocalArea2"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea2"]').prop('checked', false);
    };
  });
  $('[ID$="cblShippingPrefectures_2"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_1"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_2"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_3"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_4"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_5"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_6"]').prop('checked')) {
      $('[ID$="cbLocalArea2"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea2"]').prop('checked', false);
    };
  });
  $('[ID$="cblShippingPrefectures_3"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_1"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_2"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_3"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_4"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_5"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_6"]').prop('checked')) {
      $('[ID$="cbLocalArea2"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea2"]').prop('checked', false);
    };
  });
  $('[ID$="cblShippingPrefectures_4"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_1"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_2"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_3"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_4"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_5"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_6"]').prop('checked')) {
      $('[ID$="cbLocalArea2"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea2"]').prop('checked', false);
    };
  });
  $('[ID$="cblShippingPrefectures_5"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_1"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_2"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_3"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_4"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_5"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_6"]').prop('checked')) {
      $('[ID$="cbLocalArea2"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea2"]').prop('checked', false);
    };
  });
  $('[ID$="cblShippingPrefectures_6"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_1"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_2"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_3"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_4"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_5"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_6"]').prop('checked')) {
      $('[ID$="cbLocalArea2"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea2"]').prop('checked', false);
    };
  });

  // 関東
  $('[ID$="cblShippingPrefectures_7"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
      $('[ID$="cbLocalArea3"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea3"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_8"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
      $('[ID$="cbLocalArea3"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea3"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_9"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
      $('[ID$="cbLocalArea3"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea3"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_10"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
      $('[ID$="cbLocalArea3"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea3"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_11"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
      $('[ID$="cbLocalArea3"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea3"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_12"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
      $('[ID$="cbLocalArea3"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea3"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_13"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_7"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_8"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_9"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_10"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_11"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_12"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_13"]').prop('checked')) {
      $('[ID$="cbLocalArea3"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea3"]').prop('checked', false);
    }
  });
  // 中部
  $('[ID$="cblShippingPrefectures_14"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_15"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_16"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_17"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_18"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_19"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_20"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_21"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_22"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_14"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_15"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_16"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_17"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_18"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_19"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_20"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_21"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_22"]').prop('checked')) {
      $('[ID$="cbLocalArea4"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea4"]').prop('checked', false);
    }
  });
  // 近畿
  $('[ID$="cblShippingPrefectures_23"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
      $('[ID$="cbLocalArea5"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea5"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_24"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
      $('[ID$="cbLocalArea5"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea5"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_25"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
      $('[ID$="cbLocalArea5"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea5"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_26"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
      $('[ID$="cbLocalArea5"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea5"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_27"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
      $('[ID$="cbLocalArea5"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea5"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_28"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
      $('[ID$="cbLocalArea5"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea5"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_29"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_23"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_24"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_25"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_26"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_27"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_28"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_29"]').prop('checked')) {
      $('[ID$="cbLocalArea5"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea5"]').prop('checked', false);
    }
  });
  // 中国
  $('[ID$="cblShippingPrefectures_30"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_30"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_31"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_32"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_33"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_34"]').prop('checked')) {
      $('[ID$="cbLocalArea6"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea6"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_31"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_30"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_31"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_32"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_33"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_34"]').prop('checked')) {
      $('[ID$="cbLocalArea6"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea6"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_32"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_30"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_31"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_32"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_33"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_34"]').prop('checked')) {
      $('[ID$="cbLocalArea6"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea6"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_33"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_30"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_31"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_32"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_33"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_34"]').prop('checked')) {
      $('[ID$="cbLocalArea6"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea6"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_34"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_30"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_31"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_32"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_33"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_34"]').prop('checked')) {
      $('[ID$="cbLocalArea6"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea6"]').prop('checked', false);
    }
  });
  // 四国
  $('[ID$="cblShippingPrefectures_35"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_35"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_36"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_37"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_38"]').prop('checked')) {
      $('[ID$="cbLocalArea7"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea7"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_36"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_35"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_36"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_37"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_38"]').prop('checked')) {
      $('[ID$="cbLocalArea7"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea7"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_37"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_35"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_36"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_37"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_38"]').prop('checked')) {
      $('[ID$="cbLocalArea7"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea7"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_38"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_35"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_36"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_37"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_38"]').prop('checked')) {
      $('[ID$="cbLocalArea7"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea7"]').prop('checked', false);
    }
  });
  // 九州・沖縄
  $('[ID$="cblShippingPrefectures_39"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_40"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_41"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_42"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_43"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_44"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_45"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
  $('[ID$="cblShippingPrefectures_46"]').click(function () {
    if ($('[ID$="cblShippingPrefectures_39"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_40"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_41"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_42"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_43"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_44"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_45"]').prop('checked')
      && $('[ID$="cblShippingPrefectures_46"]').prop('checked')) {
      $('[ID$="cbLocalArea8"]').prop('checked', true);
    } else {
      $('[ID$="cbLocalArea8"]').prop('checked', false);
    }
  });
});