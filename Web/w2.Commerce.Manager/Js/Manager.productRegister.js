//------------------------------------------------------
// 商品登録画面用スクリプト
//------------------------------------------------------

/*------------------------------------------------
* 商品画像選択
------------------------------------------------*/

var productRegisterImageSelect = {
	wrapperSelector: '.product-register-image-selecter',
	imageMainItemSelector: '.product-register-image-selecter-image-main',
	imageSubItemsWrapperSelector: '.product-register-image-selecter-image-sub-wrapper',
	imageSubItemsSelector: '.product-register-image-selecter-image-sub',
	firstDropInputSelector: '.product-register-image-selecter-first-input',
	firstDropareaSelector: '.product-register-image-selecter-first-droparea',
	imageMainSelector: '.product-register-image-selecter-image-main-thum-img',
	imageSubSelector: '.product-register-image-selecter-image-sub-thum-img',
	imageMainInputSelector: '.product-register-image-selecter-image-main-input',
	imageSubInputSelector: '.product-register-image-selecter-image-sub-input',
	imageMainDropareaSelector: '.product-register-image-selecter-image-main-droparea',
	imageSubDropareaSelector: '.product-register-image-selecter-image-sub-droparea',
	btnMainDeleteSelector: '.product-register-image-selecter-image-main-btn-delete',
	btnSubDeleteSelector: '.product-register-image-selecter-image-sub-btn-delete',
	classnameHasdata: 'is-hasdata',
	classnameDragover: 'is-dragover',
	classnameDropped: 'is-dropped',
	files: [],
	variationFiles: [],
	displayRender: function (wrapper, wrapperIndex) {
		var _this = this;
		var firstDroparea = wrapper.find(_this.firstDropareaSelector);
		var firstDropareaIn = wrapper.find(_this.firstDropInputSelector);
		var imageMainItem = wrapper.find(_this.imageMainItemSelector);
		var imageSubItems = wrapper.find(_this.imageSubItemsSelector);
		var imageMain = wrapper.find(_this.imageMainSelector);
		var imageSubArray = wrapper.find(_this.imageSubSelector).not('.ui-draggable-dragging .product-register-image-selecter-image-sub-thum-img');

		if (imageMain.attr('src')) {
			// メイン画像に値がセットされている
			imageMainItem.show();
			imageMainItem.addClass(_this.classnameHasdata);
			firstDroparea.hide();
			if (wrapperIndex == 0) {
				productImages[0].source = _this.files[0];
			} else {
				var variationIndex = wrapper.closest('.product-register-variation-form-row').data('index');
				variationImages[wrapperIndex - 1] = {
					source: _this.variationFiles[wrapperIndex - 1].source,
					delFlg: _this.variationFiles[wrapperIndex - 1].delFlg,
					variationIndex: variationIndex,
					fileName: _this.variationFiles[wrapperIndex - 1].fileName
				};
			}
		} else {
			// メイン画像に値がセットされていない
			imageMainItem.removeClass(_this.classnameHasdata);
			imageSubArray.each(function (i) {
				if ($(this).attr('src')) {
					imageMainItem.show();
					firstDroparea.hide();
					return false;
				}
				firstDroparea.show();
				firstDropareaIn.val('');
			});

			if (wrapperIndex == 0) {
				productImages[0].source = undefined;
			} else {
				var variationIndex = wrapper.closest('.product-register-variation-form-row').data('index');
				variationImages[wrapperIndex - 1] = {
					source: undefined,
					delFlg: _this.variationFiles[wrapperIndex - 1] ? _this.variationFiles[wrapperIndex - 1].delFlg : false,
					variationIndex: variationIndex,
					fileName: undefined
				};
			}
		}
		imageSubArray.each(function (i) {
			if ($(this).attr('src')) {
				$(imageSubItems[i]).show();
				$(imageSubItems[i]).addClass(_this.classnameHasdata);
				if (wrapperIndex == 0) productImages[i + 1].source = _this.files[i + 1];
			} else {
				$(imageSubItems[i]).removeClass(_this.classnameHasdata);
				if (wrapperIndex == 0) productImages[i + 1].source = undefined;
			}
		});
		// 次の画像を一つ開放
		if (imageMain.attr('src')) {
			$(imageSubItems[0]).show();
		}
		imageSubArray.each(function (i) {
			if ($(this).attr('src')) {
				$(imageSubItems[i + 1]).show();

				for (var index = 1; index < i; index++) {
					$(imageSubItems[index]).show();
				}
			}
		});
	},
	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function (i) {

			var wrapperIndex = i;
			var wrapper = $(this);

			// 2重処理回避
			if ($(this).data('ini')) {
				return true;
			};
			$(this).data('ini', true);

			var flgVariation = false;
			if (wrapper.data('flg-variation')) {
				flgVariation = true;
			}

			var firstDroparea = wrapper.find(_this.firstDropareaSelector);
			var firstDropInput = wrapper.find(_this.firstDropInputSelector);
			var imageMainItem = wrapper.find(_this.imageMainItemSelector);
			var imageSubItems = wrapper.find(_this.imageSubItemsSelector);
			var imageSubItemsWrapper = wrapper.find(_this.imageSubItemsWrapperSelector);
			var imageMain = wrapper.find(_this.imageMainSelector);
			var imageSubArray = wrapper.find(_this.imageSubSelector);
			var imageMainInput = wrapper.find(_this.imageMainInputSelector);
			var imageSubInput = wrapper.find(_this.imageSubInputSelector);
			var imageMainDroparea = wrapper.find(_this.imageMainDropareaSelector);
			var imageSubDroparea = wrapper.find(_this.imageSubDropareaSelector);
			var btnMainDelete = wrapper.find(_this.btnMainDeleteSelector);
			var btnSubDelete = wrapper.find(_this.btnSubDeleteSelector);

			// 初期HTML編集処理
			// ドラッグアイテム追加
			if (!flgVariation) {
				imageMainItem.append('<div class="product-register-image-selecter-image-main-drag"><span class="icon-menu"></span></div>');
				imageSubItems.each(function () {
					$(this).append('<div class="product-register-image-selecter-image-sub-drag"><span class="icon-menu"></span></div>');
				});
			}

			// ドロップエリア要素追加
			imageMainItem.append('<div class="product-register-image-selecter-image-main-inset-area">' + insert_here_disp_text + '</div>');
			imageSubItems.each(function () {
				$(this).append('<div class="product-register-image-selecter-image-sub-inset-area">' + insert_here_disp_text + '</div>');
			});

			// input と label の対応付け
			wrapper.find('#product-register-image-selecter-image-main').attr('id', 'product-register-image-selecter-image-main-' + wrapperIndex);
			wrapper.find('label[for="product-register-image-selecter-image-main"]').attr('for', 'product-register-image-selecter-image-main-' + wrapperIndex);

			// 初期表示
			imageMainItem.hide();
			imageSubItems.hide();
			_this.displayRender(wrapper, wrapperIndex);

			// ドラッグ＆ドロップで画像順番入れ替え
			var draggingItemIndex = 0;

			// メイン画像 draggable
			imageMainItem.draggable({
				containment: '.main-contents',
				cursorAt: { top: 50, left: 50 },
				start: function (event, ui) {
					wrapper.addClass('is-draggable-active');
					draggingItemIndex = 0;
				},
				stop: function (event, ui) {
					wrapper.removeClass('is-draggable-active');
				},
				helper: 'clone',
				// revert: true,
				handle: '.product-register-image-selecter-image-main-drag'
			});
			// サブ画像 draggable
			imageSubItems.each(function (i) {
				$(this).draggable({
					containment: '.main-contents',
					cursorAt: { top: 50, left: 50 },
					start: function (event, ui) {
						wrapper.addClass('is-draggable-active');
						draggingItemIndex = i + 1;
					},
					stop: function (event, ui) {
						wrapper.removeClass('is-draggable-active');
					},
					helper: 'clone',
					// revert: true,
					handle: '.product-register-image-selecter-image-sub-drag'
				});
			})
			wrapper.find('.product-register-image-selecter-image-main-inset-area').droppable({
				drop: function (event, ui) {
					sortImage(draggingItemIndex, 0);
				},
				over: function (event, ui) {
				}
			});
			wrapper.find('.product-register-image-selecter-image-sub-inset-area').each(function (i) {
				$(this).droppable({
					drop: function (event, ui) {
						sortImage(draggingItemIndex, i + 1);
					},
					over: function (event, ui) {
					}
				});
			});
			var sortImage = function (moveItemIndex, insertPositionIndex) {
				var imgArray = [];
				var sortedArray = [];
				var sortedArray_ = [];
				var sourceIndexs = [];
				var fileNames = [];
				var delFlags = [];
				imgArray.push($('.product-register-image-selecter-image-main-thum-img').attr('src'));
				$('.product-register-image-selecter-image-sub').each(function () {
					if (!$(this).hasClass('ui-draggable-dragging')) {
						$(this).find('.product-register-image-selecter-image-sub-thum-img').each(function () {
							imgArray.push($(this).attr('src'));
						});
					}
				});
				var insertPositionIndex_ = insertPositionIndex;

				if (moveItemIndex < insertPositionIndex) {
					// 前から後ろへ移動させる場合は、指定位置の後ろに挿入する
					insertPositionIndex_ = insertPositionIndex + 1;
				}

				var moveItemSrc = imgArray[moveItemIndex];
				imgArray.forEach(function (element, index) {
					if (index == insertPositionIndex_) {
						sortedArray.push(moveItemSrc);
						sortedArray_.push(productImages[moveItemIndex].source);
						sourceIndexs.push(productImages[moveItemIndex].sourceIndex);
						fileNames.push(productImages[moveItemIndex].fileName);
						delFlags.push(productImages[moveItemIndex].delFlg);
					}
					if (index != moveItemIndex) {
						sortedArray.push(element);
						sortedArray_.push(productImages[index].source);
						sourceIndexs.push(productImages[index].sourceIndex);
						fileNames.push(productImages[index].fileName);
						delFlags.push(productImages[index].delFlg);
					}
				});

				if (insertPositionIndex_ == imgArray.length) {
					sortedArray.push(moveItemSrc);
					sortedArray_.push(productImages[moveItemIndex].source);
					sourceIndexs.push(productImages[moveItemIndex].sourceIndex);
					fileNames.push(productImages[moveItemIndex].fileName);
					delFlags.push(productImages[moveItemIndex].delFlg);
				}

				sortedArray.forEach(function (element, index) {
					if (index == 0) {
						// main
						wrapper.find('.product-register-image-selecter-image-main-thum-img').attr('src', element);
					} else {
						// sub
						wrapper.find('.product-register-image-selecter-image-sub-thum-img:eq(' + (index - 1) + ')').attr('src', element);
					}
					productImages[index].sourceIndex = sourceIndexs[index];
					productImages[index].fileName = fileNames[index];
					productImages[index].delFlg = delFlags[index];
				});
				//productImages = sortedArray_;
				_this.files = sortedArray_;
				_this.displayRender(wrapper, wrapperIndex);
			}

			// btn delete
			btnMainDelete.off('click');
			btnMainDelete.on('click', function () {
				var res = confirm(delete_iamge_confirm_disp_text);
				if (res == true) {
					// 削除処理
					$(imageMain).attr('src', '');
					$(imageMainItem).removeClass(_this.classnameHasdata);
					if (wrapperIndex == 0) {
						productImages[0].source = undefined;
						productImages[0].delFlg = true;
					} else {
						variationImages[wrapperIndex - 1].source = undefined;
						variationImages[wrapperIndex - 1].delFlg = true;
					}
				}
			});
			btnSubDelete.each(function (i) {
				$(this).off('click');
				$(this).on('click', function () {
					var res = confirm(delete_iamge_confirm_disp_text);
					if (res == true) {
						// 削除処理
						$(imageSubArray[i]).attr('src', '');
						$(imageSubItems[i]).removeClass(_this.classnameHasdata);
						productImages[i + 1].source = undefined;
						productImages[i + 1].delFlg = true;
					}
				});
			});

			// dragover & dragleave
			if (browser == 'ie') {
				// IEはドラッグ＆ドロップに標準で対応していないため個別処理
				$('body').unbind('dragover drop').bind('dragover drop', function (e) {
					e.preventDefault();
				});

				firstDroparea.bind('drop', function (e) {
					e.stopPropagation();
					e.preventDefault();

					if (e.originalEvent.dataTransfer.files.length != 0) {
						var fileList = e.originalEvent.dataTransfer.files;
						previewImageForFirstDropInput(fileList);
					}
				});

				imageMainDroparea.bind('drop', function (e) {
					e.stopPropagation();
					e.preventDefault();

					if (e.originalEvent.dataTransfer.files.length != 0) {
						var file = e.originalEvent.dataTransfer.files[0];
						previewImageForMainInput(file, imageMainInput);
					}
				});

				imageSubDroparea.each(function (i) {
					$(this).bind('drop', function (e) {
						e.stopPropagation();
						e.preventDefault();
						if (e.originalEvent.dataTransfer.files.length != 0) {
							var file = e.originalEvent.dataTransfer.files[0];
							previewImageForSubInput(file, i, $('#product-register-image-selecter-image-sub-' + (i + 1)));
						}
					});
				});
			}

			firstDroparea.bind('dragover', function (e) {
				$(this).css({
					'height': $(this).outerHeight() + 'px',
					'width': $(this).outerWidth() + 'px'
				}).addClass(_this.classnameDragover);
			});
			firstDroparea.bind('dragleave', function (e) {
				$(this).css({
					'height': '',
					'width': ''
				}).removeClass(_this.classnameDragover);
			});
			imageMainDroparea.bind('dragover', function (e) {
				$(this).addClass(_this.classnameDragover);
			});
			imageMainDroparea.bind('dragleave', function (e) {
				$(this).removeClass(_this.classnameDragover);
			});
			imageSubDroparea.bind('dragover', function (e) {
				$(this).addClass(_this.classnameDragover);
			});
			imageSubDroparea.bind('dragleave', function (e) {
				$(this).removeClass(_this.classnameDragover);
			});

			// 初回ファイル選択
			firstDropInput.change(function () {
				// ファイルリストを取得
				var fileList = this.files;
				previewImageForFirstDropInput(fileList);
			});

			// Preview image for first drop input
			function previewImageForFirstDropInput(fileList) {
				// ファイルの数を取得
				var fileCount = (productImages.length < fileList.length)
					? productImages.length
					: fileList.length;
				// 選択されたファイルの数だけ処理する
				for (var i = 0; i < fileCount; i++) {
					// ファイルを取得
					var file = fileList[i];
					// 各ブロックに配置
					var fileReader = new FileReader();
					// サムネイルセットする際のカウント用
					fileReader.index = i;

					fileReader.onload = function (event) {
						var index = this.index;
						var loadedImageUri = event.target.result;
						if (index == 0) {
							// １つ目はメイン画像にセット
							imageMain.attr('src', loadedImageUri);
						} else {
							// ２つ目以降はサブ画像に順にセット
							$(imageSubArray[index - 1]).attr('src', loadedImageUri);
						}
						_this.displayRender(wrapper, wrapperIndex);
					}

					if (wrapperIndex == 0) {
						if (i == 0) _this.files = [];

						_this.files.push(file);
						productImages[i].sourceIndex = productImages[i].imageNo;
					} else {
						_this.variationFiles[wrapperIndex - 1] = {
							source: file,
							fileName: undefined
						};
					}

					fileReader.readAsDataURL(file);
				}
				firstDroparea.css({
					'height': '',
					'width': ''
				}).removeClass(_this.classnameDragover);
			}

			// 各画像ブロックのファイル選択処理

			// メイン画像ブロックのファイル選択処理
			imageMainInput.change(function () {
				// ファイルを取得
				var file = this.files[0];
				previewImageForMainInput(file, this);
			});

			// Preview image for main input
			function previewImageForMainInput(file, element) {
				if (file) {
					// サムネイルセット
					var fileReader = new FileReader();
					fileReader.onload = function (event) {
						var loadedImageUri = event.target.result;
						imageMain.attr('src', loadedImageUri);
						_this.displayRender(wrapper, wrapperIndex);
					}

					if (wrapperIndex == 0) {
						_this.files[0] = file;
					} else {
						_this.variationFiles[wrapperIndex - 1] = {
							source: file,
							fileName: undefined
						};
					}

					fileReader.readAsDataURL(file);
				}
				imageMainDroparea.removeClass(_this.classnameDragover);
				imageMainItem.addClass(_this.classnameDropped);
				setTimeout(function () {
					imageMainItem.removeClass(_this.classnameDropped);
				}, 1000);

				// Clear input
				$(element).val('');
			}

			// サブ画像ブロックのファイル選択処理
			imageSubInput.each(function (i) {
				$(this).change(function () {
					// ファイルを取得
					var file = this.files[0];
					previewImageForSubInput(file, i, this);
				});
			});

			// Preview image for sub input
			function previewImageForSubInput(file, i, element) {
				if (file) {
					// サムネイルセット
					var fileReader = new FileReader();
					fileReader.onload = function (event) {
						var loadedImageUri = event.target.result;
						$(imageSubArray[i]).attr('src', loadedImageUri);
						_this.displayRender(wrapper, wrapperIndex);
					}
					_this.files[i + 1] = file;
					fileReader.readAsDataURL(file);
				}
				imageSubDroparea.removeClass(_this.classnameDragover);
				imageSubItems.addClass(_this.classnameDropped);
				setTimeout(function () {
					imageSubItems.removeClass(_this.classnameDropped);
				}, 1000);

				// Clear input
				$(element).val('');
			}
		});
	}
}

