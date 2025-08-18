using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Domain.Interfaces;

public interface IStockRepository
{
    Task<List<StockDto>> InquiryByItem(RequestParameter parameter);
    Task<List<StockDto>> InquiryByArea(RequestParameter parameter);
    Task<List<StockDto>> InquiryDetail(RequestParameter parameter);
    Task<List<StockDto>> DDLLotNo(string keyword, string item);
    Task<List<StockDto>> DDLLotNoByStock(string keyword, string warehouse, string area, string address, string item);

}
