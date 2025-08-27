SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER PROCEDURE [sp_Wms_Trade_Create]
	 @Trade_Code	Varchar(15)
    ,@Trade_Cls	Varchar(1)
    ,@Trade_Name	Varchar(70)
    ,@Trade_Abbr	Varchar(20)
    ,@Contact_Person	Varchar(50)
    ,@Address1	Varchar(100)
    ,@Address2	Varchar(100)
    ,@City	Varchar(50)
    ,@Country	Varchar(50)
    ,@Country_Cls	Varchar(1)
    ,@Epte_Cls	Varchar(1)
    ,@Region_Cls	Varchar(2)
    ,@Postal_Code	Varchar(10)
    ,@Telephone	Varchar(50)
    ,@Fax	Varchar(50)
    ,@Closing_Day	Varchar(2)
    ,@Pay_Day	Varchar(2)
    ,@InvoicePay_Days	Numeric(5,0)
    ,@Affiliate_Cls	Varchar(1)
    ,@Insurance_Cls	Varchar(2)
    ,@NPWP_No	Varchar(20)
    ,@NPWP_Name	Varchar(100)
    ,@NPWP_Address	Varchar(200)
    ,@NPWP_City	Varchar(50)
    ,@NPPKP_No	Varchar(20)
    ,@Invoice_To	Varchar(15)
    ,@PO_Cls	Varchar(1)
    ,@Price_Condition	Varchar(2)
    ,@POPayment_Day	Numeric(5,0)
    ,@POPayment_Terms	Varchar(2)
    ,@Transportation_Cls	Varchar(2)
    ,@POCaseMark1	Varchar(25)
    ,@POCaseMark2	Varchar(25)
    ,@POCaseMark3	Varchar(25)
    ,@POCaseMark4	Varchar(25)
    ,@POCaseMark5	Varchar(25)
    ,@POMarking1	Varchar(25)
    ,@POMarking2	Varchar(25)
    ,@POMarking3	Varchar(25)
    ,@POMarking4	Varchar(25)
    ,@POMarking5	Varchar(25)
    ,@POMarking6	Varchar(25)
    ,@Subcon_WH_Code	Varchar(15)
    ,@NG_Cls	Varchar(1)
    ,@SAP_Code	Varchar(15)
    ,@Type_BC	Varchar(15)
    ,@No_Izin	Varchar(50)
    ,@CODE_KPPBC	Varchar(6)
    ,@NoIzin_Date	dateTime
    ,@NITKU	Varchar(30)
	,@RegisterBy varchar(25)
as
begin
	if exists (select 1 from Trade_Master where Trade_Code = @Trade_Code)
	begin
		raiserror('Trade Code Already Exists',16,1)
		return;
	end

	 

	insert into Trade_Master(
	 Trade_Code
    ,Trade_Cls
    ,Trade_Name
    ,Trade_Abbr
    ,Contact_Person
    ,Address1
    ,Address2
    ,City
    ,Country
    ,Country_Cls
    ,Epte_Cls
    ,Region_Cls
    ,Postal_Code
    ,Telephone
    ,Fax
    ,Closing_Day
    ,Pay_Day
    ,InvoicePay_Days
    ,Affiliate_Cls
    ,Insurance_Cls
    ,NPWP_No
    ,NPWP_Name
    ,NPWP_Address
    ,NPWP_City
    ,NPPKP_No
    ,Invoice_To
    ,PO_Cls
    ,Price_Condition
    ,POPayment_Day
    ,POPayment_Terms
    ,Transportation_Cls
    ,POCaseMark1
    ,POCaseMark2
    ,POCaseMark3
    ,POCaseMark4
    ,POCaseMark5
    ,POMarking1
    ,POMarking2
    ,POMarking3
    ,POMarking4
    ,POMarking5
    ,POMarking6
    ,Subcon_WH_Code
    ,Last_Update
    ,Last_User
    ,Register_Date
    ,NG_Cls
    ,SAP_Code
    ,Type_BC
    ,No_Izin
    ,CODE_KPPBC
    ,NoIzin_Date
    ,NITKU
)
	values ( @Trade_Code
			,@Trade_Cls
			,@Trade_Name
			,@Trade_Abbr
			,@Contact_Person
			,@Address1
			,@Address2
			,@City
			,@Country
			,@Country_Cls
			,@Epte_Cls
			,@Region_Cls
			,@Postal_Code
			,@Telephone
			,@Fax
			,@Closing_Day
			,@Pay_Day
			,@InvoicePay_Days
			,@Affiliate_Cls
			,@Insurance_Cls
			,@NPWP_No
			,@NPWP_Name
			,@NPWP_Address
			,@NPWP_City
			,@NPPKP_No
			,@Invoice_To
			,@PO_Cls
			,@Price_Condition
			,@POPayment_Day
			,@POPayment_Terms
			,@Transportation_Cls
			,@POCaseMark1
			,@POCaseMark2
			,@POCaseMark3
			,@POCaseMark4
			,@POCaseMark5
			,@POMarking1
			,@POMarking2
			,@POMarking3
			,@POMarking4
			,@POMarking5
			,@POMarking6
			,@Subcon_WH_Code
			,GetDate()
			,@RegisterBy
			,GetDate()
			,@NG_Cls
			,@SAP_Code
			,@Type_BC
			,@No_Izin
			,@CODE_KPPBC
			,@NoIzin_Date
			,@NITKU
 )
end
GO