/*------------------------------------------------
* 在庫数
------------------------------------------------*/
var productRegisterStocknum = {
	wrapperSelector: '.js-product-register-stock-management-stocknum',
	inputSelector: '.count-input-input',
	currentValueSelector: '.product-register-stock-management-current-value',
	afterChangeSelector: '.product-register-stock-management-after-change',
	afterChangeValueSelector: '.product-register-stock-management-after-change-value',
	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);
			var input = wrapper.find(_this.inputSelector);
			var currentValue = wrapper.find(_this.currentValueSelector);
			var afterChange = wrapper.find(_this.afterChangeSelector);
			var afterChangeValue = wrapper.find(_this.afterChangeValueSelector);

			// 初期値設定
			afterChangeValue.text(currentValue.text());
			if (afterChangeValue.text() == currentValue.text()) {
				afterChange.hide();
			}

			// イベントセット
			input.on('change', function () {
				afterChangeValue.text(Number(currentValue.text()) + Number(input.val()));
				if (input.val() != 0) {
					afterChange.show();
				} else {
					afterChange.hide();
				}
			});
		});
	}
}
$(function () {
	productRegisterStocknum.ini();
});

/*------------------------------------------------
* 在庫安全基準値
------------------------------------------------*/
var productRegisterStocAlert = {
	wrapperSelector: '.js-product-register-stock-management-stockalert',
	inputSelector: '.count-input-input',
	currentValueSelector: '.product-register-stock-management-current-value',
	afterChangeSelector: '.product-register-stock-management-after-change',
	afterChangeValueSelector: '.product-register-stock-management-after-change-value',
	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);
			var input = wrapper.find(_this.inputSelector);
			var currentValue = wrapper.find(_this.currentValueSelector);
			var afterChange = wrapper.find(_this.afterChangeSelector);
			var afterChangeValue = wrapper.find(_this.afterChangeValueSelector);

			// 初期値設定
			afterChangeValue.text(currentValue.text());
			if (afterChangeValue.text() == currentValue.text()) {
				afterChange.hide();
			}

			// イベントセット
			input.on('change', function () {
				afterChangeValue.text(Number(input.val()));
				if (input.val() != 0) {
					afterChange.show();
				} else {
					afterChange.hide();
				}
			});

		});
	}
}
$(function () {
	productRegisterStocAlert.ini();
});

