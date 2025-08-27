SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Mutasi_barang_jadi](
	[KodeBarang] [char](25) NOT NULL,
	[NamaBarang] [char](75) NULL,
	[Satuan] [char](25) NULL,
	[SaldoAwal] [numeric](38, 5) NULL,
	[Penerimaan] [numeric](38, 5) NULL,
	[Pengeluaran] [numeric](38, 5) NULL,
	[Penyesuaian] [numeric](38, 5) NULL,
	[SaldoAkhir] [numeric](38, 5) NULL,
	[StockOpname] [numeric](38, 5) NULL,
	[Selisih] [numeric](38, 5) NULL,
	[Keterangan] [varchar](1) NOT NULL
) ON [PRIMARY]
GO
