if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AccessLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AccessLog]
GO
/*
=========================================================================================================
  Module      : �A�N�Z�X���O�e�[�u��(w2_AccessLog.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AccessLog] (
	[log_no] [bigint] IDENTITY (1, 1) NOT NULL,
	[access_date] [nvarchar] (20) NOT NULL DEFAULT (''),
	[access_time] [nvarchar] (20) NOT NULL DEFAULT (''),
	[client_ip] [nvarchar] (20) NOT NULL DEFAULT (''),
	[server_name] [nvarchar] (20) NOT NULL DEFAULT (''),
	[server_ip] [nvarchar] (20) NOT NULL DEFAULT (''),
	[server_port] [int] NOT NULL DEFAULT (''),
	[protocol_status] [int] NOT NULL DEFAULT (''),
	[user_agent] [nvarchar] (512) NOT NULL DEFAULT (''),
	[url_domain] [nvarchar] (50) NOT NULL DEFAULT (''),
	[url_page] [nvarchar] (1000) NOT NULL DEFAULT (''),
	[url_param] [nvarchar] (1000) NOT NULL DEFAULT (''),
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (''),
	[account_id] [nvarchar] (30) NOT NULL DEFAULT (''),
	[access_user_id] [nvarchar] (255) NOT NULL DEFAULT (''),
	[session_id] [nvarchar] (255) NOT NULL DEFAULT (''),
	[real_user_id] [nvarchar] (50) NOT NULL DEFAULT (''),
	[access_interval] [int] DEFAULT (0),
	[first_login_flg] [nvarchar] (2) NOT NULL DEFAULT ('0'),
	[referrer_domain] [nvarchar] (50) NOT NULL DEFAULT (''),
	[referrer_page] [nvarchar] (1000) NOT NULL DEFAULT (''),
	[referrer_param] [nvarchar] (1000) NOT NULL DEFAULT (''),
	[search_engine] [nvarchar] (30),
	[search_words] [nvarchar] (1000),
	[action_kbn] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s1] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s2] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s3] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s4] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s5] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s6] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s7] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s8] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s9] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s10] [nvarchar] (20) NOT NULL DEFAULT (''),
	[s11] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s12] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s13] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s14] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s15] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s16] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s17] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s18] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s19] [nvarchar] (50) NOT NULL DEFAULT (''),
	[s20] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p1] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p2] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p3] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p4] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p5] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p6] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p7] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p8] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p9] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p10] [nvarchar] (20) NOT NULL DEFAULT (''),
	[p11] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p12] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p13] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p14] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p15] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p16] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p17] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p18] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p19] [nvarchar] (50) NOT NULL DEFAULT (''),
	[p20] [nvarchar] (50) NOT NULL DEFAULT (''),
	[user_agent_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'PC')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AccessLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AccessLog] PRIMARY KEY  NONCLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_AccessLog_1] ON [dbo].[w2_AccessLog]([access_date], [access_time]) ON [PRIMARY]
GO

/*
-- 5.11
varchar�^��nvarchar�ɕύX���܂����BVerup�ł�Truncate Table => Create Table�ōX�V
*/