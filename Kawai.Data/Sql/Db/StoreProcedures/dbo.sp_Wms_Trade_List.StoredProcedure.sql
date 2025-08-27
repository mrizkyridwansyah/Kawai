SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [sp_Wms_Trade_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = ''
as
begin
	declare @sqlSort varchar(max) = ''
	declare @offset int

    -- Hitung offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	if isnull(@Sort, '') <> ''
	begin
		set @sqlSort = 'order by ' + @Sort
	end 
	else 
	begin
		set @sqlSort = 'order by a.Trade_Code'
	end 

	declare @TotalRows int =
	(
		select 
				 COUNT(1)

				from Trade_Master A
				Left JOIN vw_Cls B ON A.Trade_Cls = B.ClsCode and B.TypeData = 'Trade_Cls'
				Left JOIN vw_Cls C ON A.Country_Cls = C.ClsCode and C.TypeData = 'Country_Cls'
				Left JOIN vw_Cls D ON A.PO_Cls = D.ClsCode and D.TypeData = 'PO_Cls'
				Left JOIN vw_Cls E ON A.Price_Condition = E.ClsCode and E.TypeData = 'PriceCondition_Cls'
				Left JOIN vw_Cls F ON A.Transportation_Cls = F.ClsCode and F.TypeData = 'Transportation_Cls'
				Left JOIN vw_Cls G ON A.Affiliate_Cls = G.ClsCode and G.TypeData = 'Affiliate_Cls'
				Left JOIN vw_Cls H ON A.Insurance_Cls = H.ClsCode and H.TypeData = 'Insurance_Cls'
				Left JOIN vw_Cls I ON A.Type_BC = I.ClsCode and I.TypeData = 'BCType_Cls'
				LEFT JOIN WareHouse_Master J ON J.WH_Code = A.Subcon_WH_Code
				Left JOIN vw_Cls K ON A.Region_Cls = K.ClsCode and K.TypeData = 'Region_Cls'
				Left JOIN vw_Cls L ON A.Epte_Cls = L.ClsCode and L.TypeData = 'Epte_Cls'
				Left JOIN vw_Cls M ON A.NG_Cls = M.ClsCode and M.TypeData = 'NG_Cls'
				Left JOIN vw_Cls N ON A.POPayment_Terms = N.ClsCode and N.TypeData = 'PaymentTerm_Cls'
		where 1=1
		and (A.Trade_Code like '%'+@Keyword+'%' or A.Trade_Name like '%'+@Keyword+'%' or B.Description  like '%'+@Keyword+'%')
	)

	declare @sql varchar(max) = 
	'
	select 
				 A.Trade_Code
				,A.Trade_Cls
				,B.Description as Trade_Cls_Descs
				,A.Trade_Name
				,A.Trade_Abbr
				,A.Contact_Person
				,A.Address1
				,A.Address2
				,A.City
				,A.Country
				,A.Country_Cls
				,C.Description as Country_Cls_Descs
				,A.Epte_Cls
				,L.Description as Epte_Cls_Descs
				,A.Region_Cls
				,K.Description as Region_Cls_Descs
				,A.Postal_Code
				,A.Telephone
				,A.Fax
				,A.Closing_Day
				,A.Pay_Day
				,A.InvoicePay_Days
				,A.Affiliate_Cls
				,G.Description as  Affiliate_Cls_Descs
				,A.Insurance_Cls
				,H.Description as  Insurance_Cls_Descs
				,A.NPWP_No
				,A.NPWP_Name
				,A.NPWP_Address
				,A.NPWP_City
				,A.NPPKP_No
				,A.Invoice_To
				,A.PO_Cls
				,D.Description as  PO_Cls_Descs
				,A.Price_Condition
				,E.Description as  Price_Condition_Descs
				,A.POPayment_Day
				,A.POPayment_Terms
				,N.Description as  POPayment_Terms_Descs
				,A.Transportation_Cls
				,F.Description as Transportation_Cls_Descs
				,A.POCaseMark1
				,A.POCaseMark2
				,A.POCaseMark3
				,A.POCaseMark4
				,A.POCaseMark5
				,A.POMarking1
				,A.POMarking2
				,A.POMarking3
				,A.POMarking4
				,A.POMarking5
				,A.POMarking6
				,A.Subcon_WH_Code
				,J.WH_Name Subcon_WH_Descs
				,A.Last_Update
				,A.Last_User
				,A.Register_Date
				,A.NG_Cls
				,M.Description as NG_Cls_Descs 
				,A.SAP_Code
				,A.Type_BC
				,I.Description as Type_BC_Descs
				,A.No_Izin
				,A.CODE_KPPBC
				,A.NoIzin_Date
				,A.NITKU
				,'''+cast(@TotalRows as varchar)+''' as TotalRows
				from Trade_Master A
				Left JOIN vw_Cls B ON A.Trade_Cls = B.ClsCode and B.TypeData = ''Trade_Cls''
				Left JOIN vw_Cls C ON A.Country_Cls = C.ClsCode and C.TypeData = ''Country_Cls''
				Left JOIN vw_Cls D ON A.PO_Cls = D.ClsCode and D.TypeData = ''PO_Cls''
				Left JOIN vw_Cls E ON A.Price_Condition = E.ClsCode and E.TypeData = ''PriceCondition_Cls''
				Left JOIN vw_Cls F ON A.Transportation_Cls = F.ClsCode and F.TypeData = ''Transportation_Cls''
				Left JOIN vw_Cls G ON A.Affiliate_Cls = G.ClsCode and G.TypeData = ''Affiliate_Cls''
				Left JOIN vw_Cls H ON A.Insurance_Cls = H.ClsCode and H.TypeData = ''Insurance_Cls''
				Left JOIN vw_Cls I ON A.Type_BC = I.ClsCode and I.TypeData = ''BCType_Cls''
				LEFT JOIN WareHouse_Master J ON J.WH_Code = A.Subcon_WH_Code
				Left JOIN vw_Cls K ON A.Region_Cls = K.ClsCode and K.TypeData = ''Region_Cls''
				Left JOIN vw_Cls L ON A.Epte_Cls = L.ClsCode and L.TypeData = ''Epte_Cls''
				Left JOIN vw_Cls M ON A.NG_Cls = M.ClsCode and M.TypeData = ''NG_Cls''
				Left JOIN vw_Cls N ON A.POPayment_Terms = N.ClsCode and N.TypeData = ''PaymentTerm_Cls''
	 	where 1=1
		and (A.Trade_Code like ''%'+@Keyword+'%'' or A.Trade_Name like ''%'+@Keyword+'%'' or B.Description  like ''%'+@Keyword+'%'')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
