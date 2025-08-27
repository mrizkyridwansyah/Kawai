SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Barang_wip](
	[KodeBarang] [char](25) NOT NULL,
	[NamaBarang] [char](75) NULL,
	[Satuan] [char](25) NULL,
	[Jumlah] [numeric](18, 5) NULL,
	[Keterangan] [varchar](50) NULL
) ON [PRIMARY]
GO
