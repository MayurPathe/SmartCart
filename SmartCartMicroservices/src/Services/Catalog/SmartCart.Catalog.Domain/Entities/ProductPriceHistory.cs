using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Domain.Entities;

public class ProductPriceHistory
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public decimal OldPrice { get; set; }

    public decimal NewPrice { get; set; }

    public DateTime ChangedAt { get; set; }

    public Product? Product { get; set; }
}
