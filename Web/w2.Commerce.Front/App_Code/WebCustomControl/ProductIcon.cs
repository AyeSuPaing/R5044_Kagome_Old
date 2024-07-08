/*
=========================================================================================================
  Module      : 商品アイコンを表示する
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.Domain.Product;

namespace w2.Commerce.Front.WebCustomControl
{
	///*********************************************************************************************
	/// <summary>
	/// 商品アイコン表示コントロール
	/// </summary>
	///*********************************************************************************************
	[DefaultProperty("ProductMaster")]
	[ToolboxData("<{0}:ProductIcon runat=server></{0}:ProductIcon>")]
	public class ProductIcon : WebControl
	{
		static string[] FILED_PRODUCT_ICON_FLGS = { Constants.FIELD_PRODUCT_ICON_FLG1, Constants.FIELD_PRODUCT_ICON_FLG2, Constants.FIELD_PRODUCT_ICON_FLG3, Constants.FIELD_PRODUCT_ICON_FLG4, Constants.FIELD_PRODUCT_ICON_FLG5, Constants.FIELD_PRODUCT_ICON_FLG6, Constants.FIELD_PRODUCT_ICON_FLG7, Constants.FIELD_PRODUCT_ICON_FLG8, Constants.FIELD_PRODUCT_ICON_FLG9, Constants.FIELD_PRODUCT_ICON_FLG10 };
		static string[] FILED_PRODUCT_ICON_TERM_ENDS = { Constants.FIELD_PRODUCT_ICON_TERM_END1, Constants.FIELD_PRODUCT_ICON_TERM_END2, Constants.FIELD_PRODUCT_ICON_TERM_END3, Constants.FIELD_PRODUCT_ICON_TERM_END4, Constants.FIELD_PRODUCT_ICON_TERM_END5, Constants.FIELD_PRODUCT_ICON_TERM_END6, Constants.FIELD_PRODUCT_ICON_TERM_END7, Constants.FIELD_PRODUCT_ICON_TERM_END8, Constants.FIELD_PRODUCT_ICON_TERM_END9, Constants.FIELD_PRODUCT_ICON_TERM_END10 };

		/// <summary>商品(バリエーション)マスタ（外部必須設定）</summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public object ProductMaster
		{
			set
			{
				//------------------------------------------------------
				// 既に画像URLが設定されていたらスルー
				//------------------------------------------------------
				if (this.ImageUrl != null)
				{
					return;
				}

				//------------------------------------------------------
				// アイコンフラグ、表示期限プロパティセット
				//------------------------------------------------------
				string strIconFlg = null;
				DateTime? dtIconTermEnd = null;
				if (value is DataRowView)
				{
					strIconFlg = (string)((DataRowView)value)[FILED_PRODUCT_ICON_FLGS[this.IconNo - 1]];
					object objIconTermEnd = ((DataRowView)value)[FILED_PRODUCT_ICON_TERM_ENDS[this.IconNo - 1]];
					dtIconTermEnd = (objIconTermEnd is System.DBNull) ? null : (DateTime?)objIconTermEnd;
				}
				else if (value is CartProduct)
				{
					strIconFlg = ((CartProduct)value).IconFlg[this.IconNo - 1];
					dtIconTermEnd = ((CartProduct)value).IconTermEnd[this.IconNo - 1];
				}
				else if (value is ProductModel)
				{
					strIconFlg = ((ProductModel)value).IconFlg1;
					dtIconTermEnd = ((ProductModel)value).IconTermEnd1;
				}

				//------------------------------------------------------
				// アイコン情報から画像など取得＆データバインド
				//------------------------------------------------------
				if (strIconFlg == Constants.FLG_PRODUCT_ICON_ON)
				{
					if ((dtIconTermEnd != null)
						&& (dtIconTermEnd.Value.CompareTo(DateTime.Now) >= 0))
					{
						this.ImageUrl = SmartPhoneUtility.GetSmartPhoneContentsUrl(Constants.IMG_FRONT_PRODUCT_ICON[IconNo - 1]);
					}
				}
			}
		}

		/// <summary>画像URL</summary>
		private string ImageUrl
		{
			get { return (string)ViewState["ImageUrl"]; }
			set { ViewState["ImageUrl"] = value; }
		}
		/// <summary>アイコンNO（必須設定）</summary>
		public int IconNo { get; set; }
		/// <summary>alt文字列</summary>
		public string AltString { get; set; }

		/// <summary>
		/// レンダー処理のオーバーライド
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// WebControlクラスを継承したコントロールは、外部タグを作成するRenderメソッドがオーバーライドされる。
		/// よって、本コントロールでは画像タグのみを出力したいのでRenderメソッドをオーバーライドする必要がある。
		/// </remarks>
		protected override void Render(HtmlTextWriter writer)
		{
			RenderContents(writer);
		}
		/// <summary>
		/// コントロール出力
		/// </summary>
		/// <param name="output"></param>
		protected override void RenderContents(HtmlTextWriter output)
		{
			StringBuilder sbControl = new StringBuilder();
			if (this.ImageUrl != null)
			{
				sbControl.Append("<img ");
				sbControl.Append(" src=\"").Append(WebSanitizer.UrlAttrHtmlEncode(this.ImageUrl)).Append("\"");
				sbControl.Append(" border=\"0\"");
				sbControl.Append(" alt=\"").Append(WebSanitizer.HtmlEncode(this.AltString)).Append("\"");
				sbControl.Append(" class=\"").Append(WebSanitizer.HtmlEncode(this.CssClass)).Append("\"");
				sbControl.Append(" />");
			}

			output.Write(sbControl.ToString());
		}
	}
}
