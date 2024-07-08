if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AccessLogMobile]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AccessLogMobile]
GO
/*
=========================================================================================================
  Module      : モバイルアクセスログテーブル(w2_AccessLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AccessLogMobile] (
	[log_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[access_date] [varchar] (20) NOT NULL DEFAULT (''),
	[access_time] [varchar] (20) NOT NULL DEFAULT (''),
	[client_ip] [varchar] (20) NOT NULL DEFAULT (''),
	[server_name] [varchar] (20) NOT NULL DEFAULT (''),
	[server_ip] [varchar] (20) NOT NULL DEFAULT (''),
	[server_port] [int] NOT NULL DEFAULT (''),
	[protocol_status] [int] NOT NULL DEFAULT (''),
	[mobile_career_code] [varchar] (10) NOT NULL DEFAULT (''),
	[mobile_model_name] [varchar] (50) NOT NULL DEFAULT (''),
	[mobile_model_code] [varchar] (50) NOT NULL DEFAULT (''),
	[user_agent] [varchar] (512) NOT NULL DEFAULT (''),
	[url_domain] [varchar] (50) NOT NULL DEFAULT (''),
	[url_page] [varchar] (100) NOT NULL DEFAULT (''),
	[url_param] [varchar] (1000) NOT NULL DEFAULT (''),
	[dept_id] [varchar] (30) NOT NULL DEFAULT (''),
	[session_id] [varchar] (255) NOT NULL DEFAULT (''),
	[real_user_id] [varchar] (50) NOT NULL DEFAULT (''),
	[referrer_domain] [varchar] (50) NOT NULL DEFAULT (''),
	[referrer_page] [varchar] (100) NOT NULL DEFAULT (''),
	[referrer_param] [varchar] (1000) NOT NULL DEFAULT (''),
	[search_engine] [varchar] (30) NOT NULL DEFAULT (''),
	[search_words] [varchar] (1000) NOT NULL DEFAULT (''),
	[action_kbn] [varchar] (20) NOT NULL DEFAULT (''),
	[s1] [varchar] (20) NOT NULL DEFAULT (''),
	[s2] [varchar] (20) NOT NULL DEFAULT (''),
	[s3] [varchar] (20) NOT NULL DEFAULT (''),
	[s4] [varchar] (20) NOT NULL DEFAULT (''),
	[s5] [varchar] (20) NOT NULL DEFAULT (''),
	[s6] [varchar] (20) NOT NULL DEFAULT (''),
	[s7] [varchar] (20) NOT NULL DEFAULT (''),
	[s8] [varchar] (20) NOT NULL DEFAULT (''),
	[s9] [varchar] (20) NOT NULL DEFAULT (''),
	[s10] [varchar] (20) NOT NULL DEFAULT (''),
	[s11] [varchar] (50) NOT NULL DEFAULT (''),
	[s12] [varchar] (50) NOT NULL DEFAULT (''),
	[s13] [varchar] (50) NOT NULL DEFAULT (''),
	[s14] [varchar] (50) NOT NULL DEFAULT (''),
	[s15] [varchar] (50) NOT NULL DEFAULT (''),
	[s16] [varchar] (50) NOT NULL DEFAULT (''),
	[s17] [varchar] (50) NOT NULL DEFAULT (''),
	[s18] [varchar] (50) NOT NULL DEFAULT (''),
	[s19] [varchar] (50) NOT NULL DEFAULT (''),
	[s20] [varchar] (50) NOT NULL DEFAULT (''),
	[p1] [varchar] (20) NOT NULL DEFAULT (''),
	[p2] [varchar] (20) NOT NULL DEFAULT (''),
	[p3] [varchar] (20) NOT NULL DEFAULT (''),
	[p4] [varchar] (20) NOT NULL DEFAULT (''),
	[p5] [varchar] (20) NOT NULL DEFAULT (''),
	[p6] [varchar] (20) NOT NULL DEFAULT (''),
	[p7] [varchar] (20) NOT NULL DEFAULT (''),
	[p8] [varchar] (20) NOT NULL DEFAULT (''),
	[p9] [varchar] (20) NOT NULL DEFAULT (''),
	[p10] [varchar] (20) NOT NULL DEFAULT (''),
	[p11] [varchar] (50) NOT NULL DEFAULT (''),
	[p12] [varchar] (50) NOT NULL DEFAULT (''),
	[p13] [varchar] (50) NOT NULL DEFAULT (''),
	[p14] [varchar] (50) NOT NULL DEFAULT (''),
	[p15] [varchar] (50) NOT NULL DEFAULT (''),
	[p16] [varchar] (50) NOT NULL DEFAULT (''),
	[p17] [varchar] (50) NOT NULL DEFAULT (''),
	[p18] [varchar] (50) NOT NULL DEFAULT (''),
	[p19] [varchar] (50) NOT NULL DEFAULT (''),
	[p20] [varchar] (50) NOT NULL DEFAULT ('')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AccessLogMobile] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AccessLogMobile] PRIMARY KEY  NONCLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_AccessLogMobile_1] ON [dbo].[w2_AccessLogMobile]([access_date], [access_time]) ON [PRIMARY]
GO
