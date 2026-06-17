using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Data;

public static class StoreProcedures
{
    public static class Address
    {

        public const string Create = "sp_Wms_Address_Create";
        public const string Update = "sp_Wms_Address_Update";
        public const string Delete = "sp_Wms_Address_Delete";

        public const string Capture = "sp_Wms_Address_Capture";
        public const string GetDetail = "sp_Wms_Address_GetDetail";
        public const string List = "sp_Wms_Address_List";

        public const string DDL = "sp_Wms_Address_DDL";
        public const string DDLByStock = "sp_Wms_Address_DDLByStock";
        public const string PrivilegesDDL = "sp_Wms_AddressPrivileges_DDL";
        public const string PrivilegesDDLByStock = "sp_Wms_AddressPrivileges_DDLByStock";
    }

    public static class Area
    {

        public const string Create = "sp_Wms_Area_Create";
        public const string Update = "sp_Wms_Area_Update";
        public const string Delete = "sp_Wms_Area_Delete";

        public const string Capture = "sp_Wms_Area_Capture";
        public const string GetDetail = "sp_Wms_Area_GetDetail";
        public const string List = "sp_Wms_Area_List";

        public const string DDL = "sp_Wms_Area_DDL";
        public const string DDLByStock = "sp_Wms_Area_DDLByStock";
        public const string PrivilegesDDL = "sp_Wms_AreaPrivileges_DDL";
        public const string PrivilegesDDLByStock = "sp_Wms_AreaPrivileges_DDLByStock";
    }

    public static class Andon
    {
        public const string FilterDDL = "sp_Wms_Andon_Filter_DDL";
    }

    public static class AndonWominRequest
    {
        public const string GetList = "sp_Wms_Andon_WominRequest_GetList";
    }

    public static class BomWS
    {
        public const string Capture = "sp_Wms_BOMWorkStation_Capture";
        public const string Header = "sp_Wms_BOMWorkStation_Header";
        public const string GetHeader = "sp_Wms_BOMWorkStation_GetHeader";
        public const string GetDetail = "sp_Wms_BOMWorkStation_GetDetail";
        public const string GetQty = "sp_Wms_BOMWorkStation_GetQty";
        public const string List = "sp_Wms_BOMWorkStation_List";

        public const string DetailInsert = "sp_Wms_BOMWorkStation_detail_ins";
        public const string DetailDelete = "sp_Wms_BOMWorkStation_DetailDelete";
        public const string CopyData = "sp_Wms_BOMWorkStation_CopyData";

        public const string ItemModelClsDDL = "sp_Wms_BOMWorkStation_ItemModelClsDDL";
        public const string ModelClsDDL = "sp_Wms_BOMWorkStation_ModelClsDDL";
    }

    public static class Classification
    {
        public const string Create = "sp_Wms_Classification_Create";
        public const string Update = "sp_Wms_Classification_Update";
        public const string Delete = "sp_Wms_Classification_Delete";

        public const string Capture = "sp_Wms_Classification_Capture";
        public const string GetDetail = "sp_Wms_Classification_GetDetail";
        public const string ListTab = "sp_Wms_Classification_ListTab";
        public const string TableListDetail = "sp_Wms_Classification_TableListDetail";
    }

    public static class Cls
    {
        public const string DDL = "sp_Wms_Cls_DDL";
    }

    public static class DeliveryNote
    {
        public const string Detail = "sp_Wms_DeliveryNote_Detail";
        public const string List = "sp_Wms_DeliveryNote_List";
        public const string ListDetail = "sp_Wms_DeliveryNote_ListDetail";
    }

    public static class DeliveryPlace
    {

        public const string Create = "sp_Wms_DeliveryPlace_Create";
        public const string Update = "sp_Wms_DeliveryPlace_Update";
        public const string Delete = "sp_Wms_DeliveryPlace_Delete";

