SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Pengeluaran_barang_perdokumen](
	[JenisDokument] [varchar](15) NULL,
	[NoPabean] [varchar](30) NULL,
	[TanggalPabean] [char](12) NULL,
	[NoSuratJalan] [varchar](25) NULL,
	[TanggalSuratJalan] [char](12) NULL,
	[Penerima] [char](70) NULL,
	[KodeBarang] [varchar](30) NULL,
	[NamaBarang] [varchar](75) NULL,
	[Satuan] [varchar](25) NULL,
	[Qty] [numeric](18, 5) NOT NULL,
	[Childsupply_date] [datetime] NULL
) ON [PRIMARY]
GO
