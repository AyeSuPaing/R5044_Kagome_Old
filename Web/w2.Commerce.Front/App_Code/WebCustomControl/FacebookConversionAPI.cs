/*
=========================================================================================================
  Module      : Facebook Conversion API(FacebookConversionAPI.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.FacebookConversion;
using w2.App.Common.Order;
using w2.App.Common.Util;

namespace w2.Commerce.Front.WebCustomControl
{
	/// <summary>
	/// Facebook Conversion API
	/// </summary>
	[ToolboxData("<{0}:FacebookConversionAPI runat=server></{0}:FacebookConversionAPI>")]
	public class FacebookConversionAPI : WebControl
	{
		/// <summary>
		/// レンダー処理のオーバーライド
		/// </summary>
		/// <param name="writer">Writer</param>
		/// <remarks>
		/// WebControlクラスを継承したコントロールは、外部タグを作成するRenderメソッドがオーバーライドされる。
		/// よって、本コントロールでは画像タグのみを出力したいのでRenderメソッドをオーバーライドする必要がある。
		/// </remarks>
		protected override void Render(HtmlTextWriter writer)
		{
			if (Constants.MARKETING_FACEBOOK_CAPI_OPTION_ENABLED)
			{
				RenderContents(writer);
			}
		}

		/// <summary>
		/// コントロール出力
		/// </summary>
		/// <param name="output">Output</param>
		protected void RenderContents(HtmlTextWriter output)
		{
			var facebookConversionUtility = new FacebookConversionUtility();
			this.EventSourceUrl = StringUtility.ToEmpty(HttpContext.Current.Request.Url);
			FacebookConversionDataRequest.CustomData customData = null;

			switch (this.EventName)
			{
				case FacebookConversionConstants.FACEBOOK_EVENT_NAME_VIEW_CONTENT:
					customData = facebookConversionUtility.CreateCustomDataViewContents(
						this.CustomDataContentName,
						this.CustomDataValue,
						this.CustomDataCurrency,
						this.CustomDataContentType,
						this.CustomDataContentIds,
						this.CustomDataContentCategory);
					break;

				case FacebookConversionConstants.FACEBOOK_EVENT_NAME_COMPLETE_REGISTRATION:
					customData = facebookConversionUtility.CreateCustomDataCompleteRegistration(
						this.CustomDataContentName,
						this.CustomDataValue,
						this.CustomDataCurrency,
						this.CustomDataStatus);
					break;

				case FacebookConversionConstants.FACEBOOK_EVENT_NAME_ADD_TO_CART:
					var absoluteUri = (HttpContext.Current.Request.UrlReferrer != null)
						? StringUtility.ToEmpty(HttpContext.Current.Request.UrlReferrer.AbsoluteUri)
						: string.Empty;

					this.EventSourceUrl = absoluteUri;
					customData = facebookConversionUtility.CreateCustomDataAddToCart(this.CartList);
					break;

				case FacebookConversionConstants.FACEBOOK_EVENT_NAME_PURCHASE:
					customData = facebookConversionUtility.CreateCustomDataPurchase(this.CustomDataOrderId);
					break;

				default:
					customData = new FacebookConversionDataRequest.CustomData();
					break;
			}

			var request = facebookConversionUtility.CreateConvertFaceBookRequest(
				this.EventName,
				this.UserId,
				this.EventSourceUrl,
				this.EventId,
				false,
				customData);

			new FacebookConversionApiFacade().CallAPIFacebook(request);
		}

		/// <summary>Event name</summary>
		public string EventName { get; set; }
		/// <summary>Event id</summary>
		public string EventId { get; set; }
		/// <summary>Custom data content category</summary>
		public string CustomDataContentCategory { get; set; }
		/// <summary>Custom data content ids</summary>
		public string CustomDataContentIds { get; set; }
		/// <summary>Custom data content name</summary>
		public string CustomDataContentName { get; set; }
		/// <summary>Custom data content type</summary>
		public string CustomDataContentType { get; set; }
		/// <summary>Custom data contents</summary>
		public string CustomDataContents { get; set; }
		/// <summary>Custom data currency</summary>
		public string CustomDataCurrency { get; set; }
		/// <summary>Custom data delivery category</summary>
		public string CustomDataDeliveryCategory { get; set; }
		/// <summary>Custom Data number items</summary>
		public string CustomDataNumItems { get; set; }
		/// <summary>Custom data order id</summary>
		public string CustomDataOrderId { get; set; }
		/// <summary>Custom data predicted Ltv</summary>
		public string CustomDataPredictedLtv { get; set; }
		/// <summary>Custom data search string</summary>
		public string CustomDataSearchString { get; set; }
		/// <summary>Custom data status</summary>
		public string CustomDataStatus { get; set; }
		/// <summary>Custom data value</summary>
		public decimal CustomDataValue { get; set; }
		/// <summary>User data</summary>
		public string UserId { get; set; }
		/// <summary>Cart list</summary>
		public CartObjectList CartList { get; set; }
		/// <summary>Event source url</summary>
		public string EventSourceUrl { get; set; }
	}
}