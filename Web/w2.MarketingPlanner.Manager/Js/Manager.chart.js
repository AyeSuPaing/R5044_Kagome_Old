// Copyright (c) 2019 chart.js
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

var YAXES_FONT_SIZE = 10;
var YAXES_SCALE_VALUE = 5;
var YAXES_DEFAULT_MAX_VALUE = 1;
var YAXES_DEFAULT_MIN_VALUE = 0;

var XAXES_FONT_SIZE = 15;

var POINT_RADIUS = 6;
var LINE_TENSION = 0;

function CreateCharts(month, jsonValues, chartType, xAxisTitle, chartTitle, valueUnit, isViewNumber, graphKbn) {
	var dataValues = []; // x軸グラフデータ
	var labelValues = []; // y軸ラベルデータ
	var hoverColor; // マウスカーソルが乗ったときのカラー
	var backColor; // グラフのカラー
	for (var i = 0; i < jsonValues.length; i++) {
		var tmpDataValue = Math.floor(jsonValues[i]["value"]);
		dataValues[i] = (tmpDataValue < 0) ? 0 : tmpDataValue;
		labelValues[i] = String.format("{0}/{1}", month, jsonValues[i]["day"]);
		}

	switch (graphKbn) {
		case "report":
			hoverColor = "rgba(20,20,255,0.7)";
			backColor = "rgba(20,20,100,0.3)";
			break;
		case "ranking":
			hoverColor = "rgba(20,255,20,0.7)";
			backColor = "rgba(20,100,20,0.3)";
			break;
		default:
			hoverColor = "";
			backColor = "";
			break;
	}

	// y軸の最大値を設定します()
	var maxDataValue = Math.max.apply(null, dataValues);
	var yAxesMaxValue = ((maxDataValue == 0) == false) ? Math.ceil(maxDataValue * 1.2) : YAXES_DEFAULT_MAX_VALUE;

	// データ設定
	var mydata = {
		labels: labelValues, // x軸目盛り(日付)
		datasets: [
			{
				label: valueUnit, // 数値の単位
				hoverBackgroundColor: hoverColor, // マウスがフォーカスしたデータのグラフのカラー
				backgroundColor: backColor, // デフォルトグラフのカラー
				data: dataValues, // データ
				fill: false, // labelの表示のありなし
				pointRadius: POINT_RADIUS, // 点の大きさ(折れ線グラフ用)
				lineTension: LINE_TENSION // 直線(折れ線グラフ用)
			}
		]
	};

	// オプション設定
	var options = {
		title: {
			display: true,
			text: chartTitle + " [" + valueUnit + "]", // グラフのタイトル
			position: "top" // タイトルの位置
		},
		legend: {
			display: false
		},
		tooltips: { // ツールチップ設定(マウスカーソルがHoverした時のポップの中)
			callbacks: {
				label: function(tooltipItem) {
					var label = String.format("{0} {1}",
						tooltipItem.yLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","),
						valueUnit);
					return label;
				}
			}
		},
		scales: {
			yAxes: [ // y軸設定
				{
					display: true,
					scaleLabel: { // y軸ラベル設定
						display: false,
						labelString: valueUnit,
						fontSize: YAXES_FONT_SIZE,
						fontFamily: "arial"
					},
					ticks: {
						suggestedMax: yAxesMaxValue, // 最大表示数
						min: YAXES_DEFAULT_MIN_VALUE, // 最小表示数

						userCallback:
							function(label) {
								var result = (Math.floor(label) === label)
									? label.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") // 1000単位カンマ区切り&小数点表示なし
									: "";
								return result;
							}
					}
				}
			],
			xAxes: [// x軸設定
				{
					display: true,
					scaleLabel: { // x軸ラベル設定
						display: true,
						labelString: xAxisTitle,
						fontSize: XAXES_FONT_SIZE,
						fontFamily: "arial"
					}
				}
			]
		}
	};

	// 数値表示プラグイン
	var dataLabelPlugin = {
		afterDatasetsDraw: function (chart) {
			var ctx = chart.ctx;
			chart.data.datasets.forEach(function (dataset, series) {
				var meta = chart.getDatasetMeta(series);
				if (!meta.hidden) {
					meta.data.forEach(function (element, index) {
						// ステップ１　数値を文字列に変換
						var dataString = dataset.data[index].toString();

						// ステップ２　文字列の書体
						ctx.fillStyle = "rgba(80,80,80,0.7)"; // 色　'rgb(0, 0, 0)', 'rgba(192, 80, 77, 0.7)'
						var fontSize = 12; // サイズ
						var fontStyle = "normal"; // 書体 "bold", "italic"
						var fontFamily = "Meiryo"; // フォントの種類 "sans-serif", "ＭＳ 明朝"
						ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);

						// ステップ３　文字列の位置の基準点
						ctx.textAlign = "center"; // 文字列　start, end, left, right, center
						ctx.textBaseline = "middle"; // 文字高　middle, top, bottom

						// ステップ４　文字列のグラフでの位置
						var padding = 3; // 点と文字列の距離
						var position = element.tooltipPosition();
						//文字列の表示　 fillText(文字列, Ｘ位置, Ｙ位置)
						ctx.fillText(dataString.replace(/\B(?=(\d{3})+(?!\d))/g, ","),
							position.x,
							position.y - (fontSize / 2) - padding);
					});
				}
			});
		}
	}

	var plugs = (isViewNumber === "True") ? dataLabelPlugin : "";
	var element = document.getElementById("Chart");
	var chart = new window.Chart(element,
		{
			type: chartType , // グラフの種類
			data: mydata, // 表示するデータ
			options: options, // オプション設定
			plugins: [plugs] // 数値表示設定
		});
}