        public const string Capture = "sp_Wms_DeliveryPlace_Capture";
        public const string GetDetail = "sp_Wms_DeliveryPlace_GetDetail";
        public const string List = "sp_Wms_DeliveryPlace_List";

        public const string DDL = "sp_Wms_DeliveryPlace_DDL";
    }

    public static class Expired
    {
        public const string List = "sp_Wms_Expired_List";
    }

    public static class Factory
    {
        public const string DDL = "sp_Wms_Factory_DDL";
        public const string PrivilegesDDL = "sp_Wms_FactoryPrivileges_DDL";
    }

    public static class HS
    {
        public const string DDL = "sp_Wms_HS_DDL";
    }

    public static class Import
    {
        public const string Detail = "sp_Wms_ImportHistory_Detail";
        public const string List = "sp_Wms_ImportHistory_List";
        public const string Save = "sp_Wms_ImportHistory_Save";
    }

    public static class InventoryClosing
    {
        public const string Closing = "sp_Wms_Closing";
        public const string GetLastClosing = "sp_Wms_InventoryClosing_GetLastClosing";
    }

    public static class InventoryReport
    {
        public const string List = "sp_Wms_InventoryReport_GetList";
    }

    public static class ItemPackingSupplier
    {
        public const string Create = "sp_Wms_ItemPackingSupplier_Create";
        public const string Update = "sp_Wms_ItemPackingSupplier_Update";
        public const string Delete = "sp_Wms_ItemPackingSupplier_Delete";

        public const string Capture = "sp_Wms_ItemPackingSupplier_Capture";
        public const string GetDetail = "sp_Wms_ItemPackingSupplier_GetDetail";
        public const string List = "sp_Wms_ItemPackingSupplier_List";
    }

    public static class Item
    {
        public const string Create = "sp_Wms_Item_Create";
        public const string Update = "sp_Wms_Item_Update";
        public const string Delete = "sp_Wms_Item_Delete";

        public const string Capture = "sp_Wms_Item_Capture";
        public const string GetDetail = "sp_Wms_Item_GetDetail";
        public const string List = "sp_Wms_Item_List";

        public const string DDL = "sp_Wms_Item_DDL";
        public const string DDLByStock = "sp_Wms_Item_DDLByStock";
        public const string WarehouseDDL = "sp_Wms_ItemWarehouse_DDL";
    }

    public static class ManufactureLine
    {
        public const string LineDDL = "sp_Wms_ManufactureLine_LineDDL";
        public const string ManufactureDDL = "sp_Wms_ManufactureLine_ManufactureDDL";
    }

    public static class NGClaimMaterial
    {
        public const string Create = "sp_Wms_NGClaimMaterial_Create";
        public const string Update = "sp_Wms_NGClaimMaterial_Update";
        public const string Delete = "sp_Wms_NGClaimMaterial_Delete";
        public const string PrintLabel = "sp_Wms_NGClaimMaterial_PrintLabel";
        public const string Approve = "sp_Wms_NGClaimMaterial_Approve";
        public const string GenerateCode = "sp_Wms_NGClaimMaterial_GenerateCode";

        public const string DataHeader = "sp_Wms_NGClaimMaterial_DataHeader";
        public const string List = "sp_Wms_NGClaimMaterial_List";
        public const string ListPODetail = "sp_Wms_NGClaimMaterial_ListPODetail";
        public const string ListNGClaimDetail = "sp_Wms_NGClaimMaterial_ListNGClaimDetail";
        public const string Report = "sp_Wms_NGClaimMaterial_Report";

        public const string DDL = "sp_Wms_NGClaimMaterial_DDL";
    }

    public static class NG
    {
        public const string Create = "sp_Wms_NG_Create";
        public const string Update = "sp_Wms_NG_Update";
        public const string Delete = "sp_Wms_NG_Delete";

        public const string Capture = "sp_Wms_NG_Capture";
        public const string GetDetail = "sp_Wms_NG_GetDetail";
        public const string List = "sp_Wms_NG_List";

