using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.model;

public class LineItem
{
    public decimal amount { get; set; }
    public int sequence { get; set; }
    public string glAccountCode { get; set; }
}
