using System;
using System.Collections.Generic;
using System.Linq;

namespace Pract1
{
    public enum OrderStatus
    {
        Draft,
        Processing,
        Delivering,
        Delivered
    }
    public class Order
    {
        public int Number { get; set; }
        public DateTime CreationDate { get; set; }
        public string Address { get; set; }
        public bool ExpressDelivery { get; set; }
        public List<OrderLine> OrderLines { get; set; }
        public Client Client { get; set; }
        public OrderStatus Status { get; private set; }
        public DateTime? ConfirmationDate { get; private set; }
        public DateTime? DeliveryStartDate { get; private set; }
        public DateTime? DeliveryEndDate { get; private set; }

        public Order(int number, DateTime creationDate, string address, bool expressDelivery, Client client)
        {
            Number = number;
            CreationDate = creationDate;
            Address = address;
            ExpressDelivery = expressDelivery;
            OrderLines = new List<OrderLine>();
            Client = client;
            Status = OrderStatus.Draft;
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Draft)
            {
                throw new InvalidOperationException("Заказ можно подтвердить только в состоянии 'Формируется'.");
            }

            if (OrderLines.Count == 0)
            {
                throw new InvalidOperationException("Заказ должен содержать хотя бы одну позицию.");
            }

            Status = OrderStatus.Processing;
            ConfirmationDate = DateTime.Now;
        }

        public void StartDelivery()
        {
            if (Status != OrderStatus.Processing)
            {
                Console.WriteLine("Ошибка: Заказ можно передать в доставку только в состоянии 'В обработке'.");
                return;
            }

            Status = OrderStatus.Delivering;
            DeliveryStartDate = DateTime.Now;
            Console.WriteLine("Заказ успешно передан в доставку.");
        }

        public void CompleteDelivery()
        {
            if (Status != OrderStatus.Delivering)
            {
                Console.WriteLine("Ошибка: Заказ можно завершить только в состоянии 'Доставляется'.");
                return;
            }

            Status = OrderStatus.Delivered;
            DeliveryEndDate = DateTime.Now;
            Console.WriteLine("Заказ успешно завершён.");
        }

        public void ChangeAddress(string newAddress)
        {
            if (Status == OrderStatus.Delivering || Status == OrderStatus.Delivered)
            {
                Console.WriteLine("Ошибка: Адрес доставки нельзя изменить после передачи заказа в доставку.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newAddress))
            {
                Console.WriteLine("Ошибка: Адрес доставки не может быть пустым.");
                return;
            }

            Address = newAddress;
            Console.WriteLine("Адрес доставки успешно изменён.");
        }

        public void AddOrderLine(Item item, int quantity)
        {
            if (Status != OrderStatus.Draft)
            {
                throw new InvalidOperationException("Позиции можно добавлять только в состоянии 'Формируется'.");
            }

            if (item == null || quantity <= 0)
            {
                throw new ArgumentException("Некорректные данные для позиции заказа.");
            }

            OrderLines.Add(new OrderLine(item, quantity));
        }

        public void RemoveOrderLine(OrderLine orderLine)
        {
            if (Status != OrderStatus.Draft)
            {
                throw new InvalidOperationException("Позиции можно удалять только в состоянии 'Формируется'.");
            }

            OrderLines.Remove(orderLine);
        }

        public decimal CalculateTotalCost()
        {
            decimal totalCost = OrderLines.Sum(line => line.CalculateCost());
            if (ExpressDelivery)
            {
                totalCost *= 1.25m;
            }
            return totalCost;
        }

        public List<IDiscount> GetApplicableDiscounts(List<IDiscount> discounts)
        {
            List<IDiscount> applicableDiscounts = new List<IDiscount>();
            decimal totalCost = CalculateTotalCost();
            foreach (var discount in discounts)
            {
                decimal discountedCost = discount.ApplyDiscount(this, totalCost);
                if (discountedCost < totalCost)
                {
                    applicableDiscounts.Add(discount);
                }
            }
            return applicableDiscounts;
        }

        public decimal ApplySelectedDiscount(IDiscount selectedDiscount)
        {
            decimal totalCost = CalculateTotalCost();
            return selectedDiscount.ApplyDiscount(this, totalCost);
        }
    }

    public class OrderLine
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }

        public OrderLine(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public decimal CalculateCost()
        {
            return Item.UnitPrice * Quantity;
        }
    }
}
