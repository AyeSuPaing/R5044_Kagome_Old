<%--
=========================================================================================================
  Module      : Date Time Picker Period Input (DateTimePickerPeriodInput.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateTimePickerPeriodInput.ascx.cs" Inherits="Form_Common_DateTimePickerPeriodInput" %>
<style>
	.error-message-datepicker-container {
		text-align: left;
		color: red;
		padding: 5px;
		word-wrap: break-word;
		display: none; 
	}

	.error-message-datepicker-container {
		position: absolute;
	}

	.select-period {
		margin: 40px 0px 60px 0px;
		justify-content: center;
	}

	.select-period .select-period-start,
	.select-period .select-period-start {
		position: relative;
	}

	.input-timepicker {
		width: 4.5rem !important;
	}

	.modal.modal-size-m .modal-content-wrapper {
		width: 700px !important;
	}

	.form-element-group-content .select-display-period-value {
		font-size: 14px;
		margin: 3px;
	}
</style>
<body>
	<div class="form-element-group-content" style="display:inline-block">
		<a href="javascript:void(0);" class="btn-modal-open btn-modal-open-custom" data-id="<%= this.ID %>" data-modal-selector="#<%= this.ID %>" data-modal-classname="modal-size-m">
			<span class="select-display-period-value-<%= this.ID %>">設定なし</span>
		</a>
		<input runat="server" id="hfStartDate" type="hidden" value="" />
		<input runat="server" id="hfStartTime" type="hidden" value="" />
		<input runat="server" id="hfEndDate" type="hidden" value="" />
		<input runat="server" id="hfEndTime" type="hidden" value="" />
		<div class="modal-content-hide ">
			<div id="<%= this.ID %>">
				<div class="js-select-period-modal" data-id="<%= this.ID %>" data-canshowenddatetime="<%= this.CanShowEndDatePicker %>" data-cannullstartdatetime="<%= this.IsNullStartDateTime %>" data-cannullenddatetime="<%= this.IsNullEndDateTime %>">
					<p class="modal-inner-ttl">期間指定を設定する</p>
					<div class="select-period">
						<div class="select-period-start">
							<div class="select-period-start-title">開始</div>
							<div class="select-period-start-input">
								<div class="select-period-start-input-date select-period-start-input-date-<%= this.ID %>">
									<span class="select-period-start-input-date-label">日付</span>
									<input type="text" name="" class="input-datepicker" data-input-hidden-selector=".<%= this.StartDateDataInputHiddenSelector %>" maxlength="10" />
								</div>
								<div class="select-period-start-input-time select-period-start-input-time-<%= this.ID %>">
									<span class="select-period-start-input-time-label">時間</span>
									<input type="text" name="" class="input-timepicker" data-input-hidden-selector=".<%= this.StartTimeDataInputHiddenSelector %>" maxlength="8" />
								</div>
								<a href="javascript:void(0)" class="select-period-start-clear btn btn-txt btn-size-s">クリア</a>
							</div>
							<span class="error-message-datepicker-container">開始日は半角の正しい日付形式で入力して下さい。</span>
						</div>
						<div class="select-period-wave">～</div>
						<div class="select-period-end">
							<div class="select-period-end-title">終了</div>
							<div class="select-period-end-input">
								<div class="select-period-end-input-date select-period-end-input-date-<%= this.ID %>">
									<span class="select-period-end-input-date-label">日付</span>
									<input type="text" name="" class="input-datepicker" data-input-hidden-selector=".<%= this.EndDateDataInputHiddenSelector %>" maxlength="10" />
								</div>
								<div class="select-period-end-input-time select-period-end-input-time-<%= this.ID %>">
									<span class="select-period-end-input-time-label">時間</span>
									<input type="text" name="" class="input-timepicker" data-input-hidden-selector=".<%= this.EndTimeDataInputHiddenSelector %>" maxlength="8" />
								</div>
								<a href="javascript:void(0)" class="select-period-end-clear btn btn-txt btn-size-s">クリア</a>
							</div>
							<span class="error-message-datepicker-container">終了日は半角の正しい日付形式で入力して下さい。</span>
							<span class="error-message-datepicker-container">開始日は終了日より前に指定して下さい。</span>
						</div>
					</div>
					<div class="modal-footer-action">
						<input type="button" class="btn btn-main btn-size-l select-period-modal-submit-<%= this.ID %>" value="設定する" data-value-text-selector=".select-display-period-value-<%= this.ID %>" />
					</div>
				</div>
			</div>
		</div>
	</div>
</body>
<script>
	// Select date time period modal object
	var selectDateTimePeriodModal = {
		wrapperSelector: '.js-select-period-modal',
		selectPeriodWaveSelector: '.select-period-wave',
		selectPeriodEndSelector: '.select-period-end',
		submitBtnSelector: '.select-period-modal-submit-',
		errorMessageContainerSelector: '.error-message-datepicker-container',
		startDateInputSelector: '.select-period-start-input-date-controlId input',
		startTimeInputSelector: '.select-period-start-input-time-controlId input',
		endDateInputSelector: '.select-period-end-input-date-controlId input',
		endTimeInputSelector: '.select-period-end-input-time-controlId input',
		ini: function () {
			var _this = this;
			$(_this.wrapperSelector).each(function () {
				var wrapper = $(this);
				var controlId = wrapper.data("id");
				var isNullStartDateTime = (wrapper.data("cannullstartdatetime") === 'True');
				var isNullEndDateTime = (wrapper.data("cannullenddatetime") === 'True');
				var isCanShowEndDateTime = (wrapper.data("canshowenddatetime") === 'True');
				var submitBtn = wrapper.find(_this.submitBtnSelector + controlId);
				var startDateInput = wrapper.find(_this.startDateInputSelector.replace('controlId', controlId));
				var startTimeInput = wrapper.find(_this.startTimeInputSelector.replace('controlId', controlId));
				var endDateInput = wrapper.find(_this.endDateInputSelector.replace('controlId', controlId));
				var endTimeInput = wrapper.find(_this.endTimeInputSelector.replace('controlId', controlId));
				var errorMessageContainers = wrapper.find(_this.errorMessageContainerSelector);

				// Set show/hide select period wave and select period end control
				if (isCanShowEndDateTime) {
					$(wrapper.find(_this.selectPeriodWaveSelector)).show();
					$(wrapper.find(_this.selectPeriodEndSelector)).show();
				} else {
					$(wrapper.find(_this.selectPeriodWaveSelector)).hide();
					$(wrapper.find(_this.selectPeriodEndSelector)).hide();
				}

				// 既にデータがセットされている場合の処理
				var valStartDate = $(startDateInput.data('input-hidden-selector')).val();
				var valStartTime = $(startTimeInput.data('input-hidden-selector')).val();
				var valEndDate = $(endDateInput.data('input-hidden-selector')).val();
				var valEndTime = $(endTimeInput.data('input-hidden-selector')).val();
				startDateInput.val(valStartDate);
				startTimeInput.val(valStartTime);
				endDateInput.val(valEndDate);
				endTimeInput.val(valEndTime);

				// Set display when data is available
				if ((valStartDate + valStartTime + valEndDate + valEndTime) != '') {
					$(submitBtn.data('value-text-selector')).text(startDateInput.val() + ' ' + startTimeInput.val() + ' ～ ' + endDateInput.val() + ' ' + endTimeInput.val() + '');
				}

				submitBtn.on('click', function () {
					var format = 'YYYY/MM/DD HH:mm:ss';
					var startDateTimeInput = startDateInput.val() + ' ' + startTimeInput.val();
					var isStartDateTimeValid = moment(startDateTimeInput, format, true).isValid();
					var endDateTimeInput = endDateInput.val() + ' ' + endTimeInput.val();
					var isEndDateTimeValid = moment(endDateTimeInput, format, true).isValid();

					// Check valid input date period
					if ((((isStartDateTimeValid === false)
							&& (isEndDateTimeValid === false))
						|| ((startDateTimeInput.trim() === '')
							&& (endDateTimeInput.trim() === '')))
						&& (isNullStartDateTime + '' === 'False')
						&& (isNullEndDateTime + '' === 'False')) {
						$(errorMessageContainers[0]).show();
						$(errorMessageContainers[1]).show();
						return;
					}
					if ((startTimeInput.val().trim() != '')
						&& (endTimeInput.val().trim() != '')) {
						if ((startTimeInput.val().split(':')[0] > '23')
							&& (endTimeInput.val().split(':')[0] > '23')) {
							$(errorMessageContainers[0]).show();
							$(errorMessageContainers[1]).show();
							return;
						}
					}

					// Check start input date
					if (startDateTimeInput.trim() === '') {
						if (isNullStartDateTime === false) {
							$(errorMessageContainers[0]).show();
							return;
						}
					} else {
						if ((isStartDateTimeValid === false)
								|| (startDateInput.val().split('/')[0] < '1900')
								|| (startTimeInput.val().split(':')[0] === '23'
									&& (startTimeInput.val().split(':')[1] > '00'
										|| startTimeInput.val().split(':')[2] > '00'))
								|| (startTimeInput.val().split(':')[0] > '23')) {
							$(errorMessageContainers[0]).show();
							return;
						}
					}

					// Check end input date
					if (endDateTimeInput.trim() === '') {
						if ((isNullStartDateTime === false)
							&& (isNullEndDateTime === false)
							&& isCanShowEndDateTime) {
							$(errorMessageContainers[1]).show();
							return;
						}
					} else {
						if ((isEndDateTimeValid === false)
							|| (endDateInput.val().split('/')[0] < '1900')
							|| (endTimeInput.val().split(':')[0] > '23')) {
							$(errorMessageContainers[1]).show();
							return;
						}
					}

					// Check valid date period
					if (isCanShowEndDateTime
						&& (new Date(startDateTimeInput).valueOf() > new Date(endDateTimeInput).valueOf())) {
						$(errorMessageContainers[2]).show();
						return;
					}

					// Set display data input date
					$(startDateInput.data('input-hidden-selector')).val(startDateInput.val());
					$(startTimeInput.data('input-hidden-selector')).val(startTimeInput.val());
					$(endDateInput.data('input-hidden-selector')).val(endDateInput.val());
					$(endTimeInput.data('input-hidden-selector')).val(endTimeInput.val());
					$($(this).data('value-text-selector')).text(startDateInput.val() + ' ' + startTimeInput.val() + ' ～ ' + endDateInput.val() + ' ' + endTimeInput.val() + '');
					modal.close();
				});
			});
		},
		reload: function (ucControlId) {
			if (ucControlId === undefined) return;

			var _this = this;
			$(_this.wrapperSelector).each(function () {
				var wrapper = $(this);
				var controlId = wrapper.data("id");
				if (controlId === ucControlId) {
					var submitBtn = wrapper.find(_this.submitBtnSelector + controlId);
					var startDateInput = wrapper.find(_this.startDateInputSelector.replace('controlId', controlId));
					var startTimeInput = wrapper.find(_this.startTimeInputSelector.replace('controlId', controlId));
					var endDateInput = wrapper.find(_this.endDateInputSelector.replace('controlId', controlId));
					var endTimeInput = wrapper.find(_this.endTimeInputSelector.replace('controlId', controlId));
					var errorMessageContainers = wrapper.find(_this.errorMessageContainerSelector);

					// 既にデータがセットされている場合の処理
					var valStartDate = $(startDateInput.data('input-hidden-selector')).val();
					var valStartTime = $(startTimeInput.data('input-hidden-selector')).val();
					var valEndDate = $(endDateInput.data('input-hidden-selector')).val();
					var valEndTime = $(endTimeInput.data('input-hidden-selector')).val();
					startDateInput.val(valStartDate);
					startTimeInput.val(valStartTime);
					endDateInput.val(valEndDate);
					endTimeInput.val(valEndTime);

					// Set display when data is available
					if ((valStartDate + valStartTime + valEndDate + valEndTime) != '') {
						$(submitBtn.data('value-text-selector')).text(startDateInput.val() + ' ' + startTimeInput.val() + ' ～ ' + endDateInput.val() + ' ' + endTimeInput.val() + '');
					}
				};
			});
		}
	}
	$(function () {
		selectDateTimePeriodModal.ini();
		bindErrorMessageDatepickerContainerHideEvent();
	});

	// bind error message datepicker container hide event
	function bindErrorMessageDatepickerContainerHideEvent() {
		$('.btn-modal-open-custom').on('click', function () {
			var controlId = $(this).data("id");
			$('#' + controlId).find('.error-message-datepicker-container').hide();
			selectDateTimePeriodModal.reload(controlId);
		});
	}

	// Reload display date time period
	function reloadDisplayDateTimePeriod(ucControlId) {
		selectDateTimePeriodModal.reload(ucControlId);
	}

	// Reload init javascript function that missing when have post-back event (using for update panel)
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (evt, args) {
		modal.ini(true);
		selectPeriod.ini();
		input_datepicker.ini();
		selectDateTimePeriodModal.ini();
		bindErrorMessageDatepickerContainerHideEvent();
	});
</script>