using Kawai.Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Manufactureline
{
       public string FactoryCode { get; set; }

    public string LineCode { get; set; }

      public string LineName { get; set; }

      public string IPPrinter { get; set; }

     
}

 