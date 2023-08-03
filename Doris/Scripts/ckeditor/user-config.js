/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';

    config.toolbar = 'Basic';
    config.toolbar_Basic =
        [
        ['RemoveFormat', 'Format', 'Bold', 'Italic', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink', 'Image', 'TextColor', 'BGColor', 'Youtube', 'Iframe', 'VideoEmbed']
        ];
    config.removeDialogTabs = "image:advanced;link:advanced";
    config.removePlugins = "cssanim,imageresize,lightbox,slideshow,ckawesome,about";
    config.removeButtons = "Anchor,Subscript,Superscript,Strikethrough,Source,Flash,Scayt,Language";
    config.autoGrow_maxHeight = 400;
    config.allowedContent = true;

    config.filebrowserImageUploadUrl = "/Scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images";

    config.extraPlugins = "videoembed";
};
