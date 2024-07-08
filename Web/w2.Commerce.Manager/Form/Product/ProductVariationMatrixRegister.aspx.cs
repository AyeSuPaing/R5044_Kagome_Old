/*
=========================================================================================================
  Module      : 商品バリエーション一括登録ページ処理(ProductVariationMatrixRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

public partial class Form_Product_ProductVariationMatrixRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 画面初期化
			RefleshVariationType();
		}
	}

	/// <summary>
	/// 確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		StringBuilder errorMessages = new StringBuilder();
		{
			// Hashtable格納
			Hashtable htCheckInput = new Hashtable();
			htCheckInput.Add(Constants.FIELD_PRODUCTVARIATION_PRICE, tbPrice.Text);
			htCheckInput.Add(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE, tbSpecialPrice.Text);
			foreach (RepeaterItem ri in rRakutenTypeI_Vertical.Items)
			{
				htCheckInput.Add(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1 + "_" + ri.ItemIndex.ToString(), ((TextBox)ri.FindControl("tbMallVariationId1")).Text.Trim());
				htCheckInput.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1 + "_" + ri.ItemIndex.ToString(), ((TextBox)ri.FindControl("tbVariationName1")).Text.Trim());
			}
			foreach (RepeaterItem ri in rRakutenTypeI_Horizonal.Items)
			{
				htCheckInput.Add(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2 + "_" + ri.ItemIndex.ToString(), ((TextBox)ri.FindControl("tbMallVariationId2")).Text.Trim());
				htCheckInput.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2 + "_" + ri.ItemIndex.ToString(), ((TextBox)ri.FindControl("tbVariationName2")).Text.Trim());
			}

			// 入力チェック
			errorMessages.Append(Validator.Validate("ProductVariationMatrixRegister", htCheckInput));
			foreach (RepeaterItem ri in rRakutenTypeI_Vertical.Items)
			{
				errorMessages.Replace("@@" + ri.ItemIndex + "@@", ri.ItemIndex + "行目の");
			}

			// エラー表示
			if (errorMessages.Length != 0)
			{
				tbdyVariationMatrixErrorMessages.Visible = true;
				lbVariationMatrixErrorMessages.Text = errorMessages.ToString(); ;
				return;
			}
		}

		//------------------------------------------------------
		// 呼び出し元へ入力情報セット
		//------------------------------------------------------
		KeyValuePair<string, string> kvpVerticalIdAndNameParameters = GetVerticalIdAndNameParameters();
		KeyValuePair<string, string> kvpHorizonalIdAndNameParameters = GetHorizonalIdAndNameParameters();

		// JavaScript生成用パラメタセット
		OutputParams.Add(tbPrice.Text);
		OutputParams.Add(tbSpecialPrice.Text);
		OutputParams.Add(rblVariationType.SelectedValue);
		OutputParams.Add(kvpVerticalIdAndNameParameters.Key);
		OutputParams.Add(kvpVerticalIdAndNameParameters.Value);
		OutputParams.Add(kvpHorizonalIdAndNameParameters.Key);
		OutputParams.Add(kvpHorizonalIdAndNameParameters.Value);
		OutputParams.Add(rblReplaceOrAdd.SelectedValue);

		divExecScript.Visible = true;
	}

	/// <summary>
	/// パラメータ取得（バリエーション連携ID1・バリエーション名1）
	/// </summary>
	protected KeyValuePair<string, string> GetVerticalIdAndNameParameters()
	{
		// ターゲット確定
		Repeater rTarget = null;
		switch (rblVariationType.SelectedValue)
		{
			case "rakuten_i":
				rTarget = rRakutenTypeI_Vertical;
				break;

			case "rakuten_s":
			case "rakuten_c":
				rTarget = rRakutenTypeSC1;
				break;
		}

		// 値取得
		StringBuilder sbIDs = new StringBuilder();
		StringBuilder sbNames = new StringBuilder();
		foreach (RepeaterItem ri in rTarget.Items)
		{
			string strIdValue = ((TextBox)ri.FindControl("tbMallVariationId1")).Text.Trim();
			string strNameValue = ((TextBox)ri.FindControl("tbVariationName1")).Text.Trim();
			if ((strIdValue != "") || (strNameValue != ""))
			{
				if (sbIDs.Length + sbNames.Length != 0)
				{
					sbIDs.Append(" ");
					sbNames.Append(" ");
				}
				sbIDs.Append(strIdValue);
				sbNames.Append(strNameValue);
			}
		}
		return new KeyValuePair<string, string>(sbIDs.ToString(), sbNames.ToString());
	}

	/// <summary>
	/// パラメータ取得（バリエーション連携ID2・バリエーション名2）
	/// </summary>
	protected KeyValuePair<string, string> GetHorizonalIdAndNameParameters()
	{
		StringBuilder sbIDs = new StringBuilder();
		StringBuilder sbNames = new StringBuilder();

		// ターゲット確定
		Repeater rTarget = null;
		switch (rblVariationType.SelectedValue)
		{
			case "rakuten_i":
				rTarget = rRakutenTypeI_Horizonal;
				break;

			case "rakuten_s":
			case "rakuten_c":
				return new KeyValuePair<string, string>("", "");
		}

		foreach (RepeaterItem ri in rTarget.Items)
		{
			string mallVariationId2 = ((TextBox)ri.FindControl("tbMallVariationId2")).Text.Trim();
			string variationName2 = ((TextBox)ri.FindControl("tbVariationName2")).Text.Trim();
			if ((mallVariationId2 != "") || (variationName2 != ""))
			{
				if (sbIDs.Length + sbNames.Length != 0)
				{
					sbIDs.Append(" ");
					sbNames.Append(" ");
				}
				sbIDs.Append(mallVariationId2);
				sbNames.Append(variationName2);
			}
		}
		return new KeyValuePair<string, string>(sbIDs.ToString(), sbNames.ToString());
	}

	/// <summary>
	/// バリエーションタイプ変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblVariationType_SelectedIndexChanged(object sender, EventArgs e)
	{
		RefleshVariationType();
	}

	/// <summary>
	/// バリエーションタイプによる画面更新
	/// </summary>
	private void RefleshVariationType()
	{
		trVariationTypeI.Visible = false;
		trVariationTypeSC.Visible = false;

		switch (rblVariationType.SelectedValue)
		{
			case "rakuten_i":
				{
					trVariationTypeI.Visible = true;

					List<object> lObjects = new List<object>();
					for (int iLoop = 0; iLoop < 10; iLoop++)
					{
						lObjects.Add(null);
					}

					rRakutenTypeI_Vertical.DataSource = lObjects;
					rRakutenTypeI_Vertical.DataBind();
					rRakutenTypeI_Horizonal.DataSource = lObjects;
					rRakutenTypeI_Horizonal.DataBind();
				}
				break;

			case "rakuten_s":
			case "rakuten_c":
				trVariationTypeSC.Visible = true;
				{
					List<object> lObjects = new List<object>();
					for (int iLoop = 0; iLoop < 15; iLoop++)
					{
						lObjects.Add(null);
					}

					rRakutenTypeSC1.DataSource = lObjects;
					rRakutenTypeSC1.DataBind();
					rRakutenTypeSC2.DataSource = lObjects;
					rRakutenTypeSC2.DataBind();
				}
				break;

		}
	}

	/// <summary>出力パラメタ</summary>
	protected List<string> OutputParams
	{
		get { return m_lOutputParams; }
	}
	List<string> m_lOutputParams = new List<string>();
}