        public const string DDL = "sp_Wms_NG_DDL";
    }

    public static class Notification
    {
        public const string CountUnread = "sp_Wms_Notifications_CountUnread";
        public const string Get = "sp_Wms_Notifications_Get";
        public const string SaveToAll = "sp_Wms_Notifications_SaveToAll";
        public const string SaveToUser = "sp_Wms_Notifications_SaveToUser";
        public const string UpdateSeen = "sp_Wms_Notifications_UpdateSeen";
    }

    public static class OrderEntry
    {
        public const string DDL = "sp_Wms_OrderEntry_Filter_DDL";
    }

    public static class PartMaterialRequestBom
    {
        public const string Capture = "sp_Wms_PartMaterialRequestBom_Capture";
        public const string CaptureRequest = "sp_Wms_PartMaterialRequestBom_CaptureRequest";

        public const string GetListStock = "sp_Wms_PartMaterialRequestBom_GetListStock";
        public const string GetListDetail = "sp_Wms_PartMaterialRequestBom_GetListDetail";
        public const string GetListHeader = "sp_Wms_PartMaterialRequestBom_GetListHeader";

        public const string Delete = "sp_Wms_PartMaterialRequestBom_Delete";
        public const string Save = "sp_Wms_PartMaterialRequestBom_Save";
    }

    public static class PartMaterialRequestWomin
    {
        public const string Capture = "sp_Wms_PartMaterialRequestWomin_Capture";
        public const string CaptureRequest = "sp_Wms_PartMaterialRequestWomin_CaptureRequest";

        public const string GetListStock = "sp_Wms_PartMaterialRequestWomin_GetListStock";
        public const string GetListDetail = "sp_Wms_PartMaterialRequestWomin_GetListDetail";
        public const string GetListHeader = "sp_Wms_PartMaterialRequestWomin_GetListHeader";

        public const string Delete = "sp_Wms_PartMaterialRequestWomin_Delete";
        public const string Save = "sp_Wms_PartMaterialRequestWomin_Save";
    }

    public static class PeriodSetting
    {
        public const string Capture = "sp_Wms_PeriodSetting_Capture";
        public const string ListDetail = "sp_Wms_PeriodSetting_ListDetail";
        public const string InsertUpdate = "sp_Wms_PeriodSetting_insupd";
    }

    public static class PhysicalInventory
    {
        public const string Capture = "sp_Wms_PhysicalInventory_Capture";
        public const string ListDetail = "sp_Wms_PhysicalInventory_ListDetail";
        public const string Update = "sp_Wms_PhysicalInventory_Update";
    }

    public static class UserSetup
    {
        public const string Factory = "sp_Wms_UserSetup_UserPrivilegeFactory";
        public const string Area = "sp_Wms_UserSetup_UserPrivilegeAddress";
        public const string Address = "sp_Wms_UserSetup_UserPrivilegeArea";
    }

    public static class UserPrivileges
    {
        public const string FactoryUpdate = "sp_Wms_UserSetup_UserPrivilegeFactoryUpd";
        public const string WarehouseUpdate = "sp_Wms_UserSetup_UserPrivilegeWarehouseUpd";
        public const string AreaUpdate = "sp_Wms_UserSetup_UserPrivilegeAreaUpd";
        public const string MobileUpdate = "sp_Wms_UserSetup_UserPrivilegeMobileUpd";

        public const string Update = "sp_Wms_UserSetup_UserPrivilegeUpd";
        public const string Delete = "sp_Wms_UserSetup_UserPrivilegeDelete";


        public const string Factory = "sp_Wms_UserSetup_UserPrivilegeFactory";
        public const string Area = "sp_Wms_UserSetup_UserPrivilegeArea";
        public const string Address = "sp_Wms_UserSetup_UserPrivilegeAddress";
    }

}
