var YAXES_FONT_SIZE = 10;
var YAXES_SCALE_VALUE = 5;
var YAXES_DEFAULT_MAX_VALUE = 1;
var YAXES_DEFAULT_MIN_VALUE = 0;

var XAXES_FONT_SIZE = 15;

var POINT_RADIUS = 6;
var LINE_TENSION = 0;

function CreateFixedPurchaseForecasCharts(jsonValues, chartType, isViewNumber, xlabel, ylabel1, ylabel2) {
  var priceValues = [];
  var stockValues = [];
  var labelValues = [];
  for (var i = 0; i < jsonValues.length; i++) {
    priceValues[i] = (jsonValues[i]["price"] < 0) ? 0 : jsonValues[i]["price"];
    stockValues[i] = (jsonValues[i]["stock"] < 0) ? 0 : jsonValues[i]["stock"];
    labelValues[i] = jsonValues[i]["yearMonth"];
  }

  // 最大値設定
  var maxDataValue = Math.max.apply(null, priceValues);
  var yAxes1MaxValue = ((maxDataValue == 0) == false) ? Math.ceil(maxDataValue * 1.2) : YAXES_DEFAULT_MAX_VALUE;

  maxDataValue = Math.max.apply(null, stockValues);
  var yAxes2MaxValue = ((maxDataValue == 0) == false) ? Math.ceil(maxDataValue * 1.2) : YAXES_DEFAULT_MAX_VALUE;

  var mydata = {
    labels: labelValues,
    datasets: [
      {
        label: ylabel1,
        hoverBackgroundColor: "rgb(58, 107, 166)",
        backgroundColor: "rgb(153, 153, 255)",
        data: priceValues,
        fill: false,
        pointRadius: POINT_RADIUS,
        lineTension: LINE_TENSION,
        yAxisID: "y-axis-1"
      },
      {
        label: ylabel2,
        hoverBackgroundColor: "rgb(255, 51, 51)",
        backgroundColor: "rgb(255, 153, 153)",
        data: stockValues,
        fill: false,
        pointRadius: POINT_RADIUS,
        lineTension: LINE_TENSION,
        yAxisID: "y-axis-2"
      }
    ]
  };

  var options = {
    title: {
      display: true,
      position: "top",
      maintainAspectRatio: false
    },
    tooltips: {
      callbacks: {
        label: function (tooltipItem) {
          var unit = (tooltipItem.datasetIndex == 0) ? "円" : "個";
          var label = String.format("{0} {1}",
            tooltipItem.yLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","),
            unit);
          return label;
        }
      }
    },
    scales: {
      yAxes: [
        {
          id: "y-axis-1",
          display: true,
          scaleLabel: {
            display: false,
            fontSize: YAXES_FONT_SIZE,
            fontFamily: "arial"
          },
          ticks: {
            suggestedMax: yAxes1MaxValue,
            min: YAXES_DEFAULT_MIN_VALUE,

            userCallback:
              function (label) {
                var result = (Math.floor(label) === label)
                  ? label.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
                  : "";
                return result;
              }
          }
        },
        {
          id: "y-axis-2",
          display: true,
          position: "right",
          scaleLabel: {
            display: false,
            fontSize: YAXES_FONT_SIZE,
            fontFamily: "arial"
          },
          ticks: {
            suggestedMax: yAxes2MaxValue,
            min: YAXES_DEFAULT_MIN_VALUE,

            userCallback:
              function (label) {
                var result = (Math.floor(label) === label)
                  ? label.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
                  : "";
                return result;
              }
          }
        }
      ],
      xAxes: [
        {
          display: true,
          scaleLabel: {
            display: true,
            labelString: xlabel,
            fontSize: XAXES_FONT_SIZE,
            fontFamily: "arial"
          }
        }
      ]
    }
  };

  var dataLabelPlugin = {
    afterDatasetsDraw: function (chart) {
      var ctx = chart.ctx;
      chart.data.datasets.forEach(function (dataset, series) {
        var meta = chart.getDatasetMeta(series);
        if (!meta.hidden) {
          meta.data.forEach(function (element, index) {
            var dataString = dataset.data[index].toString();

            ctx.fillStyle = "rgba(80,80,80,0.7)";
            var fontSize = 12;
            var fontStyle = "normal";
            var fontFamily = "Meiryo";
            ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);

            ctx.textAlign = "center";
            ctx.textBaseline = "middle";

            var padding = 3;
            var position = element.tooltipPosition();
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
      type: chartType,
      data: mydata,
      options: options,
      plugins: [plugs]
    });
}