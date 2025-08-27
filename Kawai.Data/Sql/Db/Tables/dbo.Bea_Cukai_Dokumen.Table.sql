SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bea_Cukai_Dokumen](
	[ID] [int] NULL,
	[Kode_Dokumen] [varchar](5) NOT NULL,
	[Tipe_Dokumen] [varchar](2) NULL,
	[Uraian_Dokumen] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
