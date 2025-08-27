SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [global_param](
	[id_gp] [int] IDENTITY(1,1) NOT NULL,
	[name_gp] [varchar](50) NOT NULL,
	[value_gp] [varchar](320) NULL,
	[info_gp] [varchar](40) NOT NULL,
	[alias_gp] [varchar](64) NOT NULL,
	[type_gp] [varchar](12) NOT NULL,
	[disabled_gp] [tinyint] NOT NULL,
	[indeks_gp] [tinyint] NOT NULL
) ON [PRIMARY]
GO
