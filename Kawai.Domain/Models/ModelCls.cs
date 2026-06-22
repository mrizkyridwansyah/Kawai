using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Api.Models;

public class ModelCls
{
   
    public string Model_Cls { get; set; }

    [Required(ErrorMessage = "Description tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Description tidak boleh lebih dari 25 karakter")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Cycle Time tidak boleh kosong")]
    public decimal? CycleTime { get; set; }

    public IFormFile ImageAttachment { get; set; }

    public string ImageName { get; set; }

  
}

 
