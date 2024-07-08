/*
=========================================================================================================
  Module      : Result Page (ResultPage.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Product;
using w2.Domain.ScoringSale;

/// <summary>
/// Result page
/// </summary>
public partial class Form_ScoringSale_ResultPage : ProductPage
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender">Object</param>
	/// <param name="e">EventArgs</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			GoToPageSessionExpired();

			SetProductDataForDisplay();

			ReplaceTag();

			this.DataBind();

			if (this.IsScoringSalePreview == false)
			{
				var contentId = string.Format(
					"{0}_{1}",
					this.ScoringSale.ScoringSaleId,
					this.ScoringSale.IsUseTopPage
						? this.ScoringSale.QuestionPages.Length + 2
						: this.ScoringSale.QuestionPages.Length + 1);

				ContentsLogUtility.InsertPageViewContentsLog(
					Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_SCORINGSALE,
					contentId,
					this.IsSmartPhone);
			}

			this.ScoringSale.QuestionPages = Array.Empty<ScoringSaleQuestionPageInput>();
			this.ScoringSale.CurrentQuestionPage = null;
		}
	}

	/// <summary>
	/// Set product data for display
	/// </summary>
	private void SetProductDataForDisplay()
	{
		this.ScoringSaleProduct = GetProductScoringSaleResult();

		if (this.ScoringSaleProduct == null)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var products = ProductCommon.GetScoringSaleResultProduct(
			this.ScoringSaleProduct.ShopId,
			this.ScoringSaleProduct.ProductId,
			this.MemberRankId);

		if (products.Count == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);

			if (Constants.FRIENDLY_URL_ENABLED)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			else
			{
				Response.StatusCode = (int)HttpStatusCode.NotFound;
				Response.End();
			}
		}

		this.ProductMaster = products[0];
		this.ProductVariationMasterList = products;
		this.HasVariation = HasVariation(this.ProductMaster);
		this.ProductId = this.ScoringSaleProduct.ProductId;

		SetProductSubImageValue();

		if (this.HasVariation)
		{
			var checkProductVariation = string.IsNullOrEmpty(this.VariationId)
				|| (Constants.PRODUCTDETAIL_VARIATION_SINGLE_SELECTED
					&& (this.ProductVariationMasterList.Count == 1));
			if (!IsPostBack && checkProductVariation)
			{
				var productVariation = DomainFacade.Instance.ProductService.GetProductVariation(
					this.ScoringSaleProduct.ShopId,
					this.ScoringSaleProduct.ProductId,
					this.ScoringSaleProduct.VariationId,
					this.MemberRankId);
				this.VariationId = (productVariation != null)
					? this.ScoringSaleProduct.VariationId
					: StringUtility.ToEmpty(this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
				this.VariationSelected = true;
			}

			SetVariationSelect();
		}
		else
		{
			this.VariationId = StringUtility.ToEmpty(this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
			this.VariationSelected = true;
		}
	}

	/// <summary>
	/// Get product scoring sale result
	/// </summary>
	/// <returns>Recommend product</returns>
	public ScoringSaleProductModel GetProductScoringSaleResult()
	{
		var service = DomainFacade.Instance.ScoringSaleService;
		var products = service.GetScoringSaleProducts(this.ScoringSale.ScoringSaleId, Constants.CONST_DEFAULT_SHOP_ID);
		var productList = new List<ScoringSaleProductModel>();

		if (products.Length == 0) return new ScoringSaleProductModel();

		// Get product no-condition
		var productNoCodition = products.Last();

		foreach (var product in products.Take(products.Length - 1))
		{
			var conditions = service.GetScoringSaleResultConditions(product.ScoringSaleId, product.BranchNo);
			var conditionsGroup = GetListConditionScoringSaleProduct(conditions);
			var conditionCommonIndex = conditions
				.GroupBy(condition => condition.GroupNo)
				.Where(condition => condition.Count() == 1)
				.Select(condition => condition.Key);

			var conditionFlg = conditionCommonIndex.Count() > 0
				? conditions.FirstOrDefault(condition => (condition.GroupNo == conditionCommonIndex.FirstOrDefault())).Condition
				: conditions[0].Condition == Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND
					? Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR
					: Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND;

			if ((conditionFlg == Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR)
				&& conditionsGroup.Any(item => item.Values.FirstOrDefault()))
			{
				productList.Add(product);
			}
			else if ((conditionFlg == Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND)
				&& conditionsGroup.All(item => item.Values.FirstOrDefault()))
			{
				productList.Add(product);
			}
		}

		var productResult = productList
			.OrderBy(item => item.BranchNo)
			.Distinct()
			.FirstOrDefault();

		if (productResult == null) productResult = productNoCodition;

		SetScoreAxisData(this.ScoringSale.TotalScore, this.ScoringSale.ScoreAxisId);

		return productResult;
	}

	/// <summary>
	/// Get list condition scoring sale product
	/// </summary>
	/// <param name="conditions">Conditions</param>
	/// <returns>List coditions group</returns>
	public List<Dictionary<string, bool>> GetListConditionScoringSaleProduct(ScoringSaleResultConditionModel[] conditions)
	{
		var conditionGroups = new List<Dictionary<string, bool>>();

		foreach (var condition in conditions)
		{
			var axisAddition = this.ScoringSale.GetAxisAdditional(condition.ScoreAxisAxisNo);
			var axisAdditionMax = this.ScoringSale.GetAllAxisAdditionals().Max(item => item.Value);
			var isConditionValueMax = ((StringUtility.ToEmpty(condition.ScoreAxisAxisValueFrom) == Constants.SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MIN)
				&& (StringUtility.ToEmpty(condition.ScoreAxisAxisValueTo) == Constants.SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MAX));

			var isScoringSaleProduct = isConditionValueMax
				? (axisAddition == axisAdditionMax)
				: ((axisAddition >= condition.ScoreAxisAxisValueFrom)
					&& (axisAddition <= condition.ScoreAxisAxisValueTo));

			if (conditionGroups.Any(item => (item.Keys.FirstOrDefault() == condition.GroupNo.ToString())) == false)
			{
				var conditionItem = new Dictionary<string, bool>
				{
					{ condition.GroupNo.ToString() , isScoringSaleProduct },
				};
				conditionGroups.Add(conditionItem);
			}
			else
			{
				foreach (var coditionItem in conditionGroups)
				{
					if ((coditionItem.Keys.FirstOrDefault() != condition.GroupNo.ToString())
						|| ((conditions[0].Condition == Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR)
							&& coditionItem.Values.FirstOrDefault())
						|| ((conditions[0].Condition == Constants.FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND)
							&& (coditionItem.Values.FirstOrDefault() == false)))
					{
						continue;
					}

					coditionItem[condition.GroupNo.ToString()] = isScoringSaleProduct;
				}
			}
		}
		return conditionGroups;
	}

	/// <summary>
	/// Set product variation status
	/// </summary>
	/// <param name="product">Product</param>
	/// <param name="variationId">Variation id</param>
	protected void SetProductVariationStatus(DataRowView product, string variationId)
	{
		// By design, variation_id is not case sensitive
		if (string.IsNullOrEmpty(this.VariationId)) return;

		if (this.VariationId.Equals(variationId, StringComparison.OrdinalIgnoreCase))
		{
			this.ProductMaster = product;
			this.VariationSelected = true;
		}
	}

	/// <summary>
	/// Variation change common processing
	/// </summary>
	private void ChangeVariation()
	{
		// Variation selection state set
		this.VariationSelected = (string.IsNullOrEmpty(this.VariationId) == false);

		// Set product data on the screen
		SetProductDataForDisplay();

		// Data binding the variation update target
		DataBindByClass(this.Page, "ChangesByVariation");

		this.DataBind();
	}

	/// <summary>
	/// Go to product detail
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbGoProductDetail_Click(object sender, EventArgs e)
	{
		if (this.IsScoringSalePreview)
		{
			var resultUrl = new UrlCreator(this.Request.RawUrl).CreateUrl();
			Response.Redirect(resultUrl);
		}

		var products = ProductCommon.GetScoringSaleResultProduct(
			this.ShopId,
			this.ProductId,
			this.MemberRankId);

		var errMessage = string.Empty;
		if (products.Count == 0)
		{
			errMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);
		}
		else
		{
			this.ProductMaster = products[0];

			if ((int)ProductMaster[Constants.FIELD_DISP_PERIOD_FLG] == 0)
			{
				errMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_SELL)
					.Replace("@@ 1 @@", (string)ProductMaster[Constants.FIELD_PRODUCT_NAME]);
			}
			else if ((int)ProductMaster[Constants.FIELD_DISP_ONLY_FIXED_FLG] == 1 && this.LoginUserFixedPurchaseMemberFlg == null)
			{
				errMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_DISPLAY_FIXED_PURCHASE_MEMBER);
			}
			else if ((int)ProductMaster[Constants.FIELD_DISP_ENABLE_RANK_FLG] == 1)
			{
				errMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_DISPLAY_MEMBER_RANK)
					.Replace("@@ 1 @@", (string)products[0][Constants.FIELD_DISP_ENABLE_RANK]);
			}
		}
		if ((string.IsNullOrEmpty(errMessage)) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errMessage;
			if (Constants.FRIENDLY_URL_ENABLED)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			else
			{
				Response.StatusCode = (int)HttpStatusCode.NotFound;
				Response.End();
			}
		}

		foreach (DataRowView product in products)
		{
			this.ProductMaster = (StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_VARIATION_ID]) == this.VariationId)
				? product
				: this.ProductMaster;
		}

		Response.Redirect(CreateProductDetailVariationUrl(this.ProductMaster));
	}

	/// <summary>
	/// Variation image selection
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariaionImages_OnClick(object sender, EventArgs e)
	{
		this.VariationId = ((LinkButton)sender).CommandArgument;

		// Variation change common processing
		ChangeVariation();
	}

	/// <summary>
	/// Set variation select
	/// </summary>
	private void SetVariationSelect()
	{
		this.ProductValirationListItemCollection = new ListItemCollection();

		foreach (DataRowView product in this.ProductVariationMasterList)
		{
			var variationName = CreateVariationName(
				product,
				string.Empty,
				string.Empty,
				Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);
			var item = new ListItem(
				StringUtility.ToEmpty(variationName),
				StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]));

			this.ProductValirationListItemCollection.Add(item);

			SetProductVariationStatus(
				product,
				StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]));
		}
	}

	/// <summary>
	/// Set product sub image value
	/// </summary>
	private void SetProductSubImageValue()
	{
		this.ProductSubImageList = new List<DataRowView>();
		var productSubImageSettings = ProductCommon.GetProductSubImageSettingList(this.ShopId);
		var productSubImageListTmp = new List<DataRowView>();

		foreach (DataRowView productSubImage in productSubImageSettings)
		{
			if (CheckProductSubImageExist(this.ProductMaster, (int)productSubImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO]) == false) continue;

			productSubImageListTmp.Add(productSubImage);
		}

		if (productSubImageListTmp.Count != 0)
		{
			var mainImage = productSubImageSettings.AddNew();
			mainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO] = Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1;
			mainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME] = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_DETAIL,
				Constants.VALUETEXT_PARAM_PRODUCT_DATA_SETTING_MESSAGE,
				Constants.VALUETEXT_PARAM_PRODUCT_DATA_MAIN_IMAGE);

			this.ProductSubImageList.Add(mainImage);
			foreach (var productSubImage in productSubImageListTmp)
			{
				this.ProductSubImageList.Add(productSubImage);
			}
		}
	}

	/// <summary>
	/// Get product description
	/// </summary>
	/// <returns>A string of product description</returns>
	protected string GetProductDescription()
	{
		var result = string.Format(
			"{0}{1}{2}{3}",
			GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL1),
			GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL2),
			GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL3),
			GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL4));
		return result;
	}

	/// <summary>
	/// Get scoring sale datas
	/// </summary>
	private Hashtable GetScoringSaleDatas()
	{
		var scoringDatas = new Hashtable();

		if (this.ProductVariation != null)
		{
			var productPrice = CurrencyManager.ToPrice(this.ProductVariation.Price);
			scoringDatas.Add(Constants.FIELD_SCORINGSALEPRODUCT_PRODUCT_ID, this.ProductVariation.ProductId);
			scoringDatas.Add(Constants.FIELD_SCORINGSALE_PRODUCT_NAME, this.ProductVariation.Name);
			scoringDatas.Add(Constants.FIELD_SCORINGSALEPRODUCT_VARIATION_ID, this.ProductVariation.VariationId);
			scoringDatas.Add(Constants.FIELD_SCORINGSALE_PRODUCT_PRICE, productPrice);
		}

		var scoringAxis = DomainFacade.Instance.ScoreAxisService.Get(this.ScoringSale.ScoreAxisId);
		var scoreAxisScore = this.ScoringSale.TotalScore;

		if (scoringAxis != null)
		{
			foreach (string key in scoringAxis.DataSource.Keys)
			{
				if (key.Contains(Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_NAME) == false) continue;

				var axisNo = Regex.Match(key, @"\d+").Value;
				var axisValueNo = string.Format(
					"{0}{1}",
					Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO,
					axisNo);

				scoringDatas.Add(
					string.Format(
						"{0}:{1}",
						Constants.FIELD_SCORE_AXIS,
						axisNo),
					StringUtility.ToEmpty(scoreAxisScore.DataSource[axisValueNo]));
			}
		}

		if (this.LoginUser != null)
		{
			scoringDatas.Add(Constants.FIELD_USER_USER_ID, this.LoginUser.UserId);
			scoringDatas.Add(Constants.FIELD_USER_NAME, this.LoginUser.Name);
			scoringDatas.Add(Constants.FIELD_USER_NAME1, this.LoginUser.Name1);
			scoringDatas.Add(Constants.FIELD_USER_NAME2, this.LoginUser.Name2);
			scoringDatas.Add(Constants.FIELD_USER_NAME_KANA, this.LoginUser.NameKana);
			scoringDatas.Add(Constants.FIELD_USER_NAME_KANA1, this.LoginUser.NameKana1);
			scoringDatas.Add(Constants.FIELD_USER_NAME_KANA2, this.LoginUser.NameKana2);
			scoringDatas.Add(Constants.FIELD_USER_NICK_NAME, this.LoginUser.NickName);
			scoringDatas.Add(Constants.FIELD_USER_MAIL_ADDR, this.LoginUser.MailAddr);
			scoringDatas.Add(Constants.FIELD_USER_LOGIN_ID, this.LoginUser.LoginId);
			scoringDatas.Add(Constants.FIELD_USER_MEMBER_RANK_ID, this.LoginUser.MemberRankId);
			scoringDatas.Add(
				Constants.FIELD_USER_SEX,
				ValueText.GetValueText(
					Constants.TABLE_USER,
					Constants.FIELD_USER_SEX,
					this.LoginUser.Sex));
		}

		return scoringDatas;
	}

	/// <summary>
	/// Replace product id
	/// </summary>
	/// <param name="htmlContent">Html content</param>
	/// <param name="tagHead">Tag product head</param>
	/// <param name="tagFoot">Tag product foot</param>
	/// <returns>Source replace</returns>
	private string ReplaceProductId(string htmlContent, string tagHead, string tagFoot)
	{
		foreach (Match matchFind in GetTagMatches(htmlContent, tagHead, tagFoot))
		{
			var valueToCompare = matchFind.Value
				.Replace(tagHead, string.Empty)
				.Replace(tagFoot, string.Empty);

			if (valueToCompare == StringUtility.ToEmpty(this.ProductVariation.ProductId))
			{
				htmlContent = SetTagEnabled(string.Format("ProductId:{0}", valueToCompare), htmlContent);
			}
			else
			{
				htmlContent = SetTagDisabled(string.Format("ProductId:{0}", valueToCompare), htmlContent);
			}
		}

		return htmlContent;
	}

	/// <summary>
	/// Replace score axis
	/// </summary>
	/// <param name="htmlContent">Html content</param>
	/// <param name="tagHead">Tag head</param>
	/// <param name="tagFoot">Tag foot</param>
	/// <returns>Source replace</returns>
	private string ReplaceScoreAxis(string htmlContent, string tagHead, string tagFoot)
	{
		var totalScore = this.ScoringSale.TotalScore;

		foreach (Match matchFind in GetTagMatches(htmlContent, tagHead, tagFoot))
		{
			var valueToCompare = matchFind.Value
				.Replace(tagHead, string.Empty)
				.Replace(tagFoot, string.Empty);
			var scoreAxis = valueToCompare.Split('-');
			int scoreAxisFrom, scoreAxisTo;
			var isEnable = false;

			if ((scoreAxis.Length == 3)
				&& int.TryParse(scoreAxis[1], out scoreAxisFrom)
				&& int.TryParse(scoreAxis[2], out scoreAxisTo))
			{
				foreach (string key in totalScore.DataSource.Keys)
				{
					if (key.Contains(Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO))
					{
						var axisAdditionalNo = Regex.Match(key, @"\d+").Value;
						var valueAxis = int.Parse(totalScore.DataSource[key].ToString());

						if ((axisAdditionalNo == scoreAxis[0])
							&& (valueAxis >= scoreAxisFrom)
							&& (valueAxis <= scoreAxisTo))
						{
							isEnable = true;
							break;
						}
					}
				}
			}

			if (isEnable)
			{
				htmlContent = SetTagEnabled(string.Format("ScoreAxis:{0}", valueToCompare), htmlContent);
			}
			else
			{
				htmlContent = SetTagDisabled(string.Format("ScoreAxis:{0}", valueToCompare), htmlContent);
			}
		}

		return htmlContent;
	}

	/// <summary>
	/// Set html content
	/// </summary>
	private void SetHtmlContent()
	{
		var tagProductHead = "<@@ProductId:";
		var tagProductFoot = "@@>";

		this.HtmlContentUp = ReplaceProductId(this.HtmlContentUp, tagProductHead, tagProductFoot);
		this.HtmlContentDown = ReplaceProductId(this.HtmlContentDown, tagProductHead, tagProductFoot);

		var tagScoreAxisHead = "<@@ScoreAxis:";
		var tagScoreAxisFoot = "@@>";
		var totalScore = this.ScoringSale.TotalScore;

		this.HtmlContentUp = ReplaceScoreAxis(
				this.HtmlContentUp,
				tagScoreAxisHead,
				tagScoreAxisFoot)
			.Replace("\n", "<br />");

		this.HtmlContentDown = ReplaceScoreAxis(
				this.HtmlContentDown,
				tagScoreAxisHead,
				tagScoreAxisFoot)
			.Replace("\n", "<br />");

		if (this.IsLoggedIn)
		{
			this.HtmlContentUp = SetTagEnabled("IsUser", this.HtmlContentUp);
			this.HtmlContentDown = SetTagEnabled("IsUser", this.HtmlContentDown);
		}
		else
		{
			this.HtmlContentUp = SetTagDisabled("IsUser", this.HtmlContentUp);
			this.HtmlContentDown = SetTagDisabled("IsUser", this.HtmlContentDown);
		}
	}

	/// <summary>
	/// Replace tag
	/// </summary>
	private void ReplaceTag()
	{
		var productVariation = DomainFacade.Instance.ProductService.GetProductVariation(
			this.ScoringSaleProduct.ShopId,
			this.ScoringSaleProduct.ProductId,
			this.VariationId,
			this.MemberRankId);

		this.HtmlContentUp = this.ScoringSale.ResultPageBodyAbove;
		this.HtmlContentDown = this.ScoringSale.ResultPageBodyBelow;
		this.ProductVariation = productVariation;

		SetHtmlContent();

		var data = GetScoringSaleDatas();

		foreach (var key in data.Keys)
		{
			this.HtmlContentUp = this.HtmlContentUp.Replace("@@ " + key + " @@", StringUtility.ToEmpty(data[key]));
			this.HtmlContentDown = this.HtmlContentDown.Replace("@@ " + key + " @@", StringUtility.ToEmpty(data[key]));
		}
	}

	/// <summary>
	/// Set score axis data
	/// </summary>
	/// <param name="totalScore">Total score</param>
	/// <param name="scoreAxisId">Score axis ID</param>
	protected void SetScoreAxisData(ScoreAxisInput totalScore, string scoreAxisId)
	{
		var scoreAxis = DomainFacade.Instance.ScoreAxisService.Get(scoreAxisId);
		var scoreDatas = new List<int>();
		var scoreNames = new List<string>();
		var scoreAxisSort = scoreAxis.DataSource.Keys.Cast<string>().ToList()
			.Where(key => key.StartsWith(Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_NAME))
			.OrderBy(key => int.Parse(Regex.Match(key.ToString(), @"\d+").Value))
			.ToList();

		foreach (var key in scoreAxisSort)
		{
			var keyvalue = StringUtility.ToEmpty(scoreAxis.DataSource[key]);

			if ((string.IsNullOrEmpty(keyvalue) == false)
				&& (StringUtility.ToEmpty(key).Contains(Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_NAME)))
			{
				var scoreAxisNo = Regex.Match(key.ToString(), @"\d+").Value;
				var scoreAxisName = Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO + scoreAxisNo;
				int scoreAxisData;

				if (int.TryParse(StringUtility.ToEmpty(totalScore.DataSource[scoreAxisName]), out scoreAxisData))
				{
					if (scoreAxisData < 0) scoreAxisData = 0;
					scoreDatas.Add(scoreAxisData);
					scoreNames.Add(keyvalue);
				}
			}
		}

		this.RadarChartUseFlag = this.ScoringSale.RadarChartUseFlg;
		this.RadarChartTitle = this.ScoringSale.RadarChartTitle;
		this.ScoreAxisNames = JsonConvert.SerializeObject(scoreNames);
		this.ScoreAxisDatas = JsonConvert.SerializeObject(scoreDatas);
	}

	/// <summary>
	/// Set tag enabled
	/// </summary>
	/// <param name="tagName">Tag name</param>
	/// <param name="source">Source</param>
	/// <returns>Source replace</returns>
	private string SetTagEnabled(string tagName, string source)
	{
		string tagBgn = "<@@" + tagName + "@@>";
		string tagEnd = "</@@" + tagName + "@@>";

		source = Regex.Replace(source,
			tagBgn + "\r\n",
			string.Empty,
			RegexOptions.Singleline | RegexOptions.IgnoreCase);
		source = Regex.Replace(source,
			tagEnd + "\r\n",
			string.Empty,
			RegexOptions.Singleline | RegexOptions.IgnoreCase);
		source = Regex.Replace(source,
			tagBgn,
			string.Empty,
			RegexOptions.Singleline | RegexOptions.IgnoreCase);
		source = Regex.Replace(source,
			tagEnd,
			string.Empty,
			RegexOptions.Singleline | RegexOptions.IgnoreCase);

		return source;
	}

	/// <summary>
	/// Set tag disabled
	/// </summary>
	/// <param name="tagName">Tag name</param>
	/// <param name="source">Source</param>
	/// <returns>Source replace</returns>
	private string SetTagDisabled(string tagName, string source)
	{
		string tagBgn = "<@@" + tagName + "@@>";
		string tagEnd = "</@@" + tagName + "@@>";

		source = Regex.Replace(source,
			string.Format("{0}(?:(?!{1}|{2}).)*{3}\r\n",
			tagBgn,
			tagBgn,
			tagEnd,
			tagEnd),
			string.Empty,
			RegexOptions.Singleline | RegexOptions.IgnoreCase);
		source = Regex.Replace(source,
			string.Format("{0}(?:(?!{1}|{2}).)*{3}",
			tagBgn,
			tagBgn,
			tagEnd,
			tagEnd),
			string.Empty,
			RegexOptions.Singleline | RegexOptions.IgnoreCase);

		return source;
	}

	/// <summary>
	/// Get tag matches
	/// </summary>
	/// <param name="targetString">Target string</param>
	/// <param name="tagHead">Tag head</param>
	/// <param name="tagFoot">Tag foot</param>
	/// <returns>Tag pattern matching collection</returns>
	private MatchCollection GetTagMatches(string targetString, string tagHead, string tagFoot)
	{
		return Regex.Matches(targetString, tagHead + ".*?" + tagFoot, RegexOptions.Singleline | RegexOptions.IgnoreCase);
	}

	/// <summary>
	/// Go to page session expired
	/// </summary>
	protected void GoToPageSessionExpired()
	{
		if (this.ScoringSale == null)
		{
			var scoringSaleId = this.Request.Params[Constants.REQUEST_KEY_SCORINGSALE_ID];
			var scoringSale = DomainFacade.Instance.ScoringSaleService.GetScoringSale(scoringSaleId);
			var page = (scoringSale.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_ON)
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE;
			var urlPath = Path.Combine(
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				page);
			this.ScoringSaleErrorSessionExpired = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SCORINGSALE_SESSION_EXPIRED);

			var url = new UrlCreator(urlPath)
				.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, scoringSale.ScoringSaleId)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	#region +Properties
	/// <summary>Scoring sale page product</summary>
	public ScoringSaleProductModel ScoringSaleProduct { get; set; }
	/// <summary>Product master</summary>
	protected DataRowView ProductMaster { get; private set; }
	/// <summary>Product variation list Item collection</summary>
	protected ListItemCollection ProductValirationListItemCollection { get; private set; }
	/// <summary>Product sub-image list</summary>
	protected List<DataRowView> ProductSubImageList { get; private set; }
	/// <summary>Product variation master</summary>
	protected DataView ProductVariationMasterList { get; private set; }
	/// <summary>Html content up</summary>
	protected string HtmlContentUp { get; private set; }
	/// <summary>Html content down</summary>
	protected string HtmlContentDown { get; private set; }
	/// <summary>Do you have variations</summary>
	protected new bool HasVariation
	{
		get { return (bool)ViewState["HasVariation"]; }
		private set { ViewState["HasVariation"] = value; }
	}
	/// <summary>Is a variation selected?</summary>
	protected bool VariationSelected
	{
		get { return (ViewState["SelectVariation"] != null) ? (bool)ViewState["SelectVariation"] : false; }
		private set { ViewState["SelectVariation"] = value; }
	}
	/// <summary>Variation id</summary>
	protected string VariationId
	{
		get { return (string)ViewState["VariationId"]; }
		set { ViewState["VariationId"] = value; }
	}
	/// <summary>Scoring sale</summary>
	protected ScoringSaleInput ScoringSale
	{
		get { return (ScoringSaleInput)Session["ScoringSaleInput"]; }
		set { Session["ScoringSaleInput"] = value; }
	}
	/// <summary>Is scoring sale preview</summary>
	public bool IsScoringSalePreview
	{
		get { return this.Request.RawUrl.Contains(Constants.REQUEST_KEY_PREVIEW_KEY); }
	}
	/// <summary>Score axis names</summary>
	public string ScoreAxisNames { get; set; }
	/// <summary>Score axis datas</summary>
	public string ScoreAxisDatas { get; set; }
	/// <summary>Radar chart title</summary>
	public string RadarChartTitle { get; set; }
	/// <summary>Radar chart use flag</summary>
	public string RadarChartUseFlag { get; set; }
	/// <summary>Product variation</summary>
	public ProductModel ProductVariation { get; set; }
	/// <summary>Scoring sale error session expired</summary>
	public string ScoringSaleErrorSessionExpired
	{
		get { return (string)this.Session["ScoringSaleErrorSessionExpired"]; }
		set { this.Session["ScoringSaleErrorSessionExpired"] = value; }
	}
	#endregion
}