// レコメンドレポート
function CreateRecommendReportCharts(jsonValues, chartType, xAxisTitle, chartTitle, valueUnit, isViewNumber, isPercent, graphKbn)
{
	// x軸グラフデータ
	var dataValues = [];
	// y軸ラベルデータ
	var labelValues = [];
	// マウスカーソルが乗ったときのカラー
	var hoverColor;
	// グラフのカラー
	var backColor;
	for (var i = 0; i < jsonValues.length; i++)
	{
		var tmpDataValue = Math.floor(jsonValues[i]["value"]);
		dataValues[i] = (tmpDataValue < 0) ? 0 : tmpDataValue;
		// 「月」と「日」情報を取得する
		labelValues[i] = jsonValues[i]["day"].substr(5);
	}

	switch (graphKbn)
	{
		case "report":
			hoverColor = "rgba(20,20,255,0.7)";
			backColor = "rgba(20,20,100,0.3)";
			break;
		case "ranking":
			hoverColor = "rgba(20,255,20,0.7)";
			backColor = "rgba(20,100,20,0.3)";
			break;
		default:
			hoverColor = "";
			backColor = "";
			break;
	}

	// y軸の最大値を設定します
	var maxDataValue = Math.max.apply(null, dataValues);
	// パーセント表示対応
	var yAxesMaxValue = (isPercent === "True")
		? 100
		: ((maxDataValue === 0) === false)
			? Math.ceil(maxDataValue * 1.2)
			: YAXES_DEFAULT_MAX_VALUE

	// データ設定
	var mydata =
	{
		// x軸目盛り(日付)
		labels: labelValues,
		datasets:
		[{
			// 数値の単位
			label: valueUnit,
			// マウスがフォーカスしたデータのグラフのカラー
			hoverBackgroundColor: hoverColor,
			// デフォルトグラフのカラー
			backgroundColor: backColor,
			// データ
			data: dataValues,
			// labelの表示のありなし
			fill: false,
			// 点の大きさ(折れ線グラフ用)
			pointRadius: POINT_RADIUS,
			// 直線(折れ線グラフ用)
			lineTension: LINE_TENSION
		}]
	};

	// オプション設定
	var options =
	{
		title:
		{
			display: true,
			// グラフのタイトル
			text: chartTitle + " [" + valueUnit + "]",
			// タイトルの位置
			position: "top"
		},
		legend:
		{
			display: false
		},
		// ツールチップ設定(マウスカーソルがHoverした時のポップの中)
		tooltips:
		{
			callbacks:
			{
				label:function (tooltipItem)
				{
					var label = String.format("{0} {1}",
						tooltipItem.yLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","),
						valueUnit);
					return label;
				}
			}
		},
		scales:
		{
			// y軸設定
			yAxes:
			[{
				display: true,
				// y軸ラベル設定
				scaleLabel:
				{
					display: false,
					labelString: valueUnit,
					fontSize: YAXES_FONT_SIZE,
					fontFamily: "arial"
				},
				ticks: {
					// 最大表示数
					suggestedMax: yAxesMaxValue,
					// 最小表示数
					min: YAXES_DEFAULT_MIN_VALUE,
					userCallback: function (label)
					{
						var result = (Math.floor(label) === label)
							// 1000単位カンマ区切り&小数点表示なし
							? label.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
							: "";
						return result;
					}
				}
			}],
			// x軸設定
			xAxes:
			[{
				display: true,
				// x軸ラベル設定
				scaleLabel:
				{
					display: true,
					labelString: xAxisTitle,
					fontSize: XAXES_FONT_SIZE,
					fontFamily: "arial"
				}
			}]
		}
	};

	// 数値表示プラグイン
	var dataLabelPlugin =
	{
		afterDatasetsDraw: function (chart)
		{
			var ctx = chart.ctx;
			chart.data.datasets.forEach(function (dataset, series)
			{
				var meta = chart.getDatasetMeta(series);
				if (!meta.hidden) {
					meta.data.forEach(function (element, index)
					{
						// ステップ１　数値を文字列に変換
						var dataString = dataset.data[index].toString();

						// ステップ２　文字列の書体
						// 色　'rgb(0, 0, 0)', 'rgba(192, 80, 77, 0.7)'
						ctx.fillStyle = "rgba(80,80,80,0.7)";
						// サイズ
						var fontSize = 12;
						// 書体 "bold", "italic"
						var fontStyle = "normal";
						// フォントの種類 "sans-serif", "ＭＳ 明朝"
						var fontFamily = "Meiryo";
						ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);

						// ステップ３　文字列の位置の基準点
						// 文字列　start, end, left, right, center
						ctx.textAlign = "center";
						// 文字高　middle, top, bottom
						ctx.textBaseline = "middle";

						// ステップ４　文字列のグラフでの位置
						// 点と文字列の距離
						var padding = 3;
						var position = element.tooltipPosition();
						// 文字列の表示　fillText(文字列, Ｘ位置, Ｙ位置)
						ctx.fillText(dataString.replace(/\B(?=(\d{3})+(?!\d))/g, ","),
							position.x,
							position.y - (fontSize / 2) - padding);
					});
				}
			});
		}
	}

	var plugs = (isViewNumber === "True") ? dataLabelPlugin : "";
	var element = document.getElementById("Chart");
	var chart = new window.Chart(element,
	{
		// グラフの種類
		type: chartType,
		// 表示するデータ
		data: mydata,
		// オプション設定
		options: options,
		// 数値表示設定
		plugins: [plugs]
	});
}