SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bea_Cukai_TPB_Kemasan](
	[ID] [int] NULL,
	[JUMLAH_KEMASAN] [numeric](18, 2) NULL,
	[KESESUAIAN_DOKUMEN] [varchar](255) NULL,
	[KETERANGAN] [varchar](255) NULL,
	[KODE_JENIS_KEMASAN] [varchar](255) NULL,
	[MERK_KEMASAN] [varchar](255) NULL,
	[NIP_GATE_IN] [varchar](255) NULL,
	[NIP_GATE_OUT] [varchar](255) NULL,
	[NO_POLISI] [varchar](255) NULL,
	[NOMOR_SEGEL] [varchar](255) NULL,
	[SERI_KEMASAN] [int] NULL,
	[WAKTU_GATE_IN] [datetime] NULL,
	[WAKTU_GATE_OUT] [datetime] NULL,
	[ID_HEADER] [int] NULL,
	[No_Pengajuan] [varchar](50) NULL
) ON [PRIMARY]
GO
