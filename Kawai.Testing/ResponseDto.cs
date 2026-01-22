using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Testing;

public class ResponseDto<T>
{
    public int Page { get; set; }
    public int Length { get; set; }
    public int Filtered { get; set; }
    public List<T>? Items { get; set; }
}
