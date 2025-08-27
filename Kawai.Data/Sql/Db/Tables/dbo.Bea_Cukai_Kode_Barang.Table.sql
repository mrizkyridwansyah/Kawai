SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bea_Cukai_Kode_Barang](
	[ID] [int] NOT NULL,
	[BARANG_KE] [int] NULL,
	[KODE_BARANG] [varchar](255) NULL,
	[MERK] [varchar](255) NULL,
	[NOHS] [varchar](255) NULL,
	[SERI] [varchar](255) NULL,
	[SPESIFIKASI_LAIN] [varchar](255) NULL,
	[TIPE] [varchar](255) NULL,
	[URAIAN_BARANG] [varchar](255) NULL
) ON [PRIMARY]
GO