/*------------------------------------------------
* バリエーション
------------------------------------------------*/
var productRegisterVariation = {
	wrapperSelector: '.product-register-variation',
	btnAddSelector: '.product-register-variation-add',
	btnDeleteSelector: '.product-register-variation-delete-btn',
	formSelector: '.product-register-variation-form',
	formRow: '.product-register-variation-form-row',
	index: 0,
	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);
			var btnAdd = wrapper.find(_this.btnAddSelector);
			var form = wrapper.find(_this.formSelector);

			_this.index = form.find(_this.formRow).length;
			btnAdd.on('click', function () {
				form.append(getVariationPattern(_this.index));

				// 削除ボタンイベント設置
				var btnDelete = wrapper.find(_this.btnDeleteSelector);

				btnDelete.each(function () {
					$(this).off('click').on('click', function () {
						var parent = $(this).closest('.product-register-variation-form-row');
						var index = $(".product-register-variation-form-row").index(parent);
						$(this).closest('.product-register-variation-form-row').remove();
						variationImages.splice(index, 1);
						blockSectionToggle.ini();
					});
				});

				initVariation(_this.index);

				blockSectionToggle.ini();

				// 画像登録UI設置
				productRegisterImageSelect.ini();

				propductRegisterVariationErrorContainer.ini(index);

				_this.index++;
			});

		});
	},

	clear: function () {
		var _this = this;
		$(_this.formSelector).empty();
	}
}

