/*
=========================================================================================================
  Module      : モール設定 w2_MallCooperationSetting (MallCooperationSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.Common.Sql;

namespace w2.Commerce.MallBatch.MailOrder
{
    /// <summary>
    /// DBのw2_MallCooperationSettingに対応
    /// </summary>
    public class MallCooperationSetting
    {
        /// <summary>
        /// モール設定を取得・構築
        /// </summary>
        /// <param name="shop_id"></param>
        /// <param name="mall_id"></param>
        public MallCooperationSetting(string shop_id,string mall_id)
        {
			DataView dvMallCooperationSettings = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "GetMallCooperationSetting"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, shop_id);
				htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, mall_id);

				dvMallCooperationSettings = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			this.shop_id = shop_id;
			this.mall_id = mall_id;
			
			if (dvMallCooperationSettings.Count != 0)
			{
                shop_id = (string)dvMallCooperationSettings[0]["shop_id"];
                mall_id = (string)dvMallCooperationSettings[0]["mall_id"];
                mall_kbn = (string)dvMallCooperationSettings[0]["mall_kbn"];
                mall_name = (string)dvMallCooperationSettings[0]["mall_name"];
                tgt_mail_addr = (string)dvMallCooperationSettings[0]["tgt_mail_addr"];
                pop_server = (string)dvMallCooperationSettings[0]["pop_server"];
                pop_port = (int)dvMallCooperationSettings[0]["pop_port"];
                pop_user_name = (string)dvMallCooperationSettings[0]["pop_user_name"];
                pop_password = (string)dvMallCooperationSettings[0]["pop_password"];
                pop_apop_flg = (string)dvMallCooperationSettings[0]["pop_apop_flg"];
                ftp_host = (string)dvMallCooperationSettings[0]["ftp_host"];
                ftp_user_name = (string)dvMallCooperationSettings[0]["ftp_user_name"];
                ftp_password = (string)dvMallCooperationSettings[0]["ftp_password"];
                ftp_upload_dir = (string)dvMallCooperationSettings[0]["ftp_upload_dir"];
                valid_flg = (string)dvMallCooperationSettings[0]["valid_flg"];
                del_flg = (string)dvMallCooperationSettings[0]["del_flg"];
                if (dvMallCooperationSettings[0]["last_product_log_no"] != DBNull.Value)
                {
                    last_product_log_no = (string)dvMallCooperationSettings[0]["last_product_log_no"];
                }
                if (dvMallCooperationSettings[0]["last_productvariation_log_no"] != DBNull.Value)
                {
                    last_productvariation_log_no = (string)dvMallCooperationSettings[0]["last_productvariation_log_no"];
                }
                if (dvMallCooperationSettings[0]["last_productstock_log_no"] != DBNull.Value)
                {
                    last_productstock_log_no = (string)dvMallCooperationSettings[0]["last_productstock_log_no"];
                }
                date_created = (DateTime)dvMallCooperationSettings[0]["date_created"];
                date_changed = (DateTime)dvMallCooperationSettings[0]["date_changed"];
                last_changed = (string)dvMallCooperationSettings[0]["last_changed"];
            }
        }

        public bool isExist;
        public string shop_id;
        public string mall_id;
        public string mall_kbn;
        public string mall_name;
        public string tgt_mail_addr;
        public string pop_server;
        public int pop_port;
        public string pop_user_name;
        public string pop_password;
        public string pop_apop_flg;
        public string ftp_host;
        public string ftp_user_name;
        public string ftp_password;
        public string ftp_upload_dir;
        public string valid_flg;
        public string del_flg;
        public string last_product_log_no;
        public string last_productvariation_log_no;
        public string last_productstock_log_no;
        public DateTime date_created;
        public DateTime date_changed;
        public string last_changed;
    }
}
