/*
=========================================================================================================
  Module      : 商品画像を表示する
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.Domain;

namespace w2.Commerce.Front.WebCustomControl
{
	///*********************************************************************************************
	/// <summary>
	/// 商品画像表示コントロール
	/// </summary>
	///*********************************************************************************************
	[DefaultProperty("ProductMaster")]
	[ToolboxData("<{0}:ProductImage runat=server></{0}:ProductImage>")]
	public class ProductImage : WebControl
	{
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
				// value確定
				//------------------------------------------------------
				object objValue = value;
				if (value is DataView)
				{
					if (((DataView)objValue).Count != 0)
					{
						objValue = ((DataView)objValue)[0];
					}
					else
					{
						return;
					}
				}
				else if (value is DataRowView)
				{
					objValue = (DataRowView)objValue;
				}
				else if (value is IModel)
				{
					objValue = ((IModel)value).DataSource;
				}
				else if (value is IDictionary)
				{
					objValue = (IDictionary)value;
				}

				//------------------------------------------------------
				// ALT文字列決定
				//------------------------------------------------------
				if (this.AltString == null)
				{
					if (this.IsVariation)
					{
						this.AltString = ProductCommon.CreateProductJointName(
							(string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCT_NAME),
							(string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1),
							(string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2),
							(string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3));
					}
					else
					{
						this.AltString = (string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCT_NAME);
					}
				}
				//------------------------------------------------------
				// 画像タグID決定
				//------------------------------------------------------
				if (this.ImageTagId == null)
				{
					this.ImageTagId = "imgProductImage";
				}

				//------------------------------------------------------
				// 画像URL決定
				//------------------------------------------------------
				if (objValue != null)
				{
					this.ImageUrl = ProductPage.CreateProductImageUrl(
						objValue,
						this.ImageSize,
						this.IsVariation,
						this.IsGroupVariation);
				}
				else
				{
					this.ImageUrl = SmartPhoneUtility.GetSmartPhoneContentsUrl(Constants.PATH_PRODUCTIMAGES + Constants.PRODUCTIMAGE_NOIMAGE_HEADER + Constants.PRODUCTIMAGE_FOOTER_S);
				}
			}
		}

		/// <summary>画像URL</summary>
		private string ImageUrl
		{
			get { return (string)ViewState["ImageUrl"]; }
			set { ViewState["ImageUrl"] = value; }
		}

		/// <summary>商品画像サイズ（外部設定）</summary>
		public string ImageSize { get; set; }
		/// <summary>商品バリエーション用（外部設定）</summary>
		public bool IsVariation { get; set; }
		/// <summary>alt文字列（外部設定）</summary>
		public string AltString { get; set; }
		/// <summary>imgタグID（外部設定）</summary>
		public string ImageTagId { get; set; }
		/// <summary>width（外部設定）</summary>
		public string ImageWidth { get; set; }
		/// <summary>height（外部設定）</summary>
		public string ImageHeight { get; set; }
		/// <summary>Is group variation</summary>
		public bool IsGroupVariation { get; set; }

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
			sbControl.Append("<img ");
			sbControl.Append(" id=\"").Append(WebSanitizer.HtmlEncode(this.ImageTagId)).Append("\"");
			sbControl.Append(" src=\"").Append(WebSanitizer.HtmlEncode(this.ImageUrl)).Append("\"");
			sbControl.Append(" border=\"0\"");
			sbControl.Append(" alt=\"").Append(WebSanitizer.HtmlEncode(this.AltString)).Append("\"");
			sbControl.Append(" class=\"").Append(WebSanitizer.HtmlEncode(this.CssClass)).Append("\"");
			if (this.ImageWidth != null)
			{
				sbControl.Append(" width=\"").Append(WebSanitizer.HtmlEncode(this.ImageWidth)).Append("\"");
			}
			if (this.ImageHeight != null)
			{
				sbControl.Append(" height=\"").Append(WebSanitizer.HtmlEncode(this.ImageHeight)).Append("\"");
			}
			sbControl.Append(" />");

			output.Write(sbControl.ToString());
		}
	}
}