// Is object
function isObject(input) {
	return (typeof (input) === 'object');
}

// Product register error container
var propductRegisterErrorContainer = {
	wrapperSelector: '.product-validate-form-element-group-container',
	errorSelecter: '.product-error-message-container',
	tableErrorFixedPurchaseNextShippingSettingId: '#tblFixedPurchaseNextShippingProductErrorMessages',
	elementsSelector: 'a, input, textarea, select',

	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);

			// Clear error message when changing value
			var elements = wrapper.find(_this.elementsSelector);
			elements.each(function () {
				$(this).on('change', function () {
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).empty();
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).hide();
					$(this).closest(_this.wrapperSelector).find(_this.tableErrorFixedPurchaseNextShippingSettingId).css('display', 'none')
				});

				// For case element has "a" tag name
				if ($(this).prop("tagName").toLowerCase() === 'a') {
					$(this).on('click', function () {
						$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).empty();
						$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).hide();
						$(this).closest(_this.wrapperSelector).find(_this.tableErrorFixedPurchaseNextShippingSettingId).css('display', 'none')
					});
				}
			});
		});

		propductRegisterErrorContainer.clear();
	},

	bind: function (productErrorContents, isHasSetFocus) {
		if (isObject(productErrorContents)) {
			var _this = this;
			var targetIds = [];
			$(_this.errorSelecter).each(function () {
				var targetId = $(this).data('id');
				var errorContent = productErrorContents[targetId];

				// Check this error contents has error of the target id for check
				var targetIdForCheck = (targetId + '_for_check');
				var errorContentForCheck = productErrorContents[targetIdForCheck];
				if ((errorContent === undefined) && (errorContentForCheck !== undefined)) {
					errorContent = errorContentForCheck;
					targetIds.push(targetIdForCheck);
				}
				if (errorContent !== undefined) {
					targetIds.push(targetId);

					$(this).html(errorContent);
					$(this).show();

					if (isHasSetFocus === false) {
						var control = $(this).closest(_this.wrapperSelector).find(_this.elementsSelector);

						if (control.length) {
							if ((control[0].type !== 'checkbox') && (control[0].type !== 'radio')) {
								if (control[0].type === 'hidden') {
									control.not(':hidden')[0].focus();
								} else {
									control[0].focus();
								}
							} else {
								control[1].focus();
							}
							isHasSetFocus = true;
						}
					}
				}
			});

			getAndSetMissingErrorMessage(targetIds, productErrorContents);

			if (($(_this.tableErrorFixedPurchaseNextShippingSettingId).find(_this.errorSelecter).length === 1)
				&& ($(_this.tableErrorFixedPurchaseNextShippingSettingId).find(_this.errorSelecter).text() !== '')) {
				$(_this.tableErrorFixedPurchaseNextShippingSettingId).css('display', 'table');
			} else {
				$(_this.tableErrorFixedPurchaseNextShippingSettingId).css('display', 'none');
			}
		}

		return isHasSetFocus;
	},

	clear: function () {
		var _this = this;
		$(_this.errorSelecter).empty();
		$(_this.errorSelecter).hide();
	}
}
$(function () {
	propductRegisterErrorContainer.ini();
});

