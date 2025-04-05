using System;
namespace Pract1
{
    public interface IDiscount
    {
        string Description { get; }
        decimal ApplyDiscount(Order order, decimal totalCost);
    }
}

