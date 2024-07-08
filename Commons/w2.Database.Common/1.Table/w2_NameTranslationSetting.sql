if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_NameTranslationSetting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_NameTranslationSetting]
GO
/*
=========================================================================================================
  Module      : Ц╝П╠Ц|ЦєР▌ТшГ}ГXГ^ (w2_NameTranslationSetting.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_NameTranslationSetting] (
	[data_kbn] [nvarchar] (30) NOT NULL,
	[translation_target_column] [nvarchar] (50) NOT NULL,
	[master_id1] [nvarchar] (100) NOT NULL,
	[master_id2] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[master_id3] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[language_code] [nvarchar] (4) NOT NULL DEFAULT (N''),
	[language_locale_id] [nvarchar] (7) NOT NULL DEFAULT (N''),
	[after_translational_name] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[display_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate())
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_NameTranslationSetting] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_NameTranslationSetting] PRIMARY KEY  CLUSTERED
	(
		[data_kbn],
		[translation_target_column],
		[master_id1],
		[master_id2],
		[master_id3],
		[language_code],
		[language_locale_id]
	) ON [PRIMARY]
GO