// Product register price error container
var propductRegisterPriceErrorContainer = {
	wrapperSelector: '.product-price-validate-form-element-group-container',
	errorSelecter: '.product-price-error-message-container',
	elementsSelector: 'input',

	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);

			// Clear error message when changing value
			var elements = wrapper.find(_this.elementsSelector);
			elements.each(function () {
				$(this).on('change', function () {
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).empty();
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).hide();
				});
			});
		});

		propductRegisterPriceErrorContainer.clear();
	},

	bind: function (productPriceErrorContents, isHasSetFocus) {
		if (isObject(productPriceErrorContents)) {
			var _this = this;
			var targetIds = [];
			$(_this.errorSelecter).each(function () {
				var targetId = $(this).data('id');
				var errorContent = productPriceErrorContents[targetId];
				if (errorContent !== undefined) {
					targetIds.push(targetId);
					$(this).html(errorContent);
					$(this).show();

					if (isHasSetFocus === false) {
						var control = $(this).closest(_this.wrapperSelector).find(_this.elementsSelector);
						if (control.length) {
							control[0].focus();
							isHasSetFocus = true;
						}
					}
				}
			});

			getAndSetMissingErrorMessage(targetIds, productPriceErrorContents);
		}

		return isHasSetFocus;
	},

	clear: function () {
		var _this = this;
		$(_this.errorSelecter).empty();
		$(_this.errorSelecter).hide();
	}
}
$(function () {
	propductRegisterPriceErrorContainer.ini();
});

