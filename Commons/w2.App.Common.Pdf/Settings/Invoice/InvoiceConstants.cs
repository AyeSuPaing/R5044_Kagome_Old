/*
=========================================================================================================
  Module      : Invoice Constants(InvoiceConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Invoice Constants
	/// </summary>
	internal class InvoiceConstants
	{
		/// <summary>Tag: root path</summary>
		public const string CONST_ROOT_PATH_TAG = "@@ ROOT_PATH @@";
		/// <summary>Format pattern field tag</summary>
		public const string CONST_FORMAT_PATTERN_FIELD_TAG = "@@ [a-zA-Z0-9_]+ @@";
		/// <summary>Format pattern url</summary>
		public const string CONST_FORMAT_PATTERN_URL = @"(www.+|http.+)([\s]|$)";
		/// <summary>Format pattern number</summary>
		public const string CONST_FORMAT_PATTERN_NUMBER = @"\d+";
		/// <summary>Format pattern max first page rows</summary>
		public const string CONST_FORMAT_PATTERN_MAXFIRSTPAGEROWS = @"Public m_iMaxFirstPageRows = \d+";
		/// <summary>Format pattern max rows</summary>
		public const string CONST_FORMAT_PATTERN_MAXROWS = @"Public m_iMaxRows = \d+";
		/// <summary>Format pattern default max first page rows</summary>
		public const string CONST_FORMAT_PATTERN_DEFAULT_MAXFIRSTPAGEROWS = @"Public m_DefaultMaxFirstPageRows = \d+";
		/// <summary>Format pattern default max rows</summary>
		public const string CONST_FORMAT_PATTERN_DEFAULT_MAXROWS = @"Public m_DefaultMaxRows = \d+";
		/// <summary>Format field value</summary>
		public const string CONST_FORMAT_FIELD_VALUE = "=Fields!{0}.Value";
		/// <summary>Format equals to zero condition</summary>
		public const string CONST_FORMAT_EQUAL_TO_ZERO_CONDITION = "=({0} = 0)";
		/// <summary>Format OR for two condition</summary>
		public const string CONST_FORMAT_OR_FOR_TWO_CONDITION = "{0} Or {1}";
		/// <summary>Tab delimiter</summary>
		public const char CONST_TAB_DELIMITER = '\t';
		/// <summary>Format tag</summary>
		public const string CONST_FORMAT_TAG = "@@ {0} @@";
		/// <summary>Format new tag</summary>
		public static string CONST_FORMAT_NEW_TAG = CONST_TAB_DELIMITER + CONST_FORMAT_TAG + CONST_TAB_DELIMITER;
		/// <summary>Field: Set promotion discount</summary>
		public const string CONST_FIELD_SETPROMOTION_DISCOUNT = "setpromotion_discount";
		/// <summary>Prefix set promotion</summary>
		public const string CONST_PREFIX_FIELD_SETPROMOTION = "setpromotion_";
		/// <summary>Format field name text</summary>
		public const string CONST_FORMAT_FIELD_NAME_TEXT = "{0}_text";
		/// <summary>Format field name value</summary>
		public const string CONST_FORMAT_FIELD_NAME_VALUE = "{0}_value";
		/// <summary>Template field name text</summary>
		public const string CONST_TEMPLATE_FIELD_NAME_TEXT = "order_price_total_text";
		/// <summary>Template field name value</summary>
		public const string CONST_TEMPLATE_FIELD_NAME_VALUE = "order_price_total_value";
		/// <summary>カラーコードを取得する正規表現</summary>
		public const string CONST_REGEXP_GET_COLOR_CODE = "^#([0-9A-Fa-f]{6})$";
		/// <summary>値段取得正規表現</summary>
		public const string CONST_REGEXP_GET_PRICE_VALUE = "Fields!(?!order_shipping_no)(?!shipping_no_max)(.*?).Value";
		/// <summary>頒布会ごとの最大購入回数</summary>
		public const string CONST_FIELD_ORDER_SUBSCRIPTION_BOX_MAX_COUNT = "order_subscription_box_max_count";
		/// <summary>次回配送日表示切替条件式</summary>
		public const string CONDITIONAL_EXPRESSION_FOR_NEXT_SHIPPING_DATE = "=IIf((Fields!fixed_purchase_id.Value = \"\" or (Fields!subscription_box_course_id.Value <> \"\" and Fields!order_subscription_box_order_count.Value >= Fields!order_subscription_box_max_count.Value)), {0}, {1})";
		/// <summary>次回配送日の値</summary>
		public const string CONST_FIELD_VALUE_NEXT_SHIPPING_DATE = "@@ next_shipping_date @@";
		/// <summary>Operators for calculator</summary>
		public static string[] CONST_OPERATORS_FOR_CALCULATOR = new[]
		{
			"+",
			"-",
			"/",
			"*"
		};

		/// <summary>XML element name: text align</summary>
		public const string CONST_XML_ELEMENT_NAME_TEXTALIGN = "TextAlign";
		/// <summary>XML element name: padding left</summary>
		public const string CONST_XML_ELEMENT_NAME_PADDINGLEFT = "PaddingLeft";
		/// <summary>XML element name: padding right</summary>
		public const string CONST_XML_ELEMENT_NAME_PADDINGRIGHT = "PaddingRight";
		/// <summary>XML element name: padding top</summary>
		public const string CONST_XML_ELEMENT_NAME_PADDINGTOP = "PaddingTop";
		/// <summary>XML element name: padding bottom</summary>
		public const string CONST_XML_ELEMENT_NAME_PADDINGBOTTOM = "PaddingBottom";
		/// <summary>XML element name: Left</summary>
		public const string CONST_XML_ELEMENT_NAME_LEFT = "Left";
		/// <summary>XML element name: Width</summary>
		public const string CONST_XML_ELEMENT_NAME_WIDTH = "Width";
		/// <summary>XML element name: Top</summary>
		public const string CONST_XML_ELEMENT_NAME_TOP = "Top";
		/// <summary>XML element name: Height</summary>
		public const string CONST_XML_ELEMENT_NAME_HEIGHT = "Height";
		/// <summary>XML element name: ZIndex</summary>
		public const string CONST_XML_ELEMENT_NAME_ZINDEX = "ZIndex";
		/// <summary>XML element name: block</summary>
		public const string CONST_XML_ELEMENT_NAME_BLOCK = "InvoiceBlock";
		/// <summary>XML element name: embedded image</summary>
		public const string CONST_XML_ELEMENT_NAME_EMBEDDEDIMAGE = "EmbeddedImage";
		/// <summary>XML element name: image data</summary>
		public const string CONST_XML_ELEMENT_NAME_IMAGEDATA = "ImageData";
		/// <summary>XML element name: page header</summary>
		public const string CONST_XML_ELEMENT_NAME_PAGEHEADER = "PageHeader";
		/// <summary>XML element name: page footer</summary>
		public const string CONST_XML_ELEMENT_NAME_PAGEFOOTER = "PageFooter";
		/// <summary>XML element name: tablix row hierarchy</summary>
		public const string CONST_XML_ELEMENT_NAME_TABLIXROWHIERARCHY = "TablixRowHierarchy";
		/// <summary>XML element name: tablix members</summary>
		public const string CONST_XML_ELEMENT_NAME_TABLIXMEMBERS = "TablixMembers";
		/// <summary>XML element name: tablix member</summary>
		public const string CONST_XML_ELEMENT_NAME_TABLIXMEMBER = "TablixMember";
		/// <summary>XML element name: group</summary>
		public const string CONST_XML_ELEMENT_NAME_GROUP = "Group";
		/// <summary>XML element name: tablix row</summary>
		public const string CONST_XML_ELEMENT_NAME_TABLIXROW = "TablixRow";
		/// <summary>XML element name: tablix rows</summary>
		public const string CONST_XML_ELEMENT_NAME_TABLIXROWS = "TablixRows";
		/// <summary>XML element name: rectangle</summary>
		public const string CONST_XML_ELEMENT_NAME_RECTANGLE = "Rectangle";
		/// <summary>XML element name: line</summary>
		public const string CONST_XML_ELEMENT_NAME_LINE = "Line";
		/// <summary>XML element name: report items</summary>
		public const string CONST_XML_ELEMENT_NAME_REPORTITEMS = "ReportItems";
		/// <summary>XML element name: text box</summary>
		public const string CONST_XML_ELEMENT_NAME_TEXTBOX = "Textbox";
		/// <summary>XML element name: paragraphs</summary>
		public const string CONST_XML_ELEMENT_NAME_PARAGRAPHS = "Paragraphs";
		/// <summary>XML element name: paragraph</summary>
		public const string CONST_XML_ELEMENT_NAME_PARAGRAPH = "Paragraph";
		/// <summary>XML element name: text runs</summary>
		public const string CONST_XML_ELEMENT_NAME_TEXTRUNS = "TextRuns";
		/// <summary>XML element name: text run</summary>
		public const string CONST_XML_ELEMENT_NAME_TEXTRUN = "TextRun";
		/// <summary>XML element name: value</summary>
		public const string CONST_XML_ELEMENT_NAME_VALUE = "Value";
		/// <summary>XML element name: label</summary>
		public const string CONST_XML_ELEMENT_NAME_LABEL = "Label";
		/// <summary>XML element name: visibility</summary>
		public const string CONST_XML_ELEMENT_NAME_VISIBILITY = "Visibility";
		/// <summary>XML element name: hidden</summary>
		public const string CONST_XML_ELEMENT_NAME_HIDDEN = "Hidden";
		/// <summary>XML element name: style</summary>
		public const string CONST_XML_ELEMENT_NAME_STYLE = "Style";
		/// <summary>XML element name: font size</summary>
		public const string CONST_XML_ELEMENT_NAME_FONTSIZE = "FontSize";
		/// <summary>XML element name: font family</summary>
		public const string CONST_XML_ELEMENT_NAME_FONTFAMILY = "FontFamily";
		/// <summary>XML element name: font weight</summary>
		public const string CONST_XML_ELEMENT_NAME_FONTWEIGHT = "FontWeight";
		/// <summary>XML element name: format</summary>
		public const string CONST_XML_ELEMENT_NAME_FORMAT = "Format";
		/// <summary>XML element name: color</summary>
		public const string CONST_XML_ELEMENT_NAME_COLOR = "Color";
		/// <summary>XML element name: code</summary>
		public const string CONST_XML_ELEMENT_NAME_CODE = "Code";
		/// <summary>XML element name: text decoration</summary>
		public const string CONST_XML_ELEMENT_NAME_TEXTDECORATION = "TextDecoration";
		/// <summary>XML attribute name: name</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_NAME = "Name";
		/// <summary>XML element name: image</summary>
		public const string CONST_XML_ELEMENT_NAME_IMAGE = "Image";

		/// <summary>Font family default</summary>
		public const string CONST_DEFAULT_FONTFAMILY = "ＭＳ Ｐゴシック";
		/// <summary>Font size header default</summary>
		public const string CONST_DEFAULT_HEADER_FONTSIZE = "13pt";
		/// <summary>Font size footer default</summary>
		public const string CONST_DEFAULT_FOOTER_FONTSIZE = "7pt";
		/// <summary>Font size price default</summary>
		public const string CONST_DEFAULT_PRICE_FONTSIZE = "8pt";
		/// <summary>Font size body header and detail default</summary>
		public const string CONST_DEFAULT_BODYHEADER_AND_DETAIL_FONTSIZE = "9pt";
		/// <summary>次回配送日の下に配置する際の余白の値</summary>
		public const int CONST_UNDER_NEXT_SHIPPING_DATE_PADDING_POINT = 11;
		/// <summary>Font weight: normal</summary>
		public const string CONST_FONTWEIGHT_NORMAL = "Normal";
		/// <summary>Text decoration: underline</summary>
		public const string CONST_TEXTDECORATION_UNDERLINE = "Underline";
		/// <summary>Default right distance (use this to setting parts align is right)</summary>
		public const decimal CONST_DEFAULT_RIGHT_DISTANCE = 10.56097m;
		/// <summary>Default center distance (use this to setting parts align is center)</summary>
		public const decimal CONST_DEFAULT_CENTER_DISTANCE = 9.00251m;
		/// <summary>Default height per paragraph</summary>
		public const decimal CONST_DEFAULT_HEIGHT_PER_PARAGRAPH = 0.515m;
		/// <summary>「次回配送日」と同じポジションである場合のTop値</summary>
		public const decimal CONST_NEXT_SHIPPING_DATE_SAME_POSITION_TOP_VALUE = 0.38806m;
		/// <summary>Default max display items</summary>
		public const int CONST_DEFAULT_MAX_DISPLAY_ITEMS = 10;
		/// <summary>No padding value: 0 pt</summary>
		public const string CONST_NO_PADDING_VALUE = "0pt";
		/// <summary>Format style value (cm)</summary>
		public const string CONST_FORMAT_STYLE_CENTIMETR = "{0:0.#####}cm";
		/// <summary>Format style value (inch)</summary>
		public const string CONST_FORMAT_STYLE_INCH = "{0:0.#####}in";
		/// <summary>Logo image name: w2logo</summary>
		public const string CONST_LOGO_IMAGE_NAME = "w2logo";
		/// <summary>Body detail</summary>
		public const string CONST_BODY_DETAIL_NAME = "BodyDetail";
		/// <summary>Group name: order id</summary>
		public const string CONST_GROUP_NAME_ORDER_ID = "order_id_group";
		/// <summary>Blue color</summary>
		public const string CONST_BLUE_COLOR = "=Variables!blue_font_color.Value";

		/// <summary>Align kbn: left</summary>
		public const string CONST_ALIGN_KBN_LEFT = "Left";
		/// <summary>Align kbn: center</summary>
		public const string CONST_ALIGN_KBN_CENTER = "Center";
		/// <summary>Align kbn: right</summary>
		public const string CONST_ALIGN_KBN_RIGHT = "Right";

		/// <summary>Newline characters</summary>
		public static string[] CONST_NEWLINE_CHARACTERS = new string[]
		{
			"\n",
			"\r\n",
		};
		/// <summary>Block type setting</summary>
		public enum BlockType
		{
			/// <summary>Title</summary>
			Title,
			/// <summary>Body</summary>
			Body,
			/// <summary>Detail</summary>
			Detail,
			/// <summary>Product</summary>
			Product,
			/// <summary>Price</summary>
			Price,
			/// <summary>Footer</summary>
			Footer,
		}

		/// <summary>XML element name: text</summary>
		public const string CONST_XML_ELEMENT_NAME_TEXT = "Text";
		/// <summary>XML element name: price text</summary>
		public const string CONST_XML_ELEMENT_NAME_PRICETEXT = "PriceText";
		/// <summary>XML element name: logo image</summary>
		public const string CONST_XML_ELEMENT_NAME_LOGOIMAGE = "LogoImage";
		/// <summary>XML element name: max display items</summary>
		public const string CONST_XML_ELEMENT_NAME_MAXDISPLAYITEMS = "MaxDisplayItems";
		/// <summary>XML attribute name: type</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_TYPE = "type";
		/// <summary>XML attribute name: value</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_VALUE = "value";
		/// <summary>XML attribute name: path</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_PATH = "path";
		/// <summary>XML attribute name: language locale id</summary>
		public const string CONST_XML_ATTRIBUTE_LANGUAGELOCALEID = "languageLocaleId";
		/// <summary>XML attribute name: display name</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_DISPLAYNAME = "displayName";
		/// <summary>XML attribute name: id</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_ID = "ID";
		/// <summary>XML attribute name: display free amount flag</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_DISPLAYFREEAMOUNTFLG = "displayFreeAmountFlg";
		/// <summary>備考欄のRectangleの値</summary>
		public const string CONST_XML_ATTRIBUTE_RECTANGLE_BODY_NAME_RELATION_MEMO = "BodyRelationMemo";
		/// <summary>XML attribute name: parts align</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_PARTSALIGN = "parts-align";
		// <summary>次回配送日のポジション</summary>
		public static string CONST_NEXT_SHIPPING_DATE_PARTSALIGN = "";
		/// <summary>XML attribute name: text align</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_TEXTALIGN = "text-align";
		/// <summary>XML attribute name: text color</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_TEXTCOLOR = "text-color";
		/// <summary>XML attribute name: text width</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_WIDTH = "width";
		/// <summary>XML attribute name: margin top</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_MARGINTOP = "margin-top";
		/// <summary>XML attribute name: margin bottom</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_MARGINBOTTOM = "margin-bottom";
		/// <summary>XML attribute name: margin left</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_MARGINLEFT = "margin-left";
		/// <summary>XML attribute name: margin right</summary>
		public const string CONST_XML_ATTRIBUTE_NAME_MARGINRIGHT = "margin-right";
		/// <summary>XML attribute value: left</summary>
		public const string CONST_XML_ATTRIBUTE_VALUE_LEFT = "left";
		/// <summary>XML attribute value: center</summary>
		public const string CONST_XML_ATTRIBUTE_VALUE_CENTER = "center";
		/// <summary>XML attribute value: right</summary>
		public const string CONST_XML_ATTRIBUTE_VALUE_RIGHT = "right";
	}
}
