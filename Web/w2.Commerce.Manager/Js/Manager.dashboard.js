/*------------------------------------------------
* ダッシュボード
------------------------------------------------*/
var dashboard = {
	goal_progress: {
		ini: function () {
			// 売上進捗状況
			var html = '';
			$(data_goal_progress).each(function (i) {

				var style = '';
				var class_name = '';
				if (this.current.par < 30) {
					style = "left: " + this.current.par + "%";
					class_name = 'is-position-right';
				} else if ((this.current.par >= 30) && (this.current.par <= 100)) {
					style = "right: " + (100 - this.current.par) + "%";
				} else {
					style = "right: " + 0 + "%";
				}

				// 見通し
				var outlook_classname = 'is-green';
				if (this.current.outlook > 100 && this.current.outlook <= 100) {
					// 100%以上で達成見通し
					outlook_classname = 'is-green';
				} else if (this.current.outlook >= 80 && this.current.outlook <= 99) {
					// 80～99%で達成見通し
					outlook_classname = 'is-yellow';
				} else if (this.current.outlook < 80) {
					// 80%未満で達成見通し
					outlook_classname = 'is-red';
				}

				html +=
				  '<div class="dashboard-goal-progress dashboard-goal-progress-month">' +
				  '  <div class="dashboard-goal-progress-bar">' +
				  '    <div class="dashboard-goal-progress-bar-bar ' + outlook_classname + '" data-style="width:' + ((this.current.par <= 100) ? this.current.par : 100) + '%"></div>' +
				  '    <div class="dashboard-goal-progress-current-status ' + class_name + '" data-style="' + style + '">' +
				  '      <div class="dashboard-goal-progress-current-status-par">' +
				  '        <span class="dashboard-goal-progress-current-status-par-label">' + this.current.par + '</span>' +
				  '        <span class="dashboard-goal-progress-current-status-par-unit">%</span>' +
				  '      </div>' +
				  '      <div class="dashboard-goal-progress-current-status-value">' +
				  '        <span class="dashboard-goal-progress-current-status-value-label">' + this.current.value + '</span>' +
				  '        <span class="dashboard-goal-progress-current-status-value-unit">' + this.current.unit + '</span>' +
				  '      </div>' +
				  '    </div>' +
				  '  </div>' +
				  '  <div class="dashboard-goal-progress-goal">' +
				  '    <p class="dashboard-goal-progress-goal-title">' + this.title + '</p>' +
				  '    <p class="dashboard-goal-progress-goal-value">' +
				  '      <span class="dashboard-goal-progress-goal-value-label">' + this.goal.value + '</span>' +
				  '      <span class="dashboard-goal-progress-goal-value-unit">' + this.goal.unit + '</span>' +
				  '    </p>' +
				  '  </div>' +
				  '</div>';

			});
			$('.dashboard-goal-progress-wrapper').append(html);
			setTimeout(function () {
				$('.dashboard-goal-progress-current-status, .dashboard-goal-progress-bar-bar').each(function (i) {
					var style_ = $(this).data('style');
					$(this).attr('style', style_);
				});
			}, 100);
		}
	},
	latest_report_animation: {
		ini: function () {
			$('.dashboard-latest-report-box-value-text').each(function (i) {
				var num = $(this).text().trim();
				$(this).data('num', num);
				$(this).text('...')
			});
		},
		animation_start: function () {
			// 最新レポート 数字アニメーション
			$('.dashboard-latest-report-box-value-text').each(function (i) {
				var text = $(this);
				setTimeout(function () {
					var num = text.data('num');
					var tgt = text[0];
					var start_num = Number(num.replace(/,/g, '')) * .5;
					var el = tgt;

					od = new Odometer({
						el: el,
						value: start_num,
						format: '(,ddd)',
						// durationはCSSで制御
					});
					el.innerHTML = num
				}, i * 300);
			});
		}
	},
	transition_report_chart: {
		color_set: function (color) {
			switch (color) {
				case 'color-user':
					return '#24b540';
					break;
				case 'color-price':
					return '#e4981f';
					break;
				case 'color-order':
					return '#4566cc';
					break;
				case 'color-apply':
					return '#4566cc';
					break;
				case 'color-cancel':
					return '#ff4242';
					break;
				case 'color-signup':
					return '#24b540';
					break;
				case 'color-unsubscribe':
					return '#ff4242';
					break;
				default:
					return color;
			}
		},
		render_all: function () {
		  $(chartDataArray).each(function (i) {
        var canvas = $('#graph' + i + ' .dashboard-transition-report-canvas')[0].getContext('2d');
				var data = this;
				dashboard.transition_report_chart.render(canvas, data, 'default');
			});
		},
		render: function (canvas, tgt_data, mode) {
			var datasets = [];
			var yAxes = [];
			var tgt_dataset;
			var tgt_labels;

			if (mode == 'default') {
				tgt_dataset = tgt_data.data.datasets;
				tgt_labels = tgt_data.data.labels;
			} else if (mode == 'modal') {
				tgt_dataset = tgt_data.data_modal.datasets;
				tgt_labels = tgt_data.data_modal.labels;
			}

			$(tgt_dataset).each(function (n) {
				if (this.type == 'line') {
					datasets.push({
						label: this.label,
						type: this.type,
						fill: false,
						data: this.data,
						backgroundColor: dashboard.transition_report_chart.color_set(this.color),
						borderColor: dashboard.transition_report_chart.color_set(this.color),
						yAxisID: "y-axis-" + this.yAxisID,
						lineTension: 0
					});
				} else {
					datasets.push({
						label: this.label,
						data: this.data,
						backgroundColor: dashboard.transition_report_chart.color_set(this.color),
						borderColor: dashboard.transition_report_chart.color_set(this.color),
						yAxisID: "y-axis-" + this.yAxisID,
					});
				}

				if (n == 0) {
					yAxes.push({
						id: "y-axis-" + this.yAxisID,
						type: "linear",
						// position: "left",
						ticks: {
							beginAtZero: true,
							callback: function (label, index, labels) {
								// 数字のカンマ区切り
								return label.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
							}
						}
					});
				} else {
					yAxes.push({
						id: "y-axis-" + this.yAxisID,
						type: "linear",
						// position: "right",
						ticks: {
							beginAtZero: true,
							callback: function (label, index, labels) {
								// 数字のカンマ区切り
								return label.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
							}
						},
						gridLines: {
							drawOnChartArea: false,
						}
					});
				}

			});

			var myChart = new Chart(canvas, {
				type: 'bar',
				data: {
					labels: tgt_labels,
					datasets: datasets
				},
				options: {
					responsive: true,
					legend: {
						align: 'end',
						labels: {
							boxWidth: 14,
						}
					},
					scales: {
						yAxes: yAxes
					},
					tooltips: {
						callbacks: {
							title: function (tooltipItems, data) {
								return '';
							},
							label: function (tooltipItem, data) {
								var floatDataTmp = Math.round(parseFloat(data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index]));
								return data.datasets[tooltipItem.datasetIndex].label + ": " + floatDataTmp.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
							}
						}
					}
				}
			});
		},
		ini: function () {
			var html = '';
			$(chartDataArray).each(function (i) {
				html += '<div class="dashboard-box dashboard-transition-report-box dashboard-box-default" id="graph' + i + '">' +
								'  <div class="dashboard-transition-report-box-header">' +
								'    <h3 class="dashboard-transition-report-box-title">' + this.options.title + '</h3>' +
								'  </div>' +
								'  <div class="dashboard-transition-report-box-body">' +
								'    <div class="dashboard-transition-report-graph">' +
								'      <canvas class="dashboard-transition-report-canvas"></canvas>' +
								'    </div>' +
								'  </div>' +
								'</div>';
			});
			$('.dashboard-transition-report').html(html);
			// グラフクリック時のモーダル表示
			$('.dashboard-transition-report-box').each(function (i) {
				var html = '<div class="modal-content-hide">' +
										'  <div id="modal-transition-report-' + i + '">' +
										'    <div class="dashboard-box dashboard-transition-report-box">' +
										'      <div class="dashboard-transition-report-box-header">' +
										'        <h3 class="dashboard-transition-report-box-title">' + chartDataArray[i].options.title + '</h3>' +
										'      </div>' +
										'      <div class="dashboard-transition-report-box-body">' +
										'        <div class="dashboard-transition-report-graph">' +
										'          <canvas class="dashboard-transition-report-canvas"></canvas>' +
										'        </div>' +
										'      </div>' +
										'    </div>' +
										'  </div>' +
										'</div>';
				$('.dashboard-transition-report').append(html);
				$(this).click(function () {
					var canvas = $('#modal-transition-report-' + i + ' .dashboard-transition-report-canvas')[0].getContext('2d');
					var data = chartDataArray[i];

					dashboard.transition_report_chart.render(canvas, data, 'modal');

					modal.open('#modal-transition-report-' + i, 'modal-size-m modal-transition-report');
				});
			});
		}
	}
}

$(function () {
	dashboard.goal_progress.ini();
	dashboard.latest_report_animation.ini();
	dashboard.transition_report_chart.ini();
});
$(window).bind('load', function () {
	setTimeout(function () {
		dashboard.latest_report_animation.animation_start();
	}, 500);
	setTimeout(function () {
		dashboard.transition_report_chart.render_all();
	}, 1000);
});


// ランキング 注文数/注文金額 切り替え
$(function () {
	$('.dashboard-ranking-header-switch').each(function () {
		var selectors = [];
		$(this).find('[data-ranking-switch-selector]').each(function () {
			selectors.push($(this).data('ranking-switch-selector'));
			$(this).click(function (i) {
				var target_selector = $(this).data('ranking-switch-selector');
				$(selectors).each(function (n) {
					if (selectors[n] == target_selector) {
						$(selectors[n]).show();
					} else {
						$(selectors[n]).hide();
					}
				});
			});
		});
		$(this).find('');
	});
});
