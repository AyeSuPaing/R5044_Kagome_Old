var YAXES_FONT_SIZE = 10;
var YAXES_DEFAULT_MAX_VALUE = 100;
var YAXES_DEFAULT_MIN_VALUE = 0;
var XAXES_FONT_SIZE = 15;
var POINT_RADIUS = 6;
var LINE_TENSION = 0;

/**
 * Create Scoring Sale Charts
 * @param {String} elementId
 * @param {Array} jsonValues
 * @param {String} chartType
 * @param {boolean} isViewNumber
 * @param {String} xlabel
 * @param {String} ylabel1
 * @param {String} ylabel2
 */
function ScoringSaleCharts(elementId, jsonValues, chartType, isViewNumber, xlabel, ylabel1, ylabel2) {
  var pageExitRateValues = [];
  var labelValues = [];
  for (var i = 0; i < jsonValues.length; i++) {
    pageExitRateValues[i] = (jsonValues[i]["PageExitRate"] < 0) ? 0 : jsonValues[i]["PageExitRate"];
    labelValues[i] = jsonValues[i]["PageName"];
  }

  var mydata = {
    labels: labelValues,
    datasets: [
      {
        label: ylabel1,
        hoverBackgroundColor: "rgb(58, 107, 166)",
        backgroundColor: "rgb(153, 153, 255)",
        data: pageExitRateValues,
        fill: false,
        pointRadius: POINT_RADIUS,
        lineTension: LINE_TENSION,
      }
    ]
  };

  var options = {
    title: {
      display: true,
      position: "top",
    },
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      yAxes: [
        {
          display: true,
          scaleLabel: {
            display: false,
            fontSize: YAXES_FONT_SIZE,
            fontFamily: "arial"
          },
          ticks: {
            suggestedMax: YAXES_DEFAULT_MAX_VALUE,
            min: YAXES_DEFAULT_MIN_VALUE,

            userCallback:
              function (label) {
                var result = (Math.floor(label) === label)
                  ? label.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
                  : "";
                return result + "%";
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
    },
    legend: {
      position: 'right',
      align: 'center'
    },
    layout: {
      padding: {
        left: 5,
        right: 5,
        top: 10,
        bottom: -30
      }
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
  var element = document.getElementById(elementId);
  if (element) {
    // Render chart
    new window.Chart(element,
    {
      type: chartType,
      data: mydata,
      options: options,
      plugins: [plugs]
    });
  }
}
