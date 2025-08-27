SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bea_Cukai_TPB_Dokumen](
	[ID] [int] NULL,
	[Flag_Url_Dokumen] [char](1) NULL,
	[Kode_Jenis_Dokumen] [varchar](3) NULL,
	[Nomor_Dokumen] [varchar](255) NULL,
	[Seri_Dokumen] [int] NULL,
	[Tanggal_Dokumen] [datetime] NULL,
	[Tipe_Dokumen] [varchar](2) NULL,
	[Url_Dokumen] [varchar](255) NULL,
	[ID_Header] [int] NULL,
	[No_Pengajuan] [varchar](50) NULL,
	[ID_Dokumen] [int] NULL
) ON [PRIMARY]
GO