// Product register tag error container
var propductRegisterTagErrorContainer = {
	wrapperSelector: '.product-tag-validate-form-element-group-container',
	errorSelecter: '.product-tag-error-message-container',
	elementsSelector: 'input',

	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);

			// Clear error message when changing value
			var elements = wrapper.find(_this.elementsSelector);
			elements.each(function () {
				$(this).on('change', function () {
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).empty();
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).hide();
				});
			});
		});

		propductRegisterTagErrorContainer.clear();
	},

	bind: function (productTagErrorContents, isHasSetFocus) {
		if (isObject(productTagErrorContents)) {
			var _this = this;
			var targetIds = [];
			$(_this.errorSelecter).each(function () {
				var targetId = $(this).data('id');
				var errorContent = productTagErrorContents[targetId];
				if (errorContent !== undefined) {
					targetIds.push(targetId);
					$(this).html(errorContent);
					$(this).show();

					if (isHasSetFocus === false) {
						var control = $(this).closest(_this.wrapperSelector).find(_this.elementsSelector);
						if (control.length) {
							control[0].focus();
							isHasSetFocus = true;
						}
					}
				}
			});

			getAndSetMissingErrorMessage(targetIds, productTagErrorContents);
		}

		return isHasSetFocus;
	},

	clear: function () {
		var _this = this;
		$(_this.errorSelecter).empty();
		$(_this.errorSelecter).hide();
	}
}
$(function () {
	propductRegisterTagErrorContainer.ini();
});

