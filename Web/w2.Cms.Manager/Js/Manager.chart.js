/*
 * 詳細はChart.jsのドキュメントを参照
 * 英語ドキュメント：(https://www.chartjs.org/docs/latest/)
 * 日本語ドキュメント：(https://tr.you84815.space/chartjs/general/performance.html)
 */

/*
 * チャートの作成
 * context：チャートの表示先コンテキスト
 * graphType：タイプ（line, bar,razar...）
 * chartDataSets：表示させるデータ
 * chartTitle：チャートタイトル
 */
function CreateChart(context, graphType, chartDataSets, chartTitle) {
  var chart = new Chart(
    context,
    {
      type: graphType,
      data: chartDataSets,
      options: {
        maintainAspectRatio: true,
        title: {
          display: true,
          fontSize: 15,
          text: chartTitle
        },
        legend: {
          display: true
        },
        aspectRaito: 1,
        scales: {
          xAxes: [{
            ticks: {
              autoSkip: false,
              fontSize: 10
            }
          }],
          yAxes: [{
            ticks: {
              beginAtZero: true,
              min: 0,
              fontSize: 10
            }
          }]
        }
      }
    });

  return chart;
}