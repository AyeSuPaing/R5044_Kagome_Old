/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function (config) {
    // タグ、属性で定義がないものもすべて許容する（許容しないと変に編集されちゃったりします）
    config.allowedContent = true;
};