// Product register extend error container
var propductRegisterExtendErrorContainer = {
	wrapperSelector: '.product-extend-validate-form-element-group-container',
	errorSelecter: '.product-extend-error-message-container',
	elementsSelector: 'input, textarea',

	ini: function () {
		var _this = this;
		$(_this.wrapperSelector).each(function () {
			var wrapper = $(this);

			// Clear error message when changing value
			var elements = wrapper.find(_this.elementsSelector);
			elements.each(function () {
				$(this).on('change', function () {
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).empty();
					$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).hide();
				});
			});
		});

		propductRegisterExtendErrorContainer.clear();
	},

	bind: function (productExtendErrorContents, isHasSetFocus) {
		if (isObject(productExtendErrorContents)) {
			var _this = this;
			var targetIds = [];
			$(_this.errorSelecter).each(function () {
				var targetId = $(this).data('id');
				var errorContent = productExtendErrorContents[targetId];
				if (errorContent !== undefined) {
					targetIds.push(targetId);
					$(this).html(errorContent);
					$(this).show();

					if (isHasSetFocus === false) {
						var control = $(this).closest(_this.wrapperSelector).find(_this.elementsSelector);
						if (control.length) {
							control[0].focus();
							isHasSetFocus = true;
						}
					}
				}
			});

			getAndSetMissingErrorMessage(targetIds, productExtendErrorContents);
		}

		return isHasSetFocus;
	},

	clear: function () {
		var _this = this;
		$(_this.errorSelecter).empty();
		$(_this.errorSelecter).hide();
	}
}
$(function () {
	propductRegisterExtendErrorContainer.ini();
});

// Product register variation error container
var propductRegisterVariationErrorContainer = {
	wrapperSelector: '.product-variation-validate-form-element-group-container',
	errorSelecter: '.product-variation-error-message-container',
	formSelector: '.product-register-variation-form',
	formRow: '.product-register-variation-form-row',
	elementsSelector: 'input',

	ini: function (variationIndex) {
		var _this = this;
		var form = $(_this.formSelector);
		var formRows = form.find(_this.formRow);
		formRows.each(function (index, element) {
			if ((variationIndex === index) || (variationIndex === -1)) {
				var validateElements = $(element).find(_this.wrapperSelector);
				validateElements.each(function () {
					var wrapper = $(this);

					// Clear error message when changing value
					var elements = wrapper.find(_this.elementsSelector);
					elements.each(function () {
						$(this).on('change', function () {
							$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).empty();
							$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).hide();
						});
					});
				});
			}
		});

		propductRegisterVariationErrorContainer.clear(variationIndex);
	},

	bind: function (variationIndex, variationErrorContents, isHasSetFocus) {
		if (isObject(variationErrorContents)) {
			var _this = this;
			var targetIds = [];
			var form = $(_this.formSelector);
			var formRows = form.find(_this.formRow);
			formRows.each(function (index, element) {
				if (variationIndex === index) {
					var errorElements = $(element).find(_this.errorSelecter);
					errorElements.each(function () {
						var targetId = $(this).data('id');
						var errorContent = variationErrorContents[targetId];
						if (errorContent !== undefined) {
							targetIds.push(targetId);
							$(this).html(errorContent);
							$(this).show();

							if (isHasSetFocus === false) {
								var control = $(this).closest(_this.wrapperSelector).find(_this.elementsSelector);
								if (control.length) {
									control[0].focus();
									isHasSetFocus = true;
								}
							}
						}
					});
				}
			});

			getAndSetMissingErrorMessage(targetIds, variationErrorContents);
		}

		return isHasSetFocus;
	},

	clear: function (variationIndex) {
		var _this = this;
		var form = $(_this.formSelector);
		var formRows = form.find(_this.formRow);
		formRows.each(function (index, element) {
			if (variationIndex === index) {
				var errorElements = $(element).find(_this.errorSelecter);
				$(errorElements).empty();
				$(errorElements).hide();
			}
		});
	},

	clearAll: function () {
		var _this = this;
		$(_this.errorSelecter).empty();
		$(_this.errorSelecter).hide();
	}
}
$(function () {
	propductRegisterVariationErrorContainer.ini(-1);
});

