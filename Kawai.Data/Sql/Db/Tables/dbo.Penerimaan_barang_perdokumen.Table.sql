SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Penerimaan_barang_perdokumen](
	[JenisDokumen] [varchar](15) NULL,
	[NomorPabean] [char](30) NULL,
	[TanggalPabean] [char](12) NULL,
	[NomorPenerimaan] [varchar](25) NULL,
	[TanggalPenerimaan] [char](12) NULL,
	[TanggalFilter] [datetime] NOT NULL,
	[Pemasok] [char](70) NULL,
	[KodeBarang] [char](25) NOT NULL,
	[NamaBarang] [char](159) NULL,
	[Satuan] [char](25) NULL,
	[Jumlah] [numeric](18, 5) NULL,
	[MataUang] [char](25) NULL,
	[Harga] [numeric](18, 5) NULL,
	[TotalHarga] [numeric](37, 10) NULL
) ON [PRIMARY]
GO
