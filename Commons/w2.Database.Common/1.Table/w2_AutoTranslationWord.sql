﻿if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AutoTranslationWord]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AutoTranslationWord]
GO
/*
=========================================================================================================
  Module      : 自動翻訳ワード管理 (w2_AutoTranslationWord.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AutoTranslationWord] (
	[word_hash_key] [nvarchar] (64) NOT NULL DEFAULT (N''),
	[language_code] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[word_before] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[word_after] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[clear_flg] [nvarchar] (1) NOT NULL DEFAULT (1),
	[date_used] [datetime] NOT NULL DEFAULT (getdate()),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AutoTranslationWord] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AutoTranslationWord] PRIMARY KEY  CLUSTERED
	(
		[word_hash_key],
		[language_code]
	) ON [PRIMARY]
GO