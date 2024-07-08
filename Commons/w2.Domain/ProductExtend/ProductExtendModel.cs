/*
=========================================================================================================
  Module      : 商品拡張拡張項目マスタモデル (ProductExtendModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.ProductExtend
{
	/// <summary>
	/// 商品拡張拡張項目マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductExtendModel : ModelBase<ProductExtendModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductExtendModel()
		{
			this.ShopId = string.Empty;
			this.ProductId = string.Empty;
			this.Extend1 = string.Empty;
			this.Extend2 = string.Empty;
			this.Extend3 = string.Empty;
			this.Extend4 = string.Empty;
			this.Extend5 = string.Empty;
			this.Extend6 = string.Empty;
			this.Extend7 = string.Empty;
			this.Extend8 = string.Empty;
			this.Extend9 = string.Empty;
			this.Extend10 = string.Empty;
			this.Extend11 = string.Empty;
			this.Extend12 = string.Empty;
			this.Extend13 = string.Empty;
			this.Extend14 = string.Empty;
			this.Extend15 = string.Empty;
			this.Extend16 = string.Empty;
			this.Extend17 = string.Empty;
			this.Extend18 = string.Empty;
			this.Extend19 = string.Empty;
			this.Extend20 = string.Empty;
			this.Extend21 = string.Empty;
			this.Extend22 = string.Empty;
			this.Extend23 = string.Empty;
			this.Extend24 = string.Empty;
			this.Extend25 = string.Empty;
			this.Extend26 = string.Empty;
			this.Extend27 = string.Empty;
			this.Extend28 = string.Empty;
			this.Extend29 = string.Empty;
			this.Extend30 = string.Empty;
			this.Extend31 = string.Empty;
			this.Extend32 = string.Empty;
			this.Extend33 = string.Empty;
			this.Extend34 = string.Empty;
			this.Extend35 = string.Empty;
			this.Extend36 = string.Empty;
			this.Extend37 = string.Empty;
			this.Extend38 = string.Empty;
			this.Extend39 = string.Empty;
			this.Extend40 = string.Empty;
			this.Extend41 = string.Empty;
			this.Extend42 = string.Empty;
			this.Extend43 = string.Empty;
			this.Extend44 = string.Empty;
			this.Extend45 = string.Empty;
			this.Extend46 = string.Empty;
			this.Extend47 = string.Empty;
			this.Extend48 = string.Empty;
			this.Extend49 = string.Empty;
			this.Extend50 = string.Empty;
			this.Extend51 = string.Empty;
			this.Extend52 = string.Empty;
			this.Extend53 = string.Empty;
			this.Extend54 = string.Empty;
			this.Extend55 = string.Empty;
			this.Extend56 = string.Empty;
			this.Extend57 = string.Empty;
			this.Extend58 = string.Empty;
			this.Extend59 = string.Empty;
			this.Extend60 = string.Empty;
			this.Extend61 = string.Empty;
			this.Extend62 = string.Empty;
			this.Extend63 = string.Empty;
			this.Extend64 = string.Empty;
			this.Extend65 = string.Empty;
			this.Extend66 = string.Empty;
			this.Extend67 = string.Empty;
			this.Extend68 = string.Empty;
			this.Extend69 = string.Empty;
			this.Extend70 = string.Empty;
			this.Extend71 = string.Empty;
			this.Extend72 = string.Empty;
			this.Extend73 = string.Empty;
			this.Extend74 = string.Empty;
			this.Extend75 = string.Empty;
			this.Extend76 = string.Empty;
			this.Extend77 = string.Empty;
			this.Extend78 = string.Empty;
			this.Extend79 = string.Empty;
			this.Extend80 = string.Empty;
			this.Extend81 = string.Empty;
			this.Extend82 = string.Empty;
			this.Extend83 = string.Empty;
			this.Extend84 = string.Empty;
			this.Extend85 = string.Empty;
			this.Extend86 = string.Empty;
			this.Extend87 = string.Empty;
			this.Extend88 = string.Empty;
			this.Extend89 = string.Empty;
			this.Extend90 = string.Empty;
			this.Extend91 = string.Empty;
			this.Extend92 = string.Empty;
			this.Extend93 = string.Empty;
			this.Extend94 = string.Empty;
			this.Extend95 = string.Empty;
			this.Extend96 = string.Empty;
			this.Extend97 = string.Empty;
			this.Extend98 = string.Empty;
			this.Extend99 = string.Empty;
			this.Extend100 = string.Empty;
			this.Extend101 = string.Empty;
			this.Extend102 = string.Empty;
			this.Extend103 = string.Empty;
			this.Extend104 = string.Empty;
			this.Extend105 = string.Empty;
			this.Extend106 = string.Empty;
			this.Extend107 = string.Empty;
			this.Extend108 = string.Empty;
			this.Extend109 = string.Empty;
			this.Extend110 = string.Empty;
			this.Extend111 = string.Empty;
			this.Extend112 = string.Empty;
			this.Extend113 = string.Empty;
			this.Extend114 = string.Empty;
			this.Extend115 = string.Empty;
			this.Extend116 = string.Empty;
			this.Extend117 = string.Empty;
			this.Extend118 = string.Empty;
			this.Extend119 = string.Empty;
			this.Extend120 = string.Empty;
			this.Extend121 = string.Empty;
			this.Extend122 = string.Empty;
			this.Extend123 = string.Empty;
			this.Extend124 = string.Empty;
			this.Extend125 = string.Empty;
			this.Extend126 = string.Empty;
			this.Extend127 = string.Empty;
			this.Extend128 = string.Empty;
			this.Extend129 = string.Empty;
			this.Extend130 = string.Empty;
			this.Extend131 = string.Empty;
			this.Extend132 = string.Empty;
			this.Extend133 = string.Empty;
			this.Extend134 = string.Empty;
			this.Extend135 = string.Empty;
			this.Extend136 = string.Empty;
			this.Extend137 = string.Empty;
			this.Extend138 = string.Empty;
			this.Extend139 = string.Empty;
			this.Extend140 = string.Empty;
			this.DelFlg = Constants.FLG_PRODUCTEXTEND_DEL_FLG_UNDELETED;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductExtendModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductExtendModel(Hashtable source)
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_SHOP_ID]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_PRODUCT_ID]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_PRODUCT_ID] = value; }
		}
		/// <summary>拡張項目1</summary>
		public string Extend1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND1]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND1] = value; }
		}
		/// <summary>拡張項目2</summary>
		public string Extend2
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND2]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND2] = value; }
		}
		/// <summary>拡張項目3</summary>
		public string Extend3
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND3]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND3] = value; }
		}
		/// <summary>拡張項目4</summary>
		public string Extend4
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND4]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND4] = value; }
		}
		/// <summary>拡張項目5</summary>
		public string Extend5
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND5]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND5] = value; }
		}
		/// <summary>拡張項目6</summary>
		public string Extend6
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND6]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND6] = value; }
		}
		/// <summary>拡張項目7</summary>
		public string Extend7
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND7]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND7] = value; }
		}
		/// <summary>拡張項目8</summary>
		public string Extend8
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND8]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND8] = value; }
		}
		/// <summary>拡張項目9</summary>
		public string Extend9
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND9]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND9] = value; }
		}
		/// <summary>拡張項目10</summary>
		public string Extend10
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND10]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND10] = value; }
		}
		/// <summary>拡張項目11</summary>
		public string Extend11
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND11]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND11] = value; }
		}
		/// <summary>拡張項目12</summary>
		public string Extend12
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND12]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND12] = value; }
		}
		/// <summary>拡張項目13</summary>
		public string Extend13
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND13]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND13] = value; }
		}
		/// <summary>拡張項目14</summary>
		public string Extend14
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND14]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND14] = value; }
		}
		/// <summary>拡張項目15</summary>
		public string Extend15
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND15]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND15] = value; }
		}
		/// <summary>拡張項目16</summary>
		public string Extend16
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND16]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND16] = value; }
		}
		/// <summary>拡張項目17</summary>
		public string Extend17
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND17]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND17] = value; }
		}
		/// <summary>拡張項目18</summary>
		public string Extend18
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND18]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND18] = value; }
		}
		/// <summary>拡張項目19</summary>
		public string Extend19
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND19]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND19] = value; }
		}
		/// <summary>拡張項目20</summary>
		public string Extend20
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND20]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND20] = value; }
		}
		/// <summary>拡張項目21</summary>
		public string Extend21
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND21]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND21] = value; }
		}
		/// <summary>拡張項目22</summary>
		public string Extend22
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND22]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND22] = value; }
		}
		/// <summary>拡張項目23</summary>
		public string Extend23
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND23]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND23] = value; }
		}
		/// <summary>拡張項目24</summary>
		public string Extend24
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND24]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND24] = value; }
		}
		/// <summary>拡張項目25</summary>
		public string Extend25
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND25]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND25] = value; }
		}
		/// <summary>拡張項目26</summary>
		public string Extend26
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND26]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND26] = value; }
		}
		/// <summary>拡張項目27</summary>
		public string Extend27
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND27]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND27] = value; }
		}
		/// <summary>拡張項目28</summary>
		public string Extend28
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND28]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND28] = value; }
		}
		/// <summary>拡張項目29</summary>
		public string Extend29
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND29]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND29] = value; }
		}
		/// <summary>拡張項目30</summary>
		public string Extend30
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND30]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND30] = value; }
		}
		/// <summary>拡張項目31</summary>
		public string Extend31
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND31]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND31] = value; }
		}
		/// <summary>拡張項目32</summary>
		public string Extend32
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND32]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND32] = value; }
		}
		/// <summary>拡張項目33</summary>
		public string Extend33
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND33]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND33] = value; }
		}
		/// <summary>拡張項目34</summary>
		public string Extend34
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND34]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND34] = value; }
		}
		/// <summary>拡張項目35</summary>
		public string Extend35
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND35]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND35] = value; }
		}
		/// <summary>拡張項目36</summary>
		public string Extend36
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND36]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND36] = value; }
		}
		/// <summary>拡張項目37</summary>
		public string Extend37
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND37]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND37] = value; }
		}
		/// <summary>拡張項目38</summary>
		public string Extend38
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND38]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND38] = value; }
		}
		/// <summary>拡張項目39</summary>
		public string Extend39
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND39]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND39] = value; }
		}
		/// <summary>拡張項目40</summary>
		public string Extend40
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND40]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND40] = value; }
		}
		/// <summary>拡張項目41</summary>
		public string Extend41
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND41]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND41] = value; }
		}
		/// <summary>拡張項目42</summary>
		public string Extend42
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND42]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND42] = value; }
		}
		/// <summary>拡張項目43</summary>
		public string Extend43
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND43]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND43] = value; }
		}
		/// <summary>拡張項目44</summary>
		public string Extend44
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND44]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND44] = value; }
		}
		/// <summary>拡張項目45</summary>
		public string Extend45
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND45]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND45] = value; }
		}
		/// <summary>拡張項目46</summary>
		public string Extend46
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND46]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND46] = value; }
		}
		/// <summary>拡張項目47</summary>
		public string Extend47
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND47]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND47] = value; }
		}
		/// <summary>拡張項目48</summary>
		public string Extend48
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND48]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND48] = value; }
		}
		/// <summary>拡張項目49</summary>
		public string Extend49
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND49]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND49] = value; }
		}
		/// <summary>拡張項目50</summary>
		public string Extend50
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND50]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND50] = value; }
		}
		/// <summary>拡張項目51</summary>
		public string Extend51
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND51]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND51] = value; }
		}
		/// <summary>拡張項目52</summary>
		public string Extend52
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND52]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND52] = value; }
		}
		/// <summary>拡張項目53</summary>
		public string Extend53
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND53]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND53] = value; }
		}
		/// <summary>拡張項目54</summary>
		public string Extend54
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND54]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND54] = value; }
		}
		/// <summary>拡張項目55</summary>
		public string Extend55
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND55]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND55] = value; }
		}
		/// <summary>拡張項目56</summary>
		public string Extend56
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND56]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND56] = value; }
		}
		/// <summary>拡張項目57</summary>
		public string Extend57
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND57]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND57] = value; }
		}
		/// <summary>拡張項目58</summary>
		public string Extend58
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND58]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND58] = value; }
		}
		/// <summary>拡張項目59</summary>
		public string Extend59
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND59]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND59] = value; }
		}
		/// <summary>拡張項目60</summary>
		public string Extend60
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND60]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND60] = value; }
		}
		/// <summary>拡張項目61</summary>
		public string Extend61
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND61]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND61] = value; }
		}
		/// <summary>拡張項目62</summary>
		public string Extend62
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND62]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND62] = value; }
		}
		/// <summary>拡張項目63</summary>
		public string Extend63
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND63]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND63] = value; }
		}
		/// <summary>拡張項目64</summary>
		public string Extend64
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND64]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND64] = value; }
		}
		/// <summary>拡張項目65</summary>
		public string Extend65
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND65]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND65] = value; }
		}
		/// <summary>拡張項目66</summary>
		public string Extend66
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND66]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND66] = value; }
		}
		/// <summary>拡張項目67</summary>
		public string Extend67
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND67]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND67] = value; }
		}
		/// <summary>拡張項目68</summary>
		public string Extend68
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND68]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND68] = value; }
		}
		/// <summary>拡張項目69</summary>
		public string Extend69
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND69]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND69] = value; }
		}
		/// <summary>拡張項目70</summary>
		public string Extend70
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND70]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND70] = value; }
		}
		/// <summary>拡張項目71</summary>
		public string Extend71
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND71]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND71] = value; }
		}
		/// <summary>拡張項目72</summary>
		public string Extend72
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND72]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND72] = value; }
		}
		/// <summary>拡張項目73</summary>
		public string Extend73
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND73]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND73] = value; }
		}
		/// <summary>拡張項目74</summary>
		public string Extend74
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND74]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND74] = value; }
		}
		/// <summary>拡張項目75</summary>
		public string Extend75
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND75]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND75] = value; }
		}
		/// <summary>拡張項目76</summary>
		public string Extend76
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND76]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND76] = value; }
		}
		/// <summary>拡張項目77</summary>
		public string Extend77
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND77]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND77] = value; }
		}
		/// <summary>拡張項目78</summary>
		public string Extend78
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND78]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND78] = value; }
		}
		/// <summary>拡張項目79</summary>
		public string Extend79
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND79]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND79] = value; }
		}
		/// <summary>拡張項目80</summary>
		public string Extend80
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND80]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND80] = value; }
		}
		/// <summary>拡張項目81</summary>
		public string Extend81
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND81]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND81] = value; }
		}
		/// <summary>拡張項目82</summary>
		public string Extend82
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND82]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND82] = value; }
		}
		/// <summary>拡張項目83</summary>
		public string Extend83
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND83]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND83] = value; }
		}
		/// <summary>拡張項目84</summary>
		public string Extend84
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND84]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND84] = value; }
		}
		/// <summary>拡張項目85</summary>
		public string Extend85
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND85]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND85] = value; }
		}
		/// <summary>拡張項目86</summary>
		public string Extend86
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND86]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND86] = value; }
		}
		/// <summary>拡張項目87</summary>
		public string Extend87
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND87]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND87] = value; }
		}
		/// <summary>拡張項目88</summary>
		public string Extend88
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND88]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND88] = value; }
		}
		/// <summary>拡張項目89</summary>
		public string Extend89
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND89]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND89] = value; }
		}
		/// <summary>拡張項目90</summary>
		public string Extend90
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND90]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND90] = value; }
		}
		/// <summary>拡張項目91</summary>
		public string Extend91
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND91]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND91] = value; }
		}
		/// <summary>拡張項目92</summary>
		public string Extend92
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND92]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND92] = value; }
		}
		/// <summary>拡張項目93</summary>
		public string Extend93
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND93]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND93] = value; }
		}
		/// <summary>拡張項目94</summary>
		public string Extend94
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND94]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND94] = value; }
		}
		/// <summary>拡張項目95</summary>
		public string Extend95
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND95]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND95] = value; }
		}
		/// <summary>拡張項目96</summary>
		public string Extend96
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND96]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND96] = value; }
		}
		/// <summary>拡張項目97</summary>
		public string Extend97
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND97]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND97] = value; }
		}
		/// <summary>拡張項目98</summary>
		public string Extend98
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND98]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND98] = value; }
		}
		/// <summary>拡張項目99</summary>
		public string Extend99
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND99]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND99] = value; }
		}
		/// <summary>拡張項目100</summary>
		public string Extend100
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND100]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND100] = value; }
		}
		/// <summary>拡張項目101</summary>
		public string Extend101
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND101]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND101] = value; }
		}
		/// <summary>拡張項目102</summary>
		public string Extend102
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND102]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND102] = value; }
		}
		/// <summary>拡張項目103</summary>
		public string Extend103
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND103]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND103] = value; }
		}
		/// <summary>拡張項目104</summary>
		public string Extend104
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND104]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND104] = value; }
		}
		/// <summary>拡張項目105</summary>
		public string Extend105
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND105]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND105] = value; }
		}
		/// <summary>拡張項目106</summary>
		public string Extend106
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND106]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND106] = value; }
		}
		/// <summary>拡張項目107</summary>
		public string Extend107
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND107]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND107] = value; }
		}
		/// <summary>拡張項目108</summary>
		public string Extend108
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND108]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND108] = value; }
		}
		/// <summary>拡張項目109</summary>
		public string Extend109
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND109]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND109] = value; }
		}
		/// <summary>拡張項目110</summary>
		public string Extend110
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND110]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND110] = value; }
		}
		/// <summary>拡張項目111</summary>
		public string Extend111
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND111]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND111] = value; }
		}
		/// <summary>拡張項目112</summary>
		public string Extend112
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND112]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND112] = value; }
		}
		/// <summary>拡張項目113</summary>
		public string Extend113
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND113]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND113] = value; }
		}
		/// <summary>拡張項目114</summary>
		public string Extend114
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND114]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND114] = value; }
		}
		/// <summary>拡張項目115</summary>
		public string Extend115
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND115]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND115] = value; }
		}
		/// <summary>拡張項目116</summary>
		public string Extend116
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND116]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND116] = value; }
		}
		/// <summary>拡張項目117</summary>
		public string Extend117
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND117]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND117] = value; }
		}
		/// <summary>拡張項目118</summary>
		public string Extend118
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND118]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND118] = value; }
		}
		/// <summary>拡張項目119</summary>
		public string Extend119
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND119]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND119] = value; }
		}
		/// <summary>拡張項目120</summary>
		public string Extend120
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND120]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND120] = value; }
		}
		/// <summary>拡張項目121</summary>
		public string Extend121
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND121]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND121] = value; }
		}
		/// <summary>拡張項目122</summary>
		public string Extend122
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND122]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND122] = value; }
		}
		/// <summary>拡張項目123</summary>
		public string Extend123
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND123]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND123] = value; }
		}
		/// <summary>拡張項目124</summary>
		public string Extend124
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND124]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND124] = value; }
		}
		/// <summary>拡張項目125</summary>
		public string Extend125
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND125]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND125] = value; }
		}
		/// <summary>拡張項目126</summary>
		public string Extend126
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND126]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND126] = value; }
		}
		/// <summary>拡張項目127</summary>
		public string Extend127
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND127]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND127] = value; }
		}
		/// <summary>拡張項目128</summary>
		public string Extend128
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND128]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND128] = value; }
		}
		/// <summary>拡張項目129</summary>
		public string Extend129
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND129]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND129] = value; }
		}
		/// <summary>拡張項目130</summary>
		public string Extend130
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND130]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND130] = value; }
		}
		/// <summary>拡張項目131</summary>
		public string Extend131
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND131]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND131] = value; }
		}
		/// <summary>拡張項目132</summary>
		public string Extend132
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND132]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND132] = value; }
		}
		/// <summary>拡張項目133</summary>
		public string Extend133
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND133]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND133] = value; }
		}
		/// <summary>拡張項目134</summary>
		public string Extend134
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND134]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND134] = value; }
		}
		/// <summary>拡張項目135</summary>
		public string Extend135
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND135]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND135] = value; }
		}
		/// <summary>拡張項目136</summary>
		public string Extend136
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND136]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND136] = value; }
		}
		/// <summary>拡張項目137</summary>
		public string Extend137
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND137]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND137] = value; }
		}
		/// <summary>拡張項目138</summary>
		public string Extend138
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND138]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND138] = value; }
		}
		/// <summary>拡張項目139</summary>
		public string Extend139
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND139]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND139] = value; }
		}
		/// <summary>拡張項目140</summary>
		public string Extend140
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND140]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND140] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTEXTEND_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTEXTEND_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTEXTEND_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