// Product register variation price error container
var propductRegisterVariationPriceErrorContainer = {
	wrapperSelector: '.product-variation-price-validate-form-element-group-container',
	errorSelecter: '.product-variation-price-error-message-container',
	formSelector: '.product-register-variation-form',
	formRow: '.product-register-variation-form-row',
	elementsSelector: 'input',

	ini: function (variationIndex) {
		var _this = this;
		var form = $(_this.formSelector);
		var formRows = form.find(_this.formRow);
		formRows.each(function (index, element) {
			if ((variationIndex === index) || (variationIndex === -1)) {
				var validateElements = $(element).find(_this.wrapperSelector);
				validateElements.each(function () {
					var wrapper = $(this);

					// Clear error message when changing value
					var elements = wrapper.find(_this.elementsSelector);
					elements.each(function () {
						$(this).on('change', function () {
							$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).empty();
							$(this).closest(_this.wrapperSelector).find(_this.errorSelecter).hide();
						});
					});
				});
			}
		});

		propductRegisterVariationPriceErrorContainer.clear(variationIndex);
	},

	bind: function (variationIndex, variationPriceErrorContents, isHasSetFocus) {
		if (isObject(variationPriceErrorContents)) {
			var _this = this;
			var targetIds = [];
			var form = $(_this.formSelector);
			var formRows = form.find(_this.formRow);
			formRows.each(function (index, element) {
				if (variationIndex === index) {
					var errorElements = $(element).find(_this.errorSelecter);
					errorElements.each(function () {
						var targetId = $(this).data('id');
						var errorContent = variationPriceErrorContents[targetId];
						if (errorContent !== undefined) {
							targetIds.push(targetId);
							$(this).html(errorContent);
							$(this).show();

							if (isHasSetFocus === false) {
								var control = $(this).closest(_this.wrapperSelector).find(_this.elementsSelector);
								if (control.length) {
									control[0].focus();
									isHasSetFocus = true;
								}
							}
						}
					});
				}
			});

			getAndSetMissingErrorMessage(targetIds, variationPriceErrorContents);
		}

		return isHasSetFocus;
	},

	clear: function (variationIndex) {
		var _this = this;
		var form = $(_this.formSelector);
		var formRows = form.find(_this.formRow);
		formRows.each(function (index, element) {
			if (variationIndex === index) {
				var errorElements = $(element).find(_this.errorSelecter);
				$(errorElements).empty();
				$(errorElements).hide();
			}
		});
	},

	clearAll: function () {
		var _this = this;
		$(_this.errorSelecter).empty();
		$(_this.errorSelecter).hide();
	}
}
$(function () {
	propductRegisterVariationPriceErrorContainer.ini(-1);
});

// Get and set missing error message
function getAndSetMissingErrorMessage(targetIds, errorMessageContents) {
	for (var key in errorMessageContents) {
		if ((errorMessageContents.hasOwnProperty(key) === false)
			|| isJson(errorMessageContents[key])) continue;

		if (targetIds.indexOf(key) !== -1) continue;

		missingErrorMessage += (errorMessageContents[key] + '<br />');
	}
}

// Is Json
function isJson(str) {
	try {
		JSON.parse(str);
	} catch (e) {
		return false;
	}
	return true;
}