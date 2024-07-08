// フォーカスアウト・エンターキーダウンポストバックイベント用実行時間・初期化
var lastBlurOnEnterTime;

// フォーカスアウト・エンターキーダウンポストバックイベント用実行時間初期化
function InitializeLastBlurOnEnterTime()
{
    lastBlurOnEnterTime = "ok";
}
// フォーカスアウト・エンターキーダウンポストバックイベント用実行時間初期化
function ResetLastBlurOnEnterTime()
{
    lastBlurOnEnterTime = null;
}
// フォーカスアウト・エンターキーダウンイベント実行可能判定
function CheckBlurOnEnterEnabled()
{
    return (lastBlurOnEnterTime == "ok");
}