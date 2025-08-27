SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bea_Cukai_Pos_Tarif](
	[ID] [int] NOT NULL,
	[JENIS_TARIF_BM] [varchar](255) NULL,
	[JENIS_TARIF_CUKAI] [varchar](255) NULL,
	[KODE_SATUAN_BM] [varchar](255) NULL,
	[KODE_SATUAN_CUKAI] [varchar](255) NULL,
	[NOMOR_HS] [varchar](255) NULL,
	[SERI_HS] [int] NULL,
	[TARIF_BM] [decimal](20, 2) NULL,
	[TARIF_CUKAI] [decimal](20, 2) NULL,
	[TARIF_PPH] [decimal](20, 2) NULL,
	[TARIF_PPN] [decimal](20, 2) NULL,
	[TARIF_PPNBM] [decimal](20, 2) NULL
) ON [PRIMARY]
GO